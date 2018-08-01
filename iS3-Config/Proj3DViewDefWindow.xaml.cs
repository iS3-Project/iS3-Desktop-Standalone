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

        public EventHandler<UnityLayer> Model3dLoaded;

        public Proj3DViewDefWindow(ProjectDefinition prjDef)
        {
            InitializeComponent();

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
                U3DView view3d = Load3DModel(_u3dFile);
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

            if (Model3dLoaded != null)
                Model3dLoaded(this, unityLayer);
        }


        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
