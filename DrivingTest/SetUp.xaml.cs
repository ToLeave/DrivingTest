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
using System.IO;

namespace DrivingTest
{
    /// <summary>
    /// SetUp.xaml 的交互逻辑
    /// </summary>
    public partial class SetUp : Window
    {
        public delegate void ChangeTextHandler();//定义委托
        public event ChangeTextHandler ChangeTextEvent;

        public SetUp()
        {
            InitializeComponent();
            this.Closing += F;
        }

        bool power_on = false;//开机启动变量
        string[] interim_key;//临时快捷键数组 索引从0开始依次为 A;B;C;D;对;错;上一题;下一题;第一题;最后题;交卷;确认交卷

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
            //DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            //// 将数据加载到表 setting 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            //jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            //var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;

            //foreach (var s in set)
            //{
            //    if (kaiji_checkBox.IsChecked == true) //设置开机自启动  
            //    {

            //        s.power_on = 1; 
            //    }
            //}
            //jiakaoDataSetsettingTableAdapter.Update(jiakaoDataSet.setting);
            ////jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting); 
            //jiakaoDataSet.setting.AcceptChanges();

        }
        private void kaiji_checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            //// 将数据加载到表 setting 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            //jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            //var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
            //foreach (var s in set)
            //{
            //    if (kaiji_checkBox.IsChecked == false) //取消开机自启动  
            //    {

