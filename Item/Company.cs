// -----------------------------------------------------------------------
// <copyright file="Company.cs" company="Sbrinna">
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
    using System.Text;
    using GisoFramework.Activity;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Company
    {
        #region Fields
        private int id;
        private string name;
        private DateTime subscriptionStart;
        private DateTime subscriptionEnd;
        private string mailContact;
        private string web;
        private string language;
        private string nif;
        private Collection<Department> departments;
        private Collection<Employee> employees;
        private Collection<CompanyAddress> addresses;
        private CompanyAddress defaultAddress;
        #endregion

        #region Properties
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

        public DateTime SubscriptionStart
        {
            get
            {
                return this.subscriptionStart;
            }

            set
            {
                this.subscriptionStart = value;
            }
        }

        public DateTime SubscriptionEnd
        {
            get
            {
                return this.subscriptionEnd;
            }

            set
            {
                this.subscriptionEnd = value;
            }
        }

        public string MailContact
        {
            get
            {
                return this.mailContact;
            }

            set
            {
                this.mailContact = value;
            }
        }

        public string Web
        {
            get
            {
                return this.web;
            }

            set
            {
                this.web = value;
            }
        }

        public string Language
        {
            get
            {
                return this.language;
            }

            set
            {
                this.language = value;
            }
        }

        public string Nif
        {
            get
            {
                return this.nif;
            }

            set
            {
                this.nif = value;
            }
        }

        public ReadOnlyCollection<Department> Departments
        {
            get
            {
                return new ReadOnlyCollection<Department>(this.departments);
            }
        }

        public ReadOnlyCollection<Employee> Employees
        {
            get
            {
                return new ReadOnlyCollection<Employee>(this.employees);
            }
        }

        public ReadOnlyCollection<CompanyAddress> Addresses
        {
            get
            {
                return new ReadOnlyCollection<CompanyAddress>(this.addresses);
            }
        }

        public CompanyAddress DefaultAddress
        {
            get
            {
                return this.defaultAddress;
            }

            set
            {
                this.defaultAddress = value;
            }
        }

        public Collection<KeyValuePair<string, int>> Categories
        {
            get
            {
                List<KeyValuePair<string, int>> res = new List<KeyValuePair<string, int>>();
                using (SqlCommand cmd = new SqlCommand("Company_GetDocumentCategories"))
                {
                    cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                    cmd.Parameters["@CompanyId"].Value = this.id;
                    try
                    {
                        cmd.Connection.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            res.Add(new KeyValuePair<string, int>(rdr.GetString(1), rdr.GetInt32(0)));
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

                return new Collection<KeyValuePair<string, int>>(res);
            }
        }

        public Collection<KeyValuePair<string, int>> Origins
        {
            get
            {
                List<KeyValuePair<string, int>> res = new List<KeyValuePair<string, int>>();
                using (SqlCommand cmd = new SqlCommand("Company_GetDocumentProcedencias"))
                {
                    cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                    cmd.Parameters["@CompanyId"].Value = this.id;
                    try
                    {
                        cmd.Connection.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            res.Add(new KeyValuePair<string, int>(rdr.GetString(1), rdr.GetInt32(0)));
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

                return new Collection<KeyValuePair<string, int>>(res);
            }
        }
        #endregion

        public static ReadOnlyCollection<Employee> GetEmployees(int companyId)
        {
            List<Employee> res = new List<Employee>();
            using (SqlCommand cmd = new SqlCommand("Company_GetEmployees"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters["@CompanyId"].Value = companyId;

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        res.Add(new Employee());
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

            return new ReadOnlyCollection<Employee>(res);
        }

        public Company()
        {
        }

        public static int GetByCode(string code)
        {
            int res = 0;
            using (SqlCommand cmd = new SqlCommand("Company_GetByCode"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CompanyCode", SqlDbType.Text);
                cmd.Parameters["@CompanyCode"].Value = code;
                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        res = rdr.GetInt32(0);
                    }
                }
                catch
                {
                    res = 0;
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                    }
                }
            }

            return res;
        }

        public Company(int companyId)
        {
            this.id = -1;
            this.mailContact = string.Empty;
            this.web = string.Empty;
            this.subscriptionEnd = DateTime.Now;
            this.subscriptionStart = DateTime.Now;
            this.name = string.Empty;
            this.language = "es";

            using (SqlCommand cmd = new SqlCommand("Company_GetById"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters["@CompanyId"].Value = companyId;

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        this.id = rdr.GetInt32(0);
                        this.name = rdr.GetString(1);
                        this.mailContact = string.Empty;
                        this.web = string.Empty;
                        this.subscriptionStart = rdr.GetDateTime(2);
                        this.subscriptionEnd = rdr.GetDateTime(3);
                        this.language = rdr.GetString(4);
                        this.nif = rdr.GetString(5);
                    }

                    this.departments = Company.GetDepartments(this.id);
                    this.addresses = CompanyAddress.GetAddressByCompanyId(this);
                    foreach (CompanyAddress address in this.addresses)
                    {
                        if (address.DefaultAddress)
                        {
                            this.defaultAddress = address;
                            break;
                        }
                    }

                    this.GetEmployees();
                }
                catch
                {
                    this.id = -1;
                    this.mailContact = string.Empty;
                    this.web = string.Empty;
                    this.subscriptionEnd = DateTime.Now;
                    this.subscriptionStart = DateTime.Now;
                    this.name = string.Empty;
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

        public static string ToJson(Company company)
        {
            if (company == null)
            {
                return "{}";
            }

            StringBuilder res = new StringBuilder("{").Append(Environment.NewLine);
            res.Append("\t\t\"Id\":").Append(company.id).Append(",").Append(Environment.NewLine);
            res.Append("\t\t\"Name\":\"").Append(company.name).Append("\",").Append(Environment.NewLine);
            res.Append("\t\t\"Nif\":\"").Append(company.nif).Append("\",").Append(Environment.NewLine);
            res.Append("\t\t\"MailContact\":\"").Append(company.mailContact).Append("\",").Append(Environment.NewLine);
            res.Append("\t\t\"Web\":\"").Append(company.web).Append("\",").Append(Environment.NewLine);
            res.Append("\t\t\"SubscriptionStart\":\"").Append(company.subscriptionStart.ToShortDateString()).Append("\",").Append(Environment.NewLine);
            res.Append("\t\t\"SubscriptionEnd\":\"").Append(company.subscriptionEnd.ToShortDateString()).Append("\",").Append(Environment.NewLine);
            res.Append("\t\t\"Departments\":").Append(Environment.NewLine);
            res.Append("\t\t[");
            bool firstDepartment = true;
            foreach (Department department in company.departments)
            {
                if (firstDepartment)
                {
                    firstDepartment = false;
                }
                else
                {
                    res.Append(",");
                }

                res.Append(Environment.NewLine).Append("\t\t\t").Append(department.Json);
            }
            res.Append(Environment.NewLine).Append("\t\t],").Append(Environment.NewLine);
            res.Append("\t\t\"Employees\":").Append(Environment.NewLine);
            res.Append("\t\t[");
            bool firstEmployee = true;
            foreach (Employee employee in company.employees)
            {
                if (firstEmployee)
                {
                    firstEmployee = false;
                }
                else
                {
                    res.Append(",");
                }

                res.Append(Environment.NewLine).Append("\t\t\t").Append(employee.Json);
            }

            res.Append(Environment.NewLine).Append("\t\t]").Append(Environment.NewLine);

            if (company.defaultAddress != null)
            {
                res.Append(",").Append(Environment.NewLine).Append("DefaultAddress:").Append(company.defaultAddress.Json);
            }

            res.Append(Environment.NewLine).Append("\t}");
            return res.ToString();
        }

        public static Collection<Department> GetDepartments(int companyId)
        {
            Collection<Department> res = new Collection<Department>();

            using (SqlCommand cmd = new SqlCommand("Company_GetDepartments"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters["@CompanyId"].Value = companyId;

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        res.Add(new Department()
                        {
                            CompanyId = companyId,
                            Id = rdr.GetInt32(0),
                            Name = rdr.GetString(1)
                        });
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

            return res;
        }

        public void GetDepartments()
        {
            this.departments = new Collection<Department>();
            using (SqlCommand cmd = new SqlCommand("Company_GetDepartments"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters["@CompanyId"].Value = this.id;

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        this.departments.Add(new Department()
                        {
                            CompanyId = this.id,
                            Id = rdr.GetInt32(0),
                            Name = rdr.GetString(1)
                        });
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

        public void GetEmployees()
        {
            this.employees = new Collection<Employee>();
            using (SqlCommand cmd = new SqlCommand("Company_GetEmployees"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters["@CompanyId"].Value = this.id;

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    Employee newEmployee = new Employee();
                    while (rdr.Read())
                    {
                        int employeeId = rdr.GetInt32(0);
                        if (employeeId != newEmployee.Id)
                        {
                            newEmployee = new Employee()
                            {
                                Id = employeeId,
                                Name = rdr.GetString(1),
                                LastName = rdr.GetString(2),
                                SecondLastName = rdr.GetString(3),
                                Email = rdr.GetString(4),
                                Phone = rdr.GetString(5)
                            };

                            if (rdr.GetInt32(7) != 0)
                            {
                                newEmployee.User = new ApplicationUser()
                                {
                                    Id = rdr.GetInt32(7),
                                    Login = rdr.GetString(8),
                                    Language = rdr.GetString(9),
                                    Status = ApplicationLogOn.IntegerToLoginResult(rdr.GetInt32(10))
                                };
                            }

                            this.employees.Add(newEmployee);
                        }

                        int departmetId = rdr.GetInt32(6);
                        foreach (Department department in this.departments)
                        {
                            if (department.Id == departmetId)
                            {
                                newEmployee.AddDepartment(department);
                                department.AddEmployee(newEmployee);
                                break;
                            }
                        }

                    }
                }
                catch (SqlException ex)
                {
                    string s = ex.Message;
                }
                catch (NotSupportedException ex)
                {
                    string s = ex.Message;
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                    };
                }
            }
        }

        public static ActionResult Update(int companyId, string companyName, string nif, int defaultAddress, int userId)
        {
            ActionResult res = new ActionResult() { Success = false, MessageError = "no action" };

            /* 
             * CREATE PROCEDURE Company_Update
             * @CompanyId int,
             * @Name nvarchar(50),
             * @Nif nvarchar(15),
             * @DefaultAddress int,
             * @UserId int
             */

            using (SqlCommand cmd = new SqlCommand("Company_Update"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters.Add("@Name", SqlDbType.Text);
                cmd.Parameters.Add("@Nif", SqlDbType.Text);
                cmd.Parameters.Add("@DefaultAddress", SqlDbType.Int);
                cmd.Parameters.Add("@UserId", SqlDbType.Int);
                cmd.Parameters["@CompanyId"].Value = companyId;
                cmd.Parameters["@Name"].Value = companyName;
                cmd.Parameters["@Nif"].Value = nif;
                cmd.Parameters["@DefaultAddress"].Value = defaultAddress;
                cmd.Parameters["@UserId"].Value = userId;

                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    res.Success = true;
                    res.MessageError = string.Empty;
                }
                catch (Exception ex)
                {
                    res.Success = false;
                    res.MessageError = ex.Message;
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                    }
                }
            }

            return res;
        }
    }
}
