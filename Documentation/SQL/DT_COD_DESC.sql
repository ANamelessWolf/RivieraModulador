  CREATE TABLE "RIVIERA"."DT_COD_DESC" 
   (	"CODIGO" VARCHAR2(15 BYTE) NOT NULL ENABLE, 
	"DESCRIPCION" VARCHAR2(150 BYTE) NOT NULL ENABLE, 
	"BLOQUE" VARCHAR2(15 BYTE) NOT NULL ENABLE, 
	"TIPO" VARCHAR2(2 BYTE) DEFAULT 'P', 
	"PANEL_DOBLE" VARCHAR2(2 BYTE) DEFAULT 'N'
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "RIVIERA" ;