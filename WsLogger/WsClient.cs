using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WebSocket4Net;
using SuperSocket.ClientEngine;
using Newtonsoft.Json;

namespace WsLogger
{
    public delegate void SocketOpened(object sender, EventArgs e);
    public delegate void SocketClosed(object sender, EventArgs e);
    public delegate void MessageReceived(object sender, MessageReceivedEventArgs e);

    class WsClient
    {
        WebSocket m_Websocket;

        public event EventHandler Opened;
        public event EventHandler Closed;
        public event EventHandler<MessageReceivedEventArgs> Message;

        public WsClient()
        {
        }

        /// <summary>
        /// Create a new websocket instance and connect to it.
        /// </summary>
        /// <param name="url">The full URL of the websocket including "ws://" and the port (e.g. ":443")</param>
        public void Connect(String url)
        {
                m_Websocket = new WebSocket(url);
                m_Websocket.Opened += new EventHandler(websocket_Opened);
                m_Websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
                m_Websocket.Closed += new EventHandler(websocket_Closed);
                m_Websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
                m_Websocket.Open();
        }

        /// <summary>
        /// Disconnect from websocket.
        /// </summary>
        public void Disconnect()
        {
            if (m_Websocket.State == WebSocketState.Open)
            {
                m_Websocket.Close();
            }
        }

        /// <summary>
        /// The current state of the websocket connection
        /// </summary>
        public WebSocketState State
        {
            get
            {
                if (m_Websocket == null)
                {
                    return WebSocketState.None;
                }
                else
                {
                    return m_Websocket.State;
                }
            }
        }

        /// <summary>
        /// Send a string to the websocket connection
        /// </summary>
        /// <param name="str">The content of the string (for GoFree a correct JSON string is required)</param>
        public void Send(String str)
        {
            m_Websocket.Send(str);
        }

        /// <summary>
        /// Called when the websocket opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void websocket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Open");
            if (Opened != null)
                Opened(this, new EventArgs());
        }

        /// <summary>
        /// Called on websocket error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void websocket_Error(object sender, EventArgs e)
        {
            Console.WriteLine("Error");
        }

        /// <summary>
        /// Called when websocket closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void websocket_Closed(object sender, EventArgs e)
        {
            //btn_Connect.Enabled = true;
            //btn_Disconnect.Enabled = false;
            Console.WriteLine("Closed");

            if (Closed != null)
                Closed(this, new EventArgs());
        }

        /// <summary>
        /// Called on message received. Emits Message(this, e)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (Message != null)
            {
                Message(this, e);
            }
        }
    }
}
