using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models.ServerToClientModel
{

    public class DistributionHouseSpecificPosition
    {
        public String PositionId { get; set; }
        public String PositionName { get; set; }
        public String PositionTypeId { get; set; }
        public String EmployeeId { get; set; }

    }

    public class DistributionHouseSpecificPositionManager
    {
        private static DistributionHouseSpecificPosition FillEntity(SqlDataReader reader)
        {
            return new DistributionHouseSpecificPosition { PositionId = reader["PositionId"].ToString(), PositionName = reader["PositionName"].ToString(), PositionTypeId = reader["PositionTypeId"].ToString(), EmployeeId = reader["EmployeeId"].ToString() };
        }

        public static List<DistributionHouseSpecificPosition> GetPositions(int distributionHouseId)
        {
            List<DistributionHouseSpecificPosition> positions = new List<DistributionHouseSpecificPosition>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = " SELECT PositionId, PositionName, PositionTypeId, EmployeeId "
                                  + " FROM dbo.viewDistributionHouseSpecificPositions AS V "
                                  + " WHERE (Active = 1) AND (HouseId = " + distributionHouseId + ") ORDER BY PositionName";

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