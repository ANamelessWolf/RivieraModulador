SELECT
  FRE.CODIGO,
  FRE.FRENTE_NOMINAL,
  FRE.FRENTE_REAL_M,
  FON.FONDO_NOMINAL,
  FON.FONDO_REAL_M
FROM
(SELECT
  DISTINCT(CODIGO||FRENTE) AS FKEY,
    CODIGO,
    FRENTE AS FRENTE_NOMINAL,
    VREAL/1000 AS FRENTE_REAL_M
  FROM 
    BR_COD_FREN_FON_PAS LUZ, 
    BR_TAMANOS TAM 
  WHERE 
    LUZ.FRENTE=TAM.VNOMINAL 
  AND 
    TAM.TIPO = 'FLA') FRE,
(SELECT
  DISTINCT(CODIGO||FONDO) AS FKEY,
    CODIGO,
    FONDO AS FONDO_NOMINAL,
    VREAL/1000 AS FONDO_REAL_M
  FROM 
    BR_COD_FREN_FON_PAS LUZ, 
    BR_TAMANOS TAM 
  WHERE 
    LUZ.FONDO=TAM.VNOMINAL 
  AND 
    TAM.TIPO = 'FOL') FON    
WHERE
	FRE.CODIGO = FON.CODIGO 
ORDER BY FRE.CODIGO, FRE.FRENTE_NOMINAL, FON.FONDO_NOMINAL