using SplitViewCommander.Services;
using System.Diagnostics;
using Terminal.Gui;
using Microsoft.Extensions.Configuration;

#region Configuration
var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
var config = configuration.Build();
#endregion

#region Settings
string currentLeftDir = config.GetSection("ConfigStrings:CurrentLeftDir").Value!;
string currentRightDir = config.GetSection("ConfigStrings:CurrentRightDir").Value!;
string[] relativeDirectoryReferences = new string[] { ".." };
string[] leftDirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
string[] leftFiles = SvcUtils.ConcatArrays(relativeDirectoryReferences, leftDirFileList);
string[] rightDirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
string[] rightFiles = SvcUtils.ConcatArrays(relativeDirectoryReferences, rightDirFileList);
int buttonsYPos = 47;
#endregion

#region Init SVC
Application.Init();
Window win = new SplitViewCommander.Elements.Windows().GetMainWindow();
MenuBar menu = new SplitViewCommander.Elements.MenuBar().GetMenuBar();
#endregion

#region Directory Panels
SplitViewCommander.Elements.ListViews listViews = new();
ListView leftListView = listViews.GetListView(leftFiles, currentLeftDir, relativeDirectoryReferences, Pos.Percent(0), Pos.Percent(0));
ListView rightListView = listViews.GetListView(leftFiles, currentLeftDir, relativeDirectoryReferences, Pos.Percent(45), Pos.Percent(0));

win.Add(leftListView);
win.Add(rightListView);
#endregion

#region Function Buttons
List<Button> buttons = new SplitViewCommander.Elements.Buttons().GetButtons(buttonsYPos);
foreach (Button button in buttons)
{
    win.Add(button);
}
#endregion

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