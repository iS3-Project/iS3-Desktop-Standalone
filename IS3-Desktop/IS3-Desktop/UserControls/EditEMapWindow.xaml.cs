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
using IS3.Desktop;

namespace IS3.Desktop.UserControls
{
    /// <summary>
    /// Interaction logic for NewEMapWindow.xaml
    /// </summary>
    public partial class EditEMapWindow : Window
    {
        EngineeringMap _eMap;
        MainFrame _mainFrame;

        public EngineeringMap EMap { get { return _eMap; } }

        public EditEMapWindow(MainFrame mainFrame, EngineeringMap eMap)
        {
            _mainFrame = mainFrame;
            _eMap = eMap;

            if (_eMap == null)
            {
                _eMap = new EngineeringMap();
                int count = _mainFrame.views.Count() + 1;
                _eMap.MapID = "MyView" + count.ToString();
                _eMap.MapType = EngineeringMapType.FootPrintMap;
                _eMap.MinimumResolution = 0.01;
            }

            InitializeComponent();

            if (_eMap.MapType == EngineeringMapType.FootPrintMap)
                MapType.SelectedIndex = 0;
            else if (_eMap.MapType == EngineeringMapType.GeneralProfileMap)
                MapType.SelectedIndex = 1;

            Root.DataContext = _eMap;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (MapType.SelectedIndex == 0)
                _eMap.MapType = EngineeringMapType.FootPrintMap;
            else if (MapType.SelectedIndex == 1)
                _eMap.MapType = EngineeringMapType.GeneralProfileMap;

            _eMap.LocalTileFileName1 = LocalTileFile1.Text;
            _eMap.LocalTileFileName2 = LocalTileFile2.Text;
            _eMap.LocalMapFileName = LocalMapFile.Text;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnLocalTileFile_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = _mainFrame.prj.projDef.LocalTilePath;
            dlg.DefaultExt = ".tpk"; // Default file extension
            dlg.Filter = "ESRI Tile Packages (.tpk)|*.tpk"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                string filename = dlg.SafeFileName;
                if (btn.Name == "BtnLocalTileFile1")
                    LocalTileFile1.Text = filename;
                else if (btn.Name == "BtnLocalTileFile2")
                    LocalTileFile2.Text = filename;
            }
        }

        private void BtnLocalMapFile_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = _mainFrame.prj.projDef.LocalFilePath;
            dlg.DefaultExt = ".mpk"; // Default file extension
            dlg.Filter = "ESRI Map Packages (.mpk)|*.mpk"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                string filename = dlg.SafeFileName;
                LocalMapFile.Text = filename;
            }
        }

        private void BtnAddLayer_Click(object sender, RoutedEventArgs e)
        {
            EditELayerWindow window = new EditELayerWindow();
            window.Owner = this;

            Nullable<bool> result = window.ShowDialog();
            if (result == true)
            {
                //EngineeringLayer eLayer = window.ELayer;
                //_eMap.ELayers.Add(eLayer);
                LBLayers.Items.Refresh();
            }
        }

        private void BtnEditLayer_Click(object sender, RoutedEventArgs e)
        {
            //EngineeringLayer eLayer = LBLayers.SelectedItem as EngineeringLayer;
            //if (eLayer == null)
            //    return;

            //EngineeringLayer eLayerCopy = new EngineeringLayer();
            //eLayerCopy.CopyFrom(eLayer);

            //EditELayerWindow window = new EditELayerWindow(eLayerCopy);
            //window.Owner = this;

            //Nullable<bool> result = window.ShowDialog();
            //if (result == true)
            //{
            //    eLayer.CopyFrom(window.ELayer);
            //    LBLayers.Items.Refresh();
            //}
        }

        private void BtnRemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            //EngineeringLayer eLayer = LBLayers.SelectedItem as EngineeringLayer;
            //if (eLayer == null)
            //    return;

            //_eMap.ELayers.Remove(eLayer);
            LBLayers.Items.Refresh();
        }
    }
}
