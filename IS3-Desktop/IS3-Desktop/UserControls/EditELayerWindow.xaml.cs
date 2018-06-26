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

namespace IS3.Desktop.UserControls
{
    /// <summary>
    /// Interaction logic for EditELayerWindow.xaml
    /// </summary>
    public partial class EditELayerWindow : Window
    {
        //EngineeringLayer _eLayer;
        //public EngineeringLayer ELayer { get { return _eLayer; } }

        public EditELayerWindow(/*EngineeringLayer eLayer*/)
        {
            //_eLayer = eLayer;
            //if (_eLayer == null)
            //    _eLayer = new EngineeringLayer();

            //InitializeComponent();

            //Root.DataContext = _eLayer;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
