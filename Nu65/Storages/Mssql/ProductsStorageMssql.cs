using System.Collections.Generic;
using System.Data.SqlClient;

using NU65.Db;
using NU65.Entities.External;

namespace NU65.Storages.Mssql
{
    /// <summary>
    /// Обработчик запросов хранилища данных для таблицы изделий [Product]
    /// </summary>
    public class ProductsStorageMssql
    {
        /// <summary>
        /// Получение коллекции [Продуктов]
        /// </summary>   
        public static List<Product> GetAll()
        {
            const string query = "SELECT [id], [codeProduct], [name], [mark] FROM [Products] ";
            var server = Properties.Settings.Default.SqlServerNu65;
            var db = Properties.Settings.Default.SqlDbNu65Db;

            var products = new List<Product>();
            try
            {
                using (var connection = DbControl.GetConnection(server, db))
                {
                    connection.TryConnectOpen();
                    using (var sqlCommand = new SqlCommand(query, connection))
                    {
                        using (var reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new Product()
                                {
                                    Id = reader.GetInt64(0),
                                    CodeProduct = reader.GetInt64(1),
                                    Name = reader.GetString(2),
                                    Mark = reader.GetString(3)
                                };
                                products.Add(product);
                            }
                        }
                    }
                }
                return products;
            }
            catch (SqlException ex)
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }

        /// <summary>
        /// Проверка есть ли в MsSql продукт с заданым кодом
        /// </summary>
        public static bool IsDublicate(long selectProductCodeProduct, SqlConnection connection)
        {
            const string query = "SELECT * FROM [Products] WHERE [codeProduct] = @codeProduct";

            using (var sqlCommand = new SqlCommand(query, connection))
            {
                sqlCommand.Parameters.AddWithValue("@codeProduct", selectProductCodeProduct);
                using (var reader = sqlCommand.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }

        /// <summary>
        /// Добавление продукта в Mssql с возвратом Id
        /// </summary>
        public static long Insert(long codeProduct, string name, string mark, SqlConnection connection)
        {
            const string query = "INSERT INTO [Products] ([codeProduct], [name], [mark]) " +
                                 "OUTPUT INSERTED.ID " +
                                 "VALUES (@codeProduct, @name, @mark)";
            using (var sqlCommand = new SqlCommand(query, connection))
            {
                sqlCommand.Parameters.AddWithValue("@codeProduct", codeProduct);
                sqlCommand.Parameters.AddWithValue("@name", name);
                sqlCommand.Parameters.AddWithValue("@mark", mark);
                var id = (long) sqlCommand.ExecuteScalar();
                return id;
            }
        }

        public static List<long> GetProductsIdWhereMaterial(Material material)
        {
            const string query = "SELECT [productsId] " +
                                 "FROM [Nu65DB].[dbo].[Nu65Table] " +
                                 "WHERE [materialsId] = @materialsId " +
                                 "AND [auxiliaryMaterialConsumptionRate] = @auxiliaryMaterialConsumptionRate " +
                                 "AND [workGuildId] = @workGuildId " +
                                 "AND [signMaterial] = @signMaterial " +
                                 "AND [parcelId] = @parcelId " +
                                 "AND [unitValidation] = @unitValidation";
            var server = Properties.Settings.Default.SqlServerNu65;
            var db = Properties.Settings.Default.SqlDbNu65Db;

            var productsId = new List<long>();
            try
            {
                using (var connection = DbControl.GetConnection(server, db))
                {
                    connection.TryConnectOpen();
                    using (var sqlCommand = new SqlCommand(query, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@materialsId", material.Id);
                        sqlCommand.Parameters.AddWithValue("@auxiliaryMaterialConsumptionRate", material.AuxiliaryMaterialConsumptionRate);
                        sqlCommand.Parameters.AddWithValue("@workGuildId", material.WorkGuildId);
                        sqlCommand.Parameters.AddWithValue("@signMaterial", material.SignMaterial);
                        sqlCommand.Parameters.AddWithValue("@parcelId", material.ParcelId);
                        sqlCommand.Parameters.AddWithValue("@unitValidation", material.UnitValidation);

                        using (var reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productsId.Add(reader.GetInt64(0));
                            }
                        }
                    }
                }
                return productsId;
            }
            catch (SqlException ex)
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }
    }
}
