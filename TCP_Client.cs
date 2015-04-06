using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// подключаем библиотеки для работы с сокетами
// http://stackoverflow.com/questions/1488486/why-does-binarywriter-prepend-gibberish-to-the-start-of-a-stream-how-do-you-avo 
// https://www.youtube.com/embed/0ug7gJc3Lts 
// http://professorweb.ru/my/csharp/web/level4/4_2.php
//
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TCP_Client
{
    class Program
    {
        public static void Main(string[] args)
        {

            Console.Write("[ Нажми Enter для соединения ]");
            Console.ReadLine();
            Console.WriteLine("[...:Подключаемся:...]");

            Connect("192.168.128.1", 6660);

        }

        public static void Connect(String server, Int32 port)
        {
            try
            {
                //Описывает нажатую клавишу консоли, включая символ, представленный этой клавишей, и состояние управляющих клавиш CTRL, SHIFT и ALT.
                ConsoleKeyInfo cki;
                // создаем переменную, для клавиш 
                ConsoleKey key = ConsoleKey.X;
                //port
                int nomer = 0;
                //time:ms
                int time = 400;

                // начальное время ожидания
                int countinterval = 0;
                // колво интервалов ожидающих пинг
                int numinterval = 300;

                // Создаем новый экземпляр TcpClient
                TcpClient newClient = new TcpClient();

                // Соединяемся с хостом
                newClient.Connect(server, port);
                Console.WriteLine("[ Подключено! Для завершениянажми esc!!!Black, Red, Blue, Green,, Yellow  ]");

                // Получаем поток для чтения и записи данных.
                NetworkStream tcpStream = newClient.GetStream();

                do
                {// Цикл выполняется до тех пор, пока не будет нажат esc

                    if (Console.KeyAvailable)
                    //Возвращает или задает значение, указывающее, доступно ли нажатие клавиши во входном потоке.
                    { //если время нажатия раньше количества интервалов для пинга
                        cki = Console.ReadKey();
                        key = cki.Key;
                        if (key == ConsoleKey.Escape) { break; }
                        if (key == ConsoleKey.A) { nomer = 0; } //red
                        if (key == ConsoleKey.S) { nomer = 1; } //blue
                        if (key == ConsoleKey.D) { nomer = 2; } //green
                        if (key == ConsoleKey.F) { nomer = 3; } //yellow
                        // стрингую enable n ms
                        string message = "enable " + nomer + " " + time + " \n";
                        // Переводим наше сообщение в UTF8, а затем в массив Byte, отправляем его на сервер
                        byte[] sendBytes = Encoding.UTF8.GetBytes(message);
                        tcpStream.Write(sendBytes, 0, sendBytes.Length);
                        Console.Write("\nSent: {0}", message);

                        // Получаем ответ сервера
                        byte[] bytes = new byte[newClient.ReceiveBufferSize];
                        int bytesRead = tcpStream.Read(bytes, 0, newClient.ReceiveBufferSize);
                        // Строка, содержащая ответ от сервера
                        Console.Write("Server: {0}", Encoding.ASCII.GetString(bytes, 0, bytesRead));
                        Thread.Sleep(200);
                        countinterval = 0;
                    }
                    // если больше времени ожидания, то пинг
                    if (countinterval >= numinterval)
                    {

                        //Console.WriteLine("pingуем");
                        string ping = "Ping \n";
                        // Переводим наше сообщение в UTF8, а затем в массив Byte, отправляем его на сервер
                        byte[] sendByte = Encoding.UTF8.GetBytes(ping);
                        tcpStream.Write(sendByte, 0, sendByte.Length);
                        Console.Write("\nSent: {0}", ping);

                        // Получаем ответ сервера
                        byte[] bytes = new byte[newClient.ReceiveBufferSize];
                        int bytesRead = tcpStream.Read(bytes, 0, newClient.ReceiveBufferSize);
                        // Строка, содержащая ответ от сервера
                        Console.WriteLine("Server: Pong"/*, Encoding.ASCII.GetString(bytes, 0, bytesRead)*/);
                        // Если 
                        countinterval = 0;
                    }
                    //плюсуем время ожидания
                    countinterval++;
                    Thread.Sleep(10); // Loop until input is entered.

                } while (key != ConsoleKey.Escape);
                // Закрываем TCP соединение.    
                tcpStream.Close();
                newClient.Close();
            }

            // ошибки-исключения для сокетов
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Подключись к RPI, если не подключает, то по ssh: sudo /etc/init.d/pigpiod restart");
                // reconecting
                Console.WriteLine("[...:Переподключаемся:...]");
                Connect("192.168.128.1", 6660);
            }
        }

    }
}
