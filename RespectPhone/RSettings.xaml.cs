using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RespectPhone
{
    /// <summary>
    /// Логика взаимодействия для RSettings.xaml
    /// </summary>
    public partial class RSettings : Window
    {
        public RSettings()
        {
            InitializeComponent();
            UseExtConf.IsChecked = RespSIPAccount.INS.UseConfExtension;
            ats_log.Text = RespSIPAccount.INS.authenticationId;
            ats_pass.Text = RespSIPAccount.INS.registerPassword;
            ats_host.Text = RespSIPAccount.INS.domainHost;
            e_log.Text = RespSIPAccount.INS.rlogin;
            e_pass.Text = RespSIPAccount.INS.rpass;
            Vers.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void UseExtConf_Checked(object sender, RoutedEventArgs e)
        {
            RespSIPAccount.INS.UseConfExtension = true;
        }

        private void UseExtConf_Unchecked(object sender, RoutedEventArgs e)
        {
            RespSIPAccount.INS.UseConfExtension = false;
        }

        private void ats_log_TextChanged(object sender, TextChangedEventArgs e)
        {
            RespSIPAccount.INS.authenticationId = ats_log.Text.Trim();
            RespSIPAccount.INS.userName = ats_log.Text.Trim();
        }

        private void ats_pass_TextChanged(object sender, TextChangedEventArgs e)
        {
            RespSIPAccount.INS.registerPassword = ats_pass.Text.Trim();
        }

        private void ats_host_TextChanged(object sender, TextChangedEventArgs e)
        {
            RespSIPAccount.INS.domainHost = ats_host.Text.Trim();
        }

        private void e_log_TextChanged(object sender, TextChangedEventArgs e)
        {
            RespSIPAccount.INS.rlogin = e_log.Text.Trim();
        }

        private void e_pass_TextChanged(object sender, TextChangedEventArgs e)
        {
            RespSIPAccount.INS.rpass= e_pass.Text.Trim();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
