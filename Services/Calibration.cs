using NewAPP2.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewAPP.Services
{
    public class Calibration : ICalibration
    {
        public async Task CalibZero(string ip, int port, byte terminalAddress)
        {
            await Task.Run(( ) =>
            {
                try
                {
                    using(TcpClient client = new TcpClient())
                    {
                        client.Connect(ip, port);
                        using (NetworkStream stream = client.GetStream())
                        {
                            
                            //заглушка
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    // Логируем детальную информацию
                    string errorMessage = $"Ошибка калибровки нуля: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        errorMessage += $" Inner: {ex.InnerException.Message}";
                    }

                    // Пробрасываем с понятным сообщением
                    throw new Exception(errorMessage, ex);
                }
            });
        }
        public async Task CalibWeight ( string ip, int port, byte terminalAddress, int Weight)
        {
            await Task.Run(( ) =>
            {
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        client.Connect(ip, port);
                        using (NetworkStream stream = client.GetStream())
                        {
                            
                           //заглушка
                        }

                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            });
        }
    }
}
