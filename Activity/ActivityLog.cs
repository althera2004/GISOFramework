// -----------------------------------------------------------------------
// <copyright file="ActivityLog.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework.Activity
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
    public enum TargetType
    {
        Undefined = 0,
        Company = 1,
        User = 2,
        SecurityGroup = 3,
        Document = 4,
        Department = 5,
        CompanyAddress = 6,
        Login = 7
    }

    public static class ActivityLog
    {
        public static ActionResult Login(string user, string ip, string companyCode, int result, int companyId)
        {
            string extradata = string.Format(CultureInfo.InvariantCulture, "{0} - {1:dd/MM/yyyy hh:mm:ss} - {2}", companyCode, DateTime.Now, ip);
            return InsertLogActivity(TargetType.Login, 0, 0, companyId, result, extradata);
        }

        public static ActionResult Company(int targetId, int userId, int companyId, int actionId, string extraData)
        {
            return InsertLogActivity(TargetType.Company, targetId, userId, companyId, actionId, extraData);
        }

        public static ActionResult User(int targetId, int userId, int companyId, int actionId, string extraData)
        {
            return InsertLogActivity(TargetType.User, targetId, userId, companyId, actionId, extraData);
        }

        public static ActionResult SecurityGroup(int targetId, int userId, int companyId, int actionId, string extraData)
        {
            return InsertLogActivity(TargetType.SecurityGroup, targetId, userId, companyId, actionId, extraData);
        }

        public static ActionResult DocumentVersioned(int targetId, int userId, int companyId, int version)
        {
            return Document(targetId, userId, companyId, Convert.ToInt32(ActivityTrace.Document.Versioned), "Version:" + version.ToString(CultureInfo.InvariantCulture));
        }

        public static ActionResult Document(int targetId, int userId, int companyId, int actionId, string extraData)
        {
            return InsertLogActivity(TargetType.Document, targetId, userId, companyId, actionId, extraData);
        }

        public static ActionResult Department(int targetId, int userId, int companyId, int actionId, string extraData)
        {
            return InsertLogActivity(TargetType.Department, targetId, userId, companyId, actionId, extraData);
        }

        public static ActionResult InsertLogActivity(TargetType targetType, int targetId, int userId, int companyId, int actionId, string extraData)
        {
            ActionResult res = new ActionResult() { Success = false, MessageError = "No action" };
            string storedProcedureName = string.Empty;

            switch (targetType)
            {
                case TargetType.Company:
                    storedProcedureName = "ActivityCompany";
                    break;
                case TargetType.User:
                    storedProcedureName = "ActivityUser";
                    break;
                case TargetType.SecurityGroup:
                    storedProcedureName = "ActivitySecurityGroup";
                    break;
                case TargetType.Document:
                    storedProcedureName = "ActivityDocument";
                    break;
                case TargetType.Department:
                    storedProcedureName = "ActivityDepartment";
                    break;
                case TargetType.Login:
                    storedProcedureName = "ActivityLogin";
                    break;
                default:
                    storedProcedureName = string.Empty;
                    break;
            }


            if (string.IsNullOrEmpty(storedProcedureName))
            {
                res.MessageError = "No valid item";
            }

            using (SqlCommand cmd = new SqlCommand(storedProcedureName))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.Parameters.Add("@TargetId", SqlDbType.Int);
                    cmd.Parameters.Add("@UserId", SqlDbType.Int);
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                    cmd.Parameters.Add("@ActionId", SqlDbType.Int);
                    cmd.Parameters.Add("@ExtraData", SqlDbType.Text);

                    cmd.Parameters["@TargetId"].Value = targetId;
                    cmd.Parameters["@UserId"].Value = userId;
                    cmd.Parameters["@CompanyId"].Value = companyId;
                    cmd.Parameters["@ActionId"].Value = actionId;
                    cmd.Parameters["@ExtraData"].Value = extraData;

                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    res.Success = true;
                    res.MessageError = string.Empty;
                }
                catch (NullReferenceException ex)
                {
                    res.MessageError = ex.Message;
                }
                catch (SqlException ex)
                {
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

        public static ReadOnlyCollection<ActivityTrace> GetActivity(int itemId, int targetType, int companyId, DateTime? from, DateTime? to)
        {
            List<ActivityTrace> res = new List<ActivityTrace>();
            /* ALTER PROCEDURE [dbo].[Get_Activity]
             * @CompanyId int,
             * @TargetType int,
             * @ItemId int,
             * @From date,
             * @To date */

            using (SqlCommand cmd = new SqlCommand("Get_Activity"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters.Add("@TargetType", SqlDbType.Int);
                cmd.Parameters.Add("@ItemId", SqlDbType.Int);
                cmd.Parameters.Add("@From", SqlDbType.Date);
                cmd.Parameters.Add("@To", SqlDbType.Date);

                cmd.Parameters["@CompanyId"].Value = companyId;
                cmd.Parameters["@TargetType"].Value = targetType;
                cmd.Parameters["@ItemId"].Value = itemId;

                if (from.HasValue)
                {
                    cmd.Parameters["@From"].Value = from.Value;
                }
                else
                {
                    cmd.Parameters["@From"].Value = DBNull.Value;
                }

                if (to.HasValue)
                {
                    cmd.Parameters["@To"].Value = to.Value;
                }
                else
                {
                    cmd.Parameters["@To"].Value = DBNull.Value;
                }

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        res.Add(new ActivityTrace()
                        {
                            Date = rdr.GetDateTime(1),
                            Target = rdr.GetString(4),
                            Changes = rdr.GetString(6),
                            Employee = rdr.GetString(7),
                            Action = rdr.GetString(5)
                        });
                    }
                }
                catch (Exception ex)
                {
                    res = new List<ActivityTrace>();
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                    }
                }
            }

            return new ReadOnlyCollection<ActivityTrace>(res);
        }
    }
}
