using Terminal.Gui;

namespace SplitViewCommander.Elements;

public class Windows
{
    public Window GetMainWindow()
    {
        Window mainWindow = new Window("Split View Commander") { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() };

        return mainWindow;
    }
}
