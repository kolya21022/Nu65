using System;
using System.Collections.Generic;
using System.Data.Common;

using NU65.Db;
using NU65.Entities.External;
using NU65.Entities.Interal;
using NU65.Storages.Foxpro;
using NU65.Storages.Mssql;

namespace NU65.Services
{
    /// <summary>
    /// Обработчик сервисного слоя для класса nu65 - [Nu65]
    /// </summary>
    public class Nu65Service
    {
        /// <summary>
        /// Получение коллекции [Nu65]
        /// </summary>
        public static List<Nu65> GetAll()
        {
            return Nu65StorageMssql.GetAll();
        }

        /// <summary>
        /// Получение коллекции [Nu65View]
        /// </summary>
        public static List<Nu65View> GetAllView()
        {
            return Nu65StorageMssql.GetAllView();
        }

        /// <summary>
		/// Добавление нового [Nu65]
		/// </summary>
		public static long Insert(long productId, string codeProduct, long measureId, string oldDbCodeMeasure, 
            long materialId, string codeMaterial, string auxiliaryMaterialConsumptionRate, string workGuildId, 
            string signMaterial, string parcelId, string unitValidation, DateTime date, string flowRate)
        {
            var mssqlServer = Properties.Settings.Default.SqlServerNu65;
            var mssqlDb = Properties.Settings.Default.SqlDbNu65Db;

            try
            {
                using (var mssqlConnection = DbControl.GetConnection(mssqlServer, mssqlDb))
                {
                    mssqlConnection.TryConnectOpen();
                    using (var mssqlTransaction = mssqlConnection.BeginTransaction())
                    {
                        // Вставка [Nu65] в MSSQL-базу
                        var nu65Id = Nu65StorageMssql.Insert(productId, materialId, measureId, 
                            auxiliaryMaterialConsumptionRate, workGuildId, signMaterial, parcelId,
                            unitValidation, date, flowRate, mssqlConnection, mssqlTransaction);

                        var foxproDbPath = Properties.Settings.Default.FoxproDbFolder_Fox60_Arm_Base;
                        using (var oleDbConnection = DbControl.GetConnection(foxproDbPath))
                        {
                            oleDbConnection.TryConnectOpen();

                            // Проверка кодировок таблиц и соединения с таблицами FoxPro 
                            // Проверка соединений требуется, так как транзации с этими таблицами не работают
                            oleDbConnection.VerifyInstalledEncoding("nu65a");
                            Nu65StorageFoxpro.TestConnection(oleDbConnection);

                            // Вставка [Nu65] в таблицу FoxPro
                            Nu65StorageFoxpro.Insert(oleDbConnection, codeProduct, codeMaterial, oldDbCodeMeasure, 
                                auxiliaryMaterialConsumptionRate, workGuildId, signMaterial, parcelId,
                                unitValidation, date, 0);

                            // Подтверждение транзакции MSSQL
                            mssqlTransaction.Commit();

                            return nu65Id;
                        }
                    }
                }
            }
            catch (DbException ex) // DbException - суперкласс для SqlException и OleDbException
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }

        /// <summary>
		/// Обновление [Nu65]
		/// </summary>
		public static long Update(long id, long productId, string codeProduct, long measureId, string oldDbCodeMeasure,
            long materialId, string codeMaterial, string auxiliaryMaterialConsumptionRate, string workGuildId,
            string signMaterial, string parcelId, string unitValidation, DateTime date, string flowRate, string oldWorkGuildId,
            string oldParcelId)
        {
            var mssqlServer = Properties.Settings.Default.SqlServerNu65;
            var mssqlDb = Properties.Settings.Default.SqlDbNu65Db;

            try
            {
                using (var mssqlConnection = DbControl.GetConnection(mssqlServer, mssqlDb))
                {
                    mssqlConnection.TryConnectOpen();
                    using (var mssqlTransaction = mssqlConnection.BeginTransaction())
                    {
                        // Обновление [Nu65Table] в MSSQL
                        Nu65StorageMssql.Update(id, measureId, auxiliaryMaterialConsumptionRate, workGuildId, 
                            signMaterial, parcelId, unitValidation, flowRate, mssqlConnection, mssqlTransaction);

                        var foxproDbPath = Properties.Settings.Default.FoxproDbFolder_Fox60_Arm_Base;
                        using (var oleDbConnection = DbControl.GetConnection(foxproDbPath))
                        {
                            oleDbConnection.TryConnectOpen();

                            // Проверка кодировок таблиц и соединения с таблицами FoxPro 
                            // Проверка соединений требуется, так как транзации с этими таблицами не работают
                            oleDbConnection.VerifyInstalledEncoding("nu65a");
                            Nu65StorageFoxpro.TestConnection(oleDbConnection);

                            // Обновление [Nu65a] в таблице FoxPro
                            Nu65StorageFoxpro.Update(oleDbConnection, codeProduct, codeMaterial,
                            oldDbCodeMeasure, auxiliaryMaterialConsumptionRate, workGuildId,
                            signMaterial, parcelId, unitValidation, 0, oldWorkGuildId, oldParcelId);

                            // Подтверждение транзакции MSSQL
                            mssqlTransaction.Commit();
                        }
                    }
                }

                return 0;
            }
            catch (DbException ex) // DbException - суперкласс для SqlException и OleDbException
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }

        /// <summary>
        /// Удаление [Материала для продукта]
        /// </summary>
        public static bool Delete(Material material, Product editedProduct)
        {
            var mssqlServer = Properties.Settings.Default.SqlServerNu65;
            var mssqlDb = Properties.Settings.Default.SqlDbNu65Db;

            try
            {
                using (var mssqlConnection = DbControl.GetConnection(mssqlServer, mssqlDb))
                {
                    mssqlConnection.TryConnectOpen();
                    using (var mssqlTransaction = mssqlConnection.BeginTransaction())
                    {
                        // Удаление записи о материале в MSSQL таблице
                        Nu65StorageMssql.DeleteById(material.Nu65TableId, mssqlConnection, mssqlTransaction);

                        var foxproDbPath = Properties.Settings.Default.FoxproDbFolder_Fox60_Arm_Base;


                        using (var oleDbConnection = DbControl.GetConnection(foxproDbPath))
                        {
                            oleDbConnection.TryConnectOpen();
                            Nu65StorageFoxpro.TestConnection(oleDbConnection);

                            // Удаление записи о материале в FoxPro таблице
                            Nu65StorageFoxpro.DeleteByMaterialCode(oleDbConnection, editedProduct.DisplayCodeString, material.CodeMaterial, 
                                material.WorkGuildId, material.ParcelId);

                            // Подтверждение транзакции MSSQL
                            mssqlTransaction.Commit();
                            return true;

                        }
                    }
                }
            }
            catch (DbException ex) // DbException - суперкласс для SqlException и OleDbException
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }

        /// <summary>
        /// Проверка на дублиткат
        /// </summary>
        public static bool IsDublicate(long productId, long materialId, string workGuildId, string parcelId)
        {
            return Nu65StorageMssql.IsDublicate(productId, materialId, workGuildId, parcelId);
        }
    }
}
