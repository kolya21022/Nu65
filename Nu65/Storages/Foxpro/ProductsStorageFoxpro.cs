using System.Collections.Generic;
using System.Data.OleDb;

using NU65.Db;
using NU65.Entities.External;

namespace NU65.Storages.Foxpro
{
    /// <summary>
    /// Обработчик запросов хранилища данных для таблицы изделий [Product]
    /// </summary>
    public class ProductsStorageFoxPro
    {
		/// <summary>
		/// Получение коллекции [Продуктов] из prdsetmc
		/// </summary>
		public static List<Product> GetAll()
        {
            var dbFolder = Properties.Settings.Default.FoxproDbFolder_Fox60_Arm_Base;
            const string query = "SELECT kod_mater AS codeProduct, " +
                                 "naim AS name, " +
                                 "marka as mark " +
                                 "FROM [prdsetmc] WHERE kod_mater > 9999999999";

            var products = new List<Product>();
            try
            {
                using (var connection = DbControl.GetConnection(dbFolder))
                {
                    connection.TryConnectOpen();
                    // Проверки наличия установленных кодировок в DBF-файлах и проверки соединений с этими файлами
                    connection.VerifyInstalledEncoding("prdsetmc");

                    using (var oleDbCommand = new OleDbCommand(query, connection))
                    {
                        using (var reader = oleDbCommand.ExecuteReader())
                        {
                            while (reader != null && reader.Read())
                            {
                                var codeProduct = reader.GetDecimal(0);
                                var name = reader.GetString(1).Trim();
                                var mark = reader.GetValue(2) == null ? string.Empty : reader.GetString(2).Trim();

                                var product = new Product
                                {
                                    CodeProduct = (long)codeProduct,
                                    Name = name,
                                    Mark = mark 
                                };
                                products.Add(product);
                            }
                        }
                    }
                }
                return products;
            }
            catch (OleDbException ex)
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }
    }
}

