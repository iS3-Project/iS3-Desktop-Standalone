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

namespace IS3.Desktop.Monitoring
{
    /// <summary>
    /// Interaction logic for MonitoringPanel.xaml
    /// </summary>
    public partial class MonitoringPanel : UserControl
    {
        protected IS3Tree _treePanel;

        public IS3Tree TreePanel
        {
            get { return _treePanel; }
        }

        public MonitoringPanel()
        {
            InitializeComponent();

            App app = App.Current as App;
            _treePanel = new IS3Tree(this, MonitoringTreeView,
                app.Project.MonitoringTree);
        }

        private void MonitoringTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //_treePanel.TreeSelectedItemChanged(sender, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
