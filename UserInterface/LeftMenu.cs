// -----------------------------------------------------------------------
// <copyright file="LeftMenu.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework.UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeftMenu
    {
        Dictionary<string, string> dictionary;
        ApplicationUser user;

        public LeftMenu(ApplicationUser user, Dictionary<string, string> dictionary)
        {
            this.dictionary = dictionary;
            this.user = user;
        }

        public string Render()
        {
            StringBuilder res = new StringBuilder(Environment.NewLine);

            if (this.user.Groups.Contains(SecurityGroup.Company))
            {
                res.Append(LeftMenuOption.Render(this.dictionary["Cuadro de mandos"], "/DashBoard.aspx", false, "icon-dashboard"));
            }

            if (this.user.Groups.Contains(SecurityGroup.Company))
            {
                res.Append(LeftMenuOption.RenderAdmin(this.dictionary));
            }

            if (this.user.Groups.Contains(SecurityGroup.Process))
            {
                res.Append(LeftMenuOption.Render(this.dictionary["Procesos"], "/ProcesosList.aspx", false, "icon-list"));
            }

            if (this.user.Groups.Contains(SecurityGroup.Documents))
            {
                res.Append(LeftMenuOption.Render(this.dictionary["Documentos"], "/Documents.aspx", false, "icon-group"));
            }

            if (this.user.Groups.Contains(SecurityGroup.Learning))
            {
                res.Append(LeftMenuOption.Render(this.dictionary["Formación"], "/FormacionList.aspx", false, "icon-edit"));
            }

            if (this.user.Groups.Contains(SecurityGroup.Providers))
            {
                res.Append(LeftMenuOption.Render(this.dictionary["Proveedores"], "#", false, "icon-edit"));
            }

            if (this.user.Groups.Contains(SecurityGroup.Equipos))
            {
                res.Append(LeftMenuOption.Render(this.dictionary["Equipos"], "#", false, "icon-edit"));
            }

            if (this.user.Groups.Contains(SecurityGroup.Incidence))
            {
                res.Append(LeftMenuOption.Render(this.dictionary["Incidencias"], "#", false, "icon-edit"));
            }

            if (this.user.Groups.Contains(SecurityGroup.Audits))
            {
                res.Append(LeftMenuOption.Render(this.dictionary["Auditorías"], "#", false, "icon-edit"));
            }

            if (this.user.Groups.Contains(SecurityGroup.Review))
            {
                res.Append(LeftMenuOption.Render(this.dictionary["Revisión dirección"], "#", false, "icon-edit"));
            }

            res.Append(Environment.NewLine);
            return res.ToString();
        }
    }
}
