using Terminal.Gui;

namespace SplitViewCommander.Elements
{
    public class InfoFields : TextField
    {
        private Dim _height { get; } = Dim.Percent(95);
        private Dim _width { get; } = Dim.Percent(44);

        public TextField GetInfoField(Pos XPos, Pos YPos, string id) {
            TextField infoField = new TextField();
            infoField.Id = id;
            infoField.X = XPos;
            infoField.Y = YPos;
            infoField.Width = _width;
            infoField.Height = _height;
            infoField.Text = "test text";
            infoField.CanFocus = false;

            return infoField;
        }
    }
}
