using System.Collections.Generic;

namespace JahanJooy.Common.Util.Web
{
	public class RadioButtonItems<T>
	{
		public string Id { get; set; }
		private T _selectedValue;
		private List<RadioButtonItem<T>> _listItems;

		public T SelectedValue
		{
			get { return _selectedValue; }
			set
			{
				_selectedValue = value;
				UpdatedSelectedItems();
			}
		}

		public List<RadioButtonItem<T>> ListItems
		{
			get { return _listItems; }
			set
			{
				_listItems = value;
				UpdatedSelectedItems();
			}
		}

		private void UpdatedSelectedItems()
		{
			if (ListItems == null)
				return;

			ListItems.ForEach(li => li.Selected = Equals(li.Value, SelectedValue));
		}
	}

	public class RadioButtonItem<T>
	{
		public bool Selected { get; set; }

		public string Text { get; set; }

		public T Value { get; set; }

		public override string ToString()
		{
			return Value.ToString();
		}
	}

}