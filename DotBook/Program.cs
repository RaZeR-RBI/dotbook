﻿using Fclp;
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

namespace DotBook
{
    public class Program
    {
        public class ApplicationArguments
        {
            public string InputDirectory { get; set; }
            public string OutputDirectory { get; set; }
            public List<Modifier> Visibility { get; set; }
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
            var p = new FluentCommandLineParser<ApplicationArguments>();
            p.UseOwnOptionPrefix("-", "--");
            p.Setup(arg => arg.OutputDirectory)
                .As('o', "output")
                .SetDefault("doc")
                .WithDescription("Output directory for the generated documentation." +
                                 "If not specified, defaults to 'doc'.");

            p.Setup(arg => arg.InputDirectory)
                .As('s', "src")
                .WithDescription("Directory for C# code search");

            p.Setup(arg => arg.Visibility)
                .As('v', "visibility")
                .SetDefault(new List<Modifier>() { Modifier.Public })
                .WithDescription("Include types and members with the specified " +
                                 "visibilities. Defaults to 'public'.");

            p.Setup(arg => arg.UseHashAsLink)
                .As('h', "use-hash")
                .SetDefault(false)
                .WithDescription("Use hashing for documentation filenames to " +
                                 "allow deep hierarchies." +
                                 "If false, uses escaped type/member name." +
                                 "Defaults to 'false'.");
            p.Setup(arg => arg.Format)
                .As('f', "format")
                .SetDefault(FileFormat.Markdown)
                .WithDescription("Sets the output format. Default is Markdown." +
                                 "Available formats: Markdown, Html");

            p.SetupHelp("?", "help")
                .Callback(text =>
                {
                    Console.WriteLine(text);
                    Environment.Exit(0);
                });

            p.Parse(args);

            try
            {
                Info("Using the following parameters:");
                Log($"{p.Object}");
                Run(p.Object);
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
            var input = options.InputDirectory ?? Directory.GetCurrentDirectory();
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
                .Write(entities, new[] { Modifier.Public });

            Success($"Generated {entities.Descendants().Count()} documentation files");
        }
    }
}
