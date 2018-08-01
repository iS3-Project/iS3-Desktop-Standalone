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
    public partial class SelectEMapLayersWindow : Window
    {
        public string SelectedLayerName { get; set; }
        List<EMapLayers> _eMapLayersList;

        public SelectEMapLayersWindow(List<EMapLayers> eMapLayersList)
        {
            InitializeComponent();

            _eMapLayersList = eMapLayersList;
            Loaded += SelectEMapLayersWindow_Loaded;
        }

        private void SelectEMapLayersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EMapListLB.ItemsSource = _eMapLayersList;
            if (_eMapLayersList.Count > 0)
                EMapListLB.SelectedIndex = 0;
        }

        private void EMapListLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EMapLayers eMapLayers = EMapListLB.SelectedItem as EMapLayers;
            LayerListLB.ItemsSource = eMapLayers.EMapLayerNameList;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            SelectedLayerName = LayerListLB.SelectedItem as string;

            DialogResult = true;
            Close();
        }
    }
}
