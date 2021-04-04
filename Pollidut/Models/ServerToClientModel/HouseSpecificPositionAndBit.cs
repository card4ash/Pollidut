using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models.ServerToClientModel
{
    public class HouseSpecificCmPositionAndBit
    {
        public String PositionId { get; set; }
        public String PositionName { get; set; }
        public String BitId { get; set; }
        public String BitName { get; set; }
    }

    public class HouseSpecificCmPositionAndBitManager
    {
        private static HouseSpecificCmPositionAndBit FillEntity(SqlDataReader reader)
        {
            return new HouseSpecificCmPositionAndBit { PositionId = reader["PositionId"].ToString(), PositionName = reader["PositionName"].ToString(), BitId = reader["BitId"].ToString(), BitName = reader["BitName"].ToString() };
        }

        public static List<HouseSpecificCmPositionAndBit> GetHouseSpecificValidPositionAndBitList(int distributionHouseId)
        {
            List<HouseSpecificCmPositionAndBit> positions = new List<HouseSpecificCmPositionAndBit>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                //string sqlSelect = " SELECT PositionId,PositionName,BitId, BitName "
                //                  + " FROM dbo.viewHouseSpecificCmPositionAndBit WHERE HouseId = " + distributionHouseId + " ORDER BY PositionName, BitName";

                //string sqlSelect = " SELECT     TOP (100) PERCENT P.CM_POSITION_ID AS PositionId, P.CM_POSITION_NAME AS PositionName, B.BIT_ID AS BitId, B.BIT_NAME AS BitName "
                //                 + " FROM dbo.DEFAULT_CM_WISE_PLANS AS D INNER JOIN "
                //                 + " dbo.CM_POSITIONS AS P ON D.CM_POSITION_ID = P.CM_POSITION_ID INNER JOIN "
                //                 + " dbo.BITS AS B ON D.BIT_ID = B.BIT_ID "
                //                 + " WHERE (B.DISTRIBUTION_HOUSE_ID = "+ distributionHouseId +")  AND (P.POSITION_TYPE_ID = 5) "
                //                 + " ORDER BY PositionName, BitId";

                string sqlSelect = " SELECT P.CM_POSITION_ID AS PositionId, P.CM_POSITION_NAME + '(' + dbo.PERSONS.PERSON_NAME + ')' AS PositionName, B.BIT_ID AS BitId, B.BIT_NAME AS BitName " +
                                  
                                    " FROM dbo.DEFAULT_CM_WISE_PLANS AS D INNER JOIN " +
                                    " dbo.CM_POSITIONS AS P ON D.CM_POSITION_ID = P.CM_POSITION_ID INNER JOIN " +
                                    " dbo.BITS AS B ON D.BIT_ID = B.BIT_ID INNER JOIN " +
                                    " dbo.CMS ON D.CM_POSITION_ID = dbo.CMS.CM_POSITION_ID INNER JOIN " +
                                    " dbo.EMPLOYEES ON dbo.CMS.EMPLOYEE_ID = dbo.EMPLOYEES.EMPLOYEE_ID INNER JOIN " +
                                    " dbo.PERSONS ON dbo.EMPLOYEES.PERSON_ID = dbo.PERSONS.PERSON_ID " +
                                    " WHERE     (B.DISTRIBUTION_HOUSE_ID = "+ distributionHouseId +") AND (P.POSITION_TYPE_ID = 5) " +
                                    " ORDER BY PositionName, BitId ";


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

        public static List<HouseSpecificCmPositionAndBit> GetHouseSpecificUnassignedPositionList(int distributionHouseId)
        {
            List<HouseSpecificCmPositionAndBit> positions = new List<HouseSpecificCmPositionAndBit>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                //string sqlSelect = " SELECT PositionId,PositionName,BitId, BitName "
                //                  + " FROM dbo.viewHouseSpecificCmPositionAndBit WHERE HouseId = " + distributionHouseId + " ORDER BY PositionName, BitName";

                //string sqlSelect = "SELECT     CM_POSITION_ID AS PositionId, CM_POSITION_NAME AS PositionName, - 1 AS BitId, 'N/A' AS BitName "
                //                 + " FROM         dbo.CM_POSITIONS "
                //                 + " WHERE     (DISTRIBUTION_HOUSE_ID = "+ distributionHouseId +") AND (CM_POSITION_ID NOT IN "
                //                 + "   (SELECT     CM_POSITION_ID    FROM          dbo.DEFAULT_CM_WISE_PLANS)) AND (POSITION_TYPE_ID = 5) "
                //                 + " ORDER BY PositionName";


                string sqlSelect = "SELECT dbo.CM_POSITIONS.CM_POSITION_ID AS PositionId, dbo.CM_POSITIONS.CM_POSITION_NAME + ' (' + dbo.PERSONS.PERSON_NAME + ')' AS PositionName, - 1 AS BitId, 'N/A' AS BitName FROM         dbo.CM_POSITIONS INNER JOIN  dbo.CMS ON dbo.CM_POSITIONS.CM_POSITION_ID = dbo.CMS.CM_POSITION_ID INNER JOIN dbo.EMPLOYEES ON dbo.CMS.EMPLOYEE_ID = dbo.EMPLOYEES.EMPLOYEE_ID INNER JOIN dbo.PERSONS ON dbo.EMPLOYEES.PERSON_ID = dbo.PERSONS.PERSON_ID WHERE     (dbo.CM_POSITIONS.DISTRIBUTION_HOUSE_ID = "+ distributionHouseId +") AND (dbo.CM_POSITIONS.POSITION_TYPE_ID = 5) AND (dbo.CM_POSITIONS.CM_POSITION_ID NOT IN (SELECT     CM_POSITION_ID  FROM          dbo.DEFAULT_CM_WISE_PLANS)) ORDER BY PositionName";


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

        public static List<HouseSpecificCmPositionAndBit> GetHouseSpecificUnassignedBitList(int distributionHouseId)
        {
            List<HouseSpecificCmPositionAndBit> positions = new List<HouseSpecificCmPositionAndBit>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                //string sqlSelect = " SELECT PositionId,PositionName,BitId, BitName "
                //                  + " FROM dbo.viewHouseSpecificCmPositionAndBit WHERE HouseId = " + distributionHouseId + " ORDER BY PositionName, BitName";

                //string sqlSelect = " SELECT     TOP (100) PERCENT - 1 AS PositionId, 'N/A' AS PositionName, BIT_ID AS BitId, BIT_NAME AS BitName "
                //                 + " FROM         dbo.BITS "
                //                 + " WHERE     (DISTRIBUTION_HOUSE_ID = "+ distributionHouseId +") AND (BIT_ID NOT IN "
                //                 + " (SELECT     BIT_ID   FROM          dbo.DEFAULT_CM_WISE_PLANS)) "
                //                 + " ORDER BY BitId";


              string sqlSelect = "SELECT - 1 AS PositionId, 'N/A' AS PositionName, BIT_ID AS BitId, BIT_NAME AS BitName FROM dbo.BITS WHERE (DISTRIBUTION_HOUSE_ID = "+ distributionHouseId +") AND (BIT_ID NOT IN (SELECT D.BIT_ID FROM dbo.DEFAULT_CM_WISE_PLANS AS D INNER JOIN dbo.CM_POSITIONS AS C ON D.CM_POSITION_ID = C.CM_POSITION_ID WHERE (C.POSITION_TYPE_ID = 5))) ORDER BY BitId ";


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


        //Get House And Position Specific Position And Bit List
        public static List<HouseSpecificCmPositionAndBit> GetHouseAndPositionTypeSpecificPositionAndBitList(int distributionHouseId, int positionTypeId)
        {
            List<HouseSpecificCmPositionAndBit> bits = new List<HouseSpecificCmPositionAndBit>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = " SELECT CP.CM_POSITION_ID AS PositionId, CP.CM_POSITION_NAME AS PositionName, ISNULL(DCWP.BIT_ID, - 1) AS BitId, ISNULL(B.BIT_NAME, 'N/A') AS BitName FROM         dbo.BITS AS B INNER JOIN                       dbo.DEFAULT_CM_WISE_PLANS AS DCWP ON B.BIT_ID = DCWP.BIT_ID RIGHT OUTER JOIN dbo.CM_POSITIONS AS CP ON DCWP.CM_POSITION_ID = CP.CM_POSITION_ID WHERE     (CP.DISTRIBUTION_HOUSE_ID = "+ distributionHouseId +") AND (CP.POSITION_TYPE_ID = "+ positionTypeId +")";

                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            bits.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }
            return bits;
        }
    }
}