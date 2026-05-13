using SplitViewCommander.Models;
using SplitViewCommander.Services;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Terminal.Gui;

namespace SplitViewCommander.Elements;

public class ListViews : ListView
{
    private static List<ListView>? _listViews { get; set; } = new List<ListView>();
    private static Dictionary<string, string> _currentDirs = new Dictionary<string, string>();
    private Dim _height { get; set; } = Dim.Percent(93);
    private Dim _width { get; set; } = Dim.Percent(45);
    private string[] _relativeDirectoryReferences { get; set; }

    #region Event & Handlers
    public event EventHandler<SourceChangedEventArgs>? SourceChanged;
    public event EventHandler<FocusChangedEventArgs>? FocusChanged;

    public string GetActiveDirectory()
    {
        var activeListView = _listViews?.FirstOrDefault(lv => lv.HasFocus);
        if (activeListView != null && _currentDirs.TryGetValue(activeListView.Id.ToString(), out string? dir))
        {
            return dir;
        }
        return string.Empty;
    }

    public void RefreshActiveListView()
    {
        var activeListView = _listViews?.FirstOrDefault(lv => lv.HasFocus);
        if (activeListView != null)
        {
            string currentDir = _currentDirs[activeListView.Id.ToString()];
            string[] targetDirList = Directory.EnumerateFileSystemEntries(currentDir).ToArray();
            string[] newDirsAndFiles = SvcUtils.ConcatArrays(_relativeDirectoryReferences, targetDirList);
            activeListView.SetSource(newDirsAndFiles);
        }
    }

    private void HandleOpenSelectedItem(ListViewItemEventArgs args)
    {
        ListView listView = _listViews!.Where(lv => lv.HasFocus == true).First();
        string listViewId = listView.Id.ToString();
        string currentDir = _currentDirs[listViewId];

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
                string? parentDir = Directory.GetParent(currentDir)?.ToString();
                if (parentDir is not null)
                {
                    currentDir = parentDir;
                }
            }
            else
            {
                currentDir = folderPath;
            }
        }
        // if not dir - assuming file and opening it
        else
        {
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = folderPath, UseShellExecute = true };
            Process.Start(startInfo);
            return; // Don't refresh source if we just opened a file
        }

        _currentDirs[listViewId] = currentDir;

        //TODO Check for access rights to folder
        string[] targetDirList = Directory.EnumerateFileSystemEntries(currentDir).ToArray();

        string[] newDirsAndFiles = SvcUtils.ConcatArrays(_relativeDirectoryReferences, targetDirList);
        OnSourceChanged(listViewId, currentDir);
        listView.SetSource(newDirsAndFiles);
    }
    public void OnSourceChanged(string id, string currentDir)
    {
        SourceChanged?.Invoke(this, new SourceChangedEventArgs{ ListViewId = id, Directory = currentDir });
    }
    public void OnFocusChanged(string currentListView)
    #endregion

    {
        FocusChanged?.Invoke(this, new FocusChangedEventArgs { ListViewId = currentListView });
    }

    public ListView GetListView(string currentDir, string[] relativeDirectoryReferences, Pos Xpos, Pos Ypos, string id)
    {
        _relativeDirectoryReferences ??= relativeDirectoryReferences;
        _currentDirs[id] = currentDir;

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
        newListView.Enter += (focusEventArgs) => {
            OnFocusChanged(id);
        };

        _listViews.Add(newListView);
        return newListView;
    }
}