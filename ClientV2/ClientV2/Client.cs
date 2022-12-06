using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientV2
{
    internal abstract class Client
    {
        
        public abstract bool IsConnected();

        public abstract void Send(Commands command, int toSendInt);

        public abstract void Send(Commands command);
        //public abstract void ShutDownConnection();
        public abstract byte[] Receive();
        public abstract void LogValue(string commandToLog, string valueToLog);
    }
     

    internal class MyClient: Client
    {
        Socket Connection = null;
        byte[] Buffer = new byte[1024];
        private LogMachine ClientLogMachine;
        public MyClient(Socket connection, string SavePath)
        {
            this.Connection = connection;
            ClientLogMachine = new LogMachine(SavePath);

        }
        public override void Send(Commands command, int toSendInt)
        {
            Buffer = new byte[1 + sizeof(int)];

            Buffer[0] = (byte)command;
            int i = 1;
            foreach (byte b in BitConverter.GetBytes(toSendInt))
            {
                Buffer[i] = b;
                i++;
            }

            Connection.Send(Buffer);
            ClientLogMachine.ToLog(command.ToString(), toSendInt);
        }
        public override bool IsConnected()
        {   if(Connection != null)
            {
                return Connection.Connected;
            }
            else
            {
                return false;
            } ;
        }
        public override void Send(Commands command)
        {
            Buffer = new byte[1];
            Buffer[0] = (byte)command;

            Connection.Send(Buffer);
           ClientLogMachine.ToLog(command.ToString());
        }

        public override byte[] Receive()
        {
            Buffer = new byte[1024];

            Connection.Receive(Buffer);

            LogValue( ((Commands)Buffer[0]).ToString() );

            return Buffer;
        
        }
        public override void LogValue(string commandToLog, string valueToLog=null)
        {   
            if(valueToLog == null)
            {
                ClientLogMachine.ToLog(commandToLog);
            }
            else
            {
                ClientLogMachine.ToLog(commandToLog, valueToLog);
            }
        }
    }
}
