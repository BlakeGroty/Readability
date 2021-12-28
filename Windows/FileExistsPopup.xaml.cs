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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Readability.Windows
{
    /// <summary>
    /// Interaction logic for FileExistsPopup.xaml
    /// </summary>
    public partial class FileExistsPopup : Window
    {
        public string Action { get; private set; }
        public string NewName { get; private set; }

        public FileExistsPopup(string file)
        {
            InitializeComponent();
            Title = $"{Settings.Default.AppName} - File already exists";
            Action = "none";
            NewName = "none";
            DockPanel_Rename.Visibility = Visibility.Collapsed;
            TextBlock_Error.Visibility = Visibility.Hidden;

            // Truncate file name to fix issues with TextBlock height
            string name = Path.GetFileName(file);
            TextBlock messageWidthTextBlock = new TextBlock()
            {
                FontSize = TextBlock_Message.FontSize,
                FontWeight = TextBlock_Message.FontWeight,
                FontFamily = TextBlock_Message.FontFamily,
                FontStyle = TextBlock_Message.FontStyle,
                Text = name
            };
            messageWidthTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double messageWidth = messageWidthTextBlock.DesiredSize.Width;

            double widthThreshold = 0.30 * Width;
            if(messageWidth > widthThreshold)
                name = name.Substring(0, (int)(widthThreshold * name.Length / messageWidth)) + "\u2026";
            TextBlock_Message.Text = $"A file named \"{name}\" has already been imported. What would you like to do?"; ;
        }

        private void Button_Rename_Click(object sender, RoutedEventArgs e)
        {
            DockPanel_Rename.Visibility = Visibility.Visible;
            Button_Confirm.IsTabStop = true;
            Button_Confirm.TabIndex = 4;
        }

        private void Button_Overwrite_Click(object sender, RoutedEventArgs e)
        {
            Action = "overwrite";
            DialogResult = true;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Action = "cancel";
            DialogResult = false;
            Close();
        }

        private void Button_Confirm_Click(object sender, RoutedEventArgs e)
        {
            Action = "rename";
            string newName = TextBox_NewName.Text.Trim();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Settings.Default.AnalysisFolderName, newName);
            bool isValidName = !string.IsNullOrEmpty(newName) && 
                               newName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
            bool fileExists = File.Exists(path);
            
            if(isValidName && !fileExists)
            {
                NewName = newName;
                DialogResult = true;
                Close();
            }
            else
            {
                Button_Confirm.BorderBrush = Brushes.Red;
                if(!isValidName)
                    TextBlock_Error.Text = "Invalid file name. Please make sure your entry is not empty and does not contain any of the following characters: \\/:*?\"<>|";
                if(fileExists)
                    TextBlock_Error.Text = "File already exists. Please choose a different name or option.";
                TextBlock_Error.Visibility = Visibility.Visible;
            }
        }

        private void TextBox_NewName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBlock_Error.Visibility = Visibility.Hidden;
            Button_Confirm.BorderBrush = new SolidColorBrush(Color.FromRgb(112, 112, 112));
        }
    }
}
