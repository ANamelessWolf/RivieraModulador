--------------------------------------------------------
-- Archivo creado  - miércoles-agosto-17-2016   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table DT_ALT_NIV_PAN
--------------------------------------------------------

  CREATE TABLE "RIVIERA"."DT_ALT_NIV_PAN" 
   (	"ALTO" NUMBER, 
	"NIVELES" VARCHAR2(8 BYTE)
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "RIVIERA" ;
REM INSERTING into RIVIERA.DT_ALT_NIV_PAN
SET DEFINE OFF;
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (23,'1 1/2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (35,'2 1/2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (47,'3 1/2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (53,'4');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (6,'1/2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (12,'1');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (18,'1 1/2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (24,'2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (30,'2 1/2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (36,'3');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (42,'3 1/2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (5,'0');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (17,'1');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (29,'2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (41,'3');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (8,'1');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (11,'1/2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (14,'2');
Insert into RIVIERA.DT_ALT_NIV_PAN (ALTO,NIVELES) values (20,'2 1/2');
--------------------------------------------------------
--  Constraints for Table DT_ALT_NIV_PAN
--------------------------------------------------------

  ALTER TABLE "RIVIERA"."DT_ALT_NIV_PAN" MODIFY ("ALTO" NOT NULL ENABLE);
 
  ALTER TABLE "RIVIERA"."DT_ALT_NIV_PAN" MODIFY ("NIVELES" NOT NULL ENABLE);
