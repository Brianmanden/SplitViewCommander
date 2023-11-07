using SplitViewCommander.Models;
using SplitViewCommander.Services;
using Terminal.Gui;

namespace SplitViewCommander.Elements;

public class Buttons
{
    public List<FunctionKeyButton> GetButtons(FKeyActions fKeyActions,int startPosX, int buttonsYPos, int rightMargin)
    {
        List<FunctionKeyButton> buttons = new List<FunctionKeyButton>
        {
            new FunctionKeyButton{ Key = Key.F3,    Button = new Button(0, buttonsYPos, "_F3 View", false),         ButtonAction = fKeyActions.GetAction(Key.F3) },
            new FunctionKeyButton{ Key = Key.F4,    Button = new Button(0, buttonsYPos, "_F4 Edit", false),         ButtonAction = fKeyActions.GetAction(Key.F4) },
            new FunctionKeyButton{ Key = Key.F5,    Button = new Button(0, buttonsYPos, "_F5 Copy", false),         ButtonAction = fKeyActions.GetAction(Key.F5) },
            new FunctionKeyButton{ Key = Key.F6,    Button = new Button(0, buttonsYPos, "_F6 Move/Rename", false),  ButtonAction = fKeyActions.GetAction(Key.F6) },
            new FunctionKeyButton{ Key = Key.F7,    Button = new Button(0, buttonsYPos, "_F7 Directory", false),    ButtonAction = fKeyActions.GetAction(Key.F7) },
            new FunctionKeyButton{ Key = Key.F8,    Button = new Button(0, buttonsYPos, "_F8 Delete", false),       ButtonAction = fKeyActions.GetAction(Key.F8) },
            new FunctionKeyButton{ Key = Key.F10,   Button = new Button(0, buttonsYPos, "_F10 Quit", false),        ButtonAction = fKeyActions.GetAction(Key.F10) },
        };

        int nextXPos = 0;
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].Button.CanFocus = false;
            if (i == 0)
            {
                buttons[i].Button.X = startPosX;
                nextXPos = startPosX + buttons[i].Button.Text.Length + rightMargin;
            }
            else
            {
                buttons[i].Button.X = nextXPos;
                nextXPos = nextXPos + buttons[i].Button.Text.Length + rightMargin;
            }
        }

        return buttons;
    }
}