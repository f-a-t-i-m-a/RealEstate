using System.Collections.Generic;

namespace JahanJooy.Common.Util.Validation
{
	public class ValidationError
	{
		#region Initialization

		public ValidationError()
		{
		}

		public ValidationError(string errorKey)
		{
			ErrorKey = errorKey;
		}

		public ValidationError(string propertyPath, string errorKey)
		{
			PropertyPath = propertyPath;
			ErrorKey = errorKey;
		}

		public ValidationError(string errorKey, IEnumerable<object> errorParameters)
		{
			ErrorKey = errorKey;
			ErrorParameters = errorParameters;
		}

		public ValidationError(string propertyPath, string errorKey, IEnumerable<object> errorParameters)
		{
			PropertyPath = propertyPath;
			ErrorKey = errorKey;
			ErrorParameters = errorParameters;
		}

		#endregion

		#region Properties

		public string PropertyPath { get; set; }
		public string ErrorKey { get; set; }
		public IEnumerable<object> ErrorParameters { get; set; }

		public string FullResourceKey
		{
			get { return string.IsNullOrWhiteSpace(PropertyPath) ? ErrorKey : PropertyPath.Replace('.', '_') + "_" + ErrorKey; }
		}

		#endregion
	}
}