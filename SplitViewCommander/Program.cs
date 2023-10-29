using SplitViewCommander.Services;
using System.Diagnostics;
using Terminal.Gui;

SvcUtils utils = new SvcUtils();

Application.Init();
Window win = new Window("Split View Commander")
{
    X = 0,
    Y = 1,
    Width = Dim.Fill(),
    Height = Dim.Fill(),
};
MenuBar menu = new MenuBar(new MenuBarItem[] {
            new MenuBarItem ( "_File", new MenuItem [] { new MenuItem ("_Quit", "", () => { Application.RequestStop(); }) }),
            new MenuBarItem ( "_Mark", new MenuItem [] { new MenuItem ("_Quit", "", () => { Application.RequestStop(); }) }),
            new MenuBarItem ( "_Options", new MenuItem [] { new MenuItem ("_Quit", "", () => { Application.RequestStop(); }) }),
        });

string currentLeftDir = @"C:\Users\Brian\Desktop";
string currentRightDir = Directory.GetCurrentDirectory().ToString();

string[] relativeDirectoryReferences = new string[] { ".." };
string[] leftDirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
string[] leftFiles = utils.ConcatArrays(relativeDirectoryReferences, leftDirFileList);
string[] rightDirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
string[] rightFiles = utils.ConcatArrays(relativeDirectoryReferences, rightDirFileList);

ListView leftListView = new ListView(leftFiles){ Width = Dim.Percent(45), Height = Dim.Percent(45), X = Pos.Percent(0), Y = Pos.Percent(0), AllowsMarking = true, AllowsMultipleSelection = true };
ListView rightListView = new ListView(rightFiles) { Width = Dim.Percent(45), Height = Dim.Percent(45), X = Pos.Percent(51), Y = Pos.Percent(0), AllowsMarking = true, AllowsMultipleSelection = true };
leftListView.OpenSelectedItem += LeftListViewHandleOpenSelectedItem;
rightListView.OpenSelectedItem += RightListViewHandleOpenSelectedItem;

void LeftListViewHandleOpenSelectedItem(ListViewItemEventArgs args)
{
    string filePath = args.Value.ToString()!;
    bool isDir = utils.IsDirectory(filePath);

    // If dir - navigate into it
    if (isDir)
    {
        if (".." == filePath)
        {
            string? parentDir = Directory.GetParent(currentLeftDir)?.ToString();
            if (parentDir is not null)
            {
                currentLeftDir = parentDir;
            }
        }
        else
        {
            currentLeftDir = filePath;
        }
    }
    // if not dir - assuming file and opening it
    else
    {
        ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = filePath, UseShellExecute = true };
        Process.Start(startInfo);
    }

    string[] dirsAndFiles = utils.ConcatArrays(relativeDirectoryReferences, Directory.EnumerateFileSystemEntries(currentLeftDir).ToArray());
    
    leftListView.SetSource(dirsAndFiles);
}
void RightListViewHandleOpenSelectedItem(ListViewItemEventArgs args)
{
    string filePath = args.Value.ToString()!;
    bool isDir = utils.IsDirectory(filePath);

    // If dir - navigate into it
    if (isDir)
    {
        if (".." == filePath)
        {
            string? parentDir = Directory.GetParent(currentRightDir)?.ToString();
            if (parentDir is not null)
            {
                currentRightDir = parentDir;
            }
        }
        else
        {
            currentRightDir = filePath;
        }
    }
    // if not dir - assuming file and opening it
    else
    {
        ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = filePath, UseShellExecute = true };
        Process.Start(startInfo);
    }

    string[] dirsAndFiles = utils.ConcatArrays(relativeDirectoryReferences, Directory.EnumerateFileSystemEntries(currentRightDir).ToArray());

    rightListView.SetSource(dirsAndFiles);
}

win.Add(leftListView);
win.Add(rightListView);



Button f3Button = new Button(10, 25, "F3 View", false);
Button f4Button = new Button(25, 25, "F4 Edit", false);
Button f10Button = new Button(40, 25, "_F10 Quit", false);

win.Add(f3Button);
win.Add(f4Button);
win.Add(f10Button);



StatusBar statusBar = new StatusBar();
win.Add(statusBar);



Application.Top.KeyDown += OnKeyDown;
void OnKeyDown(View.KeyEventEventArgs args)
{
    Debug.WriteLine($"{args.KeyEvent.Key} pressed");

    if (args.KeyEvent.Key == Key.F10)
    {
        Application.RequestStop();
    }
}



// Add both menu and win in a single call
//Application.Top.Add(menu, win, tableView);
Application.Top.Add(menu, win);
Application.Run();
Application.Shutdown();