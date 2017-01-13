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
using System.Windows.Media.Animation;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using Util.Controls;
using System.Management;
using System.Diagnostics;
using System.Windows.Interop;

namespace DrivingTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {


        //public string SelectedValue { get; private set; }

        //public event RoutedEventHandler CheckChanged;
        int attch_down_count = 0;//待下载附件数
        int cur_cown_count = 0;//正在下载附件数
        List<string> img_down_list = new List<string>();
        List<string> voice_down_list = new List<string>();
        JArray question_json;//题目json
        JArray answer_json;//答案json
        JArray chapter_json;//章节json
        JArray subject_json;//科目json
        int cur_question_step = 0;

        int synccount = 0;
        int now_synccount = 0;

        List<PublicClass.questions> local_question_data = new List<PublicClass.questions>();
        List<PublicClass.answers> local_answer_data = new List<PublicClass.answers>();
        List<PublicClass.chapters> local_chapter_data = new List<PublicClass.chapters>();
        List<PublicClass.subjects> local_subject_data = new List<PublicClass.subjects>();

        bool question_update_compaleted = false;
        bool answer_update_compaleted = false;
        bool chapter_update_compaleted = false;
        bool subject_update_compaleted = false;

        string ip = ""; //ip

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += F;
            //IsConn();
        }

        //关闭事件
        private void F(object o, System.ComponentModel.CancelEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);


            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
            PublicClass.shezhi = "退出";
            foreach (var s in set)
            {
                //有关闭密码
                if (s.close_password != 0)
                {
                    e.Cancel = true;
                    QuitPassword qu = new QuitPassword();
                    C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
                    c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - qu.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - qu.Height / 2, 0, 0);
                    c1ma.Content = qu;
                    c1ma.Show();
                    c1ma.Name = "管理员密码";
                    c1ma.Header = "管理员密码";
                }
                //无关闭密码
                else
                {
                    MessageBoxResult result = MessageBox.Show("确定退出吗？", "询问", MessageBoxButton.OKCancel);

                    //关闭窗口
                    if (result == MessageBoxResult.OK)
                        e.Cancel = false;

                    //不关闭窗口
                    if (result == MessageBoxResult.Cancel)
                        e.Cancel = true;
                }

            }
        }


        public static T FindChild<T>(DependencyObject parent, string childName)//查找控件
