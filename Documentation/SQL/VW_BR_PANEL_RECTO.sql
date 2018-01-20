SELECT
    DISTINCT(FREN.CODIGO||FREN.FRENTE||ALT.ALTO) AS PKEY,
    FREN.CODIGO AS CODIGO,
    FREN.FRENTE AS FRENTE_NOMINAL,
    FREN.VREAL/1000 AS FRENTE_REAL_M,
    ALT.ALTO AS ALTO_NOMINAL,
    ALT.VREAL/1000 AS ALTO_REAL_M
FROM
    (SELECT * FROM BR_COD_FREN_ALT_PAN PAN, BR_TAMANOS TAM WHERE PAN.FRENTE=TAM.VNOMINAL AND TAM.TIPO = 'FPG') FREN,
    (SELECT * FROM BR_COD_FREN_ALT_PAN PAN, BR_TAMANOS TAM WHERE PAN.ALTO=TAM.VNOMINAL  AND TAM.TIPO = 'APG') ALT
WHERE
    FREN.FRENTE = ALT.FRENTE
    AND
    FREN.ALTO = ALT.ALTO
    AND
    FREN.CODIGO = ALT.CODIGO
ORDER BY 
    FREN.CODIGO, 
    FREN.FRENTE, 
    ALT.ALTO;