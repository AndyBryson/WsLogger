/*
 * Copyright (c) 2013 Andy Bryson (andy.bryson@navico.com)
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the Free
 * Software Foundation; either version 2 of the License, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc., 51
 * Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
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
                Dictionary<int, String> dict = new Dictionary<int, string>();
                
                dict.Add(1, "GPS");
                dict.Add(2, "Navigation");
                dict.Add(3, "Vessel");
                dict.Add(4, "Sonar");
                dict.Add(5, "Weather");
                dict.Add(6, "Trip");
                dict.Add(7, "Time");
                dict.Add(8, "Engine");
                dict.Add(9, "Transmission");
                dict.Add(10, "FuelTank");
                dict.Add(11, "FreshWaterTank");
                dict.Add(12, "GrayWaterTank");
                dict.Add(13, "LiveWellTank");
                dict.Add(14, "OilTank");
                dict.Add(15, "BlackWaterTank");
                dict.Add(16, "EngineRoom");
                dict.Add(17, "Cabin");
                dict.Add(18, "BaitWell");
                dict.Add(19, "Refrigerator");
                dict.Add(20, "HeatingSystem");
                dict.Add(21, "Freezer");
                dict.Add(22, "Battery");
                dict.Add(23, "Rudder");
                dict.Add(24, "TrimTab");
                dict.Add(25, "ACInput");
                dict.Add(26, "DigitalSwitching");
                dict.Add(27, "Other");
                dict.Add(28, "GPSStatus");
                dict.Add(29, "RouteData");
                dict.Add(30, "SpeedDepth");
                dict.Add(31, "LogTimer");
                dict.Add(32, "Environment");
                dict.Add(33, "Wind");
                dict.Add(34, "Pilot");
                dict.Add(35, "Sailing");
                dict.Add(36, "AcOutput");
                dict.Add(37, "Charger");
                dict.Add(38, "Inverter");

                return dict;
            }
        }
        public class DList
        {
            public class Info
            {
                public int groupId = -1;
                public int[] list = null;
            }
            public Info DataList = null;

            public bool IsValid()
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
                public int id = -1;
                //public float val = -1;
                public string valStr = "";
                public bool valid = false;
                public int inst = 0;

                public override String ToString()
                {
                    if (valid)
                    {
                        //return val.ToString();
                        return valStr.Replace("&deg;", "°");
                    }
                    else
                    {
                        return "invalid";
                    }
                }

            }
            public DataItem[] Data = null;

            public bool IsValid()
            {
                if (Data != null)
                {
                    if (Data.Length > 0)
                    {
                        return true;
                    }
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
                    public int inst;
                    public String str;
                    public int location;
                }

                public int id;
                public String sname;
                public String lname;
                public String unit;
                public float min;
                public float max;
                public int numInstances;
                public InstanceInfo[] instanceInfo;
            }
            public Info[] DataInfo = null;

            public bool IsValid()
            {
                if (DataInfo != null)
                {
                    if (DataInfo.Length > 0)
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
                public int Port;
                public String Service;
                public int Version;
            }
            public String IP = null;
            public String Model = null;
            public String Name = null;
            public ServiceInfo[] Services;

            public int WebsocketPort
            {
                get
                {
                    foreach (ServiceInfo inf in Services)
                    {
                        if (inf.Service.CompareTo(m_cNavdataWebsocket) == 0)
                        {
                            return inf.Port;
                        }
                    }
                    return 0;
                }
            }
            public bool IsValid()
            {
                if (IP != null && Model != null && Name != null)
                    return true;
                return false;
            }

            public override String ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(IP);
                sb.Append(" [");
                sb.Append(Model);
                if (Name.CompareTo("Unknown") != 0)
                {
                    sb.Append(" - ");
                    sb.Append(Name);
                }
                sb.Append("]");
                return sb.ToString();
            }
        }
    }
}

