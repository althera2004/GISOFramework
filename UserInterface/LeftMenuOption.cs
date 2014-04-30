// -----------------------------------------------------------------------
// <copyright file="LeftMenuOption.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework.UserInterface
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class LeftMenuOption
    {
        public static string Render(string text, string url, bool current, string icon)
        {
            var actualUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            if (HttpContext.Current.Request.Url.LocalPath != "/")
            {
                string baseUrl = actualUrl.Replace(HttpContext.Current.Request.Url.LocalPath.Substring(1), string.Empty);
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    actualUrl = "/" + actualUrl.Replace(baseUrl, string.Empty);
                }
            }


            current = url.ToUpperInvariant() == actualUrl.ToUpperInvariant();

            string currentText = current ? " class=\"active\"" : string.Empty;
            return string.Format(CultureInfo.GetCultureInfo("es-es"), @"<li {2}>
							<a href=""{1}"">
								<i class=""{3}""></i>
								<span class=""menu-text""> {0} </span>
							</a>
						</li>", text, url, currentText, icon);
        }

        public static string RenderAdmin(Dictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                return string.Empty;
            }

            return string.Format(CultureInfo.GetCultureInfo("es-es"), @"<li>
							<a href=""#"" class=""dropdown-toggle"">
								<i class=""icon-tag""></i>
								<span class=""menu-text""> {0} </span>
								<b class=""arrow icon-angle-down""></b>
							</a>
							<ul class=""submenu"">
								<li>
									<a href=""CompanyProfile.aspx"">
										<i class=""icon-double-angle-right""></i>
										{1}
									</a>
								</li>
								<li>
									<a href=""CargosList.aspx"">
										<i class=""icon-double-angle-right""></i>
										{2}
									</a>
								</li>
								<li>
									<a href=""Employees.aspx"">
										<i class=""icon-double-angle-right""></i>
										{3}
									</a>
								</li>
								<li>
									<a href=""ActivityLogs.html"">
										<i class=""icon-double-angle-right""></i>
										{4}
									</a>
								</li>
							</ul>
						</li>", dictionary["Compañía"], dictionary["Datos de la compañía"], dictionary["Cargos"], dictionary["Empleados"], dictionary["Procesos"]);
        }
    }
}