where T : DependencyObject
        {
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // 如果子控件不是需查找的控件类型 
                T childType = child as T;
                if (childType == null)
                {
                    // 在下一级控件中递归查找 
                    foundChild = FindChild<T>(child, childName);
                    // 找到控件就可以中断递归操作  
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // 如果控件名称符合参数条件 
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // 查找到了控件 
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }


        private void get_local_chapters(object data)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));

            // 将数据加载到表 chapter 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
            jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);

            foreach (var mychapter in jiakaoDataSet.chapter)
            {
                PublicClass.chapters chapter = new PublicClass.chapters();
                chapter.chapter_id = mychapter.chapter_id;
                chapter.chapter = mychapter.chapter;
                chapter.updated_at = mychapter.updated_at;
                local_chapter_data.Add(chapter);
            }
        }

        private void get_local_subject(object data)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));

            // 将数据加载到表 subject 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);

            foreach (var mysubject in jiakaoDataSet.subject)
            {
                PublicClass.subjects subject = new PublicClass.subjects();
                subject.subject_id = mysubject.subject_id;
                subject.subject = mysubject.subject;
                subject.updated_at = mysubject.updated_at;
                local_subject_data.Add(subject);
            }
        }

        private void get_local_questions(object data)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();

                jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

                foreach (var myques in jiakaoDataSet.question)
                {
                    PublicClass.questions question = new PublicClass.questions();
                    question.question_id = myques.question_id;
                    question.chapter_id = myques.chapter_id;
                    question.subject_id = myques.subject_id;
                    question.question_name = myques.question_name;
                    question.question_image = myques.question_image;
                    question.voice = myques.voice;
                    question.driverlicense_type = myques.driverlicense_type;
                    question.question_type = myques.question_type;
                    question.update_at = myques.update_at;
                    question.is_judge = myques.is_judge;
                    local_question_data.Add(question);

                }
           
        }

        private void get_local_answers(object data)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();

                jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
                foreach (var myanswer in jiakaoDataSet.answer)
                {
                    PublicClass.answers answer = new PublicClass.answers();
                    answer.answer_id = myanswer.answer_id;
                    answer.question_id = myanswer.question_id;
                    answer.answer = myanswer.answer;
                    answer.is_right = myanswer.is_right;
                    answer.update_at = myanswer.update_at;
                    local_answer_data.Add(answer);
                }
           
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Process[] pro = Process.GetProcesses();
            int n = pro.Where(p => p.ProcessName.Equals("DrivingTest")).Count();
            if (n > 1)
            {
                Environment.Exit(0);
                //int windows = int.Parse(new WindowInteropHelper(this).Handle.ToString());
                return;
            }


            PublicClass.timer.Interval = new TimeSpan(5000);// 计时器触发间隔 5秒  
            PublicClass.timer.Tick += new EventHandler(timer_Tick);
            PublicClass.timer.Start();

            //PublicClass.http = @"http://192.168.1.98:3000";
            //PublicClass.http = @"http://47.89.28.92";
            PublicClass.http = @"http://jiakao.cloudtimesoft.com";
            //ThreadPool.QueueUserWorkItem(get_local_questions, "");
            //ThreadPool.QueueUserWorkItem(get_local_answers, "");
            get_local_questions("");
            get_local_answers("");
            //get_local_chapters("");
            //get_local_subject("");

            //maincanvas.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2, SystemParameters.PrimaryScreenHeight / 2, 0, 0);

            try
            {
                DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
                // 将数据加载到表 updatecheck 中。可以根据需要修改此代码。
                DrivingTest.jiakaoDataSetTableAdapters.updatecheckTableAdapter jiakaoDataSetupdatecheckTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.updatecheckTableAdapter();
                jiakaoDataSetupdatecheckTableAdapter.Fill(jiakaoDataSet.updatecheck);
                // 将数据加载到表 setting 中。可以根据需要修改此代码。
                DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
                jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);
                DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter jiakaoDataSetuserTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter();
                jiakaoDataSetuserTableAdapter.Fill(jiakaoDataSet.user);



                string key = "NumPad1,NumPad2,NumPad3,NumPad5,NumPad4,NumPad6,Subtract,Add,None,None,Divide,None";//初始默认快捷键为方案一
                var setting = from c in jiakaoDataSet.setting select c;
                if (setting.Count() == 0)//初始化设置表
                {
                    //int? a = null;
                    //int b = a.Value;
                    jiakaoDataSet.setting.AddsettingRow(0, 0, 0, "", 0, 0, "", "", "", "", 0, "", "", 0, 0, "", 0, 0, 0, "", "", "", "", "1", "", "", "", "", "", "", "", "", "", "", "", 0, "", "", 0, key, 0, 2, "0,0,0", 0, 0);
                    jiakaoDataSetsettingTableAdapter.Update(jiakaoDataSet.setting);
                    jiakaoDataSet.setting.AcceptChanges();
                    foreach (var se in setting)
                    {
                        if (se.not_show.ToString() == "0")
                        {
                            Notice no = new Notice();
                            no.Show();
                        }
                    }
                }
                else
                {
                    foreach (var se in setting)
                    {
                        if (se.subject_module != "")
                        {
                            if (se.subject_module.Substring(0, 1) == "1")
                            {
                                subject2.Visibility = System.Windows.Visibility.Hidden;
                            }
                            else
                            {
                                subject2.Visibility = System.Windows.Visibility.Visible;
                            }
                            if (se.subject_module.Substring(2, 1) == "1")
                            {
                                subject3.Visibility = System.Windows.Visibility.Hidden;
                            }
                            else
                            {
                                subject3.Visibility = System.Windows.Visibility.Visible;
                            }
                        }
                        else
                        {
                            subject2.Visibility = System.Windows.Visibility.Visible;
                            subject3.Visibility = System.Windows.Visibility.Visible;
                        }

                        if (se.not_show.ToString() == "0")
                        {
                            Notice no = new Notice();
                            no.Show();
                        }
                    }
                }




                Thread newthread = new Thread(new ThreadStart(() =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ////当前进程
                        //Process current = Process.GetCurrentProcess();
                        ////进程下线程
                        //ProcessThreadCollection allThreads = current.Threads;
                        ////当前线程
                        //Console.WriteLine("ID:{0}", Thread.CurrentThread.ManagedThreadId);

                        xianshi.Text = "正在检查更新";
                        System.Windows.Forms.Application.DoEvents();
                        if (checknetwork())
                        {
                            var chkupd = from c in jiakaoDataSet.updatecheck select c;
                            var getchkupdstr = getupdatecheck();
                            var localchkupdstr = "";
                            foreach (var mychkupd in chkupd)
                            {
                                localchkupdstr = mychkupd.updatecheck;
                            }
                            if (getchkupdstr == localchkupdstr)
                            {

                                xianshi.Text = "已是最新版本";
                                ThreadPool.QueueUserWorkItem(push_to_public, "");
                            }
                            else
                            {
                                user_textBox.IsEnabled = false;
                                password_textBox.IsEnabled = false;
                                updatequestion();//开始更新题库
                                //updatedownload();
                                //version(getchkupdstr);
                                //xianshi.Text = "下载完毕,更新已完成";
                            }

                        }
                        else
                        {
                            xianshi.Text = "无法连接网络,脱机模式下请到注册页面联系客服购买注册码";
                            string cpuid = "";//历史机器码
                            int num = 0;//机器码登陆次数
                            int time = 0;//机器码截止日期
                            DateTime newtime = DateTime.Now;//获取本机时间
                            var user = from c in jiakaoDataSet.user where c.user_id == -1 select c;//检索是否有脱机注册记录
                            foreach (var u in user)
                            {
                                cpuid = u.login;
                                num = int.Parse(u.login_number);
                                time = int.Parse(u.login_time);

                                if (user.Count() == 1 && cpuid == Get_Cpu_Id())//对比机器码识别是否更换机器
                                {
                                    int stime = int.Parse(newtime.ToString("yyyyMMdd"));
                                    if (time >= stime)//对比有效期
                                    {
                                        if (num != 0)
                                        {
                                            num--;
                                            u.login_number = num.ToString();
                                            login_control_ShowHide();//登录后控件的显示或隐藏
                                        }
                                        else
                                        {
                                            MessageBox.Show("激活码可使用次数为0!请重新购买激活码!");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("激活码已过期!请重新购买激活码!");
                                    }
                                }
                                else if (user.Count() == 1 && cpuid != Get_Cpu_Id())
                                {
                                    MessageBox.Show("激活码仅供一台机器使用!更换机器请重新购买激活码!");
                                }
                            }
                            jiakaoDataSetuserTableAdapter.Update(jiakaoDataSet.user);
                            jiakaoDataSet.user.AcceptChanges();
                            PublicClass.wuwangluo = true;//无网络
                            ThreadPool.QueueUserWorkItem(push_to_public, "");
                        }

                    }));



                }));
                newthread.SetApartmentState(ApartmentState.MTA);
                newthread.IsBackground = true;
                //newthread.Priority = ThreadPriority.Lowest;
                newthread.Start();


                qianlunqipao.MouseEnter += new MouseEventHandler(qianlun_MouseEnter);
                qianlunqipao.MouseLeave += new MouseEventHandler(qianlun_MouseLeave);
                houlunqipao.MouseEnter += new MouseEventHandler(houlun_MouseEnter);
                houlunqipao.MouseLeave += new MouseEventHandler(houlun_MouseLeave);

                //显示账号剩余次数、帐号毕业、帐号异常、在其它机器登陆
                string zhanghao = "";
                if (user_textBox.Text != "")
                {
                    xianshi.Text = zhanghao;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }


        private void push_to_public(object data)//将题库写入公共内存
        {
            PublicClass.answer_data.Clear();
            PublicClass.question_data.Clear();
            //PublicClass.question_data = local_question_data.Copy();
            foreach (var myquestion in local_question_data)
            {
                PublicClass.questions question = new PublicClass.questions();
                question.question_id = myquestion.question_id;
                question.chapter_id = myquestion.chapter_id;
                question.subject_id = myquestion.subject_id;
                question.question_name = myquestion.question_name;
                question.question_image = myquestion.question_image;
                question.voice = myquestion.voice;
                question.driverlicense_type = myquestion.driverlicense_type;
                question.question_type = myquestion.question_type;
                question.update_at = myquestion.update_at;
                question.is_judge = myquestion.is_judge;
                PublicClass.question_data.Add(question);
            }
            //PublicClass.answer_data = local_answer_data.Copy();
            foreach (var myanswer in local_answer_data)
            {
                PublicClass.answers answer = new PublicClass.answers();
                answer.answer_id = myanswer.answer_id;
                answer.question_id = myanswer.question_id;
                answer.answer = myanswer.answer;
                answer.is_right = myanswer.is_right;
                answer.update_at = myanswer.update_at;
                PublicClass.answer_data.Add(answer);
            }
        }

        //读取CPU机器码
        private static string Get_Cpu_Id()
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
            return cpuInfo.ToString();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            // 距离上一次系统输入时间大于5秒  
            if (GetLastInputTime() >= 3600 * 1000)
            {
                Environment.Exit(0);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        static long GetLastInputTime()
        {
            LASTINPUTINFO vLastInputInfo = new LASTINPUTINFO();
            vLastInputInfo.cbSize = Marshal.SizeOf(vLastInputInfo);
            if (!GetLastInputInfo(ref vLastInputInfo)) return 0;
            return Environment.TickCount - (long)vLastInputInfo.dwTime;
        }

        public void updateUI()
        {
            Thread t = new Thread(new ThreadStart(ThreadMethod));
            t.Start();
        }
        void ThreadMethod()
        {
            this.Dispatcher.Invoke(new MyDelegate(DelegateMethod), "通过委托跨线程访问");
        }
        delegate void MyDelegate(string param);
        void DelegateMethod(string param)
        {
            //设置
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var setting = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
            foreach (var se in setting)
            {
                if (se.subject_module != "")
                {
                    if (se.subject_module.Substring(0, 1) == "1")
                    {
                        subject2.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        subject2.Visibility = System.Windows.Visibility.Visible;
                    }
                    if (se.subject_module.Substring(2, 1) == "1")
                    {
                        subject3.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        subject3.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    subject2.Visibility = System.Windows.Visibility.Visible;
                    subject3.Visibility = System.Windows.Visibility.Visible;
                }
            }

            //注册
            if (PublicClass.tuojizhuce == true)
            {
                login_control_ShowHide();//登录后控件的显示或隐藏
            }
        }

        //登录界面判断本机是否联网
        private bool checknetwork()
        {
            try
            {
                System.Net.NetworkInformation.Ping ping;
                System.Net.NetworkInformation.PingReply res;
                ping = new System.Net.NetworkInformation.Ping();
                bool is_ping = false;
                for (int i = 0; i < 2; i++)
                {

                    res = ping.Send("www.baidu.com");
                    if (res.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        is_ping = true;
                        i = 2;
                    }

                }
                if (is_ping)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        #region 数据接收



        //接收服务器的Json数据
        public static string GetHTTPInfo()
        {
            string str = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/getquestion");
                request.Method = "GET";
                request.Timeout = 5000;
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                str = reader.ReadToEnd();


                JArray jsonObj = JArray.Parse(str);

                string cc = "";

                foreach (JObject jobject in jsonObj)
                {
                    cc += jobject["id"];
                }


            }
            catch
            {
                str = "";
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return str;
        }


        #endregion




        //窗体拖动事件
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        #region 更新版本号
        //接收版本更新号的json文件
        private string getupdatecheck()
        {
            string str = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            string cc = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/updatecheck");
                request.Method = "GET";
                request.Timeout = 30000;
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                str = reader.ReadToEnd();


                JArray jsonObj = JArray.Parse(str);



                foreach (JObject jobject in jsonObj)
                {
                    cc += jobject["updatecheck"];
                }


            }
            catch (Exception ex)
            {
                str = "";
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return cc;

        }

        //将版本号写入数据库
        private string version(string myupdate)
        {
            try
            {
                DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
                // 将数据加载到表 updatecheck 中。可以根据需要修改此代码。
                DrivingTest.jiakaoDataSetTableAdapters.updatecheckTableAdapter jiakaoDataSetupdatecheckTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.updatecheckTableAdapter();

                int updcount = (from c in jiakaoDataSet.updatecheck select c).Count();
                if (updcount > 0)
                {
                    var delupd = from c in jiakaoDataSet.updatecheck select c;
                    foreach (var del in delupd)
                    {
                        del.Delete();
                    }

                }

                jiakaoDataSet.updatecheck.AddupdatecheckRow(myupdate);

                jiakaoDataSetupdatecheckTableAdapter.Update(jiakaoDataSet.updatecheck);
                jiakaoDataSet.updatecheck.AcceptChanges();
                //jiakaoDataSetupdatecheckTableAdapter.Fill(jiakaoDataSet.updatecheck);
                //jiakaoDataSet.AcceptChanges();
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }
        #endregion

        private void update_local_chapter(object data)
        {
            foreach (var remotechapter in chapter_json)
            {
                PublicClass.chapters chapter = new PublicClass.chapters();
                try
                {
                    chapter.chapter_id = int.Parse(remotechapter["id"].ToString());
                }
                catch { }

                try
                {
                    chapter.chapter = remotechapter["chapter"].ToString();
                }
                catch { }

                try
                {
                    chapter.updated_at = remotechapter["updated_at"].ToString();
                }
                catch { }
                local_chapter_data.Add(chapter);
            }
            //chapter_update_compaleted = true;
            //data_complete_validate();
            //set_chapter("");
        }

        private void update_subject_data(object data)
        {
            foreach (var remotesubject in subject_json)
            {
                PublicClass.subjects subject = new PublicClass.subjects();
                try
                {
                    subject.subject_id = int.Parse(remotesubject["id"].ToString());
                }
                catch { }
                try
                {
                    subject.subject = remotesubject["subject"].ToString();
                }
                catch { }
                try
                {
                    subject.updated_at = remotesubject["updated_at"].ToString();
                }
                catch { }
                local_subject_data.Add(subject);
            }
            //subject_update_compaleted = true;
            //data_complete_validate();
            //set_subject("");
        }

        private void update_local_question(object data)//更新内存题库
        {
            foreach (var remotequestion in question_json)
            {
                var localquestion = from c in local_question_data where int.Parse(remotequestion["id"].ToString()) == c.question_id select c;
                //var localquestion = from c in jiakaoDataSet.question where int.Parse(remotequestion["id"].ToString()) == c.question_id select c;
                //var t = jiakaoDataSet.question.GetEnumerator();
                if (localquestion.Count() == 0)
                {
                    int id1 = int.Parse(remotequestion["id"].ToString());

                    int id2;
                    try { id2 = int.Parse(remotequestion["chapter_id"].ToString()); }
                    catch { id2 = 0; }

                    int id3;
                    try { id3 = int.Parse(remotequestion["subject_id"].ToString()); }
                    catch { id3 = 0; }

                    string name;
                    try { name = remotequestion["questionname"].ToString(); }
                    catch { name = ""; }

                    string img;
                    try { img = remotequestion["questionimage_file_name"].ToString(); }
                    catch { img = ""; }

                    string voi;
                    try { voi = remotequestion["voice_file_name"].ToString(); }
                    catch { voi = ""; }

                    string dri;
                    try { dri = remotequestion["driverlicensetype"].ToString(); }
                    catch { dri = ""; }

                    string que;
                    try { que = remotequestion["questiontype"].ToString(); }
                    catch { que = ""; }

                    string upd;
                    try { upd = remotequestion["updated_at"].ToString(); }
                    catch { upd = ""; }

                    int isj;
                    try { isj = int.Parse(remotequestion["isjudge"].ToString()); }
                    catch { isj = 0; }
                    if (img != "")
                    {
                        
                        attch_down_count++;
                        updatedownload(img, voi);
                        //img_down_list.Add(img);
                        //voice_down_list.Add(voi);

                    }
                    //jiakaoDataSet.question.AddquestionRow(id1, id2, id3, name, img, voi, dri, que, upd, isj);
                    //jiakaoDataSet.question.AddquestionRow(id1, id2, id3, name, img, voi, dri, que, upd, isj);
                    PublicClass.questions question = new PublicClass.questions();
                    question.question_id = id1;
                    question.chapter_id = id2;
                    question.subject_id = id3;
                    question.question_name = name;
                    question.question_image = img;
                    question.voice = voi;
                    question.driverlicense_type = dri;
                    question.question_type = que;
                    question.update_at = upd;
                    question.is_judge = isj;
                    local_question_data.Add(question);
                }
                else
                {
                    //foreach (var lq in localquestion)
                    foreach (var lq in local_question_data)
                    {
                        if (remotequestion["updated_at"].ToString() != lq.update_at)
                        {
                            int int1 = int.Parse(remotequestion["id"].ToString());

                            int id2;
                            try { id2 = int.Parse(remotequestion["chapter_id"].ToString()); }
                            catch { id2 = 0; }

                            int id3;
                            try { id3 = int.Parse(remotequestion["subject_id"].ToString()); }
                            catch { id3 = 0; }

                            string name;
                            try { name = remotequestion["questionname"].ToString(); }
                            catch { name = ""; }

                            string img;
                            try { img = remotequestion["questionimage_file_name"].ToString(); }
                            catch { img = ""; }

                            string voi;
                            try { voi = remotequestion["voice_file_name"].ToString(); }
                            catch { voi = ""; }

                            string dri;
                            try { dri = remotequestion["driverlicensetype"].ToString(); }
                            catch { dri = ""; }

                            string que;
                            try { que = remotequestion["questiontype"].ToString(); }
                            catch { que = ""; }

                            string upd;
                            try { upd = remotequestion["updated_at"].ToString(); }
                            catch { upd = ""; }

                            int isj;
                            try { isj = int.Parse(remotequestion["isjudge"].ToString()); }
                            catch { isj = 0; }

                            //lq.question_id = int1;
                            //lq.chapter_id = id2;
                            //lq.subject_id = id3;
                            //lq.question_name = name;
                            //lq.question_image = img;
                            //lq.voice = voi;
                            //lq.driverlicense_type = dri;
                            //lq.question_type = que;
                            //lq.update_at = upd;
                            //lq.is_judge = isj;
                            //attch_down_count++;
                            //updatedownload(img, voi);
                            //img_down_list.Add(img);
                            //voice_down_list.Add(voi);



                            //PublicClass.questions question = new PublicClass.questions();
                            //var question=from c in local_question_data 
                            lq.question_id = int1;
                            lq.chapter_id = id2;
                            lq.subject_id = id3;
                            lq.question_name = name;
                            lq.question_image = img;
                            lq.voice = voi;
                            lq.driverlicense_type = dri;
                            lq.question_type = que;
                            lq.update_at = upd;
                            lq.is_judge = isj;

                            attch_down_count++;
                            updatedownload(img, voi);
                            //img_down_list.Add(img);
                            //voice_down_list.Add(voi);


                        }
                    }
                }
                now_synccount++;
                cur_question_step++;
                   Dispatcher.Invoke(new Action(() =>
                    {
                xianshi.Text = "同步资源..." + cur_question_step;

                progress.Value = now_synccount / synccount * 100;
                    }));
                System.Windows.Forms.Application.DoEvents();
            }
            question_update_compaleted = true;
            data_complete_validate();
        }

        private void update_local_answer(object data)//更新内存答案
        {
            foreach (var remoteanswer in answer_json)
            {
                //var localanswer = from c in jiakaoDataSet.answer where remoteanswer["id"].ToString() == c.answer_id.ToString() select c;
                var localanswer = from c in local_answer_data where remoteanswer["id"].ToString() == c.answer_id.ToString() select c;
                if (localanswer.Count() == 0)
                {
                    int id1 = int.Parse(remoteanswer["id"].ToString());

                    int id2;
                    try { id2 = int.Parse(remoteanswer["question_id"].ToString()); }
                    catch { id2 = 0; }

                    string ans;
                    try { ans = remoteanswer["asnwer"].ToString(); }
                    catch { ans = ""; }

                    string isr;
                    try { isr = remoteanswer["isright"].ToString(); }
                    catch { isr = "0"; }

                    string upd;
                    try { upd = remoteanswer["updated_at"].ToString(); }
                    catch { upd = ""; }

                    //jiakaoDataSet.answer.AddanswerRow(id1, id2, ans, isr, upd);
                    //jiakaoDataSet.answer.AddanswerRow(id1, id2, ans, isr, upd);
                    PublicClass.answers answer = new PublicClass.answers();
                    answer.answer_id = id1;
                    answer.question_id = id2;
                    answer.answer = ans;
                    answer.is_right = isr;
                    answer.update_at = upd;
                    local_answer_data.Add(answer);
                }
                else
                {
                    foreach (var la in localanswer)
                    //foreach (var la in local_answer_data)
                    {
                        if (remoteanswer["updated_at"].ToString() != la.update_at)
                        {
                            int id1 = int.Parse(remoteanswer["id"].ToString());

                            int id2;
                            try { id2 = int.Parse(remoteanswer["question_id"].ToString()); }
                            catch { id2 = 0; }

                            string ans;
                            try { ans = remoteanswer["asnwer"].ToString(); }
                            catch { ans = ""; }

                            string isr;
                            try { isr = remoteanswer["isright"].ToString(); }
                            catch { isr = "0"; }

                            string upd;
                            try { upd = remoteanswer["updated_at"].ToString(); }
                            catch { upd = ""; }

                            //la.answer_id = id1;
                            //la.question_id = id2;
                            //la.answer = ans;
                            //la.is_right = isr;
                            //la.update_at = upd;


                            la.answer_id = id1;
                            la.question_id = id2;
                            la.answer = ans;
                            la.is_right = isr;
                            la.update_at = upd;

                        }
                    }
                }
                now_synccount++;
                //xianshi.Text = "更新中..." + (now_synccount / synccount * 100).ToString() + "%";
                cur_question_step++;
                                   Dispatcher.Invoke(new Action(() =>
                    {
                xianshi.Text = "同步资源..." + cur_question_step;

                progress.Value = now_synccount / synccount * 100;
                    }));
                System.Windows.Forms.Application.DoEvents();
            }
            answer_update_compaleted = true;
            data_complete_validate();
        }


        //更新题库
        private void updatequestion()
        {
            try
            {
                DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));

                // 将数据加载到表 question 中。可以根据需要修改此代码。
                DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
                jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

                // 将数据加载到表 answer 中。可以根据需要修改此代码。
                DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
                jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

                // 将数据加载到表 chapter 中。可以根据需要修改此代码。
                DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
                jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);

                // 将数据加载到表 subject 中。可以根据需要修改此代码。
                DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
                jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);

                #region 从URL获取json

                string questionstr = null;
                string answerstr = null;
                string chapterstr = null;
                string subjectstr = null;


                HttpWebResponse response = null;
                StreamReader reader = null;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/getquestion");//题目 url
                request.Method = "GET";
                request.Timeout = 30000;
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                questionstr = reader.ReadToEnd();


                request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/getanswer");//答案 url
                request.Method = "GET";
                request.Timeout = 30000;
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                answerstr = reader.ReadToEnd();

                request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/chapter");//章节 url
                request.Method = "GET";
                request.Timeout = 30000;
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                chapterstr = reader.ReadToEnd();

                request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/subject");//科目 url
                request.Method = "GET";
                request.Timeout = 30000;
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                subjectstr = reader.ReadToEnd();

                question_json = JArray.Parse(questionstr);//题目json
                answer_json = JArray.Parse(answerstr);//答案json
                chapter_json = JArray.Parse(chapterstr);//章节json
                subject_json = JArray.Parse(subjectstr);//科目json

                #endregion
                 synccount = question_json.Count + answer_json.Count + chapter_json.Count + subject_json.Count;
                 now_synccount = 0;


                #region 题目表和答案表写入和更新

                //写入和更新题目表
                ThreadPool.QueueUserWorkItem(update_local_question, question_json);
                //xianshi.Text = "写入题...";
                //System.Windows.Forms.Application.DoEvents();
                //jiakaoDataSetquestionTableAdapter.Update(jiakaoDataSet.question);
                //jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
                //jiakaoDataSet.question.AcceptChanges();
                //jiakaoDataSetquestionTableAdapter.Update(jiakaoDataSet.question);
                //jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
                //jiakaoDataSet.question.AcceptChanges();

                //ThreadPool.QueueUserWorkItem(set_question, "");
                //set_question("");


                //写入和更新答案表
                ThreadPool.QueueUserWorkItem(update_local_answer, answer_json);
                //xianshi.Text = "写入答案...";
                //System.Windows.Forms.Application.DoEvents();
                //jiakaoDataSetanswerTableAdapter.Update(jiakaoDataSet.answer);
                //jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
                //jiakaoDataSet.answer.AcceptChanges();
                //ThreadPool.QueueUserWorkItem(get_avswer, "");
                //jiakaoDataSetanswerTableAdapter.Update(jiakaoDataSet.answer);
                //jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
                //jiakaoDataSet.answer.AcceptChanges();
                //ThreadPool.QueueUserWorkItem(set_answer, "");
                //set_answer("");

                #endregion
                #region 写入和更新章节科目表
                //写入和更新章节表
                //foreach (var remotechapter in chapter_json)
                //{
                //    var localchapter = from c in jiakaoDataSet.chapter where remotechapter["id"].ToString() == c.chapter_id.ToString() select c;

                //    if (localchapter.Count() == 0)
                //    {
                //        int id = int.Parse(remotechapter["id"].ToString());

                //        string cha;
                //        try { cha = remotechapter["chapter"].ToString(); }
                //        catch { cha = ""; }

                //        string upd;
                //        try { upd = remotechapter["updated_at"].ToString(); }
                //        catch { upd = ""; }
                //        jiakaoDataSet.chapter.AddchapterRow(id, cha, upd);
                //    }
                //    else
                //    {
                //        foreach (var lc in localchapter)
                //        {
                //            if (remotechapter["updated_at"].ToString() != lc.updated_at)
                //            {
                //                int id = int.Parse(remotechapter["id"].ToString());

                //                string cha;
                //                try { cha = remotechapter["chapter"].ToString(); }
                //                catch { cha = ""; }

                //                string upd;
                //                try { upd = remotechapter["updated_at"].ToString(); }
                //                catch { upd = ""; }

                //                lc.chapter_id = id;
                //                lc.chapter = cha;
                //                lc.updated_at = upd;
                //            }
                //        }
                //    }
                //    now_synccount++;
                //    //xianshi.Text = "更新中..." + (now_synccount / synccount * 100).ToString() + "%";
                //    cur_question_step++;

                //    xianshi.Text = "同步章节..." + cur_question_step;

                //    progress.Value = now_synccount / synccount * 100;
                //    System.Windows.Forms.Application.DoEvents();
                //}
                ThreadPool.QueueUserWorkItem(update_local_chapter, chapter_json);
                //xianshi.Text = "写入章节...";
                //System.Windows.Forms.Application.DoEvents();
                //jiakaoDataSetchapterTableAdapter.Update(jiakaoDataSet.chapter);
                //jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);
                //jiakaoDataSet.chapter.AcceptChanges();

                //写入和更新科目表
                //foreach (var remotesubject in subject_json)
                //{
                //    var localsubject = from c in jiakaoDataSet.subject where remotesubject["id"].ToString() == c.subject_id.ToString() select c;

                //    if (localsubject.Count() == 0)
                //    {
                //        int id = int.Parse(remotesubject["id"].ToString());

                //        string sub;
                //        try { sub = remotesubject["subject"].ToString(); }
                //        catch { sub = ""; }

                //        string upd;
                //        try { upd = remotesubject["updated_at"].ToString(); }
                //        catch { upd = ""; }

                //        jiakaoDataSet.subject.AddsubjectRow(id, sub, upd);
                //    }
                //    else
                //    {
                //        foreach (var ls in localsubject)
                //        {
                //            if (remotesubject["updated_at"].ToString() != ls.updated_at)
                //            {
                //                int id = int.Parse(remotesubject["id"].ToString());

                //                string sub;
                //                try { sub = remotesubject["subject"].ToString(); }
                //                catch { sub = ""; }

                //                string upd;
                //                try { upd = remotesubject["updated_at"].ToString(); }
                //                catch { upd = ""; }

                //                ls.subject_id = id;
                //                ls.subject = sub;
                //                ls.updated_at = upd;
                //            }

                //        }
                //    }
                ThreadPool.QueueUserWorkItem(update_subject_data, subject_json);
                //    now_synccount++;
                //    //xianshi.Text = "更新中..." + (now_synccount / synccount * 100).ToString() + "%";
                //    cur_question_step++;

                //    xianshi.Text = "同步科目..." + cur_question_step;

                //    progress.Value = now_synccount / synccount * 100;
                //    System.Windows.Forms.Application.DoEvents();
                //}
                //xianshi.Text = "写入科目...";
                //System.Windows.Forms.Application.DoEvents();
                //jiakaoDataSetsubjectTableAdapter.Update(jiakaoDataSet.subject);
                //jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);
                //jiakaoDataSet.subject.AcceptChanges();
                #endregion
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }


                //for (int i = 0; i < img_down_list.Count; i++)
                //{
                //    if (img_down_list[i] == "")
                //    {
                //        img_down_list.RemoveAt(i);
                //        i--;
                //    }
                //}
                //for (int i = 0; i < voice_down_list.Count; i++)
                //{
                //    if (voice_down_list[i] == "")
                //    {
                //        voice_down_list.RemoveAt(i);
                //        i--;
                //    }
                //}
                //attch_down_count = img_down_list.Count + voice_down_list.Count;
                //for (int i = 0; i < img_down_list.Count; i++)
                //{
                //    updatedownload(img_down_list[i], "");
                //}
                //for (int i = 0; i < voice_down_list.Count; i++)
                //{
                //    updatedownload("", voice_down_list[i]);
                //}
                //if (attch_down_count == 0)
                //{
                //    data_complete_validate();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void set_chapter(object data)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));

            // 将数据加载到表 chapter 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
            lock (jiakaoDataSet)
            {
                jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);
                jiakaoDataSet.chapter.Clear();
                foreach (var mychapter in local_chapter_data)
                {
                    jiakaoDataSet.chapter.AddchapterRow(mychapter.chapter_id, mychapter.chapter, mychapter.updated_at);
                }
                jiakaoDataSetchapterTableAdapter.Update(jiakaoDataSet.chapter);
                jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);
                jiakaoDataSet.chapter.AcceptChanges();
            }
        }

        private void set_subject(object data)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));

            // 将数据加载到表 subject 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            lock (jiakaoDataSet)
            {
                jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);
                jiakaoDataSet.subject.Clear();
                foreach (var mysubject in local_subject_data)
                {
                    jiakaoDataSet.subject.AddsubjectRow(mysubject.subject_id, mysubject.subject, mysubject.updated_at);
                }
                jiakaoDataSetsubjectTableAdapter.Update(jiakaoDataSet.subject);
                jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);
                jiakaoDataSet.subject.AcceptChanges();
            }
        }

        private void set_all_data(object data)
        {
            float local_count = local_question_data.Count + local_answer_data.Count + local_chapter_data.Count + local_subject_data.Count;
            float local_step = 0;
                        Dispatcher.Invoke(new Action(() =>
                    {

                        List<int> local_temp_question = new List<int>();
                        List<int> remote_temp_question = new List<int>();
                        List<int> del_question = new List<int>();
                        local_temp_question = (from c in local_question_data select c.question_id).ToList();
                        remote_temp_question = (from c in question_json select int.Parse(c["id"].ToString())).ToList();
                        del_question = local_temp_question.Except(remote_temp_question).ToList();
                        if (del_question.Count() > 0)
                        {
                            foreach (var del in del_question)
                            {
                                var mydel = from c in local_question_data where c.question_id == del select c;
                                if (mydel.Count() > 0)
                                {
                                    try
                                    {
                                        string image_attch = System.Windows.Forms.Application.StartupPath + "\\Image\\" + mydel.First().question_image;
                                        if (File.Exists(image_attch))
                                        {
                                            File.Delete(image_attch);
                                        }
                                    }
                                    catch { }


                                    try
                                    {
                                        string voice_attch = System.Windows.Forms.Application.StartupPath + "\\Voice\\" + mydel.First().voice;
                                        if (File.Exists(voice_attch))
                                        {
                                            File.Delete(voice_attch);
                                        }
                                    }
                                    catch { }
                                    local_question_data.Remove(mydel.First());


                                    var del_answer = from c in local_answer_data where c.question_id == mydel.First().question_id select c;
                                    for (int i = 0; i < del_answer.Count(); i++)
                                    {
                                        local_answer_data.Remove(del_answer.First());
                                        i--;
                                    }

                                }
                            }
                        }

            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            lock (jiakaoDataSet)
            {
                jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
                jiakaoDataSet.question.Clear();

                foreach (var myquestion in local_question_data)
                {
                    jiakaoDataSet.question.AddquestionRow(myquestion.question_id, myquestion.chapter_id, myquestion.subject_id, myquestion.question_name, myquestion.question_image, myquestion.voice, myquestion.driverlicense_type, myquestion.question_type, myquestion.update_at, myquestion.is_judge);
                    local_step++;

                }
                jiakaoDataSetquestionTableAdapter.Update(jiakaoDataSet.question);
                jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
                jiakaoDataSet.question.AcceptChanges();
                progress.Value = (int)(local_step / local_count * 100f);
                System.Windows.Forms.Application.DoEvents();
            }

            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            lock (jiakaoDataSet)
            {
                jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
                jiakaoDataSet.answer.Clear();
                foreach (var myanswer in local_answer_data)
                {
                    jiakaoDataSet.answer.AddanswerRow(myanswer.answer_id, myanswer.question_id, myanswer.answer, myanswer.is_right, myanswer.update_at);
                    local_step++;

                }
                jiakaoDataSetanswerTableAdapter.Update(jiakaoDataSet.answer);
                jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
                jiakaoDataSet.answer.AcceptChanges();
                progress.Value = (int)(local_step / local_count * 100f);
                System.Windows.Forms.Application.DoEvents();
            }

            DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
            lock (jiakaoDataSet)
            {
                jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);
                jiakaoDataSet.chapter.Clear();
                foreach (var mychapter in local_chapter_data)
                {
                    jiakaoDataSet.chapter.AddchapterRow(mychapter.chapter_id, mychapter.chapter, mychapter.updated_at);
                    local_step++;

                }
                jiakaoDataSetchapterTableAdapter.Update(jiakaoDataSet.chapter);
                jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);
                jiakaoDataSet.chapter.AcceptChanges();
                progress.Value = (int)(local_step / local_count * 100f);
                System.Windows.Forms.Application.DoEvents();
            }

            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            lock (jiakaoDataSet)
            {
                jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);
                jiakaoDataSet.subject.Clear();
                foreach (var mysubject in local_subject_data)
                {
                    jiakaoDataSet.subject.AddsubjectRow(mysubject.subject_id, mysubject.subject, mysubject.updated_at);
                    local_step++;

                }
                jiakaoDataSetsubjectTableAdapter.Update(jiakaoDataSet.subject);
                jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);
                jiakaoDataSet.subject.AcceptChanges();
                progress.Value = (int)(local_step / local_count * 100f);
                System.Windows.Forms.Application.DoEvents();
            }

            xianshi.Text = "更新完成";
            user_textBox.IsEnabled = true;
            password_textBox.IsEnabled = true;
            progress.Visibility = System.Windows.Visibility.Collapsed;
                    }));

        }

        private void set_question(object data)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            lock (jiakaoDataSet)
            {
                jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
                jiakaoDataSet.question.Clear();

                foreach (var myquestion in local_question_data)
                {
                    jiakaoDataSet.question.AddquestionRow(myquestion.question_id, myquestion.chapter_id, myquestion.subject_id, myquestion.question_name, myquestion.question_image, myquestion.voice, myquestion.driverlicense_type, myquestion.question_type, myquestion.update_at, myquestion.is_judge);
                }
                jiakaoDataSetquestionTableAdapter.Update(jiakaoDataSet.question);
                jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
                jiakaoDataSet.question.AcceptChanges();
            }


        }


        private void set_answer(object data)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            lock (jiakaoDataSet)
            {
                jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
                jiakaoDataSet.answer.Clear();
                foreach (var myanswer in local_answer_data)
                {
                    jiakaoDataSet.answer.AddanswerRow(myanswer.answer_id, myanswer.question_id, myanswer.answer, myanswer.is_right, myanswer.update_at);
                }
                jiakaoDataSetanswerTableAdapter.Update(jiakaoDataSet.answer);
                jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
                jiakaoDataSet.answer.AcceptChanges();
            }
        }

        private void get_question(object data)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
            foreach (var ques in jiakaoDataSet.question)
            {
                PublicClass.questions question = new PublicClass.questions();
                question.question_id = ques.question_id;
                question.chapter_id = ques.chapter_id;
                question.subject_id = ques.subject_id;
                question.question_name = ques.question_name;
                question.question_image = ques.question_image;
                question.voice = ques.voice;
                question.driverlicense_type = ques.driverlicense_type;
                question.question_type = ques.question_type;
                question.update_at = ques.update_at;
                question.is_judge = ques.is_judge;
                PublicClass.question_data.Add(question);
            }
        }

        private void get_avswer(object data)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
            foreach (var myanswer in jiakaoDataSet.answer)
            {
                PublicClass.answers answer = new PublicClass.answers();
                answer.answer_id = myanswer.answer_id;
                answer.question_id = myanswer.question_id;
                answer.answer = myanswer.answer;
                answer.is_right = myanswer.is_right;
                answer.update_at = myanswer.update_at;
                PublicClass.answer_data.Add(answer);
            }

        }


        //更新图片及语音
        private void updatedownload(string attch, string voice)
        {
            try
            {
                WebClient downclient = new WebClient();
                WebClient voiceclient = new WebClient();

                string imagepath = System.Windows.Forms.Application.StartupPath + "\\Image\\";
                string voicepath = System.Windows.Forms.Application.StartupPath + "\\Voice\\";
                if (!Directory.Exists(imagepath))//如果路径不存在
                {
                    Directory.CreateDirectory(imagepath);//创建一个路径的文件夹
                }
                if (!Directory.Exists(voicepath))//如果路径不存在
                {
                    Directory.CreateDirectory(voicepath);//创建一个路径的文件夹
                }


                if (attch != "")
                {
                    string httpaddr = PublicClass.http + @"/questionimages/" + attch;
                    downclient.DownloadFileAsync(new Uri(httpaddr), imagepath + attch);
                    downclient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(web_DownloadFileCompleted);
                    downclient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(web_DownloadProgressChanged);

                }
                if (voice != "")
                {
                    string httpaddr = PublicClass.http + @"/voices/" + voice;
                    downclient.DownloadFileAsync(new Uri(httpaddr), voicepath + voice);
                    downclient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(web_DownloadFileCompleted);
                    downclient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(web_DownloadProgressChanged);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        //下载进度改变时发生
        void web_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //xianshi.Text = string.Format("开始下载文件... 已下载:{0}Mb 剩余:{1}Mb 已完成:{2}%",
            //    e.BytesReceived / 1024 / 1024,
            //    e.TotalBytesToReceive / 1024 / 1024,
            //    e.ProgressPercentage.ToString("N2"));

                                               Dispatcher.Invoke(new Action(() =>
                    {
            if (e.ProgressPercentage > progress.Value)
            {
                progress.Value = e.ProgressPercentage;
                System.Windows.Forms.Application.DoEvents();
            }
            //xianshi.Text = e.ProgressPercentage.ToString();
                    }));

        }

        //下载完成时发生
        void web_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
                                                           Dispatcher.Invoke(new Action(() =>
                    {
            try
            {
                if (e.Cancelled)
                {
                    xianshi.Text = "下载取消";
                    cur_cown_count++;
                }
                else
                {
                    //xianshi.Text = "下载完毕,更新已完成";
                    cur_cown_count++;
                    xianshi.Text = "同步附件...  " + cur_cown_count + "/" + attch_down_count;

                    progress.Value = 0;
                    if (cur_cown_count == attch_down_count)
                    {
                        data_complete_validate();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                    }));
        }

        //同步完成后，本地数据完整性验证
        private void data_complete_validate()
        {
            if (question_update_compaleted && answer_update_compaleted && cur_cown_count == attch_down_count)
            {
                try
                {
                    //DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));

                    //// 将数据加载到表 question 中。可以根据需要修改此代码。
                    //DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
                    //jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

                    //// 将数据加载到表 answer 中。可以根据需要修改此代码。
                    //DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
                    //jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

                    //// 将数据加载到表 chapter 中。可以根据需要修改此代码。
                    //DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
                    //jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);

                    //// 将数据加载到表 subject 中。可以根据需要修改此代码。
                    //DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
                    //jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);

                    xianshi.Text = "验证数据...";
                    System.Windows.Forms.Application.DoEvents();
                    //progress.Visibility = System.Windows.Visibility.Collapsed;

                    var getchkupdstr = getupdatecheck();
                    version(getchkupdstr);

                    //ThreadPool.QueueUserWorkItem(set_question, "");
                    //ThreadPool.QueueUserWorkItem(set_answer, "");
                    //ThreadPool.QueueUserWorkItem(set_chapter, "");
                    //ThreadPool.QueueUserWorkItem(set_subject, "");
                    //set_question("");
                    //set_answer("");
                    ThreadPool.QueueUserWorkItem(set_all_data, "");
                    ThreadPool.QueueUserWorkItem(push_to_public, "");


                    //xianshi.Text = "更新完成";
                    //user_textBox.IsEnabled = true;
                    //password_textBox.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }



            }
        }

        //登录验证
        private bool testlogin()
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 user 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter jiakaoDataSetuserTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter();
            jiakaoDataSetuserTableAdapter.Fill(jiakaoDataSet.user);


            string loginstr = null;//数据流
            string passwordstr = null;//数据流

            string login_status = "";//登录验证状态
            int login_id = 0;//登录账号ID

            //登录用户信息
            string login_head = "";
            string login_studentid = "";
            string login_sex = "";
            string login_age = "";
            string login_idcard = "";
            string login_name = "";
            string login_money = "";
            string login_model = "";
            string login_Subject = "";
            string login_type = "";
            string login_number = "";
            string login_time = "";
            string login_education = "";
            string login_phonenumber = "";
            string login_part = "";

            HttpWebResponse response = null;
            StreamReader reader = null;

            string userlogin = user_textBox.Text;//用户名
            PublicClass.login = userlogin; //储存登录用户名
            string userpassword = password_textBox.Password;//密码

            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/getvalidate?login=" + userlogin);//随机UUID url
                request.Method = "GET";
                request.Timeout = 20000;
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                loginstr = reader.ReadToEnd();

                JArray loginUUID_json = JArray.Parse(loginstr);//UUID json


                string uuid = loginUUID_json[0]["validate"].ToString();//随机码
                string number = loginUUID_json[0]["number"].ToString();
                string time = loginUUID_json[0]["time"].ToString();//剩余时间或次数
                string state = loginUUID_json[0]["status"].ToString();//账号状态

                if (state != "1")
                {
                    goto L1;
                }




                string loginMD5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(userlogin + userpassword + uuid + "CLOUDTIMESOFT", "MD5");//账号+密码+随机码+指定字符生成MD5

                GetIP();
                request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/getuser?login=" + userlogin + "&validate=" + loginMD5 + "&ip=" + ip);//验证 url
                request.Method = "GET";
                request.Timeout = 30000;
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                passwordstr = reader.ReadToEnd();
                if (passwordstr.Substring(0, 1) != "[")
                {
                    passwordstr = "[" + passwordstr + "]";
                }
                JArray passwordMD5_json = JArray.Parse(passwordstr);
                string json = passwordMD5_json.ToString();

                login_status = passwordMD5_json[0]["status"].ToString();

                if (login_status == "error")//验证不通过
                {
                    MessageBox.Show("账号或密码错误!");
                    return false;
                }
                else //验证通过
                {
                    login_id = int.Parse(passwordMD5_json[0]["id"].ToString());
                    PublicClass.user_id = login_id;
                    login_head = passwordMD5_json[0]["head"].ToString();
                    login_studentid = passwordMD5_json[0]["studentid"].ToString();
                    login_sex = passwordMD5_json[0]["sex"].ToString();
                    login_age = passwordMD5_json[0]["age"].ToString();
                    login_idcard = passwordMD5_json[0]["idcard"].ToString();
                    login_name = passwordMD5_json[0]["name"].ToString();
                    login_money = passwordMD5_json[0]["money"].ToString();
                    login_model = passwordMD5_json[0]["model"].ToString();
                    login_Subject = passwordMD5_json[0]["Subject"].ToString();
                    login_type = passwordMD5_json[0]["logintype"].ToString();
                    login_number = passwordMD5_json[0]["loginnumber"].ToString();
                    login_time = passwordMD5_json[0]["logintime"].ToString();
                    login_education = passwordMD5_json[0]["education"].ToString();
                    login_phonenumber = passwordMD5_json[0]["phonenumber"].ToString();
                    login_part = passwordMD5_json[0]["part"].ToString();

                    var user = from c in jiakaoDataSet.user where c.user_id == PublicClass.user_id select c;

                    if (user.Count() == 0)
                    {
                        jiakaoDataSet.user.AdduserRow(PublicClass.user_id, PublicClass.login, userpassword, login_head, login_studentid, login_sex, login_age, login_idcard, login_name, login_money, login_model, login_Subject, login_type, login_number, login_time, state, login_education, login_phonenumber, login_part);
                        jiakaoDataSetuserTableAdapter.Update(jiakaoDataSet.user);
                        jiakaoDataSet.user.AcceptChanges();
                    }

                    return true;
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
            }

        L1: //goto跳转至这里
            return false;


        }

        //获取本机公网IP
        private string GetIP()
        {
            string tempip = "";
            try
            {

                WebRequest request = WebRequest.Create("http://ip.qq.com/");
                request.Timeout = 10000;
                WebResponse response = request.GetResponse();
                Stream resStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(resStream, System.Text.Encoding.Default);
                string htmlinfo = sr.ReadToEnd();
                //匹配IP的正则表达式
                Regex r = new Regex("((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|\\d)\\.){3}(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|[1-9])", RegexOptions.None);
                Match mc = r.Match(htmlinfo);
                //获取匹配到的IP
                tempip = mc.Groups[0].Value;

                resStream.Close();
                sr.Close();
                ip = tempip;
                return tempip;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return tempip;
            }
        }


        #region 首页动画
        //指针进入前轮
        private void qianlun_MouseEnter(object sender, MouseEventArgs e)
        {
            RotateTransform rtf = new RotateTransform();
            qianlun.RenderTransform = rtf;
            DoubleAnimation da = new DoubleAnimation();
            da.To = -360;
            da.Duration = TimeSpan.FromSeconds(1);
            da.RepeatBehavior = RepeatBehavior.Forever;
            rtf.BeginAnimation(RotateTransform.AngleProperty, da);

            //this.zhuce.Visibility = System.Windows.Visibility.Visible;
            DoubleAnimation touming = new DoubleAnimation();
            touming.To = 1;
            touming.Duration = TimeSpan.FromSeconds(1);
            qianlunqipao.BeginAnimation(Image.OpacityProperty, touming);

            //RotateTransform qipao = new RotateTransform();
            //qianlunqipao.RenderTransform = qipao;
            //DoubleAnimation zh = new DoubleAnimation();
            //zh.To = -360;
            //zh.Duration = TimeSpan.FromSeconds(1);
            //zh.RepeatBehavior = RepeatBehavior.Forever;
            //qipao.BeginAnimation(RotateTransform.AngleProperty, zh);
        }

        //指针离开前轮
        private void qianlun_MouseLeave(object sender, MouseEventArgs e)
        {
            RotateTransform rtf = new RotateTransform();
            qianlun.RenderTransform = rtf;
            DoubleAnimation da = new DoubleAnimation();
            da.To = -360;
            da.Duration = TimeSpan.FromSeconds(1);
            //da.RepeatBehavior = RepeatBehavior.Forever;
            rtf.BeginAnimation(RotateTransform.AngleProperty, da);

            //this.zhuce.Visibility = System.Windows.Visibility.Hidden;


            DoubleAnimation touming = new DoubleAnimation();
            touming.To = 0;
            touming.Duration = TimeSpan.FromSeconds(1);
            qianlunqipao.BeginAnimation(Image.OpacityProperty, touming);

            //RotateTransform qipao = new RotateTransform();
            //qianlunqipao.RenderTransform = qipao;
            //DoubleAnimation zh = new DoubleAnimation();
            //zh.To = -360;
            //zh.Duration = TimeSpan.FromSeconds(1);
            //zh.RepeatBehavior = RepeatBehavior.Forever;
            //qipao.BeginAnimation(RotateTransform.AngleProperty, zh);



        }
        //指针进入后轮
        private void houlun_MouseEnter(object sender, MouseEventArgs e)
        {
            RotateTransform rtf = new RotateTransform();
            houlun.RenderTransform = rtf;
            DoubleAnimation da = new DoubleAnimation();
            da.To = -360;
            da.Duration = TimeSpan.FromSeconds(1);
            da.RepeatBehavior = RepeatBehavior.Forever;
            rtf.BeginAnimation(RotateTransform.AngleProperty, da);

            //this.login.Visibility = System.Windows.Visibility.Visible;
            DoubleAnimation touming = new DoubleAnimation();
            touming.To = 1;
            touming.Duration = TimeSpan.FromSeconds(1);
            houlunqipao.BeginAnimation(Image.OpacityProperty, touming);

            //RotateTransform qipao = new RotateTransform();
            //houlunqipao.RenderTransform = qipao;
            //DoubleAnimation zh = new DoubleAnimation();
            //zh.To = -360;
            //zh.Duration = TimeSpan.FromSeconds(1);
            //zh.RepeatBehavior = RepeatBehavior.Forever;
            //qipao.BeginAnimation(RotateTransform.AngleProperty, zh);
        }
        //指针离开后轮
        private void houlun_MouseLeave(object sender, MouseEventArgs e)
        {
            RotateTransform rtf = new RotateTransform();
            houlun.RenderTransform = rtf;
            DoubleAnimation da = new DoubleAnimation();
            da.To = -360;
            da.Duration = TimeSpan.FromSeconds(1);
            //da.RepeatBehavior = RepeatBehavior.Forever;
            rtf.BeginAnimation(RotateTransform.AngleProperty, da);

            //this.login.Visibility = System.Windows.Visibility.Hidden;

            DoubleAnimation touming = new DoubleAnimation();
            touming.To = 0;
            touming.Duration = TimeSpan.FromSeconds(1);
            houlunqipao.BeginAnimation(Image.OpacityProperty, touming);

            //RotateTransform qipao = new RotateTransform();
            //houlunqipao.RenderTransform = qipao;
            //DoubleAnimation zh = new DoubleAnimation();
            //zh.To = -360;
            //zh.Duration = TimeSpan.FromSeconds(1);
            //zh.RepeatBehavior = RepeatBehavior.Forever;
            //qipao.BeginAnimation(RotateTransform.AngleProperty, zh);
        }

        //注册按钮调用前轮动画
        private void zhuce_MouseEnter(object sender, MouseEventArgs e)
        {
            qianlun_MouseEnter(null, null);
        }
        private void zhuce_MouseLeave(object sender, MouseEventArgs e)
        {
            qianlun_MouseLeave(null, null);
        }
        //登录按钮调用后轮动画
        private void login_MouseEnter(object sender, MouseEventArgs e)
        {
            houlun_MouseEnter(null, null);
        }

        private void login_MouseLeave(object sender, MouseEventArgs e)
        {
            houlun_MouseLeave(null, null);
        }

        private void image1_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void subject1_MouseEnter(object sender, MouseEventArgs e)
        {
            //image1.Visibility = System.Windows.Visibility.Visible;
            //image1_MouseEnter(null, null);
        }



        private void subject1_MouseLeave(object sender, MouseEventArgs e)
        {
            //image1.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion

        //关闭按钮
        private void guanbi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //最小化按钮
        private void zuixiao_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        //注册
        private void zhuce_Click(object sender, RoutedEventArgs e)
        {
            register re = new register();
            re.ChangeTextEvent += new DrivingTest.register.ChangeTextHandler(updateUI);
            re.Show();
        }


        //用户名框失去焦点时
        private void user_textBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (user_textBox.Text != "")
            {
                string loginstr = null;
                HttpWebResponse response = null;
                StreamReader reader = null;
                try
                {

                    string userlogin = user_textBox.Text;//用户名

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/getvalidate?login=" + userlogin);//随机UUID url
                    request.Method = "GET";
                    request.Timeout = 10000;
                    response = (HttpWebResponse)request.GetResponse();
                    reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                    loginstr = reader.ReadToEnd();

                    JArray loginUUID_json = JArray.Parse(loginstr);//UUID json


                    string state = loginUUID_json[0]["status"].ToString();//账号状态
                    string number = loginUUID_json[0]["number"].ToString();
                    string time = loginUUID_json[0]["time"].ToString();

                    //string surplus = number + time//剩余时间和次数

                    if (state == "1")
                    {
                        xianshi.Text = "账号截止日期为: " + time + " 剩余次数为: " + number;
                    }
                    else
                    {
                        xianshi.Text = "此账号已被禁用";
                    }
                }
                catch (Exception ex)
                {
                    xianshi.Text = "无法获取此账号截止日期或剩余次数";
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                    if (response != null)
                    {
                        response.Close();
                    }
                }
            }
            else { }
        }
        //登录
        private void login_Click(object sender, RoutedEventArgs e)
        {
            //Thread newthread = new Thread(new ThreadStart(() =>
            //{
            //    Dispatcher.Invoke(new Action(() =>
            //    {
            MediaPlayer player = new MediaPlayer();

            string debug = System.AppDomain.CurrentDomain.BaseDirectory;
            string proj = System.IO.Path.Combine(debug, @"..\..\");
            player.Open(new Uri(proj + "/sounds/start.wav", UriKind.Relative));
            player.Play();
            //int sta = await Task.Run(() =>
            //{
            //    return my_connect.login(_user_name, _password);
            //});
           
            if (testlogin() == false)//验证
            {
                MessageBox.Show("登录失败!");
            }
            else
            {
                login_control_ShowHide();//登录后控件的显示或隐藏

                DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
                // 将数据加载到表 question 中。可以根据需要修改此代码。

                DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
                jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);


                string err = null;//数据流
                HttpWebResponse response = null;
                StreamReader reader = null;

                try
                {
                    string url = PublicClass.http + @"/returnjsons/r_errquests?" + "user_id=" + PublicClass.user_id;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//提交请求
                    request.Method = "GET";

                    request.Timeout = 10000;
                    response = (HttpWebResponse)request.GetResponse();
                    reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                    err = reader.ReadToEnd();


                    JArray errquestion_json = JArray.Parse(err);//错题 json
                    string json = errquestion_json.ToString();

                    foreach (var err_question in errquestion_json)//写入错题
                    {
                        var errquestion = from c in jiakaoDataSet.errquest where c.question_id.ToString() == err_question["question_id"].ToString() && c.user_id.ToString() == err_question["user_id"].ToString() select c;

                        if (errquestion.Count() == 0)
                        {
                            jiakaoDataSet.errquest.AdderrquestRow(int.Parse(err_question["user_id"].ToString()), int.Parse(err_question["question_id"].ToString()), int.Parse(err_question["amount"].ToString()));
                        }
                        else
                        {
                            errquestion.First().amount = int.Parse(err_question["amount"].ToString());
                        }
                        jiakaoDataSeterrquestTableAdapter.Update(jiakaoDataSet.errquest);
                        jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);
                        jiakaoDataSet.errquest.AcceptChanges();

                    }

                    response.Close();
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
                //    }));



                //}));
        }


        //登录后控件的显示或隐藏
        private void login_control_ShowHide()
        {
            //登陆后显示
            subject1.Visibility = System.Windows.Visibility.Visible;
            subject2.Visibility = System.Windows.Visibility.Visible;
            subject3.Visibility = System.Windows.Visibility.Visible;
            subject4.Visibility = System.Windows.Visibility.Visible;
            car_button.Visibility = System.Windows.Visibility.Visible;
            bus_button.Visibility = System.Windows.Visibility.Visible;
            truck_button.Visibility = System.Windows.Visibility.Visible;
            motorcycle_button.Visibility = System.Windows.Visibility.Visible;
            regain_button.Visibility = System.Windows.Visibility.Visible;
            set_up.Visibility = System.Windows.Visibility.Visible;
            revisions.Visibility = System.Windows.Visibility.Visible;
            //登录后隐藏
            xianshi.Visibility = System.Windows.Visibility.Hidden;
            user_label.Visibility = System.Windows.Visibility.Hidden;
            user_textBox.Visibility = System.Windows.Visibility.Hidden;
            password_label.Visibility = System.Windows.Visibility.Hidden;
            password_textBox.Visibility = System.Windows.Visibility.Hidden;
            zhuce.Visibility = System.Windows.Visibility.Hidden;
            login.Visibility = System.Windows.Visibility.Hidden;
            qianlunqipao.Visibility = System.Windows.Visibility.Hidden;
            houlunqipao.Visibility = System.Windows.Visibility.Hidden;
        }

        public string subjectname;
        //科目一
        private void subject1_Click(object sender, RoutedEventArgs e)
        {
            subject1.Foreground = new SolidColorBrush(Colors.Red);
            subject4.Foreground = new SolidColorBrush(Colors.White);
            subjectname = "科目一";


        }
        //科目二
        private void subject2_Click(object sender, RoutedEventArgs e)
        {
            SubjectTwo ma = new SubjectTwo();
            C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
            c1ma.Content = ma;
            c1ma.Show();
            //c1ma.ToolTip = "科目二";
            c1ma.Name = "科目二";
            c1ma.Header = "科目二";
            c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
            this.WindowState = System.Windows.WindowState.Maximized;
            maincanvas.Visibility = Visibility.Hidden;
            c1ma.Closed += new EventHandler(c1ma_Closed);

            //this.Visibility = System.Windows.Visibility.Collapsed;
            //this.WindowState = System.Windows.WindowState.Minimized;
        }
        //科目三
        private void subject3_Click(object sender, RoutedEventArgs e)
        {
            SubjectThree ma = new SubjectThree();
            C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
            c1ma.Content = ma;
            c1ma.Show();
            //c1ma.ToolTip = "科目三";
            c1ma.Name = "科目三";
            c1ma.Header = "科目三";
            c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
            this.WindowState = System.Windows.WindowState.Maximized;
            maincanvas.Visibility = Visibility.Hidden;
            c1ma.Closed += new EventHandler(c1ma_Closed);
            //this.Visibility = System.Windows.Visibility.Collapsed;
        }
        //科目四
        private void subject4_Click(object sender, RoutedEventArgs e)
        {
            subject4.Foreground = new SolidColorBrush(Colors.Red);
            subject1.Foreground = new SolidColorBrush(Colors.White);
            subjectname = "科目四";

        }
        //设置
        private void set_up_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
            PublicClass.shezhi = "设置";
            foreach (var s in set)
            {

                if (s.password == "") //OleDbDataReader["close_password"].toString() != "")
                {
                    SetUp se = new SetUp();
                    se.Height = 600;
                    se.Width = 500;
                    se.ChangeTextEvent += new DrivingTest.SetUp.ChangeTextHandler(updateUI);
                    se.ShowDialog();
                }
                else
                {
                    QuitPassword qu = new QuitPassword();
                    C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
                    c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - qu.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - qu.Height / 2, 0, 0);
                    c1ma.Content = qu;
                    c1ma.Show();
                    c1ma.Name = "管理员密码";
                    c1ma.Header = "管理员密码";
                }
            }


        }

        //c1c2c3c4
        private void car_button_Click(object sender, RoutedEventArgs e)
        {

            if (subjectname != "")
            {
                if (subjectname == "科目一")
                {
                    MainScreen ma = new MainScreen();
                    C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
                    PublicClass.cartype = "C1";
                    PublicClass.subjection = "科目一";
                    c1ma.Content = ma;

                    //c1ma.ToolTip = "小车类:科目一";
                    c1ma.Name = "科目一";
                    c1ma.Header = "小车类:科目一";
                    c1ma.Show();
                    //c1ma.Background = Brushes.SkyBlue;
                    c1ma.IsResizable = false;

                    c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
                    maincanvas.Visibility = Visibility.Hidden;
                    System.Windows.Forms.Application.DoEvents();
                    this.WindowState = System.Windows.WindowState.Maximized;
                    
                    c1ma.Closed += new EventHandler(c1ma_Closed);
                }
                else if (subjectname == "科目四")
                {
                    MainScreen ma = new MainScreen();
                    C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
                    PublicClass.cartype = "C1";
                    PublicClass.subjection = "科目四";
                    c1ma.Content = ma;
                    c1ma.Show();
                    //c1ma.ToolTip = "小车类:科目四";
                    c1ma.Name = "科目四";
                    c1ma.Header = "小车类:科目四";
                    c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
                    maincanvas.Visibility = Visibility.Hidden;
                    System.Windows.Forms.Application.DoEvents();
                    this.WindowState = System.Windows.WindowState.Maximized;
                    //maingrid.Width = SystemParameters.WorkArea.Width;
                    //maingrid.Height = SystemParameters.WorkArea.Height;
                    
                    c1ma.Closed += new EventHandler(c1ma_Closed);
                    //this.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
            }

        }

        void c1ma_Closed(object sender, EventArgs e)
        {
            //Canvas main = MainWindow.FindChild<Canvas>(Application.Current.MainWindow, "maincanvas");
            //if (main.Visibility == Visibility.Hidden)
            //{
            this.WindowState = System.Windows.WindowState.Normal;
            maincanvas.Visibility = Visibility.Visible;
            //maincanvas.Width = 0;
            //maincanvas.Height = 0;
            //}
        }
        //a1a3b1
        private void bus_button_Click(object sender, RoutedEventArgs e)
        {
            if (subjectname != "")
            {
                if (subjectname == "科目一")
                {
                    MainScreen ma = new MainScreen();
                    C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
                    PublicClass.cartype = "A1";
                    PublicClass.subjection = "科目一";
                    c1ma.Content = ma;
                    c1ma.Show();
                    //c1ma.ToolTip = "客车类:科目一";
                    c1ma.Name = "科目一";
                    c1ma.Header = "客车类:科目一";
                    c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
                    maincanvas.Visibility = Visibility.Hidden;
                    System.Windows.Forms.Application.DoEvents();
                    this.WindowState = System.Windows.WindowState.Maximized;
                    //maingrid.Width = SystemParameters.WorkArea.Width;
                    //maingrid.Height = SystemParameters.WorkArea.Height;
                    
                    c1ma.Closed += new EventHandler(c1ma_Closed);
                    //this.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (subjectname == "科目四")
                {
                    MainScreen ma = new MainScreen();
                    C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
                    PublicClass.cartype = "A1";
                    PublicClass.subjection = "科目四";
                    c1ma.Content = ma;
                    c1ma.Show();
                    //c1ma.ToolTip = "客车类:科目四";
                    c1ma.Name = "科目四";
                    c1ma.Header = "客车类:科目四";
                    //this.Visibility = System.Windows.Visibility.Collapsed;
                    c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
                    maincanvas.Visibility = Visibility.Hidden;
                    System.Windows.Forms.Application.DoEvents();
                    this.WindowState = System.Windows.WindowState.Maximized;
                    //maingrid.Width = SystemParameters.WorkArea.Width;
                    //maingrid.Height = SystemParameters.WorkArea.Height;
                    
                    c1ma.Closed += new EventHandler(c1ma_Closed);
                }
            }
        }
        //a2b2
        private void truck_button_Click(object sender, RoutedEventArgs e)
        {
            if (subjectname != "")
            {
                if (subjectname == "科目一")
                {
                    MainScreen ma = new MainScreen();
                    C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
                    PublicClass.cartype = "A2";
                    PublicClass.subjection = "科目一";
                    c1ma.Content = ma;
                    c1ma.Show();
                    //c1ma.ToolTip = "货车类:科目一";
                    c1ma.Name = "科目一";
                    c1ma.Header = "货车类:科目一";
                    //this.Visibility = System.Windows.Visibility.Collapsed;
                    c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
                    maincanvas.Visibility = Visibility.Hidden;
                    System.Windows.Forms.Application.DoEvents();
                    this.WindowState = System.Windows.WindowState.Maximized;
                    //maingrid.Width = SystemParameters.WorkArea.Width;
                    //maingrid.Height = SystemParameters.WorkArea.Height;
                    
                    c1ma.Closed += new EventHandler(c1ma_Closed);
                }
                else if (subjectname == "科目四")
                {
                    MainScreen ma = new MainScreen();
                    C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
                    PublicClass.cartype = "A2";
                    PublicClass.subjection = "科目四";
                    c1ma.Content = ma;
                    c1ma.Show();
                    //c1ma.ToolTip = "货车类:科目四";
                    c1ma.Name = "科目四";
                    c1ma.Header = "货车类:科目四";
                    //this.Visibility = System.Windows.Visibility.Collapsed;
                    c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
                    maincanvas.Visibility = Visibility.Hidden;
                    System.Windows.Forms.Application.DoEvents();
                    this.WindowState = System.Windows.WindowState.Maximized;
                    //maingrid.Width = SystemParameters.WorkArea.Width;
                    //maingrid.Height = SystemParameters.WorkArea.Height;
                    
                    c1ma.Closed += new EventHandler(c1ma_Closed);
                }
            }
            else
            {
            }
        }
        //def
        private void motorcycle_button_Click(object sender, RoutedEventArgs e)
        {
            if (subjectname != "")
            {
                if (subjectname == "科目一")
                {
                    MainScreen ma = new MainScreen();
                    C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
                    PublicClass.cartype = "D";
                    PublicClass.subjection = "科目一";
                    c1ma.Content = ma;
                    c1ma.Show();
                    //c1ma.ToolTip = "摩托车类:科目一";
                    c1ma.Name = "科目一";
                    c1ma.Header = "摩托车类:科目一";
                    //this.Visibility = System.Windows.Visibility.Collapsed;
                    c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
                    maincanvas.Visibility = Visibility.Hidden;
                    System.Windows.Forms.Application.DoEvents();
                    this.WindowState = System.Windows.WindowState.Maximized;
                    //maingrid.Width = SystemParameters.WorkArea.Width;
                    //maingrid.Height = SystemParameters.WorkArea.Height;
                    
                    c1ma.Closed += new EventHandler(c1ma_Closed);
                }
                else if (subjectname == "科目四")
                {
                    MainScreen ma = new MainScreen();
                    C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
                    PublicClass.cartype = "D";
                    PublicClass.subjection = "科目四";
                    c1ma.Content = ma;
                    c1ma.Show();
                    //c1ma.ToolTip = "摩托车类:科目四";
                    c1ma.Name = "科目四";
                    c1ma.Header = "摩托车类:科目四";
                    //this.Visibility = System.Windows.Visibility.Collapsed;
                    c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
                    maincanvas.Visibility = Visibility.Hidden;
                    System.Windows.Forms.Application.DoEvents();
                    this.WindowState = System.Windows.WindowState.Maximized;
                    //maingrid.Width = SystemParameters.WorkArea.Width;
                    //maingrid.Height = SystemParameters.WorkArea.Height;
                    
                    c1ma.Closed += new EventHandler(c1ma_Closed);
                }
            }
            else
            {
            }
        }
        //恢复驾考
        private void regain_button_Click(object sender, RoutedEventArgs e)
        {
            MainScreen ma = new MainScreen();
            C1.WPF.C1Window c1ma = new C1.WPF.C1Window();
            PublicClass.cartype = "HF";
            c1ma.Content = ma;
            //c1ma.ToolTip = "恢复驾驶资格类";
            c1ma.Name = "恢复驾驶资格类";
            c1ma.Header = "恢复驾驶资格类";
            c1ma.Show();
            c1ma.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ma.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ma.Height / 2, 0, 0);
            maincanvas.Visibility = Visibility.Hidden;
            System.Windows.Forms.Application.DoEvents();
            this.WindowState = System.Windows.WindowState.Maximized;
            //maingrid.Width = SystemParameters.WorkArea.Width;
            //maingrid.Height = SystemParameters.WorkArea.Height;
            
            c1ma.Closed += new EventHandler(c1ma_Closed);
        }

        //修改密码
        private void revisions_Click(object sender, RoutedEventArgs e)
        {
            ModifyPassword mo = new ModifyPassword();
            mo.Title = "修改密码";
            mo.Show();
        }

        //private Thread thread1; 
        //thread1 = new Thread(new ThreadStart(a));
        //ThreadStart thrreadstart = new ThreadStart(a);



        //void a()
        //{
        //    Thread thread = new Thread(new ParameterizedThreadStart(testlogin));
        //}
        private void user_textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            user_textBox.GotFocus -= new RoutedEventHandler(user_textBox_GotFocus);
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 updatecheck 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.updatecheckTableAdapter jiakaoDataSetupdatecheckTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.updatecheckTableAdapter();
            jiakaoDataSetupdatecheckTableAdapter.Fill(jiakaoDataSet.updatecheck);
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);


        }

        private void shiyon_button_Click(object sender, RoutedEventArgs e)
        {
            MainExam ma = new MainExam();
            //ma.Title = "试用";
            //ma.Show();
        }

        private void xianshi_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        Point old_point;
        Thickness old_margin;
        string move;
        private void maincanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            old_point = e.GetPosition(this);
            old_margin = maincanvas.Margin;
            move = "move";

        }

        private void maincanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (move == "move")
                {
                    maincanvas.Margin = new Thickness(old_margin.Left + e.GetPosition(this).X - old_point.X, old_margin.Top + e.GetPosition(this).Y - old_point.Y, 0, 0);
                    //user_textBox.Text = maincanvas.Margin.Left.ToString() + "  " + e.GetPosition(this).X.ToString();
                }
            }
        }

        private void maincanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            move = "";
        }

        //密码框回车事件
        private void password_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                login_Click(null, null);
            }
        }

   











    }
}
