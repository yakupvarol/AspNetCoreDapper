using System.Data;

namespace AspNetCoreDapper.Dapper
{
    public interface IDBConnection 
    {
        IDbConnection DB { get; }
        void Dispose();
    }
}
