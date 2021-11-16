using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Readability.Windows
{
    /// <summary>
    /// Interaction logic for SingleFileAnalysis.xaml
    /// </summary>
    public partial class SingleFileAnalysis : Window
    {
        public string File { get; private set; }

        public SingleFileAnalysis()
            => InitializeComponent();

        public SingleFileAnalysis(string file) : this()
            => File = file;
    }
}
