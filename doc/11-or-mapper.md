第11章 O/Rマッパーの利用
=====

[↑目次](..\README.md "目次")

[←第10章 例外処理](10-handle-exception.md)

これまで第3章から第9章までで見てきたように、DbCommandやDbDataReaderを使ったDBアクセスは操作が煩雑ですし、型やDBNullの考慮も必要で、少々面倒なことは否めません。こういった面倒さを軽減するため、昨今ではO/Rマッパー（Object-Relational Mapper）と呼ばれる、サポートライブラリを使うことも良くあります。今回はその中でも、「軽量」なO/Rマッパーの1つである「Dapper」の使い方を学びましょう。

## O/Rマッピング

アプリケーションからDBアクセスする際、目的の項目の値を取得するには、その名前であくせすし、DBNullを考慮しつつ目的の型に変換するとうことを、必要な項目全部に行わないといけませんでした。しかし、こういった処理は実際はほとんど同じような処理の繰り返しになってしまい、全部のコードを手入力するのは割に合いません。

そこで、DBの項目とアプリケーションで使う項目を、自動的に関連付けて値のやり取りができたら良いと思いませんか？そういったことを請け負ってくれるのが、「O/Rマッパー（Object-Relational Mapper）」です。

O/Rマッパーはその名の通り、アプリケーション側のオブジェクトとRDBMSのテーブルの項目をマッピングし、DBNullや型も考慮して自動的に値をやり取りできる仕組みです。大きなものではSQL自体をアプリケーション側からは完全に排除するような物もありますし、単にSQLの実行部分に絞った物もあります。

例えば、ADO.NETに含まれる「[Entity Framework](https://msdn.microsoft.com/ja-jp/library/bb399567.aspx)」は、アプリケーションコードからSQLは見えずに、完全にオブジェクト操作をRDBMSに反映させるような仕組みになっています。したがって、従来のDBアクセスの知識とはまた別の概念を新たに学ばないと、効果的に使うことは出来ません。

それに対して、SQL実行部分に絞ったものが、今回紹介する「[Dapper](https://github.com/StackExchange/dapper-dot-net)」です。DapperはQ&Aサイトである「Stack Overflow」を運営しているStack Exchangeが開発している、オープンソースのO/Rマッパーで、SQL実行時のパラメータ設定や問い合わせ時のデータ取得等を楽にしてくれるライブラリです。

## Dapperのインストール

Dapperをアプリケーションで使うには、NuGetを通じてライブラリをインストールするだけです。VSの「ツール」メニュー→「NuGet パッケージ マネージャー」→「パッケージ マネージャー コンソール」をクリックしてください（図11-1）。

![パッケージ マネージャー コンソールの起動](../image/11-01.jpg)

図11-1 パッケージ マネージャー コンソールの起動

次に「パッケージ マネージャー コンソール」で以下のコマンドを実行して、Dapperをインストールします（図11-2）。

```
PM> Install-Package Dapper
```

![Dapperインストール](../image/11-02.jpg)

図11-2 Dapperインストール

最後に、使用するソースコードの先頭に、usingを使ってDapper名前空間をインポートします。

```csharp
using Dapper;

```

## マッピング型定義

最初に行うのは、「マッピング」するための型です（リスト11-1）。

リスト11-1 マッピング型定義（Program.csより）

```csharp
  // マッピング型定義 
  class Employee
  {
    public int EMPNO { get; set; }
    public string ENAME { get; set; }
    public string JOB { get; set; }
    public int? MGR { get; set; }
    public DateTime HIREDATE { get; set; }
    public decimal SAL { get; set; }
    public decimal? COMM { get; set; }
    public int DEPTNO { get; set; }
  }
```

マッピング型にはSQLに記載する列名、項目名と同じ名前のプロパティを作成します。この時、DB上でNULLになる可能性がある値型の項目は、null許容型として定義するのがポイントです。


