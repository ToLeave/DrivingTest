﻿using System;
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
using System.Management;
using System.Diagnostics;
using System.Security.Cryptography;


namespace DrivingTest
{
    /// <summary>
    /// register.xaml 的交互逻辑
    /// </summary>
    public partial class register : Window
    {
        public register()
        {
            InitializeComponent();
        }

        #region 获取显示机器码
        /// <summary>
        /// 读取CPU机器码
        /// </summary>
        /// <returns></returns>
        public static string ID = "";

        public static string Get_Cpu_Id()
        {
            string cpuInfo = " ";

            using (ManagementClass cimobject = new ManagementClass("Win32_Processor"))
            {
                ManagementObjectCollection moc = cimobject.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                    mo.Dispose();
                }
            }
            ID = cpuInfo;
            return cpuInfo.ToString();



        }
        /// <summary>
        /// 读取硬盘机器码
        /// </summary>
        /// <returns></returns>
        public static string Get_HardDisk_Id()
        {
            string HDid = " ";
            using (ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive"))
            {
                ManagementObjectCollection moc1 = cimobject1.GetInstances();
                foreach (ManagementObject mo in moc1)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                    mo.Dispose();
                }
            }
            //ID = HDid;
            return HDid.ToString();
        }
        /// <summary>
        /// 读取网卡机器码
        /// </summary>
        /// <returns></returns>
        public static string Get_NetworkCard_Id()
        {
            string MoAddress = " ";
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                ManagementObjectCollection moc2 = mc.GetInstances();
                foreach (ManagementObject mo in moc2)
                {
                    if ((bool)mo["IPEnabled"] == true)
                        MoAddress = mo["MacAddress"].ToString();
                    mo.Dispose();
                }
            }
            return MoAddress.ToString();
        }

        //提出机器码显示
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Get_Cpu_Id();//cpu码
            machine_code.Text = ID;
        }

        #endregion

        //付款链接调用浏览器
        private void HypeLink1_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        #region 序列号验证
        //根据序列号获取剩余次数
        private int cal_license_count(string license)
        {
            int return_num = -1;
            for (int step = 0; step < 1000; step++)
            {
                for (int num = 0; num < 10000; num++)
                {
                    string local_license = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(ID + ":" + num + ":" + step + ":CLOUDTIMESOFT", "MD5");
                    local_license = local_license.Substring(8, 16);
                    string local_license_A = local_license.Substring(0, 3);
                    string local_license_B = local_license.Substring(4);
                    local_license = local_license_A + "0" + local_license_B;
                    if (license == local_license)
                    {
                        step = 1000;
                        return_num = num;
                        num = 10000;
                    }
                }
            }
            return return_num;
        }

        //根据序列号获取剩余时间
        private string cal_license_time(string license)
        {
            DateTime return_num = DateTime.Parse("1900/1/1");
            for (int step = 0; step < 1000; step++)
            {
                for (DateTime num = DateTime.Now; num < DateTime.Now.AddYears(3); num = num.AddDays(1))
                {
                    string num_format = num.ToString("yyyyMMdd");
                    string local_license = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(ID + ":" + num_format + ":" + step + ":CLOUDTIMESOFT", "MD5");
                    local_license = local_license.Substring(8, 16);
                    string local_license_A = local_license.Substring(0, 3);
                    string local_license_B = local_license.Substring(4);
                    local_license = local_license_A + "1" + local_license_B;
                    if (license == local_license)
                    {
                        step = 1000;
                        return_num = num;
                        num.AddYears(4);
                    }
                }
            }
            return return_num.ToString("yyyy-MM-dd");

        }

        #endregion

        //复制
        private void fuzhi_button_Click(object sender, RoutedEventArgs e)
        {
            machine_code.Select(0, machine_code.Text.Length);
            this.machine_code.Copy();
        }

        //注册
        private void zhuce_button_Click(object sender, RoutedEventArgs e)
        {
            int license_count = -1;
            string license_time = "1900/1/1";
            try
            {
                string license = zhuce_textbox.Text.Trim().ToUpper();
                string license_type = license.Substring(3, 1);
                if (license_type == "0")
                {
                    license_count = cal_license_count(license);
                    MessageBox.Show(license_count.ToString());
                }
                else
                {
                    license_time = cal_license_time(license);
                    MessageBox.Show(license_time);
                }
            }
            catch
            { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。

            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);


        }

        //注册码文本框格式
        private void zhuce_textbox_KeyUp(object sender, KeyEventArgs e)
        {

            if (zhuce_textbox.Text.Length >= 4)
            {
                string s = zhuce_textbox.Text;
                string ss = "";
                string str_temp = "";
                for (int i = 0; i < s.Length; i++)
                {
                    if (((i + 1) % 5 == 0) && (s[(i)] != '-'))
                        str_temp = "-" + s[i];
                    else
                        str_temp = s[i].ToString();
                    ss = ss + str_temp;
                }
                zhuce_textbox.Text = ss;
                zhuce_textbox.SelectionStart = zhuce_textbox.Text.Length;
            }
            zhuce_textbox.Text = zhuce_textbox.Text.ToUpper();
            zhuce_textbox.SelectionStart = zhuce_textbox.Text.Length;

        }

    }


}
