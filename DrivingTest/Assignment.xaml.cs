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

namespace DrivingTest
{
    /// <summary>
    /// Assignment.xaml 的交互逻辑
    /// </summary>
    public partial class Assignment : UserControl
    {
        public Assignment()
        {
            InitializeComponent();
        }

        //继续考试
        private void jixu_button_Click(object sender, RoutedEventArgs e)
        {
            C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "交卷");
            if (cp != null)
            {
                cp.Close();
            }
        }

        //确认交卷
        public void queren_button_Click(object sender, RoutedEventArgs e)
        {
            diyiye_grid.Visibility = System.Windows.Visibility.Hidden;
            dierye_grid.Visibility = System.Windows.Visibility.Visible;

            xueyuan_textBlock.Text = "";

            if (PublicClass.fenshu >= 90)
            {
                wenzi_textBlock.Text = "恭喜你,你本次成绩得"+ PublicClass.fenshu + "分,考试合格!";
            }
            else
            {
                wenzi_textBlock.Text = "很遗憾,你本次考试得" + PublicClass.fenshu + "分,不及格!祝你下次考试成功!";
            }
            
        }

        //退出
        private void guanbi_button_Click(object sender, RoutedEventArgs e)
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
                
                //me.Content = ma;

            }

            C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "交卷");
            if (cp != null)
            {
                cp.Close();
            }
        }
    }
}
