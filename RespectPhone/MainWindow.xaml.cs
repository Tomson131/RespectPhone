﻿using AutoUpdaterDotNET;
using Newtonsoft.Json.Linq;
using RespectPhone.Connections;
using RespectPhone.Helpers;
using RespectPhone.SVOIP;
using SIPSorcery.SIP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
        bool away = false;
        System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        private MediaPlayer snd;
        private MediaPlayer bsnd;
        CallItem call_item = null;
        List<string> MessageIds= new List<string>();
        public bool ForceUpdateStart { get; set; } = false;

        public MainWindow()
        {
            CheckDoubleApp();
            InitializeComponent();
            InitNotIcon();
           
            Login();
        }

        private void CheckDoubleApp()
        {
            try
            {
                string processName = "RespectPhone";
                var pp = Process.GetProcesses();
                var isNew = pp.Where(p => p.ProcessName == processName).ToList().Count<2;
                

                if (isNew)
                {
                   
                    return;
                }
                else
                {

                    MessageBox.Show("Софтфон уже запущен!", "", MessageBoxButton.OK);
                    this.Close();
                }
            }
            catch
            {

            }
            

        }

        public async void Login(bool first=true)
        {
            var w = new WebLogin();
            mLogin.Text = RespSIPAccount.INS.rlogin;
            mPass.Text = RespSIPAccount.INS.GetPass();
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
            Phone.IncomeMessageReceived += IncomingMessage;
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
            cm.MenuItems.Add("Update", ClickUpdate);
            cm.MenuItems.Add("Exit", ClickClose);            
            nIcon.ContextMenu = cm;

        }

        private void ClickUpdate(object sender, EventArgs e)
        {
            try
            {
                //ForceUpdateStart = true;
                //AutoUpdater.Start(RespSIPAccount.INS.UpdateUrl);


                if (AutoUpdater.DownloadUpdate())
                {

                    AutoUpdater_ApplicationExitEvent();
                }
            }
            catch (Exception exception)
            {
                WMessageBox.Show(exception.Message, false, true);

            }
        }

        private void ClickClose(object sender, EventArgs e)
        {
            try
            {
                RespSIPAccount.INS.SaveToFile();
                if(Phone!=null)
                    Phone.UnRegister();
            }
            catch { }
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
                    call_item.state = CallItemState.ANSWERED;
                    break;
                case Events.RejectIncoming:
                    Phone.AnswerIncoming(true);
                    break;
                case Events.CallStateChanged:
                    CallStateChanged(data);
                    break;
                case Events.OrginateCall:
                    Originatecall((string)data);
                    break;
            }
        }

        private void Originatecall(string data)
        {
            away = false;
            AwayBtnOff.Visibility = Visibility.Collapsed;
            AwayBtn.Visibility = Visibility.Visible;

            if (CallBtn.Visibility == Visibility.Visible)
            {
                Num.Text = data;
                Num_Button_Click(CallBtn, null);
            }

        }

        private void CallStateChanged(object data)
        {
            
            if (data is CallState)
            {
                Console.WriteLine("MAINCALLSTATE: "+data.ToString());
                var st = (CallState)data;
                switch (st)
                {
                    case CallState.Cancelled:
                    case CallState.Busy:
                    case CallState.Completed:
                        StopRing();
                        StopTimer();                        
                        if (st == CallState.Busy)
                            if(!away)
                                PlayBeep(true);
                        
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            if(CallBtn.Visibility!=Visibility.Visible)
                                if (st == CallState.Completed)
                                    PlayBeep();
                            CallBtn.Visibility = Visibility.Visible;
                        }));
                        break;
                    case CallState.InCall:
                        if(Phone.InCall)
                            StartTimer();                        
                        break;
                    case CallState.Ringing:
                        if (!away)
                            Dispatcher.BeginInvoke((Action)(() =>
                            {
                                Time.Content = "Calling...";
                            }));
                       
                        break;
                    case CallState.Answered:
                        StopRing();
                        call_item.state = CallItemState.ANSWERED;
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


        private void IncomingMessage(object sender, object e)
        {
            Console.WriteLine("call " + e.ToString());
            SIPRequest c = null;
            if (e is SIPRequest)
                c = (SIPRequest)e;
            if (c == null) return;

            if (c.Method == SIPMethodsEnum.MESSAGE)
            {
                if (!MessageIds.Contains(c.Header.CallId))
                {
                    MessageIds.Add(c.Header.CallId);
                    ParseIncomeMessage(c.Body);
                }
            }


        }

        private void ParseIncomeMessage(string body)
        {
            try
            {
                var json = JObject.Parse(body);
                var action = JSONHelper.GetString(json["action"]);

                switch(action) {

                    case "hang_up":
                        Dispatcher.BeginInvoke((Action)(()=>{
                            Num_Button_Click(HangUpBtn, null);
                        }));
                        
                        break;
                    case "message":
                        Dispatcher.BeginInvoke((Action)(() => {
                            InMessages msgw = new InMessages(JSONHelper.GetString(json["text"]));
                            msgw.Show();
                            //nIcon.ShowBalloonTip(10, "Сообщение", JSONHelper.GetString(json["text"]), System.Windows.Forms.ToolTipIcon.Info);
                        }));

                        break;
                }


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void IncomingCall(object sender, object e)
        {
            
            Console.WriteLine("call " + e.ToString()); /// need to parse 
            SIPRequest c = null;
            if (e is SIPRequest)
                c = (SIPRequest)e;
            if (c == null) return;

            var its_me = c.Header.From.FromURI.User == RespSIPAccount.INS.authenticationId;
            if (away && !its_me)
            {
                Phone.AnswerIncoming(true);
                return;
            }
            Dispatcher.BeginInvoke((Action)(() => {
                if (this.WindowState == WindowState.Minimized)
                    this.WindowState = WindowState.Normal;
                call_item = RLog.AddCallItem(c.Header.From.FromName, false,false);
               
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
            var str = string.Join("", intMatch);
            if (str.Length == 11)              
                    str = "8"+str.Substring(1, str.Length - 1);
            if (str.Length == 10)
                if (str[0] != '8')
                    str = "8" + str;
            Num.Text = str;
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
                    Phone.SetDTMF(act);
                    break;
                case "call":
                    SetNumsInText();
                    if (transferOn)
                        Phone.TransferCall(Num.Text);
                    else
                    {
                        Phone.Call(Num.Text);
                        call_item = RLog.AddCallItem(Num.Text,false);
                    }
                    transferOn = false;
                    CallBtn.Visibility = Visibility.Collapsed;
                    PlayRing();
                    break;
                case "x":
                    StopRing();
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
                        PlayBeep();
                    }
                  
                    TransferBtn.Visibility = Visibility.Visible;
                    CancelTransferBtn.Visibility = Visibility.Collapsed;
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
            StopRing();
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
            if (RespSIPAccount.INS.AlwaysOnTop)
            {
                if (!this.Topmost)
                    this.Topmost = true;
            }
            else
            {
                if (this.Topmost)
                    this.Topmost = false;
            }

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
        private void AwayBtn_Click(object sender, RoutedEventArgs e)
        {
            away = true;
            AwayBtn.Visibility = Visibility.Collapsed;
            AwayBtnOff.Visibility = Visibility.Visible;
        }
        private void AwayBtnOff_Click(object sender, RoutedEventArgs e)
        {
            away = false;
            AwayBtnOff.Visibility = Visibility.Collapsed;
            AwayBtn.Visibility = Visibility.Visible;
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
            RespSIPAccount.INS.SetPass(mPass.Text.Trim());
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
            StopRing();
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            RSettings rs = new RSettings();
            rs.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RespSIPAccount.INS.AlwaysOnTop)
                {
                    if (!this.Topmost)
                        this.Topmost = true;
                }
                else
                {
                    if (this.Topmost)
                        this.Topmost = false;
                }
                AutoUpdater.Start(RespSIPAccount.INS.UpdateUrl);                
                AutoUpdater.ReportErrors = false;
                AutoUpdater.RunUpdateAsAdmin = false;
                AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
                AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;
                AutoUpdater.ParseUpdateInfoEvent += AutoUpdater_ParseUpdateInfoEvent;
            }
            catch(Exception ex)
            {
                WMessageBox.Show(ex.Message,false,false,true);
            }
        }

        private void AutoUpdater_ParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            if (args == null) return;
            try
            {
                var ui = JSONHelper.ReadVersionJson(args.RemoteData);
                args.UpdateInfo = ui;

                int x = JSONHelper.ParseNewVers(args.RemoteData);
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                //Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    // set labet for new vers available here
                //    //NewVersLabel.Visibility = x > v.Revision ? Visibility.Visible : Visibility.Collapsed;
                //}));


            }
            catch { }
        }

        private void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args == null) return;
            
            try
            {
                if (args.IsUpdateAvailable)
                {
                    DialogResult dialogResult;
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        dialogResult =
                        WMessageBox.Show(
                            $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. Do you want to update the application now?",
                            true, true, true);
                        if (dialogResult.Equals(RespectPhone.DialogResult.OK))
                        {
                            try
                            {
                                if (AutoUpdater.DownloadUpdate())
                                {

                                    AutoUpdater_ApplicationExitEvent();
                                }
                            }
                            catch (Exception exception)
                            {
                                WMessageBox.Show(exception.Message, false, true);
                                this.Close();
                            }
                        }
                    }));
                }
                //else if (ForceUpdateStart)
                //{
                //    try
                //    {
                //        if (AutoUpdater.DownloadUpdate(args))
                //        {

                //            AutoUpdater_ApplicationExitEvent();
                //        }
                //    }
                //    catch (Exception exception)
                //    {
                //        WMessageBox.Show(exception.Message, false, true);
                //        this.Close();
                //    }
                //}
            }
            catch
            {

            }
        }

        private void AutoUpdater_ApplicationExitEvent()
        {

            try
            {
                nIcon.Icon = null;
                nIcon = null;

            }
            catch
            {

            }
            try
            {
                Environment.ExitCode = 0;
                Environment.Exit(0);
                //    Thread.Sleep(5000);
            }
            catch
            {
                this.Close();
                //  Thread.Sleep(5000);
            }
        }


        #region Sound calling

        public void PlayRing()
        {
            try
            {
                
                snd = new MediaPlayer();
                snd.Open(new Uri(System.Environment.CurrentDirectory + "\\resources\\tube_sound.wav", UriKind.Relative));
                snd.Volume = 0.2;
                snd.MediaEnded += RepeatRing;
                snd.Play();
            }
            catch (Exception ex)
            {
                RLog.SaveExError(ex,"START CALL SOUND");

            }
        }
        public void PlayBeep(bool busy = false)
        {
            try
            {
                Phone.ClearMediaSession();

               
                Dispatcher.BeginInvoke((Action)(() =>
                {

                    bsnd = new MediaPlayer();
                    if (busy)
                    {
                        bsnd.Open(new Uri(System.Environment.CurrentDirectory + "\\resources\\busy_tone.wav", UriKind.Relative));
                    }
                    else
                    {
                        bsnd.Open(new Uri(System.Environment.CurrentDirectory + "\\resources\\beep1.wav", UriKind.Relative));
                    }

                    bsnd.Volume = 0.8;
                    bsnd.Play();                    
                }));
            }
            catch (Exception ex)
            {
                RLog.SaveExError(ex, "START BEEP SOUND");

            }
        }
        private void RepeatRing(object sender, EventArgs e)
        {

            try
            {
                
                snd.Position = TimeSpan.Zero;
                snd.Play();
            }
            catch (Exception ex)
            {
                RLog.SaveExError(ex,"REPEAT CALL SOUND");

            }
        }

        public void StopRing()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)(() => { 
                    if (snd == null) return;

                    snd.Stop();
                    snd.MediaEnded -= RepeatRing;
                    snd = null;
                }));
            }
            catch(Exception ex)
            {
                RLog.SaveExError(ex,"STOP CALL SOUND");
            }
        }


        #endregion

        private void Border_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            CallList cl = new CallList();
            cl.Show();
        }

       
    }
}
