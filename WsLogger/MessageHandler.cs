using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using WebSocket4Net;
using Newtonsoft.Json;

namespace WsLogger
{
    enum eConnectionState
    {
        None,
        FullPopulate_Begin,
        FullPopulate_Groups,
        FullPopulate_Infomation,
        Information,
        Data
    }

    public delegate void PopulationComplete(object sender, EventArgs e);
    public delegate void LoggingChanged(object sender, EventArgs e);
    public delegate void ValidMessageReceived(object sender, EventArgs e);
    public delegate void WriteComplete(object sender, EventArgs e);

    class MessageHandler
    {
        public event EventHandler PopulationComplete;
        public event EventHandler LoggingChanged;
        public event EventHandler ValidMessageReceived;
        public event EventHandler WriteComplete;

        #region Constants
        public const String m_cSeparator = "\t";
        private const String m_cSettingsFileName = "settings.ini";
        private const int m_cMaxMessages = 50;
        #endregion

        #region Members
        String m_LogFileLocation;
        List<String> m_EntryList;
        bool m_Logging = false;
        bool m_IsNewFile = true;
        WsClient m_WsClient;
        List<int> m_IdList;
        Dictionary<int, List<int>> m_InstanceDict;
        eConnectionState m_ConnectionState = eConnectionState.None;
        Dictionary<int, NavicoJson.IncomingDataInfo.Info> m_DataInformation;
        Dictionary<int, List<int>> m_DataGroups;
        List<int> m_DataGroupWaitingList; // List of requested data groups that haven't arrived
        List<int> m_InformationWaitingList; // List of requested data information that haven't arrived
        bool m_Disposing = false;
        #endregion

        public MessageHandler()
        {
            m_WsClient = new WsClient();
            m_DataInformation = new Dictionary<int, NavicoJson.IncomingDataInfo.Info>();
            m_DataGroups = new Dictionary<int, List<int>>();
            m_DataGroupWaitingList = new List<int>();
            m_InformationWaitingList = new List<int>();
            m_IdList = new List<int>();
            m_InstanceDict = new Dictionary<int, List<int>>();

            LoadSettingsFromFile(SettingsFileName);

            m_WsClient.Opened += new EventHandler(OnSocketOpened);
            m_WsClient.Message += new EventHandler<MessageReceivedEventArgs>(OnSocketMessage);
            m_WsClient.Closed += new EventHandler(OnSocketClosed);
        }

        ~MessageHandler()
        {
            m_Disposing = true;

            if (WsClient.State == WebSocketState.Open)
                Disconnect();
        }        

        /// <summary>
        /// Initialise the message handler. Creates the file to be logged too and opens a websocket connection
        /// </summary>
        /// <param name="WsUrl">The full URL of the web socket</param>
        public void Init( String WsUrl )
        {
            m_EntryList = new List<string>();

            m_WsClient.Connect(WsUrl);
        }

        /// <summary>
        /// Request data types from Navico websocket
        /// </summary>
        /// /// <param name="fileLocation">location of log file</param>
        /// <param name="idList">A list of every data type to be requested</param>
        /// <param name="instDict">If specifying an instance, it should be done here</param>
        public void RequestIds( String fileLocation, List<int> idList, Dictionary<int, List<int>> instDict)
        {
            m_LogFileLocation = fileLocation;
            if (!File.Exists(fileLocation))
            {
                using (File.Create(fileLocation)) { }; // using should mean that the file is allowed out of memory after creation
            }

            m_IdList = idList;
            m_InstanceDict = instDict;
            RequestData();
        }

        /// <summary>
        /// Stop logging data. This unsubscribes from any previously subscribed items.
        /// </summary>
        public void StopLogging()
        {
            if (Logging)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{\"UnsubscribeData\":[");
                foreach (int i in m_IdList)
                {
                    if (m_InstanceDict.ContainsKey(i))
                    {
                        foreach (int inst in m_InstanceDict[i])
                        {
                            sb.Append("{\"id\":");
                            sb.Append(i.ToString());
                            sb.Append(",\"inst\":");
                            sb.Append(inst.ToString());
                            sb.Append("},");
                        }
                    }
                    else
                    {
                        sb.Append("{\"id\":");
                        sb.Append(i.ToString());
                        sb.Append("},");
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]}");
                m_WsClient.Send(sb.ToString());

                WriteMessages();
                Logging = false;
                m_ConnectionState = eConnectionState.None;
            }
        }

