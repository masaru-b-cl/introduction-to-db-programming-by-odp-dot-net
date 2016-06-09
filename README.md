DBプログラミング入門 by ODP.NET
=====

## はじめに

本文書はリレーショナルデータベースシステム（以下単にDB）アクセスを行うアプリケーションを作成するために、どのようにプログラミングすればよいかを身につけることを目的としています。対象DBはOracle Database、プラットフォームは.NETで言語はC#を用います。

本書では最も基本的なADO.NETのデータプロバイダーを用いたデータアクセスを対象とし、DataSetやTableAdapterを用いた非接続型アクセス、Entity Frameworkは対象外とします。

### 想定する読者

- C#を使った基本的なプログラミングが行える
- RDBMSの基本的な知識（DDL、DML等のSQL、トランザクション制御など）を理解している

### 本文書のゴール

- アプリケーションからDBへアクセスする際の、基本的な概念を理解する
- CRUD操作を行うSQLが発行できる
- SQLインジェクション脆弱性を排除するために、パラメーターを利用したコマンド発行ができる
- データプロバイダーの構成を理解し、適切に後処理が出来る
- DBアクセスに伴う例外処理が行える

### 本文書の開発環境

- Windows 10 Pro 64bit
- Visual Studio 2015 Community
- Oracle Database 11g Release 2

## 目次

1. [2つの世界](doc/01-two-worlds.md)  
  まずはプログラムからDBにアクセスするというｎはどういうことなのか、概念を理解していきましょう。
2. [データ プロバイダー](doc/02-data-provider.md)  
  .NET Frameworkから世界をまたいでDBにアクセスするために必要なデータ プロバイダーの基本を学びましょう。
3. [DBに接続する](doc/03-connect-to-db.md)  
  プログラムからDBに接続する手順について学びましょう。
4. [単純な問い合わせ](doc/04-execute-query.md)  
  最も基本的なSELECT文を実行する手順について学びましょう。
5. [スカラー値の取得](doc/05-get-scalar-value.md)  
  数量、件数と行った単一のスカラー値を取得する手順について学びましょう。
6. [非問い合わせ](doc/06-execute-non-query.md)  
  UPDATE、INSERT、DELETEといった非問い合わせSQL文を実行する手順について学びましょう。
7. [パラメーターの利用](doc/07-use-parameter.md)  
  パラメーターを用いたSQL文を実行する手順について学びましょう。
8. [ストアド・プロシージャ](doc/08-call-stored-procedure.md)  
  ストアド・プロシージャを呼び出す手順について学びましょう。
9. [トランザクション管理](doc/09-manage-transaction.md)  
  DBアクセス時のトランザクションを管理する方法について学びましょう。
10. [例外処理](doc/10-handle-exception.md)  
  DBアクセスに伴う例外処理について学びましょう。
11. [O/Rマッパーの利用](doc/11-or-mapper.md)  
  SQL実行処理の煩雑さを緩和してくれる、O/Rマッパーの使い方を学びましょう。

## サンプルについて

各章のサンプルコードは、`src`フォルダーの下にある章番号フォルダーの中にあります。リポジトリ全体をzipファイル等でダウンロードして展開後、Visual Studioでソリューションを開いて内容を確認してください。

## 参考資料

- [.NET Framework データ プロバイダー](https://msdn.microsoft.com/ja-jp/library/a6cd7c08.aspx)
- [OTN Japan - 初心者向け講座 : はじめてのODP.NETアプリケーション開発](http://otn.oracle.co.jp/beginner/odpnet/)
- [Insider.NET > ADO.NET基礎講座 - ＠IT](http://www.atmarkit.co.jp/ait/subtop/features/dotnet/adonet_index.html)
- [.NETの例外処理 Part.2 | とあるコンサルタントのつぶやき](https://blogs.msdn.microsoft.com/nakama/2009/01/02/net-part-2/)
- [ADO.NETとORMとMicro-ORM －dapper dot netを使ってみた](http://www.slideshare.net/kiyokura/adonetormmicro-orm-dapper-dot-net)
