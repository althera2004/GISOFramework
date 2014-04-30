// -----------------------------------------------------------------------
// <copyright file="DocumentVersion.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework.Item
{
    using System;
    using System.Globalization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public enum DocumentStatus
    {
        /// <summary>Draft</summary>
        Draft = 0,

        /// <summary>Publish</summary>
        Publish = 1,

        /// <summary>Obsolete</summary>
        Obsolete = 2
    }

    public class DocumentVersion
    {
        #region Fields
        private long id;
        private DocumentStatus state;
        private int version;
        private ApplicationUser user;
        private Company company;
        private DateTime date;
        private string reason;
        private string userCreateName;
        #endregion

        #region Properties
        public DocumentStatus State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        public long Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public long DocumentId
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

        public int Version
        {
            get { return this.version; }
            set { this.version = value; }
        }

        public ApplicationUser User
        {
            get { return this.user; }
            set { this.user = value; }
        }

        public Company Company
        {
            get { return this.company; }
            set { this.company = value; }
        }

        public DateTime Date
        {
            get { return this.date; }
            set { this.date = value; }
        }

        public string Reason
        {
            get { return this.reason; }
            set { this.reason = value; }
        }

        public string UserCreateName
        {
            get { return this.userCreateName; }
            set { this.userCreateName = value; }
        }

        public string TableRow
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture, @"
                                <tr>
                                    <th style=""width:80px;"">{0}</th>
								    <th style=""width:80px;"">{1}</th>
								    <th>{2}</th>
								    <th style=""width:150px;"">{3}</th>
                                </tr>", this.version, string.Format(CultureInfo.CurrentCulture, "{0:dd/MM/yyyy}", this.Date), this.Reason, this.UserCreateName);
            }
        }
        #endregion

        public DocumentVersion()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static DocumentStatus IntegerToStatus(int value)
        {
            switch (value)
            {
                case 1:
                    return DocumentStatus.Publish;
                case 2:
                    return DocumentStatus.Obsolete;
            }

            return DocumentStatus.Draft;
        }
    }
}
