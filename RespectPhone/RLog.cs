using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespectPhone
{
    class RLog
    {

        public static RLog _ins;
        public static string file = DateTime.Today.ToString("yyyy-MM-dd-")+"main_log.log";
        public List<CallItem> calls = new List<CallItem>();
        public static RLog INS
        {
            get
            {
                if (_ins == null)
                    _ins = new RLog();
                return _ins;
            }
        }


        public static void SaveExError(Exception ex,string Tag = "INNER")
        {
            try
            {
                if (!File.Exists(file))
                    File.Create(file);

                var f = File.ReadAllText(file);
                f += "\n";
                f += DateTime.Now + ": " + Tag+"\n";
                f += ex.Message;

            }
            catch { }
        }

        public static CallItem AddCallItem(string num, bool answer=true,bool outb = true)
        {
            CallItem it = new CallItem(num, answer ? CallItemState.ANSWERED : CallItemState.NOTANSWERED, outb ? CallItemTarget.OUTBOUND : CallItemTarget.INBOUND);
            RLog.INS.calls.Insert(0, it);
            return it;
        }
    }

   public class CallItem
    {
        public string num { get; set; } = "";
        public DateTime date { get; set; } = DateTime.Now;
        public CallItemState state { get; set; } = CallItemState.NOTANSWERED;
        public CallItemTarget target { get; set; } = CallItemTarget.OUTBOUND;

        public CallItem(string num,  CallItemState state, CallItemTarget target)
        {
            this.num = num;            
            this.state = state;
            this.target = target;
        }
    }

    public enum CallItemTarget
    {
        OUTBOUND=0,
        INBOUND=1
    }
    public enum CallItemState
    {
        NOTANSWERED=0,
        ANSWERED=1
    }
}
