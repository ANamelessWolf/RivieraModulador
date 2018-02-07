using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Controller.Delta;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Model.Dev;
using DaSoft.Riviera.OldModulador.UI;
using DaSoft.Riviera.OldModulador.UI.Delta;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Controller;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
using static DaSoft.Riviera.OldModulador.Runtime.CMD;

namespace DaSoft.Riviera.OldModulador.Runtime.Delta
{
    public class DeltaCommands : MioPlugin
    {

        /// <summary>
        /// El nombre de la paleta
        /// </summary>
        public override string Name
        {
            get
            {
                return "Énfasis Plus";
            }
        }
        /// <summary>
        /// Los nombres de las pestañas de la aplicación
        /// </summary>
        public override String[] Tabs
        {
            get
            {
                if (App.Riviera.DeveloperMode)
                    return new String[]
                    {
                       "Mamparas",
                       "Developer"
                    };
                else
                    return new String[]
                    {
                        "Mamparas"
                    };
            }
        }
        /// <summary>
        /// The controls of the palette
        /// </summary>
        public override UserControl[] Controls
        {
            get
            {
                if (App.Riviera.DeveloperMode)
                    return new UserControl[]
                {
                    new Ctrl_Mampara(),
                    new Ctrl_Developer()
                };
                else
                    return new UserControl[]
                {
                    new Ctrl_Mampara()
                };
            }
        }
        /// <summary>
        /// Initialize the delta
        /// </summary>
        [CommandMethod(DELTA_UI)]
        public override void InitCommand()
        {
            App.RunCommand(
                delegate ()
                {
                    base.InitCommand();
                    try
                    {
                        new FastTransactionWrapper((Document doc, Transaction tr) => { new MamparaValidator().Validate(tr); }).Run();
                    }
                    catch (System.Exception exc)
                    {
                        Selector.Ed.WriteMessage(exc.Message);
                    }
                });
        }
        /// <summary>
        /// Realiza el proceso de inserción de la mampara
        /// </summary>
        [CommandMethod(DELTA_INSERT)]
        public void InsertMampara()
        {
            App.RunCommand(delegate ()
            {
                RivieraSize size = (this[Tabs[0]] as Ctrl_Mampara).Size;
                String code = (this[Tabs[0]] as Ctrl_Mampara).Code;
                this.Print(size.ToString());
                Mampara mampara;
                if (MamparaSower.InsertMampara(size, code, out mampara))
                {
                    MamparaSower sower = new MamparaSower((this[Tabs[0]] as Ctrl_Mampara), mampara);
                    sower.Sow();
                }
            });
        }



        /// <summary>
        /// Continua el proceso de inserción de la mampara
        /// </summary>
        [CommandMethod(DELTA_CONTINUE_INSERT)]
        public void ContinueMampara()
        {
            App.RunCommand(delegate ()
            {
                MamparaSelector mamSel = new MamparaSelector((this[Tabs[0]] as Ctrl_Mampara));
                if (mamSel.Pick())
                    mamSel.Sow();
            });
        }
        /// <summary>
        /// Elima uno o más elementos insertados con la aplicación
        /// </summary>
        [CommandMethod(DELTA_REMOVE)]
        public void RemoveMampara()
        {
            App.RunCommand(delegate ()
            {
                MamparaEraser eraser = new MamparaEraser();
                if (eraser.Pick())
                {
                    eraser.Erase();
                    Validate();
                }
            });
        }
        /// <summary>
        /// Realiza el cambio entre una y otra mampara
        /// </summary>
        [CommandMethod(DELTA_CHANGE_MAMPARA)]
        public void ChangeMampara()
        {
            App.RunCommand(delegate ()
            {
                MamparaSwaper mamSel = new MamparaSwaper((this[Tabs[0]] as Ctrl_Mampara));
                Mampara mampara;
                if (mamSel.Pick(out mampara))
                {
                    mamSel.Swap(mampara);
                    Validate();
                }
            });
        }

