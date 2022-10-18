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
    public partial class Form3 : Form
    {
        DataTable dt = new DataTable();
        bool vis = true;
        static byte[] bytesWrite;
        static byte[] bytesRead;
        static string login;
        static string password;

        public Form3()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (vis)
            {
                textBox2.UseSystemPasswordChar = false;
                vis = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
                vis = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //try
            //{
                Connect.Sclient = new TcpClient();
            Connect.Sclient.Connect(A.ip, 1234);
                Connect.Stream = Connect.Sclient.GetStream();
                login = textBox1.Text;
                password = textBox2.Text;
                //bytesWrite=Connect.GetBinaryCode(request);
                bytesWrite = Encoding.Unicode.GetBytes(login +" "+password+" avt");
                Connect.Stream.Write(bytesWrite, 0, bytesWrite.Length);//залили запрос серверу
                Connect.Stream.Flush();//удалили данные из потока

                bytesRead = new byte[10];
                int length = Connect.Stream.Read(bytesRead, 0, bytesRead.Length);
                string answer = Encoding.Unicode.GetString(bytesRead, 0, length);
            if (answer == "oks")
            {
                bytesRead = new byte[20000];
                length = Connect.Stream.Read(bytesRead, 0, bytesRead.Length);

                try
                {
                    DataTable answerdt = Connect.GetDataTable(bytesRead);
                    Form form2 = new Form2(answerdt);
                    form2.Show();
                    this.Hide();
                }
                catch (Exception er)
                { MessageBox.Show(er.Message);
                }
            }
            if (answer == "ok")
            {
                bytesRead = new byte[20000];
                length = Connect.Stream.Read(bytesRead, 0, bytesRead.Length);

                try
                {
                    DataTable answerdt = Connect.GetDataTable(bytesRead);
                    Form form1 = new Form1(login, answerdt);
                    form1.Show();
                    this.Hide();
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message);
                }
            }
            if (answer == "no")
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = null;
                textBox2.Text = null;
                Form form3 = new Form3();
                form3.Show();
                this.Close();
            }

            //}

            //catch (Exception err)
            //{
            //    MessageBox.Show(err.Message);
            //}

        }
    }
}
