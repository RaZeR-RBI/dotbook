using System;
using System.Collections.Generic;
using System.Linq;

namespace DotBook.Utils
{
    public class ArgParser
    {
        public class Argument
        {
            public string ShortName { get; internal set; }
            public string LongName { get; internal set; }
            public string Description { get; internal set; }
            public Action<IEnumerable<string>> Handler { get; internal set; }
            public IEnumerable<string> Default { get; internal set; }

            internal Argument() { }
            internal Argument(Action<IEnumerable<string>> callback) =>
                Handler = callback;

            public override string ToString() =>
                $"{ShortName}, {LongName}\t\t{Description}";
        }

        private List<Argument> declaredArgs = new List<Argument>();
        private List<string> optionPrefixes = new List<string>() { "-", "--" };

        public void UseOwnOptionPrefix(params string[] prefixes) =>
            optionPrefixes = new List<string>(prefixes);

        public void Parse(string[] args)
        {
            var options = new Dictionary<Argument, List<string>>();
            Argument currentArg = null;

            foreach (var arg in args)
            {
                if (arg.StartsWithAny(optionPrefixes))
                {
                    var argName = arg.RemovePrefix(optionPrefixes);
                    currentArg = GetByName(argName);
                    if (!options.ContainsKey(currentArg))
                        options.Add(currentArg, new List<string>());
                }
                else
                {
                    if (currentArg == null) continue;
                    options[currentArg].Add(arg);
                }
            }

            var notDefinedOptions = declaredArgs
                .Where(p => !options.Keys.Contains(p) && p.Default != null);

            foreach(var notDefined in notDefinedOptions)
                options.Add(notDefined, notDefined.Default?.ToList() ?? new List<string>());

            foreach (var pair in options)
                try
                {
                    pair.Key.Handler(pair.Value);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to parse option '{pair.Key.LongName}'", ex);
                }
        }

        private Argument GetByName(string name) =>
            declaredArgs.FirstOrDefault(a => a.ShortName == name || a.LongName == name);

        public IEnumerable<string> GetHelp()
        {
            yield return "Option format: " +
                string.Join("option, ", optionPrefixes) + "option";
            foreach(var arg in declaredArgs)
                yield return $"{arg}";
        }

        public Argument Setup(Action<IEnumerable<string>> callback)
        {
            var result = new Argument(callback);
            declaredArgs.Add(result);
            return result;
        }

        public Argument SetupHelp(string shortName, string longName)
        {
            var result = new Argument()
                .As(shortName, longName)
                .WithDescription("Displays the help message.");
            declaredArgs.Add(result);
            return result;
        }
    }

    public static class ArgParserExtensions
    {
        public static ArgParser.Argument As(this ArgParser.Argument argument,
            string shortName, string longName)
        {
            argument.ShortName = shortName;
            argument.LongName = longName;
            return argument;
        }

        public static ArgParser.Argument As(this ArgParser.Argument argument,
            char shortName, string longName) =>
            argument.As($"{shortName}", longName);

        public static ArgParser.Argument SetDefault(this ArgParser.Argument argument,
            params string[] defaultValue)
        {
            argument.Default = defaultValue;
            return argument;
        }

        public static ArgParser.Argument WithDescription(this ArgParser.Argument argument,
            string description)
        {
            argument.Description = description;
            return argument;
        }

        public static ArgParser.Argument Callback(this ArgParser.Argument argument,
            Action<IEnumerable<string>> callback)
        {
            argument.Handler = callback;
            return argument;
        }

        public static bool StartsWithAny(this string str, IEnumerable<string> prefixes)
        {
            foreach (var prefix in prefixes)
                if (str.StartsWith(prefix)) return true;
            return false;
        }

        public static string RemovePrefix(this string str, IEnumerable<string> prefixes)
        {
            foreach (var prefix in prefixes)
                if (str.StartsWith(prefix)) return str.Substring(prefix.Length);
            return str;
        }
    }
}