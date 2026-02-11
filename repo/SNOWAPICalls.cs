using ArtHandler.DAL;
using ArtHandler.Interface;
using ArtHandler.Model;
using ArtHandler.Snow;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace ArtHandler.Repository
{
    public class SNOWAPICalls : Iitsmtool
    {
        private string APIResult = string.Empty;
        private string URL = string.Empty;
        private DataTable dt = null;
        private string UserName = string.Empty;
        private string Password = string.Empty;
        private string WorkGroup = string.Empty;
        private string Description = string.Empty;
        private string Category = string.Empty;
        private string business_service = string.Empty;
        private string CallerEmailID = string.Empty;
        private string GSDUserName = string.Empty; 
        private string GSDEmailID = string.Empty;
        private string WebMethod = string.Empty;
        private string ArtCategory = string.Empty;
        private string Urgency = string.Empty;
        private string Priority = string.Empty;
        private string Impact = string.Empty; 
        private string UserLocation = string.Empty;

        private string Configuration_Item = string.Empty; 
        private string Service_Desk = string.Empty;
        private string Description_Unlock = string.Empty;
        private string Description_Change = string.Empty;
        private string Description_Forgot = string.Empty;
        private string Assigned_To = string.Empty;
        //int TicketNo = 0;
        private string UserEmailID = string.Empty;

        //string Category = Constants.PasswordResetCategory;
        private string Authorization = string.Empty;

        private string GSDWorkGroupName = string.Empty;

        public void GetSNOWInstanceDetails(string callType)
        {

            dt = new DAL_Settings().GetITSMToolInfo(Constants.Snow, callType);
            if (dt.Rows.Count > 0)
            {
                URL = Convert.ToString(dt.Rows[0]["URL"]);

                //if(callType == Constants.ITSMCreate)
                //    URL = "https://hexawareindev.service-now.com/api/now/v1/table/incident";
                //if (callType == Constants.ITSMUpdate)
                //    URL = "https://hexawareindev.service-now.com/api/now/v1/table/incident/{sysid}";


                UserName = Convert.ToString(dt.Rows[0]["Username"]);
                Password = Utility.Encryptor.Decrypt(Convert.ToString(dt.Rows[0]["Pwd"]), Constants.PASSPHARSE);
                WorkGroup = Convert.ToString(dt.Rows[0]["Workgroup"]);
                Description = Convert.ToString(dt.Rows[0]["Description"]);
                Category = Convert.ToString(dt.Rows[0]["InstanceCode"]);
                CallerEmailID = Convert.ToString(dt.Rows[0]["CallerEmailID"]);
                WebMethod = Convert.ToString(dt.Rows[0]["HTTPMethod"]);
                GSDWorkGroupName = Convert.ToString(dt.Rows[0]["GSDWorkGroupName"]);
                ArtCategory = Convert.ToString(dt.Rows[0]["Category"]);
                business_service = Convert.ToString(dt.Rows[0]["Service"]);

                Urgency = Convert.ToString(dt.Rows[0]["Urgency"]);
                Priority = Convert.ToString(dt.Rows[0]["Priority"]);
                Impact = Convert.ToString(dt.Rows[0]["Impact"]);

                Configuration_Item = Convert.ToString(dt.Rows[0]["Configuration_Item"]);
                Description_Unlock = Convert.ToString(dt.Rows[0]["Description_Unlock"]);
                Description_Change = Convert.ToString(dt.Rows[0]["Description_Change"]);
                Description_Forgot = Convert.ToString(dt.Rows[0]["Description_Forgot"]);
                Assigned_To = Convert.ToString(dt.Rows[0]["Assigned_to"]);
                Service_Desk = Convert.ToString(dt.Rows[0]["Support_Level"]);
            }
        }

        public string CreateIncident(string userID, string userEmail, int status, string UserMessage, string GSDMessage, string category, string userActivity, string adPhysicalOfficeDelivery, ref string sys_id, string gsdUserId)
        {
            try
            {
                GetSNOWInstanceDetails(Constants.ITSMCreate);
                string result = string.Empty;
                JObject res = new JObject();
                SnowCreateTicket sct = new SnowCreateTicket();

                //sct.EventType = Constants.Incident;
                //sct.TicketID = string.Empty;
                sct.short_description = GSDMessage;

                if (!string.IsNullOrEmpty(userID))
                {
                    sct.caller_id = userID;
                }
                else
                {
                    sct.caller_id = string.Empty;
                }

                if (userActivity == Constants.CHANGE_PASSWORD)
                {
                    sct.description = Description_Change;
                }
                else if (userActivity == Constants.RESET_PASSWORD)
                {
                    sct.description = Description_Forgot;
                }
                else if (userActivity == Constants.UNLOCK_ACCOUNT)
                {
                    sct.description = Description_Unlock;
                }
                sct.urgency = SnowUrgency.Medium;
                sct.impact = SnowImpact.Low;
                //sct.cmdb_ci = Configuration_Item;
                sct.business_service = business_service;
                sct.u_support_level = Service_Desk;

                sct.location = GetUserDetails(userEmail, Constants.location, Constants.display_value);

                sct.u_building_name = GetUserDetails(userEmail, Constants.u_building_name);
                sct.u_project = GetUserDetails(userEmail, Constants.u_project);
                sct.u_contact_number = GetUserDetails(userEmail, Constants.mobile_phone);

                sct.u_service_category = Constants.ARTCATEGORY;
                sct.u_reference_2 = category;
                sct.state = SnowState.New;
                sct.contact_type = Constants.SNOWContact_type;
                //sct.assigned_to = Assigned_To;
                sct.assignment_group = GSDWorkGroupName;
                //sct.u_support_level = "Level 2 Support";

                result = CallAPI(URL, JsonSerializer(sct), RequestMethod.POST);
                //SnowCreateResponse res = JsonDeSerializer<SnowCreateResponse>(result);

                if(!String.IsNullOrEmpty(result))
                {
                    res = JObject.Parse(result);
                }
                string ticketNum = res["result"]["number"].ToString();
                sys_id = res["result"]["sys_id"].ToString();

                LoggingRepository.ITSMTraceLog(Constants.ITSMSNOW, JsonSerializer(sct), result, ticketNum, "Snow CreateIncident");

                return ticketNum;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        /// <summary>
        /// call this method for incident creation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        /// <param name="ticketno"></param>
        public string CreateIncident(string userID, string userEmailID, string UserMessage, string GSDMessage, string category, string userActivity, string adPhysicalOfficeDelivery, ref string sys_id, string gsdUserId)
        {
            try
            {
                //UserEmailID = userEmailID;
                //Category = category;
                //return CreateIncident(userID, Constants.SummitNewStatus, 0, userID+" " +Description );
                return CreateIncident(userID, userEmailID, SnowState.New, UserMessage, GSDMessage, category, userActivity, adPhysicalOfficeDelivery, ref sys_id, gsdUserId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        /// <summary>
        /// post data to FS API
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public string CallAPI(string URL, string json, string method)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(URL);
                request.ContentType = RequestMethod.ApplicationJson;
                request.Method = method;
                //request.Headers["Authorization"] = Authorization;
                //UserName = method == RequestMethod.GET ? "Hexateam" : UserName;
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(UserName + ":" + Password));

                if (method != RequestMethod.GET)
                {
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    APIResult = streamReader.ReadToEnd();
                }
            }
            catch (Exception exp)
            {
                Log.LogException(new CustomException(System.Reflection.MethodBase.GetCurrentMethod().Name, exp.Message.ToString(), exp.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "caller : " + CallerEmailID + "url : " + URL + " -- method : " + method + " -- json :" + json));
            }
            return APIResult;
        }

        public string JsonSerializer(dynamic obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public T JsonDeSerializer<T>(string json) where T : class, new()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }

        
        public string UpdateIncidentState(string status, string ticketno, string category, string sys_id)
        {
            GetSNOWInstanceDetails(Constants.ITSMUpdate);

            string result = string.Empty;
            SnowUpdateTicket sut = new SnowUpdateTicket();

            sut.state = SnowState.InProgress;

            sut.assignment_group = (category == Constants.AccounUnlockCategory) ? GSDWorkGroupName : null;
          
            URL = URL.Replace("{sysid}", sys_id);
            result = CallAPI(URL, JsonSerializer(sut), RequestMethod.PATCH);

            JObject res = JObject.Parse(result);
            LoggingRepository.ITSMTraceLog(Constants.ITSMSNOW, JsonSerializer(sut), result, res["result"]["number"].ToString(), "Snow UpdateIncident");

            return res["result"]["sys_id"].ToString();
        }

        /// <summary>
        /// to update
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ticketno"></param>
        /// <param name="Usrmessage"></param>
        /// <param name="GSDmessage"></param>
        /// <param name="reassign"></param>
        /// <param name="isfirstime"></param>
        /// <param name="sys_id"></param>
        /// <returns></returns>
        public string UpdateIncident(string status, string ticketno, string Usrmessage, string GSDmessage, bool reassign, bool isfirstime, string category, string sys_id)
        {
            GetSNOWInstanceDetails(Constants.ITSMUpdate);

            string result = string.Empty;
            SnowUpdateTicket sut = new SnowUpdateTicket();
            //sut.sys_id = sys_id;
            sut.state = Constants.SNOWInProgressStatus;
            //if (!string.IsNullOrEmpty(Usrmessage))
            //{
            //    sut.comments = Usrmessage;
            //}
            //else
            //{
            //    sut.comments = string.Empty;
            //}
            //if (!string.IsNullOrEmpty(GSDmessage))
            //{
            //    sut.work_notes = GSDmessage;
            //}
            //else
            //{
            //    sut.work_notes = string.Empty;
            //}

            URL = URL.Replace("{sysid}", sys_id);
            result = CallAPI(URL, JsonSerializer(sut), RequestMethod.PATCH);

            //SnowUpdateResponse res = JsonDeSerializer<SnowUpdateResponse>(result);

            JObject res = JObject.Parse(result);
            LoggingRepository.ITSMTraceLog(Constants.ITSMSNOW, JsonSerializer(sut), result, res["result"]["number"].ToString(), "Snow UpdateIncident");

            return res["result"]["sys_id"].ToString();
        }

        /// <summary>
        /// to update
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ticketno"></param>
        /// <param name="Usrmessage"></param>
        /// <param name="GSDmessage"></param>
        /// <param name="reassign"></param>
        /// <param name="isfirstime"></param>
        /// <param name="sys_id"></param>
        /// <returns></returns>
        public string ResolveIncident(string status, string ticketno, string Usrmessage, string GSDmessage, bool reassign, bool isfirstime, string sys_id, bool iscreateResolve)
        {
            string result = string.Empty;
            string ticketNum = string.Empty;
            GetSNOWInstanceDetails(Constants.ITSMUpdate);
            SnowResolveTicket sut = new SnowResolveTicket();
            sut.close_notes = Usrmessage + GSDUserName;
            sut.state = SnowState.Resolved;
            sut.assigned_to = GSDEmailID;
            sut.close_code = SnowCloseCode.SolvedWorkAround;
            sut.work_notes = Usrmessage + GSDUserName;

            result = CallAPI(URL.Replace("{sysid}", sys_id), JsonSerializer(sut), RequestMethod.PATCH);

            JObject res = JObject.Parse(result);
            ticketNum = res["result"]["number"].ToString();

            LoggingRepository.ITSMTraceLog(Constants.ITSMSNOW, JsonSerializer(sut), result, ticketNum, "Snow ResolveIncident");

            return ticketNum;
        }

        public string ReassignIncident(string ticketno, string Usrmessage, string GSDmessage, bool reassign, bool isfirstime, string category, string sys_id)
        {
            string result = string.Empty;
            SnowReassignTicket sut = new SnowReassignTicket();
            //sut.sys_id = sys_id;
            //sut.state = Constants.SNOWWORKINPROGRESS;

            //if (!string.IsNullOrEmpty(Usrmessage))
            //{
            //    sut.comments = Usrmessage;
            //}
            //else
            //{
            //    sut.comments = string.Empty;
            //}
            //if (!string.IsNullOrEmpty(GSDmessage))
            //{
            //    sut.work_notes = GSDmessage;
            //}
            //else
            //{
            //    sut.work_notes = string.Empty;
            //}
            sut.assignment_group = GSDWorkGroupName;

            GetSNOWInstanceDetails(Constants.ITSMUpdate);
            URL = URL.Replace("{sys_id}", sys_id);
            result = CallAPI(URL, JsonSerializer(sut), RequestMethod.PATCH);
            SnowUpdateResponse res = JsonDeSerializer<SnowUpdateResponse>(result);
            return res.result.sys_id;
        }

        /// <summary>
        /// call this method for incident update
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        /// <param name="ticketno"></param>
        public string UpdateIncident(string ticketno, string usrMessage, string gsdMessage, string userEmailID, string category, bool isfirstime, string sys_id)
        {
            UserEmailID = userEmailID;
            Category = category;
            return UpdateIncident(Constants.SummitInProgressStatus, ticketno, usrMessage, gsdMessage, false, isfirstime, category, sys_id);
        }

        /// <summary>
        /// to update the ticket as Resolved
        /// </summary>
        /// <param name="ticketno"></param>
        /// <param name="usrMessage"></param>
        /// <param name="gsdMessage"></param>
        /// <param name="userEmailID"></param>
        /// <param name="sys_id"></param>
        /// <returns></returns>
        public string ResolveIncident(string ticketno, string usrMessage, string gsdMessage, string userEmailID, string category, string GSDuserName, string GSDEmail, string sys_id, bool iscreateResolve)
        {
            UserEmailID = userEmailID;
            GSDUserName = GetUserDetails(GSDEmail, Constants.first_name) + " " + GetUserDetails(GSDEmail, Constants.last_name) ;
            GSDEmailID = GSDEmail;
            if (!string.IsNullOrEmpty(ticketno))
            {
                return ResolveIncident(Constants.SummitResolvedStatus, ticketno, usrMessage, gsdMessage, false, false, sys_id, iscreateResolve);
            }
            return ticketno;
        }

        /// <summary>
        /// to update the ticket as Reassigned
        /// </summary>
        /// <param name="ticketno"></param>
        /// <param name="usrMessage"></param>
        /// <param name="gsdMessage"></param>
        /// <param name="userEmailID"></param>
        /// <param name="category"></param>
        /// <param name="sys_id"></param>
        /// <returns></returns>
        public string ReassignIncident(string ticketno, string usrMessage, string gsdMessage, string userEmailID, string category, string sys_id)
        {
            Category = category;
            if (!string.IsNullOrEmpty(ticketno))
            {
                UserEmailID = userEmailID;
                return ReassignIncident(ticketno, usrMessage, gsdMessage, true, false, category, sys_id);
            }
            return ticketno;
        }

        public string UpdateIncidentDetails(string status, bool reassign, string ticketno, string message, string userEmailID, string category, string sys_id)
        {
            UpdateIncident(ticketno, message, message, userEmailID, category, true, sys_id);

            return ticketno;
            //throw new NotImplementedException();
        }

        public string GetUserDetails(string emailId, string key, string subkey = "")
        {
            string SnowGetUserApi = ConfigurationManager.AppSettings["snowgetuserapi"].ToString();
            string result = string.Empty;
            string json = string.Empty;
            GetSNOWInstanceDetails(Constants.ITSMCreate);
            try
            {
                string SnowUrl = URL.Replace("incident", string.Format(SnowGetUserApi, "&", "&", emailId));
                result = CallAPI(SnowUrl, json, RequestMethod.GET);

                LoggingRepository.ITSMTraceLog(Constants.ITSMSNOW, json, result, "Ticket Creation", "Snow user Information");

                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        JObject jsonObject = JObject.Parse(result);
                        if (!String.IsNullOrEmpty(subkey))
                            return jsonObject["result"][0][key][subkey].ToString();
                        else
                            return jsonObject["result"][0][key].ToString();
                    }
                    catch(Exception e)
                    {
                        Log.LogException(new CustomException(System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message.ToString(), e.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "caller : " + CallerEmailID + "result :" + result + " -- key : " + key + " -- subkey : " + subkey));
                    }
                    
                }
            }
            catch (Exception exp)
            {
                
            }

            return string.Empty;
            
        }
    }
}