namespace GisoFramework.UserInterface
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BreadCrumItem
    {
        public string Link { get; set; }
        public string Label { get; set; }
        public bool Leaf { get; set; }
        public bool Invariant { get; set; }

        public BreadCrumItem()
        {
            this.Invariant = false;
        }
    }
}
