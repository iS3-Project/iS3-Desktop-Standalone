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
using IS3.Unity.Webplayer;
using IS3.Unity.Webplayer.UnityCore;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for Proj3DViewDefWindow.xaml
    /// </summary>
    public partial class Proj3DViewDefWindow : Window
    {
        ProjectDefinition _prjDef;
        string _dataPath;
        string _projID;

        string _u3dFile;
        string _u3dFilePath;
        UnityLayer _u3dLayer;
        U3DView view3d;
        string _nowLayer;
        public EventHandler<UnityLayer> Model3dLoaded;

        public Proj3DViewDefWindow(ProjectDefinition prjDef,string nowLayer)
        {
            InitializeComponent();
            _nowLayer = nowLayer;
            _prjDef = prjDef;

            _dataPath = _prjDef.LocalFilePath;
            _projID = _prjDef.ID;
            _u3dFile = _projID + ".unity3d";
            _u3dFilePath = _dataPath +  "\\" + _u3dFile;

            Model3DTB.Text = _u3dFile;

            Loaded += Proj3DViewDefWindow_Loaded;
        }

        private void Proj3DViewDefWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(_u3dFilePath))
            {
                //u3dPlayerControl.LoadScence(_u3dFilePath);
                view3d = Load3DModel(_u3dFile);
                View3DHolder.Children.Add(view3d);
            }
            else
            {
                PromptTB.Text = "The 3d model file does not exist.";
            }
        }

        U3DView Load3DModel(string fname)
        {
            // new a engineering map for U3DView
            EngineeringMap map = new EngineeringMap();
            map.LocalMapFileName = fname;

            // add 3d view - unitywebplayer
            U3DView u3DView = new U3DView(new Project() { projDef = _prjDef }, map);
            // Add listener to get layers in 3D model
            u3DView.UnityLayerHanlder += UnityLayerListener;
            u3DView.view.loadPredefinedLayers();

            return u3DView;
        }

        public void UnityLayerListener(object sender, UnityLayer unityLayer)
        {
            _u3dLayer = unityLayer;
            treeView.ItemsSource = unityLayer.UnityLayerModel.childs;
            SetInitLayerVisible();

            if (Model3dLoaded != null)
                Model3dLoaded(this, unityLayer);
        }
        string[] layerNames;
        public void SetInitLayerVisible()
        {
            layerNames = _nowLayer.Split('/');
            SetVisible(_u3dLayer.UnityLayerModel.childs, 1);
        }
        public void SetVisible(List<UnityTreeModel> model,int level)
        {
            if (level == layerNames.Count()) return;
            foreach (UnityTreeModel _model in model)
            {
                if (_model.Name==layerNames[level])
                {
                    SetVisible(_model.childs, level + 1);
                }
                else
                {
                    SetChildVisble(_model);
                    SetObjShowStateMessage message = new SetObjShowStateMessage();
                    message.path = GetFullPath(_model);
                    message.iSShow = false;
                    (view3d.view as U3dViewModel).ExcuteCommand(message);
                }
            }
        }
        public void SetChildVisble(UnityTreeModel model)
        {
            model.visible = false;
            if (model.childs!=null)
            {
                foreach (UnityTreeModel _model in model.childs)
                {
                    SetChildVisble(_model);
                }
            }
        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //图层显示开关
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            DependencyObject parent = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(checkBox));  //找到chkbox的ContentPresenter
            ContentPresenter tvi = parent as ContentPresenter;
            if (tvi != null)
            {
                List<UnityTreeModel> list = treeView.ItemsSource as List<UnityTreeModel>;
                UnityTreeModel node = tvi.DataContext as UnityTreeModel;
                TreeViewItem tvitem = FindTreeViewItemContainer(treeView, node);
                tvitem.IsSelected = true;

                SetModelVisible(node, checkBox.IsChecked.Value);
                SetObjShowStateMessage message = new SetObjShowStateMessage();
                message.path = GetFullPath(node);
                message.iSShow = checkBox.IsChecked.Value;
                (view3d.view as U3dViewModel).ExcuteCommand(message);
            }
        }
        public string GetFullPath(UnityTreeModel model)
        {
            if (model.parent==null)
            {
                return model.Name;
            }
            else
            {
                return GetFullPath(model.parent) + "/" + model.Name;
            }
        }
        //找到checkbox对应的父TreeViewItem
        public static TreeViewItem FindTreeViewItemContainer(ItemsControl root, object info)
        {
            if (root == null) { return null; }

            TreeViewItem tvi = root.ItemContainerGenerator.ContainerFromItem(info) as TreeViewItem;
            if (tvi == null)
            {
                if (root.Items != null)
                {
                    foreach (var item in root.Items)
                    {
                        tvi = FindTreeViewItemContainer(root.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem, info);

                        if (tvi != null)
                        {
                            break;
                        }
                    }
                }
            }

            return tvi;
        }
        //设置子节点的可见性，checkbox
        public void SetModelVisible(UnityTreeModel root,bool visible)
        {
            root.visible = visible;
            if (root.childs!=null)
            {
                foreach (UnityTreeModel model in root.childs)
                {
                    SetModelVisible(model, visible);
                }
            }
        }
    }
}
