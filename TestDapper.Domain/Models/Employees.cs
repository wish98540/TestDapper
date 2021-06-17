using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TestDapper.Models
{
    public class Employees
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
    }
}