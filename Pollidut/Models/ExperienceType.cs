using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class ExperienceType
    {
        public Int32 ExperienceTypeId { get; set; }
        public String ExperienceTypeName { get; set; }
    }

    public class ExperienceTypeManager
    {
        private static ExperienceType FillEntity(SqlDataReader reader)
        {
            return new ExperienceType { ExperienceTypeId = Convert.ToInt32(reader["ExperienceTypeId"]), ExperienceTypeName = reader["ExperienceTypeName"].ToString() };
        }

        public static List<ExperienceType> GetExperienceTypes()
        {
            List<ExperienceType> ExperienceTypes = new List<ExperienceType>();
            //  Designations.Add(new Designation { DesignationId = -1, DesignationName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select EXPERIENCE_TYPE_ID AS ExperienceTypeId, EXPERIENCE_TYPE_NAME AS ExperienceTypeName from EXPERIENCE_TYPES where EXPERIENCE_TYPE_ID <> 0 order by EXPERIENCE_TYPE_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ExperienceTypes.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            ExperienceTypes.Add(new ExperienceType { ExperienceTypeId = 0, ExperienceTypeName = "None" });
            return ExperienceTypes;
        }
    }
}