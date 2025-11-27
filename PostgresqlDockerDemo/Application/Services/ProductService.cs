using PostgresqlDockerDemo.Application.Abstractions;
using PostgresqlDockerDemo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresqlDockerDemo.Application.Services
{
    internal sealed class ProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public long Create(string name, decimal price)
        {
            Validate(name, price);
            return _repo.Create(name.Trim(), price);
        }

        public bool Update(long id, string name, decimal price)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            Validate(name, price);
            return _repo.Update(id, name.Trim(), price);
        }

        public bool Delete(long id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _repo.Delete(id);
        }

        public Product? Get(long id) => _repo.GetById(id);

        public IReadOnlyList<Product> ListSortedByPrice(bool ascending = true)
        {
            var all = _repo.List();
            return ascending
                ? all.OrderBy(p => p.Price).ToList()
                : all.OrderByDescending(p => p.Price).ToList();
        }

        private static void Validate(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name must not be empty.");
            if (name.Length > 200)
                throw new ArgumentException("Name too long (max 200 chars).");
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price must be >= 0.");
        }
    }
}
