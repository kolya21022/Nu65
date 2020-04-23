using System.Collections.Generic;
using System.Data.SqlClient;

using NU65.Entities.External;
using NU65.Storages.Foxpro;
using NU65.Storages.Mssql;

namespace NU65.Services
{
    /// <summary>
    /// Обработчик сервисного слоя для класса изделия - [Product]
    /// </summary>
    public class ProductsService
    {
        /// <summary>
        /// Получение коллекции [Изделий] из Mssql
        /// </summary>
        public static List<Product> GetAllMssql()
        {
            return ProductsStorageMssql.GetAll();
        }

        /// <summary>
        /// Получение коллекции [Изделий] из FoxPro
        /// </summary>
        public static List<Product> GetAllFoxPro()
        {
            return ProductsStorageFoxPro.GetAll();
        }

        /// <summary>
        /// Проверка есть ли в MsSql продукт с заданым кодом
        /// </summary>
        public static bool IsDublicate(long selectProductCodeProduct, SqlConnection connection)
        {
            return ProductsStorageMssql.IsDublicate(selectProductCodeProduct, connection);
        }

        /// <summary>
        /// Добавление продукта в Mssql с возвратом Id
        /// </summary>
        public static long Insert(long codeProduct, string name, string mark, SqlConnection connection)
        {
            return ProductsStorageMssql.Insert(codeProduct, name, mark, connection);
        }
    }
}
