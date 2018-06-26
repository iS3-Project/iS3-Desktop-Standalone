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
using System.Windows.Navigation;
using System.Windows.Shapes;

using IS3.Core;
using IS3.Desktop;

namespace IS3.Desktop.Geology
{
    /// <summary>
    /// Interaction logic for GeologyPanel.xaml
    /// </summary>
    public partial class GeologyPanel : UserControl
    {
        protected IS3Tree _treePanel;

        public IS3Tree TreePanel
        {
            get { return _treePanel; }
        }

        public GeologyPanel()
        {
            InitializeComponent();

            App app = App.Current as App;
            _treePanel = new IS3Tree(this, GeologyTreeView,
                app.Project.GeoTree);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void GeologyTreeView_SelectedItemChanged(object sender, 
            RoutedPropertyChangedEventArgs<object> e)
        {
            //_treePanel.TreeSelectedItemChanged(sender, e);
        }
    }
}
