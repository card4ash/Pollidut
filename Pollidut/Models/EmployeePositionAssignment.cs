using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{

    /// <summary>
    /// This class is used to assign/remove a position to/from an employee
    /// </summary>
    public class EmployeePositionAssignment
    {
        private Int32 pPositionId; private Int32 pEmployeeId;
        private String ConnctionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        private String pAssignTo = String.Empty;

        /// <summary>
        /// if any employee has already assigned to a position, this will return that position name
        /// </summary>
        public String PreviouslyAssignedPositionName
        {
            get { return pAssignTo; }
        }

        public EmployeePositionAssignment(Int32 positionId, Int32 employeeId)
        {
            this.pPositionId = positionId;
            this.pEmployeeId = employeeId;
        }


        /// <summary>
        /// Assign an employee to a position
        /// </summary>
        /// <returns></returns>
        public AssignmentResult AssignEmployee()
        {
            AssignmentResult result;

            using (SqlConnection connection = new SqlConnection(ConnctionString))
            {
                String sqlCheck = "SELECT CMP.CM_POSITION_NAME AS PositionName "
                                + " FROM   dbo.CMS INNER JOIN dbo.CM_POSITIONS AS CMP ON dbo.CMS.CM_POSITION_ID = CMP.CM_POSITION_ID "
                                + " WHERE (dbo.CMS.EMPLOYEE_ID = " + pEmployeeId + ")";


                String sqlInsert = "Insert into CMS (EMPLOYEE_ID, CM_POSITION_ID) values(" + pEmployeeId + "," + pPositionId + ")";
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection; command.CommandText = sqlCheck; connection.Open();
                    Object checkResult = command.ExecuteScalar();

                    if (checkResult != null && checkResult != DBNull.Value)
                    {
                         result = AssignmentResult.AlreadyAssigned;
                        pAssignTo = checkResult.ToString();
                      
                    }
                    else
                    {
                         command.CommandText = sqlInsert;
                        command.ExecuteNonQuery();
                        result = AssignmentResult.Success;
                    }
                }
            }
            return result;
        }



        public AssignmentResult RemoveEmployee()
        {
            AssignmentResult result;

            using (SqlConnection connection = new SqlConnection(ConnctionString))
            {

                String sqlRemove = "delete from CMS where EMPLOYEE_ID=" + pEmployeeId + " AND CM_POSITION_ID=" + pPositionId + "";
                // String sqlInsert = "Insert into CMS (EMPLOYEE_ID, CM_POSITION_ID) values(" + pEmployeeId + "," + pPositionId + ")";
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection; command.CommandText = sqlRemove; connection.Open();

                    command.ExecuteNonQuery();
                    result = AssignmentResult.Success;
                }
            }
            return result;
        }
    }

    public enum AssignmentResult
    {
        Success, AlreadyAssigned
    }
}