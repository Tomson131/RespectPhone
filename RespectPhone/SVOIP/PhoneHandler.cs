using SIPSorcery.Media;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;
using SIPSorceryMedia.Abstractions;
using SIPSorceryMedia.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPFTEST;

namespace RespectPhone.SVOIP
{
    public class PhoneHandler : IRespPhone, IDisposable
    {

        public static string user = "4777";
        public static string pass = "1";
        public static string url = "aster.institutrb.ru:5160";
        public static SipTransporManager transport = SipTransporManager.INS;
        SIPRegistrationUserAgent regAgent;
        SIPUserAgent sipAgent;
        SIPUserAgent transferSipAgent;
        SSipAccount sacc;
        SIPServerUserAgent incoming_server;        
        bool micOn = true;
        bool spOn = true;
        public bool isTransferAttended = false;
        List<string> incomingCallID = new List<string>();
        VoIPMediaSession voipMediaSession { get { return CreateMediaSession(); } }

        public bool InCall { get { return sipAgent.IsCallActive || sipAgent.IsRinging; } }

        bool IRespPhone.isTransferAttended { get { return isTransferAttended; } }

        System.Threading.CancellationToken transfer_cancel = new System.Threading.CancellationToken();
        public CallState CallState = CallState.None;
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
            transport.SipResponseReseivedz += SIPRESPLOG;

            regAgent = new SIPRegistrationUserAgent(transport.SIPTransport, user, pass, url, 3600);
            regAgent.RegistrationFailed += RegistrationFailed;
            regAgent.RegistrationRemoved += RegistrationRemoved;
            regAgent.RegistrationSuccessful += RegistrationSuccessful;
            regAgent.RegistrationTemporaryFailure += RegistrationTemporaryFailure;
            


            transferSipAgent = new SIPUserAgent(transport.SIPTransport, null, true, sacc);
            transferSipAgent.OnIncomingCall += TranferTransportIncomingCall;
            transferSipAgent.ClientCallAnswered += SipAgent_ClientCallAnswered;

            sipAgent = new SIPUserAgent(transport.SIPTransport, null, true, sacc);
            sipAgent.OnIncomingCall += IncoimingCallReceive;

            sipAgent.ClientCallAnswered += SipAgent_ClientCallAnswered;
            sipAgent.ClientCallFailed += SipAgent_ClientCallFailed;
            sipAgent.ClientCallRinging += SipAgent_ClientCallRinging;
            sipAgent.ClientCallTrying += SipAgent_ClientCallTrying;
            regAgent.Start();


        }

        

        private void TranferTransportIncomingCall(SIPUserAgent arg1, SIPRequest arg2)
        {
            
        }

        private void SIPRESPLOG(object sender, SIPResponse e)
        {
            Console.WriteLine("CALLSTATE:" + e.Status);
            switch (e.Status)
            {
                case SIPResponseStatusCodesEnum.SessionProgress:
                case SIPResponseStatusCodesEnum.Ringing:
                    CallStateCange?.Invoke(this, CallState.Ringing);
                    break;
                case SIPResponseStatusCodesEnum.Ok:
                    CallStateCange?.Invoke(this, CallState.InCall);
                    break;
                

            }
        }

        private void SipAgent_ClientCallTrying(ISIPClientUserAgent uac, SIPResponse sipResponse)
        {
            CallStateCange?.Invoke(this, CallState.Trying);
        }

        private void SipAgent_ClientCallRinging(ISIPClientUserAgent uac, SIPResponse sipResponse)
        {
            CallStateCange?.Invoke(this, CallState.Ringing);
        }

        private void SipAgent_ClientCallFailed(ISIPClientUserAgent uac, string errorMessage, SIPResponse sipResponse)
        {
            CallStateCange?.Invoke(this, CallState.Cancelled);
        }

        private void SipAgent_ClientCallAnswered(ISIPClientUserAgent uac, SIPResponse sipResponse)
        {
            CallStateCange?.Invoke(this, CallState.Answered);
        }

