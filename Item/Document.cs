// -----------------------------------------------------------------------
// <copyright file="Document.cs" company="Sbrinna">
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
    public class Document
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public Company Company { get; set; }

        public string Description { get; set; }

        private List<DocumentVersion> versions;

        public KeyValuePair<string, int> Category { get; set; }

        public KeyValuePair<string, int> Origin { get; set; }

        public string Location { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaBaja { get; set; }

        public bool Source { get; set; }

        public int Conservation { get; set; }

        public int ConservationType { get; set; }

        public DateTime ModifiedOn { get; set; }

        public ApplicationUser ModifiedBy { get; set; }

        public Document()
        {
            this.versions = new List<DocumentVersion>();
            this.Conservation = 0;
            this.ConservationType = 0;
            this.Source = true;
        }

        public static ReadOnlyCollection<Document> GetByCompany(Company company)
        {
            List<Document> res = new List<Document>();
            if (company != null)
            {
                using (SqlCommand cmd = new SqlCommand("Company_GetDocuments"))
                {
                    cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                    try
                    {
                        cmd.Parameters["@CompanyId"].Value = company.Id;
                        Document newDocument = new Document();
                        cmd.Connection.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            if (newDocument.Id != rdr.GetInt64(0))
                            {
                                newDocument = new Document()
                                {
                                    Id = rdr.GetInt64(0),
                                    Company = company,
                                    Description = rdr.GetString(2),
                                    Code = rdr.GetString(7),
                                    Category = new KeyValuePair<string, int>(rdr.GetString(12), rdr.GetInt32(11)),
                                    Origin = new KeyValuePair<string, int>(rdr.GetString(14), rdr.GetInt32(13))
                                };

                                res.Add(newDocument);
                            }

                            newDocument.AddVersion(new DocumentVersion()
                            {
                                DocumentId = rdr.GetInt64(0),
                                Date = rdr.GetDateTime(6),
                                Company = company,
                                Id = rdr.GetInt64(1),
                                State = DocumentVersion.IntegerToStatus(rdr.GetInt32(5)),
                                User = new ApplicationUser(rdr.GetInt32(4)),
                                Version = rdr.GetInt32(3),
                                Reason = rdr.GetString(8),
                                UserCreateName = rdr.GetString(10)
                            });
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

            return new ReadOnlyCollection<Document>(res);
        }

        public ReadOnlyCollection<DocumentVersion> Versions
        {
            get
            {
                return new ReadOnlyCollection<DocumentVersion>(this.versions);
            }
        }

        public string TableRow
        {
            get
            {
                DocumentVersion actual = this.LastVersion;
                return string.Format(CultureInfo.CurrentCulture, @"<tr><td><strong>{0}</strong></td><td class=""hidden-480"">{1}</td><td class=""hidden-480"" align=""right"">{2}</td></tr>", this.DocumentViewLink, this.Code, actual.Version.ToString());
            }
        }

        public string DocumentViewLink
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture, "<a href=\"DocumentView.aspx?id={0}\" title=\"{2} {1}\">{1}</a>", this.Id, this.Description, ((Dictionary<string, string>)HttpContext.Current.Session["Dictionary"])["Ver detalle de"]);
            }
        }

        public string Json
        {
            get
            {
                DocumentVersion actual = this.LastVersion;
                string fechaBaja = string.Empty;
                if (this.FechaBaja.HasValue)
                {
                    fechaBaja = string.Format(CultureInfo.CurrentCulture, "{0:dd/MM/yyyy}", this.FechaBaja.Value);
                }

                return string.Format(CultureInfo.CurrentCulture, @"
            {{
                Id:{0},
                Codigo:'{1}',
                Documento:'{2}',
                FechaAlta:'{3}',
                FechaBaja:'{4}',
                Categoria:{5},
                FechaRevision:'{6}',
                Revision:{7},
                Procedencia:{8},
                Conservacion:{{type:{9}, cantidad:{10}}},
                Origen:{11},
                Ubicacion:'{12}',
                Motivo:'{13}',
                Activo:{14}
            }}
        ", this.Id, this.Code, this.Description, string.Format(CultureInfo.CurrentCulture, "{0:dd/MM/yyy}", this.FechaAlta), fechaBaja, this.Category.Value, string.Format(CultureInfo.CurrentCulture, "{0:dd/MM/yyy}", actual.Date), actual.Version, this.Origin.Value, this.ConservationType, this.Conservation, this.Source ? "1" : "2", this.Location, actual.Reason, this.Active ? "true" : "false");
            }
        }

        public DocumentVersion LastVersion
        {
            get
            {
                DocumentVersion res = new DocumentVersion();
                foreach (DocumentVersion version in this.Versions)
                {
                    if (res.Date == null || version.Date > res.Date)
                    {
                        res = version;
                    }
                }

                return res;
            }
        }

        public bool Active
        {
            get
            {
                return !this.FechaBaja.HasValue;
            }
        }

        public static Document GetById(long documentId, Company company)
        {
            if (company == null)
            {
                return new Document();
            }

            return GetById(documentId, company.Id);
        }

        public static Document GetById(long documentId, int companyId)
        {
            Document res = new Document();
            using (SqlCommand cmd = new SqlCommand("Document_GetById"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@DocumentId", SqlDbType.BigInt);
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);

                Company company = new Company(companyId);

                try
                {
                    cmd.Parameters["@DocumentId"].Value = documentId;
                    cmd.Parameters["@CompanyId"].Value = companyId;
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    bool first = true;
                    while (rdr.Read())
                    {
                        if (first)
                        {
                            first = false;
                            res.Id = documentId;
                            res.Company = company;
                            res.Description = rdr.GetString(2);
                            res.Category = new KeyValuePair<string, int>(rdr.GetString(11), rdr.GetInt32(10));
                            res.Origin = new KeyValuePair<string, int>(rdr.GetString(13), rdr.GetInt32(12));
                            res.Code = rdr.GetString(14);
                            res.FechaAlta = rdr.GetDateTime(15);
                            if (!rdr.IsDBNull(16))
                            {
                                res.FechaBaja = rdr.GetDateTime(16);
                            }

                            res.Conservation = rdr.GetInt32(17);
                            res.ConservationType = rdr.GetInt32(18);
                            res.Source = rdr.GetInt32(19) == 1;
                            res.Location = rdr.GetString(20);
                            res.ModifiedOn = rdr.GetDateTime(22);
                            res.ModifiedBy = new ApplicationUser(rdr.GetInt32(21));
                        }

                        res.AddVersion(new DocumentVersion()
                        {
                            Id = rdr.GetInt64(1),
                            Company = company,
                            DocumentId = documentId,
                            User = new ApplicationUser(rdr.GetInt32(4)),
                            Version = rdr.GetInt32(3),
                            Date = rdr.GetDateTime(6),
                            State = DocumentVersion.IntegerToStatus(rdr.GetInt32(5)),
                            Reason = rdr.GetString(7),
                            UserCreateName = rdr.GetString(9)
                        });
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

            return res;
        }

        public static ReadOnlyCollection<Document> Drafts(ReadOnlyCollection<Document> documents)
        {
            List<Document> res = new List<Document>();
            if (documents != null)
            {
                foreach (Document document in documents)
                {
                    if (document.LastVersion.State == DocumentStatus.Draft)
                    {
                        res.Add(document);
                    }
                }
            }

            return new ReadOnlyCollection<Document>(res);
        }

        public static ReadOnlyCollection<Document> Recent(ReadOnlyCollection<Document> documents)
        {
            List<Document> res = new List<Document>();
            if (documents != null)
            {
                foreach (Document document in documents)
                {
                    if (document.LastVersion.Date > DateTime.Now.AddDays(-7))
                    {
                        res.Add(document);
                    }
                }
            }

            return new ReadOnlyCollection<Document>(res);
        }

        public static ActionResult Versioned(int id, int userId, int companyId, int version)
        {
            ActionResult res = new ActionResult() { Success = false, MessageError = "No action" };
            /* CREATE PROCEDURE [dbo].[Document_Versioned]
             * @DocumentId int,
             * @CompanyId int,
             * @Version int,
             * @UserId int */
            using (SqlCommand cmd = new SqlCommand("Document_Versioned"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@DocumentId", SqlDbType.BigInt);
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters.Add("@Version", SqlDbType.Int);
                cmd.Parameters.Add("@UserId", SqlDbType.Int);

                cmd.Parameters["@DocumentId"].Value = id;
                cmd.Parameters["@CompanyId"].Value = companyId;
                cmd.Parameters["@Version"].Value = version;
                cmd.Parameters["@UserId"].Value = userId;

                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    res.Success = true;
                    res.MessageError = "ok";
                }
                catch (Exception ex)
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

        public ActionResult Update(int userId)
        {
            ActionResult res = new ActionResult() { Success = false, MessageError = "No action" };
            /* CREATE PROCEDURE Document_Update
             * @DocumentId bigint,
             * @CompanyId int,
             * @Description nvarchar(50),
             * @CategoryId int,
             * @FechaBaja date,
             * @Origen int,
             * @ProcedenciaId int,
             * @Conservacion int,
             * @ConservacionType int,
             * @Activo bit,
             * @Codigo nvarchar(10),
             * @Ubicacion nvarchar(100),
             * @UserId int */
            using (SqlCommand cmd = new SqlCommand("Document_Update"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@DocumentId", SqlDbType.BigInt);
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar);
                cmd.Parameters.Add("@FechaBaja", SqlDbType.Date);
                cmd.Parameters.Add("@Origen", SqlDbType.Int);
                cmd.Parameters.Add("@CategoryId", SqlDbType.Int);
                cmd.Parameters.Add("@ProcedenciaId", SqlDbType.Int);
                cmd.Parameters.Add("@Conservacion", SqlDbType.Int);
                cmd.Parameters.Add("@ConservacionType", SqlDbType.Int);
                cmd.Parameters.Add("@Activo", SqlDbType.Bit);
                cmd.Parameters.Add("@Codigo", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Ubicacion", SqlDbType.NVarChar);
                cmd.Parameters.Add("@UserId", SqlDbType.Int);

                cmd.Parameters["@DocumentId"].Value = this.Id;
                cmd.Parameters["@CompanyId"].Value = this.Company.Id;
                cmd.Parameters["@Description"].Value = this.Description;

                if (this.FechaBaja.HasValue)
                {
                    cmd.Parameters["@FechaBaja"].Value = this.FechaBaja;
                }
                else
                {
                    cmd.Parameters["@FechaBaja"].Value = DBNull.Value;
                }

                cmd.Parameters["@Origen"].Value = this.Source;
                cmd.Parameters["@CategoryId"].Value = this.Category.Value;
                cmd.Parameters["@ProcedenciaId"].Value = this.Origin.Value;
                cmd.Parameters["@Conservacion"].Value = this.Conservation;
                cmd.Parameters["@ConservacionType"].Value = this.ConservationType;
                cmd.Parameters["@Activo"].Value = this.Active;
                cmd.Parameters["@Codigo"].Value = this.Code;
                cmd.Parameters["@Ubicacion"].Value = this.Location;
                cmd.Parameters["@UserId"].Value = userId;

                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    res.Success = true;
                }
                catch (Exception ex)
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

        public void AddVersion(DocumentVersion version)
        {
            if (this.versions == null)
            {
                this.versions = new List<DocumentVersion>();
            }

            this.versions.Add(version);
        }
    }
}
