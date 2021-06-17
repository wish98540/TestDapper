using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TestDapper.Models;

namespace TestDapper
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("應用程序執行中...");

                #region GetFirstOrDefaultAsync

                //var empObj = new Employees();
                //Console.WriteLine("開始取得物件");
                //var employeesTask = empObj.GetFirstOrDefaultAsync();
                ////var dynamic = await empObj.GetFirstOrDefault();
                //Console.WriteLine("開始序列化物件");
                //Console.WriteLine("等待取回物件");
                //var employees = await employeesTask;
                //Console.WriteLine("序列化物件");
                //var json = JsonConvert.SerializeObject(employees, Formatting.Indented);
                //Console.WriteLine(json);

                #endregion GetFirstOrDefaultAsync

                #region Get

                //var empObj = new Employees();
                //var employees = empObj.GetFirstOrDefault();
                ////var employees = empObj.GetFirstEmployeeAndDepartments(out List<Departments> departments);
                //var json = JsonConvert.SerializeObject(employees, Formatting.Indented);
                //Console.WriteLine(json);
                ////var jsonDepartments = JsonConvert.SerializeObject(departments, Formatting.Indented);
                ////Console.WriteLine(jsonDepartments);

                #endregion Get

                #region Write
                var bookObj = new Books()
                {
                    Name = "如果超過欄位長度，使用NVARCHAR(4000)能寫進去嗎?!",
                    Author = "willy",
                    Country = "Taiwan",
                    IsRead = "Y"
                };
                var bookList = new List<Books> {
                    new Books
                    {
                        Author="Test",
                        Country = "Taiwan",
                        IsRead = "N",
                        Name="TestBook1"
                    },
                    new Books
                    {
                        Author="Test",
                        Country = "Korea",
                        IsRead = "N",
                        Name="TestBook2"
                    },
                    new Books
                    {
                        Author="Test",
                        Country = "Japan",
                        IsRead = "N",
                        Name="TestBook3"
                    }};
                //bookObj.CreateWithObject(bookObj);
                //var result = bookObj.GetByIsRead(bookObj.IsRead);
                //bookObj.CreateBatch2(bookList);
                //var result = bookObj.GetByCountry("Korea");
                bookObj.DeleteById();
                var result = bookObj.QueryBatch5();
                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine(json);

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Occur: {ex.GetType()}");
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: \n {ex.StackTrace}\n");
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}