using Fclp;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Console = Colorful.Console;
using static DotBook.Logger;
using DotBook.Model;

namespace DotBook
{
    class Program
    {
        public class ApplicationArguments
        {
            public string InputDirectory { get; set; }
            public string OutputDirectory { get; set; }
        }

        /// <summary>
        /// The DotBook entry point.
        /// </summary>
        /// <param name="args">Command line args</param>
        static void Main(string[] args)
        {
            var p = new FluentCommandLineParser<ApplicationArguments>();
            p.Setup(arg => arg.OutputDirectory)
                .As('o', "output")
                .SetDefault("doc")
                .WithDescription("Output directory for the generated documentation." +
                                 "If not specified, defaults to 'doc'.");

            p.Setup(arg => arg.InputDirectory)
                .As('s', "src")
                .WithDescription("Directory for C# code search");

            p.SetupHelp("?", "help")
                .Callback(text => Console.WriteLine(text));

            p.Parse(args);
            Run(p.Object);
        }

        private static void Run(ApplicationArguments options)
        {
            var input = options.InputDirectory ?? Directory.GetCurrentDirectory();
            var output = options.OutputDirectory;
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);
           
            if (!Directory.Exists(input))
                Fatal("The specified directory does not exist");

            Info($"Loading code from '{input}'");
            var nodes = CompilationUnits.FromFolder(input);
            var sourceInfo = new SourceInfo(nodes.ToList());
        }
    }
}
