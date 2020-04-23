using System;
using System.Data.OleDb;

namespace NU65.Storages.Foxpro
{
    /// <summary>
    /// Обработчик запросов хранилища данных для таблицы [Nu65]
    /// </summary>
    public class Nu65StorageFoxpro
    {
        /// <summary>
        /// Тестирование соединения с таблицей базы данных FoxPro
        /// </summary>
        public static void TestConnection(OleDbConnection oleDbConnection)
        {
            const string testQuery = "SELECT * FROM [nu65a]";
            using (var oleDbCommand = new OleDbCommand(testQuery, oleDbConnection))
            {
                using (oleDbCommand.ExecuteReader()) { }
            }
        }

        /// <summary>
        /// Добавление записи в [Nu65a]
        /// </summary>
        public static void Insert(OleDbConnection oleDbConnection, string codeProduct, string codeMaterial,
            string oldDbCodeMeasure, string auxiliaryMaterialConsumptionRate, string workGuildId,
            string signMaterial, string parcelId, string unitValidation, DateTime date, int flowRate)
        {
            const string query = "INSERT INTO [nu65a] (kodizd, kedn, kodvmat, nrvm, kc, pr_mat, ku, ednorm, maket, " +
                                 "data_prov, num_izv, check) VALUES (?, ?, ?, ?, ?, ?, ?, ?, '650,1', ?, ?, '')";

            using (var oleDbCommand = new OleDbCommand(query, oleDbConnection))
            {
                oleDbCommand.Parameters.AddWithValue("@kodizd", codeProduct);
                oleDbCommand.Parameters.AddWithValue("@kedn", oldDbCodeMeasure);
                oleDbCommand.Parameters.AddWithValue("@kodvmat", codeMaterial);
                oleDbCommand.Parameters.AddWithValue("@nrvm", auxiliaryMaterialConsumptionRate);
                oleDbCommand.Parameters.AddWithValue("@kc", workGuildId);
                oleDbCommand.Parameters.AddWithValue("@pr_mat", signMaterial);
                oleDbCommand.Parameters.AddWithValue("@ku", parcelId);
                oleDbCommand.Parameters.AddWithValue("@ednorm", unitValidation);
                oleDbCommand.Parameters.AddWithValue("@data_prov", date);
                oleDbCommand.Parameters.AddWithValue("@num_izv", flowRate);
                oleDbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
		/// Обновление [Поставщика/Потребителя]
		/// </summary>
		public static void Update(OleDbConnection oleDbConnection, string codeProduct, string codeMaterial,
            string oldDbCodeMeasure, string auxiliaryMaterialConsumptionRate, string workGuildId,
            string signMaterial, string parcelId, string unitValidation, int flowRate, string oldWorkGuildId, 
            string oldParcelId)
        {
            const string query = "UPDATE [nu65a] SET kedn = ?, nrvm = ?, kc = ?, pr_mat = ?, ku = ?, ednorm = ?, " +
                                 "num_izv = ? " +
                                 "WHERE kodizd = ? and kodvmat = ? and kc = ? and ku = ?";

            using (var oleDbCommand = new OleDbCommand(query, oleDbConnection))
            {
                oleDbCommand.Parameters.AddWithValue("@kedn", oldDbCodeMeasure);
                oleDbCommand.Parameters.AddWithValue("@nrvm", auxiliaryMaterialConsumptionRate);
                oleDbCommand.Parameters.AddWithValue("@kc", workGuildId);
                oleDbCommand.Parameters.AddWithValue("@pr_mat", signMaterial);
                oleDbCommand.Parameters.AddWithValue("@ku", parcelId);
                oleDbCommand.Parameters.AddWithValue("@ednorm", unitValidation);
                oleDbCommand.Parameters.AddWithValue("@num_izv", flowRate);

                oleDbCommand.Parameters.AddWithValue("@kodizd", codeProduct);
                oleDbCommand.Parameters.AddWithValue("@kodvmat", codeMaterial);
                oleDbCommand.Parameters.AddWithValue("@kc", oldWorkGuildId);
                oleDbCommand.Parameters.AddWithValue("@ku", oldParcelId);
                oleDbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Удаление [Материала для продукта] с указанным кодом
        /// </summary>
        public static void DeleteByMaterialCode(OleDbConnection oleDbConnection, string codeProduct, long codeMaterial, string workGuildId,
            string parcelId)
        {
            const string query = "DELETE FROM [nu65a] WHERE kodizd = ? and kodvmat = ? and kc = ? and ku = ? ";

            using (var oleDbCommand = new OleDbCommand(query, oleDbConnection))
            {
                oleDbCommand.Parameters.AddWithValue("@kodizd", codeProduct);
                oleDbCommand.Parameters.AddWithValue("@kodvmat", codeMaterial.ToString().Trim());
                oleDbCommand.Parameters.AddWithValue("@kc", workGuildId);
                oleDbCommand.Parameters.AddWithValue("@ku", parcelId);
                oleDbCommand.ExecuteNonQuery();
            }
        }
    }
}
