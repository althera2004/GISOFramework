// -----------------------------------------------------------------------
// <copyright file="Cargo.cs" company="Sbrinna">
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

    public class JobPosition
    {
        public float Id { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public Employee Employee { get; set; }
        public Department Department { get; set; }
        public Employee ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Responsibilities { get; set; }
        public string Notes { get; set; }
        public string AcademicSkillsDesired { get; set; }
        public string SpecificSkillsDesired { get; set; }
        public string ExperienceDesired { get; set; }
        public string DesiredHabilities { get; set; }

        public string Json
        {
            get
            {
                if (this.Id > 0)
                {
                    return string.Format(CultureInfo.GetCultureInfo("es-es"), @"{{
                    ""Id"":{0},
                    ""Description"":""{1}"",
                    ""CompanyId"":{2},
                    ""Responsabilidades"":""{3}"",
                    ""Notas"":""{4}"",
                    ""FormacionAcademicaDeseada"":""{5}"",
                    ""FormacionEspecificaDeseada"":""{6}"",
                    ""ExperienciaLaboralDeseada"":""{7}"",
                    ""HabilidadesDeseadas"":""{8}"",
                    ""Employee"":{{""Id"":{9}}},
                    ""Department"":{{""Id"":{10}}},
                    ""ModifiedBy"":{{""Id"":{11}}},
                    ""ModifiedOn"":""{12}""
                    }}
                ", this.Id, this.Description, this.CompanyId, this.Responsibilities, this.Notes, this.AcademicSkillsDesired, this.SpecificSkillsDesired, this.ExperienceDesired, this.DesiredHabilities, this.Employee.Id, this.Department.Id, this.ModifiedBy.Id, this.ModifiedOn);
                }

                return string.Format(CultureInfo.GetCultureInfo("es-es"), @"{{
                    ""Id"":0,
                    ""Description"":"""",
                    ""CompanyId"":0,
                    ""Responsabilidades"":"""",
                    ""Notas"":"""",
                    ""FormacionAcademicaDeseada"":"""",
                    ""FormacionEspecificaDeseada"":"""",
                    ""ExperienciaLaboralDeseada"":"""",
                    ""HabilidadesDeseadas"":"""",
                    ""Employee"":{{""Id"":0}},
                    ""Department"":{{""Id"":0}},
                    ""ModifiedBy"":{{""Id"":0}},
                    ""ModifiedOn"":""{0}""
                    }}
                ", DateTime.Now);
            }
        }

        public JobPosition()
        {
            this.Id = 0;
            this.Department = new Department() { Id = 0, Name = string.Empty };
            this.Employee = new Employee() { Id = 0, Name = string.Empty };
        }

        public JobPosition(int id, int companyId)
        {
            using (SqlCommand cmd = new SqlCommand("Cargos_GetById"))
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
                        this.Id = rdr.GetInt64(0);
                        this.Description = rdr.GetString(1);
                        this.Responsibilities = rdr.GetString(2);
                        this.Notes = rdr.GetString(3);
                        this.AcademicSkillsDesired = rdr.GetString(4);
                        this.SpecificSkillsDesired = rdr.GetString(5);
                        this.ExperienceDesired = rdr.GetString(6);
                        this.DesiredHabilities = rdr.GetString(7);
                        this.Employee = new Employee(rdr.GetInt32(8));
                        this.Department = new Department(rdr.GetInt32(9), companyId);
                        this.CompanyId = companyId;
                        this.ModifiedBy = new Employee(rdr.GetInt32(10));
                        this.ModifiedOn = rdr.GetDateTime(11);
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

        public static JobPosition Empty
        {
            get
            {
                return new JobPosition();
            }
        }

        public static ReadOnlyCollection<JobPosition> Cargos(Company company)
        {
            if (company == null)
            {
                return new ReadOnlyCollection<JobPosition>(new List<JobPosition>());
            }

            return Cargos(company.Id);
        }

        public static ReadOnlyCollection<JobPosition> Cargos(int companyId)
        {
            List<JobPosition> res = new List<JobPosition>();
            using (SqlCommand cmd = new SqlCommand("Cargos_GetAll"))
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
                        res.Add(new JobPosition()
                        {
                            Id = rdr.GetInt64(0),
                            Description = rdr.GetString(1),
                            Responsibilities = rdr.GetString(2),
                            Notes = rdr.GetString(3),
                            AcademicSkillsDesired = rdr.GetString(4),
                            SpecificSkillsDesired = rdr.GetString(5),
                            ExperienceDesired = rdr.GetString(6),
                            DesiredHabilities = rdr.GetString(7),
                            Employee = new Employee(rdr.GetInt32(8)),
                            Department = new Department(rdr.GetInt32(9), companyId),
                            CompanyId = companyId
                        });
                    }
                }
                catch
                {
                    res = new List<JobPosition>();
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                    }
                }
            }

            return new ReadOnlyCollection<JobPosition>(res);
        }

        public string TableRow
        {
            get
            {
                return string.Format(CultureInfo.GetCultureInfo("es-es"), "<tr><td><a href=\"CargosView.aspx?id={2}\">{4}</a></td><td>{0}</td><td style=\"cursor:pointer;\" onclick=\"EditDepartment({3});\">{1}</a></td><tr>", this.Employee.FullName, this.Department.Name, this.Id, this.Department.Id, this.Description);
            }
        }
    }
}
