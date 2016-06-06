第7章 パラメーターの利用
=====

[↑目次](..\README.md "目次")

[←第6章 非問い合わせ](06-execute-non-query.md)

ここまでの章で基本的なCRUD処理を行うSQL実行が出来るようになりました。ただ、実際のアプリケーションでは、画面で入力した条件を使って、同じSQLを条件だけ変えて何度も実行するというケースが良くあります。本章ではこういったケースで使える「パラメーター」の使い方を学びます。

## 文字列連結での実行

まず、パラメーターを使わないケースを考えてみましょう。単純に考えれば、指定された値を元にWHERE句を文字列連結で組み立てることになるでしょう（リスト7-1）。

リスト7-1 パラメーター未使用（Program.csのMainメソッドより）

```csharp
Console.Write("検索したい社員名の先頭を入力してください : ");
var ename = Console.ReadLine();

// ①文字列連結
using (var dbCommand = dbConnection.CreateCommand())
{
  dbCommand.CommandText = $@"
    select
     COUNT(EMPNO)
    from
     EMP
    where
     ENAME like '{ename}%'
  ";

  var employeeCount = Convert.ToInt32(dbCommand.ExecuteScalar());
  Console.WriteLine($"該当社員数 : {employeeCount}");
}
```

ここでは例として、Console.ReadLineメソッドを使い、コンソールの標準入力から検索条件の社員名を取得して使っています。例えば、"J"を指定して実行すると、該当データが2件になります。

```
検索したい社員名の先頭を入力してください : J
該当社員数 : 2
```

## 文字列連結の問題点

文字列連結でも対象データ件数は正しく取得できていますし、何も問題無いように思えるかもしれません。しかし、実は重大な問題があります。それは「SQLインジェクション」と呼ばれるセキュリティ上の脆弱性です。

例えば、文字列連結のプログラムを実行したとき、社員名の先頭文字に`' OR 1=1 --`と入力して実行すると、なんと全件数が表示されてしまいました。

```
検索したい社員名の先頭を入力してください : ' OR 1=1 --
該当社員数 : 14
```

どうしてこのようなことになるのでしょう？それはSQL文を実際に組み立ててみればわかります。まず、文字列を埋め込む前のSQLは次のようになっています。

```
select
 COUNT(EMPNO)
from
 EMP
where
 ENAME like '%'
```

ここに先ほどの`' OR 1=1 --`を埋め込んでみるとどうなるでしょう？

```
select
 COUNT(EMPNO)
from
 EMP
where
 ENAME like '' OR 1=1 --%'
```

すると、like演算子で指定した条件と1=1という条件がOR演算子で演算され、その結果必ずTRUE担ってしまいます。したがって、いつでもTRUE＝全件が該当と判断されてしまいます。

この例では単に常にTRUEになるように条件を変更しただけですが、`';DROP TABLE EMP;--`が次のように埋め込まれたら、ユーザーの操作でテーブルを削除できてしまいます。

```
select
 COUNT(EMPNO)
from
 EMP
where
 ENAME like '';DROP TABLE EMP;--%'
```

この他、任意のユーザーの情報を始めといった機密情報を抜き取ったりと、SQLで出来ることは何でも好きに実行できてしまうのです。

こういったSQL文を注入（インジェクション）出来てしまう脆弱性を「SQLインジェクション」と呼びます。仕組みはシンプルですが、非常に怖い脆弱性で、過去には大きなシステム障害の原因にもなっています。

参考：[2014年を揺るがしたインジェクション攻撃の手口と対策 - IT、IT製品の情報なら【キーマンズネット】](http://www.keyman.or.jp/at/30007393/)

このSQLインジェクションの脆弱性を防ぐのに最も有効な手段が、「パラメーター」の利用です。


## パラメーターの利用

パラメーターとは、SQLの一部に予め専用の「プレースホルダー」を埋め込んでおき、実際の値をSQL実行時に外から設定するための仕組みです。Oracle Databaseでは「バインド変数」と呼ばれる機能が該当します。

パラメーターを使うと、SQLインジェクションに使うような文字列も、「その文字列自体」の値として比較されるようになります。今回の例では、ENAME like ''' OR 1=1--%'のように内部的に判断されるということです。

パラメーターを使うには、DbParameterクラスを使います（リスト7-2）。

リスト7-2 パラメーター使用（Program.csのMainメソッドより）

```csharp
// ②パラメーター利用
using (var dbCommand = dbConnection.CreateCommand())
{
  // (1) パラメーター埋め込み
  dbCommand.CommandText = @"
    select
     COUNT(EMPNO)
    from
     EMP
    where
     ENAME like :ENAME || '%'
  ";

  // (2) パラメーター作成
  var enameParameter = dbCommand.CreateParameter();
  // (3) パラメーターの型指定
  enameParameter.DbType = System.Data.DbType.String;
  // (4) パラメーターの値設定
  enameParameter.Value = ename;

  // (5) パラメーターを追加
  dbCommand.Parameters.Add(enameParameter);

  var employeeCount = Convert.ToInt32(dbCommand.ExecuteScalar());
  Console.WriteLine($"該当社員数 : {employeeCount}");
}
```

### (1) パラメーター埋め込み

まず、SQL文の中にパラメーターを埋め込みます。この時の記法は対象のDB製品、データプロバイダーによってま:
ちまちですが、ODP.NETでは`:～`のように":"で始まるものを使います。これがSQL Serverであれば`@～`になったり、ODBCであれば`?`のように名前の無い記述になったりします。

### (2) パラメーター作成

次に埋め込んだパラメーターを設定するためのDbParameterオブジェクトを作成します。それには、DbCommandクラスのCreateParameterメソッドを使います。

### (3) パラメーターの型指定

今度はそのパラメーターの型をDbTypeプロパティに指定します。DbTypeプロパティはSystem.Data.DbType列挙型で、Int16～64などの整数型、Date、DateTimeなどの日時型、String、AnsiStringなどの文字列型があります。そのパラメーターと比較するDB側の型に合わせて指定することになりますが、単純な文字列型などであれば、型指定は自動で行われるので省略することも可能です。言い換えれば、省略しておいて実行時に問題があれば、明示的に型を指定するというやり方でも良いです。

### (4) パラメーターの値設定

パラメーターの値を設定します。今回は標準入力から指定された社員名を設定します。

### (5) パラメーターを追加

DbParameterオブジェクトの準備ができたら、DbCommandクラスのParametersプロパティにAddメソッドを使って追加します。ParametersプロパティはDbParameterCollection型で、他のコレクション系クラスと同様、AddRangeメソッドでまとめて複数のパラメーターを追加することも出来ます。


以上でパラメーターの利用準備が整いました。先ほどのようにSQLインジェクションの脆弱性を攻撃するような値を入れても、今度は該当データが無いと表示されます。

```
検索したい社員名の先頭を入力してください : ' OR 1=1--
該当社員数 : 0
```

この例ではSELECT句でパラメーターを利用していましたが、当然INSERT/UPDATE/DELETE等のDMLでもパラメーターは利用できます。

[→第8章 ストアド・プロシージャ](08-call-stored-procedure.md)
