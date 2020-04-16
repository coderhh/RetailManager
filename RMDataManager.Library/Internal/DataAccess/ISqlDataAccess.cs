using System.Collections.Generic;
using System.Data;

namespace RMDataManager.Library.Internal.DataAccess
{
    public interface ISqlDataAccess
    {
        IDbTransaction Transaction { get; set; }

        void CommitTransaction();
        void Dispose();
        List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);
        List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters);
        void RollbackTransaction();
        void SaveData<T>(string storedProcedure, T parameters, string connectionStringName);
        void SaveDataInTransaction<T>(string storedProcedure, T parameters);
        void StartTransaction(string connectionStringName);
    }
}