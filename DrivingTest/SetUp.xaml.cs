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
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Reflection;

namespace DrivingTest
{
    /// <summary>
    /// SetUp.xaml 的交互逻辑
    /// </summary>
    public partial class SetUp : Window
    {
        public SetUp()
        {
            InitializeComponent();
        }

        bool power_on = false;//开机启动变量


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter jiakaoDataSetuserTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter();
            jiakaoDataSetuserTableAdapter.Fill(jiakaoDataSet.user);



        }

        //设置开机启动
        private void kaiji_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting select c;
            foreach (var s in set)
            {
                if (kaiji_checkBox.IsChecked == true) //设置开机自启动  
                {
                    s.power_on = 1;

                    if (s.power_on == 1)
                    {
                        power_on = true;//开机启动
                        MessageBox.Show("设置开机自启动，需要修改注册表", "提示");
                        string path = Assembly.GetExecutingAssembly().Location; ;
                        RegistryKey rk = Registry.LocalMachine;
                        RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                        rk2.SetValue("JcShutdown", path);
                        rk2.Close();
                        rk.Close();
                    }
                }
                else //取消开机自启动  
                {
                    s.power_on = 0;

                    if (s.power_on == 0)
                    {
                        power_on = false;//开机不启动
                        MessageBox.Show("取消开机自启动，需要修改注册表", "提示");
                        string path = Assembly.GetExecutingAssembly().Location;
                        RegistryKey rk = Registry.LocalMachine;
                        RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                        rk2.DeleteValue("JcShutdown", false);
                        rk2.Close();
                        rk.Close();
                    }
                }
            }



        }

        //个人模式
        private void geren_radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (geren_radioButton.IsChecked == true)
            {
                geren_grid.Visibility = System.Windows.Visibility.Visible;
                jiaxiao_grid.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        //驾校模式
        private void jiaxiao_radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (jiaxiao_radioButton.IsChecked == true)
            {
                jiaxiao_grid.Visibility = System.Windows.Visibility.Visible;
                geren_grid.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        //基本设置
        private void jibenshezhi_IsActiveChanged(object sender, EventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            if (jibenshezhi.IsActive == true)
            {
                geren_radioButton.IsChecked = true;
            }

            var set = from c in jiakaoDataSet.setting select c;
            //
            foreach (var s in set)
            {
                if (s.power_on == 1)
                {
                    power_on = true;
                }
                else
                {
                    power_on = false;
                }
            }
        }




        //保存设置密码和退出密码
        private void baocun_passbutton_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting select c;

            foreach (var s in set)
            {
                if (tuipassword_textBox.Text == quepassword_textBox.Text)
                {
                    s.close_password = tuipassword_textBox.Text;
                }
                else
                {
                    MessageBox.Show("两次密码不一致!", "提示");
                }
            }



        }

        //浏览图片
        private void liulan_button_Click(object sender, RoutedEventArgs e)
        {

        }

        //基本设置保存
        private void jibenshezhi_button_Click(object sender, RoutedEventArgs e)
        {

        }

        //信息设置保存
        private void xinxibaocun_button_Click(object sender, RoutedEventArgs e)
        {

        }

        //键盘设置保存
        private void jianpanshezhi_button_Click(object sender, RoutedEventArgs e)
        {

        }

        //验证输入
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text.Trim().Length > 0)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
            if (textBox.Text.Length >= 1)
            {
                e.Handled = true;
            }


        }

        //屏蔽中文输入和非法字符粘贴输入
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TextBox textBox = sender as TextBox;
            //TextChange[] change = new TextChange[e.Changes.Count];
            //e.Changes.CopyTo(change, 0);

            //int offset = change[0].Offset;
            //if (change[0].AddedLength > 0)
            //{
            //    double num = 0;
            //    if (!Double.TryParse(textBox.Text, out num))
            //    {
            //        textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
            //        textBox.Select(offset, 0);
            //    }
            //}
        }

        //禁用所有text
        private void readonly_false()
        {
            a_textBox.IsReadOnly = true;
            b_textBox.IsReadOnly = true;
            c_textBox.IsReadOnly = true;
            d_textBox.IsReadOnly = true;
            dui_textBox.IsReadOnly = true;
            cuo_textBox.IsReadOnly = true;
            up_textBox.IsReadOnly = true;
            down_textBox.IsReadOnly = true;
            head_textBox.IsReadOnly = true;
            end_textBox.IsReadOnly = true;
            jiaojuan_textBox.IsReadOnly = true;
            queren_textBox.IsReadOnly = true;
        }
        //启用所有text
        private void readonly_true()
        {
            a_textBox.IsReadOnly = false;
            b_textBox.IsReadOnly = false;
            c_textBox.IsReadOnly = false;
            d_textBox.IsReadOnly = false;
            dui_textBox.IsReadOnly = false;
            cuo_textBox.IsReadOnly = false;
            up_textBox.IsReadOnly = false;
            down_textBox.IsReadOnly = false;
            head_textBox.IsReadOnly = false;
            end_textBox.IsReadOnly = false;
            jiaojuan_textBox.IsReadOnly = false;
            queren_textBox.IsReadOnly = false;
        }

        //
        private void jianpanshezhi_IsActiveChanged(object sender, EventArgs e)
        {
            if (jianpanshezhi.IsActive == true)
            {
                radioButton20.IsChecked = true;
            }
        }

        //方案一单选框
        private void radioButton20_Checked(object sender, RoutedEventArgs e)
        {
            readonly_false();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\方案一.png", UriKind.Relative));

        }
        //方案二单选框
        private void radioButton21_Checked(object sender, RoutedEventArgs e)
        {
            readonly_false();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\方案二.png", UriKind.Relative));
        }
        //方案三单选框
        private void radioButton22_Checked(object sender, RoutedEventArgs e)
        {
            readonly_false();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\方案三.png", UriKind.Relative));
        }
        //方案四单选框
        private void radioButton23_Checked(object sender, RoutedEventArgs e)
        {
            readonly_false();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\方案四.png", UriKind.Relative));
        }
        //方案五单选框
        private void radioButton24_Checked(object sender, RoutedEventArgs e)
        {
            readonly_false();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\方案五.png", UriKind.Relative));
        }
        //自定义方案单选框
        private void radioButton25_Checked(object sender, RoutedEventArgs e)
        {
            readonly_true();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\自定义.png", UriKind.Relative));
        }

    }
}
public class Employee
{
    public string Name { set; get; }
    public int EmpID { set; get; }
}

