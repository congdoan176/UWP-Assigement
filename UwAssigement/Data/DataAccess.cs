using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwAssigement.Data
{
    class DataAccess
    {
        public static string SQLite_Connection_String = "Filename=sqliteSample.db";
        public static void InitializeDatabase()
        {
            using (SqliteConnection db =
                new SqliteConnection(SQLite_Connection_String))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Contacts (Phone NVARCHAR(50) PRIMARY KEY, " +
                    "Email NVARCHAR(50) NULL, Name NVARCHAR(50) NOT NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }
    }
}
