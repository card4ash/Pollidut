using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class EmployeeRoleStatus
    {
        public Int32 EmployeeRoleStatusId { get; set; }
        public String EmployeeRoleStatusName { get; set; }

    }

    public class EmployeeRoleStatusManager
    {
        private static EmployeeRoleStatus FillEntity(SqlDataReader reader)
        {
            return new EmployeeRoleStatus { EmployeeRoleStatusId = Convert.ToInt32(reader["EmployeeRoleStatusId"]), EmployeeRoleStatusName = reader["EmployeeRoleStatusName"].ToString() };
        }

        public static List<EmployeeRoleStatus> GetEmployeeRoleStatuses()
        {
            List<EmployeeRoleStatus> RoleStatuses = new List<EmployeeRoleStatus>();
            //  Designations.Add(new Designation { DesignationId = -1, DesignationName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select EMPLOYEE_ROLE_STATUS_ID AS EmployeeRoleStatusId, EMPLOYEE_ROLE_STATUS_NAME AS EmployeeRoleStatusName from EMPLOYEE_ROLE_STATUSES where EMPLOYEE_ROLE_STATUS_ID <> 0 order by EMPLOYEE_ROLE_STATUS_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            RoleStatuses.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            RoleStatuses.Add(new EmployeeRoleStatus { EmployeeRoleStatusId = 0, EmployeeRoleStatusName = "None" });
            return RoleStatuses;
        }
    }

}