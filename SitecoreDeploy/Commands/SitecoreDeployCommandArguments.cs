using System.Collections.Generic;
using System.Linq;

namespace SitecoreDeploy.Commands
{
    public class SitecoreDeployCommandArguments
    {
        public SitecoreDeployCommandArguments(IDictionary<string, string> context)
        {
            Context = context ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Get the value from the context or null if not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get => Context.TryGetValue(key, out var result) ? result : null;
            set => Context[key] = value;
        }

        public override string ToString()
        {
            return Context.Aggregate("", (s, pair) =>
            {
                s += $"{pair.Key}={pair.Value}&";
                return s;
            });
        }

        public string Command => Context["command"];

        public IDictionary<string, string> Context { get; }

        /// <summary>
        /// Result can be used to return a message to the caller
        /// </summary>
        public string Result {
            get => Context["__result"];
            set => Context["__result"] = value;
        } 
    }
}