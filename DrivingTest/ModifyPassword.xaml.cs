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
using System.IO;
using Newtonsoft.Json.Linq;

namespace DrivingTest
{
    /// <summary>
    /// ModifyPassword.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyPassword : Window
    {
        public ModifyPassword()
        {
            InitializeComponent();
        }

        //确认修改密码
        private void xiugai_button_Click(object sender, RoutedEventArgs e)
        {
            

            if (newpassword_textBox.Password == confirmpassword_textBox.Password)
            {
                string password = null;
                HttpWebResponse response = null;
                StreamReader reader = null;

                string old_password = password_textBox.Password;
                string new_password = newpassword_textBox.Password;

                old_password = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(old_password, "MD5");//加密旧密码

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/changepassword?login="+ PublicClass.login + "&oldpassword="+ old_password + "&newpassword=" + new_password);//
                    request.Method = "GET";
                    request.Timeout = 10000;
                    response = (HttpWebResponse)request.GetResponse();
                    reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                    password = reader.ReadToEnd();

                    JArray loginUUID_json = JArray.Parse(password);//修改密码确认回执 json

                    string status = loginUUID_json[0]["status"].ToString();

                    MessageBox.Show(status, "提示");
                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                MessageBox.Show("两次输入的新密码不一致!", "提示");
            }
            

        }
    }
}
