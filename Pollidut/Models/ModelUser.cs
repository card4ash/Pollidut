using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Pollidut.Models
{
    public class ModelUser
    {
        public int UserId { get; set; }
        public String UserName { get; set; }

        public static List<ModelUser> GetUserList()
        {
            List<ModelUser> dataList = new List<ModelUser>();

            //ModelHouse selectItem = new ModelHouse { HouseId = 0, HouseName = "Select Area" };
            //houseList.Add(selectItem);

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                DataTable dtData = new DataTable();
                String sqlString = "SELECT USER_ID,USER_NAME FROM USERS";
                //connection.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.CommandText = sqlString;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = command;
                connection.Open();
                da.Fill(dtData);
                connection.Close();

                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    ModelUser item = new ModelUser { UserId = int.Parse(dtData.Rows[i]["USER_ID"].ToString()), UserName = dtData.Rows[i]["USER_NAME"].ToString() };
                    dataList.Add(item);
                }
            }
            return dataList;
        }

        public static List<ModelUser> GetUserList(String selectStringFormat)
        {
            List<ModelUser> dtData = new List<ModelUser>();

            ModelUser item = new ModelUser { UserId = 0, UserName = selectStringFormat };
            dtData.Add(item);

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                DataTable dtSections = new DataTable();
                String sqlString = "SELECT USER_ID,USER_NAME FROM USERS";
                //connection.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.CommandText = sqlString;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = command;
                connection.Open();
                da.Fill(dtSections);
                connection.Close();

                for (int i = 0; i < dtSections.Rows.Count; i++)
                {
                    item = new ModelUser { UserId = int.Parse(dtSections.Rows[i]["USER_ID"].ToString()), UserName = dtSections.Rows[i]["USER_NAME"].ToString() };
                    dtData.Add(item);
                }
            }
            return dtData;
        }
    }
}