using ArtHandler.DAL;
using ArtHandler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtHandler.Repository
{
    public class ReportsRepository
    {
        /// <summary>
        /// get the user activities information
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public string GetUserActivityGSDDashboard(string userId, string mode,string startDate, string endDate)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetUserActivityGSDDashboard(mode, startDate, endDate); //post the parameter to data access layer 
                return JsonConvert.SerializeObject(ds);//userdetails dataset convert into JSON.
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        /// <summary>
        /// get the user registration details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserRegistrationGSDDashboard(string userId)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetUserRegistrationGSDDashboard();
                return JsonConvert.SerializeObject(ds);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        /// <summary>
        /// Month wise user registartion details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserRegistrationByMonth(string userId)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetUserRegistrationByMonth();
                return JsonConvert.SerializeObject(ds);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public static bool InsertGSDLog(string userid, string activity, string gsduserId)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                return objDALUser.InsertGSDLog(userid, activity, gsduserId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(gsduserId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        public string GetGSDActivityForDashboard(string userId, string mode,string startDate, string endDate)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetGSDActivityForDashboard(mode, startDate, endDate);
                return JsonConvert.SerializeObject(ds);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public string GetFrequentUserLock(string userId,string mode,string startDate, string endDate)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                UserRepository objUserRepo = new UserRepository();
                string result = string.Empty;
                List<FrequentAccountlockoutUser> lstUser = objDALUser.GetFrequentUserLock(mode, startDate, endDate);

                if (lstUser != null)
                {
                    lstUser.ForEach(c =>
                    {
                        c.UserName = objUserRepo.GetUserInfoFromAD(c.UserId, Constants.ADGIVENNAME);
                    });
                    result = JsonConvert.SerializeObject(lstUser);
                }
                else
                {
                    result = "";
                }

                return result;

            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public void DownloadGSDActivityLog(string mode,string startDt, string endDt)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetGSDActivityLog(mode, startDt, endDt);

                string facsCsv = GenerateCsvStringForGSDActivity(ds);

                // Return the file content with response body. 
                System.Web.HttpContext.Current.Response.ContentType = "text/csv";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=GSDActivityLog.csv");
                System.Web.HttpContext.Current.Response.Write(facsCsv);
                System.Web.HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException("-NA-", ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
        }
        private string GenerateCsvStringForGSDActivity(DataSet ds)
        {
            StringBuilder csv = new StringBuilder();

            csv.AppendLine("User Id,DateTime,Activity,GSD User Id");

            foreach (DataRow item in ds.Tables[0].Rows)
            {
                csv.Append(item["userid"] + ",");
                csv.Append(item["logdatetime"] + ",");
                csv.Append(item["activity"] + ",");
                csv.Append(item["gsduserid"] + ",");
                csv.AppendLine();
            }

            return csv.ToString();
        }
        /// <summary>
        /// Get the User incomplete activity deatils
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public string GetUserIncompleteActivity(string userId,string mode,string startDate, string endDate)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetUserIncompleteActivity(mode, startDate, endDate);
                return JsonConvert.SerializeObject(ds);//dataset value converted into JSON
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }


        public string GetDeviceLock(string userId, string mode, string startDate, string endDate)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                List<DeviceLockout> lstDevice = objDALUser.GetDeviceLock(mode, startDate, endDate);
                return JsonConvert.SerializeObject(lstDevice);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        /// <summary>
        /// To Get the account lock details month over month - GSD View Dashboard
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        #region GSD Dashboard
        public string GetAccountlockByMonth(string userId)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetAccountlockByMonth();
                return JsonConvert.SerializeObject(ds);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        public string AccountlockByMonthCsvData(string userId)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetAccountlockByMonth();

                return GetAccountLockByMonthCsvString(ds);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null; 
            }
        }

        private string GetAccountLockByMonthCsvString(DataSet ds)
        {
            StringBuilder csv = new StringBuilder();

            csv.AppendLine("Month,Repeat,Unique");

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                csv.Append(dr["MonthName"] + ",");
                csv.Append(dr["Count"] + ",");
                csv.Append(dr["DistinctCount"] + ",");
                csv.AppendLine();
            }

            return csv.ToString();
        }
        /// <summary>
        /// To Get the account lock details day wise - GSD View Dashboard
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetaccountlockbyDay(string userId)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetAccountlockByDay();
                return JsonConvert.SerializeObject(ds);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        /// <summary>
        /// Get the Service activity information
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetGsdActivityByDay(string userId)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetGsdActivityByDay();
                return JsonConvert.SerializeObject(ds);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        public string GetFrequentCallers(string userId)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                DataSet ds = objDALUser.GetFrequentCallers();
                return JsonConvert.SerializeObject(ds);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        /// <summary>
        /// To get user privilege access
        /// </summary>
        /// <param name="userId"></param>     
        /// <returns></returns>
        public List<UserAccessPrivilege> GetUserPrivilege(string userId)
        {
            try
            {
                DAL_Reports objDALUser = new DAL_Reports();
                return  objDALUser.GetUserPrivilege(userId); //post the parameter to data access layer 
               
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        #endregion
    }
}
