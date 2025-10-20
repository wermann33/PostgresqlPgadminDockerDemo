using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresqlDockerDemo.Infrastructure.Database
{
    internal sealed class NpgsqlConnectionFactory : IDbConnectionFactory
    {
        public string ConnectionString { get; }

        public NpgsqlConnectionFactory(string? connectionString = null)
        {
            ConnectionString = connectionString ?? BuildFromEnv();
        }

        public DbConnection CreateOpenConnection()
        {
            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        private static string BuildFromEnv()
        {
            var host = Environment.GetEnvironmentVariable("PGHOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
            var user = Environment.GetEnvironmentVariable("PGUSER") ?? "app";
            var pass = Environment.GetEnvironmentVariable("PGPASSWORD") ?? "appsecret";
            var db = Environment.GetEnvironmentVariable("PGDATABASE") ?? "appdb";

            return $"Host={host};Port={port};Username={user};Password={pass};Database={db}";
        }
    }
}
