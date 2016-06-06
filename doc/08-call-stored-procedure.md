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




[→第9章 例外処理](09-handle-exception.md)
