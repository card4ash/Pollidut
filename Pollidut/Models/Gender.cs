using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class Gender
    {
        public Int32 GenderId { get; set; }
        public String GenderName { get; set; }
    }

    public class GenderManager
    {
        private static Gender FillEntity(SqlDataReader reader)
        {
            return new Gender { GenderId = Convert.ToInt32(reader["GenderId"]), GenderName = reader["GenderName"].ToString() };
        }

        public static List<Gender> GetGenders()
        {
            List<Gender> Genders = new List<Gender>();
            //  Designations.Add(new Designation { DesignationId = -1, DesignationName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select GENDER_ID AS GenderId, GENDER_NAME AS GenderName from GENDERS where GENDER_ID <> 0 order by GENDER_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Genders.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            Genders.Add(new Gender { GenderId = 0, GenderName = "None" });
            return Genders;
        }
    }

}