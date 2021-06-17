using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace TestDapper.Domain
{
    public static class Connection
    {
        public static IDbConnection GetConnection()
        {
            return new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AdventureWorks2016;Integrated Security=True;Application Name=Study_1");
        }

        public static IDbConnection GetOtherConnection()
        {
            return new SqlConnection();
        }
    }
}
