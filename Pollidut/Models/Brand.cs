using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class Brand
    {
        public Int32 BrandId { get; set; }
        public String BrandName { get; set; }
    }
    public class BrandManager
    {
        private static Brand FillEntity(SqlDataReader reader)
        {
            return new Brand { BrandId = Convert.ToInt32(reader["BrandId"]), BrandName = reader["BrandName"].ToString() };
        }

        public static List<Brand> GetBrands()
        {
            List<Brand> Brands = new List<Brand>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select BRAND_ID AS BrandId, BRAND_NAME AS BrandName from BRANDS where brand_id <> 0 order by BRAND_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Brands.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            return Brands;
        }
    }
}