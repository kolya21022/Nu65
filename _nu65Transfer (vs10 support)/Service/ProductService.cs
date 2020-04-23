using System;
using System.Data.OleDb;
using System.IO;
using System.Text;
using TransferNu65.Db;

namespace TransferNu65.Service
{
    public class ProductService
    {
        public static void GetFileProduct()
        {
            var dbFolder = Properties.Settings.Default.FoxproDbFolder_Fox60_Arm_Base;
            const string query = "SELECT kod_mater AS id, " +
                                 "naim AS name, " +
                                 "marka as mark " +
                                 "FROM [prdsetmc] WHERE kod_mater > 9999999999";

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
                            var mark = reader.GetValue(2) == null ? string.Empty : reader.GetString(2).Trim();

                            stringBuilder.AppendFormat("INSERT INTO Products (codeProduct, name, mark) " +
                                                       "VALUES ({0}, '{1}', '{2}')", id, name, mark);
                            stringBuilder.AppendLine();
                            stringBuilder.AppendFormat("GO");

                            stringBuilder.AppendLine();
                        }
                    }
                }
            }

            // запись в файл
            using (FileStream fstream = new FileStream(@"w:\Nu65\_projecNu65Transfer (vs10 support)\InsertProduct.txt", FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(stringBuilder.ToString());
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
                Console.WriteLine("Текст Product записан в файл");
            }
        }
    }
}
