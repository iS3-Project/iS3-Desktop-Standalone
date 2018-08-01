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
using IS3.Core;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for AddDomainWindow.xaml
    /// </summary>
    public partial class AddDomainWindow : Window
    {
        public Domain Result { get; set; }
        public AddDomainWindow()
        {
            InitializeComponent();

            Result = new Domain("Unknown", DomainType.Unknown);
            MyGrid.DataContext = Result;
        }

        private void DomainTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Result.name = Result.type.ToString();
            DomainNameTB.Text = Result.name;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

    }
}
