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
            if (int.Parse(xueyuanMark) >= 80 && int.Parse(xueyuanMark) != 100)
            {
                chengji_textBlock.Text = "恭喜你,本次成绩为:";
                hege_textBlock.Text = "成绩合格";
            }
            else if (int.Parse(xueyuanMark) == 100)
            {
                chengji_textBlock.Text = "恭喜你,本次成绩为:";
                hege_textBlock.Text = "成绩合格";
                error_button.IsEnabled = false;
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
            int step = 0;
            for (int i = 0; i < PublicClass.question_list.Count(); i++)
            {
                PublicClass.question_list[i].shownum = i + 1;
                if (PublicClass.question_list[i].check_answer == true && PublicClass.question_list[i].select_answer != "")
                {
                    PublicClass.question_list.RemoveAt(i);
                    i--;
                }
                else if (PublicClass.question_list[i].check_answer == false)
                {
                    PublicClass.Question question = new PublicClass.Question();
                    question = PublicClass.question_list[i];
                    PublicClass.question_list.RemoveAt(i);
                    PublicClass.question_list.Insert(step, question);
                    step++;
                }


            }

            C1.WPF.C1Window me = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "驾考");
            if (me != null)
            {
                MainExam ma = me.Content as MainExam;
                ma.jiaojuan_button.Visibility = System.Windows.Visibility.Hidden;
                ma.chongkao_button.Visibility = System.Windows.Visibility.Visible;
                ma.zongfen_TextBlock.Visibility = System.Windows.Visibility.Visible;
                ma.zongfen_TextBlock.Text = "得分:" + PublicClass.fenshu.ToString() + "分";
                if (PublicClass.question_mode == 0)//练习下显示重考错题
                {
                    ma.chongkao_button.Content = "重考错题";
                }
                ma.show_err_question(int.Parse(xueyuanMark));
            }


            C1.WPF.C1Window end = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "end");
            if (end != null)
            {
                end.Close();
            }
        }

        //退出
        private void end_button_Click(object sender, RoutedEventArgs e)
        {
            C1.WPF.C1Window me = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "驾考");
            if (me != null)
            {
                MainExam ma = me.Content as MainExam;
                ma.jiaojuan_button.Visibility = System.Windows.Visibility.Hidden;
                ma.chongkao_button.Visibility = System.Windows.Visibility.Visible;
                ma.zongfen_TextBlock.Visibility = System.Windows.Visibility.Visible;
                ma.zongfen_TextBlock.Text = "得分:" + PublicClass.fenshu.ToString() + "分";
                if (PublicClass.question_mode == 0)//练习下显示重考错题
                {
                    ma.chongkao_button.Content = "重考错题";
                }
                ma.show_err_question(int.Parse(xueyuanMark));
            }
            C1.WPF.C1Window end = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "end");
            if (end != null)
            {
                end.Close();
            }
        }

    }
}
