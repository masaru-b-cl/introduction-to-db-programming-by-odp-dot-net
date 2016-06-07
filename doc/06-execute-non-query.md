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


## ②対象レコードなし

DMLの実行を行った結果、対象レコードがない場合も見てみましょう。例えば、存在しないレコードをDELETE文で削除してみます（リスト6-2）。

リスト6-2 対象レコードなし（Program.csのMainメソッドより）

```csharp
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

```

対象レコードがなくても、SQLの実行自体は失敗しません。ただし、ExecuteNonQueryメソッドの戻り値は0になります。

これを利用すると、例えばUPDATE文を実行した結果、0件なら対象レコードがなかったということで、業務エラーとして処理する、といったことが可能になります。

なお、DDLを実行した結果は必ず-1になりますので、戻り値を判定する必要はありません。

さて、以上で簡単なSQL実行の仕方はわかったと思います。次の章からは、少し進んだ話に入っていきましょう。

[→第7章 パラメーターの利用](07-use-parameter.md)
