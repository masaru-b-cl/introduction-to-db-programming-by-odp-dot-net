using System;
using System.Configuration;
using System.Data.Common;
using System.Transactions;

namespace ManageTransaction
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

        // ①手動トランザクション
        // (1) トランザクション開始
        using (var dbTransaction = dbConnection.BeginTransaction())
        {
          try
          {
            // データを更新する
            using (var dbCommand = dbConnection.CreateCommand())
            {
              dbCommand.CommandText = $@"
                update EMP
                set
                 ENAME = 'SMITH'
                where
                 EMPNO = 7369
              ";

              // (2) トランザクション設定
              dbCommand.Transaction = dbTransaction;

              var updateCount = dbCommand.ExecuteNonQuery();

              Console.WriteLine($"UPDATE COUNT : {updateCount}");
            }

            // (3) トランザクションコミット
            dbTransaction.Commit();
          }
          catch
          {
            // (4) トランザクションロールバック
            dbTransaction.Rollback();
            throw;
          }
        }

        // ②自動トランザクション
        // (1) トランザクションスコープ作成
        using (var transactionScope = new TransactionScope())
        {
          // データを更新する
          using (var dbCommand = dbConnection.CreateCommand())
          {
            dbCommand.CommandText = $@"
                update EMP
                set
                 ENAME = 'SMITH'
                where
                 EMPNO = 7369
              ";

            var updateCount = dbCommand.ExecuteNonQuery();

            Console.WriteLine($"UPDATE COUNT : {updateCount}");
          }

          // (2) トランザクション完了
          transactionScope.Complete();
        }

        // 変更後の社員名の取得
        var updatedName = GetEmployeeName(dbConnection, 7369);
        Console.WriteLine($"EMPLOYEE NAME : {updatedName}");

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
