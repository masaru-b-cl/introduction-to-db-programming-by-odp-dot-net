using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace CallStoredProcedure
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

        // ①直接呼出し
        using (var dbCommand = dbConnection.CreateCommand())
        {
          // (1) 対象ストアド・プロシージャ指定
          dbCommand.CommandText = "GET_EMP_CNT";
          dbCommand.CommandType = CommandType.StoredProcedure;

          // (2) 戻り値用パラメーター設定
          var returnParameter = dbCommand.CreateParameter();
          returnParameter.Direction = ParameterDirection.ReturnValue;
          returnParameter.DbType = DbType.Int32;
          dbCommand.Parameters.Add(returnParameter);

          // (3) 引数用パラメーター設定
          var enameParameter = dbCommand.CreateParameter();
          enameParameter.DbType = DbType.String;
          enameParameter.Value = ename;
          dbCommand.Parameters.Add(enameParameter);

          // (4) ストアド・プロシージャ実行
          dbCommand.ExecuteNonQuery();

          // (5) 戻り値取得
          var employeeCount = Convert.ToInt32(returnParameter.Value);

          Console.WriteLine($"該当社員数 : {employeeCount}");
        }

        Console.WriteLine();

        // ②無名ブロック使用
        using (var dbCommand = dbConnection.CreateCommand())
        {
          // (1) PL/SQL無名ブロック定義
          dbCommand.CommandText = @"
            BEGIN
              :CNT := GET_EMP_CNT(:ENAME);
            END;
          ";

          // (2) 戻り値用出力パラメーター設定
          var returnParameter = dbCommand.CreateParameter();
          returnParameter.Direction = ParameterDirection.Output;
          returnParameter.DbType = DbType.Int32;
          dbCommand.Parameters.Add(returnParameter);

          // (3) 引数用入力パラメーター設定
          var enameParameter = dbCommand.CreateParameter();
          enameParameter.DbType = DbType.String;
          enameParameter.Value = ename;
          dbCommand.Parameters.Add(enameParameter);

          // (4) ストアド・プロシージャ実行
          dbCommand.ExecuteNonQuery();

          // (5) 戻り値取得
          var employeeCount = Convert.ToInt32(returnParameter.Value);

          Console.WriteLine($"該当社員数 : {employeeCount}");
        }

        dbConnection.Close();
      }
    }

  }
}
