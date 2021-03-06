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
        bool loaded = false;
        public RSettings()
        {
            InitializeComponent();
            UseExtConf.IsChecked = RespSIPAccount.INS.UseConfExtension;
            ats_log.Text = RespSIPAccount.INS.authenticationId;
            ats_pass.Password = RespSIPAccount.INS.registerPassword;
            ats_host.Text = RespSIPAccount.INS.domainHost;
            e_log.Text = RespSIPAccount.INS.rlogin;
            e_pass.Password = RespSIPAccount.INS.rpass;
            Vers.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            loaded = true;
        }

        private void UseExtConf_Checked(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;
            RespSIPAccount.INS.UseConfExtension = true;
        }

        private void UseExtConf_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;

            RespSIPAccount.INS.UseConfExtension = false;
        }

        private void ats_log_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!loaded) return;
            RespSIPAccount.INS.authenticationId = ats_log.Text.Trim();
            RespSIPAccount.INS.userName = ats_log.Text.Trim();
        }

        private void ats_pass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!loaded) return;
            RespSIPAccount.INS.SetExtPass(ats_pass.Password.Trim());
        }
        private void ats_pass_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;
            RespSIPAccount.INS.SetExtPass(ats_pass.Password.Trim());

        }
        private void ats_host_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!loaded) return;
            RespSIPAccount.INS.domainHost = ats_host.Text.Trim();
        }

        private void e_log_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!loaded) return;
            RespSIPAccount.INS.rlogin = e_log.Text.Trim();
        }

        private void e_pass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!loaded) return;
            RespSIPAccount.INS.SetPass(e_pass.Password.Trim());
        }
        private void e_pass_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;
            RespSIPAccount.INS.SetPass(e_pass.Password.Trim());
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RespSIPAccount.INS.SaveToFile();
            this.Close();
        }

        
    }
}
