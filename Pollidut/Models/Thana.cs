using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class Thana
    {
        public Int32 ThanaId { get; set; }
        public String ThanaName { get; set; }
    }

    public class ThanaManager
    {
        private static Thana FillEntity(SqlDataReader reader)
        {
            return new Thana { ThanaId = Convert.ToInt32(reader["ThanaId"]), ThanaName = reader["ThanaName"].ToString() };
        }

        public static List<Thana> GetThanas()
        {
            List<Thana> Thanas = new List<Thana>();
            //  Districts.Add(new District { DistrictId = -1, DistrictName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select Thana_ID AS ThanaId, Thana_NAME AS ThanaName from ThanaS where thana_id<>0 order by Thana_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Thanas.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            Thanas.Add(new Thana { ThanaId = 0, ThanaName = "None" });
            return Thanas;
        }

        public static List<Thana> GetThanas(int districtId)
        {
            List<Thana> Thanas = new List<Thana>();
            //  Districts.Add(new District { DistrictId = -1, DistrictName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select Thana_ID AS ThanaId, Thana_NAME AS ThanaName from ThanaS where thana_id<>0 AND DISTRICT_ID=" + districtId + " order by Thana_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Thanas.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            Thanas.Add(new Thana { ThanaId = 0, ThanaName = "None" });
            return Thanas;
        }
    }
}