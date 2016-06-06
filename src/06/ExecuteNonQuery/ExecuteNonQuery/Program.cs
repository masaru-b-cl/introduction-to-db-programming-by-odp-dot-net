using System;
using System.Configuration;
using System.Data.Common;

namespace ExecuteNonQuery
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

        // 変更前の社員名の取得
        var currentName = GetEmployeeName(dbConnection, 7369);
        Console.WriteLine($"EMPLOYEE NAME : {currentName}");

        // ①データを更新する
        using (var dbCommand = dbConnection.CreateCommand())
        {
          dbCommand.CommandText = $@"
            update EMP
            set
             ENAME = 'WILLIAM'
            where
             EMPNO = 7369
          ";

          var updateCount = dbCommand.ExecuteNonQuery();

          Console.WriteLine($"UPDATE COUNT : {updateCount}");
        }

        // 変更前の社員名の取得
        var updatedName = GetEmployeeName(dbConnection, 7369);
        Console.WriteLine($"EMPLOYEE NAME : {updatedName}");


        // ②対象レコードなし
        using (var dbCommand = dbConnection.CreateCommand())
        {
          dbCommand.CommandText = $@"
            delete from EMP
            where
             EMPNO = 9999
          ";

          var deleteCount = dbCommand.ExecuteNonQuery();

          Console.WriteLine($"DELETE COUNT : {deleteCount}");
        }

        dbConnection.Close();
      }
    }

    private static string GetEmployeeName(DbConnection dbConnection, int empNo)
    {
      using (var dbCommand = dbConnection.CreateCommand())
      {
        dbCommand.CommandText = $@"
            select
             ENAME
            from
             EMP
            where
             EMPNO = {empNo}
          ";

        return dbCommand.ExecuteScalar() as string;
      }
    }
  }
}
