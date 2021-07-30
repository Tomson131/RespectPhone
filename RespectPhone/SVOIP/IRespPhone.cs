using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTEST
{
    public interface IRespPhone
    {
        void Call(string num);
        void TransferCall(string num);
        void HangUp();
        void TurnOnOffMic(bool on);
        void TurnOnOffSpeaker(bool on);
        void ReRegister();
        void UnRegister();
        void AnswerIncoming(bool reject = false);

        event EventHandler<object> IncomingCallReceived;
        event EventHandler<object> RegisterStateChanged;
        event EventHandler<object> RegistrationSucceded;
        event EventHandler<object> CallStateCange;

    }
}
