using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespectPhone.SVOIP
{
    public class RXLog : ILogger
    {
        public static RXLog _inc;
        public RXLog()
        {
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public static ILogger Logger { get { 
                if (_inc == null) 
                    _inc = new RXLog(); 
                return _inc; 
            } set { } }

        public bool IsEnabled(LogLevel logLevel)
        {
            //throw new NotImplementedException();
            return true; 
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //throw new NotImplementedException();
        }
    }
}
