using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespectPhone
{
    class EnumsData
    {
    }

    public enum CallState
    {
          Cancelled,
          Busy,
          Completed,
          InCall,
          Ringing, 
          Answered
    }
    public enum RegState
    {

        RegistrationSucceeded,
        RegistrationFaild
    }
}
