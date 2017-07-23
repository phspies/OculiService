using OculiService.CloudProviders.Oculi.Contracts.Workload;
using SQLite.CodeFirst;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;

namespace OculiService.Common.Database
{
    [DbConfigurationType(typeof(SQLiteConfiguration))]
    public class OculiServiceDatabaseContext : DbContext
    {
        public OculiServiceDatabaseContext() : base(GetConnection(), true) { }

        public DbSet<OculiWorkloadType> Workloads { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<OculiServiceDatabaseContext>(modelBuilder);
            System.Data.Entity.Database.SetInitializer(sqliteConnectionInitializer);
        }
        public static DbConnection GetConnection()
        {
            string dblocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string dbfilename = Path.Combine(dblocation, "Database", "OculiServiceDatabase.sqlite3");
            if (!Directory.Exists(Path.GetDirectoryName(dbfilename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbfilename));
            }
            DbProviderFactory providerFactory = DbProviderFactories.GetFactory("System.Data.SQLite.EF6");
            var connection = providerFactory.CreateConnection();
            string dbfullpath = Path.Combine(dblocation, dbfilename);
            SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder();
            conString.DataSource = dbfilename;
            conString.DefaultTimeout = 5000;
            conString.SyncMode = SynchronizationModes.Normal;
            conString.JournalMode = SQLiteJournalModeEnum.Default;
            conString.PageSize = 65536;
            conString.CacheSize = 16777216;
            conString.FailIfMissing = false;
            //conString.HexPassword = new byte[] { 66, 21, 32, 26, 69, 63, 63, 4e, 37, 73, 56, 49, 23, 38, 39, 51 };
            conString.ReadOnly = false;
            connection.ConnectionString = conString.ConnectionString;
            return connection;
        }
        public class SQLiteConfiguration : DbConfiguration
        {
            public SQLiteConfiguration()
            {
                SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
                SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
                SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
            }
        }
    }
}
