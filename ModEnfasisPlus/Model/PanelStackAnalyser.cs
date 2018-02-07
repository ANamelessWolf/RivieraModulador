using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.Runtime.Delta;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class PanelStackAnalyser
    {
        /// <summary>
        /// El lado a de la mampara
        /// </summary>
        public String[] StackSideA;
        /// <summary>
        /// El lado b de la mampara
        /// </summary>
        public String[] StackSideB;
        /// <summary>
        /// El frente de la mampara
        /// </summary>
        public Double Frente;
        /// <summary>
        /// Paneles que abarcan dos frentes en la mampara
        /// </summary>
        Double[] DobleFrente = new Double[] { 54, 60, 66, 72 };
        //Códigos de mamparas electricas
        String[] eRgtPan = new String[] { "DD2032", "DD2036", "DD2042", "DD2046" };
        String[] eLftPan = new String[] { "DD2033", "DD2037", "DD2043", "DD2047" };
        String[] ePan = new String[] { "DD203", "DD204" };
        /// <summary>
        /// Define la lista de paneles del stack A y del stack B
        /// </summary>
        /// <param name="pStkA">La colección de paneles del lado A</param>
        /// <param name="pStkB">La colección de paneles del lado B</param>
        public PanelStackAnalyser(Mampara mam, RivieraPanelStack pStkA, RivieraPanelStack pStkB)
        {
            String[] stackA = pStkA.Collection.OrderBy(y => y.Height).Select<PanelRaw, String>(x => String.Format("{0}@{1}", x.Height, x.Code)).ToArray(),
                     stackB = pStkB.Collection.OrderBy(y => y.Height).Select<PanelRaw, String>(x => String.Format("{0}@{1}", x.Height, x.Code)).ToArray(),
                     data;
            int maxPanelSize = stackA.Length > stackB.Length ? stackA.Length : stackB.Length;
            this.StackSideA = new String[maxPanelSize];
            this.StackSideB = new String[maxPanelSize];
            String code;
            Double height, heightA, heightB, num, frente;
            frente = Double.TryParse(mam.Code.Substring(6, 2), out num) ? num : 0;
            this.Frente = frente;
            for (int i = 0, aIndex = 0, bIndex = 0; i < maxPanelSize; i++)
            {
                //Lado A
                data = stackA[aIndex].Split('@');
                height = Double.TryParse(data[0], out num) ? num : 0;
                code = data[1];
                StackSideA[i] = code;
                //Lado B
                data = stackB[bIndex].Split('@');
                height = Double.TryParse(data[0], out num) ? num : 0;
                code = data[1];
                StackSideB[i] = code;
                //Se calcula el siguiente indice del
                //siguiente panel
                if ((aIndex + 1) < stackA.Length)
                {
                    data = stackA[aIndex + 1].Split('@');
                    heightA = Double.TryParse(data[0], out num) ? num : 0;
                }
                else
                    heightA = -1;
                //Se calcula el siguiente indice del
                //siguiente panel
                if ((bIndex + 1) < stackB.Length)
                {
                    data = stackB[bIndex + 1].Split('@');
                    heightB = Double.TryParse(data[0], out num) ? num : 0;
                }
                else
                    heightB = -1;
                //Se calcula el indice del siguiente panel
                if (heightA == -1 && heightB != -1 && heightA > heightB)
                    bIndex += 1;
                else if (heightA != -1 && heightB == -1 && heightB > heightA)
                    aIndex += 1;
                else if (heightA == -1 && heightB != -1)
                    bIndex += 1;
                else if (heightA != -1 && heightB == -1)
                    aIndex += 1;
                else if (heightA == heightB && heightA != -1)
                {
                    aIndex += 1;
                    bIndex += 1;
                }
                else if (heightA >= -1 && heightB != -1 && heightB < heightA)
                    bIndex += 1;
                else if (heightA != -1 && heightB != -1 && heightB > heightA)
                    aIndex += 1;
            }
        }
        /// <summary>
        /// Define la lista de paneles del stack A y del stack B
        /// </summary>
        /// <param name="frente">El frente de la mampara</param>
        /// <param name="pStkA">La colección de paneles del lado A</param>
        /// <param name="pStkB">La colección de paneles del lado B</param>
        public PanelStackAnalyser(Double frente, RivieraPanelStack pStkA, RivieraPanelStack pStkB)
        {
            String[] stackA = pStkA.Collection.OrderBy(y => y.Height).Select<PanelRaw, String>(x => String.Format("{0}@{1}", x.Height, x.Code)).ToArray(),
                     stackB = pStkB.Collection.OrderBy(y => y.Height).Select<PanelRaw, String>(x => String.Format("{0}@{1}", x.Height, x.Code)).ToArray(),
                     data;
            int maxPanelSize = stackA.Length > stackB.Length ? stackA.Length : stackB.Length;
            this.StackSideA = new String[maxPanelSize];
            this.StackSideB = new String[maxPanelSize];
            String code;
            Double height, heightA, heightB, num;
            this.Frente = frente;
            for (int i = 0, aIndex = 0, bIndex = 0; i < maxPanelSize; i++)
            {
                //Lado A
                data = stackA[aIndex].Split('@');
                height = Double.TryParse(data[0], out num) ? num : 0;
                code = data[1];
                StackSideA[i] = code;
                //Lado B
                data = stackB[bIndex].Split('@');
                height = Double.TryParse(data[0], out num) ? num : 0;
                code = data[1];
                StackSideB[i] = code;
                //Se calcula el siguiente indice del
                //siguiente panel
                if ((aIndex + 1) < stackA.Length)
                {
                    data = stackA[aIndex + 1].Split('@');
                    heightA = Double.TryParse(data[0], out num) ? num : 0;
                }
                else
                    heightA = -1;
                //Se calcula el siguiente indice del
                //siguiente panel
                if ((bIndex + 1) < stackB.Length)
                {
                    data = stackB[bIndex + 1].Split('@');
                    heightB = Double.TryParse(data[0], out num) ? num : 0;
                }
                else
                    heightB = -1;
                //Se calcula el indice del siguiente panel
                if (heightA == -1 && heightB != -1 && heightA > heightB)
                    bIndex += 1;
                else if (heightA != -1 && heightB == -1 && heightB > heightA)
                    aIndex += 1;
                else if (heightA == -1 && heightB != -1)
                    bIndex += 1;
                else if (heightA != -1 && heightB == -1)
                    aIndex += 1;
                else if (heightA == heightB && heightA != -1)
                {
                    aIndex += 1;
                    bIndex += 1;
                }
                else if (heightA != -1 && heightB != -1 && heightB < heightA)
                    bIndex += 1;
                else if (heightA != -1 && heightB != -1 && heightB > heightA)
                    aIndex += 1;
            }
        }

        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene paneles eléctricos en el lado izquierdo
        /// </summary>
        /// <param name="index">El indice del nivel seleccionado</param>
        public Boolean HasLeftElectricPanelesOnTheSameSide(int index)
        {
            return eRgtPan.Contains(StackSideA[index].Substring(0, 6)) && eLftPan.Contains(StackSideB[index].Substring(0, 6));
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene paneles eléctricos en el lado derecho
        /// </summary>
        /// <param name="index">El indice del nivel seleccionado</param>
        public Boolean HasRightElectricPanelesOnTheSameSide(int index)
        {
            return eLftPan.Contains(StackSideA[index].Substring(0, 6)) && eRgtPan.Contains(StackSideB[index].Substring(0, 6));
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene un paneles eléctricos en el lado derecho y en el lado izquierdo
        /// </summary>
        /// <param name="index">El indice del nivel seleccionado</param>
        public Boolean HasLeftAndRightElectricPaneles(int index)
        {
            return eRgtPan.Contains(StackSideA[index].Substring(0, 6)) && eRgtPan.Contains(StackSideB[index].Substring(0, 6)) ||
                   eLftPan.Contains(StackSideA[index].Substring(0, 6)) && eLftPan.Contains(StackSideB[index].Substring(0, 6));
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene solo un panel eléctrico en el lado izquierdo
        /// </summary>
        /// <param name="index">El indice del nivel seleccionado</param>
        public Boolean HasOneRightElectricPanel(int index)
        {
            return !eLftPan.Contains(StackSideA[index].Substring(0, 6)) && eRgtPan.Contains(StackSideB[index].Substring(0, 6));
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene solo un panel eléctrico en el lado derecho
        /// </summary>
        /// <param name="index">El indice del nivel seleccionado</param>
        public Boolean HasOneLeftElectricPanel(int index)
        {
            return eRgtPan.Contains(StackSideA[index].Substring(0, 6)) && !eLftPan.Contains(StackSideB[index].Substring(0, 6));
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene solo un panel eléctrico en el lado derecho
        /// </summary>
        /// <param name="index">El indice del nivel seleccionado</param>
        public Boolean HasOneElectricPanel(int index)
        {
            return eRgtPan.Union(eLftPan).Contains(StackSideA[index].Substring(0, 6)) || eRgtPan.Union(eLftPan).Contains(StackSideB[index].Substring(0, 6));
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene solo un panel eléctrico sin importar el lado
        /// </summary>
        /// <param name="index">El indice del nivel seleccionado</param>
        public Boolean HasElectricPanels(int index)
        {
            return ePan.Contains(StackSideA[index].Substring(0, 5)) || ePan.Contains(StackSideB[index].Substring(0, 5));
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene paneles eléctricos en el lado izquierdo
        /// </summary>
        public Boolean HasLeftElectricPanelesOnTheSameSide()
        {
            Boolean flag = false, res;
            for (int index = 0; !flag && index < StackSideA.Length; index++)
            {
                res = eRgtPan.Contains(StackSideA[index].Substring(0, 6)) && eLftPan.Contains(StackSideB[index].Substring(0, 6));
                flag = flag || res;
            }
            return flag;
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene paneles eléctricos en el lado derecho
        /// </summary>
        public Boolean HasRightElectricPanelesOnTheSameSide()
        {
            Boolean flag = false, res;
            for (int index = 0; !flag && index < StackSideA.Length; index++)
            {
                res = eLftPan.Contains(StackSideA[index].Substring(0, 6)) && eRgtPan.Contains(StackSideB[index].Substring(0, 6));
                flag = flag || res;
            }
            return flag;
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene un paneles eléctricos en el lado derecho y en el lado izquierdo
        /// </summary>
        public Boolean HasLeftAndRightElectricPaneles()
        {
            Boolean flag = false, res;
            for (int index = 0; !flag && index < StackSideA.Length; index++)
            {
                res = eRgtPan.Contains(StackSideA[index].Substring(0, 6)) && eRgtPan.Contains(StackSideB[index].Substring(0, 6)) ||
                      eLftPan.Contains(StackSideA[index].Substring(0, 6)) && eLftPan.Contains(StackSideB[index].Substring(0, 6));
                flag = flag || res;
            }
            return flag;
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene solo un panel eléctrico en el lado izquierdo
        /// </summary>
        public Boolean HasOneRightElectricPanel()
        {
            Boolean flag = false, res;
            for (int index = 0; !flag && index < StackSideA.Length; index++)
            {
                res = !eLftPan.Contains(StackSideA[index].Substring(0, 6)) && eRgtPan.Contains(StackSideB[index].Substring(0, 6));
                flag = flag || res;
            }
            return flag;
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene solo un panel eléctrico en el lado derecho
        /// </summary>
        public Boolean HasOneLeftElectricPanel()
        {
            Boolean flag = false, res;
            for (int index = 0; !flag && index < StackSideA.Length; index++)
            {
                res = eRgtPan.Contains(StackSideA[index].Substring(0, 6)) && !eLftPan.Contains(StackSideB[index].Substring(0, 6));
                flag = flag || res;
            }
            return flag;
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene solo un panel eléctrico en el lado derecho
        /// </summary>
        public Boolean HasOneElectricPanel()
        {
            Boolean flag = false, res;
            for (int index = 0; !flag && index < StackSideA.Length; index++)
            {
                res = eRgtPan.Union(eLftPan).Contains(StackSideA[index].Substring(0, 6)) || eRgtPan.Union(eLftPan).Contains(StackSideB[index].Substring(0, 6));
                flag = flag || res;
            }
            return flag;
        }
        /// <summary>
        /// Analiza si un nivel del panel stack
        /// contiene solo un panel eléctrico sin importar el lado
        /// </summary>
        public Boolean HasElectricPanels()
        {
            Boolean flag = false, res;
            for (int index = 0; !flag && index < StackSideA.Length; index++)
            {
                res = ePan.Contains(StackSideA[index].Substring(0, 5)) || ePan.Contains(StackSideB[index].Substring(0, 5));
                flag = flag || res;
            }
            return flag;
        }
        /// <summary>
        /// Realiza la cuantificación de los soportes
        /// </summary>
        /// <param name="qua">El cuantificador</param>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <param name="handle">El handle asignado al soporte</param>
        public void QuantifySoportes(Quantifier qua, String zoneName, long handle)
        {
            Boolean mamparaIsRotated = this.CheckIfMamparaIsRotated(handle);
            for (int i = 0; i < StackSideA.Length; i++)
                //El calculo solo aplica a paneles electricos
                if (HasElectricPanels(i))
                {
                    if (HasLeftAndRightElectricPaneles(i))
                    {
                        if (this.Frente == 54)
                        {
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                            if (!mamparaIsRotated)
                                throw new DeltaException(ERR_SOP_24IN);
                        }
                        else if (this.Frente == 60)
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 2, zoneName, handle);
                        else if (this.Frente == 66)
                        {
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 36), 1, zoneName, handle);
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                        }
                        else if (this.Frente == 72)
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 36), 2, zoneName, handle);
                    }
                    else if (HasLeftElectricPanelesOnTheSameSide(i) || HasOneLeftElectricPanel(i))
                    {
                        if (this.Frente == 54)
                        {
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                            if (mamparaIsRotated)
                                throw new DeltaException(ERR_SOP_24IN);
                        }
                        else if (this.Frente == 60)
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                        else if (this.Frente == 66)
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 36), 1, zoneName, handle);
                        else if (this.Frente == 72)
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 36), 1, zoneName, handle);
                    }
                    else if (HasRightElectricPanelesOnTheSameSide(i) || HasOneRightElectricPanel(i))
                    {
                        if (this.Frente == 54)
                        {
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                            if (!mamparaIsRotated)
                                throw new DeltaException(ERR_SOP_24IN);
                        }
                        else if (this.Frente == 60)
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                        else if (this.Frente == 66)
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                        else if (this.Frente == 72)
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 36), 1, zoneName, handle);
                    }
                    else if (HasOneElectricPanel(i))
                    {
                        if (this.Frente == 54)
                        {
                            if (StackSideB.Select(x => x.Substring(0, 6)).Count(y => eLftPan.Contains(y)) > 0)
                            {
                                qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                                if (mamparaIsRotated)
                                    throw new DeltaException(ERR_SOP_24IN);
                            }
                            else
                            {
                                qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                                if (!mamparaIsRotated)
                                    throw new DeltaException(ERR_SOP_24IN);
                            }
                        }
                        else if (this.Frente == 60)
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                        else if (this.Frente == 66)
                        {
                            if (StackSideB.Select(x => x.Substring(0, 6)).Count(y => eLftPan.Contains(y)) > 0)
                                qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 36), 1, zoneName, handle);
                            else
                                qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 30), 1, zoneName, handle);
                        }
                        else if (this.Frente == 72)
                            qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, 36), 1, zoneName, handle);
                    }
                    else
                        qua.AddItem(String.Format("{0}{1}S", DT_SOPORTE, this.Frente), 1, zoneName, handle);
                }
        }

        private bool CheckIfMamparaIsRotated(long handle)
        {
            Boolean flag = false;
            var item = App.DB[handle];
            if (item != null && item.Parent != 0)
            {
                var parent = App.DB[item.Parent];
                if (parent != null)
                {
                    ObjectId parentDicId;
                    Xrecord xRec;
                    Document doc = Application.DocumentManager.MdiActiveDocument;
                    using (doc.LockDocument())
                    {
                        new FastTransactionWrapper((Document d, Transaction tr) =>
                        {
                            try
                            {
                                parentDicId = parent.Line.Id.GetObject(OpenMode.ForRead).ExtensionDictionary;
                                if (parentDicId.IsValid)
                                {
                                    var dic = parentDicId.GetObject(OpenMode.ForRead) as DBDictionary;
                                    var dMAn = new DManager(dic);
                                    if (dMAn.TryGetRegistry(ROTATE_GEOMETRY_XRECORD, out xRec, tr))
                                    {
                                        var res = xRec.GetDataAsString(tr)[0];
                                        flag = Double.Parse(res) != 0d;
                                    }
                                }
                            }
                            catch (Exception) { }
                        }).Run();
                    }
                }
            }
            return flag;
        }


        /// <summary>
        /// Realiza la cuantificación de canaletas
        /// </summary>
        /// <param name="qua">El cuantificador</param>
        /// <param name="numOfCanaletas">El número de paneles</param>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <param name="handle">El handle asignado al soporte</param>
        public void QuantifyCanaletas(Quantifier qua, int numOfCanaletas, String zoneName, long handle)
        {
            numOfCanaletas = numOfCanaletas - 1;
            int canExtra = App.Riviera.IsArnesEnabled ? 1 : 0;
            //El calculo solo aplica a paneles electricos
            if (HasLeftAndRightElectricPaneles())
            {
                if (this.Frente == 54)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 24), numOfCanaletas, zoneName, handle);
                }
                else if (this.Frente == 60)
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas * 2, zoneName, handle);
                else if (this.Frente == 66)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                }
                else if (this.Frente == 72)
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas * 2, zoneName, handle);
            }
            else if (HasLeftElectricPanelesOnTheSameSide() || HasOneLeftElectricPanel())
            {
                if (this.Frente == 54)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 24), numOfCanaletas + canExtra, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                }
                else if (this.Frente == 60)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                }
                else if (this.Frente == 66)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas, zoneName, handle);
                }
                else if (this.Frente == 72)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas + canExtra, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas, zoneName, handle);
                }
            }
            else if (HasRightElectricPanelesOnTheSameSide() || HasOneRightElectricPanel())
            {
                if (this.Frente == 54)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 24), numOfCanaletas, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                }
                else if (this.Frente == 60)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                }
                else if (this.Frente == 66)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas + canExtra, zoneName, handle);
                }
                else if (this.Frente == 72)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas + canExtra, zoneName, handle);
                }
            }
            else if (HasOneElectricPanel())
            {
                if (this.Frente == 54)
                {
                    if (StackSideB.Select(x => x.Substring(0, 6)).Count(y => eLftPan.Contains(y)) > 0)
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 24), numOfCanaletas + canExtra, zoneName, handle);
                    }
                    else
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 24), numOfCanaletas, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                    }
                }
                else if (this.Frente == 60)
                {
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                }
                else if (this.Frente == 66)
                {
                    if (StackSideB.Select(x => x.Substring(0, 6)).Count(y => eLftPan.Contains(y)) > 0)
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                    }
                    else
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas + canExtra, zoneName, handle);
                    }
                }
                else if (this.Frente == 72)
                {

                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas, zoneName, handle);
                    qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas + canExtra, zoneName, handle);
                }
            }
            else
            {
                if (HasElectricPanels())
                {
                    if (this.Frente == 54)
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 24), numOfCanaletas, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                    }
                    else if (this.Frente == 60)
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                    }
                    else if (this.Frente == 66)
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas, zoneName, handle);
                    }
                    else if (this.Frente == 72)
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas, zoneName, handle);
                    }
                    else
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, this.Frente), numOfCanaletas, zoneName, handle);
                }
                else
                {
                    if (this.Frente == 54)
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 24), numOfCanaletas + canExtra, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                    }
                    else if (this.Frente == 60)
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                    }
                    else if (this.Frente == 66)
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 30), numOfCanaletas + canExtra, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas + canExtra, zoneName, handle);
                    }
                    else if (this.Frente == 72)
                    {
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas + canExtra, zoneName, handle);
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, 36), numOfCanaletas + canExtra, zoneName, handle);
                    }
                    else
                        qua.AddItem(String.Format("{0}{1}S", DT_CANALETA, this.Frente), numOfCanaletas + canExtra, zoneName, handle);
                }
            }
        }
    }
}
