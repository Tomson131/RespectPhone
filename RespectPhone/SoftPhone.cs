using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ozeki.Media.MediaHandlers;
using Ozeki.VoIP;
using Ozeki.VoIP.SDK;

namespace RespectPhone
{
    class SoftPhone
    {
        private ISoftPhone softPhone;
        private IPhoneLine phoneLine;
        private RegState phoneLineInformation;
        private IPhoneCall call;
        private IPhoneCall transfer_call;
        private readonly Microphone microphone;
        private readonly Speaker speaker;
        private readonly MediaConnector connector;
        private readonly PhoneCallAudioSender mediaSender;
        private readonly PhoneCallAudioReceiver mediaReceiver;

        /// <summary>
        /// Event triggered when the registered softphone has called
        /// </summary>
        public event EventHandler<VoIPEventArgs<IPhoneCall>> IncomingCallReceived;

        /// <summary>
        /// Event the softphone has successfully registered
        /// </summary>
        public event EventHandler RegistrationSucceded;
        public event EventHandler<VoIPEventArgs<RegistrationStateChangedArgs>> RegisterStateChanged;
        /// <summary>
        /// Handler of making call and receiving call
        /// </summary>
        /// <param name="registerName">The SIP ID what will registered into your PBX</param>
        /// <param name="domainHost">The address of your PBX</param>
        public SoftPhone(RespSIPAccount acc, string domainHost)
        {
            microphone = Microphone.GetDefaultDevice();
            speaker = Speaker.GetDefaultDevice();
            connector = new MediaConnector();
            mediaSender = new PhoneCallAudioSender();
            mediaReceiver = new PhoneCallAudioReceiver();
            InitializeSoftPhone(acc, domainHost);
        }


