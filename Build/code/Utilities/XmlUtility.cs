using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Sitecore.Demo.Shared.BuildTools.Utilities
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
            var doc = transforms.First();
            var rootNodeName = doc.ChildNodes.OfType<XmlElement>().FirstOrDefault().Name;

            return transforms.Aggregate(CreateEmptyXmlTransform(rootNodeName), MergeXmlDocument);
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

        internal static XmlDocument CreateEmptyXmlTransform(string rootNodeName)
        {
            var doc = new XmlDocument();

            var nsm = new XmlNamespaceManager(doc.NameTable);
            nsm.AddNamespace("xdt", "http://schemas.microsoft.com/XML-Document-Transform");

            doc.AppendChild(doc.CreateElement(rootNodeName));

            return doc;
        }
    }
}