        private VoIPMediaSession CreateMediaSession()
        {
            try
            {
                var windowsAudioEndPointRec = new WindowsAudioEndPoint(new AudioEncoder());
                // var windowsAudioEndPointSend = new WindowsAudioEndPoint(new AudioEncoder());
                //   var windowsVideoEndPoint = new WindowsVideoEndPoint(new SIPSorceryMedia.Encoders.VpxVideoEncoder());

                MediaEndPoints mediaEndPoints = new MediaEndPoints
                {
                    AudioSink = windowsAudioEndPointRec,
                    AudioSource = windowsAudioEndPointRec,
                    //VideoSink = windowsVideoEndPoint,
                    //VideoSource = windowsVideoEndPoint,
                };

                // Fallback video source if a Windows webcam cannot be accessed.
                //            var testPatternSource = new VideoTestPatternSource();

                var voipMediaSession = new VoIPMediaSession(mediaEndPoints);
                voipMediaSession.AcceptRtpFromAny = true;
                return voipMediaSession;
            }
            catch (Exception ex)
            {
                WMessageBox.Show(ex.Message, false, false);
                return new VoIPMediaSession(new MediaEndPoints());
            }
        }


        private void SIPLOG(object sender, SIPRequest e)
        {

            if (e.Method == SIPMethodsEnum.BYE)
            {
                CheckCallId(e);
                CallStateCange?.Invoke(this, CallState.Completed);                
            }
            if (e.Method == SIPMethodsEnum.CANCEL)
            {
                CheckCallId(e);
                CallStateCange?.Invoke(this, CallState.Cancelled);
            }
            


            if (e.Method == SIPMethodsEnum.OPTIONS) return;
            //Console.WriteLine("=============================");
            //Console.WriteLine(e.Method);
            //Console.WriteLine(e.Header.ToString());
            //Console.WriteLine("-----------------------------");
            //Console.WriteLine(e.Body != null? e.Body.ToString():"EMPTY");
            //Console.WriteLine("=============================");
        }

        private void CheckCallId(SIPRequest e)
        {
            if (incomingCallID.Contains(e.Header.CallId))
                incomingCallID.Remove(e.Header.CallId);
        }

