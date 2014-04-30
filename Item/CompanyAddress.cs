// -----------------------------------------------------------------------
// <copyright file="CompanyAddress.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework.Item
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>

    public class CompanyAddress
    {
        #region Fields
        private int id;
        private Company company;
        private string address;
        private string postalCode;
        private string city;
        private string province;
        private string country;
        private string phone;
        private string mobile;
        private string fax;
        private string email;
        private string notes;
        private bool defaultAddress;
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

        public Company Company
        {
            get 
            { 
                return this.company;
            }

            set
            { 
                this.company = value;
            }
        }

        public string Address
        {
            get 
            {
                return this.address; 
            }
           
            set 
            { 
                this.address = value;
            }
        }

        public string PostalCode
        {
            get
            {
                return this.postalCode;
            }
            
            set
            { 
                this.postalCode = value; 
            }
        }

        public string City
        {
            get 
            { 
                return this.city;
            }

            set 
            { 
                this.city = value; 
            }
        }

        public string Province
        {
            get
            { 
                return this.province;
            }

            set
            {
                this.province = value;
            }
        }

        public string Country
        {
            get
            { 
                return this.country; 
            }

            set
            {
                this.country = value; 
            }
        }

        public string Phone
        {
            get 
            { 
                return this.phone; 
            }

            set 
            { 
                this.phone = value; 
            }
        }

        public string Mobile
        {
            get 
            {
                return this.mobile;
            }

            set
            { 
                this.mobile = value; 
            }
        }

        public string Fax
        {
            get 
            { 
                return this.fax;
            }

            set
            {
                this.fax = value;
            }
        }

        public string Email
        {
            get
            { 
                return this.email;
            }

            set 
            {
                this.email = value;
            }
        }

        public string Notes
        {
            get
            { 
                return this.notes;
            }

            set
            { 
                this.notes = value; 
            }
        }

        public bool DefaultAddress
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
        #endregion

        public CompanyAddress()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static Collection<CompanyAddress> GetAddressByCompanyId(Company company)
        {
            if (company == null)
            {
                return new Collection<CompanyAddress>();
            }

            Collection<CompanyAddress> res = new Collection<CompanyAddress>();
            using (SqlCommand cmd = new SqlCommand("Company_GetAdress"))
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cns"].ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int);
                cmd.Parameters["@CompanyId"].Value = company.Id;

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        res.Add(new CompanyAddress()
                        {
                            id = rdr.GetInt32(0),
                            company = company,
                            address = rdr.GetString(2),
                            postalCode = rdr.GetString(3),
                            city = rdr.GetString(4),
                            province = rdr.GetString(5),
                            country = rdr.GetString(6),
                            phone = rdr.GetString(7),
                            mobile = rdr.GetString(8),
                            fax = rdr.GetString(9),
                            email = rdr.GetString(10),
                            notes = rdr.GetString(11),
                            defaultAddress = rdr.GetInt32(12) == 1
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

        public string Json
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{{Id:{0}, CompanId:{1}, Address:'{2}', PostalCode:'{3}', City:'{4}', Province:'{5}', Country:'{6}', Phone:'{7}', Mobile:'{8}', Fax:'{9}', Email:'{10}'}}", this.Id, this.company.Id, this.address.Replace("'", "\\'"), this.postalCode, this.city.Replace("'", "\\'"), this.province.Replace("'", "\\'"), this.country.Replace("'", "\\'"), this.phone, this.mobile, this.fax, this.email);
            }
        }

        public string SelectOption
        {
            get
            {
                return string.Format("<option value=\"{0}\" {2}>{1}</option>", this.id, this.address + ", " + this.city, this.Id == this.company.DefaultAddress.Id ? "selected=\"selected\"" : string.Empty);
            }
        }
    }
}
