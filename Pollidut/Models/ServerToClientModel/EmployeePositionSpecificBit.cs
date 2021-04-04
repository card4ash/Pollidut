using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models.ServerToClientModel
{

    public class EmployeePositionSpecificBit
    {
        public String BitId { get; set; }
        public String BitName { get; set; }
        public String PositionId { get; set; }
        // public Bit() { }
    }


    public class EmployeePositionSpecificBitManager
    {
        private static EmployeePositionSpecificBit FillEntity(SqlDataReader reader)
        {
            return new EmployeePositionSpecificBit { BitId = reader["BitId"].ToString(), BitName = reader["BitName"].ToString(), PositionId = reader["PositionId"].ToString() };
        }

        public static List<EmployeePositionSpecificBit> GetPositionSpecificBitList(int positionId)
        {
            List<EmployeePositionSpecificBit> sectons = new List<EmployeePositionSpecificBit>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = " SELECT   TOP (100) PERCENT DCWP.CM_POSITION_ID AS PositionId, B.BIT_ID AS BitId, B.BIT_NAME AS BitName "
                    + " FROM         dbo.DEFAULT_CM_WISE_PLANS AS DCWP INNER JOIN "
                    + "  dbo.BITS AS B ON DCWP.BIT_ID = B.BIT_ID WHERE     (DCWP.CM_POSITION_ID = "+ positionId +") ORDER BY BitId";

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

      
    }
}