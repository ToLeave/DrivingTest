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
    /// SubjectTwo.xaml 的交互逻辑
    /// </summary>
    public partial class SubjectTwo : UserControl
    {
        public SubjectTwo()
        {
            InitializeComponent();
        }


        //倒车入库
        private void daoche_image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Reversing re = new Reversing();
            this.Content = re;
        }

        //坡道定点停车起步
        private void podao_image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ramp ra = new Ramp();
            this.Content = ra;
        }

        //侧方位停车
        private void cefang_image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Lateral la = new Lateral();
            this.Content = la;
        }

        //直角转弯
        private void zhijiao_image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightAngle ri = new RightAngle();
            this.Content = ri;
        }

        //曲线行驶
        private void quxian_image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Curve cu = new Curve();
            this.Content = cu;
        }

        //注意事项
        private void zhuyi_textBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mind mi = new Mind();
            this.Content = mi;
        }

        //判定标准
        private void panding_textBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Determine de = new Determine();
            this.Content = de;
        }

        //注意事项鼠标效果
        private void zhuyi_textBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            zhuyi_textBlock.TextDecorations = TextDecorations.Underline;
        }
        private void zhuyi_textBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            zhuyi_textBlock.TextDecorations = null;
        }

        //判定标准鼠标效果
        private void panding_textBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            panding_textBlock.TextDecorations = TextDecorations.Underline;
        }
        private void panding_textBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            panding_textBlock.TextDecorations = null;
        }








    }
}
