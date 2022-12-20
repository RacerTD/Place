using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace PlaceFiller
{
    public class DBManager
    {
        private string connectionString = "Data Source=database.db;Version=3";

        public void Do()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
            }
        }

        /// <summary>
        /// Creates a new Table in the databank
        /// </summary>
        /// <param name="tableSpecifications">the specifications for the new table, must be in the proper format</param>
        public void CreateTable(string tableSpecifications)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(tableSpecifications, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddDataToTable<T>(string tableName, T data)
        {

        }

        public void AddDataToTable<T>(string tableName, List<T> data)
        {
            for(int i = 0; i < data.Count; i++)
            {
                AddDataToTable(tableName, data[i]);
            }
        }
    }
}
