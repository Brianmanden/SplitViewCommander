namespace SplitViewCommander.Models
{
    public class SourceChangedEventArgs : EventArgs
    {
        public string Directory { get; set; }
        public string ListViewId { get; set; }
    }
}
