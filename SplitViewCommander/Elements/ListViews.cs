using SplitViewCommander.Models;
using SplitViewCommander.Services;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Terminal.Gui;

namespace SplitViewCommander.Elements;

public class ListViews : ListView
{
    private class HighlightedDataSource : IListDataSource
    {
        private readonly List<string> _items;
        private readonly List<List<int>> _matches;
        private readonly BitArray _marks;

        public HighlightedDataSource(List<string> items, List<List<int>> matches)
        {
            _items = items;
            _matches = matches;
            _marks = new BitArray(items.Count);
        }

        public int Count => _items.Count;
        public int Length => _items.Count > 0 ? _items.Max(i => i.Length) : 0;

        public bool IsMarked(int item) => item >= 0 && item < _marks.Length && _marks[item];
        public void SetMark(int item, bool value) 
        {
            if (item >= 0 && item < _marks.Length) _marks[item] = value;
        }

        public void Render(ListView container, ConsoleDriver driver, bool selected, int item, int col, int line, int width, int start)
        {
            container.Move(col, line);
            string text = _items[item];
            List<int> itemMatches = _matches[item];
            
            Terminal.Gui.Attribute normal = selected ? container.ColorScheme.Focus : container.ColorScheme.Normal;
            Terminal.Gui.Attribute highlighted = new Terminal.Gui.Attribute(Color.BrightCyan, selected ? Color.BrightBlue : Color.Blue);

            for (int i = 0; i < text.Length; i++)
            {
                if (itemMatches.Contains(i))
                {
                    driver.SetAttribute(highlighted);
                }
                else
                {
                    driver.SetAttribute(normal);
                }
                driver.AddRune(text[i]);
            }

            driver.SetAttribute(normal);
            for (int i = text.Length; i < width; i++)
            {
                driver.AddRune(' ');
            }
        }

        public System.Collections.IList ToList() => _items;
    }

    private static bool IsFuzzyMatch(string text, string filter, out List<int> matchIndices)
    {
        matchIndices = new List<int>();
        if (string.IsNullOrEmpty(filter)) return true;
        
        int textIdx = 0;
        for (int filterIdx = 0; filterIdx < filter.Length; filterIdx++)
        {
            char c = char.ToLower(filter[filterIdx]);
            bool found = false;
            while (textIdx < text.Length)
            {
                if (char.ToLower(text[textIdx]) == c)
                {
                    matchIndices.Add(textIdx);
                    textIdx++;
                    found = true;
                    break;
                }
                textIdx++;
            }
            if (!found) return false;
        }
        return true;
    }

    private static List<ListView>? _listViews { get; set; } = new List<ListView>();
    private static Dictionary<string, string> _currentDirs = new Dictionary<string, string>();
    private static Dictionary<string, string> _filterTexts = new Dictionary<string, string>();
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
        var activeListView = GetActiveListView();
        if (activeListView != null)
        {
            string listViewId = activeListView.Id.ToString();
            string currentDir = _currentDirs[listViewId];
            UpdateListViewSource(activeListView, currentDir);
        }
    }

    private void UpdateListViewSource(ListView listView, string directory)
    {
        string listViewId = listView.Id.ToString();
        string filter = _filterTexts.ContainsKey(listViewId) ? _filterTexts[listViewId] : "";

        string[] targetDirList = Directory.EnumerateFileSystemEntries(directory).ToArray();
        List<string> filteredItems = new List<string>();
        List<List<int>> matches = new List<List<int>>();
        
        foreach (var rel in _relativeDirectoryReferences)
        {
            filteredItems.Add(rel);
            matches.Add(new List<int>()); // No matches for ".."
        }

        foreach (var entry in targetDirList)
        {
            string formatted = FormatFileSystemEntry(entry);
            string compareName = formatted.StartsWith("/") ? formatted.Substring(1) : formatted;
            
            if (IsFuzzyMatch(compareName, filter, out List<int> itemIndices))
            {
                // If it's a directory, adjust indices to account for the leading '/'
                if (formatted.StartsWith("/"))
                {
                    for (int i = 0; i < itemIndices.Count; i++) itemIndices[i]++;
                }
                
                filteredItems.Add(formatted);
                matches.Add(itemIndices);
            }
        }

        listView.Source = new HighlightedDataSource(filteredItems, matches);
        OnSourceChanged(listViewId, string.IsNullOrEmpty(filter) ? directory : $"{directory} [Filter: {filter}]");
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

        // Clear filter on navigation
        _filterTexts[listViewId] = "";

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

    private void HandleKeyDown(ListView listView, View.KeyEventEventArgs args)
    {
        string listViewId = listView.Id.ToString();
        string currentFilter = _filterTexts.ContainsKey(listViewId) ? _filterTexts[listViewId] : "";

        if (args.KeyEvent.Key == Key.Esc)
        {
            if (!string.IsNullOrEmpty(currentFilter))
            {
                _filterTexts[listViewId] = "";
                UpdateListViewSource(listView, _currentDirs[listViewId]);
                args.Handled = true;
            }
        }
        else if (args.KeyEvent.Key == Key.Backspace)
        {
            if (!string.IsNullOrEmpty(currentFilter))
            {
                _filterTexts[listViewId] = currentFilter.Substring(0, currentFilter.Length - 1);
                UpdateListViewSource(listView, _currentDirs[listViewId]);
                args.Handled = true;
            }
        }
        else if (char.IsLetterOrDigit((char)args.KeyEvent.KeyValue) || args.KeyEvent.Key == Key.Space)
        {
            _filterTexts[listViewId] = currentFilter + (char)args.KeyEvent.KeyValue;
            UpdateListViewSource(listView, _currentDirs[listViewId]);
            args.Handled = true;
        }
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
        newListView.KeyDown += (args) => HandleKeyDown(newListView, args);
        newListView.Enter += (focusEventArgs) => {
            OnFocusChanged(id);
        };

        _listViews.Add(newListView);
        return newListView;
    }
}