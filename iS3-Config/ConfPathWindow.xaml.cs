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
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for ConfPathWindow.xaml
    /// </summary>
    public partial class ConfPathWindow : Window
    {
        public string ExePath = "";
        public string DataPath = "";

        public ConfPathWindow()
        {
            InitializeComponent();

            SetPath();
        }

        void SetPath()
        {
            if (ExePath.Length == 0)
            {
                string exeLocation = Assembly.GetExecutingAssembly().Location;
                ExePath = System.IO.Path.GetDirectoryName(exeLocation);
            }

            DataPath = ExePath + "\\Data";
            if (!Directory.Exists(DataPath))
                DataPath = ExePath;

            iS3Labl.Content = ExePath;
            myLabl.Content = DataPath;
        }

        private void iS3LocBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.SelectedPath = ExePath;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ExePath = dialog.SelectedPath;
                SetPath();
            }
        }

        private void myDatBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.SelectedPath = DataPath;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DataPath = dialog.SelectedPath;
                myLabl.Content = DataPath;
            }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

    }
}
