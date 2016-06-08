using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace UseParameter
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

        Console.Write("検索したい社員名の先頭を入力してください : ");
        var ename = Console.ReadLine();

        // ①文字列連結
        using (var dbCommand = dbConnection.CreateCommand())
        {
          dbCommand.CommandText = $@"
            select
             COUNT(EMPNO)
            from
             EMP
            where
             ENAME like '{ename}%'
          ";

          var employeeCount = Convert.ToInt32(dbCommand.ExecuteScalar());
          Console.WriteLine($"該当社員数 : {employeeCount}");
        }

        Console.WriteLine();

        // ②パラメーター利用
        using (var dbCommand = dbConnection.CreateCommand())
        {
          // (1) パラメーター埋め込み
          dbCommand.CommandText = @"
            select
             COUNT(EMPNO)
            from
             EMP
            where
             ENAME like :ENAME || '%'
          ";

          // (2) パラメーター作成
          var enameParameter = dbCommand.CreateParameter();
          // (3) パラメーターの型指定
          enameParameter.DbType = DbType.String;
          // (4) パラメーターの値設定
          enameParameter.Value = ename;

          // (5) パラメーターを追加
          dbCommand.Parameters.Add(enameParameter);

          var employeeCount = Convert.ToInt32(dbCommand.ExecuteScalar());
          Console.WriteLine($"該当社員数 : {employeeCount}");
        }

        dbConnection.Close();
      }
    }

  }
}
