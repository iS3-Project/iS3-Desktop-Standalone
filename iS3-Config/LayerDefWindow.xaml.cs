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
    /// Interaction logic for LayerDefWindow.xaml
    /// </summary>
    public partial class LayerDefWindow : Window
    {
        public LayerDefWindow(LayerDef lyrDef)
        {
            InitializeComponent();

            BasicGrd.DataContext = lyrDef;
            LabelGrd.DataContext = lyrDef;

            CatLB.SelectedIndex = 0;
        }
        private void CatLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CatLB.SelectedIndex == 0)
            {
                BasicGrd.Visibility = Visibility.Visible;
                LabelGrd.Visibility = Visibility.Collapsed;
            }
            else if (CatLB.SelectedIndex == 1)
            {
                BasicGrd.Visibility = Visibility.Collapsed;
                LabelGrd.Visibility = Visibility.Visible;
            }
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

    }
}
