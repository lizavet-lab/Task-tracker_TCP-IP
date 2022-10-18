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
using System.Reflection;
using System.Net.Sockets;
using System.Globalization;

namespace Clients
{
    public partial class Form4 : Form
    {
        static byte[] bytesWrite;
        static byte[] bytesRead;

        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string date1=dateTimePicker1.Value.ToString();
            string date2=dateTimePicker2.Value.ToString();

            Connect.Sclient = new TcpClient();
            Connect.Sclient.Connect(A.ip, 1234);
            Connect.Stream = Connect.Sclient.GetStream();
            bytesWrite = Encoding.Unicode.GetBytes("gettable");
            Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
            Connect.Stream.Flush();//удалили данные из потока

            bytesRead = new byte[30000];
            int length = Connect.Stream.Read(bytesRead, 0, bytesRead.Length);
            DataTable answerdt = Connect.GetDataTable(bytesRead);
            foreach (DataRow dr in answerdt.Rows)
            {
                if (dr[0].ToString() == "" && dr[1].ToString() == "" && dr[2].ToString() == "" && dr[3].ToString() == "")
                    dr.Delete();
            }
            answerdt.AcceptChanges();
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            DataTable ready = new DataTable();
            foreach (DataRow row in answerdt.Rows)
            {
                if (row[3].ToString() != "")
                {
                    DateTime realdate = Convert.ToDateTime(row[3].ToString());
                    ready.Columns.Add(new DataColumn());
                    ready.Columns.Add(new DataColumn());
                    ready.Columns.Add(new DataColumn());
                    ready.Columns.Add(new DataColumn());
                    ready.Columns[0].ColumnName = "user";
                    ready.Columns[1].ColumnName = "task";
                    ready.Columns[2].ColumnName = "status";
                    ready.Columns[3].ColumnName = "date";
                    if (row[2].ToString() == "выполнил" && realdate >= Convert.ToDateTime(date1) && realdate <= Convert.ToDateTime(date2))
                    {
                        ready.NewRow();
                        ready.Rows.Add(row.ItemArray);
                    }
                }
            }
            Dictionary<string, int> dic = new Dictionary<string, int>();
            List<string> users = new List<string>();
            foreach (DataRow row in answerdt.Rows)
            {
                users.Add(row[0].ToString());
            }
            users = users.ToArray().Distinct().ToList();
            List<int> done = new List<int>();
            for (int i = 0; i < users.Count; i++)
            {dic.Add(users[i].ToString(), 0); }
            foreach (DataRow row in answerdt.Rows)
            {
                string us = row[0].ToString();
                if (row[2].ToString() == "выполнил")
                    dic[us] += 1;       
            }
            chart1.ChartAreas[0].AxisY.Interval = 1;
            int ss = 0;
            while (ss < users.Count)
            {
                string bal1 = users[ss].ToString();//user
                int bal2 = dic[bal1];
                chart1.Series[0].Points.AddXY(bal1, bal2);
                ss++;
            }
        }
    }
}
