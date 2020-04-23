using System.Collections.Generic;
using System.Data.SqlClient;

using NU65.Db;
using NU65.Entities.External;
using NU65.Storages.Mssql;

namespace NU65.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса единиц измерения - [Measure]
	/// </summary>
	public class MeasuresService
	{
		/// <summary>
		/// Получение коллекции [Единиц измерения]
		/// </summary>
		public static List<Measure> GetAll()
		{
			return MeasureStorageMssql.GetAll();
		}

        /// <summary>
        /// Добавление новой [Единицы измерения]
        /// </summary>
        public static void Insert(long id, string name, string shortName)
		{
		    var server = Properties.Settings.Default.SqlServerNu65;
		    var db = Properties.Settings.Default.SqlDbNu65Db;
		    try
		    {
		        using (var connection = DbControl.GetConnection(server, db))
		        {
		            connection.TryConnectOpen();
		            MeasureStorageMssql.Insert(id, name, shortName, connection);
                }
		    }
		    catch (SqlException ex)
		    {
		        throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
		    }
        }

        /// <summary>
        /// Удаление [Единицы измерения] с указанным [Id]
        /// </summary>
	    public static void DeleteById(long id)
	    {
	        var server = Properties.Settings.Default.SqlServerNu65;
	        var db = Properties.Settings.Default.SqlDbNu65Db;
	        try
	        {
	            using (var connection = DbControl.GetConnection(server, db))
	            {
	                connection.TryConnectOpen();
	                MeasureStorageMssql.DeleteById(id, connection);
                }
	        }
	        catch (SqlException ex)
	        {
	            throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
	        }
	    }
    }
}
