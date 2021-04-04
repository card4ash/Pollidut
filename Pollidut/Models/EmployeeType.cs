using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class EmployeeType
    {
        public Int32 EmployeeTypeId { get; set; }
        public String EmployeeTypeName { get; set; }
    }

    public class EmployeeTypeManager
    {
        private static EmployeeType FillEntity(SqlDataReader reader)
        {
            return new EmployeeType { EmployeeTypeId = Convert.ToInt32(reader["EmployeeTypeId"]), EmployeeTypeName = reader["EmployeeTypeName"].ToString() };
        }

        public static List<EmployeeType> GetEmployeeTypes()
        {
            List<EmployeeType> EmployeeTypes = new List<EmployeeType>();
            //  EmployeeTypes.Add(new EmployeeType { EmployeeTypeId = -1, EmployeeTypeName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select EMPLOYEE_TYPE_ID AS EmployeeTypeId, EMPLOYEE_TYPE_NAME AS EmployeeTypeName from EMPLOYEE_TYPES where EMPLOYEE_TYPE_ID>0  order by EMPLOYEE_TYPE_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            EmployeeTypes.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

           // EmployeeTypes.Add(new EmployeeType { EmployeeTypeId = 0, EmployeeTypeName = "None" });
            return EmployeeTypes;
        }
    }
}