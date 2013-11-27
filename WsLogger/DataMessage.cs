using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WsLogger
{
	class DataLogItem :  IComparable<DataLogItem>
	{
		public DataLogItem( int id, String n2kName )
		{
			this.id = id;
			this.n2kName = n2kName;
		}
		public DataLogItem (int id, int instance = 0)
		{
			this.id = id;
			this.instance = instance;
		}

		public DataLogItem (NavicoJson.IncomingData.DataItem incoming)
		{
			this.id = incoming.id;
			this.instance = incoming.inst;
			this.n2kName = incoming.n2kName;
			this.value = incoming.valStr;
		}

		public int CompareTo (DataLogItem other)
		{
			if (this.id != other.id) {
				return other.id - this.id;
			} 
			else if( this.instance != other.instance )
			{
				return other.instance - this.instance;
			}
			else
			{
				return this.n2kName.CompareTo( other.n2kName );
			}
			return 0;
		}


		public int id;
		public String n2kName;
		public int instance;
		public String value = "";
	}

    class DataMessage
    {
        String m_Separator;

        public DataMessage(String separator, List<DataLogItem> expectedData)
        {
            m_Separator = separator;
			m_ExpectedData = expectedData;
			m_Data = expectedData;
        }

        private DataMessage() { }

		private List<DataLogItem> m_ExpectedData;
		private List<DataLogItem> m_Data;
        
		public void AddData (NavicoJson.IncomingData.DataItem item)
		{
			if (item.valStr != String.Empty) 
			{
				DataLogItem itemIn = new DataLogItem (item);
				foreach (DataLogItem dli in m_Data) 
				{
					if (dli.CompareTo (itemIn) ==  0)
					{
						dli.value = item.valStr;
						break;
					}
				}
			}
		}

//        public void AddData(int id, String value)
//        {
//            if ( ( value != String.Empty ) || ( m_Data.ContainsKey(id) == false ) )
//            {
//				m_Data[id] = value;
//            }
//        }

        public int Count
        {
            get
            {
                return m_Data.Count;
            }
        }

        override public String ToString ()
		{
			StringBuilder sb = new StringBuilder ();

			sb.Append (System.DateTime.Now.ToString ("yyMMdd-HHmmss.ff" + m_Separator));
//            foreach( int id in m_Data.Keys )
//            {
//                sb.Append(m_Data[id]);
//                sb.Append(m_Separator);
//            }

			foreach (DataLogItem dli in m_Data) {
				sb.Append ( dli.value );
				sb.Append( m_Separator );
			}

            return sb.ToString();
        }

        internal void Clear()
        {
			m_Data = m_ExpectedData;
        }
    }
}
