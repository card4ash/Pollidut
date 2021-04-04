using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{
    //POCO object Plain Old Code Object
    public class Region
    {
        public String RegionId { get; set; }
        public String RegionName { get; set; }
    }

    public class RegionManager
    {
        private static Region FillEntity(SqlDataReader reader)
        {
            return new Region { RegionId = reader["RegionId"].ToString(), RegionName = reader["RegionName"].ToString() };
        }

        public static List<Region> GetComboList()
        {
            List<Region> regions = new List<Region>();

            //regions.Add(new Region { RegionId = "-1", RegionName = "select a region" });


            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "SELECT REGION_ID AS RegionId,REGION_NAME AS RegionName FROM REGIONS WHERE REGION_ID>0 ORDER BY REGION_NAME ";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            regions.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            // regions.Add(new Region { RegionId = "0", RegionName= "None" });
            return regions;
        }

        public static Region SearchRegion(Int32 regionId)
        {
            Region region = new Region();

            //---------
            // your code to search in database with this regionId
            //---------

            return region;

        }


        public static Region SearchRegionByArea(Int32 areaId)
        {
            Region region = new Region();

            //---------
            // your code to search in database with this regionId
            //---------

            return region;

        }

    }

}