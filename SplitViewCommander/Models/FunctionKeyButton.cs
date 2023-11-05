using Terminal.Gui;

namespace SplitViewCommander.Models;

public class FunctionKeyButton
{
    public Action ButtonAction { get; set; }
    public Button Button { get; set; }
    public Key Key { get; set; }
}
