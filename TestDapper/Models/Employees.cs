﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace TestDapper.Models
{
    internal class Employees
    {
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// 員工編號
        /// </summary>
        public int BusinessEntityID { get; set; }

        /// <summary>
        /// 性別
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 雇傭日
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// 職稱
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// 登入帳號
        /// </summary>
        public string LoginID { get; set; }

        /// <summary>
        /// 編輯日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// 身分證
        /// </summary>
        public string NationalIDNumber { get; set; }

        public Employees Get()
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT TOP 1 * FROM [AdventureWorks2016].[HumanResources].[Employee] e WHERE [e].[Gender] = 'F'";
                return conn.QueryFirstOrDefault<Employees>(sql);
            }
        }

        public IQueryable<Employees> GetAll()
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT * FROM [AdventureWorks2016].[HumanResources].[Employee] e";
                return conn.Query<Employees>(sql).AsQueryable();
            }
        }

        public dynamic GetDynamic()
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var sql = @"
                    SELECT TOP 1
                        [BusinessEntityID]
                      ,[NationalIDNumber]
                      ,[LoginID]
                      ,[JobTitle]
                      ,[BirthDate]
                      ,[Gender]
                      ,[HireDate]
                      ,[ModifiedDate]
                    FROM [AdventureWorks2016].[HumanResources].[Employee] e";
                return conn.Query(sql).FirstOrDefault();
            }
        }

        public Employees GetFirst()
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT * FROM [AdventureWorks2016].[HumanResources].[Employee] e WHERE [LoginID] = @LoginID";
                var parameters = new DynamicParameters();
                parameters.Add("LoginID", @"focus-logistics\willy");
                return conn.QueryFirst<Employees>(sql, parameters);
            }
        }

        public Employees GetFirstEmployeeAndDepartments(out List<Departments> departments)
        {
            using var conn = Connection.GetConnection();
            var sql = @"
                SELECT TOP 1 * FROM [AdventureWorks2016].[HumanResources].[Employee]
                SELECT * FROM [AdventureWorks2016].[HumanResources].[Department]
            ";
            using (var multiQuery = conn.QueryMultiple(sql))
            {
                var employee = multiQuery.Read<Employees>().FirstOrDefault();
                departments = multiQuery.Read<Departments>().ToList();
                return employee;
            }
        }

        public Employees GetFirstOrDefault()
        {
            #region Before C# 8.0
            //// 一般寫法，自行控制 Connection週期
            //using (var conn = Connection.GetConnection())
            //{
            //    // 顯示開啟
            //    conn.Open();
            //    var sql = @"SELECT * FROM [AdventureWorks2016].[HumanResources].[Employee] e WHERE [LoginID] = @LoginID";
            //    var parameters = new DynamicParameters();
            //    parameters.Add("LoginID", @"willy");
            //    return conn.QueryFirstOrDefault<Employees>(sql, parameters);
            //}
            #endregion

            //C# 8.0 寫法(語法糖)
            //using var 會在方法的大括號內，於編譯時補全為 using(var obj = ...){ //Doing Something }
            using var conn = Connection.GetConnection();
            conn.Open();
            var sql = @"SELECT * FROM [AdventureWorks2016].[HumanResources].[Employee] e WHERE [e].[Gender] = @Gender";
            return conn.QueryFirstOrDefault<Employees>(sql, new { Gender = "M" });
        }

        public async Task<Employees> GetFirstOrDefaultAsync()
        {
            var conn = Connection.GetConnection();
            //conn.Open();  // 使用隱式開啟DBConnection，Connection由Dapper控制
            var sql = @"SELECT * FROM [AdventureWorks2016].[HumanResources].[Employee] e WHERE [LoginID] = @LoginID";
            var parameters = new DynamicParameters();
            parameters.Add("LoginID", @"willy");
            return await conn.QueryFirstOrDefaultAsync<Employees>(sql, parameters);
        }

        public Employees GetSingle()
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT * FROM [AdventureWorks2016].[HumanResources].[Employee] e WHERE [e].[Gender] = 'F'";
                return conn.QuerySingle<Employees>(sql);
            }
        }

        public Employees GetSingleOrDefault()
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT * FROM [AdventureWorks2016].[HumanResources].[Employee] e WHERE [e].[Gender] = 'F'";
                return conn.QuerySingleOrDefault<Employees>(sql);
            }
        }
    }
}