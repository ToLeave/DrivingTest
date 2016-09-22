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
    /// QuestionNum.xaml 的交互逻辑
    /// </summary>
    public partial class QuestionNum : UserControl
    {
        public QuestionNum()
        {
            InitializeComponent();
        }
        public void setnum(int quesnum, bool isright,string answer)
        {
            label1.Content = quesnum;
            if (isright)
            {
                label2.Foreground = Brushes.Red;
            }
            label2.Content = answer;
            canvas1.Background = Brushes.White;
        }

        public void setbackcolor()
        {
            canvas1.Background = Brushes.SkyBlue;
        }
    }
}
