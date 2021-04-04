using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class Religion
    {
        public Int32 ReligionId { get; set; }
        public String ReligionName { get; set; }
    }

    public class ReligionManager
    {
        private static Religion FillEntity(SqlDataReader reader)
        {
            return new Religion { ReligionId = Convert.ToInt32(reader["ReligionId"]), ReligionName = reader["ReligionName"].ToString() };
        }

        public static List<Religion> GetReligions()
        {
            List<Religion> Religions = new List<Religion>();
            //  Designations.Add(new Designation { DesignationId = -1, DesignationName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select RELIGION_ID AS ReligionId, RELIGION_NAME AS ReligionName from RELIGIONS where RELIGION_ID <> 0 order by RELIGION_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Religions.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            Religions.Add(new Religion { ReligionId = 0, ReligionName = "None" });
            return Religions;
        }
    }
}