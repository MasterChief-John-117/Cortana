using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;

namespace Cortana
{
    public class CodeEval
    {
        public async Task<string> CSharp(string code, ICommandContext ctx)
        {
            string result = "";
            try{//Credit for code goes to RheaAyase, modified from Botwinder UserBot


                CodeDomProvider evaluator = CodeDomProvider.CreateProvider("CSharp");
                CompilerParameters parameters = new CompilerParameters();

                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Runtime.dll");
                parameters.ReferencedAssemblies.Add("System.Linq.dll");
                parameters.ReferencedAssemblies.Add("System.Threading.Tasks.dll");
                parameters.ReferencedAssemblies.Add("Discord.Net.Core.dll");
                parameters.ReferencedAssemblies.Add("Discord.Net.Commands.dll");
                parameters.ReferencedAssemblies.Add("Discord.Net.WebSocket.dll");
                parameters.CompilerOptions = "/t:library";
                parameters.GenerateInMemory = true;

                string classCode = "using System;\nusing System.Runtime;\nusing System.Threading.Tasks;\nusing System.Collections;" +
                                   "\nusing System.Collections.Generic;\nusing System.Text.RegularExpressions;\nusing System.Linq;" +
                                   "\nusing Discord;\nusing Discord.Commands;\nusing Discord.WebSocket;" +
                                   "\nnamespace CodeEvaluator{" +
                                   "\npublic class CodeEvaluator{" +
                                   "\nasync public Task<object> EvalCode(ICommandContext ctx){" +
                                   "\ntry{\n" + code + "} catch(Exception exception){\nreturn exception.Message;\n}}\n}\n}\n";
                CompilerResults codeResults = evaluator.CompileAssemblyFromSource(parameters, classCode);

                if (codeResults.Errors.Count > 0)
                {
                    return codeResults.Errors[0].ErrorText;
                }

                if (!string.IsNullOrEmpty(result))
                    return result;
                Assembly assembly = codeResults.CompiledAssembly;
                object instance = assembly.CreateInstance("CodeEvaluator.CodeEvaluator");
                if (instance == null)
                    return (string) null;
                MethodInfo method = instance.GetType().GetMethod("EvalCode");
                object returnedObject = await (Task<object>) method.Invoke(instance, new object[1]
                {
                    (object) ctx
                });
                result = returnedObject == null ? (string) null : returnedObject.ToString();
                evaluator.Dispose();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}