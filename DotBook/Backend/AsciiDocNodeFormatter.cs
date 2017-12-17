using DotBook.Model;
using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DotBook.Backend
{
    public class AsciiDocNodeFormatter : INodeFormatter<string, XmlNode, INameable>
    {
        public string Process(XmlNode documentation, INode<INameable> node)
        {
            var builder = new StringBuilder();

            // TODO: Do some pattern matching and stuff
            switch(documentation)
            {
                case XmlNode doc when doc.NodeType == XmlNodeType.Text:
                    builder.Append(doc.InnerText);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return builder.ToString();
        }
    }
}
