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
    /// Interaction logic for AddDObjWindow.xaml
    /// </summary>
    public partial class AddDObjWindow : Window
    {
        public string DObjName { get; set; }
        Dictionary<string, DGObjectsDefinition> _objsDefinitions;

        public AddDObjWindow(Dictionary<string, DGObjectsDefinition> objsDefinitions)
        {
            InitializeComponent();

            _objsDefinitions = objsDefinitions;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            DObjName = NameTB.Text;
            if (_objsDefinitions != null && _objsDefinitions.Keys.Contains(DObjName))
            {
                MessageBox.Show("The name has been used, please specify another name.",
                    "Error", MessageBoxButton.OK);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
