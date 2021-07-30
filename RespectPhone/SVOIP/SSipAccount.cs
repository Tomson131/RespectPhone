using SIPSorcery.SIP.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTEST
{
    class SSipAccount : ISIPAccount
    {
      
            public string ID { get; set; } = "100";

            public string SIPUsername { get; set; } = "100";

            public string SIPPassword { get; set; } = "123";

            public string HA1Digest { get; set; } = "";

            public string SIPDomain { get; set; } = "aster.example.ru";

            public bool IsDisabled { get; set; } = false;

        public SSipAccount(string sIPUsername, string sIPPassword, string sIPDomain)
        {
            ID = sIPUsername;
            SIPUsername = sIPUsername;
            SIPPassword = sIPPassword;
            SIPDomain = sIPDomain;
        }
    }
}
