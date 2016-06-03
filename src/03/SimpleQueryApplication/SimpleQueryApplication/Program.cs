// (1) 名前空間のインポート
using System.Configuration;
using System.Data.Common;

namespace SimpleQueryApplication
{
  class Program
  {
    static void Main(string[] args)
    {
      // (2) DB接続文字列情報の取得
      var connectionStringSettings = ConfigurationManager.ConnectionStrings["SCOTT"];

      // (3) DbProficerFactoryインスタンスの生成、取得
      var dbProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);

      // (4) DB接続オブジェクトを作成
      using (var dbConnection = dbProviderFactory.CreateConnection())
      {
        // (5) DB接続文字列の設定
        dbConnection.ConnectionString = connectionStringSettings.ConnectionString;

        // (6) DB接続を開く
        dbConnection.Open();

        // TODO : SQL実行処理をここに書く

        // (7) DB接続を閉じる
        dbConnection.Close();
      }
    }
  }
}
