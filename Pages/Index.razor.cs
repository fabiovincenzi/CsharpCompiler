using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsharpCompiler.Pages
{
    public class IndexBase : ComponentBase
    {
        public string CsCode { get; set; }
        public string Console { get; set; }
        [Inject]
        protected CompileService service { get; set; }

        public async Task Run()
        {
            Console = await service.CompileAndRun(CsCode);
        }
    }
}
