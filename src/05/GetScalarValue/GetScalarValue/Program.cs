using System;
using System.Configuration;
using System.Data.Common;

namespace GetScalarValue
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

        // ①集計値の取得
        using (var dbCommand = dbConnection.CreateCommand())
        {
          // 実行SQL文設定
          dbCommand.CommandText = @"
            select
             COUNT(EMPNO)
            from
             EMP
          ";

          // 取得値を変換
          var employeeCount = Convert.ToInt32(dbCommand.ExecuteScalar());

          Console.WriteLine($"EMPLOYEE COUNT : {employeeCount}");
        }

        // ②主キーを指定指定して値を取得
        using (var dbCommand = dbConnection.CreateCommand())
        {
          // 実行SQL文設定
          dbCommand.CommandText = @"
            select
             ENAME
            from
             EMP
            where
             EMPNO = 7900
          ";

          // 取得値を変換
          var employeeName = dbCommand.ExecuteScalar() as string;

          Console.WriteLine($"EMPLOYEE NAME : {employeeName}");
        }

        dbConnection.Close();
      }
    }
  }
}
