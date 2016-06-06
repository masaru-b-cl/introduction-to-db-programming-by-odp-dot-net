第4章 単純な問い合わせ
=====

[↑目次](..\README.md "目次")

[←第3章 DBに接続する](03-connect-to-db.md)

DB接続できたところで、まずは単純なSELECT文の実行（問い合わせ）を行ってみましょう。

## 問い合わせ実行方法

SELECT文の実行はDbCommand、DbDataReaderという2つのクラスを用いて行います（リスト4-1）。

リスト4-1 SELECT文実行処理（Program.csのMainメソッドより）

```csharp
using (var dbConnection = dbProviderFactory.CreateConnection())
{
  dbConnection.ConnectionString = connectionStringSettings.ConnectionString;

  dbConnection.Open();

  // ①DbCommandオブジェクト作成
  using (var dbCommand = dbConnection.CreateCommand())
  {
    // ②実行SQL文設定
    dbCommand.CommandText = @"
      select
       EMPNO
      ,ENAME
      ,SAL
      ,COMM
      from
       EMP
    ";

    // ③問い合わせ実行
    using (var dbDataReader = dbCommand.ExecuteReader())
    {
      // ④DbDataReader読み込み
      while (dbDataReader.Read())
      {
        // ⑤レコードデータ取得
        // (1) nullでないことが確定している整数を列インデックスで取得
        var empno = dbDataReader.GetInt32(0);
        // (2) 文字列を列名で取得
        var ename = dbDataReader["ENAME"] as string;
        // (3) nullでないことが確定している小数を列名で取得
        var sal = Convert.ToDecimal(dbDataReader["SAL"]);
        // (4) nullがあるかもしれない小数を列名で取得
        var comm = dbDataReader["COMM"] is DBNull ? (decimal?)null
                                                  : Convert.ToDecimal(dbDataReader["COMM"]);
        // 取得データを表示
        Console.WriteLine($"{empno}\t{ename}\t{sal,6:#,##0}\t{comm,6:#,##0}");
      }

      // ⑥DbDataReaderを閉じる
      dbDataReader.Close();
    }
  }

  dbConnection.Close();
}
```

### ①DbCommandオブジェクト作成

まず最初にSQL実行用のDbCommandオブジェクトをDbConnectionクラスのCreateCommandメソッドを使って作成します。DbCommandクラスは他のクラス同様IDisposableインターフェイスを実装していますので、using文を使って確実にDisposeメソッドが呼ばれるようにしてください。

### ②実行SQL文設定

次に実行したいSELECT文を書いたSQLをDbCommandオブジェクトに設定します。この時、逐語的リテラル文字列（@"..."）を使って自由に改行、空白を含んだ形でSQLを書くと良いでしょう。また、例えば次のような基準で、メンテナンスしやすい形でSQLを書いてやるのも良いでしょう。

- SQLキーワードは小文字
- 列名は大文字
- 1行に1キーワード
- 1行に1項目
- ","（カンマ）は前置
    - 最後に項目を追加する際、ひとつ前の行の末尾に","を付け足す必要が無いため

### ③問い合わせ実行

SQL実行準備が整ったところで、SQLを実行します。SELECT文のようなデータ読み取りには、DbCommandクラスのExecuteReaderメソッドを呼び出します。このメソッドの戻り値はDbDataReader型で、取得したDbDataReaderオブジェクトを通してDBの「カーソル」を制御することが出来ます。つまり、カーソルのオープンが行われているのです。
なお、DbDataReader型もIDisposableインターフェイスを実装しているので、using文を使うことを忘れないで下さい。

### ④DbDataReader読み込み

SELECT文を実行したので、次にデータの読み込みを行うため、DbDataReaderクラスのReadメソッドを呼び出します。このメソッドの戻り値はbool型で、実行すると内部のカーソルに対してフェッチを行い、現在位置を進めてデータがあればtrue、なければfalseを返します。この戻り値をwhile文で判定することで、最後のデータまで繰り返し読みこむことが出来ます。

なお、該当データがなければ初回のReadメソッドの時点で戻り値がfalseとなるため、whileループ内の処理は行われません。

