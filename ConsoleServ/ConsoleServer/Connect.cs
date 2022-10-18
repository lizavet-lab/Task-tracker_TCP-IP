using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Net.Sockets;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace ConsoleServer
{
    class Connect
    {
        public static string Path2 { get; set; }
        public static string ConStr { get; set; }
        public static string User { get; set; }
        public static DataSet data = new System.Data.DataSet();
        public static OleDbDataAdapter adapt { get; set; }
        public static String query { get; set; }
        public static OleDbParameter param { get; set; }

        public static string[] GetExcelSheetNames(string connectionString)
        {
            OleDbConnection con = null;
            DataTable dt = null;
            con = new OleDbConnection(connectionString);
            con.Open();
            dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                return null;
            }

            String[] excelSheetNames = new String[dt.Rows.Count];
            int i = 0;

            foreach (DataRow row in dt.Rows)
            {
                excelSheetNames[i] = row["TABLE_NAME"].ToString();
                i++;
            }

            return excelSheetNames;
        }

        public static DataSet CreateDataSet()
        {
            foreach (var sheetName in Connect.GetExcelSheetNames(ConStr))
            {
                using (OleDbConnection connect = new OleDbConnection(ConStr))
                {
                    DataTable dataTable = new DataTable();
                    query = string.Format("SELECT * FROM [{0}]", sheetName);
                    connect.Open();
                    adapt = new OleDbDataAdapter(query, connect);
                    OleDbCommandBuilder builder = new OleDbCommandBuilder(adapt);
                    adapt.Fill(dataTable);//скопировали данные через адаптер в таблицу dataTable
                    data.Tables.Add(dataTable);//таблицу добавили в DataSet "data"
                                               //adapt.UpdateCommand = builder.GetUpdateCommand();
                                               //adapt.Update(data);
                }
            }
            return data;
        }

        public static byte[] GetBinaryFormatData(DataTable dt)
        {

            BinaryFormatter formatter = new BinaryFormatter();
            byte[] outList = null;
            //dt.RemotingFormat = SerializationFormat.Binary;
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, dt);
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

        public static DataTable GetDataTable(byte[] dtData)
        {
            DataTable dt = null;
            BinaryFormatter bFormat = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(dtData))
            {
                dt = (DataTable)bFormat.Deserialize(ms);
            }
            return dt;
        }

        // public static ExcelObj.Worksheet NwSheet { get; set; }
        //public static ExcelObj.Range ShtRange { get; set; }
    }
}
