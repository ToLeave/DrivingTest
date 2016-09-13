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
    public partial class MainScreen : Window
    {
        public MainScreen()
        {
            InitializeComponent();
            this.Closing += F;
        }

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
            foreach (ListBoxItem lbi in listBox.SelectedItems)
            {
                if (lbi != null)
                {
                    string str = lbi.Content.ToString();
                    MessageBox.Show(str, "");
                }
            } 
        }
        //随机练习
        private void suijilianxi_Click(object sender, RoutedEventArgs e)
        {

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
            si.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 chapter 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter jiakaoDataSetchapterTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.chapterTableAdapter();
            jiakaoDataSetchapterTableAdapter.Fill(jiakaoDataSet.chapter);
            System.Windows.Data.CollectionViewSource chapterViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("chapterViewSource")));
            chapterViewSource.View.MoveCurrentToFirst();

            
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
