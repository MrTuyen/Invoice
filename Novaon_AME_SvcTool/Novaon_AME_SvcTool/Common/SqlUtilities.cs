using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novaon_AME_SvcTool.Common
{
    public class SqlUtilities
    {
        /// <summary>
        /// Connection string in app config
        /// </summary>
        public string configConnectionString = ConfigurationManager.ConnectionStrings["ConnectionStringData"].ConnectionString;

        /// <summary>
        /// Get connection then open
        /// </summary>
        /// <returns></returns>
        public OleDbConnection GetConnection()
        {
            OleDbConnection conn = new OleDbConnection(configConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// ExecuteQuery with query string
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public DataSet ExecuteQueryWithQuery(string queryString)
        {
            DataSet ds = new DataSet();
            using (OleDbConnection cn = GetConnection())
            {
                using (OleDbCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = queryString;
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// ExecuteNonQuery with query string
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public int ExecuteNonQueryWithQuery(string queryString)
        {
            int rc;
            using (OleDbConnection cn = GetConnection())
            {
                // create a SQL command to execute the stored procedure
                using (OleDbCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = queryString;
                    // assign parameters passed in to the command
                    rc = cmd.ExecuteNonQuery();
                }
            }
            return rc;
        }
    }
}
