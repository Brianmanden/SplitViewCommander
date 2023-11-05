﻿using SplitViewCommander.Models;
using System.Diagnostics;
using Terminal.Gui;

namespace SplitViewCommander.Elements;

public class Buttons
{
    public List<FunctionKeyButton> GetButtons(int buttonsYPos)
    {
        List<FunctionKeyButton> buttons = new List<FunctionKeyButton>
        {
            new FunctionKeyButton{ Key = Key.F3,    Button = new Button(10, buttonsYPos, "_F3 View", false),    ButtonAction = () => { Debug.WriteLine("F3");}},
            new FunctionKeyButton{ Key = Key.F4,    Button = new Button(25, buttonsYPos, "_F4 Edit", false),    ButtonAction = () => { Debug.WriteLine("F4");} },
            new FunctionKeyButton{ Key = Key.F5,    Button = new Button(40, buttonsYPos, "_F5 Copy", false),    ButtonAction = () => { Debug.WriteLine("F5");} },
            new FunctionKeyButton{ Key = Key.F6,    Button = new Button(55, buttonsYPos, "_F6 Move", false),    ButtonAction = () => { Debug.WriteLine("F6");} },
            new FunctionKeyButton{ Key = Key.F7,    Button = new Button(70, buttonsYPos, "_F7 Dir", false),     ButtonAction = () => { Debug.WriteLine("F7");} },
            new FunctionKeyButton{ Key = Key.F8,    Button = new Button(85, buttonsYPos, "_F8 Del", false),     ButtonAction = () => { Debug.WriteLine("F8");} },
            new FunctionKeyButton{ Key = Key.F10,   Button = new Button(100, buttonsYPos, "_F10 Quit", false),  ButtonAction = () => { Debug.WriteLine("F10"); Application.RequestStop(); } },
        };
     
        foreach (FunctionKeyButton button in buttons)
        {
            button.Button.CanFocus = false;
        };

        return buttons;//.AsEnumerable();
    }
}