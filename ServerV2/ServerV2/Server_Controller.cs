using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerV2
{
   

    internal  class  Server_Controller
    {
        private readonly byte _lastToServerCommand = 127;//
        private readonly string _who = "--CREDITS--\n Grapphic: Voloshyn Danil\n SystemLogic: Voloshyn Danil\nlove:Voloshyn Danil\nProebavDeadline: Voloshyn DAnil\n 20 'Vgaduvania number by MonterCarlo Method'";
        readonly string _SavePath;
        Server server = null;
        Guesser guesser = null ;
        LogMachine logMachine;
        byte[] received;
        public Server_Controller(string savePath = @"D:\test")
        {
            _SavePath = savePath;
            try 
            { 
                logMachine = new LogMachine(_SavePath);
                StartServer(); 
            }
            catch(Exception anyException) { throw new Exception("Controller constructor ERROR, savePath or socket value is wrong ERROR DATAILS:\n  " + anyException.ToString()); }
            


        }
        public void StartServer(int port = 1045)
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 1045);

                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);

                this.server = new MyServer(listener, logMachine);
            }
            catch(Exception e) { throw new Exception("SERVER NOT CREATED\nError:{0}"+ e.ToString()); }
        }
        public void ToLobby()
        {
            byte command;
            
            while (true)
            {
                try
                {
                    Console.WriteLine("In Lobby ");//clen after smt
                    received  = server.Receive();
                    
                    command = received[0];
                    Console.WriteLine("Received command: {0}", command); //clean 

                    if (command == (byte)Commands.ToServer_initializeGuessing)
                    {
                        ReceiveLog();
                        InitializeGuessing();
                        server.Send(Commands.ToClient_guesserInitialized);
                    }
                    else if (command == (byte)Commands.ToServer_tryGuess)
                    {
                        TryToGuess( );

                    }
                    else if (command == (byte)Commands.ToServer_guessingResult)
                    {
                        ReceiveLog();
                        SendGuessingResult();
                    }
                    else if (command == (byte)Commands.ToServer_hiddenNumber)
                    {
                        ReceiveLog();
                        SendHiddenNumber();
                    }
                    else if (command == (byte)Commands.ToServer_who)
                    {
                        ReceiveLog();
                        server.Send(Commands.ToClient_who, _who);
                    }
                    else if (command == (byte)Commands.ToServer_abortConnection)
                    {
                        ReceiveLog();
                        AbortConnection();
                    }
                    else if ((byte)command > _lastToServerCommand)
                    {
                        ReceiveLog();
                        Console.WriteLine("unacceptable  command is received");
                        server.Send(Commands.ToClient_unacceptableCommand);
                    }
                    else
                    {
                        ReceiveLog();
                        Console.WriteLine("unknown command is received");
                        server.Send(Commands.ToClient_unknownCommand);
                    }
                } catch (Exception e) { Console.WriteLine("Error inside Lobby:\n{0}", e.ToString()); }
            }
        }
        private void AbortConnection()
        {
            server.ShutDownConnection();
        }
        private void SendHiddenNumber()
        {
            server.Send(Commands.ToClient_hiddenNumber, guesser.GetHiddenNumber());
        }

        private void SendGuessingResult()
        {
            ReceiveLog(GetGuessingResult());
            server.Send(Commands.ToClient_guessingResult, GetGuessingResult());
        }
        private string GetGuessingResult()
        {
            string msg;
            if (guesser == null)
            {
                msg = "Never attempted to guess";
            }
            else
            {
                if (guesser.GetClosest() == guesser._neverGuess)
                {
                    msg = "There is no guessing res, there was no attemd to guess";
                }
                else if (guesser.GetClosest() == guesser.GetHiddenNumber())
                {
                    msg = "number was guessed, in took " + guesser.GetAttempts() + " tries ";
                }

                else
                {

                    msg = "the closest to target is " + guesser.GetClosest() + ", it's been  " + guesser.GetAttempts() + " tries";
                }


            }
            msg += "<EOF>";

            return msg;
        }

        private void InitializeGuessing()
        {
            guesser = new RandGuesser();
            
        }

        private void TryToGuess( )
        {
            if (guesser == null) {
                InitializeGuessing();
                TryToGuess();

                return;
            }

            if (guesser.IsGuessed())
            {
                ReceiveLog();
                SendGuessingResult();
                

                
                return;
            }

            int guess = BitConverter.ToInt32(received, 1);
            ReceiveLog(guess);

            if (guesser.IsGuessRight(guess))
            {
                SendGuessingResult();
            }
            else
            {
                server.Send(Commands.ToClient_confirmed);
            }
            
        }
        public void ReceiveLog()
        {
            ReceiveLog("null");
        }
        public void ReceiveLog(int value)
        {
            ReceiveLog(value.ToString());
        }
        public void ReceiveLog(string value)
        {
            string commandToLog = ((Commands)this.received[0]).ToString();
            if (value == null)
            {
                logMachine.ToLog(commandToLog);
            }
            else
            {
                logMachine.ToLog(commandToLog, value);
            }
        }

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

    ToClient_unknownCommand =254,
    ToClient_unacceptableCommand = 255,
}
