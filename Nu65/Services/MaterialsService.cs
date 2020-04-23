using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;

using NU65.Entities.External;
using NU65.Storages.Foxpro;
using NU65.Storages.Mssql;

namespace NU65.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса единиц измерения - [Material]
	/// </summary>
	public class MaterialsService
	{
		/// <summary>
		/// Получение коллекции [Материалов] из cenad
		/// </summary>
		public static List<Material> GetAllCenad()
		{
			return MaterialsStorageFoxpro.GetAllCenad();

		}

        /// <summary>
        /// Получение коллекции [Материалов] из Mssql
        /// </summary>
        public static List<Material> GetAllMssql()
	    {
	        return MaterialsStorageMssql.GetAll();
	    }

	    /// <summary>
	    /// Получение коллекции [Материалов] продукта
	    /// </summary>
	    public static List<Material> GetMaterialsByProductId(long productId)
	    {
	        return Nu65StorageMssql.GetMaterialsByProductId(productId);
        }

		/// <summary>
		/// Проверка есть ли в MsSql материал с заданым кодом
		/// </summary>
		public static bool IsDublicateMssql(long codeMaterial, SqlConnection connection)
		{
			return MaterialsStorageMssql.IsDublicate(codeMaterial, connection);
		}
		/// <summary>
		/// Добавление материала в Mssql с возвратом Id
		/// </summary>
		public static long InsertMssql(long codeMaterial, string name, string gost, string profile, SqlConnection connection, 
		    SqlTransaction mssqlTransaction)
		{
			return MaterialsStorageMssql.Insert(codeMaterial, name, gost, profile, connection, mssqlTransaction);
		}

        /// <summary>
        /// Проверка есть ли в prdsetmc материал с заданым кодом
        /// </summary>
        public static bool IsDublicateFoxPro(long codeMaterial, OleDbConnection connection)
	    {
	        return MaterialsStorageFoxpro.IsDublicate(codeMaterial, connection);
        }

	    /// <summary>
	    /// Добавление материала в prdsetmc
	    /// </summary>
	    public static void InsertFoxpro(OleDbConnection oleDbConnection, Material selectedMaterial)
	    {
	        MaterialsStorageFoxpro.InsertFoxpro(oleDbConnection, selectedMaterial);
        }

	    /// <summary>
	    /// Получение коллекции [Материалов] продукта с ед. изм.
	    /// </summary>
	    public static List<Material> GetMaterialsByProductIdWhithMeasure(long productId)
	    {
	        return Nu65StorageMssql.GetMaterialsByProductIdWhithMeasure(productId);
        }
	}
}
