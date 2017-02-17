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
using System.Net;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Timers;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace DrivingTest
{
    /// <summary>
    /// MainScreen.xaml 的交互逻辑
    /// </summary>
    public partial class MainScreen : UserControl
    {
        public MainScreen()
        {
            InitializeComponent();
            //this.Closing += F;
        }


        List<int> chapter_index = new List<int>();//章节index
        List<int> class_index = new List<int>();//分类index
        List<string> zhuanxiang_index = new List<string>();//专项字段关键字
        string classflag = "";//试题一级分类
        string classtype = "";//试题二级分类

        int class_status = 0;//分类状态,1为常规状态,2为专项,3为章节,4为常规模拟

        int subject_id;
        string subject_name;
        List<int> questions_id = new List<int>();//题号序列
        Timer timer = new Timer();
        int imgheight = 0;

        //int control_state = 0;//控件显示状态 1为触发,0为不触发

        private void F(object o, System.ComponentModel.CancelEventArgs e)
        {
            Window main = Application.Current.MainWindow;
            main.Visibility = System.Windows.Visibility.Visible;
            //main.WindowState = System.Windows.WindowState.Normal;


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/DrivingTest;component/Images/窗体背景.png"));
            b.Stretch = Stretch.Fill;
            this.Background = b;

            //guanggao_image.Source = new BitmapImage(new Uri(System.Windows.Forms.Application.StartupPath + "\\Image\\Advertise\\car1.jpg"));

            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 chapter 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
            jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);

            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            // 将数据加载到表 subject 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);

            var sub = from c in jiakaoDataSet.subject where c.subject.Contains(PublicClass.subjection) select c;
            subject_id = sub.First().subject_id;

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;


            foreach (var s in set)
            {
                if (s.functional_module != "")
                {
                    PublicClass.gongneng = s.functional_module.Split(',');
                }
                else
                {
                    PublicClass.gongneng = new string[6] { "", "", "", "", "", "" };
                }
                if (PublicClass.gongneng[0] == "1")//章节练习
                {
                    zhangjielianxi.Visibility = System.Windows.Visibility.Hidden;
                    simulation_test.Margin = new Thickness(31, 412, 0, 0); //仿真考试向上补位
                    my_mistakes.Margin = new Thickness(31, 458, 0, 0); //我的错题向上补位
                }
                else
                {
                    zhangjielianxi.Visibility = System.Windows.Visibility.Visible;
                }

                if (PublicClass.gongneng[4] == "1")//仿真考试
                {
                    simulation_test.Visibility = System.Windows.Visibility.Hidden;
                    my_mistakes.Margin = new Thickness(31, 458, 0, 0); //我的错题向上补位
                }
                else
                {
                    simulation_test.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.gongneng[5] == "1")//我的错题
                {
                    my_mistakes.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    my_mistakes.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.gongneng[0] == "1" && PublicClass.gongneng[4] == "1")//章节练习与我的错题
                {
                    zhangjielianxi.Visibility = System.Windows.Visibility.Hidden;
                    simulation_test.Visibility = System.Windows.Visibility.Hidden;
                    my_mistakes.Margin = new Thickness(31, 412, 0, 0); //我的错题向上补位
                }

            }

            var imggroup = from c in PublicClass.avatar_list where c.avatar_type == "left" select c;
            imgheight = imggroup.Count() * 180;
            foreach (var img in imggroup)
            {
                Image image = new Image();
                image.Width = 720;
                image.Height = 180;
                image.Source = new BitmapImage(new Uri(System.Windows.Forms.Application.StartupPath + "\\avatar\\"+img.avatarurl));
                image.MouseUp += new MouseButtonEventHandler(image_MouseUp);
                img_panel.Children.Add(image);
            }
            img_panel.Margin = new Thickness(0, -imgheight + 180, 0, 0);

            timer.Interval = 10000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();


        }

        void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (img != null)
            {
                var link = from c in PublicClass.avatar_list where c.avatarurl.Contains(img.Source.ToString().Split('/').Last()) select c;
                Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = link.First().link;
                proc.Start();

            }
                     
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ThicknessAnimation panelani = new ThicknessAnimation();
                if (img_panel.Margin.Top == 0)
                {
                    panelani.To = new Thickness(0, -imgheight + 180, 0, 0);
                }
                else
                {
                    panelani.To = new Thickness(0, img_panel.Margin.Top + 180, 0, 0);
                }
                panelani.Duration = TimeSpan.FromSeconds(1);
                img_panel.BeginAnimation(StackPanel.MarginProperty, panelani);
            }));
        }

        //生成分类UI
        private void generation_classification()
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 chapter 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
            jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);

            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;

            foreach (var s in set)
            {
                if (s.functional_module != "")
                {
                    PublicClass.gongneng = s.functional_module.Split(',');//逗号截取字符串
                }
                else
                {
                    PublicClass.gongneng = new string[6] { "", "", "", "", "", "" };//初始化空数组
                }
                if (PublicClass.gongneng[0] == "1")//章节练习
                {
                    zhangjielianxi.Visibility = System.Windows.Visibility.Hidden;
                    simulation_test.Margin = new Thickness(31, 412, 0, 0); //仿真考试向上补位
                    my_mistakes.Margin = new Thickness(31, 458, 0, 0); //我的错题向上补位
                }
                else
                {
                    zhangjielianxi.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.gongneng[1] == "1")//顺序练习
                {
                    shunxulianxi.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    shunxulianxi.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.gongneng[2] == "1")//随机练习
                {
                    suijilianxi.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    suijilianxi.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.gongneng[3] == "1")//专项练习与专项模拟
                {
                    zhuanxianglianxi.Visibility = System.Windows.Visibility.Hidden;
                    zhuanxiangmoni.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    zhuanxianglianxi.Visibility = System.Windows.Visibility.Visible;
                    zhuanxiangmoni.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.gongneng[4] == "1")//仿真考试
                {
                    simulation_test.Visibility = System.Windows.Visibility.Hidden;
                    my_mistakes.Margin = new Thickness(31, 458, 0, 0); //我的错题向上补位
                }
                else
                {
                    simulation_test.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.gongneng[5] == "1")//我的错题
                {
                    my_mistakes.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    my_mistakes.Visibility = System.Windows.Visibility.Visible;
                }
                if (PublicClass.gongneng[0] == "1" && PublicClass.gongneng[4] == "1")//章节练习与我的错题
                {
                    zhangjielianxi.Visibility = System.Windows.Visibility.Hidden;
                    simulation_test.Visibility = System.Windows.Visibility.Hidden;
                    my_mistakes.Margin = new Thickness(31, 412, 0, 0); //我的错题向上补位
                }
            }
            yuyin.Visibility = System.Windows.Visibility.Visible;//语音课堂
            lianxi.Visibility = System.Windows.Visibility.Visible;//基础练习
            moni.Visibility = System.Windows.Visibility.Visible;//基础模拟
            qianghualianxi.Visibility = System.Windows.Visibility.Visible;//强化练习
            qianghuamoni.Visibility = System.Windows.Visibility.Visible;//强化模拟
            //zhuanxiangmoni.Visibility = System.Windows.Visibility.Visible;//专项模拟

            listBox.Items.Clear();
            listBox.Visibility = System.Windows.Visibility.Visible;

            shunxulianxi.IsEnabled = false;
            suijilianxi.IsEnabled = false;
        }

        //生成listbox
        private bool list_binding(string cartype, string subjection, string class_flag, string question_type)//依次为车型,科目,一级分类,二级分类
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 class 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.classTableAdapter jiakaoDataSetclassTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.classTableAdapter();
            jiakaoDataSetclassTableAdapter.Fill(jiakaoDataSet._class);
            // 将数据加载到表 classdetail 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.classdetailTableAdapter jiakaoDataSetclassdetailTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.classdetailTableAdapter();
            jiakaoDataSetclassdetailTableAdapter.Fill(jiakaoDataSet.classdetail);

            var c_count = from c in jiakaoDataSet._class where c.driverlicense_type.Contains(cartype) && c.subject.Contains(subjection) && c.class_flag.Contains(class_flag) && c.question_type.Contains(question_type) select c;

            listBox.Items.Clear();//清空集合

            if (c_count.Count() != 0)//有满足条件的分类
            {
                foreach (var cla in c_count)
                {
                    listBox.Items.Add(cla.class_name);//把分类名加入listbox
                    class_index.Add(cla.class_id);//同时把分类ID当做下标存入
                }

                listBox.SelectedIndex = 0;//初始化焦点
                return true;
            }
            else//无满足条件的分类
            {
                MessageBox.Show("此目录下分类为空!");
                return false;
            }
        }

        //章节练习绑定list
        private bool zhangjie_bangding(string cartype, string subjection)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
            jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);

            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);

            try
            {
                var sub = from c in jiakaoDataSet.subject where c.subject.Contains(subjection) select c;
                subject_id = sub.First().subject_id;
                subject_name = sub.First().subject;
                var question = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(cartype) && c.subject_id == sub.First().subject_id select c;

                List<int> chapter_list_id = new List<int>();
                foreach (var myquestion in question)
                {
                    if (chapter_list_id.Exists(c => c == myquestion.chapter_id) == false)
                    {
                        chapter_list_id.Add(myquestion.chapter_id);
                    }
                }


                listBox.Items.Clear();

                foreach (var mysub in chapter_list_id)
                {
                    var temsub = from c in jiakaoDataSet.chapter where c.chapter_id == mysub select c;
                    listBox.Items.Add(temsub.First().chapter);
                    chapter_index.Add(temsub.First().chapter_id);
                }

                listBox.SelectedIndex = 0;
                return true;
            }
            catch
            {
                MessageBox.Show("此目录下分类为空!");
                return false;
            }

        }


        //专项练习list绑定
        private bool list_zhuanxiang(string cartype, string subjection)
        {
            try
            {
                listBox.Items.Clear();
                //listBox.ItemsSource = null;
                //listBox.Items.Refresh();

                ArrayList item = LoadListBoxData();
                for (int i = 0; i < item.Count; i++)
                {
                    listBox.Items.Add(item[i]);
                }
                //listBox.ItemsSource = LoadListBoxData();

                listBox.SelectedIndex = 0;
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("题库为空!");
                return false;
            }
        }

        //专项练习list数据
        private ArrayList LoadListBoxData()
        {
            ArrayList itemsList = new ArrayList();
            itemsList.Add("选择题");
            zhuanxiang_index.Add("XZ");
            itemsList.Add("图片题");
            zhuanxiang_index.Add("TP");
            itemsList.Add("非图片题");
            zhuanxiang_index.Add("FTP");
            itemsList.Add("判断题");
            zhuanxiang_index.Add("PD");
            itemsList.Add("手势题");
            zhuanxiang_index.Add("SS");
            itemsList.Add("选对的题");
            zhuanxiang_index.Add("XD");
            itemsList.Add("选错的题");
            zhuanxiang_index.Add("XC");
            itemsList.Add("情景分析题");
            zhuanxiang_index.Add("QJ");
            itemsList.Add("标志题");
            zhuanxiang_index.Add("BZ");
            return itemsList;
        }

        //新手速成
        private void sucheng_Click(object sender, RoutedEventArgs e)
        {
            generation_classification();//生成分类UI
            classflag = "新手速成";//试题一级分类
        }

        //速成500
        private void sucheng500_Click(object sender, RoutedEventArgs e)
        {
            generation_classification();//生成分类UI
            classflag = "速成500";//试题一级分类
        }

        //速成600
        private void sucheng600_Click(object sender, RoutedEventArgs e)
        {
            generation_classification();//生成分类UI
            classflag = "速成600";//试题一级分类
        }


        //章节练习
        private void zhangjielianxi_Click(object sender, RoutedEventArgs e)
        {
            class_status = 3;//分类状态

            yuyin.Visibility = System.Windows.Visibility.Hidden;//语音课堂
            lianxi.Visibility = System.Windows.Visibility.Hidden;//基础练习
            moni.Visibility = System.Windows.Visibility.Hidden;//基础模拟
            qianghualianxi.Visibility = System.Windows.Visibility.Hidden;//强化练习
            qianghuamoni.Visibility = System.Windows.Visibility.Hidden;//强化模拟
            zhuanxianglianxi.Visibility = System.Windows.Visibility.Hidden;//专项练习
            zhuanxiangmoni.Visibility = System.Windows.Visibility.Hidden;//专项模拟
            shunxulianxi.Visibility = System.Windows.Visibility.Visible;//
            suijilianxi.Visibility = System.Windows.Visibility.Visible;//
            listBox.Items.Clear();
            if (zhangjie_bangding(PublicClass.cartype, PublicClass.subjection) == true)//列出章节,解锁顺序随机按钮
            {
                listBox.Visibility = System.Windows.Visibility.Visible;
                shunxulianxi.IsEnabled = true;
                suijilianxi.IsEnabled = true;
            }
        }

        //仿真考试
        private void simulation_test_Click(object sender, RoutedEventArgs e)
        {
            SimulationTest si = new SimulationTest();
            C1.WPF.C1Window c1si = new C1.WPF.C1Window();
            c1si.IsResizable = false;
            c1si.Margin = PublicClass.window_thickness(si);
            c1si.Content = si;
            c1si.Show();
            //c1si.ToolTip = "全真模拟";
            c1si.Name = "全真模拟";


        }


        //我的错题
        private void my_mistakes_Click(object sender, RoutedEventArgs e)
        {
            MyError my = new MyError();
            C1.WPF.C1Window cmy = new C1.WPF.C1Window();
            cmy.Name = "错题";
            cmy.Header = "我的错题";
            cmy.Margin = PublicClass.window_thickness(my);
            cmy.Content = my;
            cmy.Show();


        }



        //语音课堂
        private void yuyin_Click(object sender, RoutedEventArgs e)
        {
            classtype = "语音课堂";//试题二级分类
            class_status = 1;//分类状态
            shunxulianxi.Visibility = System.Windows.Visibility.Visible;
            suijilianxi.Content = "随机练习";
            if (list_binding(PublicClass.cartype, PublicClass.subjection, classflag, classtype) == true)//有题解锁顺序随机按钮
            {
                shunxulianxi.IsEnabled = true;
                suijilianxi.IsEnabled = true;
            }
            else
            {
                shunxulianxi.IsEnabled = false;
                suijilianxi.IsEnabled = false;
            }
        }
        //基础练习
        private void lianxi_Click(object sender, RoutedEventArgs e)
        {
            classtype = "基础练习";//试题二级分类
            class_status = 1;//分类状态
            shunxulianxi.Visibility = System.Windows.Visibility.Visible;
            suijilianxi.Content = "随机练习";
            if (list_binding(PublicClass.cartype, PublicClass.subjection, classflag, classtype) == true)//有题解锁顺序随机按钮
            {
                shunxulianxi.IsEnabled = true;
                suijilianxi.IsEnabled = true;
            }
            else
            {
                shunxulianxi.IsEnabled = false;
                suijilianxi.IsEnabled = false;
            }
        }
        //基础模拟
        private void moni_Click(object sender, RoutedEventArgs e)
        {
            classtype = "基础练习";//试题二级分类
            class_status = 4;//分类状态
            if (list_binding(PublicClass.cartype, PublicClass.subjection, classflag, classtype) == true)//有题解锁顺序随机按钮
            {
                shunxulianxi.Visibility = System.Windows.Visibility.Hidden;
                suijilianxi.IsEnabled = true;
                suijilianxi.Content = "开始考试";
            }
            else
            {
                shunxulianxi.IsEnabled = false;
                suijilianxi.IsEnabled = false;
            }
        }
        //强化练习
        private void qianghualianxi_Click(object sender, RoutedEventArgs e)
        {
            classtype = "强化练习";//试题二级分类
            class_status = 1;//分类状态
            shunxulianxi.Visibility = System.Windows.Visibility.Visible;
            suijilianxi.Content = "随机练习";
            if (list_binding(PublicClass.cartype, PublicClass.subjection, classflag, classtype) == true)//有题解锁顺序随机按钮
            {
                shunxulianxi.IsEnabled = true;
                suijilianxi.IsEnabled = true;
            }
            else
            {
                shunxulianxi.IsEnabled = false;
                suijilianxi.IsEnabled = false;
            }
        }
        //强化模拟
        private void qianghuamoni_Click(object sender, RoutedEventArgs e)
        {
            classtype = "强化练习";//试题二级分类
            class_status = 4;//分类状态
            if (list_binding(PublicClass.cartype, PublicClass.subjection, classflag, classtype) == true)//有题解锁顺序随机按钮
            {
                shunxulianxi.Visibility = System.Windows.Visibility.Hidden;
                suijilianxi.IsEnabled = true;
                suijilianxi.Content = "开始考试";
            }
            else
            {
                shunxulianxi.IsEnabled = false;
                suijilianxi.IsEnabled = false;
            }
        }

        //专项练习
        private void zhuanxianglianxi_Click(object sender, RoutedEventArgs e)
        {
            class_status = 2;//分类状态
            shunxulianxi.Visibility = System.Windows.Visibility.Visible;
            suijilianxi.Content = "随机练习";
            if (list_zhuanxiang(PublicClass.cartype, PublicClass.subjection) == true)//有题解锁顺序随机按钮
            {
                shunxulianxi.IsEnabled = true;
                suijilianxi.IsEnabled = true;
            }
            else
            {
                shunxulianxi.IsEnabled = false;
                suijilianxi.IsEnabled = false;
            }
        }

        //专项模拟
        private void zhuanxiangmoni_Click(object sender, RoutedEventArgs e)
        {
            class_status = 4;//分类状态
            if (list_binding(PublicClass.cartype, PublicClass.subjection, classflag, classtype) == true)//有题解锁顺序随机按钮
            {
                shunxulianxi.Visibility = System.Windows.Visibility.Hidden;
                suijilianxi.IsEnabled = true;
                suijilianxi.Content = "开始考试";
            }
            else
            {
                shunxulianxi.IsEnabled = false;
                suijilianxi.IsEnabled = false;
            }
        }

        //顺序练习
        private void shunxulianxi_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 class 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
            // 将数据加载到表 classdetail 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.classdetailTableAdapter jiakaoDataSetclassdetailTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.classdetailTableAdapter();
            jiakaoDataSetclassdetailTableAdapter.Fill(jiakaoDataSet.classdetail);

            if (class_status == 1)//常规
            {
                var questions = from c in jiakaoDataSet.classdetail where c.class_id == class_index[listBox.SelectedIndex] select c;
                questions_id.Clear();
                if (questions.Count() != 0)
                {
                    foreach (var qu in questions)
                    {
                        questions_id.Add(qu.question_id);
                    }
                }
                else
                {
                    MessageBox.Show("此类型题目数量为0!");
                    goto L1;

                }
            }
            if (class_status == 2)//专项
            {
                var questions = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(PublicClass.cartype) && c.subject_id == subject_id && c.question_type.Contains(zhuanxiang_index[listBox.SelectedIndex]) select c;
                questions_id.Clear();
                if (questions.Count() != 0)
                {
                    foreach (var qu in questions)
                    {
                        questions_id.Add(qu.question_id);
                    }
                }
                else
                {
                    MessageBox.Show("此类型题目数量为0!");
                    goto L1;

                }

            }
            if (class_status == 3)//章节
            {
                var questions = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(PublicClass.cartype) && c.subject_id == subject_id && c.chapter_id == chapter_index[listBox.SelectedIndex] select c;
                questions_id.Clear();
                foreach (var qu in questions)
                {
                    questions_id.Add(qu.question_id);
                }
            }


            MainExam ma = new MainExam();
            C1.WPF.C1Window cwin = new C1.WPF.C1Window();
            ma.create_question(0, 0, PublicClass.cartype, subject_name, questions_id);
            cwin.Content = ma;
            cwin.Name = "驾考";
            cwin.Header = "驾驶理论考试系统";
            cwin.Show();
            cwin.WindowState = C1.WPF.C1WindowState.Maximized;
            cwin.Closing += new System.ComponentModel.CancelEventHandler(cwin_Closing);

        L1://goto跳转至这里
            int l = 0;//占位无用变量,不可注释

        }


        //随机练习
        private void suijilianxi_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 class 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
            // 将数据加载到表 classdetail 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.classdetailTableAdapter jiakaoDataSetclassdetailTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.classdetailTableAdapter();
            jiakaoDataSetclassdetailTableAdapter.Fill(jiakaoDataSet.classdetail);

            if (class_status == 1)//常规
            {
                var questions = from c in jiakaoDataSet.classdetail where c.class_id == class_index[listBox.SelectedIndex] select c;
                questions_id.Clear();
                if (questions.Count() != 0)
                {
                    foreach (var qu in questions)
                    {
                        questions_id.Add(qu.question_id);
                    }
                }
                else
                {
                    MessageBox.Show("此类型题目数量为0!");
                    goto L1;
                }
            }
            if (class_status == 2)//专项
            {
                var questions = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(PublicClass.cartype) && c.subject_id == subject_id && c.question_type.Contains(zhuanxiang_index[listBox.SelectedIndex]) select c;
                questions_id.Clear();
                if (questions.Count() != 0)
                {
                    foreach (var qu in questions)
                    {
                        questions_id.Add(qu.question_id);
                    }
                }
                else
                {
                    MessageBox.Show("此类型题目数量为0!");
                    goto L1;
                }
            }
            if (class_status == 3)//章节
            {
                var questions = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(PublicClass.cartype) && c.subject_id == subject_id && c.chapter_id == chapter_index[listBox.SelectedIndex] select c;
                questions_id.Clear();
                foreach (var qu in questions)
                {
                    questions_id.Add(qu.question_id);
                }

            }
            if (class_status == 4)//常规模拟
            {
                var questions = from c in jiakaoDataSet.classdetail where c.class_id == class_index[listBox.SelectedIndex] select c;
                questions_id.Clear();
                if (questions.Count() != 0)
                {
                    foreach (var qu in questions)
                    {
                        questions_id.Add(qu.question_id);
                        if (questions_id.Count > 99)
                        {
                            goto L2;
                        }
                    }

                    if (questions_id.Count > 99)
                    {
                        //List<int> questions_c = new List<int>();
                        //Random r = new Random();
                        //var r4 = questions_id.OrderBy(x => r.Next()).Take(100);

                        //Hashtable hashtable = new Hashtable();
                        //Random rm = new Random();
                        //int RmNum = questions_id.Count;
                        //for (int i = 0; hashtable.Count < RmNum; i++)
                        //{
                        //    int nValue = rm.Next(100);
                        //    if (!hashtable.ContainsValue(nValue) && nValue != 0)
                        //    {
                        //        hashtable.Add(nValue, nValue);
                        //        Console.WriteLine(nValue.ToString());
                        //    }
                        //}
                    }
                }
                else
                {
                    MessageBox.Show("此类型题目数量为0!");
                    goto L1;
                }
            }

        L2://goto跳转至这里

            MainExam ma = new MainExam();
            C1.WPF.C1Window cwin = new C1.WPF.C1Window();
            ma.create_question(1, 0, PublicClass.cartype, subject_name, questions_id);
            cwin.Content = ma;
            cwin.Name = "驾考";
            cwin.Header = "驾驶理论考试系统";
            cwin.Show();
            cwin.WindowState = C1.WPF.C1WindowState.Maximized;
            cwin.Closing += new System.ComponentModel.CancelEventHandler(cwin_Closing);

        L1://goto跳转至这里
            int l = 0;//占位无用变量,不可注释
        }




        //关闭时上传错题
        void cwin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("确定退出练习吗？", "询问", MessageBoxButton.OKCancel);

            //关闭窗口
            if (result == MessageBoxResult.OK)
            {
                e.Cancel = false;

                DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
                DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
                jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);

                var errquestion = (from c in jiakaoDataSet.errquest where c.user_id == PublicClass.user_id && c.user_id > 0 && c.user_id == PublicClass.user_id select c).ToArray();
                PublicClass.question_list = new List<PublicClass.Question>();
                try
                {
                    ServicePointManager.DefaultConnectionLimit = 1000;
                    HttpWebResponse response = null;

                    string url = PublicClass.http + @"/returnjsons/t_errquests?" + "command=clear&user_id=" + PublicClass.user_id;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//提交请求
                    request.Method = "GET";

                    request.Timeout = 10000;
                    response = (HttpWebResponse)request.GetResponse();
                    response.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    ServicePointManager.DefaultConnectionLimit = 1000;
                    HttpWebResponse response = null;
                    //StreamReader reader = null;

                    int arr_count = errquestion.Count() / 60;
                    arr_count++;

                    for (int cou = 0; cou < arr_count; cou++)
                    {
                        string url = PublicClass.http + @"/returnjsons/t_errquests?user_id=" + PublicClass.user_id + "&";
                        for (int i = cou * 60; i < (cou + 1) * 60; i++)
                        {
                            if (i < errquestion.Count())
                            {
                                url += "q[]=" + errquestion[i].question_id + "&a[]=" + errquestion[i].amount + "&";
                            }
                        }
                        url = url.Substring(0, url.Length - 1);
                        int a = url.Length;
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//
                        request.Method = "GET";

                        request.Timeout = 10000;
                        response = (HttpWebResponse)request.GetResponse();
                        response.Close();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            //不关闭窗口
            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;
        }




    }



    //public class TypeItem//
    //{
    //   public int Id { get; set; }
    //   public string Name { get; set; }
    //   public string type { get; set; }
    //}

    //public class TypeItemArr : ObservableCollection<TypeItem>//
    //{
    //    public TypeItemArr()
    //    {
    //        this.Add(new TypeItem { Id = 0, Name = "选择题", type = "XZ" });
    //        this.Add(new TypeItem { Id = 1, Name = "图片题", type = "TP" });
    //        this.Add(new TypeItem { Id = 2, Name = "非图片题", type = "FTP" });
    //        this.Add(new TypeItem { Id = 3, Name = "判断题", type = "PD" });
    //        this.Add(new TypeItem { Id = 4, Name = "手势题", type = "SS" });
    //        this.Add(new TypeItem { Id = 5, Name = "选对的题", type = "XD" });
    //        this.Add(new TypeItem { Id = 6, Name = "选错的题", type = "XC" });
    //        this.Add(new TypeItem { Id = 7, Name = "情景分析题", type = "QJ" });
    //        this.Add(new TypeItem { Id = 8, Name = "标志题", type = "BZ" });

    //    }
    //}





    //public class Processes : List<string>
    //{
    //    public Processes()
    //    {
    //        //在构造函数中取得系统中进程的名称并将其添加到类中
    //        System.Diagnostics.Process[] pList = System.Diagnostics.Process.GetProcesses();
    //        foreach (System.Diagnostics.Process p in pList)
    //        {
    //            this.Add(p.ProcessName);
    //        }
    //    }
    //}
}
