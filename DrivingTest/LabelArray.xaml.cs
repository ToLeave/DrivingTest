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
    /// LabelArray.xaml 的交互逻辑
    /// </summary>
    public partial class LabelArray : UserControl
    {
        public LabelArray()
        {
            InitializeComponent();
            List<Label> bs = new List<Label>();
            //for (int i = 1; i < 11; i++)
            //{
            //    Label la = new Label();
            //    la.Name = "Label" + i;
            //    la.Content = i;
            //    la.Width = 25;
            //    la.Height = 30;
            //    la.Margin = new Thickness { Bottom = 1, Left = 1, Right = 1, Top = 1 };
            //    la.MouseDown += new MouseButtonEventHandler(OK);
            //    if (i % 1 < 1)
            //    {
            //        bs.Add(la);
            //        if (i % 1 < 1)
            //        {
            //            StackPanel s = new StackPanel();
            //            foreach (Label lb in bs)
            //            {
            //                s.Children.Add(lb);
            //            }
            //            bs.Clear();
            //            sp.Children.Add(s);
            //        }
            //    }
            //}

            //StackPanel s = new StackPanel();

            int question_count = 998;
            if ((int)(question_count / 10) > 10)
            {
                sp.Height = (int)(question_count / 10) * 31;
            }

            for (int i = 0; i <= (int)(question_count / 10); i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (i * 10 + j < question_count)
                    {
                        Label la = new Label();
                        la.Name = "L" + i * 10 + j + 1;
                        la.Width = 25;
                        la.Height = 30;
                        la.Content = i * 10 + j + 1;
                        la.Margin = new Thickness(j * 26, i * 31, 0, 0);
                        la.Padding = new Thickness(2);
                        la.MouseDown += new MouseButtonEventHandler(OK);
                        sp.Children.Add(la);

                    }
                }

            
            }
            //sp.Children.Add(s);


        }
        public void OK(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(((Label)sender).Content.ToString());
            foreach (var i in sp.Children)
            {
                Label mylab = i as Label;
                if (mylab != null)
                {
                    mylab.Background = Brushes.White;
                }
            }
            Label l = sender as Label;
            l.Background = Brushes.SkyBlue;

              
        }

        //private void label1_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    label1.Background = Brushes.SkyBlue;
        //}
    }
}