        /// <summary>
        /// Unsubscribe, disconnect, Store and unstored messages.
        /// </summary>
        public void Disconnect()
        {
            StopLogging();
            WriteMessages();
            m_WsClient.Disconnect();
            Logging = false;
        }

        private void HandleDecodedMessage(String message)
        {
            if (Logging)
            {
                m_EntryList.Add(message);
                if (m_EntryList.Count >= m_cMaxMessages)
                {
                    WriteMessages();
                }
            }
        }

        private void WriteMessages()
        {
            if (Logging && m_IdList.Count > 0)
            {
                if (m_IsNewFile)
                {
                    System.IO.File.WriteAllText(m_LogFileLocation, String.Empty);
                }
                using (StreamWriter outfile = new StreamWriter(m_LogFileLocation, true))
                {
                    if (m_IsNewFile)
                    {
                        StringBuilder heading = new StringBuilder();
                        heading.Append("clientTime" + m_cSeparator);
                        foreach (int i in m_IdList)
                        {
                            NavicoJson.IncomingDataInfo.Info info = m_DataInformation[i];
                            if (info != null)
                            {
                                if (info.numInstances > 1)
                                {
                                    for (int instIndex = 0; instIndex < info.instanceInfo.Length; instIndex++)
                                    {
                                        heading.Append(info.lname).Append(" (").Append(info.unit.Replace("&deg;", "°"))
                                            .Append(")").Append(" [").Append(info.instanceInfo[instIndex].str).Append("]")
                                            .Append(m_cSeparator);
                                    }
                                }
                                else
                                {
                                    heading.Append(info.lname).Append(" (").Append(info.unit.Replace("&deg;", "°")).Append(")")
                                        .Append(m_cSeparator);
                                }
                            }
                            else
                            {
                                heading.Append(i.ToString()).Append(m_cSeparator);
                            }
                        }
                        heading.Remove(heading.Length - 1, 1);

                        outfile.WriteLine(heading.ToString());
                        m_IsNewFile = false;

                    }
                    foreach (String s in m_EntryList)
                    {
                        outfile.WriteLine(s);
                    }
                }
                m_EntryList.Clear();
            }
            if (WriteComplete != null)
                WriteComplete(this, new EventArgs());
        }

        internal void HandleMessage(NavicoJson.IncomingData data)
        {
            if (Logging)
            {
                StringBuilder sb = new StringBuilder();
                
                // add client time
                sb.Append(System.DateTime.Now.ToString("yyMMdd-HHmmss.ff" + m_cSeparator));

                // add stuff from the network
                foreach (NavicoJson.IncomingData.DataItem item in data.Data)
                {
                    sb.Append(item.ToString());
                    sb.Append(m_cSeparator);
                }
                sb.Remove(sb.Length - 1, 1); // remove the last comma
                HandleDecodedMessage(sb.ToString());

                if (ValidMessageReceived != null)
                    ValidMessageReceived(this, new EventArgs());
            }
        }

        internal void HandleMessage(NavicoJson.IncomingDataInfo dataInfo)
        {
            foreach (NavicoJson.IncomingDataInfo.Info info in dataInfo.DataInfo)
            {
                m_DataInformation[info.id] = info;
                m_InformationWaitingList.Remove(info.id);
            }

            if (m_InformationWaitingList.Count == 0)
            {
                StartNextTransaction();
            }
        }


        private void HandleMessage (NavicoJson.DList dl)
		{
			if (m_DataGroups.ContainsKey (dl.DataList.groupId))
			{
				foreach (int i in dl.DataList.list) 
				{
					m_DataGroups [dl.DataList.groupId].Add (i);
				}
			}
			m_DataGroupWaitingList.Remove (dl.DataList.groupId);
			if (m_DataGroupWaitingList.Count == 0) 
			{
				bool containsData = false;
				foreach( List<int> list in m_DataGroups.Values )
				{
					if( list.Count > 0 )
					{
						containsData = true;
						break;
					}
				}
				if( containsData == false && m_DataGroups.ContainsKey( 40 ) == false )
				{
					m_DataGroups.Add ( 40, new List<int>() );
					m_DataGroupWaitingList.Add( 40 );
					m_WsClient.Send("{\"DataListReq\":{\"groupId\":40}}");
				}
				else
				{
					StartNextTransaction ();
				}
			}
        }

        private void OnSocketOpened(object sender, EventArgs e)
        {
        }

        private void OnSocketClosed(object sender, EventArgs e)
        {
            Logging = false;
        }

