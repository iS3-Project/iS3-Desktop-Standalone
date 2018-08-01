using IS3.Unity.Webplayer.UnityCore;
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

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for SelectEMapLayersWindow.xaml
    /// </summary>
    public partial class Select3DEMapLayersWindow : Window
    {
        public string SelectLayerName;
        UnityLayer _unityLayer;

        public Select3DEMapLayersWindow(UnityLayer unitylayer)
        {
            InitializeComponent();
            _unityLayer = unitylayer;
            Loaded += SelectEMapLayersWindow_Loaded;
        }

        private void SelectEMapLayersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_unityLayer != null)
                treeView.ItemsSource = _unityLayer.UnityLayerModel.childs;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UnityTreeModel model = treeView.SelectedItem as UnityTreeModel;
            SelectLayerName = "";
            while (null != model)
            {
                SelectLayerName = SelectLayerName == "" ? model.Name : model.Name + "/" + SelectLayerName;
                model = model.parent;
            }
        }
    }
}
