using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models.ServerToClientModel
{

    public class HouseAndTypeSpecificUnassignedEmployee
    {

        public String EmployeeId { get; set; }
        public String EmployeeName { get; set; }
        public String EmployeeCode { get; set; }

    }

    public class HouseAndTypeSpecificUnassignedEmployeeManager
    {
        private static HouseAndTypeSpecificUnassignedEmployee FillEntity(SqlDataReader reader)
        {
            return new HouseAndTypeSpecificUnassignedEmployee { EmployeeId = reader["EmployeeId"].ToString(), EmployeeName = reader["EmployeeName"].ToString(), EmployeeCode = reader["EmployeeCode"].ToString() };
        }

        public static List<HouseAndTypeSpecificUnassignedEmployee> GetPositions(int distributionHouseId, int employeetypeid)
        {
            List<HouseAndTypeSpecificUnassignedEmployee> positions = new List<HouseAndTypeSpecificUnassignedEmployee>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = " SELECT EmployeeId, EmployeeName, EmployeeCode "
                                  + " FROM dbo.viewHouseAndTypeSpecificUnassignedEmployees "
                                  + " WHERE (EmployeeTypeId = " + employeetypeid + ") AND (DistributionHouseId = " + distributionHouseId + ") ORDER BY EmployeeName";

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