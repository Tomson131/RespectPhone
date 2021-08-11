﻿using RespectPhone.Connections;
using RespectPhone.SVOIP;
using SIPSorcery.SIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using WPFTEST;

namespace RespectPhone
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IRespPhone Phone;
        bool transferOn = false;
        int seconds = 0;
        System.Timers.Timer timer = new System.Timers.Timer();
        bool auto_answer = false;
        System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        public MainWindow()
        {
            InitializeComponent();
            InitNotIcon();
            Login();
        }

        public async void Login(bool first=true)
        {
            var w = new WebLogin();
            mLogin.Text = RespSIPAccount.INS.rlogin;
            mPass.Text = RespSIPAccount.INS.rpass;
            SetLoader(1);
            if (first)
                RespSIPAccount.ReadConf();
            if (!RespSIPAccount.INS.UseConfExtension)
            {
                w = await WebAPIRequest.RespLogin();
                if (!w.isLogin)
                {
                    LoginGrid.Visibility = Visibility.Visible;
                    mMainGrid.Visibility = Visibility.Hidden;
                    SetLoader(0);
                    return;
                }
            }

            LoginGrid.Visibility = Visibility.Hidden;
            mMainGrid.Visibility = Visibility.Visible;
            if(!RespSIPAccount.INS.UseConfExtension)
                RespSIPAccount.INS.SetExt(w);
            Phone = new PhoneHandler(RespSIPAccount.INS);
            Phone.IncomingCallReceived += IncomingCall;
            Phone.RegisterStateChanged += RegisterStateChanged;
            Phone.CallStateCange += Phone_CallStateCange;
            GlobalEvent.INS.RiseAction += RiseAction;

            timer.Interval = 1000;
            timer.Elapsed += TimerTick;
            timer.Enabled = true;
            timer.Stop();
            
            SetLoader(0);
        }

        private void Phone_CallStateCange(object sender, object e)
        {
            GlobalEvent.Event(sender,Events.CallStateChanged ,e);
        }

        public void InitNotIcon()
        {
            nIcon.Icon = RespectPhone.Properties.Resources.phico;
            nIcon.Visible = true;
            nIcon.Click += nIcon_Click;
            var cm = new System.Windows.Forms.ContextMenu();
            cm.MenuItems.Add("Exit", ClickClose);
            nIcon.ContextMenu = cm;

        }

        private void ClickClose(object sender, EventArgs e)
        {
            RespSIPAccount.INS.SaveToFile();
            Phone.UnRegister();
            this.Close();
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
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            CallBtn.Visibility = Visibility.Visible;
                        }));
                        break;
                    case CallState.InCall:
                        if(Phone.InCall)
                            StartTimer();

                        break;
                    case CallState.Ringing:
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Time.Content = "Calling...";
                        }));
                       
                        break;
                }
            }
        }


        private void RegisterStateChanged(object sender, object e)
        {
            if(e is RegState)
            if ((RegState)e == RegState.RegistrationSucceeded)
                ChangeStatusLabel(true);
            else
            {
                ChangeStatusLabel(false);
            }
        }

        private void IncomingCall(object sender, object e)
        {
            Console.WriteLine("call " + e.ToString()); /// need to parse 
            SIPRequest c = null;
            if (e is SIPRequest)
                c = (SIPRequest)e;

            Dispatcher.BeginInvoke((Action)(() => {
                if (this.WindowState == WindowState.Minimized)
                    this.WindowState = WindowState.Normal;
                if (auto_answer) {
                    Phone.AnswerIncoming();
                }
                else
                {
                    if (RespSIPAccount.INS.AnswerMyExt)
                    {
                        if (c.Header.From.FromName == RespSIPAccount.INS.authenticationId || c.Header.From.FromURI.ToString().Contains(RespSIPAccount.INS.authenticationId+"@"))
                            Phone.AnswerIncoming();
                        else
                        {
                            IncomingCall ic = new IncomingCall(e);
                            // Num.Text = e.ToString();
                            ic.Show();
                            ic.Topmost = true;
                        }
                    }
                    else
                    {
                        IncomingCall ic = new IncomingCall(e);
                       // Num.Text = e.ToString();
                        ic.Show();
                        ic.Topmost = true;
                    }
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
                    StatLabel.Text = "Подключен "+ RespSIPAccount.INS.displayName+"("+ RespSIPAccount.INS.authenticationId+ ")";
                }
                else
                {

                    ImgStat0.Visibility = Visibility.Visible;
                    ImgStat1.Visibility = Visibility.Collapsed;
                    StatLabel.Text = "Отключен";
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
                    if (Phone.isTransferAttended)
                    {
                        Phone.ContinueTransfer();
                    }
                    else
                    {
                        Num.Text = "";
                        StopTimer();
                        CallBtn.Visibility = Visibility.Visible;
                        Phone.HangUp();
                    }
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
            if (this.WindowState != WindowState.Minimized)
                this.WindowState = WindowState.Minimized;
            else
                this.WindowState = WindowState.Normal;
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
            if (Phone.isTransferAttended)
            {
           
            }
            else
            {
                Num.Text = "";
                CallBtn.Visibility = Visibility.Visible;
                transferOn = true;
                TransferBtn.Visibility = Visibility.Collapsed;
                CancelTransferBtn.Visibility = Visibility.Visible;
            }
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            Num.Focus();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
          //  Phone.UregAll();
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

        private  void Login_Click(object sender, RoutedEventArgs e)
        {
            RespSIPAccount.INS.rlogin = mLogin.Text.Trim();
            RespSIPAccount.INS.rpass= mPass.Text.Trim();
            Login(false);
        }

        public void SetLoader(int x)
        {
            if (x == 1)
            {
                Thread th = new Thread(() => {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        Loader.Children.Clear();
                        Loader ld = new Loader();
                        Loader.Children.Add(ld);
                        Loader.Visibility = Visibility.Visible;
                    }));
                });
                th.Start();
            }
            else
            {
                Thread th = new Thread(() => {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        Loader.Children.Clear();
                        Loader.Visibility = Visibility.Collapsed;
                    }));
                });
                th.Start();
            }
        }

        private void CancelTransferBtn_Click(object sender, RoutedEventArgs e)
        {
            TransferBtn.Visibility = Visibility.Visible;
            CancelTransferBtn.Visibility = Visibility.Collapsed;
            Phone.CancelTransfer();           
            transferOn = false;
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            RSettings rs = new RSettings();
            rs.Show();
        }
    }
}
