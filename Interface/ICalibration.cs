using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NewAPP2.Interface
{
    public interface ICalibration
    {
       
            public Task CalibZero ( string ip, int port, byte terminalAddress );

            public Task CalibWeight ( string ip, int port, byte terminalAddress, int Weight );

        }
}
