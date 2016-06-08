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