using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    //лист от клиенти
    private static readonly List<TcpClient> clients = new List<TcpClient>();
    private const int Port = 8888;

    static void Main()
    {
        //създаване на съръвра
        TcpListener server = new TcpListener(IPAddress.Any, Port);
        //старт на сървъра
        server.Start();
        Console.WriteLine($"Server started on port {Port}");

        while (true)
        {
            //допускане на клиента до сървъра
            TcpClient client = server.AcceptTcpClient();
            //запис на клиента
            clients.Add(client);
            //създаване на thread за клиента
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }
//метод за подържане на връзката между клиенти
    static void HandleClient(object obj)
    {
        //каства се обект към tcp клиент
        TcpClient tcpClient = (TcpClient)obj;
        NetworkStream stream = tcpClient.GetStream();

        byte[] buffer = new byte[1024];
        int bytesRead;
        //безкраен цикъл
        while (true)
        {
            try
            {//проверка на съобщението
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }
                //преобразуват се битове в стринг
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");
                //изпраща се съобщението
                BroadcastMessage(tcpClient, message);
            }
            catch (Exception)
            {
                break;
            }
        }
        //премахване на клиента от листа
        clients.Remove(tcpClient);
        tcpClient.Close();
    }    
    //метод за изпращане на съобщението до всички

    static void BroadcastMessage(TcpClient sender, string message)
    {
        //масив от битоветев на съобщението
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message);
        //проверка за всички клиенти
        foreach (TcpClient client in clients)
        {
            //ако не е този който изпраща
            if (client != sender)
            {
                //да получи съобщението ако не го е изпратил той
                NetworkStream stream = client.GetStream();
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length);
            }
        }
    }
}
