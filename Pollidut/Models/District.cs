using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{

    public class District
    {
        public Int32 DistrictId { get; set; }
        public String DistrictName { get; set; }
    }

    public class DistrictManager
    {
        private static District FillEntity(SqlDataReader reader)
        {
            return new District { DistrictId = Convert.ToInt32(reader["DistrictId"]), DistrictName = reader["DistrictName"].ToString() };
        }

        public static List<District> GetDistricts()
        {
            List<District> Districts = new List<District>();
            //  Districts.Add(new District { DistrictId = -1, DistrictName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select District_ID AS DistrictId, District_NAME AS DistrictName from DistrictS  order by District_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Districts.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            Districts.Add(new District { DistrictId = 0, DistrictName = "None" });
            return Districts;
        }
    }
}