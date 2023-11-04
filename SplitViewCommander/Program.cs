using SplitViewCommander.Services;
using System.Diagnostics;
using Terminal.Gui;

Application.Init();
Window win = new SplitViewCommander.Elements.Windows().GetMainWindow();
MenuBar menu = new SplitViewCommander.Elements.MenuBar().GetMenuBar();

#region Directory Panels
string currentLeftDir = @"C:\Users\Brian\Desktop";
string currentRightDir = Directory.GetCurrentDirectory().ToString();

string[] relativeDirectoryReferences = new string[] { ".." };
string[] leftDirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
string[] leftFiles = SvcUtils.ConcatArrays(relativeDirectoryReferences, leftDirFileList);
string[] rightDirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
string[] rightFiles = SvcUtils.ConcatArrays(relativeDirectoryReferences, rightDirFileList);

ListView leftListView = new ListView(leftFiles) { Width = Dim.Percent(45), Height = Dim.Percent(45), X = Pos.Percent(0), Y = Pos.Percent(0), AllowsMarking = true, AllowsMultipleSelection = true };
ListView rightListView = new ListView(rightFiles) { Width = Dim.Percent(45), Height = Dim.Percent(45), X = Pos.Percent(51), Y = Pos.Percent(0), AllowsMarking = true, AllowsMultipleSelection = true };
leftListView.OpenSelectedItem += LeftListViewHandleOpenSelectedItem;
rightListView.OpenSelectedItem += RightListViewHandleOpenSelectedItem;
#endregion

void LeftListViewHandleOpenSelectedItem(ListViewItemEventArgs args)
{
    string filePath = args.Value.ToString()!;
    bool isDir = SvcUtils.IsDirectory(filePath);

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

    string[] dirsAndFiles = SvcUtils.ConcatArrays(relativeDirectoryReferences, Directory.EnumerateFileSystemEntries(currentLeftDir).ToArray());
    
    leftListView.SetSource(dirsAndFiles);
}
void RightListViewHandleOpenSelectedItem(ListViewItemEventArgs args)
{
    string filePath = args.Value.ToString()!;
    bool isDir = SvcUtils.IsDirectory(filePath);

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

    string[] dirsAndFiles = SvcUtils.ConcatArrays(relativeDirectoryReferences, Directory.EnumerateFileSystemEntries(currentRightDir).ToArray());

    rightListView.SetSource(dirsAndFiles);
}

win.Add(leftListView);
win.Add(rightListView);

int buttonsYPos = 47;
int btnPadLeft = 15;

Button f3Button = new Button(10, buttonsYPos, "_F3 View", false);
f3Button.CanFocus = false;

Button f4Button = new Button(25, buttonsYPos, "_F4 Edit", false);
f4Button.CanFocus = false;

Button f5Button = new Button(40, buttonsYPos, "_F5 Copy", false);
f5Button.CanFocus = false;

Button f6Button = new Button(55, buttonsYPos, "_F6 Move", false);
f6Button.CanFocus = false;

Button f7Button = new Button(70, buttonsYPos, "_F7 Dir", false);
f7Button.CanFocus = false;

Button f8Button = new Button(85, buttonsYPos, "_F8 Del", false);
f8Button.CanFocus = false;

Button f10Button = new Button(100, buttonsYPos, "_F10 Quit", false);
f10Button.CanFocus = false;

win.Add(f3Button);
win.Add(f4Button);
win.Add(f5Button);
win.Add(f6Button);
win.Add(f7Button);
win.Add(f10Button);

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
Application.Top.Add(menu, win);
Application.Run();
Application.Shutdown();