/*
 * Copyright (c) 2013 Andy Bryson (andy.bryson@navico.com) - All rights reserved.
 * This code is released under the GPLv2 and MIT licenses. Pick the one you like.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using WebSocketSharp;

namespace WsLogger
{
    public delegate void PopulateTreeViewDelegate ();

    public delegate void MulticastReceived (object sender,String multicastMessage);

    public partial class Form1 : Form
    {
        // TODO:
        /*
         * Replace the Websocket implementation. There doesn't seem to be any really good .net implementations at the moment (hence the 50 data item limit).
         * Save log file names
         */
        public event EventHandler<MulticastMessageEventArgs> MulticastReceived;

        private TreeViewCancelEventHandler checkForCheckedChildren;

        #region Member Constants
        private const string m_cGroupAddress = "239.2.1.1";
        private const int m_cGroupPort = 2052;
        private const int m_cTimeId = 33;
        #endregion

        #region Members
        MessageHandler m_MessageHandler;
        bool m_Open = true;
        Int64 m_MessageCount = 0;
        float m_LogFileSize = 0;
        String m_LogFileSizeString = "0";
        Dictionary<String, NavicoJson.UnitServiceInfo> m_AnnouncedServers = new Dictionary<string, NavicoJson.UnitServiceInfo> ();
        Thread m_MulticastThread;
        #endregion

        public Form1 ()
        {
            InitializeComponent ();
            cb_IP.DropDownStyle = ComboBoxStyle.DropDown;

            // set up message handler
            m_MessageHandler = new MessageHandler ();
            m_MessageHandler.PopulationComplete += new EventHandler (OnPopulationComplete);
            m_MessageHandler.WsClient.Opened += new EventHandler (OnWsConnect);
            m_MessageHandler.WsClient.Closed += new EventHandler (OnWsDisconnect);
            m_MessageHandler.LoggingChanged += new EventHandler (OnLoggingChanged);
            m_MessageHandler.ValidMessageReceived += new EventHandler (OnMessageReceived);
            m_MessageHandler.WriteComplete += new EventHandler (OnWriteComplete);

            // set up Service Discovery thread
            m_MulticastThread = new Thread(new ThreadStart(ReceiveMulticast));
            m_MulticastThread.Start();
            this.MulticastReceived += new EventHandler<MulticastMessageEventArgs>(OnMulticastReceived);

            // event for giving the tree view better behavior.
            checkForCheckedChildren = new TreeViewCancelEventHandler (CheckForCheckedChildrenHandler);

            tv_DataItems.Sorted = true;
        }

        ~Form1 ()
        {
            m_Open = false;
            m_MessageHandler.PopulationComplete -= new EventHandler (OnPopulationComplete);
            m_MessageHandler.WsClient.Opened -= new EventHandler (OnWsConnect);
            m_MessageHandler.WsClient.Closed -= new EventHandler (OnWsDisconnect);
            m_MessageHandler.LoggingChanged -= new EventHandler (OnLoggingChanged);
            m_MessageHandler.ValidMessageReceived -= new EventHandler (OnMessageReceived);
            m_MessageHandler.WriteComplete -= new EventHandler (OnWriteComplete);
        }

        protected override void OnClosed (EventArgs e)
        {
            m_Open = false;
            base.OnClosed (e);
        }

        /// <summary>
        /// Connect button event handler. Try to connect to the websocket server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Connect_Click (object sender, EventArgs e)
        {
            if (m_MessageHandler.SocketState != WebSocketState.Open)
            {
                System.Net.IPAddress address;
                NavicoJson.UnitServiceInfo info = cb_IP.SelectedItem as NavicoJson.UnitServiceInfo;
                if (info != null) // if we're using a properly populated entry
                {
                    if (System.Net.IPAddress.TryParse (info.IP, out address))
                    {
                        String url = "ws://" + address.ToString () + ":" + info.WebsocketPort;
                        m_MessageHandler.Init (url);
                    }
                }
                else // has the user typed an IP address?
                {
                    if (System.Net.IPAddress.TryParse (cb_IP.Text, out address))
                    {
                        String url = "ws://" + address.ToString () + ":" + 2053;
                        m_MessageHandler.Init (url);
                    }
                }
            }
        }


        /// <summary>
        /// Disconnect button handler. Disconnect from the web server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Disconnect_Click (object sender, EventArgs e)
        {
            m_MessageHandler.Disconnect ();
        }

        /// <summary>
        /// Show save file dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SelectFile_Click (object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog ();
            dlg.Filter = "Log files (*.log)|*.log|All files (*.*)|*.*";
            if (dlg.ShowDialog () == DialogResult.OK)
            {
                txt_FileName.Text = dlg.FileName;
                m_MessageHandler.IsNewFile = true;
            }
        }

        /// <summary>
        /// Population of Data Items is complete. Add them to the TreeView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPopulationComplete (object sender, EventArgs e)
        {
            this.Invoke ((MethodInvoker)delegate()
            {
                PopulateTreeView ();
            }
            );
        }

        /// <summary>
        /// Websocket connection event. Disable/Enable buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWsConnect (object sender, EventArgs e)
        {
            m_MessageHandler.PopulateDataInfomation ();
            this.Invoke ((MethodInvoker)delegate()
            {
                btn_Connect.Enabled = false;
                cb_IP.Enabled = false;
                btn_Disconnect.Enabled = true;
                if (m_MessageHandler.Logging == false)
                {
                    btn_StartLogging.Enabled = true;
                }

                if (btn_StartLogging.Enabled == true)
                {
                    btn_StopLogging.Enabled = false;
                    tv_DataItems.Enabled = true;
                }
                else
                {
                    btn_StopLogging.Enabled = true;
                    tv_DataItems.Enabled = false;
                }
            }
            );
        }

        /// <summary>
        /// Websocket disconnect event. Disable/Enable buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWsDisconnect (object sender, EventArgs e)
        {
            this.Invoke ((MethodInvoker)delegate()
            {
                btn_Connect.Enabled = true;
                cb_IP.Enabled = true;
                btn_Disconnect.Enabled = false;
                btn_StartLogging.Enabled = false;
                btn_StopLogging.Enabled = false;
                tv_DataItems.Enabled = false;
            }
            );
        }

        /// <summary>
        /// Are we still logging? Disable/Enable buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoggingChanged (object sender, EventArgs e)
        {
            this.Invoke ((MethodInvoker)delegate()
            {
                if (m_MessageHandler.Logging == false)
                {
                    if (m_MessageHandler.WsClient.State == WebSocketState.Open)
                    {
                        btn_StartLogging.Enabled = true;
                        tb_UpdateRate.Enabled = true;
                    }
                    else
                    {
                        btn_StartLogging.Enabled = false;
                        tb_UpdateRate.Enabled = false;
                    }
                }
                else
                {
                    btn_StartLogging.Enabled = false;
                    tb_UpdateRate.Enabled = false;
                }

                if (btn_StartLogging.Enabled == true)
                {
                    btn_StopLogging.Enabled = false;
                    tv_DataItems.Enabled = true;
                }
                else
                {
                    btn_StopLogging.Enabled = true;
                    tv_DataItems.Enabled = false;
                }

            }
            );
        }

        /// <summary>
        /// Websocket message received. Update status bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived (object sender, EventArgs e)
        {
            this.Invoke ((MethodInvoker)delegate()
            {
                lbl_MessagesReceived.Text = "Message Count: " + (++m_MessageCount).ToString () + "    Log File Size: " + m_LogFileSizeString;
            }
            );
        }

        /// <summary>
        /// Finished a log file write. Update status bar with new file size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWriteComplete (object sender, EventArgs e)
        {
            if (!m_MessageHandler.Disposing)
            {
                this.Invoke ((MethodInvoker)delegate()
                {
                    if (File.Exists (txt_FileName.Text))
                    {
                        FileInfo fi = new FileInfo (txt_FileName.Text);
                        m_LogFileSize = fi.Length / 1024.0F;
                        if (m_LogFileSize < 1024)
                        {
                            m_LogFileSizeString = m_LogFileSize.ToString ("n2") + " KB";
                        }
                        else
                        {
                            m_LogFileSizeString = (m_LogFileSize / 1024F).ToString ("n2") + " MB";
                        }
                    }
                }
                );
            }
        }

        public void PopulateTreeView ()
        {
            Dictionary<float, String> dataGroupDict = NavicoJson.DataGroups;
            tv_DataItems.Nodes.Clear ();
            foreach (int i in m_MessageHandler.DataGroups.Keys)
            {
                //TreeNode parentNode = new TreeNode(dataGroupDict[i]);

                foreach (int j in m_MessageHandler.DataGroups[i])
                {
                    TreeNode childNode = new TreeNode ();
                    childNode.Text = m_MessageHandler.DataInformation [j].lname;
                    childNode.Tag = Convert.ToInt32( m_MessageHandler.DataInformation [j].id);

                    if ((m_MessageHandler.DataInformation [j].n2kNames != null) && (m_MessageHandler.DataInformation [j].n2kNames.Count > 1))
                    {
                        m_MessageHandler.DataInformation [j].n2kNames.Sort ();
                        foreach (String n2kName in m_MessageHandler.DataInformation[j].n2kNames)
                        {
                            TreeNode nameNode = new TreeNode ();
                            nameNode.Tag = n2kName;
                            if (m_MessageHandler.DeviceList.ContainsKey (n2kName))
                            {
                                nameNode.Text = m_MessageHandler.DeviceList [n2kName].ModelId + " : " + m_MessageHandler.DeviceList [n2kName].SerialNumber;
                            }
                            else
                            {
                                nameNode.Text = n2kName;
                            }
 
                            if (m_MessageHandler.N2kNameDictionary.ContainsKey (j))
                            {
                                if (m_MessageHandler.N2kNameDictionary [j].Contains (n2kName))
                                {
                                    nameNode.Checked = true;
                                }
                            }
                            childNode.Nodes.Add (nameNode);
                        }
                    }
                    else if (m_MessageHandler.DataInformation [j].numInstances > 1)
                    {
                        foreach (NavicoJson.IncomingDataInfo.Info.InstanceInfo instInfo in m_MessageHandler.DataInformation[j].instanceInfo)
                        {
                            TreeNode instNode = new TreeNode ();
                            instNode.Tag = Convert.ToInt32( instInfo.inst);
                            if (instInfo.str == String.Empty)
                            {
                                instNode.Text = instInfo.inst.ToString ();
                            }
                            else
                            {
                                instNode.Text = instInfo.str;
                            }
                            if (m_MessageHandler.InstanceDictionary.ContainsKey (j))
                            {
                                if (m_MessageHandler.InstanceDictionary [j].Contains (Convert.ToInt32(instInfo.inst)))
                                {
                                    instNode.Checked = true;
                                }
                            }
                            childNode.Nodes.Add (instNode);
                        }
                    }

                    if (m_MessageHandler.IdList.Contains (Convert.ToInt32(m_MessageHandler.DataInformation [j].id)))
                        childNode.Checked = true;
                    else
                        childNode.Checked = false;

                    String search = tb_Search.Text;
                    if (/*parentNode.Text.ToLower().Contains(search)
                        || */childNode.Text.ToLower ().Contains (search.ToLower ())
                        || search.Length == 0)
                    {
                        //parentNode.Nodes.Add(childNode);
                        tv_DataItems.Nodes.Add (childNode);
                    }
                }
                //if (parentNode.Nodes.Count > 0)
                //{
                //    tv_DataItems.Nodes.Add(parentNode);
                //}
            }
            foreach (TreeNode node in tv_DataItems.Nodes)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Checked)
                    {
                        node.Checked = true;
                        break;
                    }
                }
            }
            ShowCheckedNodes ();
        }
        
        private void btn_StartLogging_Click (object sender, EventArgs e)
        {
            if (txt_FileName.Text == String.Empty)
            {
                MessageBox.Show ("Please select log file");
                return;
            }
            int selectedItemCount = CountDataItems ();
            if (selectedItemCount > 50)
            {
                MessageBox.Show ("Please select fewer than 50 data items.\nThere are currently " + selectedItemCount.ToString () + " items selected", "Too many items");
                return;
            }

            List<int> newIdList = new List<int> ();
            Dictionary<int, List<int>> newInstDict = new Dictionary<int, List<int>> ();
            Dictionary<int, List<String>> newNameDict = new Dictionary<int, List<string>> ();


            foreach (TreeNode parentNode in tv_DataItems.Nodes)
            {
                if (parentNode.Nodes.Count > 0)
                {
                    List<String> nameList = new List<String> ();
                    List<int> instList = new List<int> ();

                    int id = (int)parentNode.Tag;

                    foreach (TreeNode childNode in parentNode.Nodes)
                    {
                        if ((childNode.Checked) && (newIdList.Contains (id) == false))
                        {
                            newIdList.Add (id);
                        }

                        if (childNode.Checked == true)
                        {
                            if (childNode.Tag is String)
                            {
                                // deal with n2kNames
                                nameList.Add (childNode.Tag.ToString ());
                            }
                            else if (childNode.Tag is int)
                            {
                                instList.Add ((int)childNode.Tag);
                            }
                        }
                    }

                    if (instList.Count > 0)
                    {
                        instList.Sort ();
                        newInstDict.Add (id, instList);
                    }
                    if (nameList.Count > 0)
                    {
                        nameList.Sort ();
                        newNameDict.Add (id, nameList);
                    }
                }
                else
                {
                    if (parentNode.Checked)
                    {
                        int id = (int)parentNode.Tag;
                        newIdList.Add (id);
                    }
                }
            }

            // ensure we have time as one of the fields
            newIdList.Remove (m_cTimeId);
            newIdList.Add (m_cTimeId);

            newIdList.Sort ();


            bool requestIds;
            if (EqualLists (newIdList, m_MessageHandler.IdList) && EqualDictionaries (newInstDict, m_MessageHandler.InstanceDictionary) && EqualDictionaries (newNameDict, m_MessageHandler.N2kNameDictionary))
            {
                requestIds = true;
            }
            else
            {
                // Lists are different so headers will be wrong. for now just offer a new file.
                if (m_MessageHandler.IsNewFile)
                {
                    requestIds = true;
                }
                else
                {
                    if (MessageBox.Show ("Selected items are different to previous options\nClick Yes to start new file, No to select a different file or change the required data", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        m_MessageCount = 0;
                        m_MessageHandler.IsNewFile = true;
                        m_LogFileSize = 0;
                        m_LogFileSizeString = "0";
                        requestIds = true;
                        OnMessageReceived(this, null);
                    }
                    else
                    {
                        requestIds = false;
                    }
                }
            }

            List<DataLogItem> expectedItems = new List<DataLogItem> ();
            foreach (int id in newIdList)
            {
                if (newInstDict != null && newInstDict.ContainsKey (id) && newInstDict [id].Count > 0)
                {
                    foreach (int inst in newInstDict[id])
                    {
                        expectedItems.Add (new DataLogItem (id, inst));
                    }
                }
                else if (newNameDict != null && newNameDict.ContainsKey (id) && newNameDict [id].Count > 0)
                {
                    foreach (String name in newNameDict[id])
                    {
                        expectedItems.Add (new DataLogItem (id, name));
                    }
                }
                else
                {
                    expectedItems.Add (new DataLogItem (id));
                }
            }

            m_MessageHandler.CurrentMessage = new DataMessage (MessageHandler.m_cSeparator, expectedItems);

            if (requestIds)
                m_MessageHandler.RequestIds (txt_FileName.Text, newIdList, newInstDict, newNameDict);

            int refreshRate = -1;
            bool useUserTimer = int.TryParse (tb_UpdateRate.Text, out refreshRate);
            m_MessageHandler.SetUserTimer (useUserTimer, refreshRate);

        }

        private bool EqualDictionaries (Dictionary<int, List<String>> dictOne, Dictionary<int, List<String>> dictTwo)
        {
            List<int> keysOne = new List<int> (dictOne.Keys);
            List<int> keysTwo = new List<int> (dictTwo.Keys);
            if (EqualLists (keysOne, keysTwo))
            {
                foreach (int key in dictOne.Keys)
                {
                    if (dictOne [key] != dictTwo [key])
                    {
                        return false;
                    }
                    if (!EqualLists (dictOne [key], dictTwo [key]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool EqualDictionaries (Dictionary<int, List<int>> dictOne, Dictionary<int, List<int>> dictTwo)
        {
            List<int> keysOne = new List<int> (dictOne.Keys);
            List<int> keysTwo = new List<int> (dictTwo.Keys);
            if (EqualLists (keysOne, keysTwo))
            {
                foreach (int key in dictOne.Keys)
                {
                    if (dictOne [key] != dictTwo [key])
                    {
                        return false;
                    }
                    if (!EqualLists (dictOne [key], dictTwo [key]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool EqualLists (List<String> listOne, List<String> listTwo)
        {
            if (listOne.Count == listTwo.Count)
            {
                for (int i = 0; i < listOne.Count; i++)
                {
                    if (listOne [i].CompareTo (listTwo [i]) != 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool EqualLists (List<int> listOne, List<int> listTwo)
        {
            if (listOne.Count == listTwo.Count)
            {
                for (int i = 0; i < listOne.Count; i++)
                {
                    if (listOne [i] != listTwo [i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private void btn_StopLogging_Click (object sender, EventArgs e)
        {
            m_MessageHandler.StopLogging ();
        }

        private void ShowCheckedNodes ()
        {
            // Disable redrawing of treeView1 to prevent flickering  
            // while changes are made.
            tv_DataItems.BeginUpdate ();

            // Collapse all nodes of treeView1.
            tv_DataItems.CollapseAll ();

            // Add the checkForCheckedChildren event handler to the BeforeExpand event.
            tv_DataItems.BeforeExpand += checkForCheckedChildren;

            // Expand all nodes of treeView1. Nodes without checked children are  
            // prevented from expanding by the checkForCheckedChildren event handler.
            tv_DataItems.ExpandAll ();

            // Remove the checkForCheckedChildren event handler from the BeforeExpand  
            // event so manual node expansion will work correctly.
            tv_DataItems.BeforeExpand -= checkForCheckedChildren;

            // Enable redrawing of treeView1.
            tv_DataItems.EndUpdate ();
        }

        // Prevent expansion of a node that does not have any checked child nodes. 
        private void CheckForCheckedChildrenHandler (object sender,
            TreeViewCancelEventArgs e)
        {
            if (!HasCheckedChildNodes (e.Node))
                e.Cancel = true;
        }

        // Returns a value indicating whether the specified  
        // TreeNode has checked child nodes. 
        private bool HasCheckedChildNodes (TreeNode node)
        {
            if (node.Nodes.Count == 0)
                return false;
            foreach (TreeNode childNode in node.Nodes)
            {
                if (childNode.Checked)
                    return true;
                // Recursively check the children of the current child node. 
                if (HasCheckedChildNodes (childNode))
                    return true;
            }
            return false;
        }

        private void tv_DataItems_NodeMouseClick (object sender, TreeNodeMouseClickEventArgs e)
        {
            lock (this)
            {
                CheckNodeChildren (e.Node);
                CheckNodeParents (e.Node);
            }
        }

        private void CheckNodeChildren (TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)
            {
                child.Checked = node.Checked;
                CheckNodeChildren (child);
            }
        }

        private void CheckNodeParents (TreeNode node)
        {
            TreeNode parent = node.Parent;
            // if any siblings are checked, leave parent, otherwise uncheck
            if (parent != null)
            {
                bool siblingChecked = false;
                foreach (TreeNode sibling in parent.Nodes)
                {
                    if (sibling.Checked)
                    {
                        siblingChecked = true;
                        break;
                    }
                }
                parent.Checked = siblingChecked;
                CheckNodeParents (parent);
            }
        }

        private void ReceiveMulticast ()
        {
            String strData = "";
            ASCIIEncoding ASCII = new ASCIIEncoding ();
            UdpClient c = new UdpClient (m_cGroupPort);
            c.MulticastLoopback = true;

            // Establish the communication endpoint.
            IPEndPoint endpoint = new IPEndPoint (IPAddress.Any, m_cGroupPort);
            IPAddress m_GrpAddr = IPAddress.Parse (m_cGroupAddress);

            bool groupJoined = false;
            while (( m_Open == true ) && ( groupJoined == false ))
            {
                try
                {
                    c.JoinMulticastGroup (m_GrpAddr);
                    c.Client.ReceiveTimeout = 1000;
                    groupJoined = true;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine (ex.ToString ());
                    Thread.Sleep (1000);
                }
            }

            while (m_Open)
            {
                try
                {
                    Byte[] data = c.Receive (ref endpoint);

                    strData = ASCII.GetString (data);
                    if (MulticastReceived != null)
                        MulticastReceived (this, new MulticastMessageEventArgs (strData));
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode != (int)SocketError.TimedOut)
                    {
                        throw ex;
                    }
                }
            }
        }

        private void OnMulticastReceived (object sender, MulticastMessageEventArgs e)
        {
            NavicoJson.UnitServiceInfo info = new JavaScriptSerializer().Deserialize<NavicoJson.UnitServiceInfo>(e.Message);
            if (info.WebsocketPort != 0)
            {
                if (m_AnnouncedServers.ContainsKey(info.IP) == false)
                {
                    m_AnnouncedServers[info.IP] = info;
                    UpdateServiceList(info);
                }
            }
        }

        private void UpdateServiceList (NavicoJson.UnitServiceInfo info)
        {
            this.Invoke ((MethodInvoker)delegate()
            {
                cb_IP.Items.Add (info);
                if (cb_IP.Items.Count == 1)
                {
                    cb_IP.SelectedItem = info;
                }
            }
            );
        }

        private int CountDataItems ()
        {
            int retVal = 0;

            foreach (TreeNode node in tv_DataItems.Nodes)
            {
                if (node.Checked)
                {
                    if (node.Nodes.Count > 0)
                    {
                        retVal += CountActiveChildren (node);
                    }
                    else
                    {
                        retVal++;
                    }
                }
            }

            Console.WriteLine (retVal.ToString ());
            return retVal;
        }

        private int CountActiveChildren (TreeNode parent)
        {
            int retVal = 0;

            foreach (TreeNode child in parent.Nodes)
            {
                if (child.Checked)
                {
                    if (child.Nodes.Count > 0)
                    {
                        retVal += CountActiveChildren (child);
                    }
                    else
                    {
                        retVal++;
                    }
                }
            }
            return retVal;
        }

        private void btn_Search_Click (object sender, EventArgs e)
        {
            PopulateTreeView ();
        }
    }

    public class MulticastMessageEventArgs : System.EventArgs
    {
        public String Message;

        public MulticastMessageEventArgs (String message)
        {
            this.Message = message;
        }
    }
}
