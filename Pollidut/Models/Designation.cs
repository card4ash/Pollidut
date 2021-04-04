using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{

    public class Designation
    {
        public Int32 DesignationId { get; set; }
        public String DesignationName { get; set; }
    }

    public class DesignationManager
    {
        private static Designation FillEntity(SqlDataReader reader)
        {
            return new Designation { DesignationId = Convert.ToInt32(reader["DesignationId"]), DesignationName = reader["DesignationName"].ToString() };
        }

        public static List<Designation> GetDesignations()
        {
            List<Designation> Designations = new List<Designation>();
            //  Designations.Add(new Designation { DesignationId = -1, DesignationName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select DESIGNATION_ID AS DesignationId, DESIGNATION_NAME AS DesignationName from DESIGNATIONS where DESIGNATION_ID>0 order by DESIGNATION_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Designations.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

          //  Designations.Add(new Designation { DesignationId = 0, DesignationName = "None" });
            return Designations;
        }
    }
}