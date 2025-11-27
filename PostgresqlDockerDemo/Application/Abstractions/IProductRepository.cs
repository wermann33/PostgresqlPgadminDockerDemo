using PostgresqlDockerDemo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresqlDockerDemo.Application.Abstractions
{
    public interface IProductRepository
    {
        long Create(string name, decimal price);
        Product? GetById(long id);
        bool Update(long id, string name, decimal price);
        bool Delete(long id);
        IReadOnlyList<Product> List();
        int Count();
    }
}
