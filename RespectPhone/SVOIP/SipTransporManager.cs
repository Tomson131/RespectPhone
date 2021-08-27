using Microsoft.Extensions.Logging;
using SIPSorcery.Net;
using SIPSorcery.SIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WPFTEST
{
    public class SipTransporManager

    {
        private static int HOMER_SERVER_PORT = 5061;
        public SIPTransport SIPTransport = new SIPTransport();        
        public static SipTransporManager _ins { get; set; }
        public static SipTransporManager INS { get { if (_ins == null) _ins = new SipTransporManager(); return _ins; } }

        public event Func<SIPRequest, bool> IncomingCall;

        public event EventHandler<SIPRequest> SipRequestReseived;
        public event EventHandler<SIPResponse> SipResponseReseivedz;
        public SipTransporManager()
        {
            SIPTransport.AddSIPChannel(new SIPUDPChannel(new IPEndPoint(IPAddress.Any, HOMER_SERVER_PORT)));
            //  SIPTransport.SIPTransportRequestReceived += RequestSIP;

            // Wire up the transport layer so incoming SIP requests have somewhere to go.
            SIPTransport.SIPTransportRequestReceived += SIPTransportRequestReceived;

            // Log all SIP packets received to a log file.
            SIPTransport.SIPRequestInTraceEvent += SIPRequestInTraceEvent;
            SIPTransport.SIPRequestOutTraceEvent += SIPRequestOutTraceEvent;
            SIPTransport.SIPResponseInTraceEvent += SIPResponseInTraceEvent;
            SIPTransport.SIPResponseOutTraceEvent += SIPResponseOutTraceEvent;

            SIPTransport.SIPTransportResponseReceived += SIPTransport_SIPTransportResponseReceived;
        }

        private Task SIPTransport_SIPTransportResponseReceived(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPResponse e)
        {

            SipResponseReseivedz?.Invoke(this, e);
            //Console.WriteLine("============================= RESPONSE RECEIVED =========================================");
            //Console.WriteLine("STATUS: " + e.Status);
            //Console.WriteLine("FROMNAME: " + e.Header.From.FromName);
            //Console.WriteLine("REASON: " + e.ReasonPhrase);
            //Console.WriteLine(e.Header.ToString());
            //Console.WriteLine("-----------------------------");
            //Console.WriteLine(e.Body != null ? e.Body.ToString() : "EMPTY");
            //Console.WriteLine("============================= RESPONSE END ==============================================");
            return Task.FromResult(0);
        }

        private Task SIPTransportRequestReceived(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPRequest sipRequest)
        {


            SipRequestReseived?.Invoke(this, sipRequest);

            if (sipRequest.Method == SIPMethodsEnum.INFO)
            {
               // logger.LogDebug("SIP " + sipRequest.Method + " request received but no processing has been set up for it, rejecting.");
                SIPResponse notAllowedResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.MethodNotAllowed, null);
                return SIPTransport.SendResponseAsync(notAllowedResponse);
            }
            else if (sipRequest.Header.From != null &&
                sipRequest.Header.From.FromTag != null &&
                sipRequest.Header.To != null &&
                sipRequest.Header.To.ToTag != null)
            {
                // This is an in-dialog request that will be handled directly by a user agent instance.
            }
            else if (sipRequest.Method == SIPMethodsEnum.INVITE)
            {
                bool? callAccepted = IncomingCall?.Invoke(sipRequest);

                if (callAccepted == false)
                {
                    // All user agents were already on a call return a busy response.
                    UASInviteTransaction uasTransaction = new UASInviteTransaction(SIPTransport, sipRequest, null);
                    SIPResponse busyResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.BusyHere, null);
                    uasTransaction.SendFinalResponse(busyResponse);
                }
            }
            else
            {
               // logger.LogDebug("SIP " + sipRequest.Method + " request received but no processing has been set up for it, rejecting.");
                SIPResponse notAllowedResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.MethodNotAllowed, null);
                return SIPTransport.SendResponseAsync(notAllowedResponse);
            }

            return Task.FromResult(0);
        }

        private void SIPRequestInTraceEvent(SIPEndPoint localEP, SIPEndPoint remoteEP, SIPRequest e)
        {
            //logger.LogDebug($"Request Received {localEP}<-{remoteEP}: {sipRequest.StatusLine}.");

            //if (_homerSIPClient != null)
            //{
            //    var hepBuffer = HepPacket.GetBytes(remoteEP, localEP, DateTime.Now, 333, "myHep", sipRequest.ToString());
            //    _homerSIPClient.SendAsync(hepBuffer, hepBuffer.Length, HOMER_SERVER_ADDRESS, HOMER_SERVER_PORT);
            //}
            if (e.Method == SIPMethodsEnum.OPTIONS) return;
            SipRequestReseived?.Invoke(this, e);
            //Console.WriteLine("=============================REQIN");
            //Console.WriteLine(e.Method);
            //Console.WriteLine(e.Header.ToString());
            //Console.WriteLine("-----------------------------");
            //Console.WriteLine(e.Body != null ? e.Body.ToString() : "EMPTY");
            //Console.WriteLine("=============================");
        }

        private void SIPRequestOutTraceEvent(SIPEndPoint localEP, SIPEndPoint remoteEP, SIPRequest e)
        {
            // logger.LogDebug($"Request Sent {localEP}<-{remoteEP}: {sipRequest.StatusLine}.");

            //if (_homerSIPClient != null)
            //{
            //    var hepBuffer = HepPacket.GetBytes(localEP, remoteEP, DateTime.Now, 333, "myHep", sipRequest.ToString());
            //    _homerSIPClient.SendAsync(hepBuffer, hepBuffer.Length, HOMER_SERVER_ADDRESS, HOMER_SERVER_PORT);
            //}
            //Console.WriteLine("============================= REQOUT");
            //Console.WriteLine(e.Method);
            //Console.WriteLine(e.Header.ToString());
            //Console.WriteLine("-----------------------------");
            //Console.WriteLine(e.Body != null ? e.Body.ToString() : "EMPTY");
            //Console.WriteLine("=============================");
        }

        private void SIPResponseInTraceEvent(SIPEndPoint localEP, SIPEndPoint remoteEP, SIPResponse e)
        {
            // logger.LogDebug($"Response Received {localEP}<-{remoteEP}: {sipResponse.ShortDescription}.");

            //if (_homerSIPClient != null)
            //{
            //    var hepBuffer = HepPacket.GetBytes(remoteEP, localEP, DateTime.Now, 333, "myHep", sipResponse.ToString());
            //    _homerSIPClient.SendAsync(hepBuffer, hepBuffer.Length, HOMER_SERVER_ADDRESS, HOMER_SERVER_PORT);
            //}
            SipResponseReseivedz?.Invoke(this, e);
            //Console.WriteLine("============================= RESPONSE RECEIVED =========================================");
            //Console.WriteLine("STATUS: " + e.Status);
            //Console.WriteLine("FROMNAME: " + e.Header.From.FromName);
            //Console.WriteLine("REASON: " + e.ReasonPhrase);
            //Console.WriteLine(e.Header.ToString());
            //Console.WriteLine("-----------------------------");
            //Console.WriteLine(e.Body != null ? e.Body.ToString() : "EMPTY");
            //Console.WriteLine("=============================");
        }

        private void SIPResponseOutTraceEvent(SIPEndPoint localEP, SIPEndPoint remoteEP, SIPResponse e)
        {
            //logger.LogDebug($"Response Sent {localEP}<-{remoteEP}: {sipResponse.ShortDescription}.");

            //if (_homerSIPClient != null)
            //{
            //    var hepBuffer = HepPacket.GetBytes(localEP, remoteEP, DateTime.Now, 333, "myHep", sipResponse.ToString());
            //    _homerSIPClient.SendAsync(hepBuffer, hepBuffer.Length, HOMER_SERVER_ADDRESS, HOMER_SERVER_PORT);
            //}
            SipResponseReseivedz?.Invoke(this, e);
            if (e.ReasonPhrase == "MethodNotAllowed") return;
            //Console.WriteLine("============================= RESPONSE RECEIVED =========================================");
            //Console.WriteLine("STATUS: " + e.Status);
            //Console.WriteLine("FROMNAME: " + e.Header.From.FromName);
            //Console.WriteLine("REASON: " + e.ReasonPhrase);
            //Console.WriteLine(e.Header.ToString());
            //Console.WriteLine("-----------------------------");
            //Console.WriteLine(e.Body != null ? e.Body.ToString() : "EMPTY");
            //Console.WriteLine("=============================");
        }

    }
}
