using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace CsharpCompiler
{
    public class CompileService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _uriHelper;
        private List<MetadataReference> references { get; set; }


        public CompileService(HttpClient http, NavigationManager uriHelper)
        {
            _http = http;
            _uriHelper = uriHelper;
        }

        public async Task Init()
        {
            if (references == null)
            {
                var response = await _http.GetFromJsonAsync<BlazorBoot>("_framework/blazor.boot.json");
                var assemblies = await Task.WhenAll(response.resources.assembly.Keys.Select(x => _http.GetAsync("_framework/_bin/" + x)));

                references = new List<MetadataReference>(assemblies.Length);
                foreach (var asm in assemblies)
                {
                    using (var task = await asm.Content.ReadAsStreamAsync())
                    {
                        references.Add(MetadataReference.CreateFromStream(task));
                    }
                }
            }
        }

        public async Task<Assembly> Compile(string code)
        {
            await Init();
            CSharpCompilation compilation = CSharpCompilation.Create("DynamicCode")
                .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication))
                .AddReferences(references)
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Preview)));
            ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics();

            bool error = false;
            foreach (Diagnostic diag in diagnostics)
            {
                switch (diag.Severity)
                {
                    case DiagnosticSeverity.Info:
                        Console.WriteLine(diag.ToString());
                        break;
                    case DiagnosticSeverity.Warning:
                        Console.WriteLine(diag.ToString());
                        break;
                    case DiagnosticSeverity.Error:
                        error = true;
                        Console.WriteLine(diag.ToString());
                        break;
                }
            }
            if (error)
            {
                return null;
            }

            using (var outputAssembly = new MemoryStream())
            {
                compilation.Emit(outputAssembly);

                return  Assembly.Load(outputAssembly.ToArray());
            }
        }

        public async Task<string> CompileAndRun(string code)
        {
            await Init();


            string output = "";
            Console.WriteLine("Compiling and Running code");
                var sw = Stopwatch.StartNew();

                var currentOut = Console.Out;
                var writer = new StringWriter();
                Console.SetOut(writer);

                Exception exception = null;
                try
                {
                var assembly = await this.Compile(code);
                if (assembly != null)
                    {
                        var entry = assembly.EntryPoint;
                        if (entry.Name == "<Main>") // sync wrapper over async Task Main
                        {
                            entry = entry.DeclaringType.GetMethod("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); // reflect for the async Task Main
                        }
                        var hasArgs = entry.GetParameters().Length > 0;
                        var result = entry.Invoke(null, hasArgs ? new object[] { new string[0] } : null);
                        if (result is Task t)
                        {
                            await t;
                        }
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                output = writer.ToString();
                if (exception != null)
                {
                    output += "\r\n" + exception.ToString();
                }
                Console.SetOut(currentOut);

                sw.Stop();
                Console.WriteLine("Done in " + sw.ElapsedMilliseconds + "ms");

            return output;
        }
    }
}
