using Terminal.Gui;
using Microsoft.Extensions.Configuration;
using SplitViewCommander.Elements;
using SplitViewCommander.Models;
using Microsoft.Extensions.DependencyInjection;
using SplitViewCommander.Services;

#region Configuration
var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
var config = configuration.Build();
#endregion

#region Services
ServiceCollection services = new ServiceCollection();
services.AddSingleton<FKeyActions>();
ServiceProvider sp = services.BuildServiceProvider();
FKeyActions functionButtonMethods = sp.GetService<FKeyActions>();
#endregion

#region Settings
string currentLeftDir = config.GetSection("ConfigStrings:CurrentLeftDir").Value!;
string currentRightDir = config.GetSection("ConfigStrings:CurrentRightDir").Value!;
string[] relativeDirectoryReferences = new string[] { ".." };
string[] leftDirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
string[] rightDirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
int buttonsYPos = 47;
int buttonsStartPosX = 5;
int buttonsRightMargin = 15;
#endregion

#region Init SVC
Application.Init();
Window win = new Windows().GetMainWindow();
Terminal.Gui.MenuBar menu = new SplitViewCommander.Elements.MenuBar().GetMenuBar();
#endregion

#region Directory Panels
ListViews listViews = new();
ListView leftListView = listViews.GetListView(currentLeftDir, relativeDirectoryReferences, Pos.Percent(0), Pos.Percent(0), "leftView");
ListView rightListView = listViews.GetListView(currentRightDir, relativeDirectoryReferences, Pos.Percent(45), Pos.Percent(0), "rightView");

win.Add(leftListView);
win.Add(rightListView);
#endregion

#region Function Buttons
List<FunctionKeyButton> buttons = new Buttons().GetButtons(functionButtonMethods, buttonsStartPosX, buttonsYPos, buttonsRightMargin);
foreach (FunctionKeyButton button in buttons)
{
    win.Add(button.Button);
}
#endregion

Application.Top.KeyDown += OnKeyDown;
void OnKeyDown(View.KeyEventEventArgs args)
{
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