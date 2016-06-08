using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data.Common;

namespace HandleException
{
  class Program
  {
    enum RegisterEmployeeResult
    {
      Success,
      Duplication
    }

    static void Main(string[] args)
    {
      {
        var result = RegisterEmployee(
          empno: 7369,
          ename: "SMITH",
          job: "CLERK",
          mgr: 7902,
          hireDate: new DateTime(1980, 12, 17),
          sal: 800d,
          comm: DBNull.Value,
          deptno: 20
          );
        Console.WriteLine(result);
      }
    }

    private static RegisterEmployeeResult RegisterEmployee(int empno, string ename, string job, object mgr, DateTime hireDate, double sal, object comm, object deptno)
    {
      var connectionStringSettings = ConfigurationManager.ConnectionStrings["SCOTT"];

      var dbProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);

      using (var dbConnection = dbProviderFactory.CreateConnection())
      {
        dbConnection.ConnectionString = connectionStringSettings.ConnectionString;

        dbConnection.Open();

        using (var dbTransaction = dbConnection.BeginTransaction())
        {
          try
          {
            // データを更新する
            using (var dbCommand = dbConnection.CreateCommand())
            {
              dbCommand.CommandText = $@"
                insert into EMP
                (
                   EMPNO
                  ,ENAME
                  ,JOB
                  ,MGR
                  ,HIREDATE
                  ,SAL
                  ,COMM
                  ,DEPTNO
                )
                values
                (
                   :EMPNO
                  ,:ENAME
                  ,:JOB
                  ,:MGR
                  ,:HIREDATE
                  ,:SAL
                  ,:COMM
                  ,:DEPTNO
                )
              ";

              dbCommand.Transaction = dbTransaction;

              {
                var param = dbCommand.CreateParameter();
                param.Value = empno;
                dbCommand.Parameters.Add(param);
              }
              {
                var param = dbCommand.CreateParameter();
                param.Value = ename;
                dbCommand.Parameters.Add(param);
              }
              {
                var param = dbCommand.CreateParameter();
                param.Value = job;
                dbCommand.Parameters.Add(param);
              }
              {
                var param = dbCommand.CreateParameter();
                param.Value = mgr;
                dbCommand.Parameters.Add(param);
              }
              {
                var param = dbCommand.CreateParameter();
                param.Value = hireDate;
                dbCommand.Parameters.Add(param);
              }
              {
                var param = dbCommand.CreateParameter();
                param.Value = sal;
                dbCommand.Parameters.Add(param);
              }
              {
                var param = dbCommand.CreateParameter();
                param.Value = DBNull.Value;
                dbCommand.Parameters.Add(param);
              }
              {
                var param = dbCommand.CreateParameter();
                param.Value = deptno;
                dbCommand.Parameters.Add(param);
              }

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
              catch (DbException dbEx)
              {
                if (dbEx.GetErrorType() == DbExceptionType.Duplication)
                {
                  return RegisterEmployeeResult.Duplication;
                }

                throw;
              }
            }

            dbTransaction.Commit();
          }
          catch
          {
            dbTransaction.Rollback();
            throw;
          }
        }

        dbConnection.Close();

        return RegisterEmployeeResult.Success;
      }
    }
  }

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
  }
}
