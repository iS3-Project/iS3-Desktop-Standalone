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
using System.Windows.Forms;
using System.IO;
using IS3.Core;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Data;

namespace iS3.Config
{
    // GdbLayer is used for UI binding and selection operations.
    // The first three members are for UI binding.
    // The last two members are for selection operations.
    //
    public class GdbLayer
    {
        public string Name { get; set; }
        public bool Visibility { get; set; }
        public GdbLayer LayerObject { get; set; }
        public GeodatabaseFeatureTable FeatureTable { get; set; }
        public Esri.ArcGISRuntime.Geometry.Envelope Extent { get; set; }
    }

    // EMapLayers is a helper class which is used for later 2D layer selection
    // of the DObjectsDefinition class.
    //
    public class EMapLayers
    {
        public string EMapName { get; set; }
        public List<string> EMapLayerNameList { get; set; }

        public EMapLayers()
        {
            EMapLayerNameList = new List<string>();
        }
    }

    /// <summary>
    /// Interaction logic for ProjEMapDefWindow.xaml
    /// </summary>
    public partial class ProjEMapDefWindow : Window
    {
        public List<EMapLayers> EMapLayersList { get; set; }
        ProjectDefinition _projDef;

        public ProjEMapDefWindow(ProjectDefinition projDef)
        {
            InitializeComponent();

            _projDef = projDef;
            foreach (EngineeringMap emap in projDef.EngineeringMaps)
                EMapsLB.Items.Add(emap);

            MyMapView.Loaded += MyMapView_Loaded;
        }

        private void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            EngineeringMap firstEMap = _projDef.EngineeringMaps.FirstOrDefault();
            if (firstEMap != null)
            {
                EMapsLB.SelectedIndex = 0;
                // refresh UI
                EMapGrd.DataContext = firstEMap;
            }
        }

