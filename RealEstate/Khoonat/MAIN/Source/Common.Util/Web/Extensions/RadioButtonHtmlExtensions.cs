using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace JahanJooy.Common.Util.Web.Extensions
{
	public static class RadioButtonHtmlExtensions
	{
		public static IEnumerable<RadioButtonListItem> RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
		{
			return from selectListItem in selectList
			       let idPrefix = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression))
			       let itemHtmlId = idPrefix + "_" + selectListItem.Value
			       select new RadioButtonListItem {
			       	Input = htmlHelper.RadioButtonFor(expression, selectListItem.Value, new {@id = itemHtmlId}),
			       	Label = htmlHelper.Label(itemHtmlId, selectListItem.Text)
			       };
		}
	}

	public class RadioButtonListItem
	{
		public MvcHtmlString Input { get; set; }
		public MvcHtmlString Label { get; set; }
	}
}