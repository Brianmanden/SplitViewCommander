using Terminal.Gui;

namespace SplitViewCommander.Elements;

public class MenuBar
{
    public Terminal.Gui.MenuBar GetMenuBar() {
        Terminal.Gui.MenuBar menu = new Terminal.Gui.MenuBar(
            new MenuBarItem[] {
                new MenuBarItem ( "_File", new MenuItem [] { new MenuItem ("_Quit", "", () => { Application.RequestStop(); }) }),
                new MenuBarItem ( "_Mark", new MenuItem [] { new MenuItem ("_Quit", "", () => { Application.RequestStop(); }) }),
                new MenuBarItem ( "_Options", new MenuItem [] { new MenuItem ("_Quit", "", () => { Application.RequestStop(); }) }),
            }
        );

        return menu;
    }
}
