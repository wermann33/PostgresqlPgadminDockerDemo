using Npgsql;
using PostgresqlDockerDemo.Application.Abstractions;
using PostgresqlDockerDemo.Domain;
using PostgresqlDockerDemo.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresqlDockerDemo.Infrastructure.Repositorys
{
    internal sealed class ProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _factory;
        public ProductRepository(IDbConnectionFactory factory) => _factory = factory;

        public long Create(string name, decimal price)
        {
            const string sql = @"
                INSERT INTO products (name, price)
                VALUES (@name, @price)
                RETURNING id;";

            using var conn = (NpgsqlConnection)_factory.CreateOpenConnection();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", NpgsqlTypes.NpgsqlDbType.Text, name);
            cmd.Parameters.AddWithValue("@price", NpgsqlTypes.NpgsqlDbType.Numeric, price);

            var result = cmd.ExecuteScalar();
            return Convert.ToInt64(result);
        }

        public Product? GetById(long id)
        {
            const string sql = @"
                SELECT id, name, price, created_at
                FROM products
                WHERE id = @id;";

            using var conn = (NpgsqlConnection)_factory.CreateOpenConnection();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            return reader.Read() ? Map(reader) : null;
        }

        public bool Update(long id, string name, decimal price)
        {
            const string sql = @"UPDATE products SET name=@name, price=@price WHERE id=@id;";
            using var conn = (NpgsqlConnection)_factory.CreateOpenConnection();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@price", price);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(long id)
        {
            const string sql = @"DELETE FROM products WHERE id=@id;";
            using var conn = (NpgsqlConnection)_factory.CreateOpenConnection();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        public IReadOnlyList<Product> List()
        {
            const string sql = @"SELECT id, name, price, created_at FROM products ORDER BY id;";
            using var conn = (NpgsqlConnection)_factory.CreateOpenConnection();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            var list = new List<Product>();
            while (reader.Read())
                list.Add(Map(reader));
            return list;
        }

        public int Count()
        {
            const string sql = @"SELECT COUNT(*) FROM products;";
            using var conn = (NpgsqlConnection)_factory.CreateOpenConnection();
            using var cmd = new NpgsqlCommand(sql, conn);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        private static Product Map(IDataRecord r)
        {
            var id = r.GetInt64(r.GetOrdinal("id"));
            var name = r.GetString(r.GetOrdinal("name"));
            var price = r.GetDecimal(r.GetOrdinal("price"));
            var createdAt = r.GetDateTime(r.GetOrdinal("created_at"));
            return new Product(id, name, price, new DateTimeOffset(createdAt));
        }
    }
}

