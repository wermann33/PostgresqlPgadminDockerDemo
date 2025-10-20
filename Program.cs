using Npgsql;
using PostgresqlDockerDemo.Application.Abstractions;
using PostgresqlDockerDemo.Application.Services;
using PostgresqlDockerDemo.Infrastructure.Database;
using PostgresqlDockerDemo.Infrastructure.Repositorys;
using System;
using System.Threading.Tasks;

class Program
{
    //static async Task<int> Main()
    //{
    //    var host = Environment.GetEnvironmentVariable("PGHOST") ?? "localhost";
    //    var port = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
    //    var user = Environment.GetEnvironmentVariable("PGUSER") ?? "app";
    //    var pass = Environment.GetEnvironmentVariable("PGPASSWORD") ?? "appsecret";
    //    var db = Environment.GetEnvironmentVariable("PGDATABASE") ?? "appdb";

    //    var cs = $"Host={host};Port={port};Username={user};Password={pass};Database={db};Pooling=true;Timeout=5;Command Timeout=5";

    //    try
    //    {
    //        await using var conn = new NpgsqlConnection(cs);

    //        const int maxAttempts = 10;
    //        var attempt = 0;
    //        while (true)
    //        {
    //            try
    //            {
    //                await conn.OpenAsync();
    //                break;
    //            }
    //            catch (Exception ex) when (attempt < maxAttempts)
    //            {
    //                attempt++;
    //                Console.WriteLine($"[PgPing] Verbindung noch nicht bereit (Versuch {attempt}/{maxAttempts}): {ex.Message}");
    //                await Task.Delay(1000);
    //            }
    //        }

    //        await using var cmd = new NpgsqlCommand("select version()", conn);
    //        var version = (string?)await cmd.ExecuteScalarAsync();

    //        Console.WriteLine("[PgPing] ✅ Verbindung steht!");
    //        Console.WriteLine($"[PgPing] PostgreSQL-Version: {version}");
    //        return 0;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.Error.WriteLine("[PgPing] ❌ Verbindung fehlgeschlagen:");
    //        Console.Error.WriteLine(ex);
    //        return 1;
    //    }
    //}

    static int Main()
    {
        try
        {
            // Setup
            var factory = new NpgsqlConnectionFactory();
            SchemaInitializer.EnsureCreated(factory.ConnectionString);

            IProductRepository repo = new ProductRepository(factory);
            var service = new ProductService(repo);

            // CREATE
            var id1 = service.Create("USB-C Cable", 9.99m);
            var id2 = service.Create("Laptop Stand", 29.90m);
            var id3 = service.Create("Mouse", 14.50m);

            Console.WriteLine("Products created.");

            // READ + LIST
            var all = service.ListSortedByPrice();
            Console.WriteLine("\nAll Products (sorted by price):");
            foreach (var p in all)
                Console.WriteLine($"{p.Id,-3} {p.Name,-20} {p.Price,6:C}");

            Console.WriteLine($"\nTotal count: {repo.Count()}");

            // UPDATE
            service.Update(id1, "USB-C Cable (2m)", 12.49m);
            Console.WriteLine("\nUpdated Product 1.");

            // DELETE
            service.Delete(id3);
            Console.WriteLine("Deleted Product 3.");

            // SHOW RESULT
            var remaining = service.ListSortedByPrice();
            Console.WriteLine("\nRemaining Products:");
            foreach (var p in remaining)
                Console.WriteLine($"{p.Id,-3} {p.Name,-20} {p.Price,6:C}");

            Console.WriteLine($"\nTotal count: {repo.Count()}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error:");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }
}
