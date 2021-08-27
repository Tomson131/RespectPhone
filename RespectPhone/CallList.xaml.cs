using RespectPhone.MiniItems;
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
    /// Логика взаимодействия для CallList.xaml
    /// </summary>
    public partial class CallList : Window
    {
        public CallList()
        {
            InitializeComponent();
            foreach(var it in RLog.INS.calls)
            {
                CallItemRow row = new CallItemRow(it);
                Calls.Children.Add(row);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
