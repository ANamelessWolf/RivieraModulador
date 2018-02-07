--------------------------------------------------------
-- Archivo creado  - miércoles-agosto-17-2016   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table DT_ALT_NIV_MAM
--------------------------------------------------------

  CREATE TABLE "RIVIERA"."DT_ALT_NIV_MAM" 
   (	"ALTO" NUMBER, 
	"NIVELES" VARCHAR2(8 BYTE)
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "RIVIERA" ;
REM INSERTING into RIVIERA.DT_ALT_NIV_MAM
SET DEFINE OFF;
Insert into RIVIERA.DT_ALT_NIV_MAM (ALTO,NIVELES) values (30,'2');
Insert into RIVIERA.DT_ALT_NIV_MAM (ALTO,NIVELES) values (36,'2 1/2');
Insert into RIVIERA.DT_ALT_NIV_MAM (ALTO,NIVELES) values (42,'3');
Insert into RIVIERA.DT_ALT_NIV_MAM (ALTO,NIVELES) values (48,'3 1/2');
Insert into RIVIERA.DT_ALT_NIV_MAM (ALTO,NIVELES) values (54,'4');
--------------------------------------------------------
--  Constraints for Table DT_ALT_NIV_MAM
--------------------------------------------------------

  ALTER TABLE "RIVIERA"."DT_ALT_NIV_MAM" MODIFY ("ALTO" NOT NULL ENABLE);
