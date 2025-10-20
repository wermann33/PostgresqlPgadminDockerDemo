using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresqlDockerDemo.Infrastructure.Database
{
    internal static class SchemaInitializer
    {
        private const string Sql = @"
                                CREATE TABLE IF NOT EXISTS products (
                                id         BIGSERIAL PRIMARY KEY,
                                name       TEXT NOT NULL,
                                price      NUMERIC(10,2) NOT NULL CHECK (price >= 0),
                                created_at TIMESTAMPTZ NOT NULL DEFAULT now()
                                );";

        public static void EnsureCreated(string connectionString)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(Sql, conn);
            cmd.ExecuteNonQuery();
        }
    }
}
