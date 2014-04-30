// -----------------------------------------------------------------------
// <copyright file="ApplicationLogin.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Web;
    using GisoFramework.Item;

    public enum LogOnResult
    {
        None = 0,

        Ok = 1,

        NoUser,

        LockUser,

        Fail,

        Admin = 1001,

        Administrative = 1002
    }

    public enum SecurityGroup
    {
        None = 0,

        Company = 1,

        Indicators = 2,

        Documents = 3,

        Learning = 4,

        Providers = 5,

        Equipos = 6,

        Incidence = 7,

        Audits = 8,

        Review = 9,

        Process = 10
    }

    public struct LogOnObject
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public LogOnResult Result { get; set; }

        public int CompanyId { get; set; }

        public Collection<SecurityGroup> Membership { get; set; }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ApplicationLogOn
    {
        public static SecurityGroup IntegerToSecurityGroup(int value)
        {
            SecurityGroup res = SecurityGroup.None;
            switch (value)
            {
                case 1:
                    res = SecurityGroup.Company;
                    break;
                case 2:
                    res = SecurityGroup.Indicators;
                    break;
                case 3:
                    res = SecurityGroup.Documents;
                    break;
                case 4:
                    res = SecurityGroup.Learning;
                    break;
                case 5:
                    res = SecurityGroup.Providers;
                    break;
                case 6:
                    res = SecurityGroup.Equipos;
                    break;
                case 7:
                    res = SecurityGroup.Incidence;
                    break;
                case 8:
                    res = SecurityGroup.Audits;
                    break;
                case 9:
                    res = SecurityGroup.Review;
                    break;
                case 10:
                    res = SecurityGroup.Process;
                    break;
            }

            return res;
        }

        public static LogOnResult IntegerToLoginResult(int value)
        {
            LogOnResult res = LogOnResult.Fail;
            switch (value)
            {
                case 0:
                    res = LogOnResult.None;
                    break;
                case 1:
                    res = LogOnResult.Ok;
                    break;
                case 2:
                    res = LogOnResult.LockUser;
                    break;
                case 3:
                    res = LogOnResult.Fail;
                    break;
                case 1001:
                    res = LogOnResult.Admin;
                    break;
                case 1002:
                    res = LogOnResult.Administrative;
                    break;
            }

            return res;
        }

        public static void LogOnFailed(int userId)
        {
            using (SqlCommand cmd = new SqlCommand("LogonFailed"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cmd.Connection.Open();
                    cmd.Parameters.Add("@UserId", SqlDbType.Int);
                    cmd.Parameters["@UserId"].Value = userId;
                    cmd.ExecuteNonQuery();
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

        public static LogOnObject GetLogin(string userName, string password, string company, string ip)
        {
            LogOnObject res = new LogOnObject()
            {
                Id = -1,
                UserName = string.Empty,
                Result = LogOnResult.NoUser,
                Membership = new Collection<SecurityGroup>()
            };

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(company))
            {
                return res;
            }

            using (SqlCommand cmd = new SqlCommand("GetLogin"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Login", SqlDbType.Text);
                    cmd.Parameters.Add("@Password", SqlDbType.Text);
                    cmd.Parameters.Add("@CompanyCode", SqlDbType.Text);

                    cmd.Parameters["@Login"].Value = userName;
                    cmd.Parameters["@Password"].Value = password;
                    cmd.Parameters["@CompanyCode"].Value = company.Trim().ToUpperInvariant();
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        bool first = true;
                        while (rdr.Read())
                        {
                            if (first)
                            {
                                first = false;
                                res.Id = rdr.GetInt32(0);
                                res.Result = IntegerToLoginResult(rdr.GetInt32(1));
                                res.UserName = userName;
                                res.CompanyId = rdr.GetInt32(3);
                                res.Membership.Add(IntegerToSecurityGroup(rdr.GetInt32(5)));
                                if (res.Result == LogOnResult.Fail)
                                {
                                    LogOnFailed(res.Id);
                                }
                                else
                                {
                                    ApplicationUser user = new ApplicationUser()
                                    {
                                        Id = res.Id,
                                        Name = res.UserName,
                                        Login = rdr.GetString(7),
                                        Language = rdr.GetString(2),
                                        Status = res.Result
                                    };
                                    user.Id = res.Id;
                                    user.Name = res.UserName;
                                    HttpContext.Current.Session["User"] = user;
                                }
                            }
                            else
                            {
                                res.Membership.Add(IntegerToSecurityGroup(rdr.GetInt32(5)));
                            }
                        }
                    }
                    else
                    {
                        res.Result = LogOnResult.NoUser;
                    }
                }
                catch (Exception ex)
                {
                    res.Result = LogOnResult.Fail;
                    res.Id = -1;
                    res.UserName = ex.Message;
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                    }
                }
            }

            bool result = res.Result == LogOnResult.Ok || res.Result == LogOnResult.Admin || res.Result == LogOnResult.Administrative;

            if (string.IsNullOrEmpty(ip))
            {
                ip = "no-ip";
            }

            InsertLog(userName, ip, result ? 1 : 2, res.Id, company, res.CompanyId);

            return res;
        }

        private static void InsertLog(string userName, string ip, int result, int userId, string companyCode, int companyId)
        {
            /* CREATE PROCEDURE Log_Login
             * @UserName nvarchar(50),
             * @Date datetime,
             * @Ip nvarchar(50),
             * @Result int,
             * @UserId int,
             * @CompanyCode nvarchar(10),
             * @CompanyId int
             * */

            using (SqlCommand cmd = new SqlCommand("Log_Login"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@UserName", SqlDbType.Text);
                cmd.Parameters.Add("@Ip", SqlDbType.Text);
                cmd.Parameters.Add("@Result", SqlDbType.Int);
                cmd.Parameters.Add("@UserId", SqlDbType.Int);
                cmd.Parameters.Add("@CompanyCode", SqlDbType.Text);
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);

                cmd.Parameters["@UserName"].Value = userName;
                cmd.Parameters["@Result"].Value = result;
                cmd.Parameters["@Ip"].Value = ip;
                cmd.Parameters["@CompanyCode"].Value = companyCode;

                if (companyId != 0)
                {
                    cmd.Parameters["@CompanyId"].Value = companyId;
                }
                else
                {
                    companyId = Company.GetByCode(companyCode);
                    if (companyId == 0)
                    {
                        cmd.Parameters["@CompanyId"].Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters["@CompanyId"].Value = companyId;
                    }
                }

                if (userId > 0)
                {
                    cmd.Parameters["@UserId"].Value = userId;
                }
                else
                {
                    cmd.Parameters["@UserId"].Value = DBNull.Value;
                }

                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
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
    }
}
