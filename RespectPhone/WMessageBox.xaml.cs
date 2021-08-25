using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
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
    /// Логика взаимодействия для WMessageBox.xaml
    /// </summary>
    /// 
    public partial class WMessageBox : Window
    {
        DialogResult res = RespectPhone.DialogResult.Cancel;
        Timer timer = new Timer(15000);
        public bool autoOK = false;
        public WMessageBox()
        {
            InitializeComponent();
            timer.Elapsed += timerFunc;
            timer.Start();
        }
        public WMessageBox(string msg, bool autoOK = false)
        {
            InitializeComponent();
            this.autoOK = autoOK;
            Dialog.Text = msg;
            timer.Elapsed += timerFunc;
            timer.Start();
        }
        private void timerFunc(object sender, ElapsedEventArgs e)
        {
            if (autoOK)
            {
                res = RespectPhone.DialogResult.OK;
                CClose();
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    this.Close();
                }));

            }
            timer.Stop();
        }
        public DialogResult ReturnResult()
        {
            return res;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            res = RespectPhone.DialogResult.OK;
            CClose();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CClose();
            this.Close();
        }
        private void CClose()
        {

            timer.Elapsed -= timerFunc;
            timer.Dispose();
        }

        public static DialogResult Show(string msg, bool btn1, bool btn2, bool autoOK = false)
        {
            WMessageBox m = new WMessageBox(msg, autoOK);
            m.btOk.Visibility = btn1 ? Visibility.Visible : Visibility.Hidden;
            m.btCancel.Visibility = btn2 ? Visibility.Visible : Visibility.Hidden;
            m.ShowDialog();
            return m.res;
        }
        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int.TryParse(((Ellipse)sender).Tag.ToString(), out int x);
                switch (x)
                {
                    case 1:
                        CClose();
                        this.Close();
                        break;
                    case 2:
                        if (this.WindowState == WindowState.Maximized)
                        { this.WindowState = WindowState.Normal; }
                        else { this.WindowState = WindowState.Maximized; }
                        break;
                    case 3:
                        if (this.WindowState == WindowState.Minimized)
                        { this.WindowState = WindowState.Normal; }
                        else { this.WindowState = WindowState.Minimized; }
                        break;
                }

            }

            catch
            {

            }

        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
            /*  if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
              {
                  if (this.WindowState == WindowState.Normal)
                  {
                      this.WindowState = WindowState.Maximized;
                  }
                  else
                  {
                      this.WindowState = WindowState.Normal;
                  }

              }*/
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //     res = Data.DialogResult.OK;
                //   CClose();
                // this.Close();
            }
            if (e.Key == Key.Escape)
            {
                res = RespectPhone.DialogResult.Cancel;
                CClose();
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btOk.Focus();
        }

    }

    public enum DialogResult
    {
        Cancel,
        OK
    }
}
