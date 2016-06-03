第3章 DBに接続する
=====

[↑目次](..\README.md "目次")

[←第2章 データ プロバイダー](02-data-provider.md)

データ プロバイダーの構成やDBアクセスの手順の大枠がわかったところで、今度は具体的な内容に入っていきましょう。まずはDBに接続する方法からです。なお、本文書ではローカルPCにOracle Databaseがインストールされている前提で以後の説明を進めます。

## ODP.NET

Oracl Database向けのデータプロバイダーは、実は.NET FrameworkにもSystem.Data.OracleClient名前空間として含まれています。ただし、現在は非推奨になっていること、7i、8i、9iなど過去のバージョン向けであることから、使わないようにしてください。

その代わり、Oracle社から提供されている「ODP.NET」を用います。ODP.NETはOracle向けODBCドライバー等の各種データアクセス用コンポーネント群をまとめた「[Oracle Data Access Components（ODAC）](64ビットのOracle Data Access Components（ODAC）のダウンロード http://www.oracle.com/technetwork/jp/database/windows/downloads/index-214820-ja.html)」に含まれる、Oracle専用データプロバイダーです。

ODP.NETを使用するには、上記ODACをマシンにインストールする方法に加え、NuGet（※）によるインストールにも対応しています。今回は後者の方法でインストールします。

※NuGet
.NETアプリケーション開発のためのライブラリ配布、管理を行う「パッケージ マネージャー」である。多くのライブラリ等が「[NuGet Gallery](https://www.nuget.org/)」で公開されている。


## ODP.NETのインストール

まずdb接続を行うサンプルアプリケーションを、コンソールアプリケーションとして作成します（図3-1）。

![プロジェクト作成](../image/03-01.jpg)

図3-1 プロジェクト作成

次にNuGetパッケージをインストールするため、「ツール」メニュー→「NuGet パッケージ マネージャー」→「ソリューションの NuGet パッケージの管理」を選択します（図3-2）。

![NuGet パッケージの管理](../image/03-02.jpg)

図3-2 NuGet パッケージの管理

「ソリューションのパッケージの管理」画面が表示されたら、「参照」タブを選び、テキストボックスに"oracle.manageddataaccess"と入力し、ODP.NETを検索します（図3-3）。

![ODP.NETの検索](../image/03-03.jpg)

図3-3 ODP.NETの検索

「Oracle.ManagedDataAcess」を選択後、インストール先プロジェクトである「SimpleQueryApplication」にチェックを入れ、「インストール」ボタンをクリックします（図3-4）。

![ODP.NETのインストール](../image/03-04.jpg)

図3-4 ODP.NETのインストール

「プレビュー」ダイアログが表示されたら、「OK」ボタンをクリックします（図3-5）。

![インストールプレビューダイアログ](../image/03-05.jpg)

図3-5 インストールプレビューダイアログ

「ライセンスへの同意」ダイアログが表示されたら、ライセンスを確認して「同意する」ボタンをクリックします（図3-6）。

![ライセンスへの同意ダイアログ](../image/03-06.jpg)

図3-6 ライセンスへの同意ダイアログ

ODP.NETのインストールが行われます。インストールした結果、プロジェクトにOracle.ManagedDataAcess.dllファイルへのライブラリ参照が追加されるとともに、アプリケーション構成ファイル（App.config）にもODP.NETを使うための記述が追加されます（）。

![パッケージインストール結果](../image/03-07.jpg)

図3-7 パッケージインストール結果

なお、ODACをインストールした場合は、プロジェクトのプロパティより「アセンブリ」→「拡張」欄に表示される「Oracle.ManagedDataAccess」への参照を追加すればよいです。

## DBアクセス準備

ODP.NETのインストールが終わったので、今度は実際に接続するための準備を進めていきます。まず、ODP.NETに含まれるDbProviderFactory型派生クラスのインスタンスを取得するため、アプリケーション構成ファイルの設定を行います（リスト3-1）。

リスト3-1 アプリケーション構成ファイル設定（App.configより）

```csharp
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <!-- (1) -->
  <configSections>
    <section name="oracle.manageddataaccess.client" type="OracleInternal.Common.ODPMSectionHandler, Oracle.ManagedDataAccess, Version=4.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342"/>
  </configSections>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1"/>
  </startup>
  <!-- (2) -->
  <system.data>
    <DbProviderFactories>
      <remove invariant="Oracle.ManagedDataAccess.Client"/>
      <add name="ODP.NET, Managed Driver" invariant="Oracle.ManagedDataAccess.Client" description="Oracle Data Provider for .NET, Managed Driver" type="Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Version=4.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342"/>
    </DbProviderFactories>
  </system.data>
  <!-- (3) -->
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <publisherPolicy apply="no"/>
        <assemblyIdentity name="Oracle.ManagedDataAccess" publicKeyToken="89b483f429c47342" culture="neutral"/>
        <bindingRedirect oldVersion="4.121.0.0 - 4.65535.65535.65535" newVersion="4.121.2.0"/>
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
  <!-- (4) -->
  <oracle.manageddataaccess.client>
    <version number="*">
      <dataSources>
        <!-- (5) -->
        <dataSource alias="ORCL"
          descriptor="(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=hostname)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL))) "/>
      </dataSources>
    </version>
  </oracle.manageddataaccess.client>
  <!-- (6) -->
  <connectionStrings>
    <add name="SCOTT"
      providerName="Oracle.ManagedDataAccess.Client"
      connectionString="Data Source=ORCL;User Id=scott;Password=tiger;" />
  </connectionStrings>
</configuration>
```

(1)から(4)については先ほどNuGetでODP.NETをインストールした時に自動で追加された部分です。ODACをインストールした場合は、(2)と(3)はマシンで共通の構成ファイル（machine.config）に追加されるので、自分でApp.configに書く必要はありません。

ポイントは(5)と(6)です。

### (5) データソース設定

(4)に含まれるoracle.manageddataaccess > version > dataSources > dataSource要素が接続先のOracleサーバー情報を記載する箇所です。alias属性には(6)で使用するデータソース名を指定し、descripter要素に実際の接続情報を指定します。記載内容はtnsnames.oraファイルに書くものと全く同じです。なお、HOST項目は既定では"localhost"が設定されていますが、ローカルにOracle Databaseがインストールされているときは自らのマシン名を指定するようにしてください。．

### (6) 接続文字列設定

次にプログラム無いから(5)で指定したデータソースに接続するための「データベース接続文字列（以後単に接続文字列）」情報を追加します。name属性にはプログラム内からこの接続文字列を取得する際に使用する名前、providerName属性には(2)のsystem.data > DbProviderFactories > add要素のinvariant属性で指定した名前を設定します。そして最も大事なconnectionString属性には、接続に使用するデータソース名、ユーザー、パスワードを最低限指定します。接続文字列の記法に就いてより詳しい内容は、次のURLを参照してください。

[Oracle Data Provider for .NET / ODP.NET Connection Strings - ConnectionStrings.com](http://www.connectionstrings.com/oracle-data-provider-for-net-odp-net/)


## 必要ライブラリ参照

次にDBアクセスに必要なライブラリ参照を追加します。次のライブラリへの参照をプロジェクトに追加します（図3-8）。

| ライブラリ名         | 説明                                                     |
|----------------------|----------------------------------------------------------|
| System.Data          | データ プロバイダー本体                                  |
| System.Configuration | 接続文字列情報をアプリケーション構成ファイルから読み込む |

表3-1 DBアクセスに必要なライブラリ

![ライブラリ参照状況](../image/03-08.jpg)

図3-8 ライブラリ参照状況



[→第4章 単純な問い合わせ](04-execute-query.md)
