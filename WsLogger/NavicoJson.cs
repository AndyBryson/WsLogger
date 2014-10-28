/*
 * Copyright (c) 2013 Andy Bryson (andy.bryson@navico.com) - all rights reserved
 * This code is released under the GPLv2 and MIT licenses. Pick the one you like.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WsLogger
{
    class NavicoJson
    {
        public static Dictionary<int, String> DataGroups
        {
            get
            {
                Dictionary<int, String> dict = new Dictionary<int, string> ();
                
                dict.Add (1, "GPS");
                dict.Add (2, "Navigation");
                dict.Add (3, "Vessel");
                dict.Add (4, "Sonar");
                dict.Add (5, "Weather");
                dict.Add (6, "Trip");
                dict.Add (7, "Time");
                dict.Add (8, "Engine");
                dict.Add (9, "Transmission");
                dict.Add (10, "FuelTank");
                dict.Add (11, "FreshWaterTank");
                dict.Add (12, "GrayWaterTank");
                dict.Add (13, "LiveWellTank");
                dict.Add (14, "OilTank");
                dict.Add (15, "BlackWaterTank");
                dict.Add (16, "EngineRoom");
                dict.Add (17, "Cabin");
                dict.Add (18, "BaitWell");
                dict.Add (19, "Refrigerator");
                dict.Add (20, "HeatingSystem");
                dict.Add (21, "Freezer");
                dict.Add (22, "Battery");
                dict.Add (23, "Rudder");
                dict.Add (24, "TrimTab");
                dict.Add (25, "ACInput");
                dict.Add (26, "DigitalSwitching");
                dict.Add (27, "Other");
                dict.Add (28, "GPSStatus");
                dict.Add (29, "RouteData");
                dict.Add (30, "SpeedDepth");
                dict.Add (31, "LogTimer");
                dict.Add (32, "Environment");
                dict.Add (33, "Wind");
                dict.Add (34, "Pilot");
                dict.Add (35, "Sailing");
                dict.Add (36, "AcOutput");
                dict.Add (37, "Charger");
                dict.Add (38, "Inverter");

                return dict;
            }
        }
        public class DList
        {
            public class Info
            {
                public int groupId { get; set; }
                public int[] list { get; set; }
            }
            public Info DataList { get; set; }

            public bool IsValid ()
            {
                if (DataList != null)
                {
                    return true;
                }
                return false;
            }
        }

        public class IncomingData
        {
            public class DataItem
            {
                public int id { get; set; }
                //public float val = -1;
                public string valStr { get; set; }
                public bool valid { get; set; }
                public int inst { get; set; }
                public string n2kName { get; set; }

                public override String ToString ()
                {
                    if (valid)
                    {
                        //return val.ToString();
                        return valStr.Replace ("&deg;", "°");
                    }
                    else
                    {
                        return "invalid";
                    }
                }

            }
            public List<DataItem> Data { get; set; }

            public bool IsValid ()
            {
                if (Data != null)
                {
                    if (Data.Count > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public class IncomingDeviceList
        {
            public class Device
            {
                public int DeviceType {get;set;}
                public String ModelId {get;set;}
                public String NDP2kName {get;set;}
                public bool ProxyAvailable {get;set;}
                public String SerialNumber {get;set;}
            }

            public List<Device> DeviceList {get;set;}
            //public Device[] Devices = null;

            public bool IsValid ()
            {
                if (DeviceList != null)
                {
                    if (DeviceList.Count > 0)
                        return true;
                }
                return false;
            }
        }

        public class IncomingDataInfo
        {
            public class Info
            {
                public class InstanceInfo
                {
                    public int inst { get; set; }
                    public String str{get;set;}
                    public int location{get;set;}
                }

                public int id{get;set;}
                public String sname{get;set;}
                public String lname{get;set;}
                public String unit{get;set;}
                public float min{get;set;}
                public float max{get;set;}
                public int numInstances{get;set;}
                public InstanceInfo[] instanceInfo {get;set;}
                public List<String> n2kNames {get;set;}
            }
            public List<Info> DataInfo {get;set;}

            public bool IsValid ()
            {
                if (DataInfo != null)
                {
                    if (DataInfo.Count > 0)
                        return true;
                }
                return false;
            }
        }

        public class UnitServiceInfo
        {
            public static String m_cNavdataWebsocket = "navico-nav-ws";
            
            public class ServiceInfo
            {
                public int Port {get;set;}
                public String Service {get;set;}
                public int Version {get;set;}
            }
            public String IP {get;set;}
            public String Model {get;set;}
            public String Name {get;set;}
            public List<ServiceInfo> Services {get;set;}

            public int WebsocketPort
            {
                get
                {
                    foreach (ServiceInfo inf in Services)
                    {
                        if (inf.Service.CompareTo (m_cNavdataWebsocket) == 0)
                        {
                            return inf.Port;
                        }
                    }
                    return 0;
                }
            }

            public bool IsValid ()
            {
                if (IP != null && Model != null && Name != null)
                    return true;
                return false;
            }

            public override String ToString ()
            {
                StringBuilder sb = new StringBuilder ();
                sb.Append (IP);
                sb.Append (" [");
                sb.Append (Model);
                if (Name.CompareTo ("Unknown") != 0)
                {
                    sb.Append (" - ");
                    sb.Append (Name);
                }
                sb.Append ("]");
                return sb.ToString ();
            }
        }
    }
}

