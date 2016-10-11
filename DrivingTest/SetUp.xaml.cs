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


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);
            System.Windows.Data.CollectionViewSource settingViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("settingViewSource")));
            settingViewSource.View.MoveCurrentToFirst();






        }

        //设置开机启动
        private void kaiji_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (kaiji_checkBox.IsChecked == true) //设置开机自启动  
            {
                MessageBox.Show("设置开机自启动，需要修改注册表", "提示");
                string path = Assembly.GetExecutingAssembly().Location; ;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.SetValue("JcShutdown", path);
                rk2.Close();
                rk.Close();
            }
            else //取消开机自启动  
            {
                MessageBox.Show("取消开机自启动，需要修改注册表", "提示");
                string path = Assembly.GetExecutingAssembly().Location;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.DeleteValue("JcShutdown", false);
                rk2.Close();
                rk.Close();
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
