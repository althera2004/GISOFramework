// -----------------------------------------------------------------------
// <copyright file="JobPositionAsigment.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework.Item
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class JobPositionAsigment
    {
        #region Fields
        private Employee employee;

        private JobPosition jobPosition;

        private DateTime startDate;

        private DateTime? endDate;
        #endregion

        #region Properties
        public Employee Employee
        {
            get
            { 
                return this.employee; 
            }

            set 
            { 
                this.employee = value;
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

        public DateTime StartDate
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

        public DateTime? EndDate
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
        #endregion

        public JobPositionAsigment Empty
        {
            get
            {
                return new JobPositionAsigment()
                {
                    employee = Employee.Empty,
                    jobPosition = JobPosition.Empty
                };
            }
        }
    }
}
