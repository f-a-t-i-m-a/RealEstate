using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.ShishDong.Server.Formatters
{
	public class ServiceStackTextFormatter : MediaTypeFormatter
	{
		public ServiceStackTextFormatter()
		{
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
			SupportedEncodings.Add(new UTF8Encoding(false, true));
		}

		public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
		{
			return Task.Factory.StartNew(() => JsonSerializer.DeserializeFromStream(type, stream));
		}

		public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
		{
			return Task.Factory.StartNew(() => JsonSerializer.SerializeToStream(value, type, stream));
		}

		public override bool CanReadType(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			
			return true;
		}

		public override bool CanWriteType(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			
			return true;
		}
	}
}