### ⑤レコードデータ取得

データがあれば、今度はレコードより各列のデータを取得します。

#### (1) 型を指定して取得

対象列にNot Null制約が指定され、nullでないことが確定していれば、型を指定して値を取り出します。コード例ではGetInt32メソッドを使い、int（System.Int32）型として取得しています。他にもGetInt16、GetString、GetDateTime等ありますが、「列名」ではなくSELECT文に記載した順での「列インデックス」でアクセスしないといけないことに注意が必要です。

#### (2) 列名を指定して取得

「列名」を指定して値を取得するには、DbDataReaderクラスの文字列を受け取るインデクサーを使います。ただし、戻り値はobject型になりますので、実際に使う前に目的の型にキャストしてやる必要があります。

コード例のように文字列型であれば、as演算子を使うことを勧めます。これは、nullならnull、そうでなければ文字列として取得できるためです。

#### (3) 型を指定して列名で取得

列名を使って型を指定して値を取得したい場合は、Convert.To(型名)メソッドを使うと良いでしょう。インデクサーから取得される値はDB側に合わせた方となっているため、桁数の少ない数値だとshort型になっており、単に(int)のようなキャスト演算子を使うと、InvalidCastExceptionが発生してしまうためです。

もちろん

```csharp
var value = (int)(short)dbDataReader["列名"];
```

の用に2重キャストを行うという方法もありますが、わざわざDB側の型を調べて、合わせて一つ目のキャストを行う必要があり、面倒ですしバグが混ざる可能性が高くなります。

#### (4) nullを想定して取得する

最後に一番面倒なDB側のnullの扱いです。DBでnullだった場合、取得される値はDBNull.Valueになります。この値はDBNull型のただひとつの値であり、is演算子でDBNull型であるかどうかで判断できます。この挙動を使い、DB側でnullの場合とそうでない場合で、プログラム側で扱う値を切り分けます。

この処理は条件演算子を使うと良いでしょう。コード例ではDB側でnullならnull、そうでなければdecimal型としています。この時、2項目で(decimal?)型へのキャストを入れているのがミソです。こうすることで、戻り値が(decimal?)型と確定するため、次の3項目では単にConvert.ToDecimalメソッドでdecimal型に変換してやるだけでよくなります。

### ⑥DbDataReaderを閉じる

データの取得と処理を終えたら、DbDataReaderクラスのCloseメソッドを呼び出し、DbDataReaderオブジェクトを閉じます。こうすることで、内部で保持していたカーソルが閉じられます。このCloseメソッド呼び出しを忘れると、アプリケーション側でカーソルを開いたままとなるため、DB側で指定した上限カーソル数にすぐ達してしまいます。

なお、Disposeメソッド呼び出しでもCloseメソッド呼び出しが行われますので、前述のとおりusing文と併用すると、カーソルの閉じ忘れをなくすことが出来ます。


## 実アプリケーションでのDBアクセス

以上が単純なSELECT文による問い合わせの処理方法です。やりたいことは単純な割に、カーソル処理、取得データの型、nullの扱いなど、案外考慮すべきことが多くあり面倒に感じたかもしれません。

そのため、実際のアプリケーションではこういった面倒事は「Data Access Component（DAC）」や「Data Access Object（DAO）」といった専用のクラス群に押しこめ、アプリケーション側では意識しなくて良いように作る事が重要です。本章で紹介した処理が例えばGUIアプリケーションのボタンクリック時の処理に書いてあったとしたらどうでしょう？本来すべきこと以外のコード量が膨れ上がり、何をしたいのかがわからなくなってしまいます。

その他、DB側での型がどうなるか？等、対象のDB製品やデータ プロバイダーによってまちまちになることもあります。こういった変更や修正が、アプリケーション側に影響しないためにも、DACやDAOといった「データアクセスの責務」を持つクラス群は必要です。


単純な問い合わせのやり方が分かったので、次の章では単一行、単一項目である「スカラー値」を取得するSELECT文の実行方法について学びましょう。

[→第5章 スカラー値の取得](05-get-scalar-value.md)  
