using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DotBook.Processing
{
    public static class XmlExtensions
    {
        public static string AttributeValue(this XmlNode node, string name) =>
            node.Attributes?
                .OfType<XmlAttribute>()
                .FirstOrDefault(n => n.Name == name)?
                .InnerText;
    }
}
