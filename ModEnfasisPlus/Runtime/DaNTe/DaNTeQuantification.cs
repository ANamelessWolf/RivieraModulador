using DaSoft.Riviera.OldModulador.Controller;
using System;
using System.IO;

namespace DaSoft.Riviera.OldModulador.Runtime.DaNTe
{
    public class DaNTeQuantification
    {
        /// <summary>
        /// El archivo de access
        /// </summary>
        public String AccessFilePath;
        /// <summary>
        /// Define el modo de conexión de una cuantificación de DaNTe
        /// </summary>
        public QuantificationMode Mode;
        /// <summary>
        /// El id del proyecto activo tomado de la tabla de Oracle
        /// </summary>
        public int ProjectId;
        /// <summary>
        /// The access file
        /// </summary>
        public FileInfo AccessFile { get { return new FileInfo(AccessFilePath); } }
        /// <summary>
        /// El nombre de la cuantificación
        /// </summary>
        public String QName;
        /// <summary>
        /// La zona de la cuantificación
        /// </summary>
        public String QZone;
        /// <summary>
        /// Los comentarios de la cuantificación
        /// </summary>
        public String QComments;
        /// <summary>
        /// La cuantificación activa
        /// </summary>
        public Quantifier Quantification;
        /// <summary>
        /// Crea una nueva cuantificación de DaNTe
        /// </summary>
        public DaNTeQuantification(String accessPth, String qName, Quantifier data)
        {
            this.AccessFilePath = accessPth;
            this.Quantification = data;
            this.QName = qName;
            this.Mode = QuantificationMode.Access;
        }
        /// <summary>
        /// Crea una nueva cuantificación de DaNTe
        /// </summary>
        public DaNTeQuantification(int projectId, String qName, Quantifier data)
        {
            this.ProjectId = projectId;
            this.Quantification = data;
            this.QName = qName;
            this.Mode = QuantificationMode.ORACLE;
        }
        /// <summary>
        /// Realiza el proceso de cuantificación de DaNTe
        /// </summary>
        /// <param name="zone">El nombre de la zona</param>
        /// <param name="comments">Los comentarios de la cuantificación</param>
        /// <param name="gQ">Verdadero para una cuantifiación agrupada</param>
        /// <param name="mQ">Verdadero para una cuantifiación anexada</param>
        public void Quantify(String zone, String comments, Boolean gQ, Boolean mQ)
        {

            DaNTe_Transaction dn = new DaNTe_Transaction(this, gQ, mQ);
            this.QZone = zone;
            this.QComments = comments;
            dn.StartTransaction();
        }

    }
}
