using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsharpCompiler
{
    public class BlazorBoot
    {
        public bool cacheBootResources { get; set; }
        public object[] config { get; set; }
        public bool debugBuild { get; set; }
        public string entryAssembly { get; set; }
        public bool linkerEnabled { get; set; }
        public Resources resources { get; set; }
    }
}