            //        s.power_on = 0;
            //    }
            //}
            //jiakaoDataSetsettingTableAdapter.Update(jiakaoDataSet.setting);
            ////jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);
            //jiakaoDataSet.setting.AcceptChanges();
        }

        //题库包含地方题库
        private void difan_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (difan_checkBox.IsChecked == true)
            {
                shengfen_comboBox.IsEnabled = true;
            }

        }
        private void difan_checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (difan_checkBox.IsChecked == false)
            {
                shengfen_comboBox.IsEnabled = false;
            }
        }

        //关闭软件时需要密码
        private void guanbi_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (guanbi_checkBox.IsChecked == true)//启用
            {
            }
        }
        private void guanbi_checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (guanbi_checkBox.IsChecked == false)//不启用
            {
            }
        }

        //功能模块
        private void gongneng_checkBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void gongneng_checkBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        //个人模式
        private void geren_radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (geren_radioButton.IsChecked == true)
            {
                geren_grid.Visibility = System.Windows.Visibility.Visible;
                jiaxiao_grid.Visibility = System.Windows.Visibility.Hidden;
                guanbi_checkBox.IsEnabled = false;
                label6.IsEnabled = false;
                tuipassword_textBox.IsEnabled = false;
                label7.IsEnabled = false;
                quepassword_textBox.IsEnabled = false;
                baocun_passbutton.IsEnabled = false;

            }
        }
        //驾校模式
        private void jiaxiao_radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (jiaxiao_radioButton.IsChecked == true)
            {
                jiaxiao_grid.Visibility = System.Windows.Visibility.Visible;
                geren_grid.Visibility = System.Windows.Visibility.Hidden;
                guanbi_checkBox.IsEnabled = true;
                label6.IsEnabled = true;
                tuipassword_textBox.IsEnabled = true;
                label7.IsEnabled = true;
                quepassword_textBox.IsEnabled = true;
                baocun_passbutton.IsEnabled = true;
            }
        }

        //基本设置选项卡
        private void jibenshezhi_IsActiveChanged(object sender, EventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;

            if (jibenshezhi.IsActive == true)
            {
                foreach (var s in set)
                {
                    if (s.province != "")//是否有省份
                    {
                        shengfen_comboBox.SelectedIndex = int.Parse(s.province);
                        difan_checkBox.IsChecked = true;
                    }
                    else
                    {
                        shengfen_comboBox.IsEnabled = false;
                    }

                    if (s.model == 0)//个人还是驾校模式
                    {
                        geren_radioButton.IsChecked = true;
                    }
                    else
                    {
                        jiaxiao_radioButton.IsChecked = true;
                    }

                    //是否有驾照类型

                    if (s.power_on == 0)//是否开机启动
                    {
                        kaiji_checkBox.IsChecked = false;
                    }
                    else
                    {
                        kaiji_checkBox.IsChecked = true;
                    }

                    if (s.close_password == 0)//是否有开机及设置密码
                    {
                        guanbi_checkBox.IsChecked = false;
                    }
                    else
                    {
                        guanbi_checkBox.IsChecked = true;
                    }

                    if (s.show_notification == 0)//是否显示最新通知
                    {
                        tongzhi_checkBox.IsChecked = false;
                    }
                    else
                    {
                        tongzhi_checkBox.IsChecked = true;
                    }

                    kaotai_textBox.Text = s.testbench_number.ToString();//考台号赋值


                }
            }

        }
        //功能标题选项卡
        private void gongnegnbiaoti_IsActiveChanged(object sender, EventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;

            if (gongnegnbiaoti.IsActive == true)
            {
                foreach (var s in set)
                {
                    if (s.functional_module != "")
                    {
                        PublicClass.gongneng = s.functional_module.Split(',');
                    }
                    else
                    {
                        PublicClass.gongneng = new string[6] { "0", "0", "0", "0", "0", "0" };
                    }
                    if (s.voice_prompt != "")
                    {
                        PublicClass.yuyin = s.voice_prompt.Split(',');
                    }
                    else
                    {
                        PublicClass.yuyin = new string[3] { "0", "0", "0" };
                    }

                    biaoti_textBox.Text = s.software_title;

                    if (PublicClass.gongneng[0] == "1")
                    {
                        zhangjie_checkBox.IsChecked = true;
                    }
                    else
                    {
                        zhangjie_checkBox.IsChecked = false;
                    }
                    if (PublicClass.gongneng[1] == "1")
                    {
                        shunxu_checkBox.IsChecked = true;
                    }
                    else
                    {
                        shunxu_checkBox.IsChecked = false;
                    }
                    if (PublicClass.gongneng[2] == "1")
                    {
                        suiji_checkBox.IsChecked = true;
                    }
                    else
                    {
                        suiji_checkBox.IsChecked = false;
                    }
                    if (PublicClass.gongneng[3] == "1")
                    {
                        zhuanxiang_checkBox.IsChecked = true;
                    }
                    else
                    {
                        zhuanxiang_checkBox.IsChecked = false;
                    }
                    if (PublicClass.gongneng[4] == "1")
                    {
                        moni_checkBox.IsChecked = true;
                    }
                    else
                    {
                        moni_checkBox.IsChecked = false;
                    }
                    if (PublicClass.gongneng[5] == "1")
                    {
                        cuoti_checkBox.IsChecked = true;
                    }
                    else
                    {
                        cuoti_checkBox.IsChecked = false;
                    }
                    if (s.subject_module != "")
                    {
                        if (s.subject_module.Substring(0, 1) == "1")
                        {
                            kemu2_checkBox.IsChecked = true;
                        }
                        else
                        {
                            kemu2_checkBox.IsChecked = false;
                        }
                        if (s.subject_module.Substring(2, 1) == "1")
                        {
                            kemu3_checkBox.IsChecked = true;
                        }
                        else
                        {
                            kemu3_checkBox.IsChecked = false;
                        }
                    }
                    else
                    {
                        kemu2_checkBox.IsChecked = false;
                        kemu3_checkBox.IsChecked = false;
                    }

                    if (s.phonetic_reading == 0)
                    {
                        nv_radioButton.IsChecked = true;
                    }
                    else if (s.phonetic_reading == 1)
                    {
                        nan_radioButton.IsChecked = true;
                    }
                    else
                    {
                        guan_radioButton.IsChecked = true;
                    }

                    if (PublicClass.yuyin[0] == "1")
                    {
                        jiangjie_checkBox.IsChecked = true;
                    }
                    else
                    {
                        jiangjie_checkBox.IsChecked = false;
                    }
                    if (PublicClass.yuyin[1] == "1")
                    {
                        duicuo_checkBox.IsChecked = true;
                    }
                    else
                    {
                        duicuo_checkBox.IsChecked = false;
                    }
                    if (PublicClass.yuyin[2] == "1")
                    {
                        dacuo_checkBox.IsChecked = true;
                    }
                    else
                    {
                        dacuo_checkBox.IsChecked = false;
                    }

                    if (s.next_question == 0)
                    {
                        shoudong_radioButton.IsChecked = true;
                    }
                    else if (s.next_question == 1)
                    {
                        zidong_radioButton.IsChecked = true;
                    }
                    else
                    {
                        dadui_radioButton.IsChecked = true;
                    }

                    if (s.display_answers == 1)
                    {
                        zhengque_checkBox.IsChecked = true;
                    }
                    else
                    {
                        zhengque_checkBox.IsChecked = false;
                    }

                    if (s.display_test == "1")
                    {
                        chongkao_checkBox.IsChecked = true;
                    }
                    else
                    {
                        chongkao_checkBox.IsChecked = false;
                    }


                    kemu4_textBox.Text = s.subject_four_button;


                }
                #region 个人与驾校模式的控件显示与隐藏
                if (geren_radioButton.IsChecked == true)//个人模式隐藏选择驾照类型-设置,功能模块和学习统计-驾校信息设置
                {
                    checkBox5.IsEnabled = false;
                    checkBox6.IsEnabled = false;
                    checkBox7.IsEnabled = false;
                    gundong_textBox.IsEnabled = false;
                    label9.IsEnabled = false;
                    slider1.IsEnabled = false;
                    label10.IsEnabled = false;
                    label11.IsEnabled = false;

                    zhangjie_checkBox.IsEnabled = false;
                    shunxu_checkBox.IsEnabled = false;
                    suiji_checkBox.IsEnabled = false;
                    zhuanxiang_checkBox.IsEnabled = false;
                    moni_checkBox.IsEnabled = false;
                    cuoti_checkBox.IsEnabled = false;

                    label12.IsEnabled = false;
                    checkBox17.IsEnabled = false;
                    label13.IsEnabled = false;
                    radioButton65.IsEnabled = false;
                    yukao_textBox.IsEnabled = false;
                    label14.IsEnabled = false;
                    radioButton66.IsEnabled = false;
                    //checkBox18.IsEnabled = false;

                }
                else if (jiaxiao_radioButton.IsChecked == true)//驾校模式显示选择驾照类型-设置,功能模块和学习统计-驾校信息设置
                {
                    checkBox5.IsEnabled = true;
                    checkBox6.IsEnabled = true;
                    checkBox7.IsEnabled = true;
                    gundong_textBox.IsEnabled = true;
                    label9.IsEnabled = true;
                    slider1.IsEnabled = true;
                    label10.IsEnabled = true;
                    label11.IsEnabled = true;

                    zhangjie_checkBox.IsEnabled = true;
                    shunxu_checkBox.IsEnabled = true;
                    suiji_checkBox.IsEnabled = true;
                    zhuanxiang_checkBox.IsEnabled = true;
                    moni_checkBox.IsEnabled = true;
                    cuoti_checkBox.IsEnabled = true;

                    label12.IsEnabled = true;
                    checkBox17.IsEnabled = true;
                    label13.IsEnabled = true;
                    radioButton65.IsEnabled = true;
                    yukao_textBox.IsEnabled = true;
                    label14.IsEnabled = true;
                    radioButton66.IsEnabled = true;
                    //checkBox18.IsEnabled = true;
                }
                #endregion
            }

        }

        //信息设置选项卡
        private void xingixshezhi_IsActiveChanged(object sender, EventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;

            if (xinxishezhi.IsActive == true)
            {
                foreach (var s in set)
                {
                    if (s.registration_display != "")
                    {
                        PublicClass.xinxi = s.registration_display.Split(',');
                    }
                    else
                    {
                        PublicClass.xinxi = new string[5] { "", "", "", "", "" };
                    }

                    if (PublicClass.xinxi[0] == "1")
                    {
                        liucheng_checkBox.IsChecked = true;
                    }
                    else
                    {
                        liucheng_checkBox.IsChecked = false;
                    }
                    if (PublicClass.xinxi[1] == "1")
                    {
                        lianjie_checkBox.IsChecked = true;
                    }
                    else
                    {
                        lianjie_checkBox.IsChecked = false;
                    }
                    if (PublicClass.xinxi[2] == "1")
                    {
                        QQ_checkBox.IsChecked = true;
                    }
                    else
                    {
                        QQ_checkBox.IsChecked = false;
                    }
                    if (PublicClass.xinxi[3] == "1")
                    {
                        WW_checkBox.IsChecked = true;
                    }
                    else
                    {
                        WW_checkBox.IsChecked = false;
                    }
                    if (PublicClass.xinxi[4] == "1")
                    {
                        beizhu_checkBox.IsChecked = true;
                    }
                    else
                    {
                        beizhu_checkBox.IsChecked = false;
                    }

                    liucheng_textBox.Text = s.registration_process;
                    lianjie_textBox.Text = s.payment_link;
                    QQ_textBox.Text = s.customer_service_QQ;
                    WW_textBox.Text = s.customer_service_WW;
                    beizhu_textBox.Text = s.comments;


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

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;

            foreach (var s in set)
            {
                if (tuipassword_textBox.Text == quepassword_textBox.Text)
                {
                    s.password = tuipassword_textBox.Text;
                    MessageBox.Show("密码设置成功!", "提示");
                }
                else
                {
                    MessageBox.Show("两次密码不一致!", "提示");
                }
            }
            jiakaoDataSetsettingTableAdapter.Update(jiakaoDataSet.setting);
            jiakaoDataSet.setting.AcceptChanges();
        }

        //浏览图片
        private void liulan_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "图片文件 (*.png*.jpg*.jpeg)|*.png;*.jpg;*.jpeg|PNG files (*.png)|*.png|JPG files (*.jpg*.jpeg)|*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                tupian_textBox.Text = dialog.FileName;
            }

        }

        //基本设置保存
        private void jibenshezhi_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;

            foreach (var s in set)
            {
                if (geren_radioButton.IsChecked == true)//个人保存
                {
                    if (difan_checkBox.IsChecked == true)
                    {
                        s.province = shengfen_comboBox.SelectedValue.ToString();
                    }
                    else
                    {
                        s.province = "";
                    }
                    if (kaiji_checkBox.IsChecked == true)
                    {
                        string path = Assembly.GetExecutingAssembly().Location; ;
                        MessageBox.Show("设置开机自启动，需要修改注册表", "提示");
                        RegistryKey rk = Registry.LocalMachine;
                        RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                        rk2.SetValue("JcShutdown", path);
                        rk2.Close();
                        rk.Close();
                        s.power_on = 1;
                    }
                    else
                    {
                        string path = Assembly.GetExecutingAssembly().Location;
                        MessageBox.Show("取消开机自启动，需要修改注册表", "提示");
                        RegistryKey rk = Registry.LocalMachine;
                        RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                        rk2.DeleteValue("JcShutdown", false);
                        rk2.Close();
                        rk.Close();
                        s.power_on = 0;
                    }
                    if (tongzhi_checkBox.IsChecked == true)
                    {
                        s.show_notification = 1;
                    }
                    else
                    {
                        s.show_notification = 0;
                    }

                }
                else//驾校保存
                {
                    s.driverschool_name = quancheng_textBox.Text;
                    s.contact = lianxi_textBox.Text;
                    s.address = dizhi_textBox.Text;
                    s.introduction = jianjie_textBox.Text;
                    s.testbench_number = int.Parse(kaotai_textBox.Text);
                    s.driverschool_picture = tupian_textBox.Text;
                    if (kaiji_checkBox.IsChecked == true)
                    {
                        s.power_on = 1;
                    }
                    else
                    {
                        s.power_on = 0;
                    }
                    if (guanbi_checkBox.IsChecked == true)
                    {
                        s.close_password = 1;
                    }
                    else
                    {
                        s.close_password = 0;
                    }
                    if (tongzhi_checkBox.IsChecked == true)
                    {
                        s.show_notification = 1;
                    }
                    else
                    {
                        s.show_notification = 0;
                    }

                }
            }

            jiakaoDataSetsettingTableAdapter.Update(jiakaoDataSet.setting);
            jiakaoDataSet.setting.AcceptChanges();


        }

        //功能标题设置保存
        private void gongneng_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;

            string[] gongneng = new string[6];//索引从0开始依次为 章节练习;顺序练习;随机练习;专项练习;模拟考试;错题强化; 0为启用不选中,1为禁用选中
            string[] yuyin = new string[3];//索引从0 开始依次为 提示语音讲解;语音提示对错;答错语音提示 0为不启用,1为启用
            string gongstring = "";
            string yustring = "";
            string kemuer = "";
            string kemusan = "";
            int langdu = 0;
            int fanti = 0;
            int zhengque = 0;
            string chongkao = "0";

            if (zhangjie_checkBox.IsChecked == true)
            {
                gongneng[0] = "1";
            }
            else
            {
                gongneng[0] = "0";
            }
            if (shunxu_checkBox.IsChecked == true)
            {
                gongneng[1] = "1";
            }
            else
            {
                gongneng[1] = "0";
            }
            if (suiji_checkBox.IsChecked == true)
            {
                gongneng[2] = "1";
            }
            else
            {
                gongneng[2] = "0";
            }
            if (zhuanxiang_checkBox.IsChecked == true)
            {
                gongneng[3] = "1";
            }
            else
            {
                gongneng[3] = "0";
            }
            if (moni_checkBox.IsChecked == true)
            {
                gongneng[4] = "1";
            }
            else
            {
                gongneng[4] = "0";
            }
            if (cuoti_checkBox.IsChecked == true)
            {
                gongneng[5] = "1";
            }
            else
            {
                gongneng[5] = "0";
            }

            if (jiangjie_checkBox.IsChecked == true)
            {
                yuyin[0] = "1";
            }
            else
            {
                yuyin[0] = "0";
            }
            if (duicuo_checkBox.IsChecked == true)
            {
                yuyin[1] = "1";
            }
            else
            {
                yuyin[1] = "0";
            }
            if (dacuo_checkBox.IsChecked == true)
            {
                yuyin[2] = "1";
            }
            else
            {
                yuyin[2] = "0";
            }

            for (int i = 0; i <= 5; i++)
            {
                if (i < 5)
                {
                    gongstring += gongneng[i] + ",";
                }
                else
                {
                    gongstring += gongneng[i];
                }
            }

            for (int i = 0; i <= 2; i++)
            {
                if (i < 2)
                {
                    yustring += yuyin[i] + ",";
                }
                else
                {
                    yustring += yuyin[i];
                }
            }

            if (kemu2_checkBox.IsChecked == true)
            {
                kemuer = "1";
            }
            else
            {
                kemuer = "0";
            }
            if (kemu3_checkBox.IsChecked == true)
            {
                kemusan = "1";
            }
            else
            {
                kemusan = "0";
            }

            if (nv_radioButton.IsChecked == true)
            {
                langdu = 0;
            }
            else if (nan_radioButton.IsChecked == true)
            {
                langdu = 1;
            }
            else
            {
                langdu = 2;
            }

            if (shoudong_radioButton.IsChecked == true)
            {
                fanti = 0;
            }
            else if (zidong_radioButton.IsChecked == true)
            {
                fanti = 1;
            }
            else
            {
                fanti = 2;
            }

            if (zhengque_checkBox.IsChecked == true)
            {
                zhengque = 1;
            }
            else
            {
                zhengque = 0;
            }

            if (chongkao_checkBox.IsChecked == true)
            {
                chongkao = "1";
            }
            else
            {
                chongkao = "0";
            }

            foreach (var s in set)
            {
                s.software_title = biaoti_textBox.Text;
                s.functional_module = gongstring;
                s.subject_module = kemuer + "," + kemusan;
                s.phonetic_reading = langdu;
                s.voice_prompt = yustring;
                s.next_question = fanti;
                s.display_answers = zhengque;
                s.display_test = chongkao;
                s.subject_four_button = kemu4_textBox.Text;
            }
            jiakaoDataSetsettingTableAdapter.Update(jiakaoDataSet.setting);
            jiakaoDataSet.setting.AcceptChanges();
        }

        //信息设置保存
        private void xinxibaocun_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;

            string[] xinxi = new string[5];//索引从0开始依次为 注册流程;付款链接;客服QQ;客服旺旺;备注; 0为启用不选中,1为禁用选中
            string xinxistring = "";

            if (liucheng_checkBox.IsChecked == true)
            {
                xinxi[0] = "1";
            }
            else
            {
                xinxi[0] = "0";
            }
            if (lianjie_checkBox.IsChecked == true)
            {
                xinxi[1] = "1";
            }
            else
            {
                xinxi[1] = "0";
            }
            if (QQ_checkBox.IsChecked == true)
            {
                xinxi[2] = "1";
            }
            else
            {
                xinxi[2] = "0";
            }
            if (WW_checkBox.IsChecked == true)
            {
                xinxi[3] = "1";
            }
            else
            {
                xinxi[3] = "0";
            }
            if (beizhu_checkBox.IsChecked == true)
            {
                xinxi[4] = "1";
            }
            else
            {
                xinxi[4] = "0";
            }

            for (int i = 0; i <= 4; i++)
            {
                if (i < 4)
                {
                    xinxistring += xinxi[i] + ",";
                }
                else
                {
                    xinxistring += xinxi[i];
                }
            }

            foreach (var s in set)
            {
                s.registration_display = xinxistring;
                s.registration_process = liucheng_textBox.Text;
                s.payment_link = lianjie_textBox.Text;
                s.customer_service_QQ = QQ_textBox.Text;
                s.customer_service_WW = WW_textBox.Text;
                s.comments = beizhu_textBox.Text;
            }
            jiakaoDataSetsettingTableAdapter.Update(jiakaoDataSet.setting);
            jiakaoDataSet.setting.AcceptChanges();
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
                if (sender == a_textBox)
                {
                    interim_key[0] = e.Key.ToString();
                }
                if (sender == b_textBox)
                {
                    interim_key[1] = e.Key.ToString();
                }
                if (sender == c_textBox)
                {
                    interim_key[2] = e.Key.ToString();
                }
                if (sender == d_textBox)
                {
                    interim_key[3] = e.Key.ToString();
                }
                if (sender == dui_textBox)
                {
                    interim_key[4] = e.Key.ToString();
                }
                if (sender == cuo_textBox)
                {
                    interim_key[5] = e.Key.ToString();
                }
                if (sender == up_textBox)
                {
                    interim_key[6] = e.Key.ToString();
                }
                if (sender == down_textBox)
                {
                    interim_key[7] = e.Key.ToString();
                }
                if (sender == head_textBox)
                {
                    interim_key[8] = e.Key.ToString();
                }
                if (sender == end_textBox)
                {
                    interim_key[9] = e.Key.ToString();
                }
                if (sender == jiaojuan_textBox)
                {
                    interim_key[10] = e.Key.ToString();
                }
                if (sender == queren_textBox)
                {
                    interim_key[11] = e.Key.ToString();
                }
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

        //禁用所有快捷键text
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
        //启用所有快捷键text
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

        //string[] interim_key = new string[12];//临时快捷键数组 索引从0开始依次为 A;B;C;D;对;错;上一题;下一题;第一题;最后题;交卷;确认交卷
        //方案一单选框
        private void radioButton20_Checked(object sender, RoutedEventArgs e)
        {
            readonly_false();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\方案一.png", UriKind.Relative));
            interim_key = new string[12] { "NumPad1", "NumPad2", "NumPad3", "NumPad5", "NumPad4", "NumPad6", "Subtract", "Add", "None", "None", "Divide", "None" };

        }
        //方案二单选框
        private void radioButton21_Checked(object sender, RoutedEventArgs e)
        {
            readonly_false();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\方案二.png", UriKind.Relative));
            interim_key = new string[12] { "NumPad7", "NumPad8", "NumPad9", "NumPad5", "NumPad4", "NumPad6", "Subtract", "Add", "None", "None", "NumPad0", "None" };
        }
        //方案三单选框
        private void radioButton22_Checked(object sender, RoutedEventArgs e)
        {
            readonly_false();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\方案三.png", UriKind.Relative));
            interim_key = new string[12] { "NumPad7", "NumPad8", "NumPad9", "Add", "NumPad4", "NumPad6", "NumPad1", "NumPad3", "None", "None", "Subtract", "Return" };
        }
        //方案四单选框
        private void radioButton23_Checked(object sender, RoutedEventArgs e)
        {
            readonly_false();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\方案四.png", UriKind.Relative));
            interim_key = new string[12] { "NumPad7", "NumPad8", "NumPad9", "NumPad4", "NumPad1", "NumPad3", "NumPad5", "NumPad6", "None", "None", "Divide", "Return" };
        }
        //方案五单选框
        private void radioButton24_Checked(object sender, RoutedEventArgs e)
        {
            readonly_false();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\方案五.png", UriKind.Relative));
            interim_key = new string[12] { "NumPad1", "NumPad2", "NumPad3", "NumPad0", "NumPad1", "NumPad2", "Subtract", "Add", "None", "None", "Return", "None" };
        }
        //自定义方案单选框
        private void radioButton25_Checked(object sender, RoutedEventArgs e)
        {
            readonly_true();
            fangan_image.Source = new BitmapImage(new Uri("\\Images\\自定义.png", UriKind.Relative));
            interim_key = new string[12];

        }

        //键盘设置保存
        private void jianpanshezhi_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var key = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;

            if (radioButton25.IsChecked == true)//自定义
            {
                #region 自定义Key不设置为空
                if (a_textBox.Text == "")
                {
                    interim_key[0] = "None";
                }
                if (b_textBox.Text == "")
                {
                    interim_key[1] = "None";
                }
                if (c_textBox.Text == "")
                {
                    interim_key[2] = "None";
                }
                if (d_textBox.Text == "")
                {
                    interim_key[3] = "None";
                }
                if (dui_textBox.Text == "")
                {
                    interim_key[4] = "None";
                }
                if (cuo_textBox.Text == "")
                {
                    interim_key[5] = "None";
                }
                if (up_textBox.Text == "")
                {
                    interim_key[6] = "None";
                }
                if (down_textBox.Text == "")
                {
                    interim_key[7] = "None";
                }
                if (head_textBox.Text == "")
                {
                    interim_key[8] = "None";
                }
                if (end_textBox.Text == "")
                {
                    interim_key[9] = "None";
                }
                if (jiaojuan_textBox.Text == "")
                {
                    interim_key[10] = "None";
                }
                if (queren_textBox.Text == "")
                {
                    interim_key[11] = "None";
                }
                #endregion

                foreach (var k in key)
                {
                    string keystring = "";
                    for (int i = 0; i <= 11; i++)
                    {
                        if (i < 11)
                        {
                            keystring += interim_key[i] + ",";
                        }
                        else
                        {
                            keystring += interim_key[i];
                        }
                    }
                    k.shortcut_key = keystring;
                }

                PublicClass.key = (string[])interim_key.Clone();

            }
            else
            {
                foreach (var k in key)
                {
                    string keystring = "";
                    for (int i = 0; i <= 11; i++)
                    {
                        if (i < 11)
                        {
                            keystring += interim_key[i] + ",";
                        }
                        else
                        {
                            keystring += interim_key[i];
                        }
                    }
                    k.shortcut_key = keystring;
                }
                PublicClass.key = (string[])interim_key.Clone();
            }
            jiakaoDataSetsettingTableAdapter.Update(jiakaoDataSet.setting);
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);
            jiakaoDataSet.setting.AcceptChanges();
        }

















    }

    //ComboBox数据绑定
    public class Province//省份
    {
        public string region { set; get; }
        public int reID { set; get; }
    }

    public class Driving//驾照类型
    {
        public string type { set; get; }
        public int tyID { set; get; }
    }

    public class ProvinceArr : ObservableCollection<Province>//省份
    {
        public ProvinceArr()
        {
            this.Add(new Province { reID = 0, region = "北京" });
            this.Add(new Province { reID = 1, region = "天津" });
            this.Add(new Province { reID = 2, region = "上海" });
            this.Add(new Province { reID = 3, region = "重庆" });
            this.Add(new Province { reID = 4, region = "河北" });
            this.Add(new Province { reID = 5, region = "山西" });
            this.Add(new Province { reID = 6, region = "辽宁" });
            this.Add(new Province { reID = 7, region = "吉林" });
            this.Add(new Province { reID = 8, region = "黑龙江" });
            this.Add(new Province { reID = 9, region = "江苏" });
            this.Add(new Province { reID = 10, region = "浙江" });
            this.Add(new Province { reID = 11, region = "安徽" });
            this.Add(new Province { reID = 12, region = "福建" });
            this.Add(new Province { reID = 13, region = "江西" });
            this.Add(new Province { reID = 14, region = "山东" });
            this.Add(new Province { reID = 15, region = "河南" });
            this.Add(new Province { reID = 16, region = "湖北" });
            this.Add(new Province { reID = 17, region = "湖南" });
            this.Add(new Province { reID = 18, region = "广东" });
            this.Add(new Province { reID = 19, region = "海南" });
            this.Add(new Province { reID = 20, region = "云南" });
            this.Add(new Province { reID = 21, region = "四川" });
            this.Add(new Province { reID = 22, region = "贵州" });
            this.Add(new Province { reID = 23, region = "陕西" });
            this.Add(new Province { reID = 24, region = "甘肃" });
            this.Add(new Province { reID = 25, region = "青海" });
            this.Add(new Province { reID = 26, region = "台湾" });
            this.Add(new Province { reID = 27, region = "内蒙古" });
            this.Add(new Province { reID = 28, region = "广西" });
            this.Add(new Province { reID = 29, region = "宁夏" });
            this.Add(new Province { reID = 30, region = "新疆" });
            this.Add(new Province { reID = 31, region = "西藏" });
        }
    }

    public class DrivingArr : ObservableCollection<Driving>//驾照类型
    {
        public DrivingArr()
        {
            this.Add(new Driving { tyID = 0, type = "C1C2C3C4" });
            this.Add(new Driving { tyID = 1, type = "A1A3B1" });
            this.Add(new Driving { tyID = 2, type = "A2B2" });
            this.Add(new Driving { tyID = 3, type = "DEF" });
            this.Add(new Driving { tyID = 4, type = "恢复驾照" });
        }
    }

}

