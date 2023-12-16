using System;
using System.Collections.Generic;
using System.Linq;

namespace JahanJooy.Common.Util.Validation
{
	public class ValidatedResult<T> : ValidationResult
	{
		public T Result { get; set; }

		public static new ValidatedResult<T> Success(T result)
		{
			return new ValidatedResult<T> { Errors = null, Result = result};
		}

        public static ValidatedResult<T> Failure(IEnumerable<ValidationError> errors)
        {
            if (errors == null)
                throw new ArgumentNullException("errors");

            var errorList = errors.ToList();
            return new ValidatedResult<T> { Errors = errorList };
        }

        public static new ValidatedResult<T> Failure(ValidationError error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            return new ValidatedResult<T> { Errors = new List<ValidationError> { error } };
        }

        public static new ValidatedResult<T> Failure(string errorKey)
        {
            return Failure(new ValidationError(errorKey));
        }

        public static new ValidatedResult<T> Failure(string errorKey, IEnumerable<object> errorParameters)
        {
            return Failure(new ValidationError(errorKey, errorParameters));
        }

        public static new ValidatedResult<T> Failure(string propertyPath, string errorKey)
        {
            return Failure(new ValidationError(propertyPath, errorKey));
        }

        public static new ValidatedResult<T> Failure(string propertyPath, string errorKey, IEnumerable<object> errorParameters)
        {
            return Failure(new ValidationError(propertyPath, errorKey, errorParameters));
        }


	}
}