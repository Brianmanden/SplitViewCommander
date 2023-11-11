using SplitViewCommander.Models;
using SplitViewCommander.Services;
using System.Diagnostics;
using Terminal.Gui;

namespace SplitViewCommander.Elements;

public class ListViews : ListView
{
    private static List<ListView>? _listViews { get; set; } = new List<ListView>();
    private Dim _height { get; set; } = Dim.Percent(95);
    private Dim _width { get; set; } = Dim.Percent(45);
    private string _currentDir { get; set; }
    private string _currentListView {  get; set; }
    private string[] _relativeDirectoryReferences { get; set; }

    private void HandleOpenSelectedItem(ListViewItemEventArgs args)
    {
        ListView listView = _listViews!.Where(lv => lv.HasFocus == true).First();
        _currentListView = listView.Id.ToString();

        if (listView == null)
        {
            throw new ArgumentException("No listview");
        }

        string folderPath = args.Value.ToString()!;

        bool isDir = SvcUtils.IsDirectory(folderPath);

        // If dir - navigate into it
        if (isDir)
        {
            if (".." == folderPath)
            {
                string? parentDir = Directory.GetParent(_currentDir)?.ToString();
                if (parentDir is not null)
                {
                    _currentDir = parentDir;
                }
            }
            else
            {
                _currentDir = folderPath;
            }
        }
        // if not dir - assuming file and opening it
        else
        {
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = folderPath, UseShellExecute = true };
            Process.Start(startInfo);
        }

        //TODO Check for access rights to folder
        string[] targetDirList = Directory.EnumerateFileSystemEntries(_currentDir).ToArray();

        string[] newDirsAndFiles = SvcUtils.ConcatArrays(_relativeDirectoryReferences, targetDirList);
        OnSourceChanged(_currentListView, _currentDir);
        listView.SetSource(newDirsAndFiles);
    }

    public event EventHandler<SourceChangedEventArgs>? SourceChanged;

    public void OnSourceChanged(string id, string currentDir)
    {
        SourceChanged?.Invoke(this, new SourceChangedEventArgs{ ListViewId = id, Directory = currentDir });
    }

    public ListView GetListView(string currentDir, string[] relativeDirectoryReferences, Pos Xpos, Pos Ypos, string id)
    {
        _currentDir ??= currentDir;
        _relativeDirectoryReferences ??= relativeDirectoryReferences;

        ListView newListView = new ListView();
        newListView.Id = id;
        newListView.Width = _width;
        newListView.Height = _height;
        newListView.X = Xpos;
        newListView.Y = Ypos;
        newListView.CanFocus = true;
        newListView.AllowsMarking = true;
        newListView.AllowsMultipleSelection = true;

        string[] dirsAndFiles = SvcUtils.ConcatArrays(relativeDirectoryReferences, Directory.EnumerateFileSystemEntries(currentDir).ToArray());
        newListView.SetSource(dirsAndFiles);

        newListView.OpenSelectedItem += HandleOpenSelectedItem;

        _listViews.Add(newListView);
        return newListView;
    }
}