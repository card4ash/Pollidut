using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class PersonCurrentStatus
    {
        public Int32 PersonCurrentStatusId { get; set; }
        public String PersonCurrentStatusName { get; set; }

        // public DistributionHouse() { }
    }


    public class PersonCurrentStatusManager
    {
        private static PersonCurrentStatus FillEntity(SqlDataReader reader)
        {
            return new PersonCurrentStatus { PersonCurrentStatusId = Convert.ToInt32(reader["PersonCurrentStatusId"]), PersonCurrentStatusName = reader["PersonCurrentStatusName"].ToString() };
        }

        public static List<PersonCurrentStatus> GetStatuses()
        {
            List<PersonCurrentStatus> sectons = new List<PersonCurrentStatus>();
            sectons.Add(new PersonCurrentStatus { PersonCurrentStatusId = -1, PersonCurrentStatusName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select Person_Current_Status_ID AS PersonCurrentStatusId, Person_Current_Status_NAME AS PersonCurrentStatusName from PERSON_CURRENT_STATUSES where Person_Current_Status_ID>0 order by Person_Current_Status_NAME ASC";
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

            sectons.Add(new PersonCurrentStatus { PersonCurrentStatusId = 0, PersonCurrentStatusName = "None" });
            return sectons;
        }
    }
}