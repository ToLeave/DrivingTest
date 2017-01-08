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
    /// Notice.xaml 的交互逻辑
    /// </summary>
    public partial class Notice : Window
    {
        public Notice()
        {
            InitializeComponent();
        }

        private void Close_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            if (xianshi_checkBox.IsChecked == true)
            {
                var setting = from c in jiakaoDataSet.setting where c.setting_id.ToString() == "1" select c;
                foreach (var se in setting)
                {
                    se.not_show = int.Parse("1");
                }
                jiakaoDataSetsettingTableAdapter.Update(jiakaoDataSet.setting);
                jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);
                jiakaoDataSet.setting.AcceptChanges();
            }
            else
            {
                this.Close();
            }
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/DrivingTest;component/Images/窗体背景.png"));
            b.Stretch = Stretch.Fill;
            this.Background = b;
        }



    }
}
