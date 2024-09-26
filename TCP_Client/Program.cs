using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private const int Port = 8888;
    private const string ServerIp = "127.0.0.1";

    static void Main()
    {
        //създаване на клиета ip и порт
        TcpClient client = new TcpClient(ServerIp, Port);
        Console.WriteLine("Connected to server. Start chatting!");

        NetworkStream stream = client.GetStream();
        //създава се stream за получаване на информацията
        Thread receiveThread = new Thread(ReceiveMessages);
        //пускане на thred-a
        receiveThread.Start(stream);
        // цикъл за кодиране
        while (true)
        {
            //вземане на текст от конзолата
            string message = Console.ReadLine();
            //масив от байтове
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            //изпращане
            stream.Write(buffer, 0, buffer.Length);
        }
    }
 //метод за съобщенията
    static void ReceiveMessages(object obj)
    {
        //кастваме обекта към networkstream
        NetworkStream stream = (NetworkStream)obj;
        byte[] buffer = new byte[1024];
        int bytesRead;
        //безкраен loop за изпращане на съобщение
        while (true)
        {
            try
            {
                //вземане на съобщение
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                //проверяване дали има някой
                if (bytesRead == 0)
                {
                    break;
                }
                //стринг за съобщението
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
            }
            //проверка за грешка
            catch (Exception)
            {
                break;
            }
        }
    }
}
