using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DotBook.Backend
{
    public class AsciiDocBackend : IBackend<string, XmlNode>
    {
        public string Process(XmlNode node)
        {
            // TODO: Do some pattern matching and stuff
            throw new NotImplementedException();
        }
    }
}
