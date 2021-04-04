using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class MaritalStatus
    {
        public Int32 MaritalStatusId { get; set; }
        public String MaritalStatusName { get; set; }
    }

    public class MaritalStatusManager
    {
        private static MaritalStatus FillEntity(SqlDataReader reader)
        {
            return new MaritalStatus { MaritalStatusId = Convert.ToInt32(reader["MaritalStatusId"]), MaritalStatusName = reader["MaritalStatusName"].ToString() };
        }

        public static List<MaritalStatus> GetMaritalStatuses()
        {
            List<MaritalStatus> MaritalStatuses = new List<MaritalStatus>();
            //  Designations.Add(new Designation { DesignationId = -1, DesignationName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select MARITAL_STATUS_ID AS MaritalStatusId, MARITAL_STATUS_NAME AS MaritalStatusName from MARITAL_STATUSES where MARITAL_STATUS_ID <> 0 order by MARITAL_STATUS_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            MaritalStatuses.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            MaritalStatuses.Add(new MaritalStatus { MaritalStatusId = 0, MaritalStatusName = "None" });
            return MaritalStatuses;
        }
    }

}