// -----------------------------------------------------------------------
// <copyright file="Employee.cs" company="Sbrinna">
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
    using System.Text;
    using System.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>

    public class Employee
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string SecondLastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int UserId { get; set; }

        private List<Department> departments;

        public Employee()
        {
            this.Initialize(0, 0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public static Employee Empty
        {
            get
            {
                return new Employee();
            }
        }

        public Collection<JobPositionAsigment> JobPositionAsigment { get; set; }

        public ApplicationUser User { get; set; }

        public ReadOnlyCollection<Department> Departments
        {
            get
            {
                return new ReadOnlyCollection<Department>(this.departments);
            }
        }

        public string FullName
        {
            get
            {
                string res = string.Empty;
                if (!string.IsNullOrEmpty(this.LastName))
                {
                    res = this.LastName;
                }

                if (!string.IsNullOrEmpty(this.SecondLastName))
                {
                    if (!string.IsNullOrEmpty(res))
                    {
                        res += " ";
                    }

                    res += this.SecondLastName;
                }

                if (!string.IsNullOrEmpty(this.Name))
                {
                    if (!string.IsNullOrEmpty(res))
                    {
                        res += ", ";
                    }

                    res += this.Name;
                }

                return res;
            }
        }

        public string Json
        {
            get
            {
                StringBuilder res = new StringBuilder();
                res.Append("{");
                res.Append("\"Id\":").Append(this.Id).Append(",");
                res.Append("\"Name\":\"").Append(this.Name).Append("\",");
                res.Append("\"LastName\":\"").Append(this.LastName).Append("\",");
                res.Append("\"SecondLastName\":\"").Append(this.SecondLastName).Append("\",");
                res.Append("\"Email\":\"").Append(this.Email).Append("\",");
                res.Append("\"Phone\":\"").Append(this.Phone).Append("\",");
                res.Append("\"Deparments\":[");
                bool first = true;
                foreach (Department department in this.Departments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        res.Append(",");
                    }

                    res.Append(department.Id);
                }

                res.Append("]");
                if (this.User != null)
                {
                    if (this.User.Id > 0)
                    {
                        res.Append(",\"User\":").Append(this.User.Json);
                    }
                }

                res.Append("}");
                return res.ToString();
            }
        }

        public string GetEmployeeRow
        {
            get
            {
                string departmentsList = string.Empty;
                bool first = true;

                string cargo = string.Empty;

                switch (this.Id)
                {
                    case 1: cargo = "Matenimiento eléctrico"; break;
                    case 2: cargo = "Recursos humanos"; break;
                    case 3: cargo = "Redactor de material"; break;
                    case 4: cargo = "Financiaro/contable"; break;
                }

                foreach (Department deparment in this.Departments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        departmentsList += ", ";
                    }

                    departmentsList += string.Format(CultureInfo.GetCultureInfo("es-es"), "<a href=\"DepartmentView.aspx?id{0} alt=\"{1}\">{1}</a>", deparment.Id, deparment.Name);
                }

                return string.Format(CultureInfo.GetCultureInfo("es-es"), @"<tr>
																<td><a href=""EmployeesView.aspx?id={0}"">{1}</a></td>
																<td><a href=""#"">{5}</a></td>
																<td class=""hidden-480"">{2}</td>
																<td>{3}</td>
																<td class=""hidden-480"">{4}</td>
															</tr>", this.Id, this.FullName, departmentsList, this.Email, this.Phone, cargo);
            }
        }

        public string ProfileViewLink
        {
            get
            {
                return string.Format(CultureInfo.GetCultureInfo("es-es"), "<a href=\"{0}\" title=\"{2} {1}\">{1}</a>", this.Id, this.FullName, ((Dictionary<string, string>)HttpContext.Current.Session["Dictionary"])["Ver perfil de"]);
            }
        }

        public void AddDepartment(Department department)
        {
            if (department == null)
            {
                return;
            }

            if (this.departments == null)
            {
                this.departments = new List<Department>();
            }

            this.departments.Add(department);
        }

        public void GetJobPositions()
        {
            this.JobPositionAsigment = new Collection<JobPositionAsigment>();
            /* ALTER PROCEDURE Employee_GetJobPositions
             * @EmployeeId int,
             * @CompanyId int */
            using (SqlCommand cmd = new SqlCommand("Employee_GetJobPositions"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                try
                {
                    cmd.Parameters.Add("@EmployeeId", SqlDbType.Int);
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                    cmd.Parameters["@EmployeeId"].Value = this.Id;
                    cmd.Parameters["@CompanyId"].Value = this.CompanyId;
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        JobPositionAsigment newJobPositionAsigment = new JobPositionAsigment()
                        {
                            Employee = this,
                            StartDate = rdr.GetDateTime(0),
                            JobPosition = new JobPosition()
                            {
                                Id = rdr.GetInt32(2),
                                Description = rdr.GetString(4),
                                Department = new Department()
                                {
                                    Id = rdr.GetInt32(3),
                                    Name = rdr.GetString(10)
                                },
                                Employee = new Employee()
                                {
                                    Id = rdr.GetInt32(5),
                                    Name = rdr.GetString(7),
                                    LastName = rdr.GetString(8),
                                    SecondLastName = rdr.GetString(9)
                                }
                            }
                        };

                        if (!rdr.IsDBNull(1))
                        {
                            newJobPositionAsigment.EndDate = rdr.GetDateTime(1);
                        }

                        this.JobPositionAsigment.Add(newJobPositionAsigment);
                    }
                }
                catch
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

        public Employee(int employeeId)
        {

            this.departments = new List<Department>();
            using (SqlCommand cmd = new SqlCommand("Employee_GetById"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmployeeId", SqlDbType.Int);
                    cmd.Parameters["@EmployeeId"].Value = employeeId;
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    bool first = true;
                    while (rdr.Read())
                    {
                        if (first)
                        {
                            first = false;
                            this.Id = rdr.GetInt32(0);
                            this.Name = rdr.GetString(1);
                            this.LastName = rdr.GetString(2);
                            this.SecondLastName = rdr.GetString(3);
                            this.Email = rdr.GetString(4);
                            this.Phone = rdr.GetString(5);
                            int userId = rdr.GetInt32(7);

                            if (userId != 0)
                            {
                                this.User = new ApplicationUser(userId);
                            }
                        }

                        int deparmentId = rdr.GetInt32(6);
                        if (deparmentId != 0)
                        {
                            Department department = new Department(deparmentId, this.CompanyId);
                            if (department != null)
                            {
                                this.departments.Add(department);
                            }
                        }
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

        public Employee(int employeeId, bool complete)
        {
            this.departments = new List<Department>();
            using (SqlCommand cmd = new SqlCommand("Employee_GetById"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmployeeId", SqlDbType.Int);
                    cmd.Parameters["@EmployeeId"].Value = employeeId;
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    bool first = true;
                    while (rdr.Read())
                    {
                        if (first)
                        {
                            first = false;
                            this.Id = rdr.GetInt32(0);
                            this.Name = rdr.GetString(1);
                            this.LastName = rdr.GetString(2);
                            this.SecondLastName = rdr.GetString(3);
                            this.Email = rdr.GetString(4);
                            this.Phone = rdr.GetString(5);
                            int userId = rdr.GetInt32(7);

                            if (userId != 0)
                            {
                                this.User = new ApplicationUser(userId);
                            }
                        }

                        int deparmentId = rdr.GetInt32(6);
                        if (deparmentId != 0)
                        {
                            Department department = new Department(deparmentId, this.CompanyId);
                            if (department != null)
                            {
                                this.departments.Add(department);
                            }
                        }
                    }

                    if (complete)
                    {
                        this.GetJobPositions();
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

        public static string ToJson(Employee employee)
        {
            if (employee == null)
            {
                return "{}";
            }

            return string.Format(@"
                {{
                    ""Id"":{0},
                    ""CompanyId"":{1},
                    ""Name"":""{2}"",
                    ""Email"":""{3}"",
                    ""UserId"":{4}
                }}", employee.Id, employee.CompanyId, employee.Name, employee.Email, employee.UserId);
        }

        private void Initialize(int id, int companyId, string name, string lastName, string secondLastName, string email, string phone)
        {
            this.Id = id;
            this.CompanyId = companyId;
            this.Name = name;
            this.LastName = lastName;
            this.SecondLastName = secondLastName;
            this.Email = email;
            this.Phone = phone;
            this.departments = new List<Department>();
            this.User = new ApplicationUser();
        }
    }
}
