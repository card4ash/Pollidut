using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class Pollidut
    {
        public int PollidutId { get; set; }
        public String PollidutName { get; set; }
        public int DistributionHouseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Target { get; set; }
        public double OrderQty { get; set; }
        public double Acheivement { get; set; }

        // public DistributionHouse() { }
    }

    public class PollidutManager
    {
        private static Pollidut FillEntity(SqlDataReader reader)
        {
            return new Pollidut { PollidutId =Convert.ToInt32(reader["PollidutId"].ToString()), PollidutName = reader["PollidutName"].ToString() };
        }
        public static List<Pollidut> GetSupervisorSpecificPollidut(int supId)
        {
            List<Pollidut> polliduts = new List<Pollidut>();

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "SELECT        dbo.EMPLOYEES.EMPLOYEE_ID, dbo.PERSONS.PERSON_NAME FROM            dbo.EMPLOYEES INNER JOIN "
                         +" dbo.PERSONS ON dbo.EMPLOYEES.PERSON_ID = dbo.PERSONS.PERSON_ID WHERE        (dbo.EMPLOYEES.SUPERVISOR_ID = "+supId+")";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            polliduts.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }
            return polliduts;
        }

    }
}