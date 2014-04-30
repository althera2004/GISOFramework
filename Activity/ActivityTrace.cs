// -----------------------------------------------------------------------
// <copyright file="ActivityTrace.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace GisoFramework.Activity
{
    using System;
    using System.Globalization;

    /// <summary>
    /// A class that implements trace of activity in application
    /// </summary>
    public class ActivityTrace
    {
        /// <summary>
        /// Actions for a department item
        /// </summary>
        public enum Department
        {
            /// <summary>Undefined action</summary>
            Undefined = 0,

            /// <summary>Department is created</summary>
            Create = 1,

            /// <summary>Department is modified</summary>
            Modify = 2,

            /// <summary>Department is deleted</summary>
            Delete = 3
        }

        /// <summary>
        /// Actions for a document item
        /// </summary>
        public enum Document
        {
            /// <summary>Undefined action</summary>
            Undefined = 0,
            
            /// <summary>Document is created</summary>            
            Create = 1,
            
            /// <summary>Document is updated</summary>            
            Update = 2,

            /// <summary>Document is deleted</summary>
            Delete = 3,

            /// <summary>Document has a new version</summary>            
            Versioned = 5,

            /// <summary>Document is in draft mode</summary>            
            Draft = 6,

            /// <summary>Document is validated</summary>            
            Validated = 7
        }

        public string Target { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the date of action
        /// </summary>
        public DateTime Date { get; set; }

        public string Changes { get; set; }

        public string Employee { get; set; }

        /// <summary>
        /// Gets the html code that shows a trace
        /// </summary>
        /// <example>
        /// <tr>
        ///     <td>Date</td>
        ///     <td>Target</td>
        ///     <td>Action</td>
        ///     <td>Data changes</td>
        ///     <td>Employee</td>
        /// </tr>
        /// </example>
        public string TableRow
        {
            get
            {
                return string.Format(CultureInfo.GetCultureInfo("es-es"), @"<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", this.Date, this.Target, this.Action, this.Changes, this.Employee);
            }
        }

        /// <summary>
        /// Gets the html code that shows a targeted trace
        /// </summary>
        /// <example>
        /// <tr>
        ///     <td>Date</td>
        ///     <td>Action</td>
        ///     <td>Data changes</td>
        ///     <td>Employee</td>
        /// </tr>
        /// </example>
        public string TableTargetedRow
        {
            get
            {
                return string.Format(CultureInfo.GetCultureInfo("es-es"), @"<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", this.Date, this.Action, this.Changes, this.Employee);
            }
        }
    }
}
