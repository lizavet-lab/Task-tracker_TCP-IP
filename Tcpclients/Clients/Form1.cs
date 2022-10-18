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
using System.Diagnostics;


namespace Clients
{
    public partial class Form1 : Form
    {
        static byte[] bytesWrite;
        static byte[] bytesRead;
        static string user;
        static string task;
        static string date;
        static string login;

        public Form1(string user, DataTable dt)
        {
            InitializeComponent();
            dataGridView1.DataSource = dt;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;            
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            login = user;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.OwningColumn.Name == "status")
            {
                dataGridView1.CurrentCell.Value = button3.Text;
                dataGridView1[dataGridView1.CurrentCell.ColumnIndex + 1, dataGridView1.CurrentCell.RowIndex].Value = DateTime.Now.ToString();
                int str = dataGridView1.CurrentCell.RowIndex;
                user = dataGridView1[0, str].Value.ToString();
                task = dataGridView1[1, str].Value.ToString();
                date = dataGridView1[3, str].Value.ToString();

                Connect.Sclient = new TcpClient();
                Connect.Sclient.Connect(A.ip, 1234);
                Connect.Stream = Connect.Sclient.GetStream();
                bytesWrite = Encoding.Unicode.GetBytes("UPDATE [Server$] SET [Server$].[status]='" + button3.Text + "'" + ", [Server$].[date]='" + date + "' WHERE (user='" + user + "' and task='" + task + "')" + " sqlupdate");
                Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
                Connect.Stream.Flush();//удалили данные из потока
            }
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.OwningColumn.Name == "status")
            {
                dataGridView1.CurrentCell.Value = button4.Text;
                dataGridView1[dataGridView1.CurrentCell.ColumnIndex + 1, dataGridView1.CurrentCell.RowIndex].Value = DateTime.Now.ToString();
                int str = dataGridView1.CurrentCell.RowIndex;
                user = dataGridView1[0, str].Value.ToString();
                task = dataGridView1[1, str].Value.ToString();
                date = dataGridView1[3, str].Value.ToString();
                Connect.Sclient = new TcpClient();
                Connect.Sclient.Connect(A.ip, 1234);
                Connect.Stream = Connect.Sclient.GetStream(); 
                bytesWrite = Encoding.Unicode.GetBytes("UPDATE [Server$] SET [Server$].[status]='" + button4.Text + "'" +", [Server$].[date]='" + date + "' WHERE (user='" + user + "' and task='" + task + "')" + " sqlupdate");
                Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
                Connect.Stream.Flush();//удалили данные из потока
                dataGridView1.Rows.RemoveAt(str);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.OwningColumn.Name == "status")
            {
                dataGridView1.CurrentCell.Value = button1.Text;
                dataGridView1[dataGridView1.CurrentCell.ColumnIndex + 1, dataGridView1.CurrentCell.RowIndex].Value = "";
                int str = dataGridView1.CurrentCell.RowIndex;
                user = dataGridView1[0, str].Value.ToString();
                task = dataGridView1[1, str].Value.ToString();
                date = "";

                Connect.Sclient = new TcpClient();
                Connect.Sclient.Connect(A.ip, 1234);
                Connect.Stream = Connect.Sclient.GetStream();
                bytesWrite = Encoding.Unicode.GetBytes("UPDATE [Server$] SET [Server$].[status]='" + button1.Text + "'" + ", [Server$].[date]='" + date + "' WHERE (user='" + user + "' and task='" + task + "')" + " sqlupdate");
                Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
                Connect.Stream.Flush();//удалили данные из потока
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Connect.Sclient = new TcpClient();
            Connect.Sclient.Connect(A.ip, 1234);
            Connect.Stream = Connect.Sclient.GetStream();
            bytesWrite = Encoding.Unicode.GetBytes(login + " ClientRefresh");
            Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
            Connect.Stream.Flush();//удалили данные из потока
            bytesRead = new byte[300000];
            Connect.Stream.Read(bytesRead, 0, bytesRead.Length);
            DataTable answerdt = Connect.GetDataTable(bytesRead);
            Form formа1 = new Form1(login, answerdt);
            formа1.Show();
            this.Close();

        }
    }
}

