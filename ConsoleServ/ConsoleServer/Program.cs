using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.OleDb;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;


namespace ConsoleServer
{
    class Program
    {
        static DataTable tbl = new DataTable();
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        
        static void Main(string[] args)
        {
            string filename = "\\database.xls";
            string path = Process.GetCurrentProcess().MainModule.FileName;
            string path2 = Path.GetDirectoryName(path);
            path2 += filename;
            Connect.Path2 = path2;
            Console.WriteLine(Connect.Path2);
            OleDbDataAdapter adap = new OleDbDataAdapter();
            //DataSet ds = new DataSet("EXCEL");
            DataTable dt = new DataTable();
            Connect.ConStr = ("Provider=Microsoft.Jet.OLEDB.4.0;" + ("Data Source="
                  + ((Connect.Path2) + (";" + "Extended Properties=\"Excel 8.0;\""))));
            try
            {
                Connect.CreateDataSet();
                //Console.WriteLine(Connect.data.Tables[1].Rows.Count.ToString());
                //Console.WriteLine(Connect.data.Tables[1].Rows[2][1].ToString());
                //Console.WriteLine(Connect.data.Tables[1].Rows.Count.ToString());
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
                //создаем сокет сервера:
                TcpListener slistener = new TcpListener(IPAddress.Any, 1234);
                //slistener..Bind(new IPEndPoint(IPAddress.Any, 1111));//Связывает сокет с локальной конечной точкой клиента
                //slistener.Listen(10);//максимальное количество клиентов, одновременно отправляющих данные или запросы
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName()).Where(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToArray();
                string ip = localIPs[0].ToString();
                Console.WriteLine("Server started, "+ ip);
                slistener.Start();//запускает ожидание входящих запросов на подключение

                TcpClient sclient; 
                NetworkStream stream;
                byte[] bytesWrite;
                byte[] bytesRead;
                string answer = "";
                string login;
                string password;
            string query;

            while (true)
            {
                sclient = slistener.AcceptTcpClient();
                stream = sclient.GetStream();
                bytesRead = new byte[1024];
                int length = stream.Read(bytesRead, 0, bytesRead.Length);
                string request = Encoding.Unicode.GetString(bytesRead, 0, length);


                if (request.EndsWith("avt"))
                {
                    login = request.Substring(0, request.IndexOf(" "));
                    password = request.Substring(request.IndexOf(" ") + 1, request.IndexOf(" avt") - (request.IndexOf(" ") + 1));
                    Console.WriteLine(login);
                    Console.WriteLine(password);
                    if (login == "Admin" && password == "0")
                    {
                        answer = "oks";
                        dt = Connect.data.Tables[0];
                    }
                    else
                    {
                        int k = 0;
                        for (int Cnum = 0; Cnum < Connect.data.Tables[1].Rows.Count; Cnum++)
                        {
                            k += 1;
                            if (login == (Connect.data.Tables[1].Rows[Cnum][0]).ToString() && password == (Connect.data.Tables[1].Rows[Cnum][1]).ToString())
                            {
                                answer = "ok";
                                dt = Connect.data.Tables[0];
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (dr[0].ToString() != login || dr[2].ToString() == "выполнил")
                                        dr.Delete();
                                }
                                dt.AcceptChanges();
                                break;
                            }
                        }
                        if (k == Connect.data.Tables[1].Rows.Count && login != "Admin" && password != "0")
                        {
                            answer = "no";
                        }
                    }

                    bytesWrite = Encoding.Unicode.GetBytes(answer);
                    stream.Write(bytesWrite, 0, bytesWrite.Length);
                    stream.Flush();//заливаем ответ сервера

                    try
                    {
                        if (answer != "no")
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr[0].ToString() == "" && dr[1].ToString() == "")
                                    dr.Delete();
                            }
                            dt.AcceptChanges();
                            bytesWrite = Connect.GetBinaryFormatData(dt);
                            stream.Write(bytesWrite, 0, bytesWrite.Length);
                            stream.Flush();//заливаем ответ-таблицу сервера 
                            sclient.Close();
                        }
                        if (answer == "no")
                            sclient.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }

