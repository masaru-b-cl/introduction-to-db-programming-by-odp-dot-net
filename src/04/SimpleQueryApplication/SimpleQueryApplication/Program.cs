using System;
using System.Configuration;
using System.Data.Common;

namespace SimpleQueryApplication
{
  class Program
  {
    static void Main(string[] args)
    {
      var connectionStringSettings = ConfigurationManager.ConnectionStrings["SCOTT"];

      var dbProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);

      using (var dbConnection = dbProviderFactory.CreateConnection())
      {
        dbConnection.ConnectionString = connectionStringSettings.ConnectionString;

        dbConnection.Open();

        // ①DbCommandオブジェクト作成
        using (var dbCommand = dbConnection.CreateCommand())
        {
          // ②実行SQL文設定
          dbCommand.CommandText = @"
            select
             EMPNO
            ,ENAME
            ,SAL
            ,COMM
            from
             EMP
          ";

          // ③問い合わせ実行
          using (var dbDataReader = dbCommand.ExecuteReader())
          {
            // ④DbDataReader読み込み
            while (dbDataReader.Read())
            {
              // ⑤レコードデータ取得
              // (1) nullでないことが確定している整数を列インデックスで取得
              var empno = dbDataReader.GetInt32(0);
              // (2) 文字列を列名で取得
              var ename = dbDataReader["ENAME"] as string;
              // (3) nullでないことが確定している小数を列名で取得
              var sal = Convert.ToDecimal(dbDataReader["SAL"]);
              // (4) nullがあるかもしれない小数を列名で取得
              var comm = dbDataReader["COMM"] is DBNull ? (decimal?)null
                                                        : Convert.ToDecimal(dbDataReader["COMM"]);
              // 取得データを表示
              Console.WriteLine($"{empno}\t{ename}\t{sal,6:#,##0}\t{comm,6:#,##0}");
            }

            // ⑥DbDataReaderを閉じる
            dbDataReader.Close();
          }
        }

        dbConnection.Close();
      }
    }
  }
}
