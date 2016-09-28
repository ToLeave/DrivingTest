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
    /// EndTest.xaml 的交互逻辑
    /// </summary>
    public partial class EndTest : Window
    {
        public EndTest()
        {
            InitializeComponent();
        }

        public int end = 0;
        private void end_button_Click(object sender, RoutedEventArgs e)
        {
            end = 1;
            this.Close();
        }
    }
}
