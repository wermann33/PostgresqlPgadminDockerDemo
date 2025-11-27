using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresqlDockerDemo.Infrastructure.Database
{
    internal interface IDbConnectionFactory
    {
        DbConnection CreateOpenConnection();
        string ConnectionString { get; }
    }
}