        /// <summary>
        ///It initializes a softphone object with a SIP PBX, and it is for requisiting a SIP account that is nedded for a SIP PBX service. It registers this SIP
        ///account to the SIP PBX through an ’IphoneLine’ object which represents the telephoneline. 
        ///If the telephone registration is successful we have a call ready softphone. In this example the softphone can be reached by dialing the registername.
        /// </summary>
        /// <param name="registerName">The SIP ID what will registered into your PBX</param>
        /// <param name="domainHost">The address of your PBX</param>
        private void InitializeSoftPhone(RespSIPAccount acc, string domainHost)
        {
            try
            {
                softPhone = SoftPhoneFactory.CreateSoftPhone(5000, 10000);
                softPhone.IncomingCall += softPhone_IncomingCall;
                var account = new SIPAccount(true, acc.displayName, acc.userName, acc.authenticationId, acc.registerPassword, acc.domainHost);
                phoneLine = softPhone.CreatePhoneLine(account);                
                phoneLine.RegistrationStateChanged += phoneLine_PhoneLineInformation;
                softPhone.RegisterPhoneLine(phoneLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("You didn't give your local IP adress, so the program won't run properly.\n {0}", ex.Message);
            }
        }

        /// <summary>
        /// Create and start the call to the dialed number
        /// </summary>
        /// <param name="dialedNumber"></param>
        /// 
        public void ReRegister()
        {
            softPhone.UnregisterPhoneLine(phoneLine);
            softPhone.RegisterPhoneLine(phoneLine);
        }
        public void UnRegister()
        {
            softPhone.UnregisterPhoneLine(phoneLine);                        
        }
        public void UregAll()
        {
            softPhone.UnregisterPhoneLine(phoneLine);
            phoneLine.Dispose();
            phoneLine = null;
            softPhone.Close();
        }
        public void HangUp()
        {
            if (call != null)
                call.HangUp();
        }

        public void TurnOnOffMic(bool v)
        {
            DisconnectMicToCall();
            if (v)
            {                
                ConnectMicToCall(call);
            }
        }
        public void TurnOnOffSpeaker(bool v)
        {
            DisconnectSpeakerToCall();
            if (v)
            {
                ConnectSpeakerToCall(call);
            }
        }
        public void Call(string dialedNumber)
        {
            if (phoneLineInformation != RegState.RegistrationSucceeded)
            {
                Console.WriteLine("Phone line state is not valid!");
                return;
            }

            if (string.IsNullOrEmpty(dialedNumber))
                return;

            if (call != null)
                return;

            call = softPhone.CreateCallObject(phoneLine, dialedNumber);
            WireUpCallEvents();
            call.Start();
            ConnectSpeakerToCall(call);
        }

        /// <summary>
        /// Occurs when phone line state has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void phoneLine_PhoneLineInformation(object sender, RegistrationStateChangedArgs e)
        {
            phoneLineInformation = e.State;
            Console.WriteLine("Register name:" + ((IPhoneLine)sender).SIPAccount.RegisterName);

            if (e.State == RegState.RegistrationSucceeded)
            {
                Console.WriteLine("Registration succeeded. Online.");
                OnRegistrationSucceded();
            }
            else
            {
                Console.WriteLine("Current state:" + e.State);
                Console.WriteLine("Errors:" + e.Error);
                Console.WriteLine("Errors:" + e.StatusCode);
                Console.WriteLine("Errors:" + e.ReasonPhrase);                
            }
            var handler = RegisterStateChanged;
            if (handler != null)
                handler(this, new VoIPEventArgs<RegistrationStateChangedArgs>(e));

        }

        /// <summary>
        /// Occurs when an incoming call request has received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void softPhone_IncomingCall(object sender, VoIPEventArgs<IPhoneCall> e)
        {
            Console.WriteLine("Incoming call from {0}", e.Item.DialInfo);
            if (call != null)
                e.Item.Respond(Ozeki.VoIP.SIP.ResponseCodes.BusyHere);
            else
            {
                call = e.Item;
                WireUpCallEvents();
                OnIncomingCallReceived(e.Item);
            }
        }

        /// <summary>
        /// Occurs when the phone call state has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void call_CallStateChanged(object sender, CallStateChangedArgs e)
        {
            Console.WriteLine("Call state changed: " + e.State);

            switch (e.State)
            {
                case CallState.InCall:
                    ConnectDevicesToCall();
                    break;
                case CallState.Completed:
                    DisconnectDevicesFromCall();
                    WireDownCallEvents();
                    call = null;
                    break;
                case CallState.Cancelled:
                    WireDownCallEvents();
                    call = null;
                    break;
                case CallState.Busy:
                    WireDownCallEvents();
                    call = null;
                    break;
            }

            GlobalEvent.Event(this, Events.CallStateChanged, e.State);
        }

        private void OnRegistrationSucceded()
        {
            var handler = RegistrationSucceded;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnIncomingCallReceived(IPhoneCall item)
        {
            var handler = IncomingCallReceived;

            if (handler != null)
                handler(this, new VoIPEventArgs<IPhoneCall>(item));
        }

        /// <summary>
        /// Connecting the microphone and speaker to the call
        /// </summary>
        private void ConnectDevicesToCall()
        {
            ConnectSpeakerToCall(call);
            ConnectMicToCall(call);
        }
        private void ConnectSpeakerToCall(ICall c)
        {
            
            if (speaker != null)
                speaker.Start();
            connector.Connect(mediaReceiver, speaker);            
            mediaReceiver.AttachToCall(c);
        }

        private void ConnectMicToCall(ICall c)
        {
            if (microphone != null)
                microphone.Start();
            connector.Connect(microphone, mediaSender);
            mediaSender.AttachToCall(c);
        }
        /// <summary>
        /// Disconnecting the microphone and speaker from the call
        /// </summary>
        private void DisconnectDevicesFromCall()
        {

            DisconnectMicToCall();
            DisconnectSpeakerToCall();
        }
        private void DisconnectSpeakerToCall()
        {
            if (speaker != null)
                speaker.Stop();
            connector.Disconnect(mediaReceiver, speaker);


            mediaReceiver.Detach();
        }

        private void DisconnectMicToCall()
        {
            if (microphone != null)
                microphone.Stop();
            connector.Disconnect(microphone, mediaSender);
            mediaSender.Detach();
        }
        /// <summary>
        ///  It signs up to the necessary events of a call transact.
        /// </summary>
        /// 
        public void AnswerIncoming(bool reject = false)
        {
            if (call != null)
                if (reject)
                    call.Respond(Ozeki.VoIP.SIP.ResponseCodes.BusyHere);
                else
                    call.Answer();
        }
        private void WireUpCallEvents()
        {
            if (call != null)
            {
                call.CallStateChanged += (call_CallStateChanged);
            }
        }

        /// <summary>
        /// It signs down from the necessary events of a call transact.
        /// </summary>
        private void WireDownCallEvents()
        {
            if (call != null)
            {
                call.CallStateChanged -= (call_CallStateChanged);
            }
        }

        public void TransferCall(string dest)
        {
            if (call == null) return;
            call.BlindTransfer(dest);
        }

      
        ~SoftPhone()
        {
            if (softPhone != null)
                softPhone.Close();
            WireDownCallEvents();
        }
    }
}
