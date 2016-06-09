using System;
using System.Configuration;
using System.Data.Common;

using Dapper;

namespace SimpleQueryApplication
{
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

  class Program
  {
    static void Main(string[] args)
    {
      var connectionStringSettings = ConfigurationManager.ConnectionStrings["SCOTT"];

      var dbProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);

      using (var dbConnection = dbProviderFactory.CreateConnection())
      {
        dbConnection.ConnectionString = connectionStringSettings.ConnectionString;

        dbConnection.Open();

        Console.Write("検索したい社員名の先頭を入力してください : ");
        var ename = Console.ReadLine();
        // ①問い合わせ
        // (1) SQL実行
        var employees = dbConnection.Query<Employee>(
          @"
            select
             EMPNO
            ,ENAME
            ,JOB
            ,MGR
            ,HIREDATE
            ,SAL
            ,COMM
            ,DEPTNO
            from
             EMP
            where
             ENAME like :ENAME || '%'
          ",
          new { ENAME = ename }
        );

        // (2) 取得データを列挙
        foreach (var employee in employees)
        {
          // (3) 取得データを表示
          Console.WriteLine(
            $"{employee.EMPNO}"
            + $"\t{employee.ENAME}"
            + $"\t{employee.JOB}"
            + $"\t{employee.MGR}"
            + $"\t{employee.HIREDATE:yyyy/MM/dd}"
            + $"\t{employee.SAL,6:#,##0}"
            + $"\t{employee.COMM,6:#,##0}"
            + $"\t{employee.DEPTNO}"
          );
        }

        // ②スカラー値取得
        // (1) SQL実行
        var employeeCount = dbConnection.ExecuteScalar<int>(
          @"
            select
             COUNT(EMPNO)
            from
             EMP
            where
             ENAME like :ENAME || '%'
          ",
          new { ENAME = ename }
        );

        // (2) 実行結果表示
        Console.WriteLine($"総社員数 : {employeeCount}");

        // ③非問い合わせ
        // (1) SQL実行
        var updatedCount = dbConnection.Execute(
          @"
            update EMP
            set
             ENAME = :ENAME
            where
             EMPNO = :EMPNO
          ",
          new Employee
          {
            ENAME = "SMITH",
            EMPNO = 7369
          }
        );

        // (2) 更新件数表示
        Console.WriteLine($"更新件数 : {updatedCount}");

        dbConnection.Close();
      }
    }
  }
}