        private void IncoimingCallReceive(SIPUserAgent agent, SIPRequest req)
        {
            try
            {
                if (sipAgent.IsCallActive || sipAgent.IsCalling || sipAgent.IsRinging)
                {

                }
                else
                {
                    incoming_server = sipAgent.AcceptCall(req);
                    IncomingCallReceived?.Invoke(sipAgent, req);
                    incomingCallID.Add(req.Header.CallId);

                }
                // sipAgent.Answer(uas, voipMediaSession);
                //   await userAgent.Answer(uas, voipMediaSession2); - ansering
            }
            catch { }
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
               
                var dest = num + "@" + url;
                bool callResult = await sipAgent.Call(dest, user, pass, voipMediaSession);
            }
        }

        public async void TransferAttended(string num)
        {
            if (sipAgent.IsCallActive)
            {
                sipAgent.PutOnHold();
                sipAgent.MediaSession.SetMediaStreamStatus(SIPSorcery.Net.SDPMediaTypesEnum.audio, SIPSorcery.Net.MediaStreamStatusEnum.Inactive);
                var dest = num + "@" + url;
              
                var res = await transferSipAgent.Call(dest, user, pass, voipMediaSession);
                if (res)
                {
                    isTransferAttended = true;
                }
                else
                {
                    sipAgent.TakeOffHold();
                    sipAgent.MediaSession.SetMediaStreamStatus(SIPSorcery.Net.SDPMediaTypesEnum.audio, SIPSorcery.Net.MediaStreamStatusEnum.SendRecv);
                }
            }
        }
        public void ContinueTransfer()
        {
            ContinueTransferAsync();

        }
        public async void ContinueTransferAsync()
        {
            var tres = await transferSipAgent.AttendedTransfer(sipAgent.Dialogue, new TimeSpan(100000), transfer_cancel);
            isTransferAttended = false;
            if (tres)
            {
                transferSipAgent.Hangup();
                sipAgent.Hangup();                
                CallStateCange?.Invoke(this, CallState.Completed);
                
            }

        }
        public void CancelTransfer()
        {
            if(sipAgent.MediaSession!=null)
                sipAgent.MediaSession.SetMediaStreamStatus(SIPSorcery.Net.SDPMediaTypesEnum.audio, SIPSorcery.Net.MediaStreamStatusEnum.SendRecv);
                      
                     
            try
            {
                sipAgent.TakeOffHold();
                transferSipAgent.Hangup();
                transferSipAgent.Cancel();
            }
            catch { }
            isTransferAttended = false;

        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void HangUp()
        {
            // if (sipAgent.IsCallActive)
            try
            {
                sipAgent.Hangup();
                sipAgent.Cancel();
               // sipAgent.
            }
            catch { }
        }

        public void ReRegister()
        {
            regAgent.Stop();

            regAgent.Start();
        }

        public void TransferCall(string num)
        {
            TransferAttended(num);
            
        }
        public async void BlindTransfer(string num)
        {
            try
            {
                var TRANSFER_DESTINATION_SIP_URI = "sip:" + num + "@" + url;
                var transferURI = SIPURI.ParseSIPURI(TRANSFER_DESTINATION_SIP_URI);
                bool result = await sipAgent.BlindTransfer(transferURI, TimeSpan.FromSeconds(20), transfer_cancel);
                if (result)
                    CallStateCange?.Invoke(this, CallState.Completed);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void TurnOnOffMic(bool on)
        {
            micOn = on;
            SetAudioStatus();
        }

        public void TurnOnOffSpeaker(bool on)
        {
            spOn = on;
            SetAudioStatus();
        }

        private void SetAudioStatus()
        {
            if(micOn && spOn)
            {
                sipAgent.MediaSession.SetMediaStreamStatus(SIPSorcery.Net.SDPMediaTypesEnum.audio, SIPSorcery.Net.MediaStreamStatusEnum.SendRecv);
            }else if(!micOn && spOn)
            {
                sipAgent.MediaSession.SetMediaStreamStatus(SIPSorcery.Net.SDPMediaTypesEnum.audio, SIPSorcery.Net.MediaStreamStatusEnum.RecvOnly);
            }
            else if (micOn && !spOn)
            {
                sipAgent.MediaSession.SetMediaStreamStatus(SIPSorcery.Net.SDPMediaTypesEnum.audio, SIPSorcery.Net.MediaStreamStatusEnum.SendOnly);
            }
            else if (!micOn && !spOn)
            {
                sipAgent.MediaSession.SetMediaStreamStatus(SIPSorcery.Net.SDPMediaTypesEnum.audio, SIPSorcery.Net.MediaStreamStatusEnum.Inactive);
            }
        }


        public void UnRegister()
        {
            regAgent.Stop();
        }

        public bool CheckActiveCallId(string s)
        {
            return incomingCallID.Contains(s);
        }

        public void SetDTMF(string act)
        {
            if (sipAgent.IsCallActive)
            {
                int.TryParse(act, out int xt);
                switch (xt)
                {
                    case 0:
                        sipAgent.SendDtmf(0x00);
                        break;
                    case 1:
                        sipAgent.SendDtmf(0x01);
                        break;
                    case 2:
                        sipAgent.SendDtmf(0x02);
                        break;
                    case 3:
                        sipAgent.SendDtmf(0x03);
                        break;
                    case 4:
                        sipAgent.SendDtmf(0x04);
                        break;
                    case 5:
                        sipAgent.SendDtmf(0x05);
                        break;
                    case 6:
                        sipAgent.SendDtmf(0x06);
                        break;
                    case 7:
                        sipAgent.SendDtmf(0x07);
                        break;
                    case 8:
                        sipAgent.SendDtmf(0x08);
                        break;
                    case 9:
                        sipAgent.SendDtmf(0x09);
                        break;

                }
              //  Byte.TryParse(xt, out byte x);
                
            }
        }
        #endregion
    }
}
