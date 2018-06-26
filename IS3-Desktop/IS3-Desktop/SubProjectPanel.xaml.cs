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

using Xceed.Wpf.AvalonDock.Layout;

using IS3.Core;
using IS3.Desktop.UserControls;
using IS3.Geology;

namespace IS3.Desktop
{
    /// <summary>
    /// Interaction logic for ProjectPanel.xaml
    /// </summary>
    public partial class SubProjectPanel : UserControl
    {
        protected IS3ProjectTree _treePanel;
        protected MainFrame _mainFrame;

        public IS3ProjectTree TreePanel
        {
            get { return _treePanel; }
        }

        public SubProjectPanel(SubProject subPrj)
        {
            InitializeComponent();

            App app = App.Current as App;
            _mainFrame = app.MainFrame;

            _treePanel = new IS3ProjectTree(this, MyTreeView, subPrj);

            GotFocus += SubProjectPanel_GotFocus;
        }

        void SubProjectPanel_GotFocus(object sender, RoutedEventArgs e)
        {
            _mainFrame.SetActiveSubProjectTree(_treePanel);
        }

        private void MyTreeView_SelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            _treePanel.TreeSelectedItemChanged(sender, e);
        }

        private void NewFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            _treePanel.NewTree();
        }

        private void DeleteFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            _treePanel.DeleteTree();
        }

        private void SelectionBtn_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.BeginSelection();
        }

        private void AddFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            _treePanel.AddTree();
        }

        private void EditFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            _treePanel.EditTree();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            _treePanel.Settings();
        }

        private void DrawBtn_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.BeginDraw();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SimpleProfileAnalysisWindow spaWindow =
                new SimpleProfileAnalysisWindow();
            spaWindow.MainFrame = _mainFrame;
            
            spaWindow.Show();
        }

        private void MyTreeView_PreviewMouseRightButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            TreeView tv = (TreeView)sender;
            IInputElement element = tv.InputHitTest(e.GetPosition(tv));
            while (!((element is TreeView) || element == null))
            {
                if (element is TreeViewItem)
                    break;

                if (element is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)element;
                    element = (IInputElement)(fe.Parent ?? fe.TemplatedParent);
                }
                else
                    break;
            }
            if (element is TreeViewItem)
            {
                element.Focus();
                //e.Handled = true;
            }
        }

        private void NewWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;

            EditEMapWindow window = new EditEMapWindow(_mainFrame, null);
            window.Owner = mainWindow;
            Nullable<bool> result = window.ShowDialog();
            if (result == true)
            {
                EngineeringMap eMap = 
                    _mainFrame.ActiveSubProject.NewUserEMap("Default",
                    EngineeringMapType.FootPrintMap);
                eMap.CopyFrom(window.EMap);

                _mainFrame.AddView(eMap, true);
            }
        }

        private void PinBtn_Click(object sender, RoutedEventArgs e)
        {
            //_mainFrame.PanToSelectedObjects();
        }

    }
}
