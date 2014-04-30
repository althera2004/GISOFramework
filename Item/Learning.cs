// -----------------------------------------------------------------------
// <copyright file="Learning.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework.Item
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Learning
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime DateStimated { get; set; }
        public DateTime? RealStart { get; set; }
        public DateTime? RealFinish { get; set; }
        public string Master { get; set; }
        public int Hours { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public int Year { get; set; }
        public Employee ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        public static Learning Empty
        {
            get
            {
                Learning res = new Learning();
                res.ModifiedBy = Employee.Empty;
                return res;
            }
        }

        public Learning()
        {
        }
    }
}
