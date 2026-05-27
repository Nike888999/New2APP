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
        //чтение веса
        public async Task<int> ReadWeightAsync ( string ip, int port, byte unitId, ushort registerAddress )
        {
            return await Task.Run(( ) =>
            {
                try
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        // Подключаемся с таймаутом 3 секунды
                        tcpClient.ReceiveTimeout = 3000;
                        tcpClient.SendTimeout = 3000;

                        // Пробуем подключиться
                        var result = tcpClient.BeginConnect(ip, port, null, null);
                        bool connected = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

                        if (!connected || !tcpClient.Connected)
                        {
                            throw new Exception($"Не удалось подключиться к {ip}:{port}");
                        }

                        tcpClient.EndConnect(result);

                        NetworkStream stream = tcpClient.GetStream();
                        stream.ReadTimeout = 3000;

                        // MODBUS запрос (функция 03 - чтение регистров)
                        byte[] request = new byte[12];

                        // Заголовок MODBUS TCP
                        request[0] = 0x00; // Transaction ID
                        request[1] = 0x01;
                        request[2] = 0x00; // Protocol ID
                        request[3] = 0x00;
                        request[4] = 0x00; // Length
                        request[5] = 0x06;

                        // Данные MODBUS
                        request[6] = unitId;        // Адрес устройства
                        request[7] = 0x03;          // Функция: чтение регистров
                        request[8] = (byte)(registerAddress >> 8);    // Адрес регистра (старший байт)
                        request[9] = (byte)(registerAddress & 0xFF);  // Адрес регистра (младший байт)
                        request[10] = 0x00;         // Количество регистров (старший байт)
                        request[11] = 0x02;         // Количество регистров (младший байт) = 2 регистра = 4 байта

                        // Отправляем запрос
                        stream.Write(request, 0, request.Length);

                        // Читаем ответ
                        byte[] header = new byte[7];
                        int bytesRead = stream.Read(header, 0, 7);

                        //if (bytesRead < 7) throw new Exception("Неполный ответ");

                        // Длина данных
                        int dataLength = header[4] << 8 | header[5];

                        // Читаем данные
                        byte[] data = new byte[dataLength];
                        bytesRead = stream.Read(data, 0, dataLength);

                        // Проверяем функцию
                        if (data[0] != 0x03) throw new Exception($"Ошибка функции: {data[0]}");

                        // Проверяем количество байт
                        if (data[1] != 0x04) throw new Exception($"Ошибка длины: {data[1]}");

                        // Парсим 32-битное значение (4 байта)
                        int rawValue = (int)((data[2] << 24) | (data[3] << 16) | (data[4] << 8) | data[5]);

                        // Преобразуем в килограммы (предполагаем что устройство отдает в граммах)
                        int weight = rawValue;

                        stream.Close();
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
        //метод обнуления терминала 
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
                            0x00, 0x04, //номер транзакции
                            0x00, 0x00, //протокол Modbus TCP
                            0x00, 0x06, //длина данных
                            0x01, //адресс устройства
                            0x10, //запись нескольких регистров
                            0x27, 0x12, // регистровый адрес, 2712 - 10002
                            0x00, 0x01, //1 регистр
                            0x02, // два байта данных
                            0x00, terminalAddress //старший байт, и младший байт это адрес терминала
                        };
                        stream.Write(SelectTerminal, 0, SelectTerminal.Length); //отправка массива в поток

                        byte[] AnswerTerminal = new byte[32];
                        int read = stream.Read(AnswerTerminal, 0, AnswerTerminal.Length); //получение ответа от терминала и его длина
                        //ответ читать так: BitConverter.ToString(AnswerTerminal, 0, read)

                        //обнуление
                        byte[] ZeroCom = new byte[]
                        {
                            0x00, 0x05, //номер транзакции
                            0x00, 0x00, //модбас протокол
                            0x00, 0x06, //длина данных
                            0x01, //адресс устройства
                            0x10, //запись нескольких регистров
                            0x27, 0x21, //это адресс 2721 либо из хекс 10017
                            0x00, 0x01, //это 1 регистр
                            0x02, //два байта данных
                            0x00, 0x01 //команда на обнуление/установка нуля
                        };
                        stream.Write(ZeroCom, 0, ZeroCom.Length); //отправка команды на внесение в поток данных, где zeroCom это массив для отправки(данные), 0 начало, и отправляем весь массив ZeroCom.
                        AnswerTerminal = new byte[32];
                        read = stream.Read(AnswerTerminal, 0, AnswerTerminal.Length);
                        //ответ читать так: BitConverter.ToString(AnswerTerminal, 0, read)

                        //можно так же проверить вес актуальный, но наверное не надо.

                        //    byte[] readWeight = new byte[]
                        //{
                        //0x00, 0x06, 0x00, 0x00, 0x00, 0x06, 0x01, 0x03,
                        //0x00, 0x1E,       // 30
                        //0x00, 0x02
                        //};

                        //    stream.Write(readWeight, 0, readWeight.Length);
                        //    resp = new byte[64];
                        //    read = stream.Read(resp, 0, resp.Length);

                        //    if (read >= 13)
                        //    {
                        //        int weight = (resp[9] << 24) | (resp[10] << 16) | (resp[11] << 8) | resp[12];
                        //        Console.WriteLine($"⚖️ Вес после TARE: {weight}");
                        //    }

                        //}

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

