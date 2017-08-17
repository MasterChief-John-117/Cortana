using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cortana
{
    public static class Extensions
    {
        public static async void FireAndForget(this Task task)
        {
            await task.ConfigureAwait(false);
        }
    }
}
