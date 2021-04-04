using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class EducationalGrade
    {
        public Int32 GradeId { get; set; }
        public String GradeName { get; set; }
    }

    public class EducationalGradeManager
    {
        private static EducationalGrade FillEntity(SqlDataReader reader)
        {
            return new EducationalGrade { GradeId = Convert.ToInt32(reader["GradeId"]), GradeName = reader["GradeName"].ToString() };
        }

        public static List<EducationalGrade> GetGrades()
        {
            List<EducationalGrade> Grades = new List<EducationalGrade>();
            //  Districts.Add(new District { DistrictId = -1, DistrictName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select GRADE_ID AS GradeId, GRADE_NAME AS GradeName from EDUCATIONAL_GRADES where GRADE_ID<>0 order by GRADE_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Grades.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            Grades.Add(new EducationalGrade { GradeId = 0, GradeName = "None" });
            return Grades;
        }
    }
}