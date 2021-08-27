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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RespectPhone.MiniItems
{
    /// <summary>
    /// Логика взаимодействия для CallItemRow.xaml
    /// </summary>
    public partial class CallItemRow : UserControl
    {
        public CallItemRow(CallItem it)
        {
            InitializeComponent();
            Date.Content = it.date.ToString("HH:mm:ss");
            Num.Text = it.num;
            Inb.Content = it.target == CallItemTarget.INBOUND ? "Вх" : "Исх";
            State.Content = it.state== CallItemState.ANSWERED ? "Отв" : "НЕОТВ";
        }
    }
}
