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

using IS3.Core;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for TreeUserControl.xaml
    /// </summary>
    public partial class TreeUserControl : UserControl
    {
        public TreeUserControl(Tree tree)
        {
            InitializeComponent();

            MyTreeView.ItemsSource = tree.Children;
        }

        public event EventHandler<object> OnTreeSelected;
        public event EventHandler<object> OnTreeAdded;
        public event EventHandler<object> OnTreeRemoved;

        private void MyTreeView_SelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            if (OnTreeSelected != null)
                OnTreeSelected(this, e.NewValue);
        }

        private void MyTreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // The sender is TreeView. 
            // We need to get the TreeViewItem by search the parents of e.OriginalSource.
            // 
            DependencyObject source = e.OriginalSource as DependencyObject;
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            TreeViewItem treeViewItem = source as TreeViewItem;

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        private void AddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (OnTreeAdded != null)
                OnTreeAdded(this, MyTreeView.SelectedItem);
        }

        private void RemoveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (OnTreeRemoved != null && MyTreeView.SelectedItem != null)
                OnTreeRemoved(this, MyTreeView.SelectedItem);
        }
    }
}
