using SIPSorcery.Media;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;
using SIPSorceryMedia.Abstractions;
using SIPSorceryMedia.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTEST;

namespace RespectPhone.SVOIP
{
    public class PhoneHandler : IRespPhone, IDisposable
    {

        public static string user = "4777";
        public static string pass = "1q2w3e4r";
        public static string url = "aster.institutrb.ru:5160";
        public static SipTransporManager transport = SipTransporManager.INS;
        SIPRegistrationUserAgent regAgent;
        SIPUserAgent sipAgent;
        SSipAccount sacc;
        SIPServerUserAgent incoming_server;       
        VoIPMediaSession voipMediaSession { get { return CreateMediaSession(); } }
        System.Threading.CancellationToken transfer_cancel = new System.Threading.CancellationToken();

        public PhoneHandler()
        {

        }

        public PhoneHandler(RespSIPAccount acc)
        {
            user = acc.authenticationId;
            pass = acc.registerPassword;
            url = acc.domainHost;
            sacc = new SSipAccount(user, pass, url);
            transport.SipRequestReseived += SIPLOG;


            regAgent = new SIPRegistrationUserAgent(transport.SIPTransport, user, pass, url, 120);
            regAgent.RegistrationFailed += RegistrationFailed;
            regAgent.RegistrationRemoved += RegistrationRemoved;
            regAgent.RegistrationSuccessful += RegistrationSuccessful;
            regAgent.RegistrationTemporaryFailure += RegistrationTemporaryFailure;
            

          


            sipAgent = new SIPUserAgent(transport.SIPTransport, null, true, sacc);
            sipAgent.OnIncomingCall += IncoimingCallReceive;

            regAgent.Start();


        }

        private VoIPMediaSession CreateMediaSession()
        {
            var windowsAudioEndPoint = new WindowsAudioEndPoint(new AudioEncoder());
         //   var windowsVideoEndPoint = new WindowsVideoEndPoint(new SIPSorceryMedia.Encoders.VpxVideoEncoder());

            MediaEndPoints mediaEndPoints = new MediaEndPoints
            {
                AudioSink = windowsAudioEndPoint,
                AudioSource = windowsAudioEndPoint,
                //VideoSink = windowsVideoEndPoint,
                //VideoSource = windowsVideoEndPoint,
            };

            // Fallback video source if a Windows webcam cannot be accessed.
//            var testPatternSource = new VideoTestPatternSource();

            var voipMediaSession = new VoIPMediaSession(mediaEndPoints);
            voipMediaSession.AcceptRtpFromAny = true;

            return voipMediaSession;
        }


        private void SIPLOG(object sender, SIPRequest e)
        {
            if (e.Method == SIPMethodsEnum.OPTIONS) return;
            Console.WriteLine("=============================");
            Console.WriteLine(e.Method);
            Console.WriteLine(e.Header.ToString());
            Console.WriteLine("-----------------------------");
            Console.WriteLine(e.Body != null? e.Body.ToString():"EMPTY");
            Console.WriteLine("=============================");
        }

        private void IncoimingCallReceive(SIPUserAgent agent, SIPRequest req)
        {
            incoming_server = sipAgent.AcceptCall(req);
            IncomingCallReceived?.Invoke(sipAgent, req);

            // sipAgent.Answer(uas, voipMediaSession);
            //   await userAgent.Answer(uas, voipMediaSession2); - ansering
        }


        #region Register events response
        private void RegistrationTemporaryFailure(SIPURI arg1, string arg2)
        {
            RegisterStateChanged?.Invoke(this, RegState.RegistrationFaild);
        }

        private void RegistrationSuccessful(SIPURI obj)
        {
            RegistrationSucceded?.Invoke(this, RegState.RegistrationSucceeded);
            RegisterStateChanged?.Invoke(this, RegState.RegistrationSucceeded);
        }

        private void RegistrationRemoved(SIPURI obj)
        {
            RegisterStateChanged?.Invoke(this, RegState.RegistrationFaild);
        }

        private void RegistrationFailed(SIPURI arg1, string arg2)
        {
            RegisterStateChanged?.Invoke(this, RegState.RegistrationFaild);
        }

        #endregion




        #region interface events
        public event EventHandler<object> IncomingCallReceived;
        public event EventHandler<object> RegisterStateChanged;
        public event EventHandler<object> RegistrationSucceded;
        public event EventHandler<object> CallStateCange;
        #endregion


        #region Interface voids
        public void AnswerIncoming(bool reject = false)
        {
            if (!reject)
            {
                if (incoming_server != null)
                {
                    try
                    {
                       
                        sipAgent.Answer(incoming_server, voipMediaSession);
                    }
                    catch { }
                }
            }
            else
            {
                if (incoming_server != null)
                {
                    incoming_server.Reject(SIPResponseStatusCodesEnum.BusyHere, "");
                    incoming_server = null;
                }
            }
        }

        public async void Call(string num)
        {
            if (!sipAgent.IsCallActive)
            {                
                voipMediaSession.AcceptRtpFromAny = true;
                var dest = num + "@" + url;
                bool callResult = await sipAgent.Call(dest, user, pass, voipMediaSession);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void HangUp()
        {
            if (sipAgent.IsCallActive)
                sipAgent.Hangup();
        }

        public void ReRegister()
        {
            throw new NotImplementedException();
        }

        public async void TransferCall(string num)
        {
            try
            {
                var TRANSFER_DESTINATION_SIP_URI = num;
                var transferURI = SIPURI.ParseSIPURI(TRANSFER_DESTINATION_SIP_URI);
                bool result = await sipAgent.BlindTransfer(transferURI, TimeSpan.FromSeconds(20), transfer_cancel);
            }
            catch
            {

            }
        }

        public void TurnOnOffMic(bool on)
        {
           

        }

        public void TurnOnOffSpeaker(bool on)
        {

           
        }

        public void UnRegister()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
