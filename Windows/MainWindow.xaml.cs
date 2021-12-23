using Microsoft.Win32;
using Readability.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Readability
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string analysisPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Settings.Default.AnalysisFolderName);
        private static FileSystemWatcher watcher;

        public MainWindow()
        {
            InitializeComponent();

            if(!Directory.Exists(analysisPath))
                Directory.CreateDirectory(analysisPath);
            
            LoadFileBox();

            watcher = new FileSystemWatcher(analysisPath);
            watcher.NotifyFilter = NotifyFilters.Attributes |
                                   NotifyFilters.CreationTime |
                                   NotifyFilters.FileName |
                                   NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite |
                                   NotifyFilters.Size |
                                   NotifyFilters.Security;
            watcher.Changed += AnalysisPathFiles_Updated;
            watcher.Deleted += AnalysisPathFiles_Updated;
            watcher.Renamed += AnalysisPathFiles_Updated;
            watcher.Error += Watcher_Error;

            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            watcher.Dispose();
            Debug.WriteLine("File watcher encountered an error");
        }

        private void AnalysisPathFiles_Updated(object sender, FileSystemEventArgs e)
            => Dispatcher.Invoke(() => LoadFileBox());

        protected override void OnSourceInitialized(EventArgs e)
            => RemoveIconHelper.RemoveIcon(this);

        /// <summary>
        /// Adds all files in program folder with analysis extensions to the ListBox
        /// </summary>
        private void LoadFileBox()
        {
            ListBox_Files.Items.Clear();
            IEnumerable<string> files = Directory.GetFiles(analysisPath).Select(f => f.Substring(f.LastIndexOf("\\") + 1));
            foreach(string file in files)
                if(file.EndsWith(Settings.Default.SingleFileAnalysisExtension) || file.EndsWith(Settings.Default.MultiFileAnalysisExtension))
                    ListBox_Files.Items.Add(file);
            TextBlock_NoRecentsFound.Visibility = ListBox_Files.HasItems ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {
            string fileExtensions = $"*.{Settings.Default.SingleFileAnalysisExtension};*.{Settings.Default.MultiFileAnalysisExtension}";
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                DefaultExt = Settings.Default.MultiFileAnalysisExtension,
                Filter = $"Writing Analysis Files ({fileExtensions})|{fileExtensions}",
                Multiselect = false
            };

            bool? result = fileDialog.ShowDialog(this);
            if(result == true)
            {
                string file = fileDialog.FileName;
                
                // TODO: Copy file to program directory

                string fileExt = file.Substring(file.LastIndexOf('.') + 1);
                if(fileExt.Equals(Settings.Default.SingleFileAnalysisExtension))
                {
                    SingleFileAnalysis newWindow = new SingleFileAnalysis();
                    newWindow.Activate();
                    Debug.WriteLine("Activated new window");
                }
                else if(fileExt.Equals(Settings.Default.MultiFileAnalysisExtension))
                    throw new NotImplementedException("Multi file analysis not yet implemented");
                else
                    throw new ArgumentException("File does not have valid extension");
                Close();
            }
        }

        private void Button_SingleAnalysis_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_NewAnalysis_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListBox_Files_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
