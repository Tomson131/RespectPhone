using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ozeki.VoIP;
using Ozeki.VoIP.SDK;

namespace RespectPhone
{
    class VoipTest
    {
        private bool registrationRequired=true;
        private string displayName ="test";
        private string userName="4777";
        private string authenticationId="4777";
        private string registerPassword="1q2w3e4r";
        private string domainHost= "aster.institutrb.ru:5160";
        private string domainPort="";
        

        public void Test1()
        {
            ISoftPhone softphone;   // softphone object  
            IPhoneLine phoneLine;   // phoneline object  
            softphone = SoftPhoneFactory.CreateSoftPhone(5000, 10000);
            var account = new SIPAccount(registrationRequired, displayName, userName, authenticationId, registerPassword, domainHost, domainPort);
            phoneLine = softphone.CreatePhoneLine(account);
            phoneLine.RegistrationStateChanged += phoneLine_PhoneLineStateChanged;
            softphone.IncomingCall += softPhone_IncomingCall;
            softphone.RegisterPhoneLine(phoneLine);
        }

        private void softPhone_IncomingCall(object sender, VoIPEventArgs<IPhoneCall> e)
        {
            Console.WriteLine("Incoming call from {0}", e.Item.DialInfo);
        }

        private void phoneLine_PhoneLineStateChanged(object sender, RegistrationStateChangedArgs e)
        {
            Console.WriteLine(e.State.ToString() + " : " + e.ReasonPhrase);
        }
    }
}
