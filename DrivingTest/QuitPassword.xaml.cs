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
    /// QuitPassword.xaml 的交互逻辑
    /// </summary>
    public partial class QuitPassword : UserControl
    {
        public QuitPassword()
        {
            InitializeComponent();
        }

        private void ok_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
            foreach (var s in set)
            {
                if (PublicClass.shezhi == "设置")
                {
                    if (password_textBox.Text == s.password)
                    {
                        SetUp se = new SetUp();
                        se.Show();
                    }
                    else
                    {
                        MessageBox.Show("密码错误!", "提示");
                    }
                }
                else
                {
                    if (password_textBox.Text == s.password)
                    {
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        MessageBox.Show("密码错误!", "提示");
                    }
                }
            }
        }

        private void no_button_Click(object sender, RoutedEventArgs e)
        {
            C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "管理员密码");
            if (cp != null)
            {
                cp.Close();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/DrivingTest;component/Images/窗体背景.png"));
            b.Stretch = Stretch.Fill;
            this.Background = b;
        }

        private void password_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ok_button_Click(null, null);
            }
        }
    }
}
