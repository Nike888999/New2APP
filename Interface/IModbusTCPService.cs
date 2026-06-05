using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NewAPP2.Interface
{
    public interface IModbusTCPService //для заглушки
    {
        public Task<int> ReadWeightAsync ( string ip, int port, byte unitId, ushort registerAddress );
        public Task<bool> TestConnectionAsync ( string ip, int port );

        //метод обнуления терминала 
        public Task<bool> TareAfterExternal ( string ip, int port, byte terminalAddress );


        public Task<int> tcpGmt ( string ip, int port );

    }
}
