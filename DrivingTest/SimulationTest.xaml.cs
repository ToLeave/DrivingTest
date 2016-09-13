using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DrivingTest
{
    /// <summary>
    /// SimulationTest.xaml 的交互逻辑
    /// </summary>
    public partial class SimulationTest : Window
    {
        public SimulationTest()
        {
            InitializeComponent();
        }

        private void kaipao_Click(object sender, RoutedEventArgs e)
        {
            PreparePage pr = new PreparePage();
            pr.Show();
            this.Close();
        }
    }
}
