using Andromeda.GameProject;
using Andromeda.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Andromeda.GameDev
{
    /// <summary>
    /// Interaction logic for NewScriptDialog.xaml
    /// </summary>
    public partial class NewScriptDialog : Window
    {
        //strings for C++ files generated in new scripts
        #region C++String 
        private static readonly string _cppCode = @"#include ""{0}.h"" 
namespace {1} {{
            
REGISTER_SCRIPT({0});
 void {0}::begin_play()
 {{
   
 }}

 void {0}::update(float dt)
{{

}}
}} // namespace {1}";

        private static readonly string _hCode = @"#pragma once
namespace {1} {{

class {0} : public andromeda::script::entity_script
{{
public:
    constexpr explicit {0}(andromeda::game_entity::entity entity)
        : andromeda::script::entity_script{{entity}} {{}}
    void begin_play() override;
    void update(float dt) override;
private:
}};
}} //namespace {1}";
        #endregion

        private static readonly string _namespace = GetNameSpaceFromProjectName();
        private static string GetNameSpaceFromProjectName()
        {
            var projectName = Project.Current.Name;
            if (string.IsNullOrEmpty(projectName)) return string.Empty;
            projectName = projectName.Replace(' ', '_');
            return projectName;
        }

        public NewScriptDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            scriptPath.Text = @"GameCode\";
        }
        bool Validate()
        {
            bool isValid = false;
            var name = scriptName.Text.Trim();
            var path = scriptPath.Text.Trim();
            string errorMsg = string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                errorMsg = "Type in a script name.";
            }
            else if (name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1 || name.Any(x => char.IsWhiteSpace(x)))
            {
                errorMsg = "Invalid character(s) used in script name";
            }
            if (string.IsNullOrEmpty(path))
            {
                errorMsg = "Select a valid script folder";
            }
            else if (path.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
            {
                errorMsg = "Invalid character(s) used in script path.";
            }
            else if (!System.IO.Path.GetFullPath(System.IO.Path.Combine(Project.Current.Path, path)).Contains(System.IO.Path.Combine(Project.Current.Path, @"GameCode\"))
                || File.Exists(System.IO.Path.GetFullPath(System.IO.Path.Combine(Project.Current.Path, path), $"{name}.h")))
            {
                errorMsg = "Script must be added to GameCode.";
            }
            else
            {
                isValid = true;
            }
            if (!isValid)
            {
                messageTextBlock.Foreground = FindResource("Editor.RedBrush") as Brush;
            }
            else
            {
                messageTextBlock.Foreground = FindResource("Editor.FontBrush") as Brush;
            }
            messageTextBlock.Text = errorMsg;
            return isValid;
        }

        private async void OnOK_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;
            IsEnabled = false;
            try
            {
                var name = scriptName.Text.Trim();
                var path = Path.GetFullPath(Path.Combine(Project.Current.Path, scriptPath.Text.Trim()));
                var solution = Project.Current.Solution;
                var projectName = Project.Current.Name;
                await Task.Run(() => CreateScript(name, path, solution, projectName));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to create the script {scriptName.Text}");
            }
        }
        private void OnScriptName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Validate()) return;
            var name = scriptName.Text.Trim();
            var project = Project.Current;
            messageTextBlock.Text = $"{name}.h and {name}.cpp will be added to the project"; //{Project.Current.Name}
        }
        private void OnPathName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }

        private void CreateScript(string name, string path, string solution, string projectName)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var cpp = Path.GetFullPath(Path.Combine(path, $"{name}.cpp"));
            var h = Path.GetFullPath(Path.Combine(path, $"{name}.h"));

            using(var sw = File.CreateText(cpp))
            {
                sw.Write(string.Format(_cppCode, name, _namespace));
            }
            using (var sw = File.CreateText(h))
            {
                sw.Write(string.Format(_hCode, name, _namespace));
            }
            string[] files = new string[] { cpp, h };

            for (int i = 0; i < 3; ++i)
            {
                if (!VisualStudio.AddFilesToSolution(solution, projectName, files)) System.Threading.Thread.Sleep(1000);
                else break;
            }
        }
     
        
    }
}
