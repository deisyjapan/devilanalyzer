using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;


namespace DevilAnalyzer.DA
{
    class DataAccess
    {
        // 接続文字列で指定しているDataSourceがファイル名になります。拡張子はなんでもよし
        // ファイルがあればそれをオープンし、なければ指定ファイル名で作成されます。
//        private const string ConnectionString = @"Data Source=E:\開発\D2メガテン\data\D2Data.db";
        private const string ConnectionString = @"Data Source=.\D2Data.db";

        // データの読み出し
        public DataTable readdata(string prmStrQuery)
        {
            var list = new List<string>();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();

                using (var cmd = con.CreateCommand())
                {
                    //                    cmd.CommandText = "select SP.ShipName from Ship as SP";
                    cmd.CommandText = prmStrQuery;


                    cmd.ExecuteNonQuery();
                    using (var reader = cmd.ExecuteReader())
                    {
                        // SELECTの結果にNULLのデータがあると例外となる。
                        dt.Load(reader);
                    }
                }
                con.Close();
            }
            // dt dswodisposeしなくて平気か？
            return dt;
        }
    }
}
