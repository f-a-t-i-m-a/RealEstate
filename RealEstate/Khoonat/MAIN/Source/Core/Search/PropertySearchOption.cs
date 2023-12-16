using System;
using System.Linq.Expressions;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Search
{
	public class PropertySearchOption
	{
		private readonly string _label;
		private readonly string _key;
		private readonly Expression<Func<PropertyListing, bool>> _expression;
		private readonly Func<PropertyListing, bool> _delegate;
		private readonly bool _dependsOnUserAccount;

		public PropertySearchOption(string label, string key, Expression<Func<PropertyListing, bool>> expression, Func<PropertyListing, bool> @delegate = null, bool dependsOnUserAccount = false)
		{
			_label = label;
			_key = key;
			_expression = expression;
			_delegate = @delegate ?? (expression != null ? expression.Compile() : null);
			_dependsOnUserAccount = dependsOnUserAccount;
		}

		public string Label
		{
			get { return _label; }
		}

		public string Key
		{
			get { return _key; }
		}

		public Expression<Func<PropertyListing, bool>> Expression
		{
			get { return _expression; }
		}

		public Func<PropertyListing, bool> Delegate
		{
			get { return _delegate; }
		}

		public bool DependsOnUserAccount
		{
			get { return _dependsOnUserAccount; }
		}
	}
}