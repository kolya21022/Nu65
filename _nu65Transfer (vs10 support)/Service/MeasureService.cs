using System;
using System.Data.OleDb;
using System.IO;
using System.Text;
using TransferNu65.Db;

namespace TransferNu65.Service
{
    public class MeasureService
    {
        public static void GetFileMeasure()
        {
            var dbFolder = Properties.Settings.Default.FoxproDbFolder_nu65;
            const string query = "SELECT kod as MeasureId, kratk_naim as shortName, naimenov as Name " +
                                 "FROM [ed_izmer]";

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
                            var id = Convert.ToInt32(reader.GetString(0).Trim());
                            var shortName = reader.GetString(1).Trim();
                            var name = reader.GetString(2).Trim();

                            stringBuilder.AppendFormat("INSERT INTO Measures (oldDbCode, name, shortName) " +
                                                       "VALUES ({0}, '{1}', '{2}')", id, name, shortName);
                            stringBuilder.AppendLine();
                            stringBuilder.AppendFormat("GO");
                            stringBuilder.AppendLine();
                        }
                    }
                }
            }

            // запись в файл
            using (FileStream fstream = new FileStream(@"w:\Nu65\_projecNu65Transfer (vs10 support)\InsertMeasure.txt", FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(stringBuilder.ToString());
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
                Console.WriteLine("Текст Measure записан в файл");
            }
        }
    }
}
