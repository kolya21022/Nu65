using System;
using System.Collections.Generic;
using System.Data.OleDb;

using NU65.Db;
using NU65.Entities.External;

namespace NU65.Storages.Foxpro
{
    public class MaterialsStorageFoxpro
    {
        /// <summary>
        /// Тестирование соединения с таблицей базы данных FoxPro
        /// </summary>
        public static void TestConnectionPrdsetmc(OleDbConnection oleDbConnection)
        {
            const string testQuery = "SELECT * FROM [prdsetmc]";
            using (var oleDbCommand = new OleDbCommand(testQuery, oleDbConnection))
            {
                using (oleDbCommand.ExecuteReader()) { }
            }
        }

        /// <summary>
        /// Получение коллекции [Материалы] из cenad в SKL  
        /// </summary>
        public static List<Material> GetAllCenad()
        {
            var dbFolder = Properties.Settings.Default.FoxproDbFolder_Skl;

            const string query = "SELECT cenmat.kmat as CodeMaterial, " +
                                 "cenmat.naim as Name, " +
                                 "cenmat.marka, " +
                                 "cenmat.gost, " +
                                 "cenmat.kedizm as cenmatMeasureId, " +
                                 "cenmat.sklad as cenmatWarehouseId " +
                                 "FROM [cenmat] " +
                                 "WHERE (sklad = 51 or sklad = 53 or sklad = 54) " +
                                        "and kmat <> 0";
            var materials = new List<Material>();
            try
            {
                using (var connection = DbControl.GetConnection(dbFolder))
                {
                    connection.TryConnectOpen();
                    connection.VerifyInstalledEncoding("cenmat");
                    using (var oleDbCommand = new OleDbCommand(query, connection))
                    {
                        using (var reader = oleDbCommand.ExecuteReader())
                        {
                            while (reader != null && reader.Read())
                            {
                                var codeMaterial = reader.GetDecimal(0);
                                var name = reader.GetString(1).Trim();
                                var marka = reader.GetString(2).Trim();
                                var gost = reader.GetString(3).Trim();
                                var measureId = reader.GetValue(4) == null ? 0
                                    : reader.GetDecimal(4);
                                var warehouse = reader.GetDecimal(5);

                                var material = new Material()
                                {
                                    CodeMaterial = (long)codeMaterial,
                                    Name = name,
                                    Profile = marka,
                                    Gost = gost,
                                    ServiceMappedMeasureId = (long)measureId,
                                    ServiceMappedWarehouseId = warehouse
                                };
                                materials.Add(material);
                            }
                        }
                    }
                }
                return materials;
            }
            catch (OleDbException ex)
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }

        /// <summary>
	    /// Проверка есть ли материал с заданым кодом  в prdsetmc
	    /// </summary>
	    public static bool IsDublicate(long codeMaterial, OleDbConnection connection)
	    {
		    const string query = "SELECT * FROM [prdsetmc] WHERE kod_mater = ?";

	        using (var oleDbCommand = new OleDbCommand(query, connection))
	        {
	            oleDbCommand.Parameters.AddWithValue("@kod_mater", Convert.ToDecimal(codeMaterial));
	            using (var reader = oleDbCommand.ExecuteReader())
	            {
                    return reader != null && reader.HasRows;
			    }
		    }
	    }

        /// <summary>
        /// Добавление материала в prdsetmc
        /// </summary>
        public static void InsertFoxpro(OleDbConnection oleDbConnection, Material selectedMaterial)
        {
            const string query = "INSERT INTO [prdsetmc] (kod_mater, naim, marka, gost, " +
                                 "pkoeff, nom_skl, pkei, naimdrag, wes, ed_norm, priz, prin, " +
                                 "oth_skl, oth_pkei, cena_pb, cena_pf, pr_othod, chislo, chislo_i, schet) " +
                                 "VALUES (?, ?, ?, ?, " +
                                 "0.0, ?, 0, '', 0.0, 1, 'V', 72, " +
                                 "0, 0, 0.0, '', '*', {..}, {..}, '')";

            using (var oleDbCommand = new OleDbCommand(query, oleDbConnection))
            {
                oleDbCommand.Parameters.AddWithValue("@kod_mater", Convert.ToDecimal(selectedMaterial.CodeMaterial));
                oleDbCommand.Parameters.AddWithValue("@naim", selectedMaterial.Name);
                oleDbCommand.Parameters.AddWithValue("@marka", selectedMaterial.Profile);
                oleDbCommand.Parameters.AddWithValue("@gost", selectedMaterial.Gost);
                oleDbCommand.Parameters.AddWithValue("@nom_skl", selectedMaterial.ServiceMappedWarehouseId);
                oleDbCommand.ExecuteNonQuery();
            }
        }
    }
}
