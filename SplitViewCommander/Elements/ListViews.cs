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

    private string FormatFileSystemEntry(string path)
    {
        if (path == "..") return "..";
        string name = Path.GetFileName(path);
        if (string.IsNullOrEmpty(name)) name = path; // Handle root drives

        if (SvcUtils.IsDirectory(path))
        {
            return $"/{name}";
        }
        return name;
    }

    public string GetFullPathFromFormatted(string formatted, string currentDir)
    {
        if (formatted == "..") return "..";

        string name = formatted;
        if (formatted.StartsWith("/"))
        {
            name = formatted.Substring(1);
        }

        return Path.Combine(currentDir, name);
    }

    public string GetActiveDirectory()
    {
        var activeListView = GetActiveListView();
        if (activeListView != null && _currentDirs.TryGetValue(activeListView.Id.ToString(), out string? dir))
        {
            return dir;
        }
        return string.Empty;
    }

    public ListView? GetActiveListView()
    {
        return _listViews?.FirstOrDefault(lv => lv.HasFocus);
    }

    public void RefreshActiveListView()
    {
        var activeListView = _listViews?.FirstOrDefault(lv => lv.HasFocus);
        if (activeListView != null)
        {
            string listViewId = activeListView.Id.ToString();
            string currentDir = _currentDirs[listViewId];
            UpdateListViewSource(activeListView, currentDir);
        }
    }

    private void UpdateListViewSource(ListView listView, string directory)
    {
        string[] targetDirList = Directory.EnumerateFileSystemEntries(directory).ToArray();
        List<string> formattedList = new List<string>();
        
        foreach (var rel in _relativeDirectoryReferences)
        {
            formattedList.Add(rel);
        }

        foreach (var entry in targetDirList)
        {
            formattedList.Add(FormatFileSystemEntry(entry));
        }

        listView.SetSource(formattedList);
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

        string selectedValue = args.Value.ToString()!;
        string fullPath = GetFullPathFromFormatted(selectedValue, currentDir);

        if (fullPath == "..")
        {
            string? parentDir = Directory.GetParent(currentDir)?.ToString();
            if (parentDir is not null)
            {
                currentDir = parentDir;
            }
        }
        else if (SvcUtils.IsDirectory(fullPath))
        {
            currentDir = fullPath;
        }
        else
        {
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = fullPath, UseShellExecute = true };
            Process.Start(startInfo);
            return;
        }

        _currentDirs[listViewId] = currentDir;
        UpdateListViewSource(listView, currentDir);
        OnSourceChanged(listViewId, currentDir);
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

        UpdateListViewSource(newListView, currentDir);

        newListView.OpenSelectedItem += HandleOpenSelectedItem;
        newListView.Enter += (focusEventArgs) => {
            OnFocusChanged(id);
        };

        _listViews.Add(newListView);
        return newListView;
    }
}