        private void LocalTileBtn_Click(object sender, RoutedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;

            string file = emap.LocalTileFileName1;
            string path = _projDef.LocalTilePath;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = path;
            dialog.Filter = "Tile Packages (.tpk)|*.tpk";
            dialog.FileName = file;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                emap.LocalTileFileName1 = dialog.SafeFileName;
                LoadTiledLayer1(emap);

                // refresh UI
                EMapGrd.DataContext = null;
                EMapGrd.DataContext = emap;
            }
        }

        private async void LocalGeoDBBtn_Click(object sender, RoutedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;

            string file = emap.LocalTileFileName1;
            string path = _projDef.LocalFilePath;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = path;
            dialog.Filter = "Geo-Database(.geodatabase)|*.geodatabase";
            dialog.FileName = file;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Load new Local GeoDB layers
                emap.LocalGeoDbFileName = dialog.SafeFileName;
                await ReloadGeoDb(emap);

                // refresh UI
                EMapGrd.DataContext = null;
                EMapGrd.DataContext = emap;
            }

        }

        private void LayerCB_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox chkBox = sender as System.Windows.Controls.CheckBox;

            // Click => Switch the layer on/off
            GdbLayer gdbLyr = chkBox.Tag as GdbLayer;
            Layer lyr = Map.Layers[gdbLyr.Name];
            lyr.IsVisible = chkBox.IsChecked.Value;

            // Update layer definition
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;
            LayerDef lyrDef = emap.LocalGdbLayersDef.Find(x => x.Name == gdbLyr.Name);
            if (lyrDef == null)
                return;
            lyrDef.IsVisible = chkBox.IsChecked.Value;
        }

        private void LayrCB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.CheckBox chkBox = sender as System.Windows.Controls.CheckBox;

            // Double click => Zoom to the layer
            GdbLayer gdbLayer = chkBox.Tag as GdbLayer;
            MyMapView.SetView(gdbLayer.Extent);
        }

        private async void AddEMap_Click(object sender, RoutedEventArgs e)
        {
            // new emap
            EngineeringMap emap = new EngineeringMap();
            emap.MapID = "Map" + _projDef.EngineeringMaps.Count.ToString();
            emap.MapType = EngineeringMapType.FootPrintMap;

            // update project definition
            _projDef.EngineeringMaps.Add(emap);

            // refresh UI
            EMapsLB.Items.Add(emap);
            EMapsLB.SelectedItem = emap;
            EMapGrd.DataContext = null;
            EMapGrd.DataContext = emap;

            // refresh map
            await ReloadMap(emap);
        }

        private async void RemoveEMap_Click(object sender, RoutedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;

            // Update project definition
            _projDef.EngineeringMaps.Remove(emap);

            // Refresh UI
            EMapsLB.Items.Remove(emap);
            EMapGrd.DataContext = null;

            // Refresh map
            await ReloadMap(null);
        }

        private async void EMapsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;

            // refresh UI
            EMapGrd.DataContext = null;
            EMapGrd.DataContext = emap;

            // refresh map
            await ReloadMap(emap);
        }

        private async void LyrSetting_Click(object sender, RoutedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;

            GdbLayer gdbLyr = GeoDBLayrLB.SelectedItem as GdbLayer;
            if (gdbLyr == null)
                return;

            LayerDef lyrDef = emap.LocalGdbLayersDef.Find(x => x.Name == gdbLyr.Name);

            LayerDefWindow lyrDefWnd = new LayerDefWindow(lyrDef);
            lyrDefWnd.Owner = this;
            lyrDefWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? success = lyrDefWnd.ShowDialog();
            if (success != null && success.Value == true)
            {
                Layer lyr = Map.Layers[gdbLyr.Name];
                if (lyr != null)
                {
                    // Reload the layer
                    Map.Layers.Remove(lyr);
                    await GdbHelper.addGeodatabaseLayer(Map, lyrDef, gdbLyr.FeatureTable);
                }
            }
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            EMapLayersList = new List<EMapLayers>();

            foreach (EngineeringMap emap in _projDef.EngineeringMaps)
            {
                string file = _projDef.LocalFilePath + "\\" + emap.LocalGeoDbFileName;
                if (File.Exists(file))
                {
                    EMapLayers eMapLayers = new EMapLayers();
                    eMapLayers.EMapName = emap.MapID;

                    // Open geodatabase
                    Geodatabase gdb = await Geodatabase.OpenAsync(file);
                    IEnumerable<GeodatabaseFeatureTable> featureTables =
                        gdb.FeatureTables;
                    foreach (var table in featureTables)
                    {
                        eMapLayers.EMapLayerNameList.Add(table.Name);
                    }

                    EMapLayersList.Add(eMapLayers);
                }
            }
                // finish 
            DialogResult = true;
            Close();
        }

        // Load ArcGISLocalTiledLayer for the specified engineering map
        //
        void LoadTiledLayer1(EngineeringMap emap)
        {
            ArcGISLocalTiledLayer tileLayr1 = Map.Layers["TiledLayer1"] as ArcGISLocalTiledLayer;
            if (tileLayr1 != null)
            {
                Map.Layers.Remove(tileLayr1);
            }

            string file = _projDef.LocalTilePath + "\\" + emap.LocalTileFileName1;
            if (File.Exists(file))
            {
                ArcGISLocalTiledLayer newLayr = new ArcGISLocalTiledLayer(file);
                newLayr.ID = "TiledLayer1";
                newLayr.DisplayName = "TileLayer1";
                Map.Layers.Add(newLayr);

                if (newLayr.FullExtent != null)
                {
                    MyMapView.SetView(newLayr.FullExtent);
                }
            }
        }

        // Reload the specified engineering map, clear the map view at first.
        //
        async Task ReloadMap(EngineeringMap emap)
        {
            Map.Layers.Clear();
            if (emap != null)
            {
                LoadTiledLayer1(emap);
                await ReloadGeoDb(emap);
            }
        }

        // Load the specifiled emap's geodatabase, and add all the features layers to the map.
        //
        async Task ReloadGeoDb(EngineeringMap emap)
        {
            // Clear existing Local GeoDB layers
            if (GeoDBLayrLB.ItemsSource != null)
            {
                foreach (var item in GeoDBLayrLB.ItemsSource)
                {
                    GdbLayer lyr = item as GdbLayer;
                    Layer mapLyr = Map.Layers[lyr.Name];
                    if (mapLyr != null)
                        Map.Layers.Remove(mapLyr);
                }
                GeoDBLayrLB.ItemsSource = null;
            }

            // Load new
            string file = _projDef.LocalFilePath + "\\" + emap.LocalGeoDbFileName;
            if (File.Exists(file))
            {
                // Open geodatabase
                Geodatabase gdb = await Geodatabase.OpenAsync(file);
                IEnumerable<GeodatabaseFeatureTable> featureTables =
                    gdb.FeatureTables;
                List<GdbLayer> gdbLayers = new List<GdbLayer>();
                foreach (var table in featureTables)
                {
                    // Search LayerDef, use default if not found.
                    LayerDef lyrDef = emap.LocalGdbLayersDef.Find(x => x.Name == table.Name);
                    if (lyrDef == null)
                    {
                        lyrDef = GdbHelper.GenerateDefaultLayerDef(table);
                        emap.LocalGdbLayersDef.Add(lyrDef);
                    }

                    // GdbLayer is used for UI binding and selection operations.
                    GdbLayer layer = new GdbLayer();
                    layer.Name = table.Name;
                    layer.Visibility = lyrDef.IsVisible;
                    layer.LayerObject = layer;
                    layer.FeatureTable = table;
                    layer.Extent = table.Extent;
                    gdbLayers.Add(layer);


                    // Add the feature layer to the map
                    await GdbHelper.addGeodatabaseLayer(Map, lyrDef, table);
                }

                // Refresh UI
                GeoDBLayrLB.ItemsSource = gdbLayers;
            }
        }

        private void MyMapView_ExtentChanged(object sender, EventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;
            if (MyMapView.Extent == null)
                return;
            emap.XMin = MyMapView.Extent.XMin;
            emap.YMin = MyMapView.Extent.YMin;
            emap.XMax = MyMapView.Extent.XMax;
            emap.YMax = MyMapView.Extent.YMax;
        }
    }
}
