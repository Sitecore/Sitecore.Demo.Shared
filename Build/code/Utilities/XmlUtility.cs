using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Sitecore.Demo.BuildTools.Utilities
{
    public class XmlUtility
    {
        internal static XmlDocument ParseXmlDocument(string xml)
        {
            var doc = new XmlDocument();
            doc.Load(xml);
            return doc;
        }

        internal static XmlDocument Merge(IEnumerable<XmlDocument> transforms)
        {
            return transforms.Aggregate(CreateEmptyXmlTransform(), MergeXmlDocument);
        }

        internal static XmlDocument MergeXmlDocument(XmlDocument target, XmlDocument source)
        {
            var targetElement = target.DocumentElement;
            var elements = source.DocumentElement.ChildNodes;

            foreach (XmlNode element in elements)
            {
                ImportChild(targetElement, element);
            }

            return target;
        }

        internal static void ImportChild(XmlElement target, XmlNode node)
        {
            var importedNode = target.OwnerDocument.ImportNode(node, true);

            target.AppendChild(importedNode);
        }

        internal static XmlDocument CreateEmptyXmlTransform()
        {
            var doc = new XmlDocument();

            var nsm = new XmlNamespaceManager(doc.NameTable);
            nsm.AddNamespace("xdt", "http://schemas.microsoft.com/XML-Document-Transform");

            doc.AppendChild(doc.CreateElement("configuration"));

            return doc;
        }
    }
}
