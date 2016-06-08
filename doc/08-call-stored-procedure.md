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


## ②無名ブロック使用

ストアド・プロシージャ呼び出しにはもう一つ方法があります。それは、PL/SQLの無名ブロックを使うことです（リスト8-3）。

リスト8-3 無名ブロック使用（Program.csのMainメソッドより）

```csharp
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
```

### (1) PL/SQL無名ブロック定義

DbCommandオブジェクトのCommandTextプロパティに、ストアド・プロシージャ呼び出しを記載したPL/SQL無名ブロックを設定します。このとき、戻り値と引数はパラメーターとして定義することに注意してください。

また、CommandTypeプロパティは既定のまま（CommandType.Text値）にします。

### (2) 戻り値用出力パラメーター設定

次に、戻り値を受け取るためのパラメーターを、出力パラメーターとして追加します。無名ブロック内での定義順の通り、一番最初に追加してやりましょう。

### (3) 引数用パラメーター設定

直接呼び出しと同じように、ストアド・プロシージャの引数に対応するパラメーターも追加します。

### (4) ストアド・プロシージャ実行

準備が整ったところで、DbCommandオブジェクトのExecuteNonQueryメソッドを呼び出して、ストアド・プロシージャを実行します。直接呼び出しと同様に、ExecuteNonQueryメソッドの戻り値は必ず-1になるので、取得する必要はありません。

### (5) 戻り値取得

最後に、戻り値があれば(2)で追加した戻り値用出力パラメーターオブジェクトのValueプロパティから、戻り値を取得します。


直接呼び出し、無名ブロックどちらの方法でもストアド・プロシージャを実行することが出来ますが、ストアド・プロシージャ名を外部で管理しているような場合は、直接呼び出しを行ったほうが良いでしょう。


これで、ストアド・プロシージャ実行も出来るようになりました。これで一通りのDBアクセスが出来るようになったところで、次の章ではトランザクションの扱いについて学びましょう。

[→第9章 トランザクション管理](09-manage-transaction.md)
