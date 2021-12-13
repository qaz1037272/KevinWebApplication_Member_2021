using System;
using System.Web.Mvc;   //輸出 Json & MVC
using System.Data.SqlClient; //建立資料庫連接物件
using System.Data.Common; //執行資料庫查詢物件
using System.Data; //DataSet資料中快取物件
using System.Security.Cryptography; //雜湊函數物件
using System.Text; //建立StringBuilder物件
using Newtonsoft.Json;

using static KevinWebApplication_Member.Models.MemberModels;


namespace KevinWebApplication_Member.Controllers
{
    public class MemberController : Controller
    {
        // GET: Member
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult DoRegister(DoRegisterIn inModel)
        {
            DoRegisterOut outModel = new DoRegisterOut(); //KevinWebApplication_Member.Models.MemberModels的類別物件
            if (String.IsNullOrEmpty(inModel.UserID) || String.IsNullOrEmpty(inModel.UserMail) || String.IsNullOrEmpty(inModel.UserName) || String.IsNullOrEmpty(inModel.UserPwd))
            {
                outModel.ErrMsg = "請輸入資料";
            }
            else
            {
                //建立資料庫連線
                SqlConnection ConnSql = null;
                try
                {
                    String ConnStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;  //呼叫Web.config資料庫連線組態設定
                    ConnSql = new SqlConnection();
                    ConnSql.ConnectionString = ConnStr;
                    ConnSql.Open(); //開啟資料庫的連線

                    //建立資料庫查詢的T-SQL陳述式
                    String sql = "select * from Member where UserID = @UserID";
                    SqlCommand cmd = new SqlCommand(); //對資料庫陳述需建立T-SQL物件指令
                    cmd.CommandText = sql;   //等同cmd.CommandText ="select * from Member where UserID = @UserID" 備註:T-SQL程式陳述指令
                    cmd.Connection = ConnSql;

                    //T-SQL陳述式內填參數值
                    cmd.Parameters.AddWithValue("@UserID", inModel.UserID);

                    //執行資料庫查詢
                    DbDataAdapter adpt = new SqlDataAdapter(); //DbDataAdapter adpt = new SqlDataAdapter("select * from Member where UserID = @UserID") 
                    adpt.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    //查詢到結果資料列加入Dataset 記憶體快取
                    adpt.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        outModel.ResultMsg = "帳號已經存在!!!!";
                    }
                    else
                    {
                        //將密碼使用sha256 不可逆加密存到sql
                        String ppw = inModel.UserID.Substring(0, 1); //UserID前一碼提取
                        SHA256 sha256 = SHA256.Create();
                        byte[] bytesPwd = Encoding.UTF8.GetBytes(ppw + inModel.UserPwd);
                        byte[] bytesHash = sha256.ComputeHash(bytesPwd);
                        StringBuilder hashResult = new StringBuilder();
                        for (int i = 0; i < bytesHash.Length; i++)
                        {
                            //bytesHash陣列的hash值附加至String(X2意思X為16進制 2為最少兩位元  如0X2A=取2A )
                            hashResult.Append(bytesHash[i].ToString("X2"));
                        }
                        //sha256雜湊函數計算完得到新的Password
                        String newPwd = hashResult.ToString();

                        //Member資料表新增使用者註冊資料
                        sql = @"INSERT INTO Member(UserID,UserPwd,UserName,UserMail) VALUES (@UserID,@UserPwd,@UserName,@UserMail)";
                        cmd.CommandText = sql;
                        cmd.Connection = ConnSql;
                        cmd.Parameters.AddWithValue("@UserID", inModel.UserID);
                        cmd.Parameters.AddWithValue("@UserPwd", newPwd);
                        cmd.Parameters.AddWithValue("@UserName", inModel.UserName);
                        cmd.Parameters.AddWithValue("@UserMail", inModel.UserMail);
                        //針對資料庫的資料表執行更新資料動作
                        cmd.ExecuteNonQuery();
                        outModel.ResultMsg = inModel.UserID + "完成註冊資料";
                    }
                }
                catch (Exception e)
                {
                    //執回例外狀況
                    throw e;
                }
                finally
                {
                    if (ConnSql != null)
                    {
                        ConnSql.Close();  //關閉資料庫連線
                        ConnSql.Dispose();  //釋放ConnSql物件的所有資源
                    }
                }


            }
            //物件outModel產出訊息都輸出成Json格式
            ContentResult resultJson = new ContentResult();
            resultJson.ContentType = "application/json";
            resultJson.Content = JsonConvert.SerializeObject(outModel);
            return resultJson;
        }



    }

}