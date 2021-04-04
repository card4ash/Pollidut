using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class EducationalDegree
    {
        public Int32 EducationalDegreeId { get; set; }
        public String EducationalDegreeName { get; set; }
    }

    public class EducationalDegreeManager
    {
        private static EducationalDegree FillEntity(SqlDataReader reader)
        {
            return new EducationalDegree { EducationalDegreeId = Convert.ToInt32(reader["EducationalDegreeId"]), EducationalDegreeName = reader["EducationalDegreeName"].ToString() };
        }

        public static List<EducationalDegree> GetEducationalDegrees()
        {
            List<EducationalDegree> EducationalDegrees = new List<EducationalDegree>();
            //  Designations.Add(new Designation { DesignationId = -1, DesignationName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select EDUCATIONAL_DEGREE_ID AS EducationalDegreeId, EDUCATIONAL_DEGREE_NAME AS EducationalDegreeName from EDUCATIONAL_DEGREES where EDUCATIONAL_DEGREE_id <> 0 order by EDUCATIONAL_DEGREE_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            EducationalDegrees.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            //EducationalDegrees.Add(new EducationalDegree { EducationalDegreeId = 0, EducationalDegreeName = "None" });
            return EducationalDegrees;
        }
    }
}