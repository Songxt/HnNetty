using System;
using System.Data;
using Dapper.Contrib.Extensions;
using Npgsql;

namespace HnDb
{
    public class DbHelper
    {
        private static string _conn;

        public static void Initialization(string conn)
        {
            _conn = conn;
        }

        public static void Initialization(string conn, DbType dbType)
        {
            _conn = conn;
            SetDbType(dbType);
        }

        public static IDbConnection Connect()
        {
            if (_conn == String.Empty)
            {
                return null;
            }
            return new NpgsqlConnection(_conn);
        }

        public static void SetDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.NPGSQL:
                    SqlMapperExtensions.GetDatabaseType += connection =>
                    {
                        return "npgsqlconnection";
                    };
                    break;
                default:
                    SqlMapperExtensions.GetDatabaseType += connection =>
                    {
                        return "sqlconnection";
                    };
                    break;
            }
        }

        public enum DbType
        {
            SQLSEVER,
            SQLCE,
            MYSQL,
            SQLITE,
            NPGSQL
        }
    }
}