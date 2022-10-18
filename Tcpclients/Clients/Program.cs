using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using ExcelObj = Microsoft.Office.Interop.Excel;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Diagnostics;



namespace Clients
{ 
    static class Program
    {   
        
        
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new Form3());
        }
    }
}
