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
using System.IO;
using IS3.Core;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Data;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for Preview2DLayerWindow.xaml
    /// </summary>
    public partial class Preview2DLayerWindow : Window
    {
        ProjectDefinition _prjDef;
        string _lyrName;
        List<EngineeringMap> _maps = new List<EngineeringMap>();

        public Preview2DLayerWindow(ProjectDefinition projDef, string lyrName)
        {
            InitializeComponent();

            _prjDef = projDef;
            _lyrName = lyrName;

            MyMapView.Loaded += MyMapView_Loaded; ;
        }

        private void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            // search maps that contains the specified layer
            //
            foreach (EngineeringMap emap in _prjDef.EngineeringMaps)
            {
                List<LayerDef> lyrsDef = emap.LocalGdbLayersDef;
                foreach (LayerDef lyrDef in lyrsDef)
                {
                    if (lyrDef.Name == _lyrName)
                    {
                        _maps.Add(emap);
                        break;
                    }
                }
            }

            // show maps selection combox box if the layer exist in more than one map.
            //
            MapsCB.ItemsSource = _maps;
            if (_maps.Count > 0)
                MapsCB.SelectedIndex = 0;
            else if (_maps.Count > 1)
                MapsCB.Visibility = Visibility.Visible;
        }

        private async void MapsCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EngineeringMap emap = MapsCB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;

            List<LayerDef> lyrsDef = emap.LocalGdbLayersDef;
            LayerDef lyrDef = lyrsDef.Find(x => x.Name == _lyrName);
            if (lyrDef == null)
                return;

            // load tiled layer
            //
            string file = _prjDef.LocalTilePath + "\\" + emap.LocalTileFileName1;
            if (File.Exists(file))
            {
                ArcGISLocalTiledLayer newLayr = new ArcGISLocalTiledLayer(file);
                newLayr.ID = "TiledLayer1";
                newLayr.DisplayName = "TileLayer1";
                Map.Layers.Add(newLayr);
                //if (newLayr.FullExtent != null)
                //{
                //    MyMapView.SetView(newLayr.FullExtent);
                //}
            }

            // load the specified layer
            //
            file = _prjDef.LocalFilePath + "\\" + emap.LocalGeoDbFileName;
            if (File.Exists(file))
            {
                // Open geodatabase
                Geodatabase gdb = await Geodatabase.OpenAsync(file);
                IEnumerable<GeodatabaseFeatureTable> featureTables =
                    gdb.FeatureTables;
                foreach (var table in featureTables)
                {
                    if (table.Name == _lyrName)
                    {
                        // Add the feature layer to the map
                        await GdbHelper.addGeodatabaseLayer(Map, lyrDef, table);
                        MyMapView.SetView(table.Extent);
                        break;
                    }
                }
            }
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
