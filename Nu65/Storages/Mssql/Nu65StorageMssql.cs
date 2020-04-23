using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using NU65.Db;
using NU65.Entities.External;
using NU65.Entities.Interal;

namespace NU65.Storages.Mssql
{
    /// <summary>
    /// Обработчик запросов хранилища данных для таблицы [Nu65] в Mssql
    /// </summary>
    public class Nu65StorageMssql
    {
        /// <summary>
        /// Получение коллекции [Nu65]
        /// </summary>   
        public static List<Nu65> GetAll()
        {
            const string query = "SELECT " +
                                 "[Products].[id], " +
                                 "[Products].[name], " +
                                 "[Products].[mark], " +
                                 "[Products].[codeProduct], " +
                                 "[j1].[idNu65], " +
                                 "[j1].[materialsId], " +
                                 "[j1].[nameMaterials], " +
                                 "[j1].[profile], " +
                                 "[j1].[gost], " +
                                 "[j1].[codeMaterial], " +
                                 "[j1].[auxiliaryMaterialConsumptionRate], " +
                                 "[j1].[workGuildId], " +
                                 "[j1].[signMaterial], " +
                                 "[j1].[parcelId], " +
                                 "[j1].[date], " +
                                 "[j1].[flowRate], " +
                                 "[j1].[measuresId], " +
                                 "[j1].[unitValidation] " +
                                 "FROM[Nu65DB].[dbo].[Products] " +
                                 "LEFT JOIN( " +
                                 "SELECT[Nu65Table].[id] as [idNu65], " +
                                 "[Nu65Table].[productsId] as [productsId], " +
                                 "[materialsId] as [materialsId], " +
                                 "[Materials].[name] as [nameMaterials], " +
                                 "[Materials].[profile] as [profile], " +
                                 "[Materials].[gost] as [gost], " +
                                 "[Materials].[codeMaterial] as [codeMaterial], " +
                                 "[Nu65Table].[auxiliaryMaterialConsumptionRate] as [auxiliaryMaterialConsumptionRate], " +
                                 "[Nu65Table].[workGuildId] as [workGuildId], " +
                                 "[Nu65Table].[signMaterial] as [signMaterial], " +
                                 "[Nu65Table].[parcelId] as [parcelId], " +
                                 "[Nu65Table].[date] as [date], " +
                                 "[Nu65Table].[flowRate] as [flowRate], " +
                                 "[Nu65Table].[unitValidation] as [unitValidation], " +
                                 "[Nu65Table].[flowRate] as [flowRate] " +
                                 "FROM [Nu65DB].[dbo].[Nu65Table], " +
                                 "[Nu65DB].[dbo].[Materials] " +
                                 "WHERE [Materials].[id] = [materialsId]) as j1 " +
                                 "ON j1.[productsId] = [Products].[id]";
            var server = Properties.Settings.Default.SqlServerNu65;
            var db = Properties.Settings.Default.SqlDbNu65Db;

            var nu65 = new List<Nu65>();
            try
            {
                using (var connection = DbControl.GetConnection(server, db))
                {
                    connection.TryConnectOpen();
                    using (var sqlCommand = new SqlCommand(query, connection))
                    {
                        sqlCommand.CommandTimeout = 300;
                        using (var reader = sqlCommand.ExecuteReader())
                        {

                            var groups = from DbDataRecord record in reader
                                group record by
                                    new
                                    {
                                        r1 = record.GetInt64(0),
                                        r2 = record.GetString(1),
                                        r3 = record.GetString(2),
                                        r4 = record.GetInt64(3)
                                    };

                            foreach (var group in groups)
                            {
                                var materials = new List<Material>();
                                var product = new Product()
                                {
                                    Id = group.Key.r1,
                                    CodeProduct = group.Key.r4,
                                    Name = group.Key.r2,
                                    Mark = group.Key.r3
                                };
                                foreach (var item in group)
                                {
                                    if (item.GetValue(4) != DBNull.Value)
                                    {
                                        var id = item.GetInt64(4);
                                        var materialId = item.GetInt64(5);
                                        var nameMaterial = item.GetString(6);
                                        var profile = item.GetString(7);
                                        var gost = item.GetString(8);
                                        var codeMaterial = item.GetInt64(9);
                                        var auxiliaryMaterialConsumptionRate = item.GetString(10);
                                        var workGuildId = item.GetString(11);
                                        var signMaterial = item.GetString(12);
                                        var parcelId = item.GetString(13);
                                        var date = item.GetDateTime(14);
                                        var flowRate = item.GetString(15);

                                        var measuresId = item.GetInt64(16);

                                        var unitValidation = item.GetString(17);

                                        materials.Add(new Material()
                                        {
                                            Id = materialId,
                                            Nu65TableId = id,
                                            AuxiliaryMaterialConsumptionRate = auxiliaryMaterialConsumptionRate,
                                            WorkGuildId = workGuildId,
                                            SignMaterial = signMaterial,
                                            ParcelId = parcelId,
                                            Date = date,
                                            FlowRate = flowRate,
                                            UnitValidation = unitValidation,
                                            Measure = new Measure()
                                            {
                                                Id = measuresId,
                                            },
                                            CodeMaterial = codeMaterial,
                                            Name = nameMaterial,
                                            Profile = profile,
                                            Gost = gost,
                                            ServiceMappedMeasureId = measuresId
                                        });
                                    }
                                }

                                nu65.Add(new Nu65()
                                {
                                    Product = product,
                                    Materials = materials
                                });
                            }
                        }
                    }
                }

                return nu65;
            }
            catch (SqlException ex)
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }

