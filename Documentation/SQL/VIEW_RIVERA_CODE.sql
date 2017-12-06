SELECT 
    tab_line.LINEA as linea,
    tab_desc.CODIGO as codigo, 
    tab_desc.DESCRIPCION AS codigo_desc,  
    tab_desc.BLOQUE AS bloque,  
    tab_desc.TIPO AS tipo,  
    tab_desc.PANEL_DOBLE AS panel_doble,  
    tab_acab.ACBADO AS acabado,
    tab_acab.DESCRIPCION AS acabado_desc
FROM 
    DT_COD_ACAB tab_acab, 
    DT_COD_DESC tab_desc, 
    DT_COD_LINEA tab_line
WHERE
    tab_acab.CODIGO = tab_desc.CODIGO
AND
    tab_desc.CODIGO = tab_line.CODIGO
ORDER BY 
    tab_line.LINEA, 
    tab_desc.CODIGO, 
    tab_acab.ACBADO;