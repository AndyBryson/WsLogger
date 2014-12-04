/*
 * Copyright (c) 2013 Andy Bryson (andy.bryson@navico.com) - all rights reserved.
 * This code is released under the GPLv2 and MIT licenses. Pick the one you like.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WebSocketSharp;

namespace WsLogger
{
    public delegate void SocketOpened (object sender,EventArgs e);

    public delegate void SocketClosed (object sender,EventArgs e);

    public delegate void MessageReceived (object sender,MessageEventArgs e);

    class WsClient
    {
        WebSocket m_Websocket;

        public event EventHandler Opened;
        public event EventHandler Closed;
        public event EventHandler<MessageEventArgs> Message;

        public WsClient ()
        {
        }

        ~WsClient ()
        {
            //Disconnect();
        }

        /// <summary>
        /// Create a new websocket instance and connect to it.
        /// </summary>
        /// <param name="url">The full URL of the websocket including "ws://" and the port (e.g. ":443")</param>
        public void Connect (String url)
        {
            m_Websocket = new WebSocket (url);
            m_Websocket.OnOpen += new EventHandler (websocket_Opened);
            m_Websocket.OnError += new EventHandler<ErrorEventArgs> (websocket_Error);
            m_Websocket.OnClose += new EventHandler<CloseEventArgs> (websocket_Closed);
            m_Websocket.OnMessage += new EventHandler<MessageEventArgs> (websocket_MessageReceived);
            m_Websocket.Connect ();
        }

        /// <summary>
        /// Disconnect from websocket.
        /// </summary>
        public void Disconnect ()
        {
            if (m_Websocket.ReadyState == WebSocketState.Open)
            {
                m_Websocket.Close (CloseStatusCode.Normal);
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
                    return WebSocketState.Closed;
                }
                return m_Websocket.ReadyState;
            }
        }

        /// <summary>
        /// Send a string to the websocket connection
        /// </summary>
        /// <param name="str">The content of the string (for GoFree a correct JSON string is required)</param>
        public void Send (String str)
        {
            m_Websocket.Send (str);
        }

        /// <summary>
        /// Called when the websocket opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void websocket_Opened (object sender, EventArgs e)
        {
            Console.WriteLine ("Open");
            if (Opened != null)
                Opened (this, new EventArgs ());
        }

        /// <summary>
        /// Called on websocket error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void websocket_Error (object sender, EventArgs e)
        {
            Console.WriteLine ("Error");
        }

        /// <summary>
        /// Called when websocket closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void websocket_Closed (object sender, EventArgs e)
        {
            //btn_Connect.Enabled = true;
            //btn_Disconnect.Enabled = false;
            Console.WriteLine ("Closed");

            if (Closed != null)
                Closed (this, new EventArgs ());
        }

        /// <summary>
        /// Called on message received. Emits Message(this, e)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void websocket_MessageReceived (object sender, MessageEventArgs e)
        {
            if (Message != null)
            {
                Message (this, e);
            }
        }
    }
}
