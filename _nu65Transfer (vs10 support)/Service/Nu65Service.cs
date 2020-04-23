using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using TransferNu65.Db;

namespace TransferNu65.Service
{
    public class Nu65Service
    {
        public static void GetFileNu65()
        {
            var dbFolder = Properties.Settings.Default.FoxproDbFolder_Fox60_Arm_Base;
            const string query = "SELECT kodizd, " +
                                 "kedn, " +
                                 "kodvmat, " +
                                 "nrvm, " +
                                 "kc, " +
                                 "pr_mat, " +
                                 "ku, " +
                                 "ednorm, " +
                                 "data_prov, " +
                                 "num_izv " +
                                 "FROM [nu65a]";

            var stringBuilder = new StringBuilder();

            using (var connection = DbControl.GetConnection(dbFolder))
            {
                connection.TryConnectOpen();
                using (var oleDbCommand = new OleDbCommand(query, connection))
                {
                    using (var reader = oleDbCommand.ExecuteReader())
                    {
                        while (reader != null && reader.Read())
                        {
                            var productId = Convert.ToDecimal(reader.GetString(0).Trim());
                            var measureId = Convert.ToDecimal(reader.GetString(1).Trim());
                            var materialId = Convert.ToDecimal(reader.GetString(2).Trim());
                            var nrvm = reader.GetString(3).Trim();
                            var workGuildId = reader.GetString(4).Trim();
                            var prMat = reader.GetString(5).Trim();
                            var ku = reader.GetString(6).Trim();
                            var ednorm = reader.GetString(7).Trim();
                            var date = DBNull.Value == reader.GetValue(8) ? new DateTime(1899,12,30) : reader.GetDateTime(8);
                            var flowRate = reader.GetDecimal(9) == 0 ? string.Empty : reader.GetDecimal(9).ToString(CultureInfo.InvariantCulture);

                            stringBuilder.AppendFormat("INSERT INTO Nu65Table (productsId, materialsId, measuresId, " +
                                                       "auxiliaryMaterialConsumptionRate, workGuildId, signMaterial, " +
                                                       "parcelId, date, flowRate, unitValidation) " +
                                                       "SELECT f1.Id, f2.Id, f3.Id, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}' " +
                                                       "FROM (SELECT Id FROM Products WHERE codeProduct = {0}) AS f1 CROSS JOIN " +
                                                       "(SELECT Id FROM Materials WHERE codeMaterial = {1}) AS f2 CROSS JOIN " +
                                                       "(SELECT Id FROM Measures WHERE oldDbCode = {2}) AS f3", 
                                productId, materialId, measureId, nrvm, workGuildId, prMat, ku, date.ToString("yyyyMMdd"), flowRate, ednorm);
                            stringBuilder.AppendLine();
                            stringBuilder.AppendFormat("GO");
                            stringBuilder.AppendLine();
                        }
                    }
                }
            }
   
            // запись в файл
            using (var fstream = new FileStream(@"w:\Nu65\_projecNu65Transfer (vs10 support)\InsertNu65.txt", FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                var array = System.Text.Encoding.Default.GetBytes(stringBuilder.ToString());
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
                Console.WriteLine("Текст Nu65 записан в файл");
            }
        }

        /// <summary>
        /// Добавление в таблицу с записью в файл не добавившихся
        /// </summary>
        public static void InsertNu65()
        {
            var dbFolder = Properties.Settings.Default.FoxproDbFolder_Fox60_Arm_Base;
            var server = Properties.Settings.Default.SqlServerNu65;
            var db = Properties.Settings.Default.SqlDbNu65Db;

            const string queryDbf = "SELECT kodizd, " +
                                 "kedn, " +
                                 "kodvmat, " +
                                 "nrvm, " +
                                 "kc, " +
                                 "pr_mat, " +
                                 "ku, " +
                                 "ednorm, " +
                                 "data_prov, " +
                                 "num_izv " +
                                 "FROM [nu65a]";

            using (var connectionDbf = DbControl.GetConnection(dbFolder))
            {
                connectionDbf.TryConnectOpen();
                using (var oleDbCommand = new OleDbCommand(queryDbf, connectionDbf))
                {
                    using (var reader = oleDbCommand.ExecuteReader())
                    {
                        var stringBuilder = new StringBuilder();
                        using (var connection = DbControl.GetConnection(server, db))
                        {
                            connection.TryConnectOpen();
                            while (reader != null && reader.Read())
                            {
                                var productId = Convert.ToDecimal(reader.GetString(0).Trim());
                                var measureId = Convert.ToDecimal(reader.GetString(1).Trim());
                                var materialId = Convert.ToDecimal(reader.GetString(2).Trim());
                                var nrvm = reader.GetString(3).Trim();
                                var workGuildId = reader.GetString(4).Trim();
                                var prMat = reader.GetString(5).Trim();
                                var ku = reader.GetString(6).Trim();
                                var ednorm = reader.GetString(7).Trim();
                                var date = DBNull.Value == reader.GetValue(8)
                                    ? new DateTime(1899, 12, 30)
                                    : reader.GetDateTime(8);
                                var flowRate = reader.GetDecimal(9) == 0 ? string.Empty : reader.GetDecimal(9).ToString(CultureInfo.InvariantCulture);

                                var stringQuery = string.Format(
                                    "INSERT INTO Nu65Table (productsId, materialsId, measuresId, " +
                                    "auxiliaryMaterialConsumptionRate, workGuildId, signMaterial, " +
                                    "parcelId, date, flowRate, unitValidation) " +
                                    "SELECT f1.Id, f2.Id, f3.Id, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}' " +
                                    "FROM (SELECT Id FROM Products WHERE codeProduct = {0}) AS f1 CROSS JOIN " +
                                    "(SELECT Id FROM Materials WHERE codeMaterial = {1}) AS f2 CROSS JOIN " +
                                    "(SELECT Id FROM Measures WHERE oldDbCode = {2}) AS f3",
                                    productId, materialId, measureId, nrvm, workGuildId, prMat, ku,
                                    date.ToString("yyyyMMdd"), flowRate, ednorm);


                                using (var sqlCommand = new SqlCommand(stringQuery, connection))
                                {
                                    var id = (long) sqlCommand.ExecuteNonQuery();

                                    if (id == 0)
                                    {
                                        stringBuilder.AppendFormat(stringQuery);
                                        stringBuilder.AppendLine();
                                        stringBuilder.AppendFormat("GO");
                                        stringBuilder.AppendLine();
                                    }
                                }
                            }
                        }

                        // запись в файл
                        using (FileStream fstream = new FileStream(@"w:\Nu65\_projecNu65Transfer (vs10 support)\ErorrInsert.txt", FileMode.OpenOrCreate))
                        {
                            // преобразуем строку в байты
                            byte[] array = Encoding.Default.GetBytes(stringBuilder.ToString());
                            // запись массива байтов в файл
                            fstream.Write(array, 0, array.Length);
                            Console.WriteLine("Текст Error записан в файл");
                        }
                    }
                }
            }

        }
    }
}
