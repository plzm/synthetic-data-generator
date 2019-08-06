using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace pelazem.xmlgen
{
	public class XmlGenerator
	{
		public Stream Generate(XmlSchema schema, Encoding encoding)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = encoding;
			settings.Indent = true;

			XmlProcessor processor = new XmlProcessor(schema, XmlQualifiedName.Empty);

			var stream = new MemoryStream();

			using (var xmlWriter = XmlWriter.Create(stream, settings))
			{
				processor.WriteXml(xmlWriter);

				xmlWriter.Flush();
			}

			return stream;
		}
	}
}
