using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;

namespace TransferNu65inDbf
{
    class Program
    {
        private const string server = "SQL2017\\SQL2017_1";
        private const string db = "Nu65DB";
        private const string dbFolder = "d:\\_TeacherProjects\\";

        /// <summary>
        /// Получение коллекции [Nu65]
        /// </summary>   
        public static List<Nu65> GetAll()
        {
            const string query = "SELECT [codeProduct], [Measures].oldDbCode, [codeMaterial], " +
                                 "[auxiliaryMaterialConsumptionRate], [workGuildId], [signMaterial], " +
                                 "[parcelId], [unitValidation], [date], [flowRate] " +
                                 "FROM [Nu65DB].[dbo].[Nu65Table] left join Materials on Materials.id = [materialsId] " +
                                 "left join Products on productsId = Products.id  " +
                                 "left join Measures on measuresId = Measures.id";

            var nu65 = new List<Nu65>();
            using (var connection = GetConnection(server, db))
            {
                connection.Open();

                using (var sqlCommand = new SqlCommand(query, connection))
                {
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var codeProduct = reader.GetInt64(0);
                            var oldmeasureid = reader.GetInt32(1);
                            var codeMaterial = reader.GetInt64(2);
                            var auxiliaryMaterialConsumptionRate = reader.GetString(3);
                            var workGuildId = reader.GetString(4);
                            var signMaterial = reader.GetString(5);
                            var parcelId = reader.GetString(6);
                            var unitValidation = reader.GetString(7);
                            var date = reader.GetDateTime(8);
                            var flowRate = reader.GetString(9);

                            nu65.Add(new Nu65(codeProduct, oldmeasureid, codeMaterial, auxiliaryMaterialConsumptionRate,
                                workGuildId, signMaterial, parcelId, unitValidation, date, flowRate));
                        }
                    }
                }
            }

