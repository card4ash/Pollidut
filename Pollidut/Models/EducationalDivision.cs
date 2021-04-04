using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class EducationalDivision
    {
        public Int32 DivisionId { get; set; }
        public String DivisionName { get; set; }
    }

    public class EducationalDivisionManager
    {
        private static EducationalDivision FillEntity(SqlDataReader reader)
        {
            return new EducationalDivision { DivisionId = Convert.ToInt32(reader["DivisionId"]), DivisionName = reader["DivisionName"].ToString() };
        }

        public static List<EducationalDivision> GetDivisions()
        {
            List<EducationalDivision> Divisions = new List<EducationalDivision>();
            //  Districts.Add(new District { DistrictId = -1, DistrictName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select DIVISION_ID AS DivisionId, DIVISION_NAME AS DivisionName from EDUCATIONAL_DIVISIONS where DIVISION_ID<>0 order by DIVISION_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Divisions.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            Divisions.Add(new EducationalDivision { DivisionId = 0, DivisionName = "None" });
            return Divisions;
        }
    }
}