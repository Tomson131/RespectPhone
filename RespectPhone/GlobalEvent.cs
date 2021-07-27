using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespectPhone
{
    public class GlobalEvent
    {
        public static GlobalEvent _ins;       
        public static GlobalEvent INS
        {
            get
            {
                if (_ins == null)
                    _ins = new GlobalEvent();
                return _ins;
            }
        }

        public delegate void RiseEventAction(object sender, Events ev, object data);
        public RiseEventAction RiseAction;

        public static void Event(object sender, Events ev, object data)
        {
            GlobalEvent.INS.RiseAction?.Invoke(sender, ev, data);
        }
        
    }

    public enum Events
    {
        AnswerIncoming,
        RejectIncoming,
        CallStateChanged
    }
}
