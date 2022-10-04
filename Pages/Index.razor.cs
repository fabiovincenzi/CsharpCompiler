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
        public string ResultText { get; set; }
        public string CompileText { get; set; }
        [Inject]
        protected CompileService service { get; set; }

        public async Task Run()
        {
            try
            {
                service.CompileLog = new List<string>();
                ResultText = await service.CompileAndRun(CsCode);
            }
            catch (Exception e)
            {
                service.CompileLog.Add(e.Message);
                service.CompileLog.Add(e.StackTrace);
                throw;
            }
            finally
            {
                CompileText = string.Join("\r\n", service.CompileLog);
                this.StateHasChanged();
            }
        }
    }
}
