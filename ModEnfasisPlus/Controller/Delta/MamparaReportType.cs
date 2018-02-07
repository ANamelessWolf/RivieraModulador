using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Tsumugi;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Controller.Delta
{
    public class MamparaReportType
    {
        const double TEXT_HEIGHT = 0.1d;
        const double BIOMBO_OFFSET = 0.5d;
        const double FLOOR_OFFSET = 0.128d;
        const double PANEL_X_OFFSET = 0.004d;
        const double TXT_X_OFFSET = (2d * TEXT_HEIGHT / 3d);
        const double TXT_Y_OFFSET = (TEXT_HEIGHT + 0.02);
        const double ROW_OFFSET = 0.6d;
        /// <summary>
        /// La colección de mamparas que define la aplicación
        /// </summary>
        public List<Mampara> Mamparas, DoubleMamparas;
        /// <summary>
        /// El punto de inserción del reporte
        /// </summary>
        public Point3d InsertionPoint;
        /// <summary>
        /// Los elementos clasificados
        /// </summary>
        public Dictionary<String, MamparaReportGroup> Items;
        /// <summary>
        /// La lista de elementos a tratar
        /// </summary>
        ObjectIdCollection Ids;
        /// <summary>
        /// La lista de entidades a dibujar
        /// </summary>
        List<Entity> Entities;
        /// <summary>
        /// Las entidades por mamparas
        /// </summary>
        public List<List<Entity>> EntitiesGroup;
        /// <summary>
        /// Las entidades de texto que son ignoradas al momento de aplicar la transformada
        /// en el modo imperial
        /// </summary>
        public List<Entity> IgnoreIdsInImperialMode;
        /// <summary>
        /// Guarda la cuantificación de la mampara
        /// </summary>
        public String Data;
        

        /// <summary>
        /// Crea un nuevo reportador de mamparas
        /// </summary>
        /// <param name="m">La colección de mamparas analizar</param>
        /// <param name="pt">El punto de inserción de la mampara</param>
        public MamparaReportType(List<Mampara> m, Point3d pt)
        {
            this.Mamparas = m.Where(x => !x.HasDoublePanels).ToList();
            this.DoubleMamparas = m.Where(x => x.HasDoublePanels).ToList();
            this.InsertionPoint = pt;
            this.Items = new Dictionary<String, MamparaReportGroup>();
            this.Entities = new List<Entity>();
            this.EntitiesGroup = new List<List<Entity>>();
            this.IgnoreIdsInImperialMode = new List<Entity>();
            AutoCADLayer layer = new AutoCADLayer(LAYER_RIVIERA_REPORT);
            layer.SetStatus(NamelessOld.Libraries.HoukagoTeaTime.Mio.Model.LayerStatus.EnableStatus);
        }
        /// <summary>
        /// Realiza la cuantificación por reporte
        /// </summary>
        public void Quantify()
        {
            MamparaReportItem itemA, itemB;
            DBText tagA, tagB;
            int index;
            foreach (Mampara mampara in this.Mamparas)
            {
                try
                {
                    itemA = new MamparaReportItem()
                    {
                        Mampara = mampara.Code.AddAcabado(mampara),
                        Biombo = mampara.BiomboId != 0 ? App.DB[mampara.BiomboId] as RivieraBiombo : null,
                        Panels = MamparaReportItem.LoaPaneles(mampara.Children[FIELD_RIGHT_FRONT])
                    };
                    //En el lado b guardo la mampara sin acabado
                    itemB = new MamparaReportItem()
                    {
                        Mampara = mampara.Code,
                        Biombo = mampara.BiomboId != 0 ? App.DB[mampara.BiomboId] as RivieraBiombo : null,
                        Panels = MamparaReportItem.LoaPaneles(mampara.Children[FIELD_LEFT_FRONT])
                    };
                    MamparaReportGroup grp = new MamparaReportGroup() { LadoA = itemA, LadoB = itemB, Count = 1 };
                    String key = grp.ToString();
                    if (this.Items.ContainsKey(key))
                        index = this.Items.Keys.ToList().IndexOf(grp.ToString());
                    else
                        index = this.Items.Count;
                    //El indice de la mampara
                    itemA.Lado = String.Format("A{0}", index);
                    itemB.Lado = String.Format("B{0}", index);

                    //Crea las etiquetas del reporte
                    mampara.CreateReportTags(itemA.Lado, itemB.Lado, out tagA, out tagB);
                    this.Entities.Add(tagA);
                    this.Entities.Add(tagB);
                    if (App.Riviera.Units == DaNTeUnits.Imperial)
                    {
                        IgnoreIdsInImperialMode.Add(tagA);
                        IgnoreIdsInImperialMode.Add(tagB);
                    }
                    //Lado A
                    if (this.Items.ContainsKey(key))
                        this.Items[grp.ToString()].Count++;
                    else
                        this.Items.Add(grp.ToString(), grp);
                }
                catch (Exception exc)
                {
                    App.Riviera.Log.AppendEntry(String.Format("Error en el reporte de mamparas tipo (Mampara: \"{0}\", exc: {1})", mampara.Code, exc.Message));
                    Selector.Ed.WriteMessage(exc.Message);
                }
            }
            this.QuantifyDoublePaneles();
        }

        private void QuantifyDoublePaneles()
        {
            DoubleMamparaReportItem itemDouble;
            DBText tagA, tagB, tagC;
            int index;
            Dictionary<long, JointObject> jointWithDoublePanels = new Dictionary<long, JointObject>();
            foreach (Mampara mampara in this.DoubleMamparas)
            {
                var handles = mampara.Parent != 0 ? mampara.Children.Where(x => x.Value != 0).Select(y => y.Value).Union(new long[] { mampara.Parent }) : mampara.Children.Where(x => x.Value != 0).Select(y => y.Value);
                foreach (var joint in handles.Select(x => App.DB[x]).Where(y => y is JointObject && (y as JointObject).HasDoublePanels))
                    if (!jointWithDoublePanels.ContainsKey(joint.Handle.Value))
                        jointWithDoublePanels.Add(joint.Handle.Value, joint as JointObject);

            }
            foreach (JointObject joint in jointWithDoublePanels.Values)
            {
                try
                {
                    itemDouble = new DoubleMamparaReportItem(joint);
                    DoubleMamparaReportGroup grp = new DoubleMamparaReportGroup() { LadoA = itemDouble.LadoA, LadoB_Left = itemDouble.LadoB_left, LadoB_Right = itemDouble.LadoB_Right, Count = 1 };
                    grp.Frente = itemDouble.GetFrente();
                    String key = grp.ToString();
                    if (this.Items.ContainsKey(key))
                        index = this.Items.Keys.ToList().IndexOf(grp.ToString());
                    else
                        index = this.Items.Count;
                    //El indice de la mampara
                    itemDouble.LadoA.Lado = String.Format("A{0}", index);
                    itemDouble.LadoB_left.Lado = String.Format("B{0}", index);
                    itemDouble.LadoB_Right.Lado = String.Format("B{0}", index);

                    //Crea las etiquetas del reporte
                    itemDouble.CreateReportTags(itemDouble.LadoA.Lado, itemDouble.LadoB_left.Lado, out tagA, out tagB, out tagC);
                    this.Entities.Add(tagA);
                    this.Entities.Add(tagB);
                    this.Entities.Add(tagC);
                    if (App.Riviera.Units == DaNTeUnits.Imperial)
                    {
                        IgnoreIdsInImperialMode.Add(tagA);
                        IgnoreIdsInImperialMode.Add(tagB);
                        IgnoreIdsInImperialMode.Add(tagC);
                    }
                    //Lado A
                    if (this.Items.ContainsKey(key))
                        this.Items[grp.ToString()].Count++;
                    else
                        this.Items.Add(grp.ToString(), grp);
                }
                catch (Exception exc)
                {
                    App.Riviera.Log.AppendEntry(String.Format("Error en el reporte de mamparas tipo (Mamparas dobles: \"{0}\", exc: {1})", joint.PanelArray.DobleFront.Raw.Code, exc.Message));
                    Selector.Ed.WriteMessage(exc.Message);
                }
            }
        }

        /// <summary>
        /// Realiza el dibujado del reporte
        /// </summary>
        public void Draw()
        {
            Entity[] ents = this.Entities.Union(this.CreateContent()).ToArray();
            new FastTransactionWrapper(
                delegate (Document doc, Transaction tr)
                {
                    for (int i = 0; i < ents.Length; i++)
                        ents[i].Layer = LAYER_RIVIERA_REPORT;
                    this.Ids = Drawer.Entity(tr, ents);
                    foreach (List<Entity> entByGrp in this.EntitiesGroup)
                    {
                        int index = GroupManager.GetGroupNames(tr).Where(x => x.Contains(GROUP_MAM_REP)).Count();
                        GroupManager gpMan = new GroupManager(tr, String.Format("{0}{1:000}", GROUP_MAM_REP, index + 1));
                        IEnumerable<ObjectId> entIds = entByGrp.Select(x => x.Id);
                        gpMan.AppendEntity(tr, new ObjectIdCollection(entIds.ToArray()));
                    }

                    if (App.Riviera.Units == DaNTeUnits.Imperial)
                    {
                        var ignoreIds = IgnoreIdsInImperialMode.Select(x => x.Id);
                        new ObjectIdCollection(this.Ids.OfType<ObjectId>().Where(x => ignoreIds.Count(y => y == x) == 0).ToArray()).Transform(Matrix3d.Scaling(IMPERIAL_FACTOR, this.InsertionPoint), tr);
                    }
                }).Run();
        }

        /// <summary>
        /// Crea el contenido del reporte de la mampara
        /// </summary>
        /// <returns>La colección de entidades dibujadas</returns>
        private List<Entity> CreateContent()
        {
            MamparaReportGroup grp;
            List<Entity> tmpEnts,
                         ents = new List<Entity>();
            //Hay dos códigos de mamparas uno de 10 y uno de 12,
            //El de 12 es un frente compuesto por cuatro digitos

            Point3d rowCenter = this.InsertionPoint,
                    panelStackAOrigin,
                    panelStackBOrigin, panelStackB_leftOrigin, panelStackB_rightOrigin,
                    biomboOrigin;
            int index = 0, count;
            Double rOffSet, biomboOffset;
            Boolean aPiso;
            foreach (String key in this.Items.Keys)
            {
                tmpEnts = new List<Entity>();
                grp = this.Items[key];
                count = grp.Count;
                if (grp is DoubleMamparaReportGroup)
                {
                    aPiso = true;
                    biomboOffset = 0;
                    var biomboData = this.Items[key].LadoA.Panels.Select(x => x.Value).Where(y => y.Direction == ArrowDirection.Same).FirstOrDefault();
                    if (biomboData != null)
                    {
                        biomboOffset = App.DB.Alto_Nivel.Where(x => x.Nivel == biomboData.Nivel && x.Type == ElementType.Panel).FirstOrDefault().Alto;
                        biomboOffset = biomboOffset.ConvertUnits(Unit_Type.inches, Unit_Type.m);
                    }
                    //frente = (grp as DoubleMamparaReportGroup).Frente;
                    rOffSet = this.DrawMamparas(rowCenter, grp.LadoA.Mampara, index, ref tmpEnts,
                                out panelStackAOrigin, out panelStackB_leftOrigin, out panelStackB_rightOrigin, aPiso, count, biomboOffset) + biomboOffset;

                    this.DrawPaneles(panelStackAOrigin, grp.LadoA.Panels, ref tmpEnts);
                    this.DrawPaneles(panelStackB_leftOrigin, (grp as DoubleMamparaReportGroup).LadoB_Left.Panels, ref tmpEnts);
                    this.DrawPaneles(panelStackB_rightOrigin, (grp as DoubleMamparaReportGroup).LadoB_Right.Panels, ref tmpEnts);
                }
                else
                {
                    aPiso = grp.LadoA.Panels.Count > 0 ? grp.LadoA.Panels.FirstOrDefault().Value.APiso : false;
                    //frente = double.Parse(grp.LadoA.Mampara.Substring(grp.LadoB.Mampara.Length - 4, 2));
                    //MamparaSize fSize = App.DB.Mampara_Sizes.Where(x => grp.LadoB.Mampara.Substring(0, 6) == x.Code && x.Frente == frente.ToString()).FirstOrDefault();
                    rOffSet = this.DrawMamparas(rowCenter, grp.LadoB.Mampara, index, ref tmpEnts,
                        out panelStackAOrigin, out panelStackBOrigin, out biomboOrigin, grp.LadoA.Biombo != null, aPiso, count);
                    if (grp.LadoA.Panels.Count > 0)
                    {
                        this.DrawPaneles(panelStackBOrigin, grp.LadoA.Panels, ref tmpEnts);
                        this.DrawPaneles(panelStackAOrigin, grp.LadoB.Panels, ref tmpEnts);
                    }
                    if (grp.LadoA.Biombo != null)
                        this.DrawBiombo(biomboOrigin, grp.LadoA.Biombo, ref tmpEnts);
                }
                index++;
                rowCenter = rowCenter + new Vector3d(0, rOffSet + 0.2, 0);

                foreach (Entity ent in tmpEnts)
                    ents.Add(ent);
                this.EntitiesGroup.Add(tmpEnts);
            }
            return ents;
        }

        private double DrawMamparas(Point3d rowCenter, string mamparaCode, int index, ref List<Entity> ents,
            out Point3d panelStackAOrigin, out Point3d panelStackB_leftOrigin, out Point3d panelStackB_rightOrigin, bool aPiso, int count, double biomboOffset)
        {
            DBText mamparaTag, heightTag, hEndTagLeft, hEndTagRight, hMiddle, hMiddleTag1, hMiddleTag2, hMiddleTag3, countTag;

            String codes = mamparaCode.Replace(" ", "").Split('/')[0];
            String fCode = codes.Substring(6, 2),
                    aCode = codes.Substring(codes.Length - 3, 2),
                    dFCode = fCode == "18" ? "40" : "52"; ;
            double frente = App.DB.GetRealSize(fCode, true) / 1000,
                   alto = App.DB.GetRealSize(aCode, false) / 1000, mamTagLength;

            Point3d startPoint = rowCenter + new Vector3d(0.4d, 0, 0),
                    endPoint = rowCenter + new Vector3d(0.4d + ANCHO_M + frente * 2, 0, 0);

            mamparaTag = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("{0}", mamparaCode),
                Rotation = 0d,
            };
            mamTagLength = (mamparaTag.GeometricExtents.MaxPoint.X - mamparaTag.GeometricExtents.MinPoint.X);
            mamparaTag.Position = startPoint + new Vector3d(ANCHO_M * 2, -TXT_Y_OFFSET * 2.5d, 0);
            mamparaTag.Position = mamparaTag.Position + new Vector3d(0.128d, 0, 0);


            countTag = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("Cantidad: ({0})", count),
                Rotation = 0d,
                Position = mamparaTag.Position + new Vector3d(ANCHO_M + mamTagLength, 0, 0)
            };
            countTag.Position = new Point3d(countTag.Position.X, mamparaTag.Position.Y, 0);
            heightTag = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("{0}", aCode),
                Rotation = 0d,
                Position = rowCenter + new Vector3d(ANCHO_M * 3, alto / 2, 0)
            };


            hMiddle = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("{0}", dFCode),
                Rotation = 0d,
                Position = rowCenter + new Vector3d(0.4d + ANCHO_M + frente - TXT_X_OFFSET, -TXT_Y_OFFSET, 0)
            };
            hMiddleTag1 = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("A{0}", index),
                Rotation = 0d,
                Position = rowCenter + new Vector3d(0.4d + ANCHO_M + frente - TXT_X_OFFSET, alto + 0.2 + biomboOffset, 0)
            };
            hEndTagLeft = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("{0}", fCode),
                Rotation = 0d,
                Position = endPoint + new Vector3d(0.4d + ANCHO_M + frente / 2 - TXT_X_OFFSET, -TXT_Y_OFFSET, 0)
            };
            hEndTagRight = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("{0}", fCode),
                Rotation = 0d,
                Position = endPoint + new Vector3d(0.4d + ANCHO_M + frente * 1.5, -TXT_Y_OFFSET, 0)
            };
            hMiddleTag2 = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("B{0}", index),
                Rotation = 0d,
                Position = endPoint + new Vector3d(0.4d + ANCHO_M + frente - TXT_X_OFFSET, alto + 0.2, 0)
            };
            hMiddleTag2.Position -= new Vector3d(hMiddleTag2.GeometricExtents.MaxPoint.X - hMiddleTag2.GeometricExtents.MinPoint.X, 0, 0);
            hMiddleTag3 = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("C{0}", index),
                Rotation = 0d,
                Position = endPoint + new Vector3d(0.4d + ANCHO_M + frente + 0.128d - TXT_X_OFFSET, alto + 0.2, 0)
            };
            hMiddleTag3.Position += new Vector3d(hMiddleTag2.GeometricExtents.MaxPoint.X - hMiddleTag2.GeometricExtents.MinPoint.X, 0, 0);
            Point3d startPoint2 = endPoint + new Vector3d(0.4d, 0, 0),
                    endPoint2 = endPoint + new Vector3d(0.4d + ANCHO_M + frente, 0, 0);
            BoundingBox2D h1Start = new BoundingBox2D(startPoint.ToPoint2d() + new Vector2d(ANCHO_M + 0.005, 0), startPoint.ToPoint2d() + new Vector2d(ANCHO_M * 2, 0.128d)),
                          h1End = new BoundingBox2D(endPoint.ToPoint2d() + new Vector2d(-ANCHO_M - 0.005, 0), endPoint.ToPoint2d() + new Vector2d(-0.005, 0.128d)),
                          h2Start = new BoundingBox2D(startPoint2.ToPoint2d() + new Vector2d(ANCHO_M + 0.005, 0), startPoint2.ToPoint2d() + new Vector2d(ANCHO_M * 2, 0.128d)),
                          h2End = new BoundingBox2D(endPoint2.ToPoint2d() + new Vector2d(-ANCHO_M - 0.005, 0), endPoint2.ToPoint2d() + new Vector2d(-0.005, 0.128d));
            //Se agregan las mamparas
            ents.Add(mamparaTag);
            ents.Add(hEndTagLeft);
            ents.Add(hEndTagRight);
            ents.Add(hMiddle);
            ents.Add(hMiddleTag1);
            ents.Add(hMiddleTag2);
            ents.Add(hMiddleTag3);
            ents.Add(heightTag);
            ents.Add(countTag);
            if (!aPiso)
            {
                ents.Add(Drawer.CreatePolyline(h1Start));
                ents.Add(Drawer.CreatePolyline(h1End));
                ents.Add(Drawer.CreatePolyline(h2Start));
                ents.Add(Drawer.CreatePolyline(h2End));
            }
            panelStackAOrigin = startPoint + new Vector3d(ANCHO_M, 0, 0);
            panelStackB_leftOrigin = startPoint2 + new Vector3d(ANCHO_M, 0, 0);
            panelStackB_rightOrigin = endPoint2 + new Vector3d(ANCHO_M, 0, 0);
            return alto + ROW_OFFSET;
        }

        /// <summary>
        /// Realiza el dibujado del biombo de la mampara
        /// </summary>
        /// <param name="origin">El punto base para dibujar el biombo</param>
        /// <param name="biombo">El biombo biombo</param>
        /// <param name="ents">La lista de entidades a dibujar</param>
        private void DrawBiombo(Point3d origin, RivieraBiombo biombo, ref List<Entity> ents)
        {
            DBText panelTag;
            BoundingBox2D panelGEo;
            String code, fCode, aCode;
            double frente, alto;
            origin = origin + new Vector3d(PAN_OFFSET_M, 0, 0);
            code = biombo.Code;
            //Hay dos códigos de mamparas uno de 10 y uno de 12,
            //El de 12 es un frente compuesto por cuatro digitos
            if (code.Length == 10)
            {
                fCode = code.Substring(code.Length - 4, 2);
                aCode = int.Parse(code.Substring(code.Length - 2, 2)).ToString();
            }
            else
            {
                fCode = (int.Parse(code.Substring(code.Length - 6, 2)) +
                        int.Parse(code.Substring(code.Length - 4, 2))).ToString();
                aCode = int.Parse(code.Substring(code.Length - 2, 2)).ToString();
            }
            PanelSize fSize = App.DB.Panel_Size.Where(x => code.Substring(0, 6) == x.Code && x.Frente == fCode).FirstOrDefault(),
                      aSize = App.DB.Panel_Size.Where(x => code.Substring(0, 6) == x.Code && x.Alto == aCode).FirstOrDefault();
            frente = fSize != null ? fSize.Real.Frente / 1000 : 0d;
            alto = aSize != null ? aSize.Real.Alto / 1000 : 0d;
            panelGEo = new BoundingBox2D(origin.ToPoint2d(), origin.ToPoint2d() + new Vector2d(frente, alto));
            panelTag = new DBText()
            {
                Height = TEXT_HEIGHT * 0.7d,
                TextString = String.Format("{0}", biombo.Code.AddAcabado(biombo)),
                Rotation = 0d,
            };
            panelTag.Position = panelGEo.MidPoint.ToPoint3d() - new Vector3d((panelTag.GeometricExtents.MaxPoint.X - panelTag.GeometricExtents.MinPoint.X) / 2,
                (panelTag.GeometricExtents.MaxPoint.Y - panelTag.GeometricExtents.MinPoint.Y) / 2, 0);
            ents.Add(panelTag);
            ents.Add(Drawer.CreatePolyline(panelGEo));
        }

        /// <summary>
        /// Realiza el dibujado de los paneles de la mampara
        /// </summary>
        /// <param name="origin">El punto base para dibujar los paneles</param>
        /// <param name="panels">La colección de paneles</param>
        /// <param name="ents">La lista de entidades a dibujar</param>
        private void DrawPaneles(Point3d origin, Dictionary<double, PanelRaw> panels, ref List<Entity> ents)
        {
            DBText panelTag;
            BoundingBox2D panelGEo;
            String panelCode, fCode, aCode;
            double frente, alto, heightOffset;
            origin = origin + new Vector3d(PAN_OFFSET_M, 0, 0);

            foreach (KeyValuePair<double, PanelRaw> panel in panels)
            {
                panelCode = panel.Value.Code;
                if (panel.Value.Direction == ArrowDirection.Left_Back)
                    heightOffset = 0.1;
                else if (panel.Value.Direction == ArrowDirection.Right_Back)
                    heightOffset = -0.1;
                else
                    heightOffset = 0;
                //Hay dos códigos de mamparas uno de 10 y uno de 12,
                //El de 12 es un frente compuesto por cuatro digitos
                if (panelCode.Length == 10)
                {
                    fCode = panelCode.Substring(panelCode.Length - 4, 2);
                    aCode = int.Parse(panelCode.Substring(panelCode.Length - 2, 2)).ToString();
                }
                else
                {
                    fCode = (int.Parse(panelCode.Substring(panelCode.Length - 6, 2)) +
                            int.Parse(panelCode.Substring(panelCode.Length - 4, 2))).ToString();
                    aCode = int.Parse(panelCode.Substring(panelCode.Length - 2, 2)).ToString();
                }
                PanelSize fSize = App.DB.Panel_Size.Where(x => panelCode.Substring(0, 6) == x.Code && x.Frente == fCode).FirstOrDefault(),
                          aSize = App.DB.Panel_Size.Where(x => panelCode.Substring(0, 6) == x.Code && x.Alto == aCode).FirstOrDefault();
                frente = fSize != null ? fSize.Real.Frente / 1000 : 0d;
                alto = aSize != null ? aSize.Real.Alto / 1000 : 0d;
                //En caso de que el panel no se a piso se dibuja la mampara
                //con desfazamiento con respecto al piso
                if (!panel.Value.APiso && panel.Key == 0)
                    origin = origin + new Vector3d(0, FLOOR_OFFSET, 0);
                panelGEo = new BoundingBox2D(origin.ToPoint2d(), origin.ToPoint2d() + new Vector2d(frente, alto));
                panelTag = new DBText()
                {
                    Height = TEXT_HEIGHT * 0.7d,
                    TextString = String.Format("{0}", panel.Value.Code.AddAcabado(panel.Value)),
                    Rotation = 0d,
                };
                panelTag.Position = panelGEo.MidPoint.ToPoint3d() - new Vector3d((panelTag.GeometricExtents.MaxPoint.X - panelTag.GeometricExtents.MinPoint.X) / 2,
                    (panelTag.GeometricExtents.MaxPoint.Y - panelTag.GeometricExtents.MinPoint.Y) / 2 + heightOffset, 0);
                ents.Add(panelTag);
                ents.Add(Drawer.CreatePolyline(panelGEo));
                origin = origin + new Vector3d(0, alto + PANEL_X_OFFSET, 0);
            }
        }

        /// <summary>
        /// Realiza el dibujo de las mamparas
        /// </summary>
        /// <param name="rowCenter">El punto inicial de la fila</param>
        /// <param name="mamparaCode">El código de la mampara</param>
        /// <param name="index">El indice de la mampra tipo</param>
        /// <param name="ents">La lista en donde se guardan las entidades a dibujar</param>
        /// <param name="panelStackAOrigin">Como salida el punto de inserción para la colección de paneles del lado A</param>
        /// <param name="panelStackBOrigin">Como salida el punto de inserción para la colección de paneles del lado B</param>
        /// <param name="biombioOrigin">Como salida el punto de inserción del biombo</param>
        /// <param name="hasBiombo">Verdadero si la mampara tiene biombo</param>
        /// <param name="aPiso">Checa si la mampara esta a piso, para no dibujar los marcos de la mampara</param>
        /// <param name="count">El número de mamparas del mismo tipo</param>
        /// <returns>El tamaño de la fila de la mampara dibujada</returns>
        private Double DrawMamparas(Point3d rowCenter, String mamparaCode, int index, ref List<Entity> ents,
            out Point3d panelStackAOrigin, out Point3d panelStackBOrigin, out Point3d biombioOrigin, Boolean hasBiombo, Boolean aPiso, int count)
        {
            DBText mamparaTag, heightTag, hEndTag, hMiddle, hMiddleTag1, hMiddleTag2, countTag;
            String fCode = mamparaCode.Substring(mamparaCode.Length - 4, 2),
                   aCode = mamparaCode.Substring(mamparaCode.Length - 2, 2);
            double frente = App.DB.GetRealSize(fCode, true) / 1000,
                   alto = App.DB.GetRealSize(aCode, false) / 1000;
            Double biomboOffset = hasBiombo ? 0.2d : 0;

            Point3d startPoint = rowCenter + new Vector3d(0.4d, 0, 0),
                    endPoint = rowCenter + new Vector3d(0.4d + ANCHO_M + frente, 0, 0);

            mamparaTag = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("{0}S", mamparaCode),
                Rotation = 0d,
            };
            mamparaTag.Position = startPoint + new Vector3d(ANCHO_M * 2, -TXT_Y_OFFSET * 2.5d, 0);
            mamparaTag.Position = mamparaTag.Position + new Vector3d(((frente - 0.077 * 2) - (mamparaTag.GeometricExtents.MaxPoint.X - mamparaTag.GeometricExtents.MinPoint.X)) / 2, 0, 0);


            countTag = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("Cantidad: ({0})", count),
                Rotation = 0d,
                Position = endPoint + new Vector3d(0.4d + ANCHO_M + 0.005, 0, 0)
            };
            countTag.Position = new Point3d(countTag.Position.X, mamparaTag.Position.Y, 0);
            heightTag = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("{0}", aCode),
                Rotation = 0d,
                Position = rowCenter + new Vector3d(ANCHO_M * 3, alto / 2, 0)
            };


            hMiddle = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("{0}", fCode),
                Rotation = 0d,
                Position = rowCenter + new Vector3d(0.4d + ANCHO_M + frente / 2 - TXT_X_OFFSET, -TXT_Y_OFFSET, 0)
            };
            hMiddleTag1 = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("A{0}", index),
                Rotation = 0d,
                Position = rowCenter + new Vector3d(0.4d + ANCHO_M + frente / 2 - TXT_X_OFFSET, alto + 0.2 + biomboOffset, 0)
            };
            hEndTag = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("{0}", fCode),
                Rotation = 0d,
                Position = endPoint + new Vector3d(0.4d + ANCHO_M + frente / 2 - TXT_X_OFFSET, -TXT_Y_OFFSET, 0)
            };
            hMiddleTag2 = new DBText()
            {
                Height = TEXT_HEIGHT,
                TextString = String.Format("B{0}", index),
                Rotation = 0d,
                Position = endPoint + new Vector3d(0.4d + ANCHO_M + frente / 2 - TXT_X_OFFSET, alto + 0.2 + biomboOffset, 0)
            };
            Point3d startPoint2 = endPoint + new Vector3d(0.4d, 0, 0),
                    endPoint2 = endPoint + new Vector3d(0.4d + ANCHO_M + frente, 0, 0);
            BoundingBox2D h1Start = new BoundingBox2D(startPoint.ToPoint2d() + new Vector2d(ANCHO_M + 0.005, 0), startPoint.ToPoint2d() + new Vector2d(ANCHO_M * 2, 0.128d)),
                          h1End = new BoundingBox2D(endPoint.ToPoint2d() + new Vector2d(-ANCHO_M - 0.005, 0), endPoint.ToPoint2d() + new Vector2d(-0.005, 0.128d)),
                          h2Start = new BoundingBox2D(startPoint2.ToPoint2d() + new Vector2d(ANCHO_M + 0.005, 0), startPoint2.ToPoint2d() + new Vector2d(ANCHO_M * 2, 0.128d)),
                          h2End = new BoundingBox2D(endPoint2.ToPoint2d() + new Vector2d(-ANCHO_M - 0.005, 0), endPoint2.ToPoint2d() + new Vector2d(-0.005, 0.128d));
            //Se agregan las mamparas
            ents.Add(mamparaTag);
            ents.Add(hEndTag);
            ents.Add(hMiddle);
            ents.Add(hMiddleTag1);
            ents.Add(hMiddleTag2);
            ents.Add(heightTag);
            ents.Add(countTag);
            if (!aPiso)
            {
                ents.Add(Drawer.CreatePolyline(h1Start));
                ents.Add(Drawer.CreatePolyline(h1End));
                ents.Add(Drawer.CreatePolyline(h2Start));
                ents.Add(Drawer.CreatePolyline(h2End));
            }
            panelStackAOrigin = startPoint + new Vector3d(ANCHO_M, 0, 0);
            panelStackBOrigin = startPoint2 + new Vector3d(ANCHO_M, 0, 0);
            biombioOrigin = startPoint + new Vector3d(ANCHO_M, alto, 0);
            return alto + ROW_OFFSET + biomboOffset;
        }
        /// <summary>
        /// Realiza la selección de elementos a eliminar
        /// </summary>
        /// <returns>Los elementos a eliminar</returns>
        public static Boolean Pick(out List<Mampara> mamparas)
        {
            ObjectIdCollection ids;
            SelectionFilterBuilder fb = new SelectionFilterBuilder(typeof(BlockReference), typeof(Polyline));
            mamparas = new List<Mampara>();
            if (Selector.ObjectIds(MSG_SEL_MAM, out ids, fb.Filter))
            {
                RivieraObject obj;
                foreach (ObjectId id in ids)
                {
                    obj = App.DB[id];
                    if (obj != null && obj is Mampara)
                        mamparas.Add(obj as Mampara);
                }
            }
            return mamparas.Count > 0;
        }
    }
}
