using Ozeki.VoIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RespectPhone
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SoftPhone Phone;
        bool transferOn = false;
        int seconds = 0;
        System.Timers.Timer timer = new System.Timers.Timer();
        bool auto_answer = false;
        System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        public MainWindow()
        {
            InitializeComponent();
            RespSIPAccount.ReadConf();
            Phone = new SoftPhone(RespSIPAccount.INS, "");
            Phone.IncomingCallReceived += IncomingCall;
            Phone.RegisterStateChanged += RegisterStateChanged;
            GlobalEvent.INS.RiseAction += RiseAction;
        
            timer.Interval = 1000;
            timer.Elapsed += TimerTick;
            timer.Enabled = true;
            timer.Stop();
            InitNotIcon();
        }
        public void InitNotIcon()
        {
            nIcon.Icon = RespectPhone.Properties.Resources.phico;
            nIcon.Visible = true;
            nIcon.Click += nIcon_Click;

        }
        private void nIcon_Click(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
            if (this.WindowState != WindowState.Minimized)
                this.WindowState = WindowState.Minimized;
            else
                this.WindowState = WindowState.Normal;
        }
        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            seconds += 1;
            double z = seconds / 60;
            double m = Math.Round(z);
            var s = seconds - m * 60;
            Dispatcher.BeginInvoke((Action)(() => { 
            Time.Content = (m > 9 ? m.ToString("F0") : "0" + m.ToString("F0")) + ":" + (s > 9 ? s.ToString() : "0" + s.ToString());
            }));
        }

        private void StartTimer()
        {
            seconds = 0;
            timer.Start();
        }

        private void StopTimer()
        {
            timer.Stop();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Time.Content = "";
            }));
        }


        private void RiseAction(object sender, Events ev, object data)
        {
            switch (ev)
            {
                case Events.AnswerIncoming:
                    Phone.AnswerIncoming();
                    break;
                case Events.RejectIncoming:
                    Phone.AnswerIncoming(true);
                    break;
                case Events.CallStateChanged:
                    CallStateChanged(data);
                    break;
            }
        }

        private void CallStateChanged(object data)
        {
            if (data is CallState)
            {
                var st = (CallState)data;
                switch (st)
                {
                    case CallState.Cancelled:
                    case CallState.Busy:
                    case CallState.Completed:
                        StopTimer();
                        break;
                    case CallState.InCall:
                        StartTimer();

                        break;
                }
            }
        }


        private void RegisterStateChanged(object sender, VoIPEventArgs<RegistrationStateChangedArgs> e)
        {
            if (e.Item.State == RegState.RegistrationSucceeded)
                ChangeStatusLabel(true);
            else
            {
                ChangeStatusLabel(false);
            }
        }

        private void IncomingCall(object sender, VoIPEventArgs<IPhoneCall> e)
        {
            Console.WriteLine("call " + e.Item.CallID);
            Dispatcher.BeginInvoke((Action)(() => {
                if (this.WindowState == WindowState.Minimized)
                    this.WindowState = WindowState.Normal;
                if (auto_answer) {
                    e.Item.Answer();
                }
                else
                {
                    IncomingCall ic = new IncomingCall(e.Item);
                    Num.Text = e.Item.DialInfo.CallerID;
                    ic.Show();
                    ic.Topmost = true;
                }
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public void ChangeStatusLabel(bool stat)
        {
            Dispatcher.BeginInvoke((Action)(()=>{
                if (stat)
                {
                    ImgStat1.Visibility = Visibility.Visible;
                    ImgStat0.Visibility = Visibility.Collapsed;
                    StatLabel.Content = "Подключен "+ RespSIPAccount.INS.displayName+"("+ RespSIPAccount.INS.authenticationId+ ")";
                }
                else
                {

                    ImgStat0.Visibility = Visibility.Visible;
                    ImgStat1.Visibility = Visibility.Collapsed;
                    StatLabel.Content = "Отключен";
                }
            
            }));
        }

        public void SetNumsInText()
        {
            var st = Num.Text != null ? Num.Text : "";
            int[] intMatch = st.Where(Char.IsDigit).Select(x => int.Parse(x.ToString())).ToArray();
            Num.Text =string.Join("", intMatch);
        }
        private void Num_Button_Click(object sender, RoutedEventArgs e)
        {
            string act = "";
            if (sender is Button)
                act = ((Button)sender).Tag.ToString();
            switch (act)
            {
                case "1":                    
                case "2":                    
                case "3":                    
                case "4":                    
                case "5":                    
                case "6":                    
                case "7":                   
                case "8":                   
                case "9":
                case "0":
                    Num.Text += act;
                    break;
                case "call":
                    SetNumsInText();
                    if (transferOn)
                        Phone.TransferCall(Num.Text);
                    else
                        Phone.Call(Num.Text);
                    transferOn = false;
                    CallBtn.Visibility = Visibility.Collapsed;
                    break;
                case "x":
                    Num.Text = "";
                    CallBtn.Visibility = Visibility.Visible;
                    Phone.HangUp();
                    break;                    
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Phone.ReRegister();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Phone.UnRegister();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Phone.UnRegister();
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MicOn_Click(object sender, RoutedEventArgs e)
        {
            MicOff.Visibility = Visibility.Visible;
            MicOn.Visibility = Visibility.Collapsed;
            Phone.TurnOnOffMic(false);
        }

        private void MicOff_Click(object sender, RoutedEventArgs e)
        {
            MicOff.Visibility = Visibility.Collapsed;
            MicOn.Visibility = Visibility.Visible;
            Phone.TurnOnOffMic(true);
        }

        private void SpOn_Click(object sender, RoutedEventArgs e)
        {

            SpOn.Visibility = Visibility.Collapsed;
            SpOff.Visibility = Visibility.Visible;
            Phone.TurnOnOffSpeaker(false);
        }

        private void SpOff_Click(object sender, RoutedEventArgs e)
        {
            SpOn.Visibility = Visibility.Visible;
            SpOff.Visibility = Visibility.Collapsed;
            Phone.TurnOnOffSpeaker(true);
        }

        private void TransferBtn_Click(object sender, RoutedEventArgs e)
        {
            Num.Text = "";
            CallBtn.Visibility = Visibility.Visible;
            transferOn = true;
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            Num.Focus();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Phone.UregAll();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
                this.ShowInTaskbar = false;
            else
                this.ShowInTaskbar = true;
        }

        private void AutoAnswerOn_Click(object sender, RoutedEventArgs e)
        {
            auto_answer = false;
            AutoAnswerOn.Visibility = Visibility.Collapsed;
            AutoAnswerOff.Visibility = Visibility.Visible;
        }

        private void AutoAnswerOff_Click(object sender, RoutedEventArgs e)
        {
            auto_answer = true;
            AutoAnswerOn.Visibility = Visibility.Visible;
            AutoAnswerOff.Visibility = Visibility.Collapsed;
        }

        private void Num_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Num_Button_Click(CallBtn, null);
            }
        }
    }
}
