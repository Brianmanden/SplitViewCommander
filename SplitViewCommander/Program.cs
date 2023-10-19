using SplitViewCommander;
using System;
using System.Data;
using System.Data.SqlClient;
using Terminal.Gui;

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

string theNow = DateTime.Now.ToString("HH:mm:ss");

var menu = new MenuBar(new MenuBarItem[] {
            new MenuBarItem (
                "_File", new MenuItem [] {
                    new MenuItem ("_Quit", "", () => { Application.RequestStop(); })
            }),
            new MenuBarItem (
                "_Table", new MenuItem [] {
                    new MenuItem ("_Add Table", "", () => { win.Add(tableView); }),
                    new MenuItem ("_Close Table", "", () => { win.Remove(tableView); }),
            }),
            new MenuBarItem (
                "_Mark", new MenuItem [] {
                    new MenuItem ("_Quit", "", () => { Application.RequestStop(); })
            }),
            new MenuBarItem (
                "_Options", new MenuItem [] {
                    new MenuItem ("_Quit", "", () => { Application.RequestStop(); })
            }),
        });



// Add both menu and win in a single call
//Application.Top.Add(menu, win, tableView);
Application.Top.Add(menu, win);
Application.Run();
Application.Shutdown();


