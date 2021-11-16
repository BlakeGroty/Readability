using Microsoft.Win32;
using Readability.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Readability
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadRecents();
            RefreshRecentsBox();
        }

        protected override void OnSourceInitialized(EventArgs e)
            => RemoveIconHelper.RemoveIcon(this);

        private void RefreshRecentsBox(bool saveInSettings = true)
        {
            TextBlock_NoRecentsFound.Visibility = ListBox_Recents.HasItems ? Visibility.Collapsed : Visibility.Visible;
            if(saveInSettings)
            {
                StringCollection sc = new StringCollection();
                sc.AddRange(ListBox_Recents.Items.Cast<string>().ToArray());
                Settings.Default.RecentsList = sc;
                Settings.Default.Save();
            }
        }

        private void LoadRecents()
        {
            if(Settings.Default.RecentsList != null)
                foreach(string s in Settings.Default.RecentsList)
                    ListBox_Recents.Items.Add(s);
        }

        private void Button_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = Settings.Default.MultiFileAnalysisExtension;
            string fileExtensions = $"*.{Settings.Default.SingleFileAnalysisExtension};*.{Settings.Default.MultiFileAnalysisExtension}";
            fileDialog.Filter = $"Writing Analysis Files ({fileExtensions})|{fileExtensions}";
            fileDialog.Multiselect = false;

            bool? result = fileDialog.ShowDialog(this);
            if(result == true)
            {
                string file = fileDialog.FileName;
                if(Settings.Default.RecentsList.Contains(file))
                    Settings.Default.RecentsList.Remove(file);
                Settings.Default.RecentsList.Add(file);
                Settings.Default.Save();

                string fileExt = file.Substring(file.LastIndexOf('.') + 1);
                if(fileExt.Equals(Settings.Default.SingleFileAnalysisExtension))
                {
                    SingleFileAnalysis newWindow = new SingleFileAnalysis();
                    newWindow.Activate();
                    Debug.WriteLine("Activated new window");
                }
                else if(fileExt.Equals(Settings.Default.MultiFileAnalysisExtension))
                {
                    throw new NotImplementedException("Multi file analysis not yet implemented");
                }
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
    }
}
