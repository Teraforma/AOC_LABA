using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerV2
{
    internal abstract class Server
    {
        public abstract void Reconnect();
        public abstract void Send(Commands command, int toSendInt);
        public abstract void Send(Commands command, string toSendStr);
        public abstract void Send(Commands command);
        public abstract void ShutDownConnection();
        public abstract byte[] Receive();
        public abstract void Makelog(string CommandToLog, string toLog);
    }


    internal class MyServer : Server
    {

        Socket Handler = null, Listener = null;
        byte[] Buffer = null;
        private readonly string _BreakPointStr = "<EOF>";
        LogMachine ServerLogs;

        public MyServer(Socket listener, LogMachine serverLogs)
        {
            this.Listener = listener;
            ServerLogs = serverLogs;
        }

        public bool IsConnected()
        {
            if (Handler == null)
            {
                return false;

            }else
            {
                return Handler.Connected;   
            }
        }
        public override void Reconnect()
        {
            Listener.Listen();

            Console.WriteLine("Waiting for a connection...");
            this.Handler = Listener.Accept();
            Console.WriteLine("connection established");
        }
        
        private void SanityCheck()
        {
            if (!IsConnected())
            {
                Reconnect();
            }
        }

       

        public override void Send(Commands command)
        {
            SanityCheck();

            Buffer = new byte[1];
            Buffer[0] = (byte)command;

            ServerLogs.ToLog(command.ToString());
            Handler.Send(Buffer);
        }
        public override void Send(Commands command, int toSendInt)
        {
            SanityCheck();

            Buffer = new byte[1 + sizeof(int)];

            Buffer[0] = (byte)command;
            int i = 1;
            foreach(byte b in BitConverter.GetBytes(toSendInt))
            {
                Buffer[i] = b;
                i++;
            }


            ServerLogs.ToLog(command.ToString(), toSendInt.ToString());
            Handler.Send(Buffer); 
        }
        public override void Send(Commands command, string toSendStr)
        {
            SanityCheck();

            toSendStr += _BreakPointStr;

            Buffer = new byte[1 + toSendStr.Length];

            Buffer[0] = (byte)command;
            int i = 1;
            foreach (byte b in Encoding.ASCII.GetBytes(toSendStr))
            {
                Buffer[i] = b;
                i++;
            }

            ServerLogs.ToLog(command.ToString(), toSendStr);
            Handler.Send(Buffer);
        }
        public override byte[] Receive()
        {
            SanityCheck();
            Buffer = new byte[1024];

            Handler.Receive(Buffer);

            return Buffer;
        }
        public override void ShutDownConnection()
        {
            Handler.Shutdown(SocketShutdown.Both);
            Handler.Close();
        }
        public override void Makelog(string commandToLog, string toLog)
        {
            if (toLog == "null")
            {

            }
        }
    }    
}

    






    

