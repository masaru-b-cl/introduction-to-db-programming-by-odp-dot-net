第10章 例外処理
=====

[↑目次](..\README.md "目次")

[←第9章 トランザクション管理](09-manage-transaction.md)

DBアクセスを行うアプリケーションではDB側で発生したエラーの処理も考えなければなりません。DBエラーを処理するためのDbException例外クラスの使い方を学んでいきましょう。

## DbException例外クラス

.NET データプロバイダーでは、DB型で発生したエラーはDbException例外クラスで表現されます。ただ、対象DB製品ごとに扱うエラーの情報がまちまちであるため、実際に扱うのはその製品ごとの派生クラスになります（リスト10-1）。

リスト10-1 DB例外処理（Program.csのRegisterEmployeeメソッドより）

```csharp
// (1) DB例外用try-catchブロック
try
{
  dbCommand.ExecuteNonQuery();
}
catch (OracleException oraEx) // (2) DB例外キャッチ
{
  // (3) DB例外判定
  if (oraEx.Number == 1)
  {
    // DUP_VAL_ON_INDEX：一意制約違反
    dbTransaction.Rollback();
    // (4) 業務エラーとして戻す
    return RegisterEmployeeResult.Duplication;
  }

  throw;
}
```

### (1) DB例外用try-catchブロック

まず大前提として、DBエラーにはディスク容量がいっぱいになった、そもそもDBに接続できないといった、処理の継続ができないクリティカルなものと、登録しようとしたデータが既に登録されていた、更新対象データがロックされていた、といった処理をやり直すことのできるものが両方含まれています。そのため、DB例外ではその切り分けを行う必要があります。

ただし、こういった処理は実行するSQL毎に判断すべきものであり、いくつもSQLを実行するような処理の全体で統一した処理にはなりません。

そのため、まずSQL実行処理ごとにtry-catchブロックで囲う必要があるのです。

### (2) DB例外キャッチ

次にDB例外のキャッチを行いますが、個々ではDB製品ごとの.NET データ プロバイダーに含まれる、DbExceptionを派生した具体的なDB例外型でキャッチします。ODP.NETの場合、Oracle.ManagedDataAccess.Client.OracleException型でキャッチすることになります。

では、なぜDbException型でなくその派生型でキャッチするかというと、対象DB製品で発生したエラーの詳細情報は、DbException型からは取得することが出来ないためです。

なお、Exception型でキャッチするとDB例外以外も捕捉してしまうので、絶対にやらないようにしてください。

### (3) DB例外判定

DB例外の具象型で例外をキャッチしたら、今度は発生したDB例外がどのようなものであるのか判定します。例えばOracleException型にはNumberプロパティがあり、発生したOracleエラーの番号を取得できます。コード例では番号1のDUP_VAL_ON_INDEXかどうかを判定しています。

このNumberプロパティの値により、やり直し可能と判断したら、必要ならロールバックし、業務ロジックの戻り値として一意制約違反があったことを、呼び出し元に戻します。

そして、やり直し不可能なDB例外については、その後のthrow;により再スローし、他の実行時例外と同じ流れで処理されます。

以上がDB例外の基本的な処理の流れです。ポイントは、DB例外を局所的に具体的な例外型で処理するということです。以上のことを守ってさえいれば、データ プロバイダーの各種オブジェクトの後始末等に影響を与えずに、特定のDBエラーのみ処理できるようになります。


なお、これまで具体的なデータ プロバイダーの派生型が登場してこなかったのに、ここだけはそれを扱わざるをえないところに若干の抵抗はあります。これを緩和するなら、DbExceptionを引数に取り想定する各種やり直し可能なエラー（一意制約違反、ロック中、タイムアウト、など）かどうかを判定する、ユーティリティクラス、メソッドを作って、具体的な型はその中だけで扱うようにするといった方法があります（リスト10-2、3）。

リスト10-2 DB例外処理ユーティリティ（Program.csより）

```csharp
enum DbExceptionType
{
  Duplication,
  Unknown
}

static class DbExceptionExtensions
{
  public static DbExceptionType GetErrorType(this DbException dbException)
  {
    if (dbException == null) throw new ArgumentNullException(nameof(dbException));

    var oracleException = dbException as Oracle.ManagedDataAccess.Client.OracleException;

    if (oracleException == null) return DbExceptionType.Unknown;

    switch (oracleException.Number)
    {
      case 1:
        return DbExceptionType.Duplication;

      default:
        return DbExceptionType.Unknown;
    }
  }
```

リスト10-3 DB例外処理ユーティリティ使用例（Program.csのRegisterEmployeeメソッドより）

```csharp
catch (DbException dbEx)
{
  if (dbEx.GetErrorType() == DbExceptionType.Duplication)
  {
    return RegisterEmployeeResult.Duplication;
  }

  throw;
}
```


これでデータ プロバイダーをそのまま使って、ほぼ全てのDBアクセス処理が行えるようになりました。最後に次の章では、煩雑なSQL実行処理をサポートしてくれる、O/R マッパーについて紹介します。

[→第11章 O/Rマッパーの利用](11-or-mapper.md)
