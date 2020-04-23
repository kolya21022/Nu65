using System.Collections.Generic;
using System.Data.SqlClient;

using NU65.Db;
using NU65.Entities.External;

namespace NU65.Storages.Mssql
{
    /// <summary>
    /// Обработчик запросов хранилища данных для таблицы материалов [Material]
    /// </summary>
    public class MaterialsStorageMssql
    {
        /// <summary>
        /// Получение коллекции [Материалов]
        /// </summary>   
        public static List<Material> GetAll()
        {
            const string query = "SELECT [id], [codeMaterial], [name], [profile], [gost] FROM [Materials] ";
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
                        using (var reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var material = new Material()
                                {
                                    Id = reader.GetInt64(0),
                                    CodeMaterial = reader.GetInt64(1),
                                    Name = reader.GetString(2),
                                    Profile = reader.GetString(3),
                                    Gost = reader.GetString(4)
                                };
                                materials.Add(material);
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
	    /// Проверка есть ли в MsSql материал с заданым кодом
	    /// </summary>
	    public static bool IsDublicate(long codeMaterial, SqlConnection connection)
	    {
		    const string query = "SELECT * FROM [Materials] WHERE [codeMaterial] = @codeMaterial";

		    using (var sqlCommand = new SqlCommand(query, connection))
		    {
			    sqlCommand.Parameters.AddWithValue("@codeMaterial", codeMaterial);
			    using (var reader = sqlCommand.ExecuteReader())
			    {
				    return reader.HasRows;
			    }
		    }
	    }

		/// <summary>
		/// Добавление материала в Mssql с возвратом Id
		/// </summary>
		public static long Insert(long codeMaterial, string name, string gost, string profile, SqlConnection connection, 
		    SqlTransaction mssqlTransaction)
	    {
		    const string query = "INSERT INTO [Materials] ([codeMaterial], [name], [gost], [profile]) " +
		                         "OUTPUT INSERTED.ID " +
								 "VALUES (@codeMaterial, @name, @gost, @profile)";
		    using (var sqlCommand = new SqlCommand(query, connection, mssqlTransaction))
		    {
			    sqlCommand.Parameters.AddWithValue("@codeMaterial", codeMaterial);
			    sqlCommand.Parameters.AddWithValue("@name", name);
			    sqlCommand.Parameters.AddWithValue("@gost", gost);
			    sqlCommand.Parameters.AddWithValue("@profile", profile);
				var id = (long)sqlCommand.ExecuteScalar();
			    return id;
		    }
	    }
	}
}
