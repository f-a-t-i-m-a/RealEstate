using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using log4net.Config;

namespace JahanJooy.Common.Util.Log4Net
{
	public class Log4NetConfiguration
	{
		public static string ConfigureFromFiles(IEnumerable<FileInfo> fileInfos)
		{
			var combinedDocument = new XmlDocument();
			var combinedElement = combinedDocument.CreateElement("log4net");

			foreach (var fileInfo in fileInfos)
			{
				using (var fs = fileInfo.OpenRead())
				{
					var xmlDocument = new XmlDocument();
					using (var xmlReader = XmlReader.Create(fs))
					{
						xmlDocument.Load(xmlReader);
					}

					if (xmlDocument.DocumentElement == null)
						throw new ArgumentException("The configuration file " + fileInfo.FullName + " does not have a root element.");

					foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes)
					{
						combinedElement.AppendChild(combinedDocument.ImportNode(childNode, true));
					}
				}
			}

			var errors = XmlConfigurator.Configure(combinedElement);
			if (errors == null || errors.Count == 0)
				return null;

			var result = new StringBuilder();
			foreach (var error in errors)
			{
				result.Append(error).Append(Environment.NewLine);
			}

			return result.ToString();
		}
	}
}