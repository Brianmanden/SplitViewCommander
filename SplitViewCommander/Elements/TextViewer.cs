using Terminal.Gui;
using System.IO;

namespace SplitViewCommander.Elements;

public class TextViewer
{
    public void Show(string title, string filePath)
    {
        var dialog = new Dialog()
        {
            Title = title,
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 2
        };
        
        var textView = new TextView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 1,
            ReadOnly = true,
            Text = File.ReadAllText(filePath)
        };

        var closeButton = new Button("Close", is_default: true);
        closeButton.Clicked += () => {
            Application.RequestStop();
        };

        dialog.Add(textView, closeButton);
        
        closeButton.X = Pos.Center();
        closeButton.Y = Pos.AnchorEnd(1);

        Application.Run(dialog);
    }
}