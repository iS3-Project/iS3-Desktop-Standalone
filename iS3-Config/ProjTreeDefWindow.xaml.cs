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

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for ProjTreeDefWindow.xaml
    /// </summary>
    public partial class ProjTreeDefWindow : Window
    {
        Project _prj;

        public ProjTreeDefWindow(Project prj)
        {
            InitializeComponent();

            _prj = prj;
            Loaded += ProjTreeDefWindow_Loaded;
        }

        private void ProjTreeDefWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> types = ObjectTypeHelper.GetDObjectTypes();
            NameCB.ItemsSource = types;

            foreach (Domain dm in _prj.domains.Values)
            {
                TreeUserControl treeCtrl = new TreeUserControl(dm.root);
                treeCtrl.OnTreeSelected += TreeCtrl_OnTreeSelected;
                treeCtrl.OnTreeAdded += TreeCtrl_OnTreeAdded;
                treeCtrl.OnTreeRemoved += TreeCtrl_OnTreeRemoved;

                TabItem tab = new TabItem();
                tab.Header = dm.name;
                TreeTabHolder.Items.Add(tab);
                tab.Content = treeCtrl;
            }

            if (_prj.domains.Count > 0)
                TreeTabHolder.SelectedIndex = 0;
        }

        private void TreeTabHolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = TreeTabHolder.SelectedItem as TabItem;
            string name = tab.Header as string;
            Domain domain = _prj.domains[name];

            DObjsCB.ItemsSource = domain.objsDefinitions.Keys;
        }

        private void TreeCtrl_OnTreeSelected(object sender, object e)
        {
            Tree tree = e as Tree;
            TreeItemGrid.DataContext = tree;
        }

        private void TreeCtrl_OnTreeAdded(object sender, object e)
        {
            TabItem tab = TreeTabHolder.SelectedItem as TabItem;
            string name = tab.Header as string;

            Tree newTree = new Tree();
            newTree.Name = "New-name";
            newTree.DisplayName = "Input New name";
            newTree.RefDomainName = name;

            Tree tree = e as Tree;
            if (tree == null)
            {
                Domain domain = _prj.domains[name];
                domain.root.Children.Add(newTree);
            }
            else
            {
                tree.Children.Add(newTree);
            }
        }

        private void TreeCtrl_OnTreeRemoved(object sender, object e)
        {
            TabItem tab = TreeTabHolder.SelectedItem as TabItem;
            string name = tab.Header as string;
            Domain domain = _prj.domains[name];

            Tree tree = e as Tree;
            Tree parent = Tree.FindParent(domain.root, tree);
            if (parent != null)
                parent.Children.Remove(tree);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

    }
}
