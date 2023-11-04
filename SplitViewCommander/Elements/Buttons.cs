using Terminal.Gui;

namespace SplitViewCommander.Elements
{
    public class Buttons
    {
        public List<Button> GetButtons(int buttonsYPos) {
            List<Button> buttons = new List<Button>
            {
                new Button(10, buttonsYPos, "_F3 View", false),
                new Button(25, buttonsYPos, "_F4 Edit", false),
                new Button(40, buttonsYPos, "_F5 Copy", false),
                new Button(55, buttonsYPos, "_F6 Move", false),
                new Button(70, buttonsYPos, "_F7 Dir", false),
                new Button(85, buttonsYPos, "_F8 Del", false),
                new Button(100, buttonsYPos, "_F10 Quit", false),
            };

            foreach (Button button in buttons)
            {
                button.CanFocus = false;
            };
            
            return buttons;
        }
    }
}
