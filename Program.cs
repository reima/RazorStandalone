using System;
using Microsoft.AspNetCore.Razor.Language;

namespace RazorStandalone
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var razorEngine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create("Views"));

            var item = razorEngine.FileSystem.GetItem("Test.cshtml");
            var cSharpDocument = razorEngine.Process(item).GetCSharpDocument();

            Console.WriteLine(cSharpDocument.GeneratedCode);
        }
    }
}
