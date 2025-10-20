using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresqlDockerDemo.Domain
{
    internal sealed class Product
    {
        public long Id { get; }
        public string Name { get; }
        public decimal Price { get; }
        public DateTimeOffset CreatedAt { get; }

        public Product(long id, string name, decimal price, DateTimeOffset createdAt)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Price = price;
            CreatedAt = createdAt;
        }
    }
}
