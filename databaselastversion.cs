using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace SQLiteDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            //---------------------
            int item = 1;
            Console.WriteLine("1. Display all data: ");
            Console.WriteLine("2. Travels to given places: ");
            Console.WriteLine("3. Insert data: ");
            Console.WriteLine("4. Insert data: ");
            Console.WriteLine("0. Exit ");
            item = Int32.Parse(Console.ReadLine());

            if (item == 0)
            {
                Environment.Exit(0);
            }

                   
            if (item == 1)
            {
                SQLiteConnection sqlite_conn;
                sqlite_conn = CreateConnection();
                ReadData(sqlite_conn);
            }

            if (item == 2)
            {
                Console.WriteLine("Show all the trips to: ");
                string place =Console.ReadLine();
                SQLiteConnection sqlite_conn;
                sqlite_conn = CreateConnection();
                ReadDataPlaces(sqlite_conn, place);
            }


            if (item == 3)
            {
                SQLiteConnection sqlite_conn;
                sqlite_conn = CreateConnection();
                CreateTable(sqlite_conn);
                InsertData(sqlite_conn);
            }

            if (item == 4)
            {
                Console.WriteLine("What item you want to delete: ");
                int id = Int32.Parse(Console.ReadLine());
                SQLiteConnection sqlite_conn;
                sqlite_conn = CreateConnection();
                DeletePlaces(sqlite_conn, id);
            }
           
        }

        static SQLiteConnection CreateConnection()
        {            
            SQLiteConnection sqlite_conn;            
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True; ");
           // Open the connection:
         try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database no found");
            }
            return sqlite_conn;
        }

        static void CreateTable(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            string strCosts = "CREATE TABLE if not exists  Costs(id INTEGER PRIMARY KEY AUTOINCREMENT, cost double, place_id INT)";
            string strPlaces = "CREATE TABLE if not exists Places(id INTEGER PRIMARY KEY AUTOINCREMENT, place VARCHAR(20), date VARCHAR(20))";
            sqlite_cmd = conn.CreateCommand();
           
            sqlite_cmd.CommandText = strCosts;
            sqlite_cmd.ExecuteNonQuery();

            sqlite_cmd.CommandText = strPlaces;
            sqlite_cmd.ExecuteNonQuery();

            //conn.Close();
        }

        static void InsertData(SQLiteConnection conn)
        {
            Console.Write("Enter the place: ");
            string place = Console.ReadLine();

            Console.Write("Enter the date: ");
            string date1 = Console.ReadLine();

            Console.Write("Enter the cost: ");
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            double cost = Convert.ToDouble(Console.ReadLine(), provider);

            SQLiteCommand sqlite_cmd;

            sqlite_cmd = conn.CreateCommand();          
            sqlite_cmd.CommandText = "INSERT INTO Places(place, date) VALUES('" +  place + "','"  + date1+"' ); ";
            //Console.WriteLine(sqlite_cmd.CommandText);
            sqlite_cmd.ExecuteNonQuery();
            //------------------------
            SQLiteDataReader sqlite_datareader;
            //SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Places ";
            int place_id = 0;
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                place_id = (int)sqlite_datareader.GetInt64(0);        
            }
            //------------------------
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO Costs(cost, place_id) VALUES(" + cost + "," + place_id + " ); ";
            Console.WriteLine(sqlite_cmd.CommandText);
            sqlite_cmd.ExecuteNonQuery();

            conn.Close();
        }

        static void ReadData(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT places.id, places.place, places.date, costs.cost FROM Places, Costs WHERE places.id = costs.place_id";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
           
            while (sqlite_datareader.Read())
            {                
                int col0 = (int)sqlite_datareader.GetInt64(0);
                string col1 = sqlite_datareader.GetString(1);
                string col2 = sqlite_datareader.GetString(2);
                Console.WriteLine("{0} {1} {2}", col0, col1, col2);
            }
            conn.Close();
        }

        static void ReadDataPlaces(SQLiteConnection conn, string place)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT places.place, places.date, costs.cost FROM Places, Costs WHERE places.id = costs.place_id and " + "'" + place + "' = places.place;" ;
            Console.WriteLine(sqlite_cmd.CommandText);
           
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                //int col0 = (int)sqlite_datareader.GetInt64(0);
                string col1 = sqlite_datareader.GetString(0);
                string col2 = sqlite_datareader.GetString(1);
                Console.WriteLine("{0} {1}", col1, col2);
            }
           
            conn.Close();
        }

        static void DeletePlaces(SQLiteConnection conn, int id)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            SQLiteCommand sqlite_cmd_2;            
            sqlite_cmd_2 = conn.CreateCommand();

            sqlite_cmd.CommandText = "Delete FROM Places WHERE places.id = " + id + " ;";
            sqlite_cmd_2.CommandText = "Delete FROM Costs WHERE costs.place_id = " + id + " ;";

            //Console.WriteLine(sqlite_cmd.CommandText);

            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd_2.ExecuteNonQuery();

            conn.Close();
        }



    }
}