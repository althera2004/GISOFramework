// -----------------------------------------------------------------------
// <copyright file="ShortCut.cs" company="Sbrinna">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GisoFramework.UserInterface
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Shortcut
    {
        private string label;

        private string link;

        private string icon;

        public string Label
        {
            get 
            { 
                return this.label; 
            }

            set 
            {
                this.label = value;
            }
        }

        public string Link
        {
            get 
            {
                return this.link; 
            }

            set 
            {
                this.link = value; 
            }
        }

        public string Icon
        {
            get 
            {
                return this.icon; 
            }

            set 
            {
                this.icon = value;
            }
        }        
    }
}
