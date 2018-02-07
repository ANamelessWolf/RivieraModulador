--------------------------------------------------------
-- Archivo creado  - mi�rcoles-agosto-17-2016   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table REVIT_PROYECTO
--------------------------------------------------------

  CREATE TABLE "RIVIERA"."REVIT_PROYECTO" 
   (	"ID_USER" VARCHAR2(36 BYTE), 
	"ID_REVIT" NUMBER(7,0), 
	"ARCHIVO_REVIT" VARCHAR2(100 BYTE), 
	"FECHA_ULTIMO_ACCESO" DATE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "RIVIERA" ;
REM INSERTING into RIVIERA.REVIT_PROYECTO
SET DEFINE OFF;
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',41,'Project2.rvt',to_date('21/04/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',181,'RACKS PRUEBA 1.rvt',to_date('05/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',246,'prueba 4.rvt',to_date('05/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',248,'prueba 6.rvt',to_date('06/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',255,'pruev.rvt',to_date('09/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',256,'JCYH-RACKS.rvt',to_date('09/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',259,'Project3',to_date('15/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',265,'ejercicio 2.rvt',to_date('20/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',266,'ejercicio 2.rvt',to_date('20/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',287,'prueba protecciones 2.rvt',to_date('17/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',288,'AVI_SOFIA.rvt',to_date('23/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',290,'revit neolpharma',to_date('25/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',299,'ORSL-0714-P074-M1',to_date('24/07/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',305,'COMPONENTE_11',to_date('27/08/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',306,'COMPONENTE_9',to_date('27/08/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',307,'COMPONENTE_8',to_date('27/08/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',310,'prueba revit 09jun2014',to_date('24/09/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',312,'MODULO PENAL DE MORELOS',to_date('26/09/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',314,'ExtensionTest',to_date('08/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',318,'ficticio 3000 posiciones',to_date('20/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',328,'ENSAMBLE_credenza',to_date('24/11/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',331,'MR81-01-06 DOBLE',to_date('05/01/15','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',338,'DaSoft Prueba',to_date('05/05/16','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',141,'PRUEBA RACKS.rvt',to_date('25/04/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',162,'prueba racks.rvt',to_date('02/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',251,'RACKS 5.rvt',to_date('06/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',252,'PLEXUS1.rvt',to_date('06/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',253,'PLEXUS PARRILLA.rvt',to_date('06/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',268,'princel.rvt',to_date('20/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',271,'proyecto21mayo.rvt',to_date('21/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',272,'FLOPAN.rvt',to_date('22/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',277,'Project4',to_date('28/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',289,'REVIT COPAMEX',to_date('24/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',293,'REVIT CAPAMEX 02',to_date('02/07/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',294,'ONETS',to_date('11/07/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',298,'Project2cotizacion.rvt',to_date('17/07/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',309,'ENSAMBLE_ensamble credenza en partes',to_date('24/09/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',311,'Prueba Mirlin',to_date('25/09/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',315,'Project3',to_date('09/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',316,'Project2',to_date('09/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',317,'9oct2014',to_date('09/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',319,'ficticio 3000 posiciones.rvt',to_date('22/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',320,'imbursa 2000 posiciones.rvt',to_date('22/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',321,'dante revit 22 oct',to_date('22/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',322,'Prueba',to_date('22/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',323,'SEMBRADO',to_date('23/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',327,'ENSAMBLE_credenza FAO',to_date('03/11/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',334,'CuantificacionFronteras',to_date('23/03/15','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',335,'prueba con mamparas 2060',to_date('01/04/15','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',337,'ENSAMBLE_EnsambleGeneral',to_date('22/10/15','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',249,'prueba 7.rvt',to_date('06/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',254,'ejemplo racks.rvt',to_date('07/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',257,'EJERCICIO1.rvt',to_date('09/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',258,'FLOPAN.rvt',to_date('15/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',260,'princel.rvt',to_date('16/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',273,'PRUEBA 22052014.rvt',to_date('22/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',274,'PRINCEL 22052014.rvt',to_date('22/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',275,'PRINCEL 23052014.rvt',to_date('26/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',276,'ISEM.rvt',to_date('27/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',279,'proyecto03jun2014.rvt',to_date('03/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',281,'imbursa.rvt',to_date('04/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',284,'prueba revit 09jun2014.rvt',to_date('11/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',291,'RACKS BOLIBIA',to_date('26/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',292,'RACKS BOLIBIA',to_date('30/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',325,'REHAU CELAYA OPCION 2',to_date('27/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',329,'EnsambleGeneral',to_date('25/11/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',336,'Test Riv',to_date('10/07/15','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',61,'Project2.rvt',to_date('24/04/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',101,'Project1',to_date('24/04/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',201,'Project4',to_date('05/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',221,'Project1',to_date('05/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',241,'Uno.rvt',to_date('05/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',243,'DOS.rvt',to_date('05/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',244,'Project2',to_date('05/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',245,'PRUEBA 3.rvt',to_date('05/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',247,'prueba 5.rvt',to_date('06/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',250,'racks 2.rvt',to_date('06/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',262,'prueba protecciones.rvt',to_date('20/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',267,'Project2nuevo.rvt',to_date('20/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',269,'Princel.rvt',to_date('21/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',270,'PLEXUS.rvt',to_date('21/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',278,'RACKESA.rvt',to_date('28/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',286,'revit amado.0002',to_date('13/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',295,'Project1ensayo.rvt',to_date('16/07/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',296,'Project2ensayo.rvt',to_date('16/07/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',297,'Project3ensayo.rvt',to_date('16/07/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',300,'ENSAMBLE_Assembly2',to_date('21/08/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',301,'ENSAMBLE_EnsambleGeneral',to_date('25/08/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',302,'ENSAMBLE_Librero 1',to_date('25/08/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',326,'REHAU CELAYA OPCION 3',to_date('31/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',332,'Envolvente_Validaci�n Test',to_date('19/03/15','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',21,'Project1',to_date('15/04/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',81,'Project2',to_date('24/04/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',121,'Project3',to_date('24/04/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',122,'Project5',to_date('24/04/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',161,'prueba.rvt',to_date('02/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',261,'PRINCEL2.rvt',to_date('19/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',263,'protecciones prueba2.rvt',to_date('20/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',264,'protecciones prueba3.rvt',to_date('20/05/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',280,'IMBURSA',to_date('04/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',282,'imbursa',to_date('04/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',283,'imbursa prueba.rvt',to_date('04/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',285,'revit amado',to_date('12/06/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',303,'ENSAMBLE_B',to_date('27/08/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',304,'COMPONENTE_10',to_date('27/08/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',308,'ENSAMBLE_LUIS',to_date('12/09/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('91',313,'PRUEBA3oct',to_date('03/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',324,'REHAU CELAYS',to_date('24/10/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('991',330,'ENSAMBLE_CREDENZA',to_date('25/11/14','DD/MM/RR'));
Insert into RIVIERA.REVIT_PROYECTO (ID_USER,ID_REVIT,ARCHIVO_REVIT,FECHA_ULTIMO_ACCESO) values ('99',333,'QuantificacionFronteras',to_date('19/03/15','DD/MM/RR'));
--------------------------------------------------------
--  DDL for Trigger REVIT_PROYECTO_INSERT
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "RIVIERA"."REVIT_PROYECTO_INSERT" 
BEFORE INSERT ON REVIT_PROYECTO
FOR EACH ROW
DECLARE
  SEC REVIT_PROYECTO.ID_REVIT%TYPE;
BEGIN
  SELECT INC_REVIT_PROYECTO.NEXTVAL INTO SEC 
  FROM DUAL;  
  :NEW.ID_REVIT := SEC;
  :NEW.FECHA_ULTIMO_ACCESO := SYSDATE;
END;
/
ALTER TRIGGER "RIVIERA"."REVIT_PROYECTO_INSERT" ENABLE;
