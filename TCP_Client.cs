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
            //port
            int nomer = 0;
            //time:ms
            int time = 200;
            
            // Создаем новый экземпляр TcpClient
            TcpClient newClient = new TcpClient();

            // Соединяемся с хостом
            newClient.Connect(server,port);
            Console.WriteLine("[ Подключено! Для правильного завершения соединения нажми esc!!! ]");
            
            // Получаем поток для чтения и записи данных.
            NetworkStream tcpStream = newClient.GetStream();
            
            do
            {
            // Цикл выполняется до тех пор, пока не будет нажат esc
               
            cki = Console.ReadKey(); 
            if (cki.Key == ConsoleKey.Escape) { break; }
            if (cki.Key == ConsoleKey.A) { nomer = 0;} //red
            if (cki.Key == ConsoleKey.S) { nomer = 1;} //blue
            if (cki.Key == ConsoleKey.D) { nomer = 2;} //green
            if (cki.Key == ConsoleKey.F) { nomer = 3;} //yellow
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
            Console.Write("Server: {0}",Encoding.ASCII.GetString(bytes, 0, bytesRead));
            
            } while (cki.Key != ConsoleKey.Escape);
            // Закрываем TCP соединение.    
            tcpStream.Close();
            newClient.Close();
            }

            // ошибки-исключения для сокетов
            catch (Exception ex)
            {Console.WriteLine(ex.Message);
            Console.WriteLine("Подключись к VZDRYZHNI, если не подключает, то по ssh: sudo /etc/init.d/pigpiod restart");
            Console.ReadLine();
            }
        }
         
    }
}