        /// <summary>
        /// Получение коллекции [Nu65View] для просмотра в справочнике
        /// </summary>
        public static List<Nu65View> GetAllView()
        {
            const string query = "SELECT [Nu65Table].[id], " +
                                 "[productsId], " +
                                 "[materialsId], " +
                                 "[measuresId], " +
                                 "[auxiliaryMaterialConsumptionRate], " +
                                 "[workGuildId], " +
                                 "[signMaterial], " +
                                 "[parcelId], " +
                                 "[date], " +
                                 "[flowRate], " +
                                 "[Products].[name], " +
                                 "[Products].[mark], " +
                                 "[Materials].[name], " +
                                 "[Materials].[profile], " +
                                 "[Materials].[gost], " +
                                 "[Materials].[codeMaterial], " +
                                 "[Products].[codeProduct], " +
                                 "[unitValidation] " +
                                 "FROM[Nu65DB].[dbo].[Nu65Table], " +
                                 "[Nu65DB].[dbo].[Products], " +
                                 "[Nu65DB].[dbo].[Materials] " +
                                 "WHERE [Materials].[id] = [materialsId] " +
                                 "and [Products].[id] = [productsId] ";
            var server = Properties.Settings.Default.SqlServerNu65;
            var db = Properties.Settings.Default.SqlDbNu65Db;

            var nu65View = new List<Nu65View>();
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
                                var id = reader.GetInt64(0);
                                var productId = reader.GetInt64(1);
                                var materialId = reader.GetInt64(2);
                                var measuresId = reader.GetInt64(3);
                                var auxiliaryMaterialConsumptionRate = reader.GetString(4);
                                var workGuildId = reader.GetString(5);
                                var signMaterial = reader.GetString(6);
                                var parcelId = reader.GetString(7);
                                var date = reader.GetDateTime(8);
                                var flowRate = reader.GetString(9);

                                var nameProduct = reader.GetString(10);
                                var markProduct = reader.GetString(11);

                                var nameMaterial = reader.GetString(12);
                                var profile = reader.GetString(13);
                                var gost = reader.GetString(14);

                                var codeMaterial = reader.GetInt64(15);
                                var codeProduct = reader.GetInt64(16);

                                var unitValidation = reader.GetString(17);

                                var product = new Product()
                                {
                                    Id = productId,
                                    CodeProduct = codeProduct,
                                    Name = nameProduct,
                                    Mark = markProduct
                                };

                                var material = new Material()
                                {
                                    Id = materialId,
                                    Nu65TableId = id,
                                    AuxiliaryMaterialConsumptionRate = auxiliaryMaterialConsumptionRate,
                                    WorkGuildId = workGuildId,
                                    SignMaterial = signMaterial,
                                    ParcelId = parcelId,
                                    Date = date,
                                    FlowRate = flowRate,
                                    UnitValidation = unitValidation,
                                    CodeMaterial = codeMaterial,
                                    Name = nameMaterial,
                                    Profile = profile,
                                    Gost = gost,
                                    ServiceMappedMeasureId = measuresId,
                                    Measure = new Measure()
                                    {
                                        Id = measuresId,
                                    }
                                };

                                nu65View.Add(new Nu65View()
                                {
                                    Id = id,
                                    Product = product,
                                    Material = material
                                });
                            }
                        }
                    }
                }

                return nu65View;
            }
            catch (SqlException ex)
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }

        /// <summary>
        /// Получение коллекции [Материалов] для продукта
        /// </summary>
        public static List<Material> GetMaterialsByProductId(long productId)
        {
            const string query = "SELECT [Nu65Table].[id], " +
                                 "[Nu65Table].[materialsId], " +
                                 "[Nu65Table].[auxiliaryMaterialConsumptionRate], " +
                                 "[Nu65Table].[workGuildId], " +
                                 "[Nu65Table].[signMaterial], " +
                                 "[Nu65Table].[parcelId], " +
                                 "[Nu65Table].[date], " +
                                 "[Nu65Table].[flowRate], " +
                                 "[Nu65Table].[measuresId], " +

                                 "[Materials].[name], " +
                                 "[Materials].[profile], " +
                                 "[Materials].[gost], " +
                                 "[Materials].[codeMaterial], " +
                                 "[Nu65Table].[unitValidation] " +
                                 "FROM[Nu65DB].[dbo].[Nu65Table] " +
                                 "INNER JOIN [Nu65DB].[dbo].[Materials] on [materialsId] = [Materials].[id] " +
                                 "WHERE [productsId] = @productsId";
            var server = Properties.Settings.Default.SqlServerNu65;
            var db = Properties.Settings.Default.SqlDbNu65Db;

            var materials = new List<Material>();
            try
            {
                using (var connection = DbControl.GetConnection(server, db))
                {
                    connection.TryConnectOpen();

                    using (var sqlCommand = new SqlCommand(query, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@productsId", productId);
                        using (var reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var id = reader.GetInt64(0);
                                var materialId = reader.GetInt64(1);
                                var auxiliaryMaterialConsumptionRate = reader.GetString(2);
                                var workGuildId = reader.GetString(3);
                                var signMaterial = reader.GetString(4);
                                var parcelId = reader.GetString(5);
                                var date = reader.GetDateTime(6);
                                var flowRate = reader.GetString(7);
                                var measuresId = reader.GetInt64(8);

                                var materialName = reader.GetString(9);
                                var profile = reader.GetString(10);
                                var gost = reader.GetString(11);
                                var codeMaterial = reader.GetInt64(12);

                                var unitValidation = reader.GetString(13);

                                materials.Add(new Material()
                                {
                                    Id = materialId,
                                    Nu65TableId = id,
                                    AuxiliaryMaterialConsumptionRate = auxiliaryMaterialConsumptionRate,
                                    WorkGuildId = workGuildId,
                                    SignMaterial = signMaterial,
                                    ParcelId = parcelId,
                                    Date = date,
                                    FlowRate = flowRate,
                                    CodeMaterial = codeMaterial,
                                    Name = materialName,
                                    Profile = profile,
                                    Gost = gost,
                                    UnitValidation = unitValidation,
                                    ServiceMappedMeasureId = measuresId,
                                    Measure = new Measure()
                                    {
                                        Id = measuresId
                                    }
                                });
                            }
                        }
                    }
                }

                return materials;
            }
            catch (SqlException ex)
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }

        /// <summary>
        /// Добавление записи в [Nu65Table]
        /// </summary>
        public static long Insert(long productId, long materialId, long measureId, string auxiliaryMaterialConsumptionRate, 
            string workGuildId, string signMaterial, string parcelId,
            string unitValidation, DateTime date, string flowRate, SqlConnection connection,
            SqlTransaction transaction)
        {
            const string query = "INSERT INTO Nu65Table (productsId, materialsId, measuresId, " +
                                 "auxiliaryMaterialConsumptionRate, workGuildId, signMaterial, " +
                                 "parcelId, date, flowRate, unitValidation) OUTPUT INSERTED.ID " +
                                 "VALUES (@productsId, @materialsId, @measuresId, @auxiliaryMaterialConsumptionRate, " +
                                 "@workGuildId, @signMaterial, @parcelId, @date, @flowRate, @unitValidation)";

            using (var sqlCommand = new SqlCommand(query, connection, transaction))
            {
                sqlCommand.Parameters.AddWithValue("@productsId", productId);
                sqlCommand.Parameters.AddWithValue("@materialsId", materialId);
                sqlCommand.Parameters.AddWithValue("@measuresId", measureId);
                sqlCommand.Parameters.AddWithValue("@auxiliaryMaterialConsumptionRate", auxiliaryMaterialConsumptionRate);
                sqlCommand.Parameters.AddWithValue("@workGuildId", workGuildId);
                sqlCommand.Parameters.AddWithValue("@signMaterial", signMaterial);
                sqlCommand.Parameters.AddWithValue("@parcelId", parcelId);
                sqlCommand.Parameters.AddWithValue("@date", date);
                sqlCommand.Parameters.AddWithValue("@flowRate", flowRate);
                sqlCommand.Parameters.AddWithValue("@unitValidation", unitValidation);
                var id = (long) sqlCommand.ExecuteScalar();
                return id;
            }
        }

        /// <summary>
		/// Обновление [Nu65Table] с указанным [ID]
		/// </summary>
		public static void Update(long id, long measureId, string auxiliaryMaterialConsumptionRate,
            string workGuildId, string signMaterial, string parcelId,
            string unitValidation, string flowRate, SqlConnection mssqlConnection, SqlTransaction mssqlTransaction)
        {
            const string query = "UPDATE [Nu65Table] SET [measuresId] = @measuresId, " +
                                 "[auxiliaryMaterialConsumptionRate] = @auxiliaryMaterialConsumptionRate, " +
                                 "[workGuildId] = @workGuildId, [signMaterial] = @signMaterial, [parcelId] = @parcelId, " +
                                 "[flowRate] = @flowRate, [unitValidation] = @unitValidation " +
                                 "WHERE [id] = @id ";

            using (var mssqlCommand = new SqlCommand(query, mssqlConnection, mssqlTransaction))
            {
                mssqlCommand.Parameters.AddWithValue("@measuresId", measureId);
                mssqlCommand.Parameters.AddWithValue("@auxiliaryMaterialConsumptionRate", auxiliaryMaterialConsumptionRate);
                mssqlCommand.Parameters.AddWithValue("@workGuildId", workGuildId);
                mssqlCommand.Parameters.AddWithValue("@signMaterial", signMaterial);
                mssqlCommand.Parameters.AddWithValue("@parcelId", parcelId);
                mssqlCommand.Parameters.AddWithValue("@flowRate", flowRate);
                mssqlCommand.Parameters.AddWithValue("@unitValidation", unitValidation);
                mssqlCommand.Parameters.AddWithValue("@id", id);
                mssqlCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Удаление [Материалов для продукта] с указанным [ID]
        /// </summary>
        public static void DeleteById(long nu65Id, SqlConnection mssqlConnection, SqlTransaction mssqlTransaction)
        {
            const string query = "DELETE FROM [Nu65Table] WHERE [id] = @id";

            using (var mssqlCommand = new SqlCommand(query, mssqlConnection, mssqlTransaction))
            {
                mssqlCommand.Parameters.AddWithValue("@id", nu65Id);

                mssqlCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Проверка на дубликат
        /// </summary>
        public static bool IsDublicate(long productId, long materialId, string workGuildId, string parcelId)
        {
            const string query = "SELECT * FROM [Nu65DB].[dbo].[Nu65Table] " +
                                 "WHERE [productsId] = @productsId and [materialsId] = @materialsId and " +
                                 "[workGuildId] = @workGuildId and [parcelId] = @parcelId";
            var server = Properties.Settings.Default.SqlServerNu65;
            var db = Properties.Settings.Default.SqlDbNu65Db;

            try
            {
                using (var connection = DbControl.GetConnection(server, db))
                {
                    connection.TryConnectOpen();

                    using (var sqlCommand = new SqlCommand(query, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@productsId", productId);
                        sqlCommand.Parameters.AddWithValue("@materialsId", materialId);
                        sqlCommand.Parameters.AddWithValue("@workGuildId", workGuildId);
                        sqlCommand.Parameters.AddWithValue("@parcelId", parcelId);
                        using (var reader = sqlCommand.ExecuteReader())
                        {
                            return reader.HasRows;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }

        /// <summary>
        /// Получение коллекции [Материалов] для продукта с ед. изм
        /// </summary>
        public static List<Material> GetMaterialsByProductIdWhithMeasure(long productId)
        {
            const string query = "SELECT [Nu65Table].[id], " +
                                 "[Nu65Table].[materialsId], " +
                                 "[Nu65Table].[auxiliaryMaterialConsumptionRate], " +
                                 "[Nu65Table].[workGuildId], " +
                                 "[Nu65Table].[signMaterial], " +
                                 "[Nu65Table].[parcelId], " +
                                 "[Nu65Table].[date], " +
                                 "[Nu65Table].[flowRate], " +
                                 "[Nu65Table].[measuresId], " +

                                 "[Materials].[name], " +
                                 "[Materials].[profile], " +
                                 "[Materials].[gost], " +
                                 "[Materials].[codeMaterial], " +
                                 "[Nu65Table].[unitValidation], " +
                                 "[Measures].[oldDbCode], " +
                                 "[Measures].[name], " +
                                 "[Measures].[shortName] " +
                                 "FROM[Nu65DB].[dbo].[Nu65Table] " +
                                 "INNER JOIN [Nu65DB].[dbo].[Materials] on [materialsId] = [Materials].[id] " +
                                 "INNER JOIN [Nu65DB].[dbo].[Measures] on [measuresId] = [Measures].[id] " +
                                 "WHERE [productsId] = @productsId";
            var server = Properties.Settings.Default.SqlServerNu65;
            var db = Properties.Settings.Default.SqlDbNu65Db;

            var materials = new List<Material>();
            try
            {
                using (var connection = DbControl.GetConnection(server, db))
                {
                    connection.TryConnectOpen();

                    using (var sqlCommand = new SqlCommand(query, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@productsId", productId);
                        using (var reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var id = reader.GetInt64(0);
                                var materialId = reader.GetInt64(1);
                                var auxiliaryMaterialConsumptionRate = reader.GetString(2);
                                var workGuildId = reader.GetString(3);
                                var signMaterial = reader.GetString(4);
                                var parcelId = reader.GetString(5);
                                var date = reader.GetDateTime(6);
                                var flowRate = reader.GetString(7);
                                var measuresId = reader.GetInt64(8);

                                var materialName = reader.GetString(9);
                                var profile = reader.GetString(10);
                                var gost = reader.GetString(11);
                                var codeMaterial = reader.GetInt64(12);

                                var unitValidation = reader.GetString(13);
                                var oldDbCode = reader.GetInt32(14);
                                var measureName = reader.GetString(15);
                                var measureShortName = reader.GetString(16);

                                materials.Add(new Material()
                                {
                                    Id = materialId,
                                    Nu65TableId = id,
                                    AuxiliaryMaterialConsumptionRate = auxiliaryMaterialConsumptionRate,
                                    WorkGuildId = workGuildId,
                                    SignMaterial = signMaterial,
                                    ParcelId = parcelId,
                                    Date = date,
                                    FlowRate = flowRate,
                                    CodeMaterial = codeMaterial,
                                    Name = materialName,
                                    Profile = profile,
                                    Gost = gost,
                                    UnitValidation = unitValidation,
                                    ServiceMappedMeasureId = measuresId,
                                    Measure = new Measure()
                                    {
                                        Id = measuresId,
                                        OldDbCode = oldDbCode,
                                        Name = measureName,
                                        ShortName = measureShortName
                                    }
                                });
                            }
                        }
                    }
                }

                return materials;
            }
            catch (SqlException ex)
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }
    }
}

