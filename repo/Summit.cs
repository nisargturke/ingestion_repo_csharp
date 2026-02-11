
using ArtHandler.DAL;
using ArtHandler.Interface;
using ArtHandler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ArtHandler.Repository
{
    public class Summit : Iitsmtool
    {
        string APIResult = string.Empty;
        string URL = string.Empty;
        string UserName = string.Empty;
        string Password = string.Empty;
        string WorkGroup = string.Empty;
        string Description = string.Empty;
        string InstanceCode = string.Empty;
        string CallerEmailID = string.Empty;
        string WebMethod = string.Empty;
        int TicketNo = 0;
        string UserEmailID = string.Empty;
        string Category = string.Empty;
        string userId = string.Empty;
        string UserEmailId = string.Empty;
        private DataTable dt = null;
        string gsdWorkGroupName = string.Empty;
        string Type = string.Empty;

        public Summit()
        {
            try
            {

                dt = new DAL_Settings().GetITSMToolInfo(Constants.Summit, Constants.ITSMCreate);
                if (dt.Rows.Count > 0)
                {
                    URL = Convert.ToString(dt.Rows[0]["URL"]);
                    UserName = Convert.ToString(dt.Rows[0]["Username"]);
                    Password = Utility.Encryptor.Decrypt(Convert.ToString(dt.Rows[0]["Pwd"]), Constants.PASSPHARSE);
                    WorkGroup = Convert.ToString(dt.Rows[0]["Workgroup"]);
                    Description = Convert.ToString(dt.Rows[0]["Description"]);
                    InstanceCode = Convert.ToString(dt.Rows[0]["InstanceCode"]);
                    CallerEmailID = Convert.ToString(dt.Rows[0]["CallerEmailID"]);
                    WebMethod = Convert.ToString(dt.Rows[0]["HTTPMethod"]);
                    Type = Convert.ToString(dt.Rows[0]["Type"]);
                    Category = Convert.ToString(dt.Rows[0]["Category"]);
                    gsdWorkGroupName = Convert.ToString(dt.Rows[0]["GsDWorkGroupName"]);
                }
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
        }
        /// <summary>
        /// post data to SUMMIT API
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        private string PostAPI(string URL, string json)
        {
            // try
            // {
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(URL);
            request.ContentType = "text/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                APIResult = streamReader.ReadToEnd();
            }
            // }
            // catch (Exception exp)
            // {
            //    LogWriter.WriteException(MethodBase.GetCurrentMethod().Name, exp);
            //  }
            return APIResult;
        }

        private int CreateIncident(string userID, string status, int ticketno, string message)
        {
            SummitIncident inc = new SummitIncident();

            // try
            //{
            inc._ProxyDetails = new _Proxydetails();
            inc._ProxyDetails.Password = Password;
            //if (!string.IsNullOrEmpty(UserEmailID))
            //{
            //    inc._ProxyDetails.UserName = UserEmailID;
            //}
            //else
            //{
            //    inc._ProxyDetails.UserName = UserName;
            //}
            inc._ProxyDetails.UserName = UserName;
            inc._ProxyDetails.ProxyID = 0;
            inc._ProxyDetails.ReturnType = Constants.ResponseTypeJson;
            inc.oTicket = new Oticket();
            inc.oTicket.InstanceCode = InstanceCode;
            inc.oTicket.Workgroup = WorkGroup;
            inc.oTicket.Symptom = Constants.SummitSymptom;
            inc.oTicket.Description = message;
            inc.oTicket.InternalLog = message;
            inc.oTicket.Solution = message;
            inc.oTicket.Status = status;
            inc.oTicket.Source = Constants.SummitSource;
            if (!string.IsNullOrEmpty(UserEmailID))
            {
                if (userID.Length >= 4)
                {
                    string usrID = userID.Substring(userID.Length - 4, 4);
                    if (!Utility.IsNumberonly(usrID))
                    {
                        inc.oTicket.CallerEmailID = CallerEmailID;
                    }
                    inc.oTicket.CallerEmailID = UserEmailID;
                }
            }
            else
            {
                inc.oTicket.CallerEmailID = CallerEmailID;
            }

            inc.oTicket.Priority = Constants.SummitPriorityName;
            inc.oTicket.SLA = Constants.SLA;
            inc.oTicket.OpenCategory = Category;
            //inc.oTicket.OpenCategory = Constants.OpenCategory;

            inc.oTicket.Impact = Constants.SummitImpact;
            inc.oTicket.Urgency = Constants.SummitUrgency;
            inc.oTicket.Solution = Constants.SummitSource;
            inc.oTicket.Classification = Constants.SummitClassification;
            //not required if it new
            //inc.oTicket.AssignedExecEmailID = CallerEmailID;

            string json = JsonSerializer(inc);
            SummitAPIResponse sar = new SummitAPIResponse();
            string res = PostAPI(URL + "/" + Type, json);

            //Utility.WriteLog("created ticket result:" + res);
            if (!string.IsNullOrEmpty(res))
            {
                res = res.Replace("\\", "");
                res = res.Replace("\"", "'");
                res = res.Substring(1, res.Length - 2);
                res.Replace("\"", "'");
                sar = JsonDeSerializer<SummitAPIResponse>(res); //JsonConvert.DeserializeObject<SummitAPIResponse>(res);
                if (sar != null)
                {
                    TicketNo = Convert.ToInt32(sar.TicketNo);
                }
            }

            //}
            // catch (Exception exp)
            // {
            //  LogWriter.WriteException(MethodBase.GetCurrentMethod().Name, exp);
            // TicketNo = 0;
            //}
            return TicketNo;
        }
        /// <summary>
        /// Update Incident
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ticketno"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// 
        public string UpdateIncidentDetails(string status, bool reassign, string ticketno, string message, string userEmailID, string category, string sys_id)
        {
            UpdateIncident(ticketno, message, message, userEmailID, category, true, sys_id);

            return ticketno;
            //throw new NotImplementedException();
        }

        public string UpdateIncident(string ticketno, string Usrmessage, string GSDmessage, string emailID, string category, bool isfirstime, string sys_id)
        {
            bool reassign = true;
            string status = Constants.SummitAssignedStatus;

            SummitUpdateIncident objCreate = new SummitUpdateIncident();
            objUpdateIncidentCommonParameters objCommon = new objUpdateIncidentCommonParameters();
            _Proxydetails objPro = new _Proxydetails();
            objPro.Password = Password;
            objPro.UserName = UserName;
            objPro.ProxyID = 0;
            objPro.ReturnType = Constants.ResponseTypeJson;
            objPro.OrgID = 1;
            objCommon._ProxyDetails = objPro;
            //  objCreate.objCommonParameters = objCommon;
            UpdateIncidentTicket objTicket = new UpdateIncidentTicket();
            objTicket.IsFromWebService = true;
            objTicket.Urgency_Name = Constants.SummitUrgency;
            objTicket.Classification_Name = Constants.SummitClassification;
            objTicket.Desc = Description;
            objTicket.Sup_Function = InstanceCode;
            objTicket.Caller_EmailID = emailID;
            //objTicket.ResolutionCodeName = "Resolved Permanently";
            objTicket.Status = status;
            objTicket.Priority_Name = Constants.SummitLowPriority;
            objTicket.SLA_Name = Constants.SummitSLAName; //Convert.ToString(SLA);
            objTicket.Assigned_WorkGroup_Name = WorkGroup;
            objTicket.Medium = Constants.SummitMedium;
            objTicket.Impact_Name = Constants.SummitImpact;
            objTicket.Category_Name = category;
            objTicket.OpenCategory_Name = category;
            //objTicket.Assigned_Engineer = Workgroup;
            objTicket.Description = Description;
            objTicket.Ticket_No = Convert.ToString(ticketno);
            objTicket.Description = Usrmessage;
            objTicket.Resolution_Time = Constants.SummitDefaultDateTime;// GetUTCNow();
            objTicket.Updated_Time = GetUTCNow();
            objTicket.PageName = Constants.SummitRestApiUpdateStatusPageName;
            objTicket.Solution = Usrmessage;
            // objTicket.Source = Constants.SummitSource;
            //  objTicket.InternalLog = message;

            IConfigProvider objConfig = new ConfigProvider();
            objTicket.Resolution_SLA_Reason = Constants.SummitResponse_SLA_Reason;
            objTicket.Response_SLA_Reason = Constants.SummitResponse_SLA_Reason;

            if (status == Constants.SummitResolvedStatus)
            {
                objTicket.Closure_Code_Name = Constants.SummitClosureCodeName;
                objTicket.Resolution_Time = GetUTCNow();
                objTicket.Assigned_Engineer_Email = UserName;
                objTicket.Resolution_SLA_Reason = Constants.SummitResponse_SLA_Reason;
                objTicket.Resolution_SLA_Met = true;
                objTicket.Response_SLA_Met = true;
            }
            if (reassign)
            {
                objTicket.Assigned_WorkGroup_Name = WorkGroup;
                objTicket.Status = Constants.SummitAssignedStatus;
                objTicket.OpenCategory_Name = category;
                objTicket.Category_Name = category;
            }

            TicketInformation objTicketInfo = new TicketInformation();
            objTicketInfo.Information = category;
            objTicketInfo.InternalLog = Usrmessage;
            objTicketInfo.Solution = Usrmessage;
            UpdateIncidentContainerJson objCont = new UpdateIncidentContainerJson();
            objCont.Ticket = objTicket;
            objCont.TicketInformation = objTicketInfo;
            objCont.Updater = Constants.SummitRestApiUpdaterExecutive;
            UpdateIncidentParamsJSON objParam = new UpdateIncidentParamsJSON();
            objParam.IncidentContainerJson = JsonConvert.SerializeObject(objCont);
            objCommon.incidentParamsJSON = objParam;
            objCommon.RequestType = Constants.SummitRestApiRequestTypeRemoteCall;
            objCreate.objCommonParameters = objCommon;
            objCreate.ServiceName = Type;
            string json = JsonConvert.SerializeObject(objCreate);
            SummitAPIResponse sar = new SummitAPIResponse();
            string res = PostAPI(URL, json);


            if (!string.IsNullOrEmpty(res))
            {
                sar = JsonConvert.DeserializeObject<SummitAPIResponse>(res);
                if (sar != null)
                {
                    TicketNo = Convert.ToInt32(sar.TicketNo);
                }
            }

            sys_id = Convert.ToString(TicketNo);

            LoggingRepository.ITSMTraceLog(Constants.Summit.ToUpper(), json, res, sys_id, "GSD Summit UpdateIncident");

            return Convert.ToString(TicketNo);
        }

        public string UpdateIncidentState(string status, string ticketno, string category, string sys_id)
        {
            return string.Empty;
        }

            //public string CreateIncident(string userID, string emailID, string Usrmessage, string GSDmessage, string category, ref string sys_id)
            //{
            //    throw new NotImplementedException();
            //}
            /// <summary>
            /// call this method for incident creation
            /// </summary>
            /// <param name="userID"></param>
            /// <param name="status"></param>
            /// <param name="ticketno"></param>
            public string CreateIncident(string userID, string userEmailID, string message, string GSDmessage, string category, string description, string adPhysicalOfficeDelivery, ref string sys_id, string gsdUserId)
        {
            try
            {
                int incidentId = LogIncident(userID, userEmailID, message, GSDmessage, category, description, ref sys_id);

                return Convert.ToString(incidentId);

                //userId = userID;
                //UserEmailID = userEmailID;

                //Category = category;

                //SummitIncident inc = new SummitIncident();
                //inc._ProxyDetails = new _Proxydetails();
                //inc._ProxyDetails.Password = Password;
                //inc._ProxyDetails.UserName = UserName;
                //inc._ProxyDetails.ProxyID = 0;
                //inc._ProxyDetails.ReturnType = Constants.ResponseTypeJson;
                //inc.oTicket = new Oticket();
                //inc.oTicket.InstanceCode = InstanceCode;
                //inc.oTicket.Workgroup = WorkGroup;
                //inc.oTicket.Symptom = category;//Constants.SummitSymptom;
                //inc.oTicket.Description = description;//description;
                //inc.oTicket.InternalLog = message;
                //inc.oTicket.Solution = description;
                //inc.oTicket.Status = Constants.SummitResolvedStatus;
                //inc.oTicket.Source = Constants.SummitSource;

                //inc.oTicket.CallerEmailID = UserEmailID;

                ////if (!string.IsNullOrEmpty(UserEmailID))
                ////{
                ////    if (userID.Length >= 4)
                ////    {
                ////        string usrID = userID.Substring(userID.Length - 4, 4);
                ////        if (!Utility.IsNumberonly(usrID))
                ////        {
                ////            inc.oTicket.CallerEmailID = CallerEmailID;
                ////        }
                ////        inc.oTicket.CallerEmailID = UserEmailID;
                ////    }
                ////}
                ////else
                ////{
                ////    inc.oTicket.CallerEmailID = CallerEmailID;
                ////}

                //inc.oTicket.Priority = Constants.SummitPriorityName;
                //inc.oTicket.SLA = Constants.SLA;
                //inc.oTicket.OpenCategory = Category;
                ////inc.oTicket.OpenCategory = Constants.OpenCategory;

                //inc.oTicket.Impact = Constants.SummitImpact;
                //inc.oTicket.Urgency = Constants.SummitUrgency;
                ////inc.oTicket.Solution = Constants.SummitSource;
                //inc.oTicket.Classification = Constants.SummitClassification;
                //inc.oTicket.Resolution_Time = GetUTCNow();

                ////not required if it new
                ////inc.oTicket.AssignedExecEmailID = CallerEmailID;

                //string json = JsonSerializer(inc);
                //SummitAPIResponse sar = new SummitAPIResponse();
                ////string res = PostAPI(URL + "/" + Type, json);
                //string res = PostAPI(URL, json);

                ////Utility.WriteLog("created ticket result:" + res);
                //if (!string.IsNullOrEmpty(res))
                //{
                //    res = res.Replace("\\", "");
                //    res = res.Replace("\"", "'");
                //    res = res.Substring(1, res.Length - 2);
                //    res.Replace("\"", "'");
                //    sar = JsonDeSerializer<SummitAPIResponse>(res); //JsonConvert.DeserializeObject<SummitAPIResponse>(res);
                //    if (sar != null)
                //    {
                //        TicketNo = Convert.ToInt32(sar.TicketNo);
                //    }
                //}

                //sys_id = Convert.ToString(TicketNo);
                //return sys_id;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }


        /// <summary>
        /// call this method for STG service group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="wrkgrp"></param>
        /// <returns></returns>
        public int AssignIncident(string userID, string userEmailID)
        {
            WorkGroup = Constants.SummitSrvGrp;
            UserEmailID = userEmailID;
            return CreateIncident(userID, Constants.SummitNewStatus, 0, userID + " " + Description);
        }


        /// <summary>
        /// call this method for incident update
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        /// <param name="ticketno"></param>
        //public int UpdateIncident(int ticketno, string message, string userEmailID, string category, bool isfirstime)
        //{
        //    UserEmailID = UserEmailId;

        //    //UserEmailID = userEmailID;
        //    Category = category;
        //    return UpdateIncident(Constants.SummitInProgressStatus, ticketno, message, false, isfirstime);
        //}


        /// <summary>
        /// call this method to set the incident status as resolved
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        /// <param name="ticketno"></param>
        //public int ResolveIncident(int ticketno, string message, string userEmailID)
        //{
        //    if (ticketno > 0)
        //    {
        //        UserEmailID = userEmailID;
        //        return UpdateIncident(Constants.SummitResolvedStatus, ticketno, message, false, false);
        //    }
        //    return ticketno;
        //}
        public string ResolveIncident(string ticketno, string Usrmessage, string GSDmessage, string emailID, string sys_id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// reassign a ticket to other group
        /// </summary>
        /// <param name="ticketno"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        //public int ReassignIncident(int ticketno, string message, string userEmailID, string category)
        //{
        //    Category = category;
        //    if (ticketno > 0)
        //    {
        //        UserEmailID = userEmailID;
        //        return UpdateIncident(Constants.SummitInProgressStatus, ticketno, message, true, false);
        //    }
        //    return ticketno;
        //}

        private string GetUTCNow()
        {
            //DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //long mss = (long)(DateTime.Now - epoch).TotalMilliseconds;
            //return "/Date(" + mss.ToString() + ")/";
            return DateTime.UtcNow.ToString();
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

        public int LogIncident(string userID, string userEmailID, string message, string GSDmessage,
            string category, string description, ref string sys_id)
        {
            try
            {
                SummitCreateIncident objCreate = new SummitCreateIncident();
                objIncidentCommonParameters objCommon = new objIncidentCommonParameters();
                _Proxydetails objPro = new _Proxydetails();
                objPro.Password = Password;
                objPro.UserName = UserName;
                objPro.ProxyID = 0;
                objPro.ReturnType = Constants.ResponseTypeJson;
                objPro.OrgID = 1;
                objCommon._ProxyDetails = objPro;
                //  objCreate.objCommonParameters = objCommon;

                Ticket objTicket = new Ticket();
                //objTicket.Assigned_Engineer_Email = UserName;
                objTicket.IsFromWebService = true;
                objTicket.Urgency_Name = Constants.SummitUrgency;
                objTicket.Classification_Name = Constants.SummitClassification;
                objTicket.Desc = message;
                objTicket.SLA_Name = Constants.SummitSLAName; //Convert.ToString(SLA);
                objTicket.Sup_Function = InstanceCode;
                //  objTicket.Caller = userEmailId;
                objTicket.Caller_EmailID = userEmailID;
                //objTicket.Closure_Code = Constants.SummitClosureCode;
                //objTicket.Closure_Code_Name = Constants.SummitClosureCodeName; 
                objTicket.ResolutionCodeName = Constants.SummitClosureCodeName; //"Resolved Permanently";
                objTicket.Status = Constants.SummitInProgressStatus;
                objTicket.Priority_Name = Constants.SummitLowPriority;
                objTicket.Assigned_WorkGroup_Name = gsdWorkGroupName;
                objTicket.Medium = Constants.SummitMedium; //"Web";
                objTicket.Impact_Name = Constants.SummitImpact;
                objTicket.Category_Name = category;
                objTicket.OpenCategory_Name = category;
                //objTicket.Assigned_Engineer = Workgroup;
                objTicket.Description = message;
                objTicket.Resolution_Time = GetUTCNow();
                objTicket.PageName = Constants.SummitRestApiNewStatusPageName; //"LogTicket";
                objTicket.Source = Constants.SummitSymptom;

                //objTicket.Resolution_SLA_Reason = Constants.SummitResponseSLAReason;
                //objTicket.Resolution_SLA_Met = true; 

                TicketInformation objTicketInfo = new TicketInformation();
                objTicketInfo.Information = category;
                //objTicketInfo.Solution = message;
                //objTicketInfo.InternalLog = message;
                //objTicket.Resolution_Time = GetUTCNow();
                objTicket.Response_Time = GetUTCNow();
                objTicket.Updated_Time = GetUTCNow();
                //objTicketInfo.UserLog = description;


                IncidentContainerJson objCont = new IncidentContainerJson();
                objCont.Ticket = objTicket;
                objCont.TicketInformation = objTicketInfo;
                objCont.Updater = Constants.SummitRestApiUpdaterExecutive; //"Executive";
                IncidentParamsJSON objParam = new IncidentParamsJSON();
                objParam.IncidentContainerJson = JsonConvert.SerializeObject(objCont);
                objCommon.incidentParamsJSON = objParam;
                objCommon.RequestType = Constants.SummitRestApiRequestTypeRemoteCall; //"RemoteCall";
                objCreate.objCommonParameters = objCommon;
                objCreate.ServiceName = Type;

                string json = JsonConvert.SerializeObject(objCreate);
                SummitAPIResponse sar = new SummitAPIResponse();
                string res = PostAPI(URL, json);


                if (!string.IsNullOrEmpty(res))
                {
                    sar = JsonConvert.DeserializeObject<SummitAPIResponse>(res);
                    if (sar != null)
                    {
                        TicketNo = Convert.ToInt32(sar.TicketNo);
                    }
                }

                sys_id = Convert.ToString(TicketNo);

                LoggingRepository.ITSMTraceLog(Constants.Summit.ToUpper(), json, res, sys_id, "GSD Summit CreateIncident");

                //LogWriter.WriteLog(MethodBase.GetCurrentMethod().Name, "Summit Ticket Id :" + TicketNo, Constants.Summit, URL, json, res, traceRefId, Convert.ToString(TicketNo));

                //LogWriter.WriteFile("Summit Ticket Id :" + TicketNo + ";" +
                //    Constants.Summit + ";" + URL + ";" + json + ";" + res + ";" + traceRefId + ";" +
                //    Convert.ToString(TicketNo), PriorityLevels.HighPriority);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return TicketNo;
        }


        public string ResolveIncident(string ticketno, string message, string GSDmessage, string userEmailID, string category, string gsduserName, string gsdemail,  string sys_id, bool iscreateResolve)
        {
            try
            {
                SummitUpdateIncident objCreate = new SummitUpdateIncident();
                objUpdateIncidentCommonParameters objCommon = new objUpdateIncidentCommonParameters();
                _Proxydetails objPro = new _Proxydetails();
                objPro.Password = Password;
                objPro.UserName = UserName;
                objPro.ProxyID = 0;
                objPro.ReturnType = Constants.ResponseTypeJson;
                objPro.OrgID = 1;
                objCommon._ProxyDetails = objPro;
                //  objCreate.objCommonParameters = objCommon;

                UpdateIncidentTicket objTicket = new UpdateIncidentTicket();
                objTicket.IsFromWebService = true;
                objTicket.Urgency_Name = Constants.SummitUrgency;
                objTicket.Classification_Name = Constants.SummitClassification;
                objTicket.Desc = message;
                objTicket.Sup_Function = InstanceCode;
                objTicket.Caller_EmailID = userEmailID;
                //objTicket.ResolutionCodeName = "Resolved Permanently";
                objTicket.Status = Constants.SummitResolvedStatus;
                objTicket.Priority_Name = Constants.SummitLowPriority;
                objTicket.Assigned_WorkGroup_Name = gsdWorkGroupName;
                objTicket.Medium = Constants.SummitMedium;
                objTicket.Impact_Name = Constants.SummitImpact;

                objTicket.Category_Name = category;
                objTicket.OpenCategory_Name = category;

                //objTicket.Assigned_Engineer = Workgroup;
                //objTicket.Description = message;
                objTicket.Ticket_No = Convert.ToString(ticketno);
                objTicket.Description = message;
                objTicket.Resolution_Time = Constants.SummitDefaultDateTime;// GetUTCNow();
                objTicket.Updated_Time = GetUTCNow();
                objTicket.PageName = Constants.SummitRestApiUpdateStatusPageName;
                objTicket.Solution = message;
                //objTicket.InternalLog = message;


                //if (status == Constants.SummitResolvedStatus)
                //{
                objTicket.Closure_Code_Name = Constants.SummitClosureCodeName;
                objTicket.Resolution_Time = GetUTCNow();
                //objTicket.Assigned_Engineer_Email = gsdUserEmailId;

                objTicket.Response_SLA_Met = true;
                objTicket.Resolution_SLA_Reason = Constants.SummitResponseSLAReason;
                objTicket.Resolution_SLA_Met = true;
                //}
                //if (reassign)
                //{
                //    objTicket.Assigned_WorkGroup_Name = Workgroup; //RequestExecutionValuesRepository.GetRequestExecInfo(eventName, RequestConstants.reassignworkgroup);
                //    objTicket.Status = Constants.SummitAssignedStatus;
                //    objTicket.OpenCategory_Name = OpenCategory; //RequestExecutionValuesRepository.GetRequestExecInfo(eventName, RequestConstants.Reassigncategory);
                //    objTicket.Category_Name = Category; //RequestExecutionValuesRepository.GetRequestExecInfo(eventName, RequestConstants.Reassigncategory);
                //}

                TicketInformation objTicketInfo = new TicketInformation();

                objTicketInfo.Information = category;

                objTicketInfo.InternalLog = message;
                objTicketInfo.Solution = message;
                UpdateIncidentContainerJson objCont = new UpdateIncidentContainerJson();
                objCont.Ticket = objTicket;
                objCont.TicketInformation = objTicketInfo;
                objCont.Updater = Constants.SummitRestApiUpdaterExecutive;
                UpdateIncidentParamsJSON objParam = new UpdateIncidentParamsJSON();
                objParam.IncidentContainerJson = JsonConvert.SerializeObject(objCont);
                objCommon.incidentParamsJSON = objParam;
                objCommon.RequestType = Constants.SummitRestApiRequestTypeRemoteCall;
                objCreate.objCommonParameters = objCommon;
                objCreate.ServiceName = Type;

                string json = JsonConvert.SerializeObject(objCreate);
                SummitAPIResponse sar = new SummitAPIResponse();
                string res = PostAPI(URL, json);


                if (!string.IsNullOrEmpty(res))
                {
                    sar = JsonConvert.DeserializeObject<SummitAPIResponse>(res);
                    if (sar != null)
                    {
                        TicketNo = Convert.ToInt32(sar.TicketNo);
                    }
                }

                sys_id = Convert.ToString(TicketNo);
                LoggingRepository.ITSMTraceLog(Constants.Summit.ToUpper(), json, res, sys_id, "GSD Summit ResolveIncident");

            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return Convert.ToString(TicketNo);
        }

    }
}
