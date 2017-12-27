using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotBook.Model;
using DotBook.Processing;

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
            if (entity.Base != null)
            {
                var path = Path.Combine(subfolder, entity.Link + extension);
                var contents = formatter.Process(entity, visibilities);
                if (!Directory.Exists(subfolder))
                    Directory.CreateDirectory(subfolder);
                File.WriteAllText(path, contents);
            }

            if (entity.ChildrenNodes != null)
                foreach (var child in entity.ChildrenNodes)
                    Write(child as Entity, visibilities);
        }
    }
}