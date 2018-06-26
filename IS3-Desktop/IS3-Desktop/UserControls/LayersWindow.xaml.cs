using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;

namespace IS3.Desktop.UserControls
{
    public class LayerItem
    {
        public string Name { get; set; }        // map layer name
        public bool Visibility { get; set; }    // map layer visibility
        public object LayerObject { get; set; } // layer object
    }

    public class LayerCheckBoxClickArgs : EventArgs
    {
        public LayerItem Item { get; set; }
    }

    public partial class LayersWindow : Window
    {
        public event EventHandler<LayerCheckBoxClickArgs> OnLayerCheckBoxClick;

        public LayersWindow(IEnumerable<LayerItem> items)
        {
            InitializeComponent();
            BaseLayerList.ItemsSource = items;
        }

        private void LayerCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (OnLayerCheckBoxClick != null)
            {
                CheckBox tickedCheckBox = sender as CheckBox;

                LayerItem item = new LayerItem();
                if (tickedCheckBox.Content != null)
                    item.Name = tickedCheckBox.Content.ToString();
                item.Visibility = (bool)tickedCheckBox.IsChecked;
                item.LayerObject = tickedCheckBox.Tag;

                LayerCheckBoxClickArgs args = new LayerCheckBoxClickArgs();
                args.Item = item;

                OnLayerCheckBoxClick(this, args);
            }
        }
    }
}
