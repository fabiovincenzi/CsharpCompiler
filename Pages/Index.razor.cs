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
        public IndexBase()
        {
            CsCode = @"using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello World!"");
        }
    }
}
";
        }

        public async Task Run()
        {
            Console = await service.CompileAndRun(CsCode);
        }
    }
}
