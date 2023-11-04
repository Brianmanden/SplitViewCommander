using SplitViewCommander.Services;
using System.Diagnostics;
using Terminal.Gui;

namespace SplitViewCommander.Elements
{
    public class ListViews : ListView
    {
        private ListView _listView { get; set; }
        private Dim _height { get; set; } = Dim.Percent(45);
        private Dim _width { get; set; } = Dim.Percent(45);
        private string _currentDir { get; set; } = "";
        private string[] _relativeDirectoryReferences { get; set; }

        private void HandleOpenSelectedItem(ListViewItemEventArgs args)
        {
            string filePath = args.Value.ToString()!;
            bool isDir = SvcUtils.IsDirectory(filePath);

            // If dir - navigate into it
            if (isDir)
            {
                if (".." == filePath)
                {
                    string? parentDir = Directory.GetParent(_currentDir)?.ToString();
                    if (parentDir is not null)
                    {
                        _currentDir = parentDir;
                    }
                }
                else
                {
                    _currentDir = filePath;
                }
            }
            // if not dir - assuming file and opening it
            else
            {
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = filePath, UseShellExecute = true };
                Process.Start(startInfo);
            }

            string[] dirsAndFiles = SvcUtils.ConcatArrays(_relativeDirectoryReferences, Directory.EnumerateFileSystemEntries(_currentDir).ToArray());

            _listView.SetSource(dirsAndFiles);
        }

        public ListView GetListView(string[] leftFiles, string currentDir, string[] relativeDirectoryReferences, Pos Xpos, Pos Ypos)
        {
            _currentDir ??= currentDir;
            _relativeDirectoryReferences ??= relativeDirectoryReferences;
            _listView = new ListView(leftFiles) { Width = _width, Height = _height, X = Xpos, Y = Ypos, AllowsMarking = true, AllowsMultipleSelection = true };

            string[] dirsAndFiles = SvcUtils.ConcatArrays(relativeDirectoryReferences, Directory.EnumerateFileSystemEntries(currentDir).ToArray());

            _listView.OpenSelectedItem += HandleOpenSelectedItem;

            return _listView;
        }

    }
}
