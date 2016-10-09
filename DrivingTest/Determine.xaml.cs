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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrivingTest
{
    /// <summary>
    /// Determine.xaml 的交互逻辑
    /// </summary>
    public partial class Determine : UserControl
    {
        public Determine()
        {
            InitializeComponent();
        }

        private void fanhui_button_Click(object sender, RoutedEventArgs e)
        {
            SubjectTwo su = new SubjectTwo();
            this.Content = su;
        }
    }
}
