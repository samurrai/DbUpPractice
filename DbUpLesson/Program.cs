using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using DbUp;
using System.Reflection;

namespace DbUpLesson
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["appConnection"].ConnectionString;
        
        static void Main(string[] args)
        {
            CheckMigrations();
            var user = new User
            {
                Login = "admin",
                Password = "123456"
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute("insert into users values(@Id, @Login, @Password)", user);
            }
        }

        private static void CheckMigrations()
        {
            EnsureDatabase.For.SqlDatabase(_connectionString);

            var upgrader = DeployChanges.To
            .SqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception("Ошибка соединения");
            }
        }
    }
}
