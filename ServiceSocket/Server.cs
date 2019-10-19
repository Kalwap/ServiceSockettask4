using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClassLibtask1;
using Newtonsoft.Json;


namespace ServiceSocket

{
    class Server
    {
        private static readonly List<Book> books = new List<Book>()
        {
            new Book("Harry Potter","J.K.Rowling",435,"1234567891235"),
            new Book("Wiedzmin","A.Sapkowski",234,"5432167891235"),
            new Book("Koszmar","D.Pieniek",435,"9876543213215"),
            new Book("Faramon","T.Okon",332,"4567891234567"),
            new Book("Akustyka","O.Kasztan",444,"2345678912345")

        };


        public void ServerStart()
        {
            TcpListener SrvSocket = null;
            try
            {
                int NumberofClient = 0;
                Console.WriteLine("welcome to server");
                SrvSocket = new TcpListener(IPAddress.Loopback, 8000);
                SrvSocket.Start();
                while (true)
                {
                    Console.WriteLine("Connection is pending");
                    TcpClient Client = SrvSocket.AcceptTcpClient();
                    Console.WriteLine("Server is on");
                    Task.Run(() => StreamHandling(Client, ref NumberofClient));
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Socket Exception: {e}");
            }
            finally
            {
                SrvSocket.Stop();
            }
            Console.WriteLine("Type for any button to continue");
            Console.Read();
        }



        public void StreamHandling(TcpClient client, ref int clientNumber)
        {
            Byte[] bytes = new byte[256];
            String data = null;
            clientNumber++;

            NetworkStream stream = client.GetStream();
            int i;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = Encoding.UTF8.GetString(bytes, 0, i).Trim();
                Console.WriteLine($"Received: {data} from client {clientNumber}");

                string msg = "Not a valid command. Try again!";
                string[] words = data.ToLower().Split();
                if (words[0] == "getall")
                {
                    msg = JsonConvert.SerializeObject(books);
                }
                if (words[0] == "get")
                {
                    msg = JsonConvert.SerializeObject(books.Find(e => e.Lsbn13 == words[1]));
                }
                if (words[0] == "save")
                {
                    string myjson = data.Split("{")[1].Split("}")[0];
                    myjson = "{" + myjson + "}";
                    books.Add(JsonConvert.DeserializeObject<Book>(myjson));
                    msg = "";
                }

                byte[] msge = Encoding.ASCII.GetBytes(msg);
                Thread.Sleep(1000);
                stream.Write(msge, 0, msge.Length);
                Console.WriteLine($"Sent: {msg}");
            }

            client.Close();
            clientNumber--;

        }





    }
}