            return nu65;
        }

        /// <summary>
        /// Получение коллекции [Nu65] после даты
        /// </summary>   
        public static List<Nu65> GetSqlByDate(DateTime selectDate)
        {
            const string query = "SELECT " +
                                 "[codeProduct], " +
                                 "[Measures].oldDbCode, " +
                                 "[codeMaterial], " +
                                 "[auxiliaryMaterialConsumptionRate], " +
                                 "[workGuildId], " +
                                 "[signMaterial], " +
                                 "[parcelId], " +
                                 "[unitValidation], " +
                                 "[date], " +
                                 "[flowRate] " +
                                 "FROM [Nu65DB].[dbo].[Nu65Table] " +
                                 "left join Materials on Materials.id = [materialsId] " +
                                 "left join Products on productsId = Products.id where date > @date";

            var nu65 = new List<Nu65>();
            using (var connection = GetConnection(server, db))
            {
                connection.Open();

                using (var sqlCommand = new SqlCommand(query, connection))
                {
                    sqlCommand.Parameters.AddWithValue("@date", selectDate);
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var codeProduct = reader.GetInt64(0);
                            var oldmeasureid = reader.GetInt32(1);
                            var codeMaterial = reader.GetInt64(2);
                            var auxiliaryMaterialConsumptionRate = reader.GetString(3);
                            var workGuildId = reader.GetString(4);
                            var signMaterial = reader.GetString(5);
                            var parcelId = reader.GetString(6);
                            var unitValidation = reader.GetString(7);
                            var date = reader.GetDateTime(8);
                            var flowRate = reader.GetString(9);

                            nu65.Add(new Nu65(codeProduct, oldmeasureid, codeMaterial, auxiliaryMaterialConsumptionRate,
                                workGuildId, signMaterial, parcelId, unitValidation, date, flowRate));
                        }
                    }
                }
            }

            return nu65;
        }

        public static void InsertDbf(OleDbConnection oleDbConnection,
            string kodizd, string kedn, string kodvmat,
            string nrvm, string kc, string prmat,
            string ku, string ednorm, DateTime dataprov, decimal numizv)
        {
            string query = "INSERT INTO nu65a (kodizd, kedn, kodvmat, nrvm, kc, pr_mat, ku, " +
                           "ednorm, maket, data_prov, num_izv, check) " +
                           "VALUES (?, ?, ?, ?, ?, ?, ?, " +
                           "?, '650,1', ctod( \'{0}\' ), ?, '')";

            using (var oleDbCommand = new OleDbCommand(string.Format(query, dataprov.ToString("MM/dd/yyyy")), oleDbConnection))
            {
                oleDbCommand.Parameters.AddWithValue("kodizd", kodizd);
                oleDbCommand.Parameters.AddWithValue("kedn", kedn);
                oleDbCommand.Parameters.AddWithValue("kodvmat", kodvmat);
                oleDbCommand.Parameters.AddWithValue("nrvm", nrvm);
                oleDbCommand.Parameters.AddWithValue("kc", kc);
                oleDbCommand.Parameters.AddWithValue("pr_mat", prmat);
                oleDbCommand.Parameters.AddWithValue("ku", ku);
                oleDbCommand.Parameters.AddWithValue("ednorm", ednorm);
                oleDbCommand.Parameters.AddWithValue("num_izv", numizv);

                oleDbCommand.ExecuteNonQuery();
            }
        }

        public class Nu65
        {
            public long CodeProduct { get; }
            public int OldMeasureId { get; }
            public long CodeMaterial { get; }
            public string AuxiliaryMaterialConsumptionRate { get; }
            public string WorkGuildId { get; }
            public string SignMaterial { get; }
            public string ParcelId { get; }
            public string UnitValidation { get; }
            public DateTime Date { get; }
            public string FlowRate { get; }


            public Nu65(long codeProduct, int oldMeasureId, long codeMaterial, string auxiliaryMaterialConsumptionRate,
                string workGuildId, string signMaterial, string parcelId,
                string unitValidation, DateTime date, string flowRate)
            {
                CodeProduct = codeProduct;
                OldMeasureId = oldMeasureId;
                CodeMaterial = codeMaterial;
                AuxiliaryMaterialConsumptionRate = auxiliaryMaterialConsumptionRate;
                WorkGuildId = workGuildId;
                SignMaterial = signMaterial;
                ParcelId = parcelId;
                UnitValidation = unitValidation;
                Date = date;
                FlowRate = flowRate;
            }
        }

        static void Main(string[] args)
        {
            //var nu65 = GetSqlByDate(new DateTime(2019, 9, 1));
            var nu65 = GetAll();
            using (var oleDbConnection = GetConnection(dbFolder))
            {
                oleDbConnection.Open();
                foreach (var item in nu65)
                {
                    InsertDbf(oleDbConnection,
                        ParseLongInStringWhithAddZero(item.CodeProduct, 14),
                        ParseIntInStringWhithAddZero(item.OldMeasureId, 3),
                        ParseLongInStringWhithAddZero(item.CodeMaterial, 10),
                        item.AuxiliaryMaterialConsumptionRate,
                        item.WorkGuildId, item.SignMaterial, item.ParcelId, item.UnitValidation,
                        item.Date, item.FlowRate == string.Empty ? 0 : Convert.ToInt32(item.FlowRate));
                }
            }
        }

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
        /// Получение строки соединения с сервером mssql
        /// </summary>
        public static string SqlServerConnectRow(string server, string database)
        {
            var connectionTimeoutInSecond = 20;
            const string connectionPattern = "Data Source={0};Initial Catalog={1};" +
                                             "Persist Security Info=True;Integrated Security=SSPI";
            return string.Format(connectionPattern, server, database);
        }

        /// <summary>
        /// Получение строки соединения с каталогом базы данных foxpro
        /// </summary>
        public static string FoxProConnectRow(string path)
        {
            const string connectionPattern = "Provider={0};Data Source={1};";
            const string provider = "VFPOLEDB.1";
            return string.Format(connectionPattern, provider, path);
        }

        /// <summary>
        /// Перевод Int значения в String с добавлением нулей в начало, пока length != size
        /// </summary>
        /// <param name="value"> Значение </param>
        /// <param name="size"> Размер строки </param>
        /// <returns></returns>
        public static string ParseIntInStringWhithAddZero(int value, int size)
        {
            var valueString = value.ToString(CultureInfo.InvariantCulture);
            while (valueString.Length < size)
            {
                valueString = "0" + valueString;
            }

            return valueString;
        }

        /// <summary>
        /// Перевод Long значения в String с добавлением нулей в начало, пока length != size
        /// </summary>
        /// <param name="value"> значение </param>
        /// <param name="size"> размер строки </param>
        /// <returns></returns>
        public static string ParseLongInStringWhithAddZero(long value, int size)
        {
            var valueString = value.ToString(CultureInfo.InvariantCulture);
            while (valueString.Length < size)
            {
                valueString = "0" + valueString;
            }

            return valueString;
        }
    }
}