public class EmployeeArr : ObservableCollection<Employee>
{
    public EmployeeArr()
    {
        this.Add(new Employee { EmpID = 1, Name = "北京" });
        this.Add(new Employee { EmpID = 2, Name = "天津" });
        this.Add(new Employee { EmpID = 3, Name = "上海" });
        this.Add(new Employee { EmpID = 4, Name = "重庆" });
        this.Add(new Employee { EmpID = 5, Name = "河北" });
        this.Add(new Employee { EmpID = 6, Name = "山西" });
        this.Add(new Employee { EmpID = 7, Name = "辽宁" });
        this.Add(new Employee { EmpID = 8, Name = "吉林" });
        this.Add(new Employee { EmpID = 9, Name = "黑龙江" });
        this.Add(new Employee { EmpID = 10, Name = "江苏" });
        this.Add(new Employee { EmpID = 11, Name = "浙江" });
        this.Add(new Employee { EmpID = 12, Name = "安徽" });
        this.Add(new Employee { EmpID = 13, Name = "福建" });
        this.Add(new Employee { EmpID = 14, Name = "江西" });
        this.Add(new Employee { EmpID = 15, Name = "山东" });
        this.Add(new Employee { EmpID = 16, Name = "河南" });
        this.Add(new Employee { EmpID = 17, Name = "湖北" });
        this.Add(new Employee { EmpID = 18, Name = "湖南" });
        this.Add(new Employee { EmpID = 19, Name = "广东" });
        this.Add(new Employee { EmpID = 20, Name = "海南" });
        this.Add(new Employee { EmpID = 21, Name = "四川" });
        this.Add(new Employee { EmpID = 22, Name = "贵州" });
        this.Add(new Employee { EmpID = 23, Name = "云南" });
        this.Add(new Employee { EmpID = 24, Name = "陕西" });
        this.Add(new Employee { EmpID = 25, Name = "甘肃" });
        this.Add(new Employee { EmpID = 26, Name = "青海" });
        this.Add(new Employee { EmpID = 27, Name = "台湾" });
        this.Add(new Employee { EmpID = 28, Name = "内蒙古" });
        this.Add(new Employee { EmpID = 29, Name = "广西" });
        this.Add(new Employee { EmpID = 30, Name = "宁夏" });
        this.Add(new Employee { EmpID = 31, Name = "新疆" });
        this.Add(new Employee { EmpID = 32, Name = "西藏" });
    }
}
