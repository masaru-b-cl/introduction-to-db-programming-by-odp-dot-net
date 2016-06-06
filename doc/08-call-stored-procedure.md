第8章 ストアド・プロシージャ
=====

[↑目次](..\README.md "目次")

[←第7章 パラメーターの利用](07-use-parameter.md)

ここまでの章の内容を使えば、安全にSQLを実行してDBアクセスを行えるようになりました。ただ、DBに用意されたものはテーブルやビューなどだけではありません。本章では、ストアド・プロシージャをプログラムから呼び出す方法について学びましょう。

## 呼び出すストアド・プロシージャ

今回は、以下の社員名から該当社員数を取得するファンクションを呼び出してみます（リスト8-1）。

リスト8-1 社員数取得ファンクション（GET_EMP_CNT.sql）

```
CREATE OR REPLACE FUNCTION GET_EMP_CNT(IN_ENAME IN EMP.ENAME%TYPE)
RETURN PLS_INTEGER
IS
  CNT PLS_INTEGER;
BEGIN

  select
   COUNT(EMPNO)
  into
   CNT
  from
   EMP
  where
   ENAME like IN_ENAME || '%'
  ;

  RETURN CNT;
END;
/
```

## ①直接呼出し

まずはストアド・プロシージャの名前を指定して直接呼び出してみましょう（リスト8-2）。

リスト8-2 直接呼び出し（Program.csのMainメソッドより）

```csharp
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
```

### (1) 対象ストアド・プロシージャ指定

まずDbCommandオブジェクトのCommandTextプロパティに、呼び出したいストアド・プロシージャ名を設定します。このとき、引数等は指定せず、あくまで名前だけであることに注意してください。

その後、CommandTypeプロパティにストアド・プロシージャを表すSystem.Data.CommandType列挙型のStoredProcedure値を設定します。

### (2) 戻り値用パラメーター設定

次に、呼び出すストアド・プロシージャの戻り値を受け取るため、戻り値専用のパラメーターを追加します。戻り値専用パラメーターは、DirectionプロパティにSystem.Data.ParameterDirection列挙型のReturnValue値を設定します。

戻り値専用パラメーターは必ず一番最初に追加してやらないといけないことに注意してください。ただし、呼び出すストアド・プロシージャがファンクションではなくプロシージャのように戻り値の無いものであれば、パラメーター追加は不要です。

### (3) 引数用パラメーター設定

今度はストアド・プロシージャの引数に対応するパラメーターを追加していきます。

サンプルコードでは入力引数しか使っていませんが、出力引数がある場合、DirectionプロパティにParameterDirection.Output値を設定すればよいです。

### (4) ストアド・プロシージャ実行

準備が整ったところで、DbCommandオブジェクトのExecuteNonQueryメソッドを呼び出して、ストアド・プロシージャを実行します。このときExecuteNonQueryメソッドの戻り値は必ず-1になるので、取得する必要はありません。

### (5) 戻り値取得

最後に、戻り値があれば(2)で追加した戻り値専用パラメーターオブジェクトのValueプロパティから、戻り値を取得します。当然object型になっているので、必要な型に変換、キャストしてください。

なお、ストアド・プロシージャの出力引数や戻り値がカーソル型の場合、ODP.NET固有の機能を使うことで対応は可能です。詳しくは、以下のページを参照してください。

※参考：[ODP.NETファーストステップ（1）：ODP.NETでOracle固有の機能を活用する (4/4) - ＠IT](http://www.atmarkit.co.jp/ait/articles/0411/27/news013_4.html)


[→第9章 例外処理](09-handle-exception.md)
