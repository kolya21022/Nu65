using System.Data.OleDb;
using System.Data.SqlClient;

namespace TransferNu65.Db
{
    /// <summary>
    /// Класс управление соединением с БД (MS SQL сервер и каталог базы данных FoxPro)
    /// </summary>
    public static class DbControl
    {
        /// <summary>
        /// Получение соединения с базой данных на MSSQL-сервере
        /// </summary>
        public static SqlConnection GetConnection(string server, string database)
        {
            var connectRow = SqlServerConnectRow(server, database);
            return new SqlConnection(connectRow);
        }

        /// <summary>
        /// Получение соединения с каталогом базы данных FoxPro
        /// </summary>
        public static OleDbConnection GetConnection(string path)
        {
            var connectRow = FoxProConnectRow(path);
            return new OleDbConnection(connectRow);
        }

        /// <summary>
        /// Попытка открытия соединения с базой данных MSSQL
        /// </summary>
        public static void TryConnectOpen(this SqlConnection connect)
        {
            connect.Open();
        }

        /// <summary>
        /// Попытка открытия соединения с каталогом базы данных FoxPro
        /// </summary>
        public static void TryConnectOpen(this OleDbConnection connect)
        {
            connect.Open();
        }


        /// <summary>
        /// Получение строки соединения с сервером mssql
        /// </summary>
        private static string SqlServerConnectRow(string server, string database)
        {
            var connectionTimeoutInSecond = Properties.Settings.Default.TimeoutSqlServerConnectInSecond;
            const string connectionPattern = "Data Source={0};Initial Catalog={1};Connection Timeout={2};" +
                                             "Persist Security Info=True;Integrated Security=SSPI";
            return string.Format(connectionPattern, server, database, connectionTimeoutInSecond);
        }

        /// <summary>
        /// Получение строки соединения с каталогом базы данных foxpro
        /// </summary>
        private static string FoxProConnectRow(string path)
        {
            const string connectionPattern = "Provider={0};Data Source={1};";
            const string provider = "VFPOLEDB.1";
            return string.Format(connectionPattern, provider, path);
        }

    }
}
