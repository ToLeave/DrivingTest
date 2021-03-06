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
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;


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
            this.Closing += F;
        }

        string num = "";
        string time = "";
        List<string> wenhua = new List<string> { "A", "B", "C", "D" };
        public delegate void ChangeTextHandler();//定义委托
        public event ChangeTextHandler ChangeTextEvent;

        private void F(object o, System.ComponentModel.CancelEventArgs e)
        {
            StrikeEvent();
        }

        //触发事件改变MainWindow的值
        private void StrikeEvent()
        {
            if (ChangeTextEvent != null)
            {
                ChangeTextEvent();
            }
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
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/DrivingTest;component/Images/窗体背景.png"));
            b.Stretch = Stretch.Fill;
            grid.Background = b;

            Get_Cpu_Id();//cpu码
            machine_code.Text = ID;
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/DrivingTest;component/Images/窗体背景.png"));
            b.Stretch = Stretch.Fill;
            grid1.Background = b;
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
        //private int cal_license_count(string license)
        //{
        //    int return_num = -1;
        //    for (int step = 0; step < 1000; step++)
        //    {
        //        for (int num = 0; num < 10000; num++)
        //        {
        //            string local_license = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(ID + ":" + num + ":" + step + ":CLOUDTIMESOFT", "MD5");
        //            local_license = local_license.Substring(8, 16);
        //            string local_license_A = local_license.Substring(0, 3);
        //            string local_license_B = local_license.Substring(4);
        //            local_license = local_license_A + "0" + local_license_B;
        //            if (license == local_license)
        //            {
        //                step = 1000;
        //                return_num = num;
        //                num = 10000;
        //            }
        //        }
        //    }
        //    return return_num;
        //}



        //private void thread_license(int license_count, string license)
        //{
        //    Thread newthread = new Thread(new ThreadStart(() =>
        //    {
        //        Dispatcher.Invoke(new Action(() =>
        //        {

        //            for (int license_num = 0; license_num < 1010; license_num++)
        //            {
        //                DateTime endtime = DateTime.Now;
        //                for (DateTime license_time = DateTime.Now; license_time < endtime.AddYears(4); license_time = license_time.AddDays(1))
        //                {
        //                    string num_format = license_time.ToString("yyyyMMdd");
        //                    string local_license = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(ID + ":" + license_num + ":" + num_format + ":" + license_count + ":CLOUDTIMESOFT", "MD5");
        //                    string tt = ID + ":" + license_num + ":" + num_format + ":" + license_count + ":CLOUDTIMESOFT";
        //                    local_license = local_license.Substring(8, 16);

        //                    if (license == local_license)
        //                    {

        //                        //return license_num + "," + license_time;
        //                        license_count = 1000;
        //                        license_num = 10000;
        //                        license_time = license_time.AddYears(10);
        //                    }

        //                }
        //                liucheng_textBox.Text = "license_count;" + license_count + "license_num;" + license_num;
        //                System.Windows.Forms.Application.DoEvents();


        //            }


        //        }));



        //    }));
        //    newthread.SetApartmentState(ApartmentState.MTA);
        //    newthread.IsBackground = false;
        //    //newthread.Priority = ThreadPriority.AboveNormal;
        //    newthread.Start();
        //}



        private string cal_license(string license)
        {
            string ret = "";
            if (license.Length == 16)
            {

                for (int license_num = 0; license_num <= 1000; license_num++)
                {
                    DateTime endtime = DateTime.Now;
                    for (DateTime license_time = DateTime.Now; license_time <= endtime.AddYears(3); license_time = license_time.AddDays(1))
                    {
                        string num_format = license_time.ToString("yyyyMMdd");
                        string local_license = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(ID + ":" + license_num + ":" + num_format + ":CLOUDTIMESOFT", "MD5");
                        //string tt = ID + ":" + license_num + ":" + num_format + ":" + license_count + ":CLOUDTIMESOFT";
                        local_license = local_license.Substring(8, 16);

                        if (license == local_license)
                        {
                            ret = license_num.ToString() + "," + license_time.ToShortDateString();//返回参数为次数 + , + 时间
                            num = license_num.ToString();
                            time = license_time.ToShortDateString();
                            //return license_num + "," + license_time;
                            //license_count = 1000;
                            license_num = 10000;
                            license_time = license_time.AddYears(10);

                        }


                    }
                    //liucheng_textBox.Text = "license_num;" + license_num;
                    //System.Windows.Forms.Application.DoEvents();
                    zhuce_progressBar.Value = license_num / 10;
                    System.Windows.Forms.Application.DoEvents();
                }


                return ret;
            }
            else
            {
                return ret;
            }

        }


        ////根据序列号获取剩余时间
        //private string cal_license_time(string license)
        //{
        //    DateTime return_num = DateTime.Parse("1900/1/1");
        //    for (int step = 0; step < 1000; step++)
        //    {
        //        for (DateTime num = DateTime.Now; num < DateTime.Now.AddYears(3); num = num.AddDays(1))
        //        {
        //            string num_format = num.ToString("yyyyMMdd");
        //            string local_license = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(ID + ":" + num_format + ":" + step + ":CLOUDTIMESOFT", "MD5");
        //            local_license = local_license.Substring(8, 16);
        //            string local_license_A = local_license.Substring(0, 3);
        //            string local_license_B = local_license.Substring(4);
        //            local_license = local_license_A + "1" + local_license_B;
        //            if (license == local_license)
        //            {
        //                step = 1000;
        //                return_num = num;
        //                num.AddYears(4);
        //            }
        //        }
        //    }
        //    return return_num.ToString("yyyy-MM-dd");

        //}

        #endregion

        //复制
        private void fuzhi_button_Click(object sender, RoutedEventArgs e)
        {
            machine_code.Select(0, machine_code.Text.Length);
            this.machine_code.Copy();
            MessageBox.Show("复制成功!");
        }

        //注册
        private void zhuce_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter jiakaoDataSetuserTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter();
            jiakaoDataSetuserTableAdapter.Fill(jiakaoDataSet.user);


            var user = from c in jiakaoDataSet.user select c;

            //int license_count = -1;
            //string license_time = "1900/1/1";
            try
            {
                if (zhuce_textbox.Text != "")
                {
                    string license = zhuce_textbox.Text.Trim().ToUpper();
                    license = license.Replace("-", "");
                    string str = cal_license(license);//调用验证方法

                    if (str != "")
                    {
                        string[] s = str.Split(',');
                        string n = s[0];//脱机码次数
                        string t = s[1];//脱机码时间

                        if (user.Count() == 0)
                        {
                            jiakaoDataSet.user.AdduserRow(-1, machine_code.Text,zhuce_textbox.Text, "", "", "", "", "", "", "", "", "", "", n, t.Replace("-", ""), "1", "", "", "");
                            jiakaoDataSetuserTableAdapter.Update(jiakaoDataSet.user);
                            jiakaoDataSet.user.AcceptChanges();
                            MessageBox.Show("次数:" + n + " 截止日期:" + t + " 激活成功!");
                            PublicClass.tuojizhuce = true;
                        }
                        else
                        {
                            foreach(var u in user)
                            {
                                if (zhuce_textbox.Text == u.password)//防止相同激..活码重复激活
                                {
                                    MessageBox.Show("此激活码已使用!请勿重复激活!");
                                }
                                else
                                {
                                    jiakaoDataSet.user.AdduserRow(-1, machine_code.Text, zhuce_textbox.Text, "", "", "", "", "", "", "", "", "", "", n, t.Replace("-", ""), "1", "", "", "");
                                    jiakaoDataSetuserTableAdapter.Update(jiakaoDataSet.user);
                                    jiakaoDataSet.user.AcceptChanges();
                                    MessageBox.Show("次数:" + n + " 截止日期:" + t + " 激活成功!");
                                    PublicClass.tuojizhuce = true;
                                }
                            }
                        }
                    }
                    else//返回参数为空则为验证不成功
                    {
                        MessageBox.Show("激活失败!激活码错误或无效!");
                    }

                }
                else//验证码为空
                {
                    MessageBox.Show("注册码为空!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/DrivingTest;component/Images/窗体背景.png"));
            b.Stretch = Stretch.Fill;
            this.Background = b;

            if (PublicClass.wuwangluo == true)//断网不显示联网注册
            {
                jiaxiaozhuce.Hide();
            }

            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            wenhua_comboBox.DataContext = wenhua;


            var setting = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
            foreach (var se in setting)
            {
                if (se.registration_display != "")
                {
                    PublicClass.xinxi = se.registration_display.Split(',');
                }
                else
                {
                    PublicClass.xinxi = new string[5] { "", "", "", "", "" };
                }
                if (PublicClass.xinxi[0] == "1")
                {
                    liucheng_textBox.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    liucheng_textBox.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.xinxi[1] == "1")
                {
                    label4.Visibility = System.Windows.Visibility.Hidden;
                    lianjie_textBlock.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    label4.Visibility = System.Windows.Visibility.Visible;
                    lianjie_textBlock.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.xinxi[2] == "1")
                {
                    label6.Visibility = System.Windows.Visibility.Hidden;
                    qq_textblock.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    label6.Visibility = System.Windows.Visibility.Visible;
                    qq_textblock.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.xinxi[3] == "1")
                {
                    label7.Visibility = System.Windows.Visibility.Hidden;
                    ww_textblock.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    label7.Visibility = System.Windows.Visibility.Visible;
                    ww_textblock.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.xinxi[4] == "1")
                {
                    beizhu_textblock.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    beizhu_textblock.Visibility = System.Windows.Visibility.Visible;
                }

                liucheng_textBox.Text = se.registration_process;
                if (se.payment_link != "")
                {
                    if (se.payment_link.Substring(0, 8) == "https://" && se.payment_link.Substring(0, 7) == "http://")
                    {
                        HypeLink.NavigateUri = new Uri(se.payment_link);
                    }
                    else
                    {
                        HypeLink.NavigateUri = new Uri("https://" + se.payment_link);
                    }
                }
                else
                {
                    HypeLink.NavigateUri = new Uri("https://www.baidu.com");
                }
                qq_textblock.Text = se.customer_service_QQ;
                ww_textblock.Text = se.customer_service_WW;
                beizhu_textblock.Text = se.comments;
            }
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

        //上传本地图片
        private void shangchuan_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "图片文件 (*.png*.jpg*.jpeg)|*.png;*.jpg;*.jpeg|PNG files (*.png)|*.png|JPG files (*.jpg*.jpeg)|*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                zhaopian_textBox.Text = dialog.FileName;
            }
        }

        private void jiaxiaozhuce_IsActiveChanged(object sender, EventArgs e)
        {
            nan_radioButton.IsChecked = true;
            geren_checkBox.IsChecked = true;
            bianhao_textBox.IsEnabled = false;
        }

        //确认注册
        private void queren_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter jiakaoDataSetuserTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter();
            jiakaoDataSetuserTableAdapter.Fill(jiakaoDataSet.user);

            if (zhanghao_textBox.Text != "" && pass_textBox.Password != "" && passwoed_textBox.Password != "" && name_textBox.Text != "" && phone_textBox.Text != "" && idcard_textBox.Text != "")//账号密码姓名手机号身份证号不能为空
            {
                if (pass_textBox.Password == passwoed_textBox.Password)//密码要相同
                {
                    string loginstr = null;
                    HttpWebResponse response = null;
                    StreamReader reader = null;


                    string login = zhanghao_textBox.Text;//账号
                    string password = passwoed_textBox.Password;//密码
                    string name = name_textBox.Text;//名字
                    string phone = phone_textBox.Text;//手机号
                    string sex = "";//性别
                    if (nan_radioButton.IsChecked == true)
                    {
                        sex = "男";
                    }
                    else
                    {
                        sex = "女";
                    }
                    string idcard = idcard_textBox.Text;//身份证号
                    string chengdu = wenhua_comboBox.SelectionBoxItem.ToString();//学员程度
                    string bianhao = bianhao_textBox.Text;//学员编号
                    string jine = shoufei_textBox.Text;//收费金额

                    try
                    {
                        string url = PublicClass.http + @"/returnjsons/reguser?login=" + login + "&password=" + password + "&name=" + name + "&phone=" + phone + "&sex=" + sex + "&idcard=" + idcard + "&education=" + chengdu + "&studentid=" + bianhao + "&money=" + jine;
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//注册 url
                        request.Method = "GET";
                        request.Timeout = 10000;
                        response = (HttpWebResponse)request.GetResponse();
                        reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                        loginstr = reader.ReadToEnd();

                        JArray loginUUID_json = JArray.Parse(loginstr);//json

                        string state = loginUUID_json[0]["status"].ToString();//获取返回值

                        if (state == "0")
                        {
                            MessageBox.Show("此账号已被注册!请重试!");
                        }
                        else if (state == "1")
                        {
                            MessageBox.Show("此手机号已被注册!请重试!");
                        }
                        else if (state == "2")
                        {
                            MessageBox.Show("注册成功!请等待管理员审核!");

                            string imagepath = System.Windows.Forms.Application.StartupPath + "\\Image\\User\\";
                            if (!Directory.Exists(imagepath))//如果路径不存在
                            {
                                Directory.CreateDirectory(imagepath);//创建一个路径的文件夹
                            }
                            string path = imagepath + login + ".jpg";

                            File.Copy(zhaopian_textBox.Text, path,true);//源文件目录,目标目录
                            this.Close();
                        }

                        response.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("两次密码不一致!");
                }
            }
            else
            {
                string zh = "";
                string pa = "";
                string pass = "";
                string na = "";
                string ph = "";
                string id = "";
                if (zhanghao_textBox.Text == "")
                {
                    zh = "账号";
                }
                if (pass_textBox.Password == "")
                {
                    pa = "密码";
                }
                if (passwoed_textBox.Password == "")
                {
                    pass = "确认密码";
                }
                if (name_textBox.Text == "")
                {
                    na = "姓名";
                }
                if (phone_textBox.Text == "")
                {
                    ph = "手机号";
                }
                if (idcard_textBox.Text == "")
                {
                    id = "身份证号";
                }
                MessageBox.Show(zh + pa + pass + na + ph + id + "不能为空!");
            }


        }

        private void geren_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (geren_checkBox.IsChecked == true)
            {
                bianhao_textBox.IsEnabled = false;
                jiaxiao_checkBox.IsChecked = false;
            }
        }
        private void geren_checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (geren_checkBox.IsChecked == false)
            {
                jiaxiao_checkBox.IsChecked = true;
                bianhao_textBox.IsEnabled = true;
                geren_checkBox.IsChecked = false;
            }
        }
        private void jiaxiao_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (jiaxiao_checkBox.IsChecked == true)
            {
                bianhao_textBox.IsEnabled = true;
                geren_checkBox.IsChecked = false;
            }
        }
        private void jiaxiao_checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (jiaxiao_checkBox.IsChecked == false)
            {
                geren_checkBox.IsChecked = true;
                bianhao_textBox.IsEnabled = false;
                jiaxiao_checkBox.IsChecked = false;
            }
        }

        //确认密码焦点离开
        private void passwoed_textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pass_textBox.Password != passwoed_textBox.Password)//密码不一致
            {
                tishi_textBlock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                tishi_textBlock.Visibility = System.Windows.Visibility.Hidden;
            }
        }





    }


}
