using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{

    public class EmployeePosition
    {
        public Int32 PositionId { get; set; }
        public String PositionName { get; set; }

        // public EmployeePosition() { }
    }


    public class EmployeePositionManager
    {
        private static EmployeePosition FillEntity(SqlDataReader reader)
        {
            return new EmployeePosition { PositionId = Convert.ToInt32(reader["PositionId"]), PositionName = reader["PositionName"].ToString() };
        }

        public static List<EmployeePosition> GetEmployeePositions()
        {
            List<EmployeePosition> sectons = new List<EmployeePosition>();
            // sectons.Add(new EmployeePosition { PositionId = -1, PositionName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select EmployeePosition_ID AS PositionId, EmployeePosition_NAME AS PositionName from EmployeePositionS where EmployeePosition_ID>0 order by PositionName ASC";
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

            sectons.Add(new EmployeePosition { PositionId = 0, PositionName = "None" });
            return sectons;
        }

        public static List<EmployeePosition> GetHouseAndTypeSpecificEmployeePositions(Int32 DistributionHouseId, Int32 positionTypeId)
        {
            List<EmployeePosition> EmployeePositions = new List<EmployeePosition>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "SELECT CM_POSITION_ID AS PositionId, CM_POSITION_NAME AS PositionName FROM dbo.CM_POSITIONS WHERE (DISTRIBUTION_HOUSE_ID = "+ DistributionHouseId +") AND (POSITION_TYPE_ID = "+ positionTypeId +") ORDER BY PositionName";


                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            EmployeePositions.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            return EmployeePositions;
        }


    }
}