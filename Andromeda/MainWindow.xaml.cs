using Andromeda.GameProject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Andromeda
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string AndromedaPath { get; private set; } = @"D:\Development\GameEngine\Andromeda\";
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;

        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            Closing -= OnMainWindowClosing;
            Project.Current?.Unload();
        }
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            GetEnginePath();
            OpenProjectBrowserDialog();
        }

        private void OpenProjectBrowserDialog()
        {
            var projectBrowser = new ProjectBrowserDialog();
           if (projectBrowser.ShowDialog() == false  || projectBrowser.DataContext == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Project.Current?.Unload();
                DataContext = projectBrowser.DataContext;
            }
        }

        private void GetEnginePath()
        {
            var enginePath = Environment.GetEnvironmentVariable("ANDROMEDA_ENGINE", EnvironmentVariableTarget.User);
            if (enginePath == null || !Directory.Exists(System.IO.Path.Combine(enginePath, @"Engine\EngineAPI")))
            {
                var dlg = new EnginePathDialog();
                if (dlg.ShowDialog() == true)
                {
                    AndromedaPath = dlg.AndromedaPath;
                    Environment.SetEnvironmentVariable("ANDROMEDA_ENGINE", AndromedaPath.ToUpper(), EnvironmentVariableTarget.User);
                }
            }
            else
            {
                AndromedaPath = enginePath;
            }
        }
    }
}
