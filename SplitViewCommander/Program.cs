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
#endregion

#region Init SVC
Application.Init();
Window win = new SplitViewCommander.Elements.Windows().GetMainWindow();
MenuBar menu = new SplitViewCommander.Elements.MenuBar().GetMenuBar();
#endregion

#region Directory Panels
ListView leftListView = new SplitViewCommander.Elements.ListViews().GetListView(leftFiles, currentLeftDir, relativeDirectoryReferences, Pos.Percent(0), Pos.Percent(0));
ListView rightListView = new SplitViewCommander.Elements.ListViews().GetListView(leftFiles, currentLeftDir, relativeDirectoryReferences, Pos.Percent(45), Pos.Percent(0));
#endregion

win.Add(leftListView);
win.Add(rightListView);

#region F Buttons
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