        [CommandMethod(DELTA_SNOOP_MAMPARA)]
        public void SnoopMampara()
        {
            App.RunCommand(delegate ()
            {
                ObjectId id;
                if (Selector.ObjectId(MSG_SEL_OBJ, out id, typeof(BlockReference), typeof(Polyline), typeof(Line)))
                {
                    Entity ent = id.OpenEntity();
                    if (ent is Line)
                    {
                        RivieraObject obj = App.DB[ent.Handle.Value];
                        Selector.Ed.SetImpliedSelection(obj.Ids.OfType<ObjectId>().ToArray());
                        id = obj.Ids[obj.Ids.Count - 1];
                    }
                        (this[Tabs[0]] as Ctrl_Mampara).CurrentObject = App.DB[id];
                }
            });
        }

        [CommandMethod(DELTA_CRETE_PANEL_TEMPLATE)]
        public void CreatePanelTemplate()
        {
            App.RunCommand(delegate ()
            {
                RivieraSize size = (this[Tabs[0]] as Ctrl_Mampara).Size;
                String code = (this[Tabs[0]] as Ctrl_Mampara).Code;
                Mampara mampara = new Mampara(new Point2d(0, 0), new Point2d(10, 0), size, code);
                PanelBuilder pB = new PanelBuilder(mampara);
                List<PanelRaw> stack;
                PanelRaw biombo;
                if (pB.SummonPanelEditor(mampara, out stack, out biombo))
                {
                    Ctrl_Mampara.StackPanelTemplate = stack;
                    Ctrl_Mampara.BiomboTemplate = biombo;
                    Ctrl_Mampara.MamparaTemplate = mampara;
                }
                else
                {
                    Ctrl_Mampara.StackPanelTemplate = null;
                    Ctrl_Mampara.BiomboTemplate = null;
                    Ctrl_Mampara.MamparaTemplate = null;
                }
            });
        }

        [CommandMethod(DELTA_COPY_PASTE_PANELS)]
        public void CopyPastePanels()
        {
            App.RunCommand(delegate ()
            {
                PanelBuilder.Copy();
            });
        }


        [CommandMethod(DELTA_EDIT_PANELS)]
        public void EditPanels()
        {
            App.RunCommand(delegate ()
            {
                PanelBuilder.EditPanels();
            });
        }

        [CommandMethod(DELTA_EDIT_PANELS_DOUBLE)]
        public void EditDoublePanels()
        {
            App.RunCommand(delegate ()
            {
                PanelBuilder.EditDoublePanels();
            });
        }

        /// <summary>
        /// Realiza un proceso rápido de validación
        /// </summary>
        [CommandMethod(DELTA_VALIDATE_REMATES)]
        public void Validate()
        {
            FastTransactionWrapper trw = new FastTransactionWrapper(
              delegate (Document doc, Transaction tr)
              {
                  MamparaValidator val = new MamparaValidator();
                  val.Validate(tr);
              });
            trw.Run();
        }

        /// <summary>
        /// Realiza un proceso rápido de validación
        /// </summary>
        [CommandMethod(REGEN_IDS_ENDED)]
        public void RegenEnded()
        {
            FastTransactionWrapper trw = new FastTransactionWrapper(
              delegate (Document doc, Transaction tr)
              {
                  this.CleanGeometry(tr);
                  MamparaValidator val = new MamparaValidator();
                  val.Validate(tr);
              });
            trw.Run();
        }

