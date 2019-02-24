using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueprintMgmt
{
    public class Arguments
    {
        public string Command { get; set; }
        public IDictionary<string, string> Parameters { get; private set; } 

        public Arguments()
        {
            Command = "";
            Parameters = new Dictionary<string, string>();
        }
    }
}
