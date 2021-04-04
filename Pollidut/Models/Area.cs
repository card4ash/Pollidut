using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class Area
    {
        public String AreaId { get; set; }
        public String AreaName { get; set; }
    }
    public class AreaManager
    {
        public static Area FillEntity(SqlDataReader reader)
        {
            return new Area { AreaId = reader["AREA_ID"].ToString(), AreaName = reader["AREA_NAME"].ToString() };

        }
        public static List<Area> GetComboList(int regionid)
        {
            List<Area> Areas = new List<Area>();
            String CS = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(CS))
            {
                string sqlSelect = "SELECT AREA_ID, AREA_NAME FROM AREAS WHERE AREA_ID >0 and REGION_ID = " + regionid + " ORDER BY AREA_NAME DESC";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Areas.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }

            }
            return Areas;


        }

        public static List<Area> GetComboList()
        {
            List<Area> Areas = new List<Area>();
            String CS = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(CS))
            {
                string sqlSelect = "SELECT AREA_ID, AREA_NAME FROM AREAS WHERE AREA_ID >0 ORDER BY AREA_NAME ASC";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Areas.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }

            }
            return Areas;


        }
    }
}