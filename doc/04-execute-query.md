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

    // ③問い合わせ実行
    using (var dbDataReader = dbCommand.ExecuteReader())
    {
      // ④DbDataReader読み込み
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

      // ⑥DbDataReaderを閉じる
      dbDataReader.Close();
    }
  }

  dbConnection.Close();
}
```

### ①DbCommandオブジェクト作成

まず最初にSQL実行用のDbCommandオブジェクトをDbConnectionクラスのCreateCommandメソッドを使って作成します。DbCommandクラスは他のクラス同様IDisposableインターフェイスを実装していますので、using文を使って確実にDisposeメソッドが呼ばれるようにしてください。

### ②実行SQL文設定

次に実行したいSELECT文を書いたSQLをDbCommandオブジェクトに設定します。この時、逐語的リテラル文字列（@"..."）を使って自由に改行、空白を含んだ形でSQLを書くと良いでしょう。また、例えば次のような基準で、メンテナンスしやすい形でSQLを書いてやるのも良いでしょう。

- SQLキーワードは小文字
- 列名は大文字
- 1行に1キーワード
- 1行に1項目
- ","（カンマ）は前置
    - 最後に項目を追加する際、ひとつ前の行の末尾に","を付け足す必要が無いため

### ③問い合わせ実行

SQL実行準備が整ったところで、SQLを実行します。SELECT文のようなデータ読み取りには、DbCommandクラスのExecuteReaderメソッドを呼び出します。このメソッドの戻り値はDbDataReader型で、取得したDbDataReaderオブジェクトを通してDBの「カーソル」を制御することが出来ます。つまり、カーソルのオープンが行われているのです。
なお、DbDataReader型もIDisposableインターフェイスを実装しているので、using文を使うことを忘れないで下さい。





[→第5章 スカラー値の取得](05-get-scalar-value.md)  
