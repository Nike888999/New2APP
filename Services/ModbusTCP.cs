using NewAPP2.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NewAPP
{
    public class ModbusTCP : IModbusTCPService
    {
        //чтение веса /заглушка
        public async Task<int> ReadWeightAsync ( string ip, int port, byte unitId, ushort registerAddress )
        {
            return await Task.Run(( ) =>
            {
                try
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        // Подключаемся с таймаутом 3 секунды
                        int weight = 0;
                        return weight;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"MODBUS ошибка: {ex.Message}");
                }
            });
        }

        // Метод для проверки подключения
        public async Task<bool> TestConnectionAsync ( string ip, int port )
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    var connectTask = tcpClient.ConnectAsync(ip, port);
                    var timeoutTask = Task.Delay(3000);

                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                    return completedTask != timeoutTask && tcpClient.Connected;
                }
            }
            catch
            {
                return false;
            }
        }
        //метод обнуления терминала заглушка
        public async Task<bool> TareAfterExternal ( string ip, int port, byte terminalAddress )
        {
            try
            {
                using (TcpClient tcp = new TcpClient())
                {
                    tcp.Connect(ip, port);
                    using (NetworkStream stream = tcp.GetStream())
                    {
                        //выбор терминала
                        byte[] SelectTerminal = new byte[]
                        {
                            
                        };
                        stream.Write(SelectTerminal, 0, SelectTerminal.Length); //отправка массива в поток

                        byte[] AnswerTerminal = new byte[32];
                        int read = stream.Read(AnswerTerminal, 0, AnswerTerminal.Length); //получение ответа от терминала и его длина
                        

                        //обнуление
                        byte[] ZeroCom = new byte[]
                        {
                          
                        };
                        stream.Write(ZeroCom, 0, ZeroCom.Length); //отправка команды на внесение в поток данных, где zeroCom это массив для отправки(данные)
                        AnswerTerminal = new byte[32];
                        read = stream.Read(AnswerTerminal, 0, AnswerTerminal.Length);
                        

                        return read > 0;

                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<int> tcpGmt(string ip, int port)
        {
            System.Diagnostics.Debug.WriteLine($"tcpGmt вызван с IP={ip}, Port={port}");
            using (TcpClient tcp = new TcpClient())
            {
                tcp.Connect(ip, port);
                using (NetworkStream stream = tcp.GetStream())
                {

                    byte[] tcpGmt = new byte[12];
                    tcpGmt[0] = 0x00;
                    tcpGmt[1] = 0x01;
                    tcpGmt[2] = 0x00;
                    tcpGmt[3] = 0x00;
                    tcpGmt[4] = 0x00;
                    tcpGmt[5] = 0x06;
                    tcpGmt[6] = 0x01;
                    tcpGmt[7] = 0x03;
                    tcpGmt[8] = 0x00;
                    tcpGmt[9] = 0x00;
                    tcpGmt[10] = 0x00;
                    tcpGmt[11] = 0x02;

                    stream.Write(tcpGmt, 0, tcpGmt.Length);

                    byte[] answer = new byte[36];
                    stream.Read(answer, 0, answer.Length);

                    int weight = (answer[9] << 24) | (answer[10] << 16) | (answer[11] << 8) | answer[12];
                    return weight;
                }
            }
        }
    }
}

