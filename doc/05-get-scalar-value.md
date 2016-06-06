第5章 スカラー値の取得
=====

[↑目次](..\README.md "目次")

[←第4章 単純な問い合わせ](04-execute-query.md)

前章では単純な問い合わせのやり方について学びました。しかし、実際のアプリケーションでは件数を始めとした集計や、主キーを使った名称取得など、「単一行、単一項目」を取得するケースも良くあります。本章ではこういった「スカラー値」を取得する方法について学びましょう。

## ExecuteReaderメソッド

件数や単一項目のような「スカラー値」を取得するにはどうすればいいでしょうか？前章で紹介したDbDataReaderを使うことはもちろん出来ます。しかし、あまりにも仰々しいと感じられるでしょう。

そこで、DbCommandクラスに用意されたスカラー値取得メソッドが、ExecuteScalarです。

### ①集計値の取得

例えば、COUNT関数を使った集計値を取得する事を考えましょう。集計値は、集計対象レコードの値をまとめて1つにしたものなので、「単一行、単一項目」の代表と言えるでしょう。前述のExecuteScalarメソッドを使って取得してみます（リスト5-1）。

リスト5-1 件数取得処理（Program.csのMainメソッドより）

```csharp
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
```

ExecuteScalarメソッド実行時も、DB接続を開いてDbCommandオブジェクトを作成する手順は変わりません。ExecuteScalarメソッドの戻り値はobject型ですので、DbDataReaderの列名指定のインデクサーと同様に、キャストもしくは変換が必要です。そして、同じ理由でキャストではなくConvet.To型名メソッドを勧めます。



[→第6章 非問い合わせ](06-execute-non-query.md)  
