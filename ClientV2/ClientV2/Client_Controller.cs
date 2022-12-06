using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ClientV2
{
    internal class Client_Controller
    {
        private readonly byte _lastToServerCommand = 127;
        private readonly string _strBreakPoint = "<EOF>";
        Client client = null;
        byte CurComand;
        byte[] Buffer;
        private string _SavePath;
        readonly int _port;

        public Client_Controller(string savePath = @"D:\test", int port = 1045)
        {
            _port = port;
            
            _SavePath = savePath;
            try
            {
                Reconnect();
            }
            catch(Exception ex)
            {
                throw new Exception("Constructor controller error,savePath or socket value is wrong, ERROR DATAILS: \n" + ex.ToString());
            }
        }

        public void Reconnect()
        {
            
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, _port);

            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(remoteEP);

            Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

            client = new MyClient(sender, _SavePath);
        }
        private void SanityCheck()
        {
            if (client == null || !client.IsConnected() )
            {
                Reconnect();
            }
        }
        public void ContinueGuessing(int milisecs)
        {
            SanityCheck();
            GuesingFor(milisecs);
            AskGuessingResult();
        }
        public void LaunchGuessing(int milisecs) 
        {
            SanityCheck();

            InitializeGuessing();
            GuesingFor(milisecs);
            AskGuessingResult();
        }

        public void AskNewSecretNumber()
        {
            SanityCheck(); 
            //send requsest
            //receive accept

            InitializeGuessing();
            Console.WriteLine("Secret number changed");

           
        }
        public void AskSecretNumber()
        {
            SanityCheck();
            //ask
            //recieve
            //print

            client.Send(Commands.ToServer_hiddenNumber);
            MyReceive();
            if (Buffer[0] == (byte)Commands.ToClient_hiddenNumber)
            {
                Console.WriteLine("Hidden Number is {0}", bufferToInt());
            }
            else
            {
                throw new Exception("SERVER SEND WRONG COMMAND");
            }
            
        }
        public void AskWho() 
        {
            SanityCheck();

            //send request
            //recieve
            //print

            client.Send(Commands.ToServer_who);
            MyReceive();
            if (Buffer[0] == (byte)Commands.ToClient_who)
            {
                Console.WriteLine(BufferToStr());
            }
            else
            {
                throw new Exception("ASK WHO ERROR: server had to send ToClient_who command");
            }
        }

        private void AskGuessingResult()
        {
            SanityCheck();

            if (Buffer[0] != (byte)Commands.ToClient_guessingResult)
            {
                Buffer = new byte[1024]; 
                client.Send(Commands.ToServer_guessingResult);
                MyReceive();
            }
            if (Buffer[0] == (byte)Commands.ToClient_guessingResult)
            {
                Console.WriteLine(BufferToStr());
            }else
            {
                throw new Exception("WRONG COMMAND WHEN IT HAVE TO BE ToClient_guessingResult");
            }
        }

        private void InitializeGuessing()
        {
            SanityCheck();

            client.Send(Commands.ToServer_initializeGuessing);

            MyReceive();
            CurComand = Buffer[0];
            if (CurComand == (byte)Commands.ToClient_guesserInitialized)
            {
                Console.WriteLine("Succesfuly Inisialized");
            }
            else
            {
                throw new Exception("Server did not answer //InisializeGuessing");
            }
        }
        public void GuesingFor(int givenMilisecs)
        {
            SanityCheck();

            Stopwatch timer = new Stopwatch();
            Random r = new Random();
            Buffer = new byte[1024];
            
            timer.Start();
            while (timer.ElapsedMilliseconds < givenMilisecs)
            {
                client.Send(Commands.ToServer_tryGuess, r.Next(0,2000000));
                MyReceive();


                if (Buffer[0] == (byte)Commands.ToClient_confirmed)
                {
                    continue;

                }else if (Buffer[0] == (byte)Commands.ToClient_guessingResult)
                {
                    break;
                }
            }

        }
        private string BufferToStr()
        {

            string msg;

            msg = Encoding.ASCII.GetString(Buffer);
            msg = msg.Remove(msg.IndexOf(_strBreakPoint)).Remove(0, 1);

            client.LogValue(((Commands)Buffer[0]).ToString(), msg);

            return msg;
        }
        private int bufferToInt()
        {
            int fromBuffer = BitConverter.ToInt32(Buffer, 1);
            client.LogValue(((Commands)Buffer[0]).ToString(),fromBuffer.ToString());
            return fromBuffer;
        }
        private void MyReceive()
        {
            Buffer = client.Receive();
        }

        
    }

    enum Commands : byte
    {
        ToServer_initializeGuessing,
        ToServer_tryGuess,
        ToServer_guessingResult,
        ToServer_hiddenNumber,
        ToServer_who,
        ToServer_abortConnection,

        ToClient_guesserInitialized = 128,
        ToClient_confirmed = 129,
        ToClient_guessingResult = 130,
        ToClient_hiddenNumber = 131,
        ToClient_who = 132,

        ToClient_unknownCommand = 254,
        ToClient_unacceptableCommand = 255,
    }
}