        private void OnSocketMessage(object sender, MessageReceivedEventArgs e)
        {
            lock (this)
            {
                if (e.Message.StartsWith("{\"DataInfo\":"))
                {
                    NavicoJson.IncomingDataInfo info = JsonConvert.DeserializeObject<NavicoJson.IncomingDataInfo>(e.Message);
                    if (info.IsValid())
                    {
                        HandleMessage(info);
                    }
                }
                else
                {
                    NavicoJson.IncomingData data = JsonConvert.DeserializeObject<NavicoJson.IncomingData>(e.Message);
                    if (data.IsValid())
                    {
                        HandleMessage(data);
                    }
                    else
                    {
                        NavicoJson.DList dl = JsonConvert.DeserializeObject<NavicoJson.DList>(e.Message);
                        if (dl.IsValid())
                        {
                            HandleMessage(dl);
                        }
                    }
                }
            }
        }

        private void RequestData()
        {
            SaveSettingsToFile();
            m_ConnectionState = eConnectionState.Data;

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"DataReq\":[");
            foreach (int i in m_IdList)
            {
                List<int> instList = null;
                if (m_InstanceDict.ContainsKey(i))
                {
                    if (m_InstanceDict[i].Count > 0)
                    {
                        instList = m_InstanceDict[i];
                    }
                }
                if (instList == null)
                {
                    instList = new List<int>();
                    instList.Add(0);
                }

                foreach (int inst in instList)
                {
                    sb.Append("{\"id\":");
                    sb.Append(i.ToString());
                    sb.Append(",\"inst\":").Append(inst.ToString());
                    sb.Append(",\"repeat\":true},");
                }
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");
            m_WsClient.Send(sb.ToString());

            Logging = true;
        }



        private void RequestInformation (List<int> idList)
		{
			m_InformationWaitingList.AddRange (idList);

			StringBuilder sb = new StringBuilder ();
			sb.Append ("{\"DataInfoReq\":[");
			foreach (int i in idList) {
				sb.Append (i.ToString () + ",");
			}
			sb.Remove (sb.Length - 1, 1);
			sb.Append ("]}");

			if (IsLinux) 
			{
				System.Threading.Thread.Sleep (25);
			}
            m_WsClient.Send(sb.ToString());
        }

        /// <summary>
        /// The current state of the WebSocket connection
        /// </summary>
        public WebSocketState SocketState
        {
            get
            {
                return m_WsClient.State;
            }
        }

        /// <summary>
        /// Start the process of obtaining information about every data type
        /// </summary>
        public void PopulateDataInfomation()
        {
            m_ConnectionState = eConnectionState.FullPopulate_Begin;
            StartNextTransaction();
        }

        /// <summary>
        /// Request every data id in every data group.
        /// </summary>
        private void GetAllDataGroups()
        {
            foreach (int i in NavicoJson.DataGroups.Keys)
            {
                m_DataGroups[i] = new List<int>();
                m_WsClient.Send("{\"DataListReq\":{\"groupId\":" + i.ToString() + "}}");
                m_DataGroupWaitingList.Add(i); // add to waiting list. When we receive ids we remove from list. When list is empty we will call StartNextTransaction()
            }
        }

        /// <summary>
        /// Request data information for every known data id.
        /// </summary>
        private void GetAllDataInformation()
        {
            List<int> everyItemList = new List<int>();
            foreach (int i in m_DataGroups.Keys)
            {
                everyItemList.AddRange(m_DataGroups[i]);
            }

            // only request 5 items at a time.
            int j = 0;
            while (j < everyItemList.Count)
            {
                List<int> innerRequest = new List<int>();
                while (innerRequest.Count < 5 && j < everyItemList.Count)
                {
                    innerRequest.Add(everyItemList[j]);
                    j++;
                }
                RequestInformation(innerRequest);
            }
        }

