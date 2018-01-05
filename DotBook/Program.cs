using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Console = Colorful.Console;
using static DotBook.Logger;
using DotBook.Model;
using DotBook.Processing;
using System.Collections.Generic;
using DotBook.Backend;
using static System.Environment;
using DotBook.Utils;

namespace DotBook
{
    public class Program
    {
        public class ApplicationArguments
        {
            public string InputDirectory { get; set; }
            public string OutputDirectory { get; set; }
            public IEnumerable<Modifier> Visibility { get; set; }
            public bool UseHashAsLink { get; set; }
            public FileFormat Format { get; set; }

            public override string ToString() =>
                $"Source directory: {InputDirectory ?? "(current)"}\n" +
                $"Output directory: {OutputDirectory}\n" +
                $"Format: {Format}\n" +
                $"Visibility filter: {string.Join(",", Visibility)}\n" +
                $"Using hashes as links: {UseHashAsLink}\n";
        }

        /// <summary>
        /// The DotBook entry point.
        /// </summary>
        /// <param name="args">Command line args</param>
        public static void Main(string[] args)
        {
            var p = new ArgParser();
            var arg = new ApplicationArguments();
            p.UseOwnOptionPrefix("-", "--");
            p.Setup(v => arg.OutputDirectory = string.Join(" ", v))
                .As('o', "output")
                .SetDefault("docs")
                .WithDescription("Output directory for the generated documentation. " +
                                 "If not specified, defaults to 'docs'.");

            p.Setup(v => arg.InputDirectory = string.Join(" ", v))
                .As('s', "src")
                .WithDescription("Directory for C# code search");

            p.Setup(v => arg.Visibility = v.ToEnum<Modifier>())
                .As('v', "visibility")
                .SetDefault("public")
                .WithDescription("Include types and members with the specified " +
                                 "visibilities. Defaults to 'public'.");

            p.Setup(v => bool.Parse(v.FirstOrDefault() ?? "true"))
                .As('h', "use-hash")
                .SetDefault("false")
                .WithDescription("Use hashing for documentation filenames to " +
                                 "allow deep hierarchies. " +
                                 "If false, uses escaped type/member name. " +
                                 "Defaults to 'false'.");

            p.Setup(v => arg.Format = v.ToEnum<FileFormat>().First())
                .As('f', "format")
                .SetDefault("Markdown")
                .WithDescription("Sets the output format. Default is Markdown. " +
                                 "Available formats: Markdown, Html");

            p.SetupHelp("?", "help")
                .Callback(v =>
                {
                    Console.WriteLine(string.Join("\n", p.GetHelp()));
                    Environment.Exit(0);
                });

            p.Parse(args);

            try
            {
                Info("Using the following parameters:");
                Log($"{arg}");
                Run(arg);
            }
            catch (PathTooLongException pex)
            {
                Fatal("Hierarchy is too deep to use type/member names " +
                      "as filenames. Try using the --use-hash flag." +
                      $"{pex}");
            }
        }

        private static void Run(ApplicationArguments options)
        {
            if (options.InputDirectory == null)
                options.InputDirectory = Directory.GetCurrentDirectory();
            var input = options.InputDirectory;
            var output = options.OutputDirectory;
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);

            if (!Directory.Exists(input))
                Fatal("The specified directory does not exist");

            Info($"Loading code from '{input}'");
            var nodes = CompilationUnits.FromFolder(input);
            if (!nodes.Any())
                Fatal($"No source files found in {input}");

            var sourceInfo = new SourceInfo(nodes.ToList());

            Info("Creating documentation tree");
            var entities = sourceInfo.WithVisibility(modifiers: Modifier.Public);
            File.WriteAllText(Path.Combine(output, "structure.json"), entities.AsJson());

            // TODO: Add output format selection
            var format = options.Format;
            Info("Writing documentation files");
            format.BeginWritingAt(output)
                .IncludePreface(entities, "README", options)
                .Write(entities, new[] { Modifier.Public });

            Success($"Generated {entities.Descendants().Count()} documentation files");
        }
    }
}
