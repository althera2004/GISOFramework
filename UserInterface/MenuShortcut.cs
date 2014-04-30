// -----------------------------------------------------------------------
// <copyright file="MenuShortCut.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework.UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MenuShortcut
    {
        private Shortcut green;

        private Shortcut yellow;

        private Shortcut blue;

        private Shortcut red;

        public Shortcut Green
        {
            get
            {
                return this.green;
            }

            set
            {
                this.green = value;
            }
        }

        public Shortcut Yellow
        {
            get
            {
                return this.yellow;
            }

            set
            {
                this.yellow = value;
            }
        }

        public Shortcut Blue
        {
            get
            {
                return this.blue;
            }

            set
            {
                this.blue = value;
            }
        }

        public Shortcut Red
        {
            get
            {
                return this.red;
            }

            set
            {
                this.red = value;
            }
        }
    }
}
