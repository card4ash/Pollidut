using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Pollidut.Models
{
    public class Bank
    {
        public Int32 BankId { get; set; }
        public String BankName { get; set; }
    }

    public class BankManager
    {
        private static Bank FillEntity(SqlDataReader reader)
        {
            return new Bank { BankId = Convert.ToInt32(reader["BankId"]), BankName = reader["BankName"].ToString() };
        }

        public static List<Bank> GetBanks()
        {
            List<Bank> Banks = new List<Bank>();
            //  Designations.Add(new Designation { DesignationId = -1, DesignationName = "select" });

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sqlSelect = "select BANK_ID AS BankId, BANK_NAME AS BankName from BANKS where Bank_id <> 0 order by BANK_NAME ASC";
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Banks.Add(FillEntity(reader));
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                }
            }

            //Brands.Add(new Brand { BrandId = 0, BrandName = "None" });
            return Banks;
        }
    }
}