using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Query;
using DaSoft.Riviera.OldModulador.UI;
using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Misa.Model;
using NamelessOld.Libraries.DB.Tessa;
using NamelessOld.Libraries.DB.Tessa.Model;
using NamelessOld.Libraries.Yggdrasil.Lain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Runtime.DaNTe
{
    public class DaNTe_Transaction
    {
        const string WORKING_TAG = "DANTE_TRANSACTION";

        DaNTeQuantification Quantification;
        AccessConnectionContent AccessConnection;
        BackgroundWorker bgWorker;
        Dialog_Loader win = new Dialog_Loader(MSG_DANTE_WORKING);
        Boolean GroupedQuantification;
        Boolean MergeQuantification;
        static int CurrentProgress { get { return (int)((double)Current / (double)Total * 100); } }
        static int Total;
        static int Current;



        /// <summary>
        /// Create a new DaNTe Transaction
        /// </summary>
        /// <param name="q">The quantification</param>
        /// <param name="gQ">Verdadero para realizar una cuantificación agrupada</param>
        /// <param name="mQ">Verdadero para una cuantifiación anexada</param>
        public DaNTe_Transaction(DaNTeQuantification q, Boolean gQ, Boolean mQ)
        {
            Current = 0;
            this.Quantification = q;
            this.GroupedQuantification = gQ;
            this.MergeQuantification = mQ;
            if (q.Mode == QuantificationMode.Access)
            {
                this.AccessConnection = new AccessConnectionContent(q.AccessFile);
                win.Show();
                bgWorker = new BackgroundWorker();
                bgWorker.WorkerReportsProgress = true;
                bgWorker.DoWork += AccessBG_DoWork;
                bgWorker.ProgressChanged += bg_Changes;
                bgWorker.RunWorkerCompleted += bg_WorkIsComplete;
                bgWorker.WorkerReportsProgress = true;
            }
            else if (q.Mode == QuantificationMode.ORACLE)
            {
                win.Show();
                bgWorker = new BackgroundWorker();
                bgWorker.WorkerReportsProgress = true;
                bgWorker.DoWork += OracleBG_DoWork;
                bgWorker.ProgressChanged += bg_Changes;
                bgWorker.RunWorkerCompleted += bg_WorkIsComplete;
                bgWorker.WorkerReportsProgress = true;
            }

        }

        public void StartTransaction()
        {
            ConnectionData connData;
            if (this.Quantification.Mode == QuantificationMode.Access)
                connData = new ConnectionData(App.Riviera.Key, App.Riviera.IV, App.Riviera.AccessConnectionFile, this.AccessConnection);
            else
                connData = App.Riviera.Connection;
            bgWorker.RunWorkerAsync(new Object[] { this.Quantification, connData });
        }

        private void bg_WorkIsComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            win.AllowClose = true;
            win.Close();
            if (e.Result != null)
                Dialog_MessageBox.Show(e.Result as String, MessageBoxButton.OK, MessageBoxImage.Information);
            else
                Dialog_MessageBox.Show(MSG_TRANSACTION_FINISHED, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void bg_Changes(object sender, ProgressChangedEventArgs e)
        {
            win.Update(e.ProgressPercentage);
        }

        private void AccessBG_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Object[] input = e.Argument as Object[];
                DaNTeQuantification qDaNTe = input[0] as DaNTeQuantification;
                ConnectionData connData = input[1] as ConnectionData;
                connData.CreateConnectionFile();

                using (Access_Connector conn = new Access_Connector(connData))
                {
                    App.Riviera.Log.AppendEntry("Inicia Transacción", Protocol.Ok, "StartTransaction", WORKING_TAG);
                    StartAccessWork(qDaNTe.QName, qDaNTe.QZone, qDaNTe.QComments, conn, qDaNTe.Quantification);
                    App.Riviera.Log.AppendEntry("Inicia Transacción", Protocol.Ok, "StartTransaction", WORKING_TAG);
                }
            }
            catch (System.Exception exc)
            {
                String msg = exc.InnerException != null ? exc.InnerException.Message : exc.Message;
                App.Riviera.Log.AppendEntry(msg, Protocol.Error, "bg_DoWork", "Abort");
                bgWorker.ReportProgress(100);
                string extErr = String.Empty;
                if (exc is System.Runtime.InteropServices.ExternalException)
                    extErr = String.Format("Code: {0}, {1}", ((System.Runtime.InteropServices.ExternalException)exc).ErrorCode, ((System.Runtime.InteropServices.ExternalException)exc).Message);
                e.Result = String.Format("{0}\n{1}\n{2}", exc.Message, exc.InnerException != null ? exc.InnerException.Message : String.Empty, extErr);
            }

        }
        private void OracleBG_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Object[] input = e.Argument as Object[];
                DaNTeQuantification qDaNTe = input[0] as DaNTeQuantification;
                ConnectionData connData = input[1] as ConnectionData;
                using (Oracle_Connector conn = new Oracle_Connector(connData))
                {
                    try
                    {
                        App.Riviera.Log.AppendEntry("Inicia Transacción", Protocol.Ok, "StartTransaction", WORKING_TAG);
                        StartOracleWork(App.Riviera.Credentials, qDaNTe.QName, qDaNTe.QZone, qDaNTe.QComments, conn, qDaNTe.Quantification);
                        App.Riviera.Log.AppendEntry("Inicia Transacción", Protocol.Ok, "StartTransaction", WORKING_TAG);
                    }
                    catch (Exception exc)
                    {
                        conn.Dispose();
                        throw new Exception(exc.Message);
                    }
                }
            }
            catch (System.Exception exc)
            {
                String msg = exc.InnerException != null ? exc.InnerException.Message : exc.Message;
                App.Riviera.Log.AppendEntry(msg, Protocol.Error, "bg_DoWork", "Abort");
                bgWorker.ReportProgress(100);
                e.Result = String.Format("{0}\n{1}", exc.Message, exc.InnerException != null ? exc.InnerException.Message : String.Empty);
            }
        }

        private void StartOracleWork(UserCredential cred, string qName, string qZone, string qComments, Oracle_Connector conn, Quantifier data)
        {
            Query_Bases qBases = new Query_Bases();
            if (!MergeQuantification)
                conn.Update(qBases.CleanBases(cred.ProjectId.ToString(), qZone));
            if (conn.QuerySucced)
            {
                if (this.GroupedQuantification)
                {
                    Dictionary<String, int> claveCantidad = new Dictionary<string, int>();
                    //Elementos Individuales
                    foreach (var item in data.ItemQuantification)
                        if (item.Zone == qZone || qZone.ToUpper() == "TODO")
                        {
                            if (claveCantidad.ContainsKey(item.Code))
                                claveCantidad[item.Code] += item.Count;
                            else
                                claveCantidad.Add(item.Code, item.Count);
                        }
                    //Elementos de uniones
                    String[] codes = data.ItemQuantification.Select<QuantifiableObject, String>(y => y.Code).ToArray();
                    //Se excluyen los elementos cuantificados de forma individual
                    foreach (var item in data.UnionQuantification)
                        if (item.ZoneName == qZone || qZone.ToUpper() == "TODO")
                        {
                            foreach (var subItem in item.Members.Where(x => !codes.Contains(x)))
                            {
                                if (claveCantidad.ContainsKey(subItem))
                                    claveCantidad[subItem] += item.Count;
                                else
                                    claveCantidad.Add(subItem, item.Count);
                            }
                        }
                    Total = claveCantidad.Count;
                    //1: Creación de la cuantificación de DaNTe
                    this.CreateDaNTeQuantificationOracle(cred, qName, qZone, conn, claveCantidad);
                }
                else
                {
                    List<Tuple<String, int, long>> claveCantidad = new List<Tuple<String, int, long>>();
                    //Elementos Individuales
                    foreach (var item in data.ItemQuantification)
                        if (item.Zone == qZone || qZone.ToUpper() == "TODO")
                            claveCantidad.Add(new Tuple<string, int, long>(item.Code, item.Count, item.Handle));
                    //Elementos de uniones
                    String[] codes = data.ItemQuantification.Select<QuantifiableObject, String>(y => y.Code).ToArray();
                    //Se excluyen los elementos cuantificados de forma individual
                    foreach (var item in data.UnionQuantification)
                        if (item.ZoneName == qZone || qZone.ToUpper() == "TODO")
                        {
                            //foreach (var subItem in item.Members.Where(x => !codes.Contains(x)))
                            //    claveCantidad.Add(new Tuple<string, int, long>(subItem, item.Count, item.Handle));
                            foreach (var subItem in item.Members.Where(x => !codes.Contains(x)))
                            {
                                if (claveCantidad.Count(x => x.Item1 == subItem && x.Item3 == item.Handle) > 0)
                                {
                                    Tuple<string, int, long> current = claveCantidad.Where(x => x.Item1 == subItem && x.Item3 == item.Handle).First();
                                    int t = current.Item2 + 1;
                                    claveCantidad.Remove(current);
                                    claveCantidad.Add(new Tuple<string, int, long>(subItem, t, item.Handle));
                                }
                                else
                                    claveCantidad.Add(new Tuple<string, int, long>(subItem, item.Count, item.Handle));
                            }
                        }
                    claveCantidad = claveCantidad.Where(x => x.Item2 > 0).ToList();
                    Total = claveCantidad.Count;
                    //1: Creación de la cuantificación de DaNTe
                    this.CreateDaNTeQuantificationOracle(cred, qName, qZone, conn, claveCantidad);
                    //2: Se crea la cuantificación con asociados
                    this.CreateAsociadosQuantificationOracle(cred, qName, qZone, conn, claveCantidad);
                }
            }
        }



        private void StartAccessWork(String qName, String qZone, String qComments, Access_Connector conn, Quantifier data)
        {
            //1: Preparación de las tablas para la cuantificación
            this.ValidateTables(qName, qComments, conn);
            if (this.GroupedQuantification)
            {
                Dictionary<String, int> claveCantidad = new Dictionary<string, int>();
                //Elementos Individuales
                foreach (var item in data.ItemQuantification.Union(data.ModuleQuantification))
                    if (item.Zone == qZone || qZone.ToUpper() == "TODO")
                    {
                        if (claveCantidad.ContainsKey(item.Code))
                            claveCantidad[item.Code] += item.Count;
                        else
                            claveCantidad.Add(item.Code, item.Count);
                    }
                //Elementos de uniones
                String[] alwayQCodes = new string[] { "DD106024S" };
                String[] codes = data.ItemQuantification.Select<QuantifiableObject, String>(y => y.Code).ToArray();
                //Se excluyen los elementos cuantificados de forma individual
                foreach (var item in data.UnionQuantification)
                    if (item.ZoneName == qZone || qZone.ToUpper() == "TODO")
                    {
                        foreach (var subItem in item.Members.Where(x => !codes.Contains(x)))
                        {
                            if (claveCantidad.ContainsKey(subItem))
                                claveCantidad[subItem] += item.Count;
                            else
                                claveCantidad.Add(subItem, item.Count);
                        }
                    }

                Total = claveCantidad.Count;
                //1: Creación de la cuantificación de DaNTe
                this.CreateDaNTeQuantificationAccess(qName, qZone, conn, claveCantidad);
            }
            else
            {
                List<Tuple<String, int, long>> claveCantidad = new List<Tuple<String, int, long>>();
                //Elementos Individuales
                foreach (var item in data.ItemQuantification.Union(data.ModuleQuantification))
                    if (item.Zone == qZone || qZone.ToUpper() == "TODO")
                        claveCantidad.Add(new Tuple<string, int, long>(item.Code, item.Count, item.Handle));
                //Elementos de uniones
                //Códigos que siempre se cuantifican
                String[] alwayQCodes = new string[] { "DD106024S" };
                String[] codes = data.ItemQuantification.Select<QuantifiableObject, String>(y => y.Code).Where(c => !alwayQCodes.Contains(c)).ToArray();
                //Se excluyen los elementos cuantificados de forma individual
                foreach (var item in data.UnionQuantification)
                    if (item.ZoneName == qZone || qZone.ToUpper() == "TODO")
                    {
                        //foreach (var subItem in item.Members.Where(x => !codes.Contains(x)))
                        //    claveCantidad.Add(new Tuple<string, int, long>(subItem, item.Count, item.Handle));
                        foreach (var subItem in item.Members.Where(x => !codes.Contains(x)))
                        {
                            if (claveCantidad.Count(x => x.Item1 == subItem && x.Item3 == item.Handle) > 0)
                            {
                                Tuple<string, int, long> current = claveCantidad.Where(x => x.Item1 == subItem && x.Item3 == item.Handle).First();
                                int t = current.Item2 + 1;
                                claveCantidad.Remove(current);
                                claveCantidad.Add(new Tuple<string, int, long>(subItem, t, item.Handle));
                            }
                            else
                                claveCantidad.Add(new Tuple<string, int, long>(subItem, item.Count, item.Handle));
                        }
                    }

                Total = claveCantidad.Count;
                claveCantidad = claveCantidad.Where(x => x.Item2 > 0).ToList();
                //1: Creación de la cuantificación de DaNTe
                this.CreateDaNTeQuantificationAccess(qName, qZone, conn, claveCantidad);
                //2: Se crea la cuantificación con asociados
                this.CreateAsociadosQuantificationAccess(qName, qZone, claveCantidad, conn);
            }

        }

        private void CreateAsociadosQuantificationAccess(string qName, string qZone, List<Tuple<string, int, long>> claveCantidad, Access_Connector qConn)
        {
            if (App.Riviera.AsociadoMDB != null)
            {
                //Conexión al MDB de Asociados
                AccessConnectionBuilder aBuilder = new AccessConnectionBuilder()
                {
                    Access_DB_File = App.Riviera.AsociadoMDB.FullName,
                    OleDbProvider = new AccessProvider(OledbProviders.Microsoft_ACE_OleDb_12),
                };
                new VoidAccess_Transaction<object>(new AccessConnectionContent(aBuilder.Data).GenerateConnectionString(),
                    delegate (Access_Connector conn, Object[] trParameters)
                {
                    //Se insertan los registros en la BD de DaNTe

                    String tableName = qName;
                    DaNTeRow dRow;

                    foreach (var row in claveCantidad.OrderBy<Tuple<string, int, long>, String>(x => x.Item1))
                    {
                        dRow = new DaNTeRow(row.Item2, row.Item1) { Handle = String.Format("{0:x}", row.Item3) };
                        if (dRow.HasAsociados(conn))
                        {
                            List<AsociadoRow> asocRows = GetAsociados(conn, dRow.Clave, dRow.Value);
                            Query_BasesMDB query = new Query_BasesMDB();
                            foreach (var asocRow in asocRows)
                                qConn.Update(query.InsertAsociadoRow(qName, asocRow, String.Format("{0:x}", dRow.Handle)));
                        }
                    }
                }).Run();
            }
            else
                NamelessOld.Libraries.HoukagoTeaTime.Yui.Selector.Ed.WriteMessage("No se declararon asociados, falta definir tabla de asociados. Realizar el cambio en configuraciones de la aplicación.");
        }
        /// <summary>
        /// Obtiene los asociados de una lista de códigos
        /// </summary>
        /// <param name="claveCantidad">Clave y cantidad de los códigos a cuantificar</param>
        /// <returns>La lista de asociados</returns>
        public static List<AsociadoRow> GetAsociados(List<Tuple<string, int, long>> claveCantidad)
        {
            if (App.Riviera.AsociadoMDB == null)
                throw new Exception(ERR_ASOCIADOS_MDB_NOT_DEFINED);
            else if (!File.Exists(App.Riviera.AsociadoMDB.FullName))
                throw new Exception(String.Format(ERR_ASOCIADOS_MDB_MISSING, App.Riviera.AsociadoMDB.FullName));
            //Conexión al MDB de Asociados
            AccessConnectionBuilder aBuilder = new AccessConnectionBuilder()
            {
                Access_DB_File = App.Riviera.AsociadoMDB.FullName,
                OleDbProvider = new AccessProvider(OledbProviders.Microsoft_ACE_OleDb_12),
            };
            List<AsociadoRow> rows = new List<AsociadoRow>();
            new VoidAccess_Transaction<object>(new AccessConnectionContent(aBuilder.Data).GenerateConnectionString(),
                delegate (Access_Connector conn, Object[] trParameters)
                {
                    //Se insertan los registros en la BD de DaNTe
                    DaNTeRow dRow;
                    foreach (var row in claveCantidad.OrderBy<Tuple<string, int, long>, String>(x => x.Item1))
                    {
                        dRow = new DaNTeRow(row.Item2, row.Item1) { Handle = String.Format("{0:x}", row.Item3) };
                        if (dRow.HasAsociados(conn))
                        {
                            List<AsociadoRow> asocRows = GetAsociados(conn, dRow.Clave, dRow.Value);
                            Query_BasesMDB query = new Query_BasesMDB();
                            foreach (var asocRow in asocRows)
                                rows.Add(asocRow);
                        }
                    }
                }).Run();
            return rows;
        }

        private void CreateAsociadosQuantificationOracle(UserCredential cred, string qName, string qZone, Oracle_Connector conn, List<Tuple<string, int, long>> claveCantidad)
        {
            //Se insertan los registros en la BD de DaNTe
            Query_Bases query = new Query_Bases();
            String tableName = qName;
            DaNTeRow dRow;

            foreach (var row in claveCantidad.OrderBy<Tuple<string, int, long>, String>(x => x.Item1))
            {
                dRow = new DaNTeRow(row.Item2, row.Item1) { Handle = String.Format("{0:x}", row.Item3) };
                if (dRow.HasAsociados(conn))
                {
                    List<AsociadoRow> rows = GetAsociados(conn, dRow.Clave, dRow.Value);
                    foreach (var asocRow in rows)
                        conn.Update(query.InsertAsociadoRow(cred, qZone, asocRow, String.Format("{0:x}", dRow.Handle)));
                }
            }
        }
        private void CreateDaNTeQuantificationOracle(UserCredential cred, string qName, String zone, Oracle_Connector conn, List<Tuple<string, int, long>> claveCantidad)
        {
            //Se insertan los registros en la BD de DaNTe
            Query_Bases query = new Query_Bases();
            String tableName = qName;
            DaNTeRow dRow;
            String err;
            foreach (var row in claveCantidad.OrderBy<Tuple<string, int, long>, String>(x => x.Item1))
            {
                dRow = new DaNTeRow(row.Item2, row.Item1) { Handle = String.Format("{0:x}", row.Item3) };
                conn.Update(query.InsertDaNTeRow(cred, zone, dRow));
                err = conn.Error;
                Current++;
                bgWorker.ReportProgress(CurrentProgress);
            }
        }
        private void CreateDaNTeQuantificationOracle(UserCredential cred, String qName, String zone, Oracle_Connector conn, Dictionary<String, int> claveCantidad)
        {
            //Se insertan los registros en la BD de DaNTe
            Query_Bases query = new Query_Bases();
            String tableName = qName;
            DaNTeRow dRow;
            foreach (var row in claveCantidad.OrderBy<KeyValuePair<String, int>, String>(x => x.Key))
            {
                dRow = new DaNTeRow(row.Value, row.Key) { Handle = "0" };
                conn.Update(query.InsertDaNTeRow(cred, zone, dRow));
                Current++;
                bgWorker.ReportProgress(CurrentProgress);
            }
        }
        private void CreateDaNTeQuantificationAccess(string qName, String zone, Access_Connector conn, List<Tuple<string, int, long>> claveCantidad)
        {
            //Se insertan los registros en la BD de DaNTe
            Query_BasesMDB query = new Query_BasesMDB();
            String tableName = qName;
            DaNTeRow dRow;
            String err;
            foreach (var row in claveCantidad.OrderBy<Tuple<string, int, long>, String>(x => x.Item1))
            {
                dRow = new DaNTeRow(row.Item2, row.Item1) { Handle = String.Format("{0:x}", row.Item3) };
                conn.Update(query.InsertDaNTeRow(tableName, dRow));
                err = conn.Error;
                Current++;
                bgWorker.ReportProgress(CurrentProgress);
            }
        }

        private void CreateDaNTeQuantificationAccess(String qName, String zone, Access_Connector conn, Dictionary<String, int> claveCantidad)
        {
            //Se insertan los registros en la BD de DaNTe
            Query_BasesMDB query = new Query_BasesMDB();
            String tableName = qName;
            DaNTeRow dRow;
            foreach (var row in claveCantidad.OrderBy<KeyValuePair<String, int>, String>(x => x.Key))
            {
                dRow = new DaNTeRow(row.Value, row.Key) { Handle = "0" };
                conn.Update(query.InsertDaNTeRow(tableName, dRow));
                Current++;
                bgWorker.ReportProgress(CurrentProgress);
            }
        }

        public static List<AsociadoRow> GetAsociados(Access_Connector conn, params Object[] trParameters)
        {
            String key = trParameters[0] as String;
            int total = (int)((Double)trParameters[1]), count;
            List<AsociadoRow> rows = new List<AsociadoRow>();
            String[] cells;
            List<String> asociados = conn.SelectRows(Query_Asociados.SelectAsociadosAccess(key), '¬');

            foreach (String asociado in asociados)
            {
                cells = asociado.Split('¬');
                rows.Add(new AsociadoRow()
                {
                    Mascara = key,
                    Count = total * (int.TryParse(cells[2], out count) ? count : 1),
                    ItemKey = cells[1]
                });
            }
            return rows;
        }
        public static List<AsociadoRow> GetAsociados(Oracle_Connector conn, params Object[] trParameters)
        {
            String key = trParameters[0] as String;
            int total = (int)trParameters[1], count;
            List<AsociadoRow> rows = new List<AsociadoRow>();
            String[] cells;
            List<String> asociados = conn.SelectRows(Query_Asociados.SelectAsociadosOracle(key), '¬');

            foreach (String asociado in asociados)
            {
                cells = asociado.Split('¬');
                rows.Add(new AsociadoRow()
                {
                    Mascara = key,
                    Count = total * (int.TryParse(cells[2], out count) ? count : 1),
                    ItemKey = cells[1]
                });
            }
            return rows;
        }
        /// <summary>
        /// Se realiza el proceso de validadición de la tabla
        /// </summary>
        /// <param name="conn">La conexión activa del conector</param>
        private void ValidateTables(String qName, String qComments, Access_Connector conn)
        {
            Query_BasesMDB query = new Query_BasesMDB();
            String tableName = qName;
            if (!conn.TableExist(tableName))                        //Cuantifiación DaNTe
            {
                conn.Update(query.CreateDaNTeTable(tableName));
                //Agregamos un comentario al archivo de cuantificación
                //El comentario solo se agregá si no se realizá la mezcla de la cuantificación
                conn.Update(query.InsertComment(tableName, qComments != String.Empty ? qComments : "Cauntificación Riviera. Realizada: " + DateTime.Now.ToShortDateString(), this.Quantification.QZone));
            }
            else if (!MergeQuantification)
                conn.Update(query.ClearDaNTeTable(tableName));
            else
                conn.Update(query.ClearCurrentDaNTeTable(tableName));
        }


    }
}
