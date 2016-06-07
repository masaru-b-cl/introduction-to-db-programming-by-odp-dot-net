第6章 非問い合わせ
=====

[↑目次](..\README.md "目次")

[←第5章 スカラー値の取得](05-get-scalar-value.md)  

前の2つの章ではSELECT文を使った問い合わせの実行方法を学びました。本章ではそれ以外のINSERT、UPDATE、DELETE文などの「非問い合わせ」の実行方法を学びましょう。

## ①データを更新する

非問い合わせの代表的なものはINSERT、UPDATE、DELETE文といったDMLです。もちろんCREATE文等のDDLも実行できますが、あまり機会はないでしょう。そこで、まずはUPDATE文の実行方法を見てみましょう（リスト6-1）。

リスト6-1 データ更新処理（Program.csのMainメソッドより）

```csharp
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
```

実行方法は問い合わせとほぼ変わらず、DbCommandオブジェクトを作成し、SQLを設定後に、ExecuteNonQueryメソッドを呼び出します。ExecuteNonQueryメソッドの戻り値は、SQLによって影響を受けたレコード件数です。今回の場合、主キーを指定して実行しているので、戻り値は1になります。


[→第7章 パラメーターの利用](07-use-parameter.md)
