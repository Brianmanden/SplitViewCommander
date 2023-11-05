using System.Diagnostics;
using Terminal.Gui;
using Microsoft.Extensions.Configuration;
using SplitViewCommander.Elements;
using SplitViewCommander.Models;

#region Configuration
var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
var config = configuration.Build();
#endregion

#region Settings
string currentLeftDir = config.GetSection("ConfigStrings:CurrentLeftDir").Value!;
string currentRightDir = config.GetSection("ConfigStrings:CurrentRightDir").Value!;
string[] relativeDirectoryReferences = new string[] { ".." };
string[] leftDirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
string[] rightDirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
int buttonsYPos = 47;
#endregion

#region Init SVC
Application.Init();
Window win = new SplitViewCommander.Elements.Windows().GetMainWindow();
Terminal.Gui.MenuBar menu = new SplitViewCommander.Elements.MenuBar().GetMenuBar();
#endregion

#region Directory Panels
SplitViewCommander.Elements.ListViews listViews = new();
ListView leftListView = listViews.GetListView(currentLeftDir, relativeDirectoryReferences, Pos.Percent(0), Pos.Percent(0), "leftView");
ListView rightListView = listViews.GetListView(currentRightDir, relativeDirectoryReferences, Pos.Percent(45), Pos.Percent(0), "rightView");

win.Add(leftListView);
win.Add(rightListView);
#endregion

#region Function Buttons
List<FunctionKeyButton> buttons = new SplitViewCommander.Elements.Buttons().GetButtons(buttonsYPos);
foreach (FunctionKeyButton button in buttons)
{
    win.Add(button.Button);
}
#endregion

Application.Top.KeyDown += OnKeyDown;
void OnKeyDown(View.KeyEventEventArgs args)
{
    //Debug.WriteLine($"{args.KeyEvent.Key} pressed");

    FunctionKeyButton functionKey = buttons.Where(btn => btn.Key! == args.KeyEvent.Key).FirstOrDefault();
    if (functionKey != null)
    {
        functionKey.ButtonAction.Invoke();
    }
}

// Add both menu and win in a single call
Application.Top.Add(menu, win);
Application.Run();
Application.Shutdown();