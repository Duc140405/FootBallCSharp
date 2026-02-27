using System.Data.Common;
using System.Data.Entity;

namespace Football_Management_System.Database
{
    public partial class FootballManagementDBEntities : DbContext
    {
        public FootballManagementDBEntities(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }
    }
}