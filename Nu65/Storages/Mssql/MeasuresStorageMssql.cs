using System.Collections.Generic;
using System.Data.SqlClient;

using NU65.Db;
using NU65.Entities.External;

namespace NU65.Storages.Mssql
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблицы единиц измерения [Measures]
	/// </summary>
	public static class MeasureStorageMssql
    {
		/// <summary>
		/// Получение коллекции [Единиц измерения]
		/// </summary>
	    public static List<Measure> GetAll()
	    {
	        const string query = "SELECT [id], [oldDbCode], [name], [shortName] FROM [Measures] ";
	        var server = Properties.Settings.Default.SqlServerNu65;
	        var db = Properties.Settings.Default.SqlDbNu65Db;

	        var measures = new List<Measure>();
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
	                            var measure = new Measure
                                {
	                                Id = reader.GetInt64(0),
	                                OldDbCode = reader.GetInt32(1),
	                                Name = reader.GetString(2),
	                                ShortName = reader.GetString(3)
	                            };
	                            measures.Add(measure);
                            }
	                    }
	                }
	            }
	            return measures;
	        }
	        catch (SqlException ex)
	        {
	            throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
	        }
	    }

        /// <summary>
        /// Добавление новой [Единицы измерения]
        /// </summary>
	    public static void Insert(long oldDbCode, string name, string shortName, SqlConnection connection)
	    {
	        const string query = "INSERT INTO [Measures] ([oldDbCode], [name], [shortName]) " +
	                             "VALUES (@oldDbCode, @name, @shortName)";
	        using (var sqlCommand = new SqlCommand(query, connection))
	        {
	            sqlCommand.Parameters.AddWithValue("@oldDbCode", oldDbCode);
	            sqlCommand.Parameters.AddWithValue("@name", name);
	            sqlCommand.Parameters.AddWithValue("@shortName", shortName);
	            sqlCommand.ExecuteNonQuery();
	        }
	    }

        /// <summary>
        /// Удаление [Единицы измерения]
        /// </summary>
	    public static void DeleteById(long measureId, SqlConnection connection)
	    {
	        const string queryDelete = "DELETE FROM [Measures] WHERE [id] = @id";
	        using (var sqlCommand = new SqlCommand(queryDelete, connection))
	        {
	            sqlCommand.Parameters.AddWithValue("@id", measureId);
	            sqlCommand.ExecuteNonQuery();
	        }
	    }
    }
}