        Dialog_Quantification qPre;
        [CommandMethod(DELTA_QUANTIFY)]
        public void Quantify()
        {
            App.RunCommand(delegate ()
            {
                qPre = new Dialog_Quantification();
                if (qPre.QMan.Erroneous_Items.Count == 0)
                    qPre.Show();
                else
                {
                    Dialog_MessageBox.Show(ERR_QERRORS, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    new FastTransactionWrapper(
                        delegate (Document doc, Transaction tr)
                        {
                            AutoCADLayer errLayer = new AutoCADLayer(LAYER_ERR, Autodesk.AutoCAD.Colors.Color.FromRgb(255, 255, 0), tr);
                            errLayer.SetStatus(LayerStatus.EnableStatus, tr);
                            ObjectIdCollection ids = new ObjectIdCollection();
                            Polygon2D pol;
                            double minValue = 0.3d;
                            if (App.Riviera.Units == DaNTeUnits.Imperial)
                                minValue = RivieraExtender.ConvertUnits(minValue, Unit_Type.m, Unit_Type.inches);
                            foreach (RivieraObject obj in qPre.QMan.Erroneous_Items)
                            {
                                pol = new Polygon2D(8, obj.Start.MiddlePointTo(obj.End), obj.Line.Length > minValue ? obj.Line.Length : minValue);
                                ids.Add(Drawer.Geometry2D(pol, tr));
                            }
                            errLayer.AddToLayer(ids, tr);
                        }).Run();
                }
            });
        }

        [CommandMethod(DELTA_INSERT_I)]
        public void InsertI()
        {
            App.RunCommand(delegate ()
            {
                ObjectId id;
                if (Selector.ObjectId(MSG_SEL_OBJ, out id, typeof(BlockReference)))
                {
                    Mampara mampara = App.DB[id] as Mampara;
                    if (mampara != null)
                    {
                        MamparaIUpgrader m = new MamparaIUpgrader(mampara);
                        if (m.IsIMampara)
                        {
                            RivieraSize size = (this[Tabs[0]] as Ctrl_Mampara).Size;
                            String code = (this[Tabs[0]] as Ctrl_Mampara).Code;
                            m.UpgradeIToT();
                            if (m.Joint.GetType() == typeof(MamparaJoint))
                            {
                                m.InsertT(size, code);
                                MamparaSelector mamSel = new MamparaSelector((this[Tabs[0]] as Ctrl_Mampara));
                                mamSel.SelectedId = m.Mampara.Ids[1];
                                mamSel.Sow();
                            }
                        }
                    }
                }

            });
        }

        [CommandMethod(DELTA_CLEAR_CACHE)]
        public void CleanCache()
        {
            App.RunCommand(delegate ()
            {
                qPre.QMan = new Quantifier();
                qPre.tableElements.List.Items.Clear();
                qPre.tableUnion.List.Items.Clear();
                qPre.tableElements_Fill(qPre.tableElements, null);
                qPre.tableUnion_Fill(qPre.tableUnion, null);
            });
        }
        [CommandMethod(DELTA_MAMPARA_REPORT_TIPO)]
        public void ReporteMamparaTipo()
        {
            App.RunCommand(delegate ()
            {
                List<Mampara> mamparas;
                Point3d insPt;
                if (MamparaReportType.Pick(out mamparas) && Selector.Point(MSG_SEL_MAM_REP_TIPO, out insPt))
                {
                    MamparaReportType mRep = new MamparaReportType(mamparas, insPt);
                    mRep.Quantify();
                    mRep.Draw();
                }
            });
        }

        [CommandMethod("SelectGeometry")]
        public void SelectGeom()
        {
            App.RunCommand(delegate ()
            {
                ObjectId id;
                if (Selector.ObjectId(MSG_SEL_OBJ, out id, typeof(BlockReference), typeof(Polyline)))
                {
                    ObjectIdCollection ids = new ObjectIdCollection();
                    Selector.Ed.SetImpliedSelection(App.DB[id].FindGeometry(true).OfType<ObjectId>().ToArray());
                }
            });
        }
        #region DevCommands
        [CommandMethod(DEV_SNOOP_DATA)]
        public void SnoopRivieraData()
        {
            App.RunCommand(delegate ()
            {
                ObjectId id;
                if (Selector.ObjectId(MSG_SEL_OBJ, out id, typeof(BlockReference), typeof(Polyline), typeof(Line)))
                {
                    Ctrl_Developer devWin = (this[Tabs[1]] as Ctrl_Developer);
                    RivieraObject obj =
                    new BlankTransactionWrapper<RivieraObject>(
                            delegate (Document doc, Transaction tr)
                        {
                            RivieraObject rivObj;
                            Entity ent = id.OpenEntity(tr);
                            if (ent is Line)
                                rivObj = App.DB[ent.Handle.Value];
                            else
                                rivObj = App.DB[id];
                            ExtensionDictionaryManager dMan = new ExtensionDictionaryManager(rivObj != null ? rivObj.Line.Id : ent.Id, tr);
                            List<SnoopRowItem> items = new List<SnoopRowItem>();
                            String key;
                            Boolean isDoubleMampara = (rivObj is MamparaJoint) && (rivObj as MamparaJoint).HasDoublePanels;
                            foreach (var entry in (rivObj != null ? rivObj.Line : ent as Line).ExtensionDictionary.GetObject(OpenMode.ForRead) as DBDictionary)
                            {
                                key = entry.m_key;
                                String[] data = dMan.GetRegistry(key, tr).GetDataAsString(tr);
                                long h;
                                if (data.Length > 0 && long.TryParse(data[0], out h))
                                {
                                    if (key != FIELD_GEOMETRY && key != FIELD_REG_UNITS && h > 0)
                                    {
                                        String rowName;
                                        if (entry.m_key == "Extra" && devWin.Code.Contains("DD1"))
                                            rowName = "Biombo";
                                        else if (entry.m_key.Contains("Left"))
                                            rowName = "Left";
                                        else if (entry.m_key.Contains("Right"))
                                            rowName = "Right";
                                        else
                                            rowName = entry.m_key;
                                        items.Add(new SnoopRowItem()
                                        {
                                            XRecordName = rowName,
                                            Value = data[0],
                                            IsXRecord = true,
                                        });
                                    }
                                }
                                else if (entry.m_key == FIELD_CODE)
                                {
                                    devWin.Code = data[0];
                                    if (devWin.Code == PANEL_STACK || devWin.Code.Contains("DD8") ||
                                        devWin.Code.Contains("DD7") || devWin.Code.Contains("DD3"))
                                    {
                                        devWin.locationFinder.Children.Clear();
                                        Button btn = new Button();
                                        btn.Content = "Parent";
                                        btn.Click += devWin.FindElement_Click;
                                        devWin.locationFinder.Children.Add(btn);
                                    }
                                    else if (devWin.Code.Contains("DD1010"))
                                    {
                                        devWin.locationFinder.Children.Clear();
                                        String[] names = new String[]
                                        {
                                            "Same",
                                            "Parent",
                                            "Back",
                                            "Front",
                                            "Left",
                                            "Right"
                                        };
                                        Button btn;
                                        for (int i = 0; i < names.Length; i++)
                                        {
                                            btn = new Button();
                                            btn.Content = names[i];
                                            btn.Click += devWin.FindElement_Click;
                                            devWin.locationFinder.Children.Add(btn);
                                        }
                                    }
                                }
                                else if (devWin.Code == JOINT || devWin.Code == DT_JOINT)
                                {
                                    devWin.locationFinder.Children.Clear();
                                    String[] names;
                                    if (isDoubleMampara)
                                        names = new String[]
                                    {
                                            "Parent",
                                            "Back",
                                            "Front",
                                            "Left",
                                            "Right",
                                            "DoubleFront",
                                            "DoubleBack",
                                            "DoubleLeft",
                                            "DoubleRight",
                                    };
                                    else
                                        names = new String[]
                                    {
                                            "Parent",
                                            "Back",
                                            "Front",
                                            "Left",
                                            "Right"
                                    };
                                    Button btn;
                                    for (int i = 0; i < names.Length; i++)
                                    {
                                        btn = new Button();
                                        btn.Content = names[i];
                                        btn.Click += devWin.FindElement_Click;
                                        devWin.locationFinder.Children.Add(btn);
                                    }
                                }
                                else if (entry.m_key == FIELD_END || entry.m_key == FIELD_START)
                                {
                                    Point2d pt = data[0].ParseAsPoint();

                                    items.Add(new SnoopRowItem()
                                    {
                                        XRecordName = entry.m_key,
                                        Value = pt.ToFormat(2, true),
                                        IsCoordinate = true,
                                    });
                                }



                            }
                            devWin.Fill(items);
                            devWin.Handle = rivObj.Line.Handle.Value;
                            return rivObj;
                        }).Run();
                    if (obj != null)
                    {
                        Selector.Ed.SetImpliedSelection(obj.Ids.OfType<ObjectId>().ToArray());

                    }
                }
            });
        }

        [CommandMethod(DEV_FIND_ELEMENT)]
        public void FindElement()
        {
            App.RunCommand(delegate ()
            {
                ObjectId id;
                Ctrl_Developer devWin = (this[Tabs[1]] as Ctrl_Developer);
                new FastTransactionWrapper(
                        delegate (Document doc, Transaction tr)
                        {
                            id = devWin.Handle.GetId();
                            Entity ent = id.OpenEntity(tr);
                            RivieraObject rivObj = App.DB[ent.Handle.Value];
                            ElementFinder finder = new ElementFinder(rivObj.Code, rivObj.Line, rivObj.Children, rivObj.Parent);
                            if (rivObj is MamparaJoint && (rivObj as MamparaJoint).HasDoublePanels)
                            {
                                Mampara left, right, top;
                                //Se realiza el proceso de selección de la mampara
                                RivieraPanelDoubleLocation.FindMampara(rivObj as MamparaJoint, out left, out right, out top);
                                RivieraPanelDoubleLocation location =
                                    new RivieraPanelDoubleLocation(RivieraPanelDoubleLocation.PickFrente(left), top, left, right, rivObj as MamparaJoint);
                                finder.PanelDoubleLocation = location;
                            }
                            id = finder.Find(devWin.Direction);
                            // finder.Draw(tr);
                            if (id.IsValid)
                                Ed.SetImpliedSelection(App.DB[(id.GetObject(OpenMode.ForRead) as Entity).Handle.Value].Ids.OfType<ObjectId>().ToArray());
                        }).Run();
            });
        }


        [CommandMethod(DEV_VALIDATE_CONNECTION)]
        public void ValidateConnection()
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            SelectionFilterBuilder filter = new SelectionFilterBuilder(typeof(Line));
            if (Selector.ObjectIds("Selecciona los grupos a validar", out ids, filter.Filter))
            {
                new FastTransactionWrapper(delegate (Document doc, Transaction tr)
                {
                    Line line;
                    Dictionary<long, Line> foundRivieraItems = new Dictionary<long, Line>();
                    foreach (ObjectId lineIds in ids)
                    {
                        line = lineIds.GetObject(OpenMode.ForRead) as Line;
                        if (App.DB[line.Handle.Value] != null)
                            foundRivieraItems.Add(line.Handle.Value, line);
                    }
                    RivieraObject rivObj;
                    IEnumerable<long> pointers;
                    foreach (var item in foundRivieraItems)
                    {
                        try
                        {
                            rivObj = App.DB[item.Key];
                            pointers = rivObj.Children.Values.Union(new long[] { rivObj.Parent });
                            if (rivObj is Mampara)
                                pointers = pointers.Union(new long[] { (rivObj as Mampara).BiomboId });
                            pointers = pointers.Where(x => x != 0);
                            IEnumerable<long> except;
                            ObjectId foundId;
                            long foundHandle;
                            //El elemento esta mal
                            if (pointers.Intersect(foundRivieraItems.Keys).Count() != pointers.Count())
                            {
                                except = pointers.Except(pointers.Intersect(foundRivieraItems.Keys));
                                ElementFinder finder = new ElementFinder(rivObj.Code, foundRivieraItems[item.Key], rivObj.Children, rivObj.Parent);
                                foreach (long handle in except)
                                {
                                    if (handle == rivObj.Parent)
                                    {
                                        foundId = finder.Find(PointerDirection.Parent);
                                        if (foundId.IsValid)
                                        {
                                            foundHandle = foundId.GetObject(OpenMode.ForRead).Handle.Value;
                                            rivObj.Data.Set(FIELD_PARENT, tr, foundHandle.ToString());
                                            rivObj.Parent = foundHandle;
                                        }
                                        else
                                            Selector.Ed.WriteMessage("Error al encontrar el padre de {0}\n[{1}{2}]", rivObj.Code, rivObj.Start, rivObj.End);
                                    }
                                    else if ((rivObj is Mampara) && handle == (rivObj as Mampara).BiomboId)
                                    {
                                        foundId = finder.Find(PointerDirection.Same);
                                        if (foundId.IsValid)
                                        {
                                            foundHandle = foundId.GetObject(OpenMode.ForRead).Handle.Value;
                                            (rivObj as Mampara).BiomboId = foundHandle;
                                            rivObj.Data.Set(FIELD_EXTRA, tr, (rivObj as Mampara).Extra);
                                        }
                                        else
                                            Selector.Ed.WriteMessage("Error al encontrar el biombo de la mampara {0}\n[{1}{2}]", rivObj.Code, rivObj.Start, rivObj.End);
                                    }
                                    else
                                    {
                                        String key = rivObj.Children.Where(x => x.Value == handle).FirstOrDefault().Key;
                                        String[] keys = new String[] { FIELD_BACK, FIELD_FRONT, FIELD_LEFT_BACK, FIELD_LEFT_FRONT, FIELD_RIGHT_BACK, FIELD_RIGHT_FRONT };
                                        PointerDirection[] dir = new PointerDirection[] { PointerDirection.Back, PointerDirection.Front, PointerDirection.Left, PointerDirection.Left, PointerDirection.Right, PointerDirection.Right };
                                        foundId = finder.Find(dir[keys.ToList().IndexOf(key)]);
                                        if (foundId.IsValid)
                                        {
                                            foundHandle = foundId.GetObject(OpenMode.ForRead).Handle.Value;
                                            rivObj.Data.Set(key, tr, foundHandle.ToString());
                                            rivObj.Children[key] = foundHandle;
                                        }
                                        else
                                            Selector.Ed.WriteMessage("Error al encontrar el children {3} del {0}\n[{1}{2}]", rivObj.Code, rivObj.Start, rivObj.End, key);

                                    }
                                }
                            }
                        }
                        catch (System.Exception exc)
                        {
                            App.Riviera.Log.AppendEntry(String.Format("{0}\nError al encontrar el elemento asociado de {2}\n[{3}{4}]",
                                exc.Message, item.Key, foundRivieraItems[item.Key].StartPoint, foundRivieraItems[item.Key].EndPoint), NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, "ValidateConnection", "DEV");
                        }
                    }
                }).Run();
            }
        }
        private void CleanGeometry(Transaction tr)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            App.DB.Objects.Select(x => x.Ids).ToList().ForEach(coll => coll.OfType<ObjectId>().ToList().ForEach(id => ids.Add(id)));
            BlockTableRecord model = Application.DocumentManager.MdiActiveDocument.Database.CurrentSpaceId.GetObject(OpenMode.ForRead) as BlockTableRecord;
            DBObject dbObj;
            Entity ent;
            String[] appLayer = new String[] { LAYER_ERR, LAYER_RIVIERA_GEOMETRY, LAYER_RIVIERA_OBJECT, LAYER_RIVIERA_REPORT };
            foreach (ObjectId objId in model)
            {
                try
                {
                    if (!ids.Contains(objId))
                    {
                        dbObj = objId.GetObject(OpenMode.ForWrite);
                        if (dbObj is Entity)
                        {
                            ent = dbObj as Entity;
                            if (ent.Id.IsValid && !ent.IsErased && appLayer.Contains(ent.Layer))
                                ent.Erase();
                        }
                    }
                }
                catch (System.Exception exc)
                {
                    Selector.Ed.WriteMessage("Error al borrar un elemento de la aplicación: ", exc.Message);
                }
            }
        }

        #endregion
    }
}

