using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace testMySql
{
    class Program
    {
        static void Main(string[] args)
        {
            string conStr = "Database=cowdb;Data Source=127.0.0.1;User Id=root;Password=andy123456789;pooling=false;CharSet=utf8;port=3306";
            MySqlConnection mySqlConn = new MySqlConnection(conStr);
            mySqlConn.Open();
            string comStr = "select * from cow";
            MySqlCommand comm = new MySqlCommand(comStr, mySqlConn);
            MySqlDataReader dr = comm.ExecuteReader();
            while (dr.Read()) {
                Console.WriteLine(dr.GetString(2));
            }



            mySqlConn.Close();
        }
    }
}
