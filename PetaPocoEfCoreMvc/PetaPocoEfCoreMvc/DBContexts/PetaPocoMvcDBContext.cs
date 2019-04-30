using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetaPocoEfCoreMvc.DBContext
{
    using System.Data.Common;

    using PetaPoco;
    using PetaPoco.Core;

    public class PetaPocoMvcDBContext:Database
    {
        public PetaPocoMvcDBContext(DbConnection connection, IMapper defaultMapper = null)
            : base(connection, defaultMapper)
        {
        }

        public PetaPocoMvcDBContext(string connectionString, string providerName, IMapper defaultMapper = null)
            : base(connectionString, providerName, defaultMapper)
        {
        }

        public PetaPocoMvcDBContext(string connectionString, DbProviderFactory factory, IMapper defaultMapper = null)
            : base(connectionString, factory, defaultMapper)
        {
        }

        public PetaPocoMvcDBContext(string connectionString, IProvider provider, IMapper defaultMapper = null)
            : base(connectionString, provider, defaultMapper)
        {
        }

        public PetaPocoMvcDBContext(IDatabaseBuildConfiguration configuration)
            : base(configuration)
        {
        }
    }
}
