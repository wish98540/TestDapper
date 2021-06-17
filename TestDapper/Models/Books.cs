using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Linq;
using Dapper;

namespace TestDapper.Models
{
    public class Books
    {
        public DateTime AddDate { get; set; }
        public string Author { get; set; }
        public string Country { get; set; }
        public int Id { get; set; }
        public string IsRead { get; set; }
        public string Name { get; set; }
        private string _isbn;
        public string ISBN
        {
            get; set;
            //get
            //{
            //    return _isbn;
            //}
            //set
            //{
            //    _isbn = value.Trim();
            //}
        }

        public void Create(Books book)
        {
            var sql = @"
                INSERT INTO [Test].[dbo].[Books]
	                ([Name], [Author], [Country], [IsRead], [ISBN])
                VALUES
	                (@Name, @Author, @Country, @IsRead, @ISBN)
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            conn.Execute(sql, new { Name = book.Name, Author = book.Author, Country = book.Country, IsRead = book.IsRead, ISBN = book.ISBN });
        }

        public void CreateBatch1(List<Books> books)
        {
            var sql = @"
                INSERT INTO [Test].[dbo].[Books]
	                ([Name], [Author], [Country], [IsRead], [ISBN])
                VALUES
	                (@Name, @Author, @Country, @IsRead, @ISBN)
            ";

            #region Before C# 8.0

            //using(var conn = Connection.GetConnection())
            //{
            //    conn.Open();
            //    using(var tran = conn.BeginTransaction())
            //    {
            //        var bookArray = new[] {
            //        new { Name = books[0].Name, Author = books[0].Author, Country = books[0].Country, IsRead = books[0].IsRead },
            //        new { Name = books[1].Name, Author = books[1].Author, Country = books[1].Country, IsRead = books[1].IsRead }
            //    };
            //        try
            //        {
            //            conn.Execute(sql, bookArray, tran);
            //            tran.Commit();
            //            tran.Rollback();
            //        }
            //        catch (Exception)
            //        {
            //            tran.Rollback();
            //            throw;
            //        }
            //    }
            //}

            #endregion Before C# 8.0

            using var conn = Connection.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            // 匿名參數陣列
            // new[] {new{...}, new{...}, ...}
            var bookArray = new[] {
                    new { Name = books[0].Name, Author = books[0].Author, Country = books[0].Country, IsRead = books[0].IsRead, ISBN = books[0].ISBN },
                    new { Name = books[1].Name, Author = books[1].Author, Country = books[1].Country, IsRead = books[1].IsRead, ISBN = books[1].ISBN }
                };
            try
            {
                conn.Execute(sql, bookArray, tran);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public void CreateBatch2(List<Books> books)
        {
            var sql = @"
                INSERT INTO [Test].[dbo].[Books]
	                ([Name], [Author], [Country], [IsRead], [ISBN])
                VALUES
	                (@Name, @Author, @Country, @IsRead, @ISBN)
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            // 將參數包裹成 參數清單，每個 DynamicParameters 代表一個 Object
            var parameterList = new List<DynamicParameters>();
            books.ForEach(book =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("Name", book.Name, DbType.String, ParameterDirection.Input, 50);
                parameters.Add("Author", book.Author, DbType.String, ParameterDirection.Input, 50);
                parameters.Add("Country", book.Country, DbType.AnsiString, ParameterDirection.Input, 50);
                parameters.Add("IsRead", book.IsRead, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                parameters.Add("ISBN", book.ISBN, DbType.AnsiStringFixedLength, ParameterDirection.Input, 13);
                parameterList.Add(parameters);
            });

            try
            {
                conn.Execute(sql, parameterList, tran);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public void CreateBatch3(List<Books> books)
        {
            var sql = @"
                INSERT INTO [Test].[dbo].[Books]
	                ([Name], [Author], [Country], [IsRead], [ISBN])
                VALUES
	                (@Name, @Author, @Country, @IsRead, @ISBN)
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                conn.Execute(sql, books, tran);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public void CreateByTransaction1(List<Books> books)
        {
            var sql = @"
                INSERT INTO [Test].[dbo].[Books]
	                ([Name], [Author], [Country], [IsRead])
                VALUES
	                (@Name, @Author, @Country, @IsRead)
            ";

            #region Before C# 8.0

            //using (var conn = Connection.GetConnection())
            //{
            //    conn.Open();
            //    using (var tran = conn.BeginTransaction())
            //    {
            //        try
            //        {
            //            conn.Execute(sql, books, tran);
            //            tran.Commit();
            //        }
            //        catch (Exception)
            //        {
            //            tran.Rollback();
            //            throw;
            //        }
            //    }
            //}

            #endregion Before C# 8.0

            using var conn = Connection.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                // Execute(sql, params, transaction, timeout, commandType)
                conn.Execute(sql, books, tran);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public void CreateByTransaction2(List<Books> books)
        {
            // TODO Transaction Scope 分散式交易
            var sql = @"
                INSERT INTO [Test].[dbo].[Books]
	                ([Name], [Author], [Country], [IsRead])
                VALUES
	                (@Name, @Author, @Country, @IsRead)
            ";

            try
            {
                // using System.Transactions
                // 使用交易區塊，處理分散式交易
                using var tranScope = new TransactionScope();
                using var conn1 = Connection.GetConnection();
                conn1.Open();
                // 使用 TransactionScope 不用在參數帶 Transaction
                conn1.Execute(sql, books);

                // 當 conn1 的事務處理完成才開啟 conn2 連線
                // 在跨資料庫伺服器交易時，透過 MSDTC (分散式交易調節器) 確保跨伺服器操作可以受到分散式交易調節器保護
                using var conn2 = Connection.GetOtherConnection();
                conn2.Open();
                var ids = books.Select(x => x.Id).ToArray();
                var sql2 = "UPDATE ... FROM ... WHERE [Id] IN @Ids ";
                conn2.Execute(sql2, new { Ids = ids });

                // 透過 TransactionScope 調節跨資料庫伺服器交易
                // 全部操作成功，才承認交易
                // 任一操作失敗，全部 RollBack
                tranScope.Complete();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CreateWithObject(Books book)
        {
            var sql = @"
                INSERT INTO [Test].[dbo].[Books]
	                ([Name], [Author], [Country], [IsRead])
                VALUES
	                (@Name, @Author, @Country, @IsRead)
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            conn.Execute(sql, book);
        }

        public void DeleteById()
        {
            var sql = @"
                DELETE FROM  [Test].[dbo].[Books]
                WHERE [Id] IN @Ids
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                var bookName = "全部一起修改~~";
                var ids = new List<int> { 1, 2, 3, 4, 5 };
                conn.Execute(sql, new { Ids = ids }, tran);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public Books Get(string name)
        {
            var sql = @"
                SELECT * FROM [Test].[dbo].[Books]
                WHERE [Name] = @Name
            ";
            var conn = Connection.GetConnection();
            return conn.QueryFirstOrDefault<Books>(sql, new { Name = name });
        }

        public Books GetByCountry(string country)
        {
            var sql = @"
                SELECT * FROM [Test].[dbo].[Books]
                WHERE [Country] = @Country
            ";
            var conn = Connection.GetConnection();
            return conn.QueryFirstOrDefault<Books>(sql, new { Country = country });
        }

        public Books GetById(int id)
        {
            var sql = @"
                SELECT * FROM [Test].[dbo].[Books]
                WHERE [Id] = @Id
            ";
            var conn = Connection.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);
            return conn.QueryFirstOrDefault<Books>(sql, parameters);
        }

        public Books GetById_UseStoreProcedure()
        {
            var sql = @"[Test].[dbo].[usp_GetBookById]";
            var conn = Connection.GetConnection();
            var id = 1007;
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);
            return conn.QueryFirstOrDefault<Books>(sql, parameters, commandType: CommandType.StoredProcedure);
        }

        public Books GetByIsRead(string isRead)
        {
            var sql = @"
                SELECT * FROM [Test].[dbo].[Books]
                WHERE [IsRead] = @IsRead
            ";
            var conn = Connection.GetConnection();
            return conn.QueryFirstOrDefault<Books>(sql, new { IsRead = isRead });
        }

        public Books GetByName(string name)
        {
            var sql = @"
                SELECT * FROM [Test].[dbo].[Books]
                WHERE [Name] = @Name
            ";
            var conn = Connection.GetConnection();
            // 宣告 DynamicParameter 物件
            var parameters = new DynamicParameters();
            // 新增參數 DynamicParameters.Add(string name, object value, DBType? dbType, ParameterDirection direction, int? size);
            parameters.Add("Name", name, DbType.String, ParameterDirection.Input, 50);
            return conn.QueryFirstOrDefault<Books>(sql, parameters);
        }

        public DataTable GetDataTable()
        {
            var sql = @"
                SELECT * FROM [Test].[dbo].[Books]
                WHERE [Id] IN @Ids
            ";
            using var conn = Connection.GetConnection();
            var ids = new List<int> { 1, 2, 3, 4, 5 };
            var table = new DataTable("Books");
            // 取得 IDataReader
            using var dataReader = conn.ExecuteReader(sql, new { Ids = ids });
            // 將 IDataReader 加載至 Table 中
            table.Load(dataReader);
            return table;
        }

        public int GetIdByScalar()
        {
            var sql = @"
                SELECT TOP (1000) [Id]
                      ,[Name]
                      ,[Author]
                      ,[Country]
                      ,[IsRead]
                      ,[AddDate]
                  FROM [Test].[dbo].[Books]
            ";
            using var conn = Connection.GetConnection();
            // 返回 第一筆資料，第一個欄位，因此會返回 ID(1)
            return conn.ExecuteScalar<int>(sql);
        }

        public List<Books> GetInAddDateList()
        {
            var sql = @"
                SELECT * FROM [Test].[dbo].[Books]
                WHERE [AddDate] IN @AddDate
            ";
            var AddDate = new List<DateTime> {
                new DateTime(2021, 06, 12),
                new DateTime(2021, 06, 13),
                new DateTime(2021, 06, 14),
                new DateTime(2021, 06, 17)
            };

            var conn = Connection.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("AddDate", AddDate);

            return conn.Query<Books>(sql, parameters).ToList();
        }

        public List<Books> GetInIdList1()
        {
            var sql = @"
                SELECT * FROM [Test].[dbo].[Books]
                WHERE [Id] IN @Ids
            ";
            var conn = Connection.GetConnection();
            var ids = new List<int> { 1, 2, 3, 4, 5 };
            return conn.Query<Books>(sql, new { Ids = ids }).ToList();
        }

        public List<Books> GetInIdList2()
        {
            var sql = @"
                SELECT * FROM [Test].[dbo].[Books]
                WHERE [Id] IN @Ids
            ";
            var ids = new List<int> { 1, 2, 3, 4, 5 };

            var conn = Connection.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Ids", ids);

            return conn.Query<Books>(sql, parameters).ToList();
        }

        /// <summary>
        /// 不可行
        /// </summary>
        /// <returns></returns>
        public List<Books> QueryBatch1()
        {
            var sql = @"
                SELECT * FROM [Test].[dbo].[Books]
                WHERE [Id] = @Id
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            var ids = new List<int> { 1, 2, 3, 4, 5 };

            return conn
                .Query<Books>(
                sql,
                new[] {
                    new { Id = 1},
                    new { Id = 2},
                    new { Id = 3},
                    new { Id = 4},
                    new { Id = 5}
                })
                .ToList();
        }

        /// <summary>
        /// 不可行
        /// </summary>
        /// <returns></returns>
        public List<Books> QueryBatch2()
        {
            var sql = @"
                SELECT TOP 1 * FROM [Test].[dbo].[Books]
                WHERE [Id] = @Id
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            var ids = new List<int> { 1, 2, 3, 4, 5 };

            return conn
                .Query<Books>(sql, ids)
                .ToList();
        }

        public List<Books> QueryBatch3()
        {
            var sql = @"
                SELECT TOP 1 * FROM [Test].[dbo].[Books]
                WHERE [Id] = @Id
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            var ids = new List<int> { 1, 2, 3, 4, 5 };
            var parameterList = new List<DynamicParameters>();
            ids.ForEach(id =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);
                parameterList.Add(parameters);
            });

            return conn.Query<Books>(sql, parameterList).ToList();
        }

        public List<Books> QueryBatch4()
        {
            var sql = @"
                SELECT TOP 1 * FROM [Test].[dbo].[Books]
                WHERE [Id] = @Id
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            var result = new List<Books>();
            var ids = new List<int> { 1, 2, 3, 4, 5 };
            //foreach (var id in ids)
            //{
            //    var book = conn.QueryFirstOrDefault<Books>(sql, new { Id = id });
            //    result.Add(book);
            //}

            // 使用LINQ的ForEach
            ids.ForEach(id =>
            {
                var book = conn.QueryFirstOrDefault<Books>(sql, new { Id = id });
                result.Add(book);
            });
            return result;
        }

        public List<Books> QueryBatch5()
        {
            var sql = @"
                SELECT TOP 1 * FROM [Test].[dbo].[Books]
                WHERE [Id] = @Id
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            var ids = new List<int> { 1, 2, 3, 4, 5 };

            // 使用 LINQ 的 Select() 功能
            // 將 ids 的每個 id 映射為 Book，返回 IEnumerable<Book>
            var result = ids.Select<int, Books>(id =>
             {
                 return conn.QueryFirstOrDefault<Books>(sql, new { Id = id });
             }).ToList();

            return result;
        }

        public List<Books> QueryBatch6()
        {
            var ids = new List<int> { 1, 2, 3, 4, 5 };
            var result = ids.Select(id =>
            {
                return GetById(id);
            }).ToList();

            return result;
        }

        public void UpdateById()
        {
            var sql = @"
                UPDATE [Test].[dbo].[Books]
                SET [Name] = @BookName
                WHERE [Id] = @Id
            ";

            using var conn = Connection.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                var bookName = "全部一起修改~~";
                var ids = new List<int> { 1, 2, 3, 4, 5 };
                var parameterList = ids.Select(id =>
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("BookName", bookName, DbType.String, ParameterDirection.Input, 50);
                    parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);
                    return parameters;
                }).ToList();
                conn.Execute(sql, parameterList, tran);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }
    }
}