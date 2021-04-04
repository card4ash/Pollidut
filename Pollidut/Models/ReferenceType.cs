using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class ReferenceType
    {
        public Int32 ReferenceTypeId { get; set; }
        public String ReferenceTypeName { get; set; }
    }
    public class ReferenceTypeManager
    {
        private static ReferenceType FillEntity(SqlDataReader reader)
        {
            return new ReferenceType { ReferenceTypeId = Convert.ToInt32(reader["ReferenceTypeId"]), ReferenceTypeName = reader["ReferenceTypeName"].ToString() };
        }

        public static List<ReferenceType> GetReferenceTypes()
        {
            List<ReferenceType> ReferenceTypes = new List<ReferenceType>();
            //  Designations.Add(new Designation { DesignationId = -1, DesignationName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select REFERENCE_TYPE_ID AS ReferenceTypeId, REFERENCE_TYPE_NAME AS ReferenceTypeName from REFERENCE_TYPES where REFERENCE_TYPE_ID <> 0 order by REFERENCE_TYPE_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReferenceTypes.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            ReferenceTypes.Add(new ReferenceType { ReferenceTypeId = 0, ReferenceTypeName = "None" });
            return ReferenceTypes;
        }
    }
}