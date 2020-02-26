using FirebirdSql.Data.FirebirdClient;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SGA.Interfaces;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;

namespace SGA.Lib
{
    public class DatabaseConnection
    {
        private readonly IUnitOfWork _iuw;
        public DatabaseConnection(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }


        public List<ApplicationSQLResult> GetDatabaseValues(ApplicationSQL applicationSQL) {

            if (applicationSQL == null)
            {
                throw new ArgumentException("O Parâmetro não pode ser nulo.", nameof(applicationSQL));
            }

            GetDatabaseConnectionData(applicationSQL, out string connectionString, out dynamic connection, out dynamic cmd);
            return GetDatabaseReader(applicationSQL, connection, cmd, connectionString);

        }


        private List<ApplicationSQLResult> GetDatabaseReader(ApplicationSQL applicationSQL, dynamic connection, dynamic cmd, string connectionString)
        {
            List<ApplicationSQLResult> readerList = new List<ApplicationSQLResult>();

            dynamic reader = null;
            try
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = applicationSQL.SQL;
                cmd.CommandType = System.Data.CommandType.Text;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ApplicationSQLResult reportLine = new ApplicationSQLResult();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        reportLine.AddColumns(reader[i].ToString());
                    }
                    readerList.Add(reportLine);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
                connection.Dispose();

            }
            return readerList;
         }

 
        private static void GetDatabaseConnectionData(ApplicationSQL applicationSQL, out string connectionString, out dynamic connection, out dynamic cmd)
        {
            DatabaseSGA database = applicationSQL.DatabaseSGA;

            connectionString = "";
            connection = null;
            cmd = null;
            string databaseUser = Cipher.Decrypt(database.DatabaseUser, database.ChangeDate.ToString());
            string databasePassword = Cipher.Decrypt(database.DatabasePassword, database.ChangeDate.ToString());

            switch (database.DatabaseTypeId)
            {

                //MySQL/MariaDB
                case 1:
                    connection = new MySqlConnection();
                    cmd = new MySqlCommand();
                    connectionString = $"server={database.DatabaseServer};uid={databaseUser};Pwd={databasePassword};Database={database.DatabaseName};Port={database.Port}";
                    break;

                //Oracle
                case 2:
                    connection = new OracleConnection();
                    cmd = new OracleCommand();
                    connectionString = $"User Id={databaseUser};Password={databasePassword};Data Source={database.DatabaseServer}:{database.Port}/{database.DatabaseName}";
                    break;

                //PostgreSQL
                case 3:
                    connection = new NpgsqlConnection();
                    cmd = new NpgsqlCommand();
                    //connectionString = $"Host={database.DatabaseServer};Username={databaseUser};Password={databasePassword};Database={database.DatabaseName};Port={database.Port}";
                    connectionString = $"Server={database.DatabaseServer};User Id={databaseUser};Password={databasePassword};Database={database.DatabaseName};Port={database.Port};CommandTimeout=20;";
                    break;

                //SQL Server
                case 4:
                    connection = new SqlConnection();
                    cmd = new SqlCommand();
                    connectionString = $"Server={database.DatabaseServer},{database.Port};User Id={databaseUser};Password={databasePassword};Initial Catalog={database.DatabaseName}";
                    break;

                //Firebird
                case 5:
                    connection = new FbConnection();
                    cmd = new FbCommand();
                    connectionString = $"Charset=\"WIN1252\";User={databaseUser};Password={databasePassword};Database={database.DatabaseName};DataSource={database.DatabaseServer};Port={database.Port}";
                    break;

                //ODBC
                case 6:
                    connection = new OdbcConnection();
                    cmd = new OdbcCommand();
                    connectionString = database.ConnectionString;
                    connectionString = connectionString.Replace("Nome do servidor", database.DatabaseServer);
                    connectionString = connectionString.Replace("Porta", database.Port.ToString());
                    connectionString = connectionString.Replace("Nome do banco", database.DatabaseName);
                    connectionString = connectionString.Replace("Nome do usuário", databaseUser);
                    connectionString = connectionString.Replace("Senha", databasePassword);

                    break;
            }
        }



        public int SetDatabaseValues(ApplicationSQL applicationSQL) {
            GetDatabaseConnectionData(applicationSQL, out string connectionString, out dynamic connection, out dynamic cmd);
            return DatabaseWrite(applicationSQL, connection, cmd, connectionString);
        }

        private int? DatabaseWrite(ApplicationSQL applicationSQL, dynamic connection, dynamic cmd, string connectionString)
        {
            int? response = null;

            try
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                cmd.Connection = connection;
                cmd.CommandText = applicationSQL.SQL.Replace("\r"," ").Replace("\n"," ");
                cmd.CommandType = System.Data.CommandType.Text;
                response = cmd.ExecuteNonQuery();
            }

            catch
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
                connection.Dispose();
            }

            return response;
        }


    }
}