        /// <summary>
        /// Begins the next transaction in websockets code. Used because we need to strictly order 
        /// asynchronous request. If the websocket client implementation was better, we probably wouldn't
        /// need it.
        /// </summary>
        private void StartNextTransaction()
        {
            switch (m_ConnectionState)
            {
                case eConnectionState.FullPopulate_Begin:
                    GetAllDataGroups();
                    m_ConnectionState++;
                    break;
                case eConnectionState.FullPopulate_Groups:
                    m_ConnectionState = eConnectionState.FullPopulate_Infomation;
                    GetAllDataInformation();
                    break;
                case eConnectionState.FullPopulate_Infomation:
                    if (PopulationComplete != null)
                        PopulationComplete(this, new EventArgs());

                    m_ConnectionState = eConnectionState.None;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Save settings to the settings file
        /// </summary>
        private void SaveSettingsToFile()
        {
            if (m_IdList != null && m_IdList.Count > 0)
            {
                using (StreamWriter sw = new StreamWriter(SettingsFileName))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("ids:");
                    foreach (int i in m_IdList)
                    {
                        sb.Append(i.ToString()).Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sw.WriteLine(sb.ToString());

					sb = new StringBuilder();
                    //sb.Clear();

                    sb.Append("instances:");
                    foreach (int key in m_InstanceDict.Keys)
                    {
                        sb.Append(key.ToString() + "=");

                        if (m_InstanceDict[key] != null && m_InstanceDict[key].Count > 0)
                        {
                            foreach (int inst in m_InstanceDict[key])
                            {
                                sb.Append(inst.ToString() + ",");
                            }
                            sb.Remove(sb.Length - 1, 1);
                        }
                        sb.Append(";");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sw.WriteLine(sb.ToString());

                }
            }
        }

        private void LoadSettingsFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line = sr.ReadLine();
                    while( line != null )
                    {
                        String[] setting = line.Split(':');
                        if (setting.Length == 2)
                        {
                            if (setting[0].CompareTo("ids") == 0)
                            {
                                m_IdList.Clear();
                                String[] values = setting[1].Split(',');
                                foreach (String value in values)
                                {
                                    m_IdList.Add(Int16.Parse(value));
                                }
                            }
                            if (setting[0].CompareTo("instances") == 0)
                            {
                                String[] values = setting[1].Split(';');
                                if (values.Length > 0)
                                {
                                    foreach (String value in values)
                                    {
                                        String[] details = value.Split('=');
                                        int id = Int32.Parse(details[0]);
                                        String[] instances = details[1].Split(',');
                                        if (instances.Length > 0)
                                        {
                                            m_InstanceDict[id] = new List<int>();
                                            foreach (String inst in instances)
                                            {
                                                m_InstanceDict[id].Add(Int32.Parse(inst));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        line = sr.ReadLine();
                    }
                }
            }
        }

        /// <summary>
        /// A list of every data id to log
        /// </summary>
        public List<int> IdList
        {
            get
            {
                return m_IdList;
            }
        }

        /// <summary>
        /// A dictionary of every instance specified. Formatted: [dataId] = instId1,instId2
        /// </summary>
        public Dictionary<int, List<int>> InstanceDictionary
        {
            get
            {
                return m_InstanceDict;
            }
        }
        
        /// <summary>
        /// Are we starting a new file?
        /// </summary>
        public bool IsNewFile
        {
            get
            {
                return m_IsNewFile;
            }
            set
            {
                m_IsNewFile = value;
            }
        }
        
        /// <summary>
        /// Are we currently logging
        /// </summary>
        public bool Logging
        {
            get
            {
                return m_Logging;
            }
            set
            {
                m_Logging = value;
                if (LoggingChanged != null && m_Disposing == false)
                    LoggingChanged(this, new EventArgs());
            }
        }
        
        /// <summary>
        /// The websocket client.
        /// </summary>
        public WsClient WsClient
        {
            get
            {
                return m_WsClient;
            }
        }

        /// <summary>
        /// Datainformation about every dataId
        /// </summary>
        public Dictionary<int, NavicoJson.IncomingDataInfo.Info> DataInformation
        {
            get
            {
                return m_DataInformation;
            }
        }
        
        /// <summary>
        /// Every data item in every data group ([GroupId]:item1,item2)
        /// </summary>
        public Dictionary<int, List<int>> DataGroups
        {
            get
            {
                return m_DataGroups;
            }
        }


        public string SettingsFileName
        {
            get
            {
				if( IsLinux )
				{
                	return "/" + Path.GetDirectoryName(Assembly.GetAssembly(typeof(MessageHandler)).CodeBase).Remove(0, 6) +"/" + m_cSettingsFileName;
				}
				else
				{
					return Path.GetDirectoryName(Assembly.GetAssembly(typeof(MessageHandler)).CodeBase).Remove(0, 6) +"\\" + m_cSettingsFileName;
				}
            }
        }

		private static bool IsLinux
		{
		    get
		    {
		        int p = (int) Environment.OSVersion.Platform;
		        return (p == 4) || (p == 6) || (p == 128);
		    }
		}

        public bool Disposing
        {
            get
            {
                return m_Disposing;
            }
        }
    }
}


