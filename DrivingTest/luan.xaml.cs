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
    /// luan.xaml 的交互逻辑
    /// </summary>
    public partial class luan : UserControl
    {
        public luan()
        {
            InitializeComponent();
        }

        private void mypolygon_Loaded(object sender, RoutedEventArgs e)
        {
            //maincanvas.Width = 130;
            //maincanvas.Height = 130;
            //Path path = new Path();
            //Polygon mypol = new Polygon();
            //Point p1 = new Point();
            //Point p2 = new Point();
            //Point p3 = new Point();
            //p3.X = 72;
            //p3.Y = 0;
            //p2.X = 64;
            //p2.Y = 0;
            //p1.X = 64;
            //p1.Y = 64;
            //mypol.Points.Add(p1);
            //mypol.Points.Add(p2);
            //mypol.Points.Add(p3);
            //mypol.Fill = new SolidColorBrush(Colors.Blue);
            //mypol.StrokeThickness = 1;
            //mypol.Stroke = new SolidColorBrush(Colors.Blue);
            //mypol.Margin = new Thickness(64, 64, 0, 0);
            //maincanvas.Children.Add(mypol);
            //Label ne = new Label();
            //ne.Content = "asdfasdf";
            //maincanvas.Children.Add(ne);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 360; i=i+9)
            {
                Polygon mypol = new Polygon();
                Point p1 = new Point();
                Point p2 = new Point();
                Point p3 = new Point();
                p3.X = 72;
                p3.Y = 0;
                p2.X = 64;
                p2.Y = 0;
                p1.X = 64;
                p1.Y = 64;
                mypol.Points.Add(p1);
                mypol.Points.Add(p2);
                mypol.Points.Add(p3);
                mypol.Fill = new SolidColorBrush(Colors.Blue);
                mypol.StrokeThickness = 1;
                mypol.Stroke = new SolidColorBrush(Colors.White);
                Point ori = new Point();
                ori.X = 0.89;
                ori.Y = 0.96;
                mypol.RenderTransformOrigin = ori;
                RotateTransform rot = new RotateTransform();
                rot.Angle = i;
                mypol.RenderTransform = rot;
                maincanvas.Children.Add(mypol);

                Ellipse ell = new Ellipse();
                ell.Width = 128;
                ell.Height = 128;
                ell.Fill = new SolidColorBrush(Colors.Yellow);
                ell.Margin = new Thickness(1, 2, 0, 0);
                maincanvas.Children.Add(ell);

            }
           
        }
    }
}
