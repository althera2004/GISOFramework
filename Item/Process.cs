// -----------------------------------------------------------------------
// <copyright file="Process.cs" company="Sbrinna">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Process
    {
        /// <summary>
        /// Type of proccess
        /// </summary>
        public enum Type
        {
            /// <summary>Undefined</summary>
            Undefined = 0,

            /// <summary>Personal</summary>
            Personal = 1,

            /// <summary>Support</summary>
            Support = 2,

            /// <summary>Strategic</summary>
            Strategic = 3
        }

        #region Fields
        /// <summary>Identifier of proccess</summary>
        private int id;

        /// <summary>Identifier of proccess</summary>
        private int companyId;

        /// <summary>Identifier of proccess</summary>
        private Type processType;

        /// <summary>Identifier of proccess</summary>
        private string description;

        /// <summary>Identifier of proccess</summary>
        private string work;

        /// <summary>Identifier of proccess</summary>
        private string startDate;

        /// <summary>Identifier of proccess</summary>
        private string endDate;

        /// <summary>Job position of process</summary>
        private JobPosition jobPosition;

        /// <summary>Identifier of employee that modifies process</summary>
        private Employee modifiedBy;

        /// <summary>Date of proccess modification</summary>
        private DateTime modifiedOn;
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

        public Type ProcessType
        {
            get
            {
                return this.processType;
            }

            set
            {
                this.processType = value;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
            }
        }

        public string Work
        {
            get
            {
                return this.work;
            }

            set
            {
                this.work = value;
            }
        }

        public string Startdate
        {
            get
            {
                return this.startDate;
            }

            set
            {
                this.startDate = value;
            }
        }

        public string EndDate
        {
            get
            {
                return this.endDate;
            }

            set
            {
                this.endDate = value;
            }
        }

        public JobPosition JobPosition
        {
            get
            {
                return this.jobPosition;
            }

            set
            {
                this.jobPosition = value;
            }
        }

        public Employee ModifiedBy
        {
            get
            {
                return this.modifiedBy;
            }

            set
            {
                this.modifiedBy = value;
            }
        }

        public DateTime ModifiedOn
        {
            get
            {
                return this.modifiedOn;
            }

            set
            {
                this.modifiedOn = value;
            }
        }

        public static Process Empty
        {
            get
            {
                Process res = new Process();
                res.modifiedBy = new Employee() { Id = -1, Name = string.Empty, LastName = string.Empty };
                res.jobPosition = new JobPosition() { Id = -1, Description = string.Empty };
                return res;
            }
        }
        #endregion

        public Process()
        {
            this.jobPosition = JobPosition.Empty;
            this.modifiedBy = Employee.Empty;
            this.processType = Type.Undefined;
        }

        public static string TypeToString(Type processType)
        {
            string res = string.Empty;
            switch (processType)
            {
                case Type.Strategic:
                    res = "Estratégico";
                    break;
                case Type.Personal:
                    res = "Personal";
                    break;
                case Type.Support:
                    res = "Soporte";
                    break;
            }
            return res;
        }

        public static ReadOnlyCollection<Process> GetProcesos(int companyId)
        {
            List<Process> res = new List<Process>();

            /* CREATE PROCEDURE Get_ProcesosById
             * @Id int,
             * @CompanyId int */

            using (SqlCommand cmd = new SqlCommand("Get_Procesos"))
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
                        res.Add(new Process()
                        {
                            id = rdr.GetInt32(0),
                            companyId = companyId,
                            processType = (Type)rdr.GetInt32(2),
                            startDate = rdr.GetString(3),
                            work = rdr.GetString(4),
                            endDate = rdr.GetString(5),
                            description = rdr.GetString(6),
                            jobPosition = new JobPosition(Convert.ToInt32(rdr.GetInt64(7)), companyId)
                        });
                    }
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                    }
                }

                return new ReadOnlyCollection<Process>(res);
            }
        }

        public Process(int id, int companyId)
        {
            /* CREATE PROCEDURE Get_ProcesosById
             * @Id int,
             * @CompanyId int */

            using (SqlCommand cmd = new SqlCommand("Get_ProcesosById"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters["@Id"].Value = id;
                cmd.Parameters["@CompanyId"].Value = companyId;
                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        this.id = id;
                        this.companyId = companyId;
                        this.processType = (Type)rdr.GetInt32(2);
                        this.startDate = rdr.GetString(3);
                        this.work = rdr.GetString(4);
                        this.endDate = rdr.GetString(5);
                        this.jobPosition = new JobPosition(Convert.ToInt32(rdr.GetInt64(7)), companyId);
                        this.modifiedBy = new Employee(rdr.GetInt32(8));
                        this.modifiedOn = rdr.GetDateTime(9);
                    }
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