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

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for SelectTableNamesWindow.xaml
    /// </summary>
    public partial class SelectTableNamesWindow : Window
    {
        public string SelectedName = "";
        string[] _names;
        public SelectTableNamesWindow(List<string> nameList, string name)
        {
            InitializeComponent();
            TableNamesLB.ItemsSource = nameList;

            _names = name.Split(new char[] { ',' });
            Loaded += SelectTableNamesWindow_Loaded;
        }

        private void SelectTableNamesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _names.Count(); ++i)
            {
                TableNamesLB.SelectedItems.Add(_names[i]);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            int count = TableNamesLB.SelectedItems.Count;
            for (int i = 0; i<count; ++i)
            {
                SelectedName += TableNamesLB.SelectedItems[i].ToString();
                if (i < count - 1)
                    SelectedName += ",";
            }

            DialogResult = true;
            Close();
        }
    }
}
