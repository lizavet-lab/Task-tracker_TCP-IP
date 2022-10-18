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
namespace Clients
{
    public partial class Form2 : Form
    {
        static string user;
        static string task;
        static byte[] bytesWrite;
        static byte[] bytesRead;
        static DataTable ex;
        
        public Form2(DataTable dt)
        {
            ex = dt.Copy();
            InitializeComponent();
            //this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.DataSource = dt;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int str = dataGridView1.CurrentCell.RowIndex;
            user = dataGridView1[0, str].Value.ToString();
            task = dataGridView1[1, str].Value.ToString();
            Connect.Sclient = new TcpClient();
            Connect.Sclient.Connect(A.ip, 1234);
            Connect.Stream = Connect.Sclient.GetStream();
            bytesWrite = Encoding.Unicode.GetBytes("insert into [Server$] ([user], [task]) Values('" + user + " ',' " + task + "')" + " sqlinsert");
            Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
            Connect.Stream.Flush();//удалили данные из потока
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Connect.Sclient = new TcpClient();
            Connect.Sclient.Connect(A.ip, 1234);
            Connect.Stream = Connect.Sclient.GetStream();
            bytesWrite = Encoding.Unicode.GetBytes(button2.Text);
            Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
            Connect.Stream.Flush();//удалили данные из потока
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Connect.Sclient = new TcpClient();
            Connect.Sclient.Connect(A.ip, 1234);
            Connect.Stream = Connect.Sclient.GetStream();
            bytesWrite = Encoding.Unicode.GetBytes("AdminRefresh");
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
            Form form2 = new Form2(answerdt);
            form2.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form form4 = new Form4();
            form4.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.OwningColumn.Name == "user")
            {
                int str = dataGridView1.CurrentCell.RowIndex;
                task = dataGridView1[1, str].Value.ToString();
                string user1 = ex.Rows[str][0].ToString();
                user = dataGridView1[0, str].Value.ToString();
                Connect.Sclient = new TcpClient();
                Connect.Sclient.Connect(A.ip, 1234);
                Connect.Stream = Connect.Sclient.GetStream();
                bytesWrite = Encoding.Unicode.GetBytes("UPDATE [Server$] SET [Server$].[user]='" + user + "' WHERE (task='" + task + "' and user='" + user1 + "')" + " sqlupdate");
                Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
                Connect.Stream.Flush();//удалили данные из потока
            }
            if (dataGridView1.CurrentCell.OwningColumn.Name == "task")
            {
                int str = dataGridView1.CurrentCell.RowIndex;
                user = dataGridView1[0, str].Value.ToString();
                string task1 = ex.Rows[str][1].ToString();
                task = dataGridView1[1, str].Value.ToString();

                Connect.Sclient = new TcpClient();
                Connect.Sclient.Connect(A.ip, 1234);
                Connect.Stream = Connect.Sclient.GetStream();
                bytesWrite = Encoding.Unicode.GetBytes("UPDATE [Server$] SET [Server$].[task]='" + task + "' WHERE (user='" + user + "' and task='" + task1 + "')" + " sqlupdate");
                Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
                Connect.Stream.Flush();//удалили данные из потока
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int str = dataGridView1.CurrentCell.RowIndex;
            user = dataGridView1[0, str].Value.ToString();
            task = dataGridView1[1, str].Value.ToString();
            Connect.Sclient = new TcpClient();
            Connect.Sclient.Connect(A.ip, 1234);
            Connect.Stream = Connect.Sclient.GetStream();
            bytesWrite = Encoding.Unicode.GetBytes("UPDATE [Server$] SET [Server$].[user]='', [Server$].[task]='', [Server$].[status]='', [Server$].[date]='' WHERE (user='" + user + "' and task='" + task + "')" + " sqlupdate");
            Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
            Connect.Stream.Flush();//удалили данные из потока
            dataGridView1.Rows.RemoveAt(str);
        }
    }
}