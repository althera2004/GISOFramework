// -----------------------------------------------------------------------
// <copyright file="ApplicationUser.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;
    using GisoFramework.Item;
    using GisoFramework.UserInterface;

    public enum UserGrant
    {
        /// <summary>Undefined grant</summary>
        Undefined = 0,

        CompanyData = 1,

        Departments = 2,

        Employees = 3,

        Documents = 4,

        Providers = 5
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ApplicationUser
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Name { get; set; }

        private List<SecurityGroup> groups;

        public string Language { get; set; }

        public LogOnResult Status { get; set; }

        public Employee Employee { get; set; }

        public MenuShortcut MenuShortcuts { get; set; }

        public static ApplicationUser Empty
        {
            get
            {
                return new ApplicationUser()
                {
                    Id = -1,
                    Name = string.Empty,
                    Login = string.Empty
                };
            }
        }

        public ReadOnlyCollection<SecurityGroup> Groups
        {
            get
            {
                return new ReadOnlyCollection<SecurityGroup>(this.groups);
            }
        }

        public string Json
        {
            get
            {
                StringBuilder res = new StringBuilder("{").Append(Environment.NewLine);
                res.Append("\t\t\"Id\":").Append(this.Id).Append(",").Append(Environment.NewLine);
                res.Append("\t\t\"Login\":\"").Append(this.Login).Append("\",").Append(Environment.NewLine);
                res.Append("\t\t\"Language\":\"").Append(this.Language).Append("\",").Append(Environment.NewLine);
                res.Append("\t\t\"Status\":\"").Append(this.Status).Append("\"");
                if (this.Employee != null)
                {
                    res.Append(",").Append(Environment.NewLine).Append("\t\tEmploye:{").Append(Environment.NewLine);
                    res.Append("\t\t\tName:'").Append(this.Employee.Name).Append("',").Append(Environment.NewLine);
                    res.Append("\t\t\tLastName:'").Append(this.Employee.LastName).Append("',").Append(Environment.NewLine);
                    res.Append("\t\t\tSecondLastName:'").Append(this.Employee.SecondLastName).Append("',").Append(Environment.NewLine);
                    res.Append("\t\t}");
                }
                res.Append(Environment.NewLine).Append("}");
                return res.ToString();
            }
        }

        public ApplicationUser()
        {
            this.Id = -1;
            this.Login = string.Empty;
            this.Name = string.Empty;
            this.groups = new List<SecurityGroup>();
        }

        public ApplicationUser(int userId)
        {
            using (SqlCommand cmd = new SqlCommand("User_GetById"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserId", SqlDbType.Int);
                cmd.Parameters["@UserId"].Value = userId;

                this.Id = -1;
                this.Name = string.Empty;
                this.Login = string.Empty;

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    bool first = true;
                    while (rdr.Read())
                    {
                        if (first)
                        {
                            first = false;
                            this.groups = new List<SecurityGroup>();
                            this.Id = rdr.GetInt32(0);
                            this.Login = rdr.GetString(1);
                            this.Status = ApplicationLogOn.IntegerToLoginResult(rdr.GetInt32(2));
                            this.Language = rdr.GetString(3);
                            this.Employee = new Employee()
                            {
                                Name = rdr.GetString(5),
                                LastName = rdr.GetString(6),
                                SecondLastName = rdr.GetString(7)
                            };

                            this.MenuShortcuts = new MenuShortcut();

                            if (!string.IsNullOrEmpty(rdr.GetString(8)))
                            {
                                this.MenuShortcuts.Green = new Shortcut() { Label = rdr.GetString(8), Icon = rdr.GetString(9), Link = rdr.GetString(10) };
                            }

                            if (!string.IsNullOrEmpty(rdr.GetString(11)))
                            {
                                this.MenuShortcuts.Blue = new Shortcut() { Label = rdr.GetString(11), Icon = rdr.GetString(12), Link = rdr.GetString(13) };
                            }

                            if (!string.IsNullOrEmpty(rdr.GetString(14)))
                            {
                                this.MenuShortcuts.Red = new Shortcut() { Label = rdr.GetString(14), Icon = rdr.GetString(15), Link = rdr.GetString(16) };
                            }

                            if (!string.IsNullOrEmpty(rdr.GetString(17)))
                            {
                                this.MenuShortcuts.Yellow = new Shortcut() { Label = rdr.GetString(17), Icon = rdr.GetString(18), Link = rdr.GetString(19) };
                            }
                        }

                        this.groups.Add(ApplicationLogOn.IntegerToSecurityGroup(rdr.GetInt32(4)));
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                    }
                }
            }
        }

        public static UserGrant IntegerToGrant(int value)
        {
            UserGrant res = UserGrant.Undefined;
            switch (value)
            {
                case 1:
                    res = UserGrant.CompanyData;
                    break;
                case 2:
                    res = UserGrant.Departments;
                    break;
                case 3:
                    res = UserGrant.Employees;
                    break;
                case 4:
                    res = UserGrant.Documents;
                    break;
                case 5:
                    res = UserGrant.Providers;
                    break;
            }

            return res;
        }

        public static string GrantToText(SecurityGroup group)
        {
            string res = string.Empty;
            switch (group)
            {
                case SecurityGroup.Company:
                    res = "Datos de la compañía";
                    break;
                case SecurityGroup.Indicators:
                    res = "Indicadores";
                    break;
                case SecurityGroup.Documents:
                    res = "Documentos";
                    break;
                case SecurityGroup.Learning:
                    res = "Formación";
                    break;
                case SecurityGroup.Providers:
                    res = "Proveedores";
                    break;
                case SecurityGroup.Review:
                    res = "Revisión dirección";
                    break;
                case SecurityGroup.Equipos:
                    res = "Equipos";
                    break;
                case SecurityGroup.Audits:
                    res = "Auditorias";
                    break;
                case SecurityGroup.Incidence:
                    res = "Incidencias";
                    break;
            }

            return res;
        }

        public string GrantToJson()
        {
            StringBuilder res = new StringBuilder("[");
            bool first = true;
            foreach (SecurityGroup group in this.groups)
            {
                if (first == true)
                {
                    first = false;
                }
                else
                {
                    res.Append(",");
                }

                res.Append("{Id:'").Append(group).Append("',Description:'").Append(GrantToText(group)).Append("'}");
            }

            res.Append("]");
            return res.ToString();
        }
    }
}
