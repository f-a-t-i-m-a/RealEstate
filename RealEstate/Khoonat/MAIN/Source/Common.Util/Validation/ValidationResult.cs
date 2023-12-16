using System;
using System.Collections.Generic;

namespace JahanJooy.Common.Util.Validation
{
	public class ValidationResult
	{
		public List<ValidationError> Errors { get; set; }

		public bool IsValid
		{
			get { return Errors == null || Errors.Count < 1; }
        }

        #region Public helper methods for construction

        public static readonly ValidationResult Success = new ValidationResult { Errors = null }; 

        public static ValidationResult Failure(ValidationError error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            return new ValidationResult {Errors = new List<ValidationError> {error}};
        }

        public static ValidationResult Failure(string errorKey)
        {
            return Failure(new ValidationError(errorKey));
        }

        public static ValidationResult Failure(string errorKey, IEnumerable<object> errorParameters)
        {
            return Failure(new ValidationError(errorKey, errorParameters));
        }

        public static ValidationResult Failure(string propertyPath, string errorKey)
        {
            return Failure(new ValidationError(propertyPath, errorKey));
        }

        public static ValidationResult Failure(string propertyPath, string errorKey, IEnumerable<object> errorParameters)
        {
            return Failure(new ValidationError(propertyPath, errorKey, errorParameters));
        }

        public ValidationResult Append(ValidationError error)
        {
            Errors.Add(error);
            return this;
        }

        public ValidationResult Append(string errorKey)
        {
            return Append(new ValidationError(errorKey));
        }

        public ValidationResult Append(string errorKey, IEnumerable<object> errorParameters)
        {
            return Append(new ValidationError(errorKey, errorParameters));
        }

        public ValidationResult Append(string propertyPath, string errorKey)
        {
            return Append(new ValidationError(propertyPath, errorKey));
        }

        public ValidationResult Append(string propertyPath, string errorKey, IEnumerable<object> errorParameters)
        {
            return Append(new ValidationError(propertyPath, errorKey, errorParameters));
        }

		public ValidatedResult<T> ToFailedValidatedResult<T>()
		{
			if (IsValid)
				throw new InvalidOperationException(
					"This method should only be called on a ValidationResult instance that contains errors. " +
					"Consider checking the state using IsValid property before calling this method.");

			return ValidatedResult<T>.Failure(Errors);
		}

		#endregion
    }
}