 CREATE TABLE data (
    value TEXT
);

INSERT INTO data  VALUES ('Hello, in-memory SQLite dbms!');

select * from data;


--;with 
--  t1 as (select x=1),
--  t2 as (select y=2),
--  t3 as (
--SELECT t1x.custid
--FROM
--  (VALUES
--     (1, 'cust 1', '(111) 111-1111', 'address 1'),
--     (2, 'cust 2', '(222) 222-2222', 'address 2')
--  ) AS t1x(custid, companyname, phone, address))
  
--select * from t1, t2,t3