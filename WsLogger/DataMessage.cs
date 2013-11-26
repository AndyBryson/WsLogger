using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WsLogger
{
    class DataMessage
    {
        String m_Separator;

        public DataMessage(String separator)
        {
            m_Separator = separator;
        }

        private DataMessage() { }

        private Dictionary<int, String> m_Data = new Dictionary<int,string>();
        
        public void AddData(int id, String value)
        {
            if ( ( value != String.Empty ) || ( m_Data.ContainsKey(id) == false ) )
            {
				m_Data[id] = value;
            }
        }

        public int Count
        {
            get
            {
                return m_Data.Count;
            }
        }

        override public String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(System.DateTime.Now.ToString("yyMMdd-HHmmss.ff" + m_Separator));
            foreach( int id in m_Data.Keys )
            {
                sb.Append(m_Data[id]);
                sb.Append(m_Separator);
            }
            return sb.ToString();
        }

        internal void Clear()
        {
            m_Data.Clear();
        }
    }
}
