using SplitViewCommander.Elements;
using SplitViewCommander.Models;
using System.Diagnostics;
using Terminal.Gui;

namespace SplitViewCommander.Services
{
    public class FKeyActions
    {
        private readonly ListViews _listViews;
        private readonly string[] _textExtensions = { ".txt", ".md", ".cs", ".json", ".xml", ".config", ".csproj", ".sln", ".js", ".ts", ".html", ".css", ".py" };

        public FKeyActions(ListViews listViews)
        {
            _listViews = listViews;
        }

        public Action GetAction(Key functionKey)
        {
            var actions = new List<KeyValuePair<Key, Action>>(){
                new KeyValuePair<Key, Action>(Key.F3, () => { ViewFile(); }),
                new KeyValuePair<Key, Action>(Key.F4, () => { Debug.WriteLine("F4"); }),
                new KeyValuePair<Key, Action>(Key.F5, () => { Debug.WriteLine("F5"); }),
                new KeyValuePair<Key, Action>(Key.F6, () => { Debug.WriteLine("F6"); }),
                new KeyValuePair<Key, Action>(Key.F7, () => { CreateDirectory(); }),
                new KeyValuePair<Key, Action>(Key.F8, () => { Debug.WriteLine("F8"); }),
                new KeyValuePair<Key, Action>(Key.F10, () => {
                    Debug.WriteLine("F10");
                    Application.RequestStop();
                }),
            };

            Action functionAction = actions.Where(k => k.Key == functionKey).First().Value;
            return functionAction;
        }

        private void ViewFile()
        {
            string activeDir = _listViews.GetActiveDirectory();
            var activeListView = _listViews.GetActiveListView();
            
            if (activeListView != null && activeListView.SelectedItem >= 0)
            {
                string selectedItem = activeListView.Source.ToList()[activeListView.SelectedItem].ToString()!;
                string fullPath = _listViews.GetFullPathFromFormatted(selectedItem, activeDir);

                if (fullPath == ".." || SvcUtils.IsDirectory(fullPath)) return;

                if (File.Exists(fullPath))
                {
                    string ext = Path.GetExtension(fullPath).ToLower();
                    if (_textExtensions.Contains(ext))
                    {
                        new TextViewer().Show($"Viewing: {Path.GetFileName(fullPath)}", fullPath);
                    }
                    else
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = fullPath, UseShellExecute = true };
                        Process.Start(startInfo);
                    }
                }
            }
        }

        private void CreateDirectory()
        {
            var dialog = new Dialog("Create Directory", 50, 10);
            var label = new Label("Name:") { X = 1, Y = 1 };
            var textField = new TextField("") { X = 7, Y = 1, Width = Dim.Fill() - 1 };
            var okButton = new Button("OK", is_default: true);
            var cancelButton = new Button("Cancel");

            okButton.Clicked += () => {
                string folderName = textField.Text.ToString()!;
                if (SvcUtils.IsValidFolderName(folderName))
                {
                    string activeDir = _listViews.GetActiveDirectory();
                    if (!string.IsNullOrEmpty(activeDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(Path.Combine(activeDir, folderName));
                            _listViews.RefreshActiveListView();
                            Application.RequestStop();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.ErrorQuery("Error", $"Could not create directory: {ex.Message}", "OK");
                        }
                    }
                }
                else
                {
                    MessageBox.ErrorQuery("Invalid Name", "The directory name contains invalid characters or is empty.", "OK");
                }
            };

            cancelButton.Clicked += () => {
                Application.RequestStop();
            };

            dialog.Add(label, textField, okButton, cancelButton);
            
            // Positioning buttons at the bottom
            okButton.X = Pos.Center() - 10;
            okButton.Y = Pos.Bottom(textField) + 2;
            cancelButton.X = Pos.Center() + 2;
            cancelButton.Y = Pos.Bottom(textField) + 2;

            Application.Run(dialog);
        }
    }
}
