using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Shrinerain.AutoTester.Core
{
    public sealed class SocketWrapper : IDisposable
    {
        #region Fields

        private string _hostName;
        private int _port;
        private bool _isConnected;
        private TcpClient _client;
        private StreamReader _messageReader;
        private StreamWriter _messageWriter;

        //thread to check new message;
        private Thread _notifyThread;

        public delegate void MessageHandler(string msg);
        public event MessageHandler OnNewMessage;
        #endregion

        #region Properties

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        #endregion

        #region Constructor

        public SocketWrapper(string hostname, int port)
        {
            _hostName = hostname;
            _port = port;
        }

        #endregion

        #region Methods
        public void Connect()
        {
            if (_isConnected)
                return;
            try
            {
                _client = new TcpClient();
                _client.Connect(_hostName, _port);
                _messageReader = new StreamReader(_client.GetStream());
                _messageWriter = new StreamWriter(_client.GetStream());
                _isConnected = true;
            }
            catch
            {
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                if (_isConnected)
                {
                    _messageWriter.WriteLine(message);
                    _messageWriter.Flush();
                }
            }
            catch
            {
            }
        }

        private string ReadMessage()
        {
            if (_isConnected)
            {
                return _messageReader.ReadLine();
            }

            return null;
        }

        public void Notify()
        {
            if (_notifyThread == null)
            {
                _notifyThread = new Thread(new ThreadStart(CheckMessage));
                _notifyThread.Start();
            }
        }

        private void CheckMessage()
        {
            string newMsg;
            while ((newMsg = ReadMessage()) != null)
            {
                if (OnNewMessage != null)
                {
                    OnNewMessage(newMsg);
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                if (!_isConnected || _client == null)
                    return;
                if (_client.Connected)
                {
                    _client.Close();
                }
                _messageReader.Close();
                _messageReader = null;
                _messageWriter.Close();
                _messageWriter = null;
                _client = null;
            }
            finally
            {
                _isConnected = false;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Disconnect();
        }

        #endregion
    }
}
