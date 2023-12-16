using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace JahanJooy.Common.Util.Web
{
	public class DisposableOutput : IDisposable
	{
        private bool _disposed; 
        private readonly TextWriter _writer;
		private readonly string _endHtml;

		public DisposableOutput(ViewContext viewContext, string startHtml, string endHtml)
		{
            if (viewContext == null) 
                throw new ArgumentNullException("viewContext"); 
 
            _writer = viewContext.Writer;
			_endHtml = endHtml;

 			if (!string.IsNullOrEmpty(startHtml))
				_writer.Write(startHtml);
        } 

        public void Dispose() 
		{ 
            Dispose(true); 
            GC.SuppressFinalize(this);
        } 

        protected virtual void Dispose(bool disposing) 
		{
        	if (_disposed) 
				return;

        	_disposed = true;
			if (!string.IsNullOrEmpty(_endHtml))
        		_writer.Write(_endHtml);
		} 

        public void EndOutput() 
		{ 
            Dispose(true); 
        }
	}
}