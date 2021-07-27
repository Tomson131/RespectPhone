using Ozeki.VoIP;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для IncomingCall.xaml
    /// </summary>
    public partial class IncomingCall : Window
    {
        MediaPlayer snd;
        public IncomingCall(ICall call)
        {
            InitializeComponent();
            Caller.Text = call.DialInfo.CallerDisplay + " " + call.DialInfo.CallerID;
            GlobalEvent.INS.RiseAction += RiseAction;
            PlayRing();
        }

        private void RiseAction(object sender, Events ev, object data)
        {
            switch (ev)
            {
                case Events.CallStateChanged:
                    CallStateChanged(data);
                    break;
            }
        }

        public void PlayRing()
        {
            try
            {
                this.Topmost = false;
                System.IO.Stream str = Properties.Resources.Ringing;
                snd = new MediaPlayer();
                snd.Open(new Uri(System.Environment.CurrentDirectory + "\\resources\\Ringing.wav", UriKind.Relative));
                snd.Volume = 0.2;
                snd.Play();
            }
            catch (Exception e)
            {

                
            }
        }
        public void StopRing()
        {
            try
            {
                if (snd == null) return;

                snd.Stop();                
                snd = null;
            }
            catch 
            {

            }
        }

        private void CallStateChanged(object data)
        {
            if(data is CallState)
            {
                var st = (CallState)data;
                switch (st)
                {
                    case CallState.Cancelled:
                    case CallState.Busy:
                    case CallState.Answered:
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            this.Close();
                        }));
                        break;
                }
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    CallStateText.Text = st.ToString();
                }));
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StopRing();
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Num_Button_Click(object sender, RoutedEventArgs e)
        {
            GlobalEvent.Event(this, Events.RejectIncoming, null);
            StopRing();
            this.Close();
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GlobalEvent.Event(this, Events.AnswerIncoming, null);
            StopRing();
            this.Close();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
         
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GlobalEvent.INS.RiseAction -= RiseAction;
            StopRing();
        }
    }
}
