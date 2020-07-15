using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ChatClient
{
    class Program
    {
        static string userName;
        private const string host = "172.24.188.21";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        private static List<NetworkStream> threadsTest = new List<NetworkStream>();


        static void Main(string[] args)
        {
            Console.Write("Enter your name: ");
            userName = Console.ReadLine();

            client = new TcpClient();
            try
            {
                client.Connect(host, port); //client connection
                //stream = client.GetStream(); // get stream
                threadsTest.Add(client.GetStream()); //threadsTest[0]

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                threadsTest[0].Write(data, 0, data.Length);

                // create new stream for receiving the data
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //start the stream
                Console.WriteLine("Welcome, {0}", userName);
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        static void SendMessage()
        {
            Console.WriteLine("Enter your message: ");

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                threadsTest[0].Write(data, 0, data.Length);
            }
        }
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // buffer for receiving data
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = threadsTest[0].Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (threadsTest[0].DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);//print the message
                }
                catch
                {
                    Console.WriteLine("Connection failed!"); 
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (threadsTest[0] != null)
                threadsTest[0].Close();//disconnect the stream
            if (client != null)
                client.Close();//disconnect the client
            Environment.Exit(0); //end process
        }
    }
}
