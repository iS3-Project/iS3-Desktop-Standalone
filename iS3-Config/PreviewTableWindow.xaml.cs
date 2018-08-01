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

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for PreviewTableWindow.xaml
    /// </summary>
    public partial class PreviewTableWindow : Window
    {
        string[] _names;
        DataSet _dataSet;

        public PreviewTableWindow(string tableName, DataSet dataSet)
        {
            InitializeComponent();

            _names = tableName.Split(new char[] { ',' });
            TablesLB.ItemsSource = _names;
            _dataSet = dataSet;

            Loaded += PreviewTableWindow_Loaded;
        }

        private void PreviewTableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_names.Count() > 0)
                TablesLB.SelectedIndex = 0;
        }

        private void TablesLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = TablesLB.SelectedItem as string;
            DataTable dt = _dataSet.Tables[name];
            DataView dv = new DataView(dt);
            TableDG.ItemsSource = dv;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
