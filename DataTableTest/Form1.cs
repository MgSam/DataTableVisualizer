using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTableViewer;
using NamedPipeWrapper;

namespace Test2
{
    public partial class Form1 : Form
    {
        private NamedPipeServer<String> _server;
        private NamedPipeClient<String> _client;

        public Form1()
        {
            InitializeComponent();

            //test2();
            //test(StringComparer.OrdinalIgnoreCase);

            var table = new DataTable();
            table.Columns.Add("FirstName", typeof(String));
            table.Columns.Add("LastName", typeof(String));
            table.Columns.Add("Age", typeof(int));
            table.Columns.Add("Address", typeof(String));
            table.Columns.Add("Healthy", typeof(bool));
            table.Columns.Add("Karma", typeof(double));
            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("DOB", typeof(DateTime));
            table.Rows.Add(new object[] { "Lassie", "Dog" , 5 , "111", true , 1.0, Guid.NewGuid(), new DateTime(1995,01,01) });
            table.Rows.Add(new object[] { "Rover" , "Dog" , 6 , "123", true , 1.1, Guid.NewGuid(), new DateTime(2010,10,02) });
            table.Rows.Add(new object[] { "Fido"  , "Dog" , 7 , "121", false, 1.2, Guid.NewGuid(), new DateTime(1990,01,09) });
            table.Rows.Add(new object[] { "Timmy" , "Rat" , 1 , "141", false, 1.3, Guid.NewGuid(), new DateTime(1990,02,08) });
            table.Rows.Add(new object[] { "Boo"   , "Jack", 7 , "314", false, 1.4, Guid.NewGuid(), new DateTime(1990,03,07) });
            table.Rows.Add(new object[] { "Fido"  , "Wolf", 8 , "181", false, 1.5, Guid.NewGuid(), new DateTime(1990,04,06) });
            table.Rows.Add(new object[] { "MrMeow", "Cat" , 7 , "111", true , 1.6, Guid.NewGuid(), new DateTime(1990,05,05) });
            table.Rows.Add(new object[] { "Rover" , "Dog" , 17, "151", true , 1.7, Guid.NewGuid(), new DateTime(1990,06,04) });
            table.Rows.Add(new object[] { "Par"   , "Jack", 7 , "111", true , 1.8, Guid.NewGuid(), new DateTime(1990,07,03) });
            table.Rows.Add(new object[] { "Par"   , "Jack", 7 , "111", true , 1.9, Guid.NewGuid(), new DateTime(1990,08,02) });
            table.Rows.Add(new object[] { "Par"   , "Jack", 7 , "111", true , 2.0, Guid.NewGuid(), new DateTime(1990,09,01) });

            var w = new ViewerWindow() { Table = table };
            w.ShowDialog();

            Application.Exit();
        }

        private void test<T>(IEqualityComparer<T> comparer)
        {
            var param = Expression.Parameter(typeof(IEqualityComparer<T>));
            var call = Expression.Call(param, comparer.GetType().GetMethod("Equals"), Expression.Constant("abc"), Expression.Constant("ABC"));
            var func = Expression.Lambda(call, param).Compile();
            var result = func.DynamicInvoke(comparer);
        }

        private void test2()
        {
            _server = new NamedPipeServer<String>("DataTablePipe");
            _server.ClientMessage += ServerOnClientMessage;
            _server.ClientConnected += ServerOnClientConnected;
            _server.Error += ServerError;
            _server.Start();

            _client = new NamedPipeClient<String>("DataTablePipe");
            _client.ServerMessage += ClientOnServerMessage;
            _client.Start();
            _client.WaitForConnection();

            _client.PushMessage("Dog");
            _server.PushMessage("Cat");
        }

        private void ServerError(Exception exception)
        {
            
        }

        private void ServerOnClientConnected(NamedPipeConnection<String, String> connection)
        {
            //_server.PushMessage("Cat");

        }

        private void ClientOnServerMessage(NamedPipeConnection<String, String> connection, String message)
        {
            var table = message;
        }

        private void ServerOnClientMessage(NamedPipeConnection<String, String> connection, String message)
        {
            var table = message;

        }
    }
}
