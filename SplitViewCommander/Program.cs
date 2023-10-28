using SplitViewCommander.Services;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Terminal.Gui;

SvcUtils utils = new SvcUtils();

Application.Init();
var win = new Window("Hello")
{
    X = 0,
    Y = 1,
    Width = Dim.Fill(),
    Height = Dim.Fill(),
};

#region DataTable
var dt = new DataTable();
using (var con = new SqlConnection("server=(localdb)\\MSSQLLocalDb; database = BooksDB; integrated security = true; encrypt = false;"))
{
    con.Open();
    var cmd = new SqlCommand("SELECT * FROM Books;", con);
    var adapter = new SqlDataAdapter(cmd);

    adapter.Fill(dt);
}

var tableView = new TableView()
{
    X = 30,
    Y = 20,
    Width = 150,
    Height = 10,
};
tableView.Table = dt;
#endregion

var menu = new MenuBar(new MenuBarItem[] {
            new MenuBarItem ( "_File", new MenuItem [] { new MenuItem ("_Quit", "", () => { Application.RequestStop(); })            }),
            new MenuBarItem ( "_Table", new MenuItem [] { new MenuItem ("_Add Table", "", () => { win.Add(tableView); }), new MenuItem ("_Close Table", "", () => { win.Remove(tableView); }),}),
            new MenuBarItem ( "_Mark", new MenuItem [] { new MenuItem ("_Quit", "", () => { Application.RequestStop(); }) }),
            new MenuBarItem ( "_Options", new MenuItem [] { new MenuItem ("_Quit", "", () => { Application.RequestStop(); }) }),
        });

var listSource  = new List<string>{ "123", "324", "123", "324", "123", "324", };

string currentLeftDir = @"C:\Users\Brian\Desktop";
string currentRightDir = Directory.GetCurrentDirectory().ToString();

string[] relDirRefs = new string[] { ".." };
string[] dirFileList = Directory.GetFileSystemEntries(currentLeftDir, "*", SearchOption.TopDirectoryOnly);
string[] files1 = utils.ConcatArrays(relDirRefs, dirFileList);

// Border WIP
//Border listViewBorder = new Border {
//    Background = Color.BrightCyan,
//    BorderBrush = Color.Red,
//    BorderStyle = BorderStyle.Single,
//    BorderThickness = new Thickness(13),
//    DrawMarginFrame = true
//};

var listView = new ListView(files1){ Width = Dim.Percent(45), Height = Dim.Percent(45), X = Pos.Percent(0), Y = Pos.Percent(0), AllowsMarking = true, AllowsMultipleSelection = true };
var listView2 = new ListView() { Width = Dim.Percent(45), Height = Dim.Percent(45), X = Pos.Percent(51), Y = Pos.Percent(0), AllowsMultipleSelection = true };
listView.OpenSelectedItem += HandleOpenSelectedItem;

void HandleOpenSelectedItem(ListViewItemEventArgs args)
{
    string filePath = args.Value.ToString();
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

    string[] dirsAndFilesAsList = Directory.EnumerateFileSystemEntries(currentLeftDir).ToArray();
    
    //listView.Clear();
    listView.SetSource(utils.ConcatArrays(relDirRefs, dirsAndFilesAsList));
}

win.Add(listView);
win.Add(listView2);

// Add both menu and win in a single call
//Application.Top.Add(menu, win, tableView);
Application.Top.Add(menu, win);
Application.Run();
Application.Shutdown();


