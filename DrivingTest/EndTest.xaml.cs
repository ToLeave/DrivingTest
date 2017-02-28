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
    /// EndTest.xaml 的交互逻辑
    /// </summary>
    public partial class EndTest : UserControl
    {
        string xueyuanName = "";//学员姓名
        string xueyuanMark = "";//学员成绩

        public EndTest(string xueyuanName, string xueyuanMark)
        {
            InitializeComponent();

            this.xueyuanName = xueyuanName;
            this.xueyuanMark = xueyuanMark;
        }

        //初始界面
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MainExam ma = new MainExam();
            name_textBlock.Text = xueyuanName;
            fenshu_textBlock.Text = xueyuanMark;
            if (int.Parse(xueyuanMark) >= 80)
            {
                chengji_textBlock.Text = "恭喜你,本次成绩为:";
                hege_textBlock.Text = "成绩合格";
            }
            else
            {
                chengji_textBlock.Text = "很遗憾,本次成绩为:";
                hege_textBlock.Text = "成绩不合格";
            }
            
        }

        //查看错题
        private void error_button_Click(object sender, RoutedEventArgs e)
        {
            C1.WPF.C1Window end = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "end");
            if (end != null)
            {
                end.Close();
            }
        }

        //退出
        private void end_button_Click(object sender, RoutedEventArgs e)
        {
            C1.WPF.C1Window end = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "end");
            if (end != null)
            {
                end.Close();
            }
        }

    }
}
