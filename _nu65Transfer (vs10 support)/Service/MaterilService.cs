using System;
using System.Data.OleDb;
using System.IO;
using System.Text;

using TransferNu65.Db;

namespace TransferNu65.Service
{
    public class MaterilService
    {
        public static void GetFileMaterial()
        {
            var dbFolder = Properties.Settings.Default.FoxproDbFolder_Fox60_Arm_Base;
            const string query = "SELECT prdsetmc.kod_mater as MaterialId, " +
                                 "prdsetmc.naim as Name, " +
                                 "prdsetmc.marka, " +
                                 "prdsetmc.gost " +
                                 "FROM [prdsetmc] " +
                                 "WHERE (nom_skl = 51 or nom_skl = 53 or nom_skl = 54) " +
                                 "and kod_mater <> 0 and kod_mater <= 9999999999";

            var stringBuilder = new StringBuilder();

            using (var connection = DbControl.GetConnection(dbFolder))
            {
                connection.TryConnectOpen();
                using (var oleDbCommand = new OleDbCommand(query, connection))
                {
                    using (var reader = oleDbCommand.ExecuteReader())
                    {
                        while (reader != null && reader.Read())
                        {
                            var id = reader.GetDecimal(0);
                            var name = reader.GetString(1).Trim();
                            var marka = reader.GetString(2).Trim();
                            var gost = reader.GetString(3).Trim();

                            stringBuilder.AppendFormat("INSERT INTO Materials (codeMaterial, name, profile, gost) " +
                                                       "VALUES ({0}, '{1}', '{2}', '{3}')", id, name, marka, gost);
                            stringBuilder.AppendLine();
                            stringBuilder.AppendFormat("GO");
                            stringBuilder.AppendLine();
                        }
                    }
                }
            }

            // запись в файл
            using (FileStream fstream = new FileStream(@"w:\Nu65\_projecNu65Transfer (vs10 support)\InsertMaterial.txt", FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(stringBuilder.ToString());
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
                Console.WriteLine("Текст Material записан в файл");
            }
        }
    }
}
