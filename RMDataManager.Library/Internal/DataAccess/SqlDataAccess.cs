using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Dapper;

using Microsoft.Extensions.Configuration;

namespace RMDataManager.Library.Internal.DataAccess
{
    public class SqlDataAccess : IDisposable, ISqlDataAccess
    {
        private IDbConnection _connection;
        private bool isClosed;
        private readonly IConfiguration _config;

        public IDbTransaction Transaction { get; set; }

        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }

        public SqlDataAccess()
        {
        }

        private string GetConnectionString(string name)
        {
            if (_config == null)
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            else
            {
                return _config.GetConnectionString(name);
            }
            //return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return rows;
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            Transaction = _connection.BeginTransaction();
            isClosed = false;
        }

        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: Transaction);
        }

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            List<T> rows = _connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: Transaction).ToList();
            return rows;
        }

        public void CommitTransaction()
        {
            Transaction?.Commit();
            _connection?.Close();
            isClosed = true;
        }

        public void RollbackTransaction()
        {
            Transaction?.Rollback();
            _connection?.Close();
            isClosed = true;
        }

        public void Dispose()
        {
            if (!isClosed)
            {
                try
                {
                    CommitTransaction();
                }
                catch (Exception)
                {
                    //TODO
                }
            }

            Transaction = null;
            _connection = null;
        }
    }
}
