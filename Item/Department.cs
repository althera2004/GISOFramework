// -----------------------------------------------------------------------
// <copyright file="Department.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace GisoFramework.Item
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Web;
    using GisoFramework.Activity;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Department
    {
        #region Fields
        private int id;
        private int companyId;
        private string name;
        private List<Employee> employees;
        #endregion

        #region Properties
        public ReadOnlyCollection<Employee> Employees
        {
            get
            {
                return new ReadOnlyCollection<Employee>(this.employees);
            }
        }

        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        public int CompanyId
        {
            get
            {
                return this.companyId;
            }

            set
            {
                this.companyId = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        public string Json
        {
            get
            {
                return "{\"Id\":" + this.id.ToString() + ",\"Description\":\"" + this.Name + "\",\"CompanyId\":" + this.companyId.ToString(CultureInfo.CurrentCulture) + "}";
            }
        }

        public string TableRow
        {
            get
            {
                string employeesList = string.Empty;
                bool first = true;
                foreach (Employee employee in this.employees)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        employeesList += ", ";
                    }

                    employeesList += employee.ProfileViewLink;
                }

                return string.Format(CultureInfo.InvariantCulture, @"<tr><td><strong>{0}</strong></td><td class=""hidden-480"">({1}) {2}</td></tr>", this.ProfileViewLink, this.employees.Count, employeesList);
            }
        }

        public string ProfileViewLink
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "<a href=\"DepartmentView.aspx?id={0}\" title=\"{2} {1}\">{1}</a>", this.id, this.name, ((Dictionary<string, string>)HttpContext.Current.Session["Dictionary"])["Ver perfil de"]);
            }
        }
        #endregion

        public Department()
        {
            this.employees = new List<Employee>();
        }

        public Department(int id, int companyId)
        {
            this.employees = new List<Employee>();
            SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
            SqlCommand cmd = new SqlCommand("Department_GetById", cnn);

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters["@Id"].Value = id;
                cmd.Parameters["@CompanyId"].Value = companyId;
                cnn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    this.id = id;
                    this.companyId = companyId;
                    this.name = rdr["Name"].ToString();

                    this.employees.Add(new Employee()
                    {
                        Id = rdr.GetInt32(3),
                        Name = rdr.GetString(4),
                        LastName = rdr.GetString(5),
                        SecondLastName = rdr.GetString(6)
                    });
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (cnn.State != ConnectionState.Closed)
                {
                    cnn.Close();
                }

                cnn.Dispose();
                cmd.Dispose();
            }
        }

        public void Insert()
        {
            using (SqlCommand cmd = new SqlCommand("Department_Insert"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                    cmd.Parameters["@CompanyId"].Value = this.companyId;

                    cmd.Parameters.Add("@Description", SqlDbType.Text);
                    cmd.Parameters["@Description"].Value = this.name;

                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters["@Id"].Value = this.Id;
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    this.id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                    ActivityLog.Department(this.id, Convert.ToInt32(HttpContext.Current.Session["UserId"], CultureInfo.CurrentCulture), Convert.ToInt32(HttpContext.Current.Session["CompanyId"], CultureInfo.CurrentCulture), Convert.ToInt32(ActivityTrace.Department.Create, CultureInfo.CurrentCulture), this.name);
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

        public void AddEmployee(Employee employee)
        {
            if (employee != null)
            {
                if (this.employees == null)
                {
                    this.employees = new List<Employee>();
                }

                this.employees.Add(employee);
            }
        }
    }
}
