using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models.ServerToClientModel
{

    public class HouseSpecificPositionAndEmployee
    {
        //PositionId, PositionName, PositionTypeId, ACTIVE, EmployeeId, HouseId, EmployeeCode, PersonName, Photo

        public String PositionId { get; set; }
        public String PositionName { get; set; }
        public String PositionTypeId { get; set; }
        public String EmployeeId { get; set; }
        public String EmployeeCode { get; set; }
        public String PersonName { get; set; }
        public String Photo { get; set; }

    }

    public class HouseSpecificPositionAndEmployeeManager
    {
        private static HouseSpecificPositionAndEmployee FillEntity(SqlDataReader reader)
        {
            return new HouseSpecificPositionAndEmployee { PositionId = reader["PositionId"].ToString(), PositionName = reader["PositionName"].ToString(), PositionTypeId = reader["PositionTypeId"].ToString(), EmployeeId = reader["EmployeeId"].ToString(), EmployeeCode = reader["EmployeeCode"].ToString(), PersonName = reader["PersonName"].ToString(), Photo = reader["Photo"].ToString() };
        }

        public static List<HouseSpecificPositionAndEmployee> GetPositions(int distributionHouseId)
        {
            List<HouseSpecificPositionAndEmployee> positions = new List<HouseSpecificPositionAndEmployee>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = " SELECT PositionId, PositionName, PositionTypeId, EmployeeId, EmployeeCode, PersonName, Photo "
                                  + " FROM dbo.viewHouseSpecificPositionAndEmployee AS V "
                                  + " WHERE (ACTIVE = 1) AND (HouseId = " + distributionHouseId + ") ORDER BY PositionName";

                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            positions.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }


            return positions;
        }


        public static List<HouseSpecificPositionAndEmployee> GetPositions(int distributionHouseId, int positionTypeId)
        {
            List<HouseSpecificPositionAndEmployee> positions = new List<HouseSpecificPositionAndEmployee>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = " SELECT PositionId, PositionName, PositionTypeId, EmployeeId, EmployeeCode, PersonName, Photo "
                                  + " FROM dbo.viewHouseSpecificPositionAndEmployee AS V "
                                  + " WHERE (ACTIVE = 1) AND (HouseId = " + distributionHouseId + ") AND (PositionTypeId = " + positionTypeId + ")  ORDER BY PositionName";

                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            positions.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }


            return positions;
        }
    }
}