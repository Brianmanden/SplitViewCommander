using SplitViewCommander.Models;
using System.Diagnostics;
using Terminal.Gui;

namespace SplitViewCommander.Services
{
    public class FKeyActions
    {
        public Action GetAction(Key functionKey)
        {
            var actions = new List<KeyValuePair<Key, Action>>(){
                new KeyValuePair<Key, Action>(Key.F3, () => { Debug.WriteLine("F3"); }),
                new KeyValuePair<Key, Action>(Key.F4, () => { Debug.WriteLine("F4"); }),
                new KeyValuePair<Key, Action>(Key.F5, () => { Debug.WriteLine("F5"); }),
                new KeyValuePair<Key, Action>(Key.F6, () => { Debug.WriteLine("F6"); }),
                new KeyValuePair<Key, Action>(Key.F7, () => { Debug.WriteLine("F7"); }),
                new KeyValuePair<Key, Action>(Key.F8, () => { Debug.WriteLine("F8"); }),
                new KeyValuePair<Key, Action>(Key.F10, () => {
                    Debug.WriteLine("F10");
                    Application.RequestStop();
                }),
            };

            Action functionAction = actions.Where(k => k.Key == functionKey).First().Value;
            return functionAction;
        }
    }
}
