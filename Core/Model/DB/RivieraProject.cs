using Nameless.Libraries.DB.Mikasa;
using Nameless.Libraries.DB.Mikasa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model.DB
{
    /// <summary>
    /// Defines a Riviera project originally used the Revit Data
    /// </summary>
    /// <seealso cref="Nameless.Libraries.DB.Mikasa.Model.DBMappedObject" />
    public class RivieraProject : DBMappedObject
    {
        const string TABLENAME = "REVIT_PROYECTO";
        const string FIELD_ID = "ID_REVIT";
        const string FIELD_USERID = "ID_USER";
        const string FIELD_NAME = "ARCHIVO_REVIT";
        const string FIELD_LAST_ACCESS = "FECHA_ULTIMO_ACCESO";
        /// <summary>
        /// The project identifier
        /// </summary>
        public new int Id;
        /// <summary>
        /// The user identifier
        /// </summary>
        public int UserId;
        /// <summary>
        /// The project name
        /// </summary>
        public String ProjectName;
        /// <summary>
        /// The last access
        /// </summary>
        public DateTime LastAccess;
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public override string TableName => TABLENAME;
        /// <summary>
        /// Gets the primary key.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public override string PrimaryKey => FIELD_ID;
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraProject"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public RivieraProject(SelectionResult[] result)
            : base(result)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraProject"/> class.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="userId">The user identifier.</param>
        public RivieraProject(string projectName, int userId)
            : base()
        {
            this.ProjectName = projectName;
            this.UserId = userId;
            this.LastAccess = DateTime.Now;
        }

        /// <summary>
        /// Parses the object.
        /// </summary>
        /// <param name="result">The result.</param>
        protected override void ParseObject(SelectionResult[] result)
        {
            this.Id = result.GetInteger(this.PrimaryKey);
            this.UserId = result.GetInteger(FIELD_USERID);
            this.ProjectName = result.GetString(FIELD_NAME);
            this.LastAccess = result.GetValue<DateTime>(FIELD_LAST_ACCESS);
        }
        /// <summary>
        /// Selects the project.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="conn">The database connection.</param>
        /// <returns>The Riviera project</returns>
        public static RivieraProject SelectProject(int userId, String projectName, DB_Connector conn)
        {
            QueryBuilder qB = new QueryBuilder(TABLENAME, conn);
            qB.AddSelectionColumn();
            qB.AddEqualCondition(FIELD_USERID, userId, "AND");
            qB.AddEqualCondition(FIELD_NAME, projectName);
            String query = qB.GetQuery();
            return conn.Select<RivieraProject>(query).FirstOrDefault();
        }
        /// <summary>
        /// Selects all projects.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <returns>The Riviera project</returns>
        public static IEnumerable<RivieraProject> SelectProjects(DB_Connector conn)
        {
            QueryBuilder qB = new QueryBuilder(TABLENAME, conn);
            qB.AddSelectionColumn();
            String query = qB.GetQuery();
            return conn.Select<RivieraProject>(query);
        }
        /// <summary>
        /// Selects the project.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="conn">The database connection.</param>
        /// <returns>The Riviera project</returns>
        public static RivieraProject SelectProject(int userId, int projectId, DB_Connector conn)
        {
            QueryBuilder qB = new QueryBuilder(TABLENAME, conn);
            qB.AddSelectionColumn();
            qB.AddEqualCondition(FIELD_USERID, userId, "AND");
            qB.AddEqualCondition(FIELD_ID, projectId);
            String query = qB.GetQuery();
            return conn.Select<RivieraProject>(query).FirstOrDefault();
        }
        /// <summary>
        /// Gets the insertion fields.
        /// </summary>
        /// <returns>The database fields</returns>
        public override DBField[] GetInsertionFields(DB_Connector conn)
        {
            return new DBField[]
            {
               this.CreateFieldAsString(conn, FIELD_USERID, this.UserId.ToString()),
               this.CreateFieldAsNull(conn, FIELD_ID),
               this.CreateFieldAsString(conn, FIELD_NAME, this.ProjectName),
               this.CreateFieldAsDate(conn, FIELD_LAST_ACCESS, this.LastAccess)
            };
        }
        /// <summary>
        /// Gets the update fields.
        /// Only Last access can be updated
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns></returns>
        public override DBField[] GetUpdateFields(DB_Connector conn)
        {
            return new DBField[]
            {
               this.CreateFieldAsDate(conn, FIELD_NAME, this.LastAccess)
            };
        }
        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="input">The input.</param>
        public override void UpdateData(KeyValuePair<string, object>[] input)
        {
            foreach (var val in input)
                switch (val.Key)
                {
                    case FIELD_LAST_ACCESS:
                        this.LastAccess = (DateTime)val.Value;
                        break;
                }
        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.ProjectName;
        }
    }
}
