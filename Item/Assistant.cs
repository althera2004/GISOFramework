// -----------------------------------------------------------------------
// <copyright file="Assistant.cs" company="Sbrinna">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>

    public class Assistant
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Employee Employee { get; set; }
        public JobPosition JobPosition { get; set; }
        public Learning Learning { get; set; }
        public bool Completed { get; set; }
        public bool Success { get; set; }

        public static Assistant Empty
        {
            get
            {
                return new Assistant();
            }
        }

        public Assistant()
        {
            this.JobPosition = JobPosition.Empty;
            this.Employee = Employee.Empty;
            this.Learning = Learning.Empty;
        }

        public string Json
        {
            get
            {
                return string.Format(CultureInfo.GetCultureInfo("es-es"), @"
                {{
                    ""Id"":{0},
                    ""CompanyId"":{1},
                    ""Employee"":{{""Id"":{2}, ""FullName"":""{3}""}},
                    ""Cargo"":{{""Id"":{4}, ""Description"":""{5}""}},
                    ""Learning"":""{{""Id:{6},""Description"":""{6}""}},
                    ""Completed"":{7},
                    ""Success"":{8}
                }}
                ", this.Id, this.CompanyId, this.Employee.Id, this.Employee.FullName, this.JobPosition.Id, this.JobPosition.Description, this.Learning.Id, this.Learning.DateStimated, this.Completed ? "true" : "false", this.Success ? "true" : "false");
            }
        }

        public string GetRow(Dictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                return string.Empty;
            }

            string yes = dictionary["Sí"];
            string no = dictionary["No"];

            string month = string.Empty;
            switch (this.Learning.DateStimated.Month)
            {
                case 1:
                    month = dictionary["Enero"];
                    break;
                case 2:
                    month = dictionary["Febrero"];
                    break;
                case 3:
                    month = dictionary["Marzo"];
                    break;
                case 4:
                    month = dictionary["Abril"];
                    break;
                case 5:
                    month = dictionary["Mayo"];
                    break;
                case 6:
                    month = dictionary["Junio"];
                    break;
                case 7:
                    month = dictionary["Julio"];
                    break;
                case 8:
                    month = dictionary["Agosto"];
                    break;
                case 9:
                    month = dictionary["Septiembre"];
                    break;
                case 10:
                    month = dictionary["Octubre"];
                    break;
                case 11:
                    month = dictionary["Noviembre"];
                    break;
                case 12:
                    month = dictionary["Diciembre"];
                    break;
            }

            return string.Format(CultureInfo.CurrentCulture, @"
                <tr id=""{0}"">
                    <td><input type=""checkbox"" name=""Assistant{1}"" id=""Assistant{1}"" /></td>
                    <td><a href=""FormacionView.aspx?id={10}"">{2}</a></td>
                    <td>{3}</td>
                    <td align=""center"" style=""color:{4};"">{5}</td>
                    <td align=""center"" style=""color:{6};"">{7}</td>
                    <td align=""center"">{8} {9}</td>
                </tr>
                ", this.Id, string.Format("{0:000}", this.Id), this.Learning.Description, this.Employee.FullName, this.Completed ? "green" : "red", this.Completed ? yes : no, this.Success ? "gree" : "red", this.Success ? yes : no, month, this.Learning.DateStimated.Year, this.Learning.Id);
        }

        public static ReadOnlyCollection<Assistant> GetAll(int companyId, int? year, int? status)
        {
            List<Assistant> res = new List<Assistant>();

            /* ALTER PROCEDURE Learning_GetAll
             * @CompanyId int,
             * @Year int,
             * @Status int */
            using (SqlCommand cmd = new SqlCommand("Learning_GetAll"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters.Add("@Year", SqlDbType.Int);
                cmd.Parameters.Add("@Status", SqlDbType.Int);

                cmd.Parameters["@CompanyId"].Value = companyId;
                if (year.HasValue)
                {
                    cmd.Parameters["@Year"].Value = year.Value;
                }
                else
                {
                    cmd.Parameters["@Year"].Value = DBNull.Value;
                }

                if (status.HasValue)
                {
                    cmd.Parameters["@Status"].Value = status.Value;
                }
                else
                {
                    cmd.Parameters["@Status"].Value = DBNull.Value;
                }

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        res.Add(new Assistant()
                        {
                            Id = rdr.GetInt32(2),
                            Completed = rdr.GetBoolean(7),
                            Success = rdr.GetBoolean(8),
                            Employee = new Employee()
                            {
                                Id = rdr.GetInt32(3),
                                Name = rdr.GetString(4),
                                LastName = rdr.GetString(5),
                                SecondLastName = rdr.GetString(6)
                            },
                            Learning = new Learning()
                            {
                                Id = rdr.GetInt32(0),
                                Description = rdr.GetString(1),
                                DateStimated = rdr.GetDateTime(9)
                            }
                        });
                    }
                }
                catch
                {
                    res = new List<Assistant>();
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                    }
                }
            }

            return new ReadOnlyCollection<Assistant>(res);
        }
    }
}
