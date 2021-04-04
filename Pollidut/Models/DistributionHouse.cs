using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{

    public class DistributionHouse
    {
        public String DistributionHouseId { get; set; }
        public String DistributionHouseName { get; set; }

        // public DistributionHouse() { }
    }


    public class DistributionHouseManager
    {
        private static DistributionHouse FillEntity(SqlDataReader reader)
        {
            return new DistributionHouse { DistributionHouseId = reader["DistributionHouseId"].ToString(), DistributionHouseName = reader["DistributionHouseName"].ToString() };
        }

        public static List<DistributionHouse> GetDistributionHouses()
        {
            List<DistributionHouse> sectons = new List<DistributionHouse>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select Distribution_House_ID AS DistributionHouseId, Distribution_House_NAME AS DistributionHouseName from DISTRIBUTION_HOUSES where Distribution_House_ID>0 order by DISTRIBUTION_HOUSE_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            sectons.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            return sectons;
        }


        public static List<DistributionHouse> GetAreaSpecificDistHouses(int areaId)
        {
            List<DistributionHouse> sectons = new List<DistributionHouse>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select Distribution_House_ID AS DistributionHouseId, Distribution_House_NAME AS DistributionHouseName from DISTRIBUTION_HOUSES where Distribution_House_ID>0 and AREA_ID = " + areaId + " order by DistributionHouseName ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            sectons.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }
            return sectons;
        }


        public static DistributionHouse GetEmployeeHouse(int userId) 
        {
            DistributionHouse DH = null;
            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "Select Distribution_House_Id AS DistributionHouseId, Distribution_House_Name AS DistributionHouseName from view_Users Where User_Id=@UserId";
                    command.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                           DH=FillEntity(reader);
                        }
                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            return DH;
        }

        public static List<DistributionHouse> GetSupervisorHouse(int userId)
        {
            List<DistributionHouse> sectons = new List<DistributionHouse>();
            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "SELECT        dbo.EMPLOYEE_DISTRIBUTION_HOUSES.DistributionHouseId as DistributionHouseId, dbo.DISTRIBUTION_HOUSES.DISTRIBUTION_HOUSE_NAME as DistributionHouseName " +
                                            "FROM            dbo.EMPLOYEE_DISTRIBUTION_HOUSES INNER JOIN " +
                         " dbo.DISTRIBUTION_HOUSES ON dbo.EMPLOYEE_DISTRIBUTION_HOUSES.DistributionHouseId = dbo.DISTRIBUTION_HOUSES.DISTRIBUTION_HOUSE_ID " +
                         " WHERE        (dbo.EMPLOYEE_DISTRIBUTION_HOUSES.EmployeeId = @UserId )";
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            sectons.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }
            return sectons;
        }
        public static List<DistributionHouse> GetDistributionHouses(string HouseIds)
        {
            List<DistributionHouse> houses = new List<DistributionHouse>();
            // sectons.Add(new DistributionHouse { DistributionHouseId = -1, DistributionHouseName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select Distribution_House_ID AS DistributionHouseId, Distribution_House_NAME AS DistributionHouseName from DISTRIBUTION_HOUSES where Distribution_House_ID IN( " + HouseIds + ") order by DistributionHouseName ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            houses.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            // sectons.Add(new DistributionHouse { DistributionHouseId = 0, DistributionHouseName = "None" });
            return houses;
        }
    }
}