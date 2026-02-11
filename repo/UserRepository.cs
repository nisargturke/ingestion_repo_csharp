using ArtHandler.DAL;
using ArtHandler.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace ArtHandler.Repository
{
    //[Flags]
    //public enum AdsUserFlags
    //{
    //    Script = 1,                  // 0x1
    //    AccountDisabled = 2,              // 0x2
    //    HomeDirectoryRequired = 8,           // 0x8 
    //    AccountLockedOut = 16,             // 0x10
    //    PasswordNotRequired = 32,           // 0x20
    //    PasswordCannotChange = 64,           // 0x40
    //    EncryptedTextPasswordAllowed = 128,      // 0x80
    //    TempDuplicateAccount = 256,          // 0x100
    //    NormalAccount = 512,              // 0x200
    //    InterDomainTrustAccount = 2048,        // 0x800
    //    WorkstationTrustAccount = 4096,        // 0x1000
    //    ServerTrustAccount = 8192,           // 0x2000
    //    PasswordDoesNotExpire = 65536,         // 0x10000
    //    MnsLogonAccount = 131072,           // 0x20000
    //    SmartCardRequired = 262144,          // 0x40000
    //    TrustedForDelegation = 524288,         // 0x80000
    //    AccountNotDelegated = 1048576,         // 0x100000
    //    UseDesKeyOnly = 2097152,            // 0x200000
    //    DontRequirePreauth = 4194304,          // 0x400000
    //    PasswordExpired = 8388608,           // 0x800000
    //    TrustedToAuthenticateForDelegation = 16777216, // 0x1000000
    //    NoAuthDataRequired = 33554432         // 0x2000000
    //}

    public class UserRepository
    {
        string DomainName = SingletonLDAPSettings.Instance.LDAPSettings.DomainName;
        string DomainExtn = SingletonLDAPSettings.Instance.LDAPSettings.DomainExtn;
        string LdapPath = SingletonLDAPSettings.Instance.LDAPSettings.LdapConnectionPath;
        string NetUsername = SingletonLDAPSettings.Instance.LDAPSettings.LdapnetworkUsername;
        string NetUserCred = SingletonLDAPSettings.Instance.LDAPSettings.LdapNetworkUserPass;

        #region User
        public string GetUserDateOfBirth(string userId)
        {
            DAL_User objDALUser = new DAL_User();

            return objDALUser.GetUserDateOfBirth(userId);
        }
        /// <summary>
        /// Get the Account lock information , islocked 1 - is account locked and 
        /// waitTime will tell how many minutes user wants to wait
        /// </summary>
        /// <returns></returns>
        public AccountLockModelResponse GetAccountLockDetails(string userId)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.GetAccountLockDetails(userId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        /// <summary>
        /// Used to log the user failure attempts , if the attempts are exceed the configured limit , it will automatically lock
        /// the user , and return as true, else false
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ArtAccountLock(string userId, string sessionId, string activity, string status)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.ArtAccountLock(userId, sessionId, activity, status);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        /// <summary>
        /// delete art user account lock logs
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool DeleteArtAccountLockLogs(string userId)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.DeleteArtAccountLockLogs(userId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        /// <summary>
        /// Enable the OTP option to the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isOtpEnabled"></param>
        /// <returns></returns>
        public bool EnableUserOTP(string userId, bool isOtpEnabled)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.EnableUserOTP(userId, isOtpEnabled);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        /// <summary>
        /// Register the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isRegistered"></param>
        /// <returns></returns>
        public bool RegisterUser(string userId, bool isRegistered)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.RegisterUser(userId, isRegistered);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        /// <summary>
        /// To Check the user enabled OTP
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckUserOTPEnabled(string userId)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.CheckUserOTPEnabled(userId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        /// <summary>
        /// Check the user is registered
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckUserRegistered(string userId)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.CheckUserRegistered(userId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        /// <summary>
        /// Insert the user otp for the session 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="otp"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public bool InsertUserOtp(string userId, string otp, string sessionId, string activity)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.InsertUserOtp(userId, otp, sessionId, activity);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        /// <summary>
        /// To insert the SMS sent details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileNum"></param>
        /// <param name="sentdatetime"></param>
        /// <param name="messageId"></param>
        /// <param name="sessionId"></param>
        /// <param name="activity"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool InsertArtUserSMSSent(string userId, string mobileNum, DateTime sentdatetime, string messageId, string sessionId,
          string activity, string message)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.InsertArtUserSMSSent(userId, mobileNum, sentdatetime, messageId, sessionId, activity, message);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        /// <summary>
        /// To validate the user otp for the session id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="otp"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public bool ValidateUserOtp(string userId, string otp, string sessionId)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.ValidateUserOtp(userId, otp, sessionId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }

        public bool InsertArtUserOTPAttemptInfo(string userId, string sessionId, string activity)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.InsertArtUserOTPAttemptInfo(userId, sessionId, activity);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }

        public bool ResetArtUserOtpAttempts(string userId, string activity)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.ResetArtUserOtpAttempts(userId, activity);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }

        public UserOtpAttemptModel CheckUserOtpAttemptExceed(string userId, string activity)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.CheckUserOtpAttemptExceed(userId, activity);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public string CheckUnUsedOTPExist(string userId, string activity, string sessionId)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.CheckUnUsedOTPExist(userId, activity, sessionId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public List<RptUserModel> CheckUserAccess(string userId)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.CheckUserAccess(userId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        public List<UserQuestionAndAnswer> GetUserQuestionAndAnswer(string userId)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.GetUserQuestionAndAnswer(userId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public List<UserEventsModelDateWise> GetUserEventsModelDateWise(UserEventsInput objUserEventInput)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.GetUserEventsModelDateWise(objUserEventInput);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(objUserEventInput.UserId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public List<UserEventsModel> GetUserEventsDetails(UserEventsInput objUserEventInput)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.GetUserEventsDetails(objUserEventInput);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(objUserEventInput.UserId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public List<UserAgentModel> GetUserAgentDetails(UserAgentInput objUserEventInput)
        {
            try
            {
                List<UserAgentModel> objuseragent = new List<UserAgentModel>();
                DAL_User objDALUser = new DAL_User();
                objuseragent = objDALUser.GetUserAgentDetails(objUserEventInput);
                return objuseragent;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(objUserEventInput.UserId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        public string GetUserActivity(UserEventsInput objUserEventInput)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                DataSet ds = objDALUser.GetUserActivity(objUserEventInput);
                return JsonConvert.SerializeObject(ds);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(objUserEventInput.UserId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        /// <summary>
        /// To Get the User Information
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserInfoModel> GetUserInfo(string userId)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.GetUserInfo(userId);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        /// <summary>
        /// To Get the User Info from DB
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GetUserInfoFromDB(string userid, string propertyName)
        {
            string value = string.Empty;
            List<UserInfoModel> lstUserModel = GetUserInfo(userid);
            if (lstUserModel != null && lstUserModel.Count > 0)
            {
                if (propertyName == Constants.Mobile)
                {
                    if (!string.IsNullOrEmpty(lstUserModel[0].mobilenumber))
                        value = Utility.Encryptor.Decrypt(lstUserModel[0].mobilenumber, Constants.PASSPHARSE);
                }
                else if (propertyName == Constants.ADCOUNTRYCODE)
                {
                    if (!string.IsNullOrEmpty(lstUserModel[0].mobilenumber))
                        value = Utility.Encryptor.Decrypt(lstUserModel[0].countrycode, Constants.PASSPHARSE);
                }
            }
            return value;
        }

        public string GetUserMobileNumber(string userId)
        {
            string mobileNumber = string.Empty;

            try
            {
                List<UserInfoModel> lstUserModel = GetUserInfo(userId);

                if (Singleton.Instance.ClientSessionID.Is_AD_Enabled == "N")
                {
                    if (lstUserModel.Count > 0 && !string.IsNullOrEmpty(lstUserModel[0].mobilenumber))
                        mobileNumber = Utility.Encryptor.Decrypt(lstUserModel[0].mobilenumber, Constants.PASSPHARSE);
                }
                else
                {
                    if (lstUserModel.Count > 0 && lstUserModel[0].ismobilenumberprivate)
                    {
                        mobileNumber = Utility.Encryptor.Decrypt(lstUserModel[0].mobilenumber, Constants.PASSPHARSE);
                    }
                    else
                    {
                        mobileNumber = new UserRepository().GetUserInfoFromAD(userId, Constants.Mobile);
                    }
                }
                return mobileNumber;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return mobileNumber;
            }
        }

        public string GetUserCountryCode(string userId)
        {
            string countrycode = string.Empty;

            try
            {
                UserRepository objUserRepo = new UserRepository();
                List<UserInfoModel> lstUserModel = GetUserInfo(userId);

                if (Singleton.Instance.ClientSessionID.Is_AD_Enabled == "N")
                {
                    if (lstUserModel.Count > 0 && !string.IsNullOrEmpty(lstUserModel[0].countrycode))
                        countrycode = Utility.Encryptor.Decrypt(lstUserModel[0].countrycode, Constants.PASSPHARSE);
                }
                else
                {
                    if (lstUserModel.Count > 0 && lstUserModel[0].ismobilenumberprivate)
                    {
                        countrycode = Utility.Encryptor.Decrypt(lstUserModel[0].countrycode, Constants.PASSPHARSE);
                    }
                    else
                    {
                        countrycode = new UserRepository().GetUserInfoFromAD(userId, Constants.ADCOUNTRYCODE);
                    }
                }
                return countrycode;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException("GetUserCountryCode", ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return countrycode;
            }
        }

        #endregion

        #region Active Directory

        public bool Authenticate(string Username, string Otp)
        {
            bool result;
            try
            {
                bool isValid = false;
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, DomainName + "." + DomainExtn, DomainName + "\\" + NetUsername, NetUserCred))
                {
                    // validate the credentials
                    isValid = pc.ValidateCredentials(Username, Otp);
                }
                return isValid;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(Username, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                result = false;
            }
            return result;
        }
        public bool CheckUserExists(string Username)
        {

            try
            {
                DirectoryEntry directoryEntry = new DirectoryEntry(LdapPath, DomainName + "\\" + NetUsername, NetUserCred);
                DirectorySearcher search = new DirectorySearcher(directoryEntry);
                search.Filter = "(SAMAccountName=" + Username + ")";
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    //DirectoryEntry userEntry = result.GetDirectoryEntry();
                    //userEntry.Properties["pwdLastSet"][0] = -1;
                    //userEntry.CommitChanges();
                    //userEntry.Close();
                    //userEntry.Dispose();
                    directoryEntry.Close();
                    directoryEntry.Dispose();
                    return true;
                }
                else
                {
                    directoryEntry.Close();
                    directoryEntry.Dispose();

                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(Username, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        public bool ResetAndUnlock(string password, string userid)
        {
            try
            {
                //DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://OU=Users,OU=Campus,DC=pwtest1,DC=com", @"pwtest1\32918", "Wks@060s@");
                DirectoryEntry directoryEntry = new DirectoryEntry(LdapPath, DomainName + "\\" + NetUsername, NetUserCred);
                DirectorySearcher search = new DirectorySearcher(directoryEntry);
                search.Filter = "(SAMAccountName=" + userid + ")";
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    DirectoryEntry userEntry = result.GetDirectoryEntry();

                    userEntry.Invoke("SetPassword", new object[] { password });
                    userEntry.Properties["LockOutTime"].Value = 0; //unlock account
                    userEntry.Properties["pwdLastSet"][0] = -1; // from -1 to 0 for user need to change the password in next logon.

                    userEntry.CommitChanges();
                    userEntry.Close();
                    userEntry.Dispose();
                    directoryEntry.Close();
                    directoryEntry.Dispose();

                    return true;
                }
                else
                {
                    directoryEntry.Close();
                    directoryEntry.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userid, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        public bool CheckAccountIsLock(string userid)
        {
            try
            {
                //DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://OU=Users,OU=Campus,DC=pwtest1,DC=com", @"pwtest1\32918", "Wks@060s@");
                DirectoryEntry directoryEntry = new DirectoryEntry(LdapPath, DomainName + "\\" + NetUsername, NetUserCred);
                DirectorySearcher search = new DirectorySearcher(directoryEntry);
                search.Filter = "(SAMAccountName=" + userid + ")";
                SearchResult result = search.FindOne();
                bool isAccountLock = false;
                if (result != null)
                {
                    DirectoryEntry userEntry = result.GetDirectoryEntry();
                    isAccountLock = (bool)userEntry.InvokeGet("IsAccountLocked");

                    userEntry.Close();
                    userEntry.Dispose();
                }

                directoryEntry.Close();
                directoryEntry.Dispose();

                return isAccountLock;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userid, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        public bool UnlockAccount(string userid)
        {
            try
            {
                //DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://OU=Users,OU=Campus,DC=pwtest1,DC=com", @"pwtest1\32918", "Wks@060s@");
                DirectoryEntry directoryEntry = new DirectoryEntry(LdapPath, DomainName + "\\" + NetUsername, NetUserCred);
                DirectorySearcher search = new DirectorySearcher(directoryEntry);
                search.Filter = "(SAMAccountName=" + userid + ")";
                SearchResult result = search.FindOne();
                DirectoryEntry userEntry = result.GetDirectoryEntry();

                userEntry.Properties["LockOutTime"].Value = 0; //unlock account
                userEntry.Properties["pwdLastSet"][0] = -1;

                userEntry.CommitChanges();
                userEntry.Close();
                userEntry.Dispose();
                directoryEntry.Close();
                directoryEntry.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userid, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        public string GetUserInfoFromAD(string userid, string propertyName)
        {
            try
            {
                string userInfo = string.Empty;
                List<UserInfoModel> lstUserModel = GetUserInfo(userid);

                if (!string.IsNullOrEmpty(userid))
                {
                    bool isMobileNumberPrivate = false;

                    if (lstUserModel.Count > 0 && lstUserModel[0].ismobilenumberprivate)
                        isMobileNumberPrivate = true;

                    if (Singleton.Instance.ClientSessionID.Is_AD_Enabled == "N" && (propertyName == Constants.Mobile || propertyName == Constants.ADCOUNTRYCODE))
                    {
                        return GetUserInfoFromDB(userid, propertyName);
                    }
                    else if (Singleton.Instance.ClientSessionID.Is_AD_Enabled == "Y" && isMobileNumberPrivate && (propertyName == Constants.Mobile || propertyName == Constants.ADCOUNTRYCODE))
                    {
                        return GetUserInfoFromDB(userid, propertyName);
                    }

                    //DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://OU=Users,OU=Campus,DC=pwtest1,DC=com", @"pwtest1\32918", "Wks@060s@");
                    DirectoryEntry directoryEntry = new DirectoryEntry(LdapPath, DomainName + "\\" + NetUsername, NetUserCred);
                    DirectorySearcher search = new DirectorySearcher(directoryEntry);
                    search.Filter = "(SAMAccountName=" + userid + ")";
                    SearchResult result = search.FindOne();

                    if (result != null)
                    {
                        DirectoryEntry userEntry = result.GetDirectoryEntry();

                        if (result.Properties.Contains(propertyName))
                            if (result.Properties[propertyName].Count > 0)
                                userInfo = Convert.ToString(result.Properties[propertyName][0]);

                        userEntry.Close();
                        userEntry.Dispose();
                    }
                    else
                    {
                        userInfo = string.Empty;
                    }

                    directoryEntry.Close();
                    directoryEntry.Dispose();
                }

                return userInfo;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userid, ex.Message.ToString() + ": User Id :" + userid + " : Propertyname: " + propertyName, ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public string GetUserPasswordAge(string userId)
        {
            try
            {
                string lastset = GetUserInfoFromAD(userId, Constants.ADPWDLASTSET);
                string days = string.Empty;
                if ((lastset != null && lastset != ""))
                {
                    DateTime dtpwdlstset = DateTime.FromFileTimeUtc(Convert.ToInt64(lastset));
                    TimeSpan ts = DateTime.Now - dtpwdlstset;
                    days = Convert.ToString(ts.Days);
                }
                return days;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return "-NA-";
            }
        }
        /// <summary>
        /// Check the User Password is Expired 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool CheckUserPasswordExpired(string userid)
        {
            try
            {
                PrincipalContext pc = new PrincipalContext(ContextType.Domain, DomainName + "." + DomainExtn, DomainName + "\\" + NetUsername, NetUserCred);
                UserPrincipal user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, userid);

                if (user != null)
                {
                    if (user.PasswordNeverExpires)
                    {
                        return false;
                    }
                }

                //DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://OU=Users,OU=Campus,DC=pwtest1,DC=com", @"pwtest1\32918", "Wks@060s@");
                DirectoryEntry directoryEntry = new DirectoryEntry(LdapPath, DomainName + "\\" + NetUsername, NetUserCred);
                DirectorySearcher search = new DirectorySearcher(directoryEntry);
                search.Filter = "(SAMAccountName=" + userid + ")";
                SearchResult result = search.FindOne();
                bool isExpired = false;

                if (result != null)
                {

                    //AdsUserFlags userFlags = (AdsUserFlags)
                    //  result.GetDirectoryEntry().Properties["userAccountControl"].Value;

                    //Console.WriteLine(
                    //  "AdsUserFlags for {0}: {1}",
                    //  directoryEntry.Path,
                    //  userFlags
                    //  );
                    //userFlags.HasFlag(AdsUserFlags.PasswordDoesNotExpire);


                    DirectoryEntry userEntry = result.GetDirectoryEntry();

                    DateTime dtpwdexpiryDate = Convert.ToDateTime(userEntry.InvokeGet("PasswordExpirationDate"));
                    TimeSpan ts = dtpwdexpiryDate - DateTime.Now;

                    if (ts.Days < 0)
                        isExpired = true;

                    userEntry.Close();
                    userEntry.Dispose();
                }

                directoryEntry.Close();
                directoryEntry.Dispose();

                return isExpired;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userid, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        public bool SetUserInfoInAD(string userId, string propertyName, string value)
        {
            try
            {
                //DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://OU=Users,OU=Campus,DC=pwtest1,DC=com", @"pwtest1\32918", "Wks@060s@");
                DirectoryEntry directoryEntry = new DirectoryEntry(LdapPath, DomainName + "\\" + NetUsername, NetUserCred);
                DirectorySearcher search = new DirectorySearcher(directoryEntry);
                search.Filter = "(SAMAccountName=" + userId + ")";
                SearchResult result = search.FindOne();
                DirectoryEntry userEntry = result.GetDirectoryEntry();

                userEntry.Properties[propertyName].Value = value;
                userEntry.CommitChanges();

                userEntry.Close();
                userEntry.Dispose();
                directoryEntry.Close();
                directoryEntry.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        public string GetUserOU(string userId)
        {
            string transformedLDAPUrl = string.Empty;

            try
            {
                DirectoryEntry directoryEntry = new DirectoryEntry(LdapPath, DomainName + "\\" + NetUsername, NetUserCred);
                DirectorySearcher search = new DirectorySearcher(directoryEntry);
                search.Filter = "(SAMAccountName=" + userId + ")";
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    transformedLDAPUrl = TransformLDAP(result.Path);

                    directoryEntry.Close();
                    directoryEntry.Dispose();
                }
                return transformedLDAPUrl;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return transformedLDAPUrl;
            }
        }
        public string TransformLDAP(string ldapurl)
        {
            string transformedURL = string.Empty;
            StringBuilder strbul = new StringBuilder();
            try
            {
                string[] paths = ldapurl.Split(',');
                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].StartsWith("DC="))
                    {
                        strbul.Append(paths[i].Trim().Split('=').GetValue(1) + ".");
                    }
                }
                transformedURL = strbul.ToString();
                transformedURL = transformedURL.Substring(0, transformedURL.Length - 1);
                strbul = new StringBuilder();
                strbul.Append("/");
                for (int i = paths.Length - 1; i > 0; i--)
                {
                    if (paths[i].StartsWith("OU="))
                    {
                        strbul.Append(paths[i].Trim().Split('=').GetValue(1) + "/");
                    }
                }
                transformedURL += strbul.ToString();
                transformedURL = transformedURL.Substring(0, transformedURL.Length - 1);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(string.Empty, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
            return transformedURL;
        }

        public List<Tuple<string, string>> GetUserGroupInfo(string userName)
        {
            try
            {
                PrincipalContext pc = new PrincipalContext(ContextType.Domain, DomainName + "." + DomainExtn, DomainName + "\\" + NetUsername, NetUserCred);
                UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, userName);
                List<Tuple<string, string>> lstGroup = new List<Tuple<string, string>>();

                if (up != null)
                {
                    // fetch the group list
                    PrincipalSearchResult<Principal> groups = up.GetAuthorizationGroups();

                    foreach (Principal item in groups)
                    {
                        lstGroup.Add(new Tuple<string, string>(item.Name, item.DistinguishedName));
                    }
                }

                return lstGroup;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userName, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public bool UpdateUserInfo(string userId, string countryCode, string mobileNumber)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.UpdateUserInfo(userId, countryCode, mobileNumber);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }

        public List<Tuple<string, string>> GetBitLockerInfo(string userId, string computerName)
        {
            List<Tuple<string, string>> lstbitInfo = new List<Tuple<string, string>>();

            try
            {
                //DirectoryEntry child = new DirectoryEntry("LDAP://corp.hexaware.com", "CORP" + "\\" + "RaiseITPwdReset", "W3lc0m3@12");
                DirectoryEntry child = new DirectoryEntry(LdapPath, DomainName + "\\" + NetUsername, NetUserCred);

                DirectoryEntries enteries = child.Children;
                DirectorySearcher ds = new DirectorySearcher(child);
                ds.Filter = "(&(objectCategory=Computer)(cn=" + computerName + "))";
                ds.SearchScope = SearchScope.Subtree;
                SearchResult item = ds.FindOne();
                if (item != null)
                {
                    string dn = item.GetDirectoryEntry().Properties["distinguishedname"].Value.ToString();
                    // string temp1 = System.DirectoryServices.AccountManagement.UserPrincipal.Current.DistinguishedName;
                    //child = new DirectoryEntry(("LDAP://" + dn), "CORP" + "\\" + "RaiseITPwdReset", "W3lc0m3@12");
                    child = new DirectoryEntry(("LDAP://" + dn), DomainName + "\\" + NetUsername, NetUserCred);

                    enteries = child.Children;
                    ds = new DirectorySearcher(child);
                    ds.Filter = "(objectClass=msFVE-RecoveryInformation)";
                    ds.SearchScope = SearchScope.Subtree;

                    SearchResultCollection result = ds.FindAll();


                    if (result != null && result.Count > 0)
                    {
                        foreach (SearchResult sr in result)
                        {
                            //string recoveryKeyPackage = sr.GetDirectoryEntry().Properties["msFVE-KeyPackage"].Value.ToString();
                            string recoveryGUID = new Guid((byte[])sr.GetDirectoryEntry().Properties["msFVE-RecoveryGuid"].Value).ToString();
                            string recoveryPassword = sr.GetDirectoryEntry().Properties["msFVE-RecoveryPassword"].Value.ToString();
                            //string recoveryVolumeGUID = new Guid((byte[])sr.GetDirectoryEntry().Properties["msFVE-VolumeGuid"].Value).ToString();

                            lstbitInfo.Add(new Tuple<string, string>(recoveryGUID.ToUpper(), recoveryPassword.ToUpper()));
                        }
                    }
                }

                return lstbitInfo;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        #endregion

        public bool InsertEnrollmentLink(string userId, string mobileno, DateTime date, string messageId, string sessionId, string activity, string tempbody, string status, string key, string sendOTP)
        {
            try
            {
                DAL_User objDALUser = new DAL_User();
                return objDALUser.InsertEnrollmentLink(userId, mobileno, date, messageId, sessionId, activity, tempbody, status, key, sendOTP);
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(userId, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
    }
}
