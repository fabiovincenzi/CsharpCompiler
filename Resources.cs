using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsharpCompiler
{
    public class Resources
    {
        public Dictionary<string, string> assembly { get; set; }
        public Dictionary<string, string> pdb { get; set; }
        public Dictionary<string, string> runtime { get; set; }
    }
}
