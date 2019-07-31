using System;

namespace EIP.System.DataAccess.Fixture
{
    /// <summary>
    /// 
    /// </summary>
    public class MsSqlDatabaseFixture : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public MsSqlDatabaseFixture()
        {
            var connString = "server=.;database=EIP;uid=sa;pwd=P@ssw0rd";
            Db = new MsSqlDbContext(connString);
        }
        /// <summary>
        /// 
        /// </summary>
        public MsSqlDbContext Db { get; }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Db.Dispose();
        }
    }
}