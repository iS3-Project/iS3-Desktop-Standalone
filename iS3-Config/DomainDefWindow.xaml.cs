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
using System.Data;
using IS3.Core;
using IS3.Unity.Webplayer.UnityCore;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for DomainDefWindow.xaml
    /// </summary>
    public partial class DomainDefWindow : Window
    {
        ProjectDefinition _prjDef;
        Project _prj;
        List<EMapLayers> _eMapLayersList;
        UnityLayer _u3dLayer;

        public DomainDefWindow(ProjectDefinition prjDef, Project prj,
            List<EMapLayers> eMapLayersList)
        {
            InitializeComponent();

            _prjDef = prjDef;
            _prj = prj;
            _eMapLayersList = eMapLayersList;
            DomainListLB.ItemsSource = prj.domains;
            Loaded += DomainDefWindow_Loaded;
        }

        private void DomainDefWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_prj.domains.Count > 0)
                DomainListLB.SelectedIndex = 0;

            List<string> types = ObjectTypeHelper.GetDObjectTypes();
            TypeCB.ItemsSource = types;
        }

        private void DomainListLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear DObjsLB at first
            DObjsLB.ItemsSource = null;

            // Check if this is triggered by clearing event
            if (DomainListLB.SelectedItem == null)
                return;

            KeyValuePair<string, Domain> item = (KeyValuePair<string, Domain>)DomainListLB.SelectedItem;
            Domain domain = _prj.domains[item.Key];
            Dictionary<string, DGObjectsDefinition> objsDef = domain.objsDefinitions;
            DObjsLB.ItemsSource = objsDef;

            if (domain.objsDefinitions.Count > 0)
                DObjsLB.SelectedIndex = 0;
        }

        private void DObjsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if this is triggered by clearing event
            if (DObjsLB.ItemsSource == null)
            {
                DObjsDefGrid.DataContext = null;
                return;
            }

            KeyValuePair<string, DGObjectsDefinition> item =
                (KeyValuePair<string, DGObjectsDefinition>)DObjsLB.SelectedItem;
            DGObjectsDefinition DObjsDef = item.Value;
            DObjsDefGrid.DataContext = DObjsDef;
        }

        private void TableNameSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            DGObjectsDefinition DObjsDef = DObjsDefGrid.DataContext as DGObjectsDefinition;
            if (DObjsDef == null)
                return;

            string dbFile = _prjDef.LocalFilePath + "\\" + _prjDef.LocalDatabaseName;
            List<string> names = DbHelper.GetDbTablenames(dbFile);

            SelectTableNamesWindow selectTableNamesWnd = new SelectTableNamesWindow(names, TableNameTB.Text);
            selectTableNamesWnd.Owner = this;
            selectTableNamesWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ok = selectTableNamesWnd.ShowDialog();
            if (ok != null && ok.Value == true)
            {
                TableNameTB.Text = selectTableNamesWnd.SelectedName;
                DObjsDef.TableNameSQL = selectTableNamesWnd.SelectedName;
            }
        }
        private void PreviewTableBtn_Click(object sender, RoutedEventArgs e)
        {
            string dbFile = _prjDef.LocalFilePath + "\\" + _prjDef.LocalDatabaseName;
            DGObjectsDefinition dObjsDef = DObjsDefGrid.DataContext as DGObjectsDefinition;
            if (dObjsDef == null)
                return;

            DataSet dataSet = DbHelper.LoadTable(dbFile, 
                dObjsDef.TableNameSQL, dObjsDef.ConditionSQL, dObjsDef.OrderSQL);

            PreviewTableWindow previewTblWnd = new PreviewTableWindow(TableNameTB.Text, dataSet);
            previewTblWnd.Owner = this.Owner;
            previewTblWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            previewTblWnd.Show();
        }

        private void TwoDimLayerBtn_Click(object sender, RoutedEventArgs e)
        {
            DGObjectsDefinition DObjsDef = DObjsDefGrid.DataContext as DGObjectsDefinition;
            if (DObjsDef == null)
                return;

            SelectEMapLayersWindow selectEMapLayersWnd = new SelectEMapLayersWindow(_eMapLayersList);
            selectEMapLayersWnd.Owner = this;
            selectEMapLayersWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ok = selectEMapLayersWnd.ShowDialog();
            if (ok != null && ok.Value == true)
            {
                if (selectEMapLayersWnd.SelectedLayerName != null)
                {
                    LayerNameTB.Text = selectEMapLayersWnd.SelectedLayerName;
                    DObjsDef.GISLayerName = selectEMapLayersWnd.SelectedLayerName;
                }
            }
        }

        private void ThreeDimLayerBtn_Click(object sender, RoutedEventArgs e)
        {
            DGObjectsDefinition DObjsDef = DObjsDefGrid.DataContext as DGObjectsDefinition;
            if (DObjsDef == null)
                return;

            if (_u3dLayer == null)
            {
                PromptTB.Text = "Click the button again until the 3D model is loaded.";
                Preview3DLayerBtn_Click(this, null);
                return;
            }

            Select3DEMapLayersWindow select3DEMapLayersWnd = new Select3DEMapLayersWindow(_u3dLayer);
            select3DEMapLayersWnd.Owner = this;
            select3DEMapLayersWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ok = select3DEMapLayersWnd.ShowDialog();
            if (ok != null && ok.Value == true)
            {
                if (select3DEMapLayersWnd.SelectLayerName != null)
                {
                    Layer3DNameTB.Text = select3DEMapLayersWnd.SelectLayerName;
                    DObjsDef.Layer3DName = select3DEMapLayersWnd.SelectLayerName;
                }
            }
        }

        private void AddDomain_Click(object sender, RoutedEventArgs e)
        {
            AddDomainWindow addDomainWnd = new AddDomainWindow();
            addDomainWnd.Owner = this;
            addDomainWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ok = addDomainWnd.ShowDialog();
            if (ok != null && ok.Value == true)
            {
                Domain result = addDomainWnd.Result;
                if (_prj.domains.Keys.Contains(result.name))
                {
                    MessageBox.Show("Domain already exist!", "Error", MessageBoxButton.OK);
                }
                else
                {
                    _prj.domains.Add(result.name, result);

                    // force update UI
                    int index = DomainListLB.SelectedIndex;
                    DomainListLB.ItemsSource = null;
                    DomainListLB.ItemsSource = _prj.domains;
                    if (index != -1)
                        DomainListLB.SelectedIndex = index;
                    else
                        DomainListLB.SelectedIndex = 0;
                }
            }
        }

        private void RemoveDomain_Click(object sender, RoutedEventArgs e)
        {
            int index = DomainListLB.SelectedIndex;
            if (index == -1)
                return;
            KeyValuePair<string, Domain> item = (KeyValuePair<string, Domain>)DomainListLB.SelectedItem;

            _prj.domains.Remove(item.Key);

            // force update UI
            DomainListLB.ItemsSource = null;
            DomainListLB.ItemsSource = _prj.domains;

            if (index > 0)
                DomainListLB.SelectedIndex = index-1;
            else if (index == 0)
            {
                if (DomainListLB.Items.Count > 0)
                    DomainListLB.SelectedIndex = 0;
            }
        }

        private void AddDObj_Click(object sender, RoutedEventArgs e)
        {
            // In case there is no domain, just return.
            if (DomainListLB.Items.Count == 0)
                return;

            Dictionary<string, DGObjectsDefinition> objsDefinitions =
                DObjsLB.ItemsSource as Dictionary<string, DGObjectsDefinition>;
            AddDObjWindow addDObjWnd = new AddDObjWindow(objsDefinitions);
            addDObjWnd.Owner = this;
            addDObjWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ok = addDObjWnd.ShowDialog();
            if (ok != null && ok.Value == true)
            {
                string name = addDObjWnd.DObjName;
                DGObjectsDefinition dObjDef = new DGObjectsDefinition();
                dObjDef.Name = name;
                objsDefinitions.Add(name, dObjDef);

                // force update UI
                int index = DObjsLB.SelectedIndex;
                DObjsLB.ItemsSource = null;
                DObjsLB.ItemsSource = objsDefinitions;
                if (index != -1)
                    DObjsLB.SelectedIndex = index;
                else
                    DObjsLB.SelectedIndex = 0;
            }

        }

        private void RemoveDObj_Click(object sender, RoutedEventArgs e)
        {
            int index = DObjsLB.SelectedIndex;
            if (index == -1)
                return;
            KeyValuePair<string, DGObjectsDefinition> item = 
                (KeyValuePair<string, DGObjectsDefinition>)DObjsLB.SelectedItem;

            Dictionary<string, DGObjectsDefinition> objsDefinitions=
                DObjsLB.ItemsSource as Dictionary<string, DGObjectsDefinition>;
            objsDefinitions.Remove(item.Key);

            // force update UI
            DObjsLB.ItemsSource = null;
            DObjsLB.ItemsSource = objsDefinitions;
            if (index > 0)
                DObjsLB.SelectedIndex = index - 1;
            else if (index == 0)
            {
                if (DObjsLB.Items.Count > 0)
                    DObjsLB.SelectedIndex = 0;
            }
        }

        private void Preview2DLayerBtn_Click(object sender, RoutedEventArgs e)
        {
            string lyrName = LayerNameTB.Text;
            Preview2DLayerWindow preview2dWnd = new Preview2DLayerWindow(_prjDef, lyrName);
            preview2dWnd.Owner = this.Owner;
            preview2dWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            preview2dWnd.Title += ": " + lyrName;
            preview2dWnd.Show();
        }

        private void Preview3DLayerBtn_Click(object sender, RoutedEventArgs e)
        {
            Proj3DViewDefWindow proj3DViewDefWnd = new Proj3DViewDefWindow(_prjDef);
            proj3DViewDefWnd.Model3dLoaded += (send, args)=>
            {
                _u3dLayer = args;
            };
            proj3DViewDefWnd.Owner = this.Owner;
            proj3DViewDefWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            proj3DViewDefWnd.Show();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            // finish 
            DialogResult = true;
            Close();
        }
    }
}
