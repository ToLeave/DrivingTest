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

            var set = from c in jiakaoDataSet.setting select c;
            foreach (var s in set)
            {
 
            }
        }

        private void no_button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
