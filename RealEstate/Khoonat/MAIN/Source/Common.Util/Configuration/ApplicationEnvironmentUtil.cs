using System;
using System.Configuration;
using System.Web;
using System.Web.WebPages;

namespace JahanJooy.Common.Util.Configuration
{
	public static class ApplicationEnvironmentUtil
	{
		public static ApplicationEnvironmentType Type
		{
			get
			{
				ApplicationEnvironmentType result;
				if (Enum.TryParse(ConfigurationManager.AppSettings["Environment"], true, out result))
					return result;

				return ApplicationEnvironmentType.Development;
			}
		}

        public static IHtmlString DevelopmentOnly(Func<dynamic, HelperResult> block)
        {
            if (Type == ApplicationEnvironmentType.Development)
            {
                return block(null);
            }

            return new HtmlString("");
        }

	}
}