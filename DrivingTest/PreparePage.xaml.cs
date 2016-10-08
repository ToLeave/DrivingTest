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
    /// PreparePage.xaml 的交互逻辑
    /// </summary>
    public partial class PreparePage : UserControl
    {
        public PreparePage()
        {
            InitializeComponent();
        }

        private void kaikao_Click(object sender, RoutedEventArgs e)
        {
            MainExam ma = new MainExam();
            
            C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "驾驶理论考试系统");
            if (cp != null)
            {
                cp.Close();
            }

            C1.WPF.C1Window cwin = new C1.WPF.C1Window();
            cwin.Content = ma;
            cwin.Name = "驾考";
            cwin.Header = "驾驶理论考试系统";
            cwin.Show();
            this.Content = ma;
            //cwin.WindowState = C1.WPF.C1WindowState.Maximized;
            //cwin.CenterOnScreen();
            //cwin.Margin = new Thickness(0);
            cwin.Width = SystemParameters.PrimaryScreenWidth;
            cwin.Height = SystemParameters.WorkArea.Height;
        }
    }
}
