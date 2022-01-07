using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using Dapper;
using System.Threading.Tasks;
using EpubWpf.Sqlite;

namespace EpubWpf
{
    internal class SqliteHelper
    {
        private SQLiteConnection connection;
        public SqliteHelper()
        {
            connection = new SQLiteConnection(@"Data Source=C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Kitaplar.sqlite");
        }
        public List<SqliteBook> GetBooks()
        {
            connection.Open();
            var sql = "select * from Kitaplar";
            var a = connection.Query<SqliteBook>(sql);
            connection.Close();
            return a.ToList();
        }
        public void InsertBook(string foldername)
        {
            if (!GetBooks().Any(X=>X.FolderName==foldername))
            {
                connection.Open();
                var paramaters = new { FolderName = foldername };
                var sql = "INSERT INTO Kitaplar(FolderName) VALUES(@FolderName)";
                connection.Execute(sql, paramaters);
                connection.Close();
            }
        }
    }
}
