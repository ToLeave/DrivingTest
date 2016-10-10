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
    /// MainScreen.xaml 的交互逻辑
    /// </summary>
    public partial class MainScreen : UserControl
    {
        public MainScreen()
        {
            InitializeComponent();
            //this.Closing += F;
        }

        public string cartype = "";
        public string subjection = "";
        List<int> chapter_index = new List<int>();
        int subject_id;
        string subject_name;
        List<int> questions_id = new List<int>();

        private void F(object o, System.ComponentModel.CancelEventArgs e)
        {
            Window main = Application.Current.MainWindow;
            main.Visibility = System.Windows.Visibility.Visible;
            //main.WindowState = System.Windows.WindowState.Normal;
            

        }

        //新手速成
        private void sucheng_Click(object sender, RoutedEventArgs e)
        {

        }
        //速成600
        private void sucheng600_Click(object sender, RoutedEventArgs e)
        {

        }
        //速成500
        private void sucheng500_Click(object sender, RoutedEventArgs e)
        {

        }
        //语音课堂
        private void yuyin_Click(object sender, RoutedEventArgs e)
        {

        }
        //基础练习
        private void lianxi_Click(object sender, RoutedEventArgs e)
        {

        }
        //基础模拟
        private void moni_Click(object sender, RoutedEventArgs e)
        {

        }
        //强化练习
        private void qianghualianxi_Click(object sender, RoutedEventArgs e)
        {

        }
        //强化模拟
        private void qianghuamoni_Click(object sender, RoutedEventArgs e)
        {

        }
        //我的错题
        private void my_mistakes_Click(object sender, RoutedEventArgs e)
        {


        }
        //顺序练习
        private void shunxulianxi_Click(object sender, RoutedEventArgs e)
        {

            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));

            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
            var questions = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(cartype) && c.subject_id == subject_id && c.chapter_id == chapter_index[listBox.SelectedIndex] select c;
            questions_id.Clear();
            foreach (var qu in questions)
            {
                questions_id.Add(qu.question_id);
            }
            MainExam ma = new MainExam();


            C1.WPF.C1Window cwin = new C1.WPF.C1Window();
            ma.create_question(0, 0, cartype, subject_name, questions_id);
            cwin.Content = ma;
            cwin.Name = "驾考";
            cwin.Header = "驾驶理论考试系统";
            cwin.Show();
            //this.Content = ma;
            cwin.WindowState = C1.WPF.C1WindowState.Maximized;
        }
        //随机练习
        private void suijilianxi_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));

            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
            var questions = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(cartype) && c.subject_id == subject_id && c.chapter_id == chapter_index[listBox.SelectedIndex] select c;
            questions_id.Clear();
            foreach (var qu in questions)
            {
                questions_id.Add(qu.question_id);
            }
            MainExam ma = new MainExam();
            C1.WPF.C1Window cwin = new C1.WPF.C1Window();
            ma.create_question(1, 0, cartype, subject_name, questions_id);
            cwin.Content = ma;
            cwin.Name = "驾考";
            cwin.Header = "驾驶理论考试系统";
            cwin.Show();
            //this.Content = ma;
            cwin.WindowState = C1.WPF.C1WindowState.Maximized;
        }
        //专项练习
        private void zhuanxianglianxi_Click(object sender, RoutedEventArgs e)
        {

        }
        //专项模拟
        private void zhuanxiangmoni_Click(object sender, RoutedEventArgs e)
        {

        }
        //章节练习
        private void zhangjielianxi_Click(object sender, RoutedEventArgs e)
        {

        }
        //仿真考试
        private void simulation_test_Click(object sender, RoutedEventArgs e)
        {
            SimulationTest si = new SimulationTest();
            C1.WPF.C1Window c1si = new C1.WPF.C1Window();
            c1si.IsResizable = false;
            c1si.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - si.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - si.Height / 2, 0, 0);
            c1si.Content = si;
            c1si.Show();
            c1si.ToolTip = "全真模拟";
            c1si.Name = "全真模拟";
          
            //C1.WPF.C1Window ma = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "科目一");
            //if (ma != null)
            //{
            //    ma.Close();
            //}
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 chapter 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
            jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);
            System.Windows.Data.CollectionViewSource chapterViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("chapterViewSource")));
            chapterViewSource.View.MoveCurrentToFirst();
           
            list_bangding(cartype, subjection);
        }


        private void list_bangding(string cartype, string subjection)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
            jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);

            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);

            var sub = from c in jiakaoDataSet.subject where c.subject.Contains(subjection) select c;
            subject_id = sub.First().subject_id;
            subject_name = sub.First().subject;
            var question = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(cartype) && c.subject_id == sub.First().subject_id select c;

            List<int> chapter_list_id = new List<int>();
            foreach (var myquestion in question)
            {
                if (chapter_list_id.Exists(c=>c==myquestion.chapter_id)==false)
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


        }

        
    }

    public class Processes : List<string>
    {
        public Processes()
        {
            //在构造函数中取得系统中进程的名称并将其添加到类中
            System.Diagnostics.Process[] pList = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process p in pList)
            {
                this.Add(p.ProcessName);
            }
        }
    }
}
