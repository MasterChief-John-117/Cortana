using System.Linq;

namespace UrbanDictionary
{
    public class UrbanClient
    {
        public UrbanClient()
        {
            
        }

        public UrbanClientResponse GetClientResponse(string rawWord)
        {
            string word = SanitizeQuery(rawWord);
            return new UrbanClientResponse(word);   
        }

        public string GetFirstDefinition(UrbanClientResponse response)
        {
            if (response.ResultType.Equals("no_results"))
            {
                return "No definition found";
            }
            else return response.List.First().Definition;
        }
        
        private string SanitizeQuery(string initial)
        {
            string noSpecial = new string(initial.ToCharArray().Where(c => char.IsLetter(c) || c == ' ').ToArray());
            return noSpecial.Replace(' ', '+');
        }
    }
}