                if (request.EndsWith("sqlupdate"))
                {
                    query = request.Substring(0, request.IndexOf(" sqlupdate"));
                    using (OleDbConnection connect = new OleDbConnection(Connect.ConStr))
                    {
                        try
                        {
                            connect.Open();

                            OleDbCommand command = new OleDbCommand(query, connect);
                            Connect.adapt = new OleDbDataAdapter();
                            Connect.adapt.UpdateCommand = command;
                            //Connect.adapt.UpdateCommand.Connection = connect;
                            //Connect.adapt.UpdateCommand = command;
                            
                            Console.WriteLine(Connect.adapt.UpdateCommand.CommandText);

                            OleDbCommandBuilder builder = new OleDbCommandBuilder(Connect.adapt);

                            //Connect.adapt.UpdateCommand = builder.GetUpdateCommand();
                            //Connect.adapt.Update(tbl);
                            //Connect.adapt.Update(Connect.data.Tables[0]);
                            Connect.adapt.UpdateCommand.ExecuteNonQuery();
                            Connect.data.Tables[0].AcceptChanges();
                            Console.WriteLine("Row(s) Updated !! ");
                            query = "SELECT * FROM [Server$]";
                            command = new OleDbCommand(query,connect);
                            Connect.adapt.SelectCommand = command;
                            tbl.Clear();
                            tbl.Locale = System.Globalization.CultureInfo.InvariantCulture;
                            Connect.adapt.Fill(tbl);
                            connect.Close();

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                    sclient.Close();
                }

                if (request.EndsWith("sqlinsert"))
                {
                    query = request.Substring(0, request.IndexOf(" sqlinsert"));
                    using (OleDbConnection connect = new OleDbConnection(Connect.ConStr))
                    {
                        try
                        {
                            connect.Open();

                            OleDbCommand command = new OleDbCommand(query, connect);
                            Connect.adapt = new OleDbDataAdapter();
                            Connect.adapt.InsertCommand = command;
                            Console.WriteLine(Connect.adapt.InsertCommand.CommandText);

                            OleDbCommandBuilder builder = new OleDbCommandBuilder(Connect.adapt);

                            //adapter.Fill(dataSet);
                            //Connect.adapt.Update(Connect.data.Tables[0]);
                            Connect.adapt.InsertCommand.ExecuteNonQuery();
                            Connect.data.Tables[0].AcceptChanges();
                            Console.WriteLine("Row(s) Inserted !! ");
                            query = "SELECT * FROM [Server$]";
                            command = new OleDbCommand(query, connect);
                            Connect.adapt.SelectCommand = command;
                            tbl.Clear();
                            tbl.Locale = System.Globalization.CultureInfo.InvariantCulture;
                            Connect.adapt.Fill(tbl);
                            connect.Close();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                    sclient.Close();
                }

                if (request.EndsWith("ClientRefresh"))
                {
                    login = request.Substring(0, request.IndexOf(" "));
                    Console.WriteLine(login + "'");
                    //using (OleDbConnection connect = new OleDbConnection(Connect.ConStr))
                    //{
                    //    connect.Open();
                    //    dt = Connect.data.Tables[0];
                    //}
                    dt = tbl;
                    foreach (DataRow dr in dt.Rows)
                            {
                                if (dr[0].ToString() != login || dr[2].ToString() == "выполнил")
                                    dr.Delete();
                            }
                            dt.AcceptChanges();
                    Console.WriteLine(dt.Rows.Count.ToString());
                    bytesWrite = Connect.GetBinaryFormatData(dt);
                            stream.Write(bytesWrite, 0, bytesWrite.Length);
                            stream.Flush();//заливаем ответ-таблицу сервера 
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Console.WriteLine(e.ToString());
                    //    }
                    //}
                    sclient.Close();
                }

                    if (request.EndsWith("AdminRefresh"))
                {
                    //using (OleDbConnection connect = new OleDbConnection(Connect.ConStr))
                    //{
                    //    connect.Open();
                    //    dt = Connect.data.Tables[0];
                    //}
                    dt = tbl;
                    try
                    {                   
                            bytesWrite = Connect.GetBinaryFormatData(dt);
                            stream.Write(bytesWrite, 0, bytesWrite.Length);
                            stream.Flush();//заливаем ответ-таблицу сервера 
                            sclient.Close();                   
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                if (request.EndsWith("gettable"))
                {
                    using (OleDbConnection connect = new OleDbConnection(Connect.ConStr))
                    {
                        connect.Open();
                        dt = Connect.data.Tables[0];
                    }
                    bytesWrite = Connect.GetBinaryFormatData(dt);
                    stream.Write(bytesWrite, 0, bytesWrite.Length);
                    stream.Flush();//заливаем ответ-таблицу сервера 
                    sclient.Close();
                }

                if (request.EndsWith("stop server"))
                {
                    sclient.Close();
                    break;
                }
            }
                slistener.Stop();
                Console.WriteLine("Server stopped");
            Console.ReadKey();
        }
    }
}

