第9章 トランザクション管理
=====

[↑目次](..\README.md "目次")

[←第8章 ストアド・プロシージャ](08-call-stored-procedure.md)

実際にアプリケーションを作成する際、DBアクセスにはトランザクション管理が欠かせません。本章ではトランザクションの開始、終了をどのように制御するのか学びましょう。

## ①手動トランザクション

まず、トランザクションをすべて手動で管理する方法です。トランザクションを制御するには、DbTransactionクラスを用います（リスト9-1）。

リスト9-1 手動トランザクション（Program.csのMainメソッドより）

```csharp
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
```

### (1) トランザクション開始

まず、DbConnectionクラスのBeginTransactionメソッドを呼び出し、トランザクションを開始します。戻り値のDbTransactionオブジェクトを使い、以後のトランザクション管理に使います。

なお、DbTransactionクラスもIDisposableインターフェイスを実装していますので、using文を使うことを忘れないで下さい。

### (2) トランザクション設定

トランザクションに参加させたいSQL実行を行うDbCommandオブジェクトのTransactionプロパティに、(1)で取得したDbTransactionオブジェクトを設定します。つまり、複数のトランザクションを併用して、別々のSQL実行を行うことも可能だということです。

### (3) トランザクションコミット

トランザクション内のSQLを実行が全て終わったら、DbTransactionクラスのCommitメソッドを呼び出し、トランザクションをコミットして終了させます。

### (4) トランザクションロールバック

何かしらのエラー（業務エラー、実行エラーに関わらず）が発生した際は、DbTransactionクラスのRollbackメソッドを呼び出し、トランザクションをロールバックします。この時、実行エラーを漏れ無く補足するため、例外型を指定しないcatchブロックにてロールバックし、その後throw文で例外を再スローします。

なお、Commitメソッド、Rollbackメソッドのどちらも呼ばれずにDisposeメソッドが呼ばれると、トランザクションはロールバックされます。したがって、usingブロックを使っていればcatchブロックは無くても、例外発生時はロールバックされます。

ただ、明示的にロールバックしている方が、プログラムの意図がわかりやすいので、省略しない方をお勧めしておきます。





[→第10章 例外処理](10-handle-exception.md)
