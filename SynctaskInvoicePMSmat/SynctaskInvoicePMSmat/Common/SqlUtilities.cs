using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace SynctaskInvoicePMSmat
{
    public class SqlUtilities
    {
        /// <summary>
        /// Connection string in app config
        /// </summary>
        public string configConnectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        /// <summary>
        /// Get connection then open
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(configConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// ExecuteQuery with StoredProcedure
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="storedProcName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string storedProcName, Dictionary<string, SqlParameter> procParameters )
        {
            DataSet ds = new DataSet();
            using (SqlConnection cn = GetConnection())
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = storedProcName;
                    foreach (var procParameter in procParameters)
                    {
                        cmd.Parameters.Add(procParameter.Value);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// ExecuteNonQuery with StoredProcedure
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="storedProcName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string storedProcName, Dictionary<string, SqlParameter> procParameters)
        {
            int rc;
            using (SqlConnection cn = GetConnection())
            {
                // create a SQL command to execute the stored procedure
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = storedProcName;
                    // assign parameters passed in to the command
                    foreach (var procParameter in procParameters)
                    {
                        cmd.Parameters.Add(procParameter.Value);
                    }
                    rc = cmd.ExecuteNonQuery();
                }
            }
            return rc;
        }

        /// <summary>
        /// ExecuteQuery with query string
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="storedProcName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public DataSet ExecuteQueryWithQuery(string queryString)
        {
            DataSet ds = new DataSet();
            using (SqlConnection cn = GetConnection())
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = queryString;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
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
        /// <param name="connectionName"></param>
        /// <param name="storedProcName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public int ExecuteNonQueryWithQuery(string queryString)
        {
            int rc;
            using (SqlConnection cn = GetConnection())
            {
                // create a SQL command to execute the stored procedure
                using (SqlCommand cmd = cn.CreateCommand())
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
