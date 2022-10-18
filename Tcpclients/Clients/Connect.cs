using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Net.Sockets;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using ExcelObj = Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;


namespace Clients
{
    class Connect
    {   
        public static string Path2 { get; set; }
        public static string User { get; set; }
        public static TcpClient Sclient { get; set; }
        public static NetworkStream Stream { get; set; }

        public static byte[] GetBinaryFormatData(DataTable dt)
        {
            BinaryFormatter bFormat = new BinaryFormatter();
            byte[] outList = null;
            dt.RemotingFormat = SerializationFormat.Binary;
            using (MemoryStream ms = new MemoryStream())
            {
                bFormat.Serialize(ms, dt);
                outList = ms.ToArray();
            }
            return outList;
        }

        public static byte[] GetBinaryCode(string str)
        {
            BinaryFormatter bFormat = new BinaryFormatter();
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                bFormat.Serialize(ms, str);
                bytes = ms.ToArray();
            }
            return bytes;
        }

     
        public static DataTable GetDataTable(byte[] Data)
        {
            DataTable dt = null;
            BinaryFormatter bFormat = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(Data))
            {
                dt = (DataTable)bFormat.Deserialize(ms);
            }
            return dt;
        }
    }
}
