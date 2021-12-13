using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KevinWebApplication_Member.Models
    //Member Models類別，將取得變數初始值
{
    public class MemberModels
    {
        public class DoRegisterIn
        {
            //get=取得資料方法，set=設定資料方法
            public String UserID { get; set; }
            public String UserPwd { get; set; }
            public String UserName { get; set; }
            public String UserMail { get; set; } 
            public DoRegisterIn()
            {
                UserID = String.Empty;
                UserPwd = String.Empty;
                UserName= String.Empty;
                UserMail = String.Empty;
                
            }

        }
        public class DoRegisterOut
        {
            public String ErrMsg { get; set; }
            public String ResultMsg { get; set; }
            public DoRegisterOut()
            {
                ErrMsg = String.Empty;
                ResultMsg = String.Empty;
            }
        }
    }
}