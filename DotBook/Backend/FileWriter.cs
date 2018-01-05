using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotBook.Model;
using DotBook.Processing;
using static DotBook.Program;
using static DotBook.Logger;

namespace DotBook.Backend
{
    public class FileWriter : IWriter
    {
        private IFormatter<string> formatter;
        private string extension;
        private string baseFolder;

        public FileWriter(IFormatter<string> formatter, string extension, string baseFolder) =>
            (this.formatter, this.extension, this.baseFolder) =
            (formatter, extension, baseFolder);

        public void Write(Entity entity, IEnumerable<Modifier> visibilities) =>
            Write(entity, visibilities, baseFolder);

        private void Write(Entity entity, IEnumerable<Modifier> visibilities,
            string subfolder)
        {
            visibilities = visibilities.Intersect(Modifiers.Visibility);
            var link = entity.IsRoot() ? "index" : entity.Link;

            var path = Path.Combine(subfolder, link + extension);
            var contents = formatter.Process(entity, visibilities);
            if (!Directory.Exists(subfolder))
                Directory.CreateDirectory(subfolder);
            File.WriteAllText(path, contents);

            if (entity.ChildrenNodes != null)
                foreach (var child in entity.ChildrenNodes)
                    Write(child.NodeValue, visibilities);
        }

        public FileWriter IncludePreface(Entity entity, 
            string filename, ApplicationArguments options)
        {
            var path = Path.Combine(options.InputDirectory, filename + extension);
            if (File.Exists(path))
            {
                var prefaceContents = File.ReadAllText(path);
                Success($"Found {filename + extension}, including it in the index");
                (entity.GetRoot() as Entity).Base.Documentation = prefaceContents;
            } else Info($"No {filename + extension} found");
            return this;
        }
    }
}