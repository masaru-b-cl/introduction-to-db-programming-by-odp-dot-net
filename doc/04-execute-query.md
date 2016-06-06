第4章 単純な問い合わせ
=====

[↑目次](..\README.md "目次")

[←第3章 DBに接続する](03-connect-to-db.md)

DB接続できたところで、まずは単純なSELECT文の実行（問い合わせ）を行ってみましょう。

## 問い合わせ実行方法

SELECT文の実行はDbCommand、DbDataReaderという2つのクラスを用いて行います（リスト4-1）。

リスト4-1 SELECT文実行処理（Program.csのMainメソッドより）

```csharp
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

    // ③カーソルオープン
    using (var dbDataReader = dbCommand.ExecuteReader())
    {
      // ④カーソル読み込み
      while (dbDataReader.Read())
      {
        // ⑤カーソルからデータ取得
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

      // ⑥カーソルクローズ
      dbDataReader.Close();
    }
  }

  dbConnection.Close();
}
```

### ①DbCommandオブジェクト作成

まず最初にSQL実行用のDbCommandオブジェクトをDbConnectionクラスのCreateCommandメソッドを使って作成します。DbCommandクラスは他のクラス同様IDisposableインターフェイスを実装していますので、using文を使って確実にDisposeメソッドが呼ばれるようにしてください。


[→第5章 スカラー値の取得](05-get-scalar-value.md)  
