using ArtHandler.DAL;
using ArtHandler.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtHandler.Repository
{
    public class SettingsRepository
    {
        //public List<SettingsModel> GetSetting(string settingName)
        //{
        //    try
        //    {
        //        List<SettingsModel> lstSettings = new List<SettingsModel>();
        //        DAL_Settings objDALSetting = new DAL_Settings();

        //        lstSettings = objDALSetting.GetSetting(settingName);

        //        return lstSettings;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString()));
        //        return null;
        //    }
        //}
        public List<OptionsModel> GetOptions()
        {
            try
            {
                List<OptionsModel> lstOption = new List<OptionsModel>();
                DAL_Settings objDALSetting = new DAL_Settings();

                lstOption = objDALSetting.GetOptions();

                return lstOption;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        public List<LanguageModel> Getlanguages()
        {
            try
            {
                List<LanguageModel> lstLang = new List<LanguageModel>();
                DAL_Settings objDALSetting = new DAL_Settings();

                lstLang = objDALSetting.Getlanguages();

                return lstLang;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        public static bool InitializeApplicationSettings()
        {
            try
            {
                List<SettingsModel> lstSettings = new List<SettingsModel>();
                ApplicationSettingsModel objAppSettings = new ApplicationSettingsModel();

                Singleton clientsessionidinstance = Singleton.Instance;
                DAL_Settings objDALSetting = new DAL_Settings();
                objAppSettings = objDALSetting.GetApplicationSettings();

                clientsessionidinstance.ClientSessionID = objAppSettings;

                return true;
            }
            catch (Exception ex)
            {
                //Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }

        public static bool InitializePasswordSettings()
        {
            try
            {
                List<PasswordSettingsModel> lstSettings = new List<PasswordSettingsModel>();

                SingletonPasswordSettings singletonPassword = SingletonPasswordSettings.Instance;
                DAL_Settings objDALSetting = new DAL_Settings();
                lstSettings = objDALSetting.GetPasswordSettings();

                singletonPassword.PasswordSettings = lstSettings[0];

                return true;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }

        public static bool InitializeLDAPSettings()
        {
            try
            {
                List<LdapSettingsModel> lstldapSettings = new List<LdapSettingsModel>();

                SingletonLDAPSettings singletonLdap = SingletonLDAPSettings.Instance;
                DAL_Settings objDALSetting = new DAL_Settings();
                lstldapSettings = objDALSetting.GetArtLdapSettings();

                singletonLdap.LDAPSettings = lstldapSettings[0];

                return true;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }
        public static bool InitializeArtValidationDBContext()
        {
            try
            {
                List<ArtValidationDBContext> lstdbContext = new List<ArtValidationDBContext>();

                SingletonArtValidationDBContext singletonLdap = SingletonArtValidationDBContext.Instance;
                DAL_Settings objDALSetting = new DAL_Settings();
                lstdbContext = objDALSetting.GetArtValidationDBContext();

                singletonLdap.DBValidationContext = lstdbContext[0];

                return true;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return false;
            }
        }

        public List<CountryCodeModel> GetAllCountryCodeDetails()
        {
            try
            {
                List<CountryCodeModel> lstLang = new List<CountryCodeModel>();
                DAL_Settings objDALSetting = new DAL_Settings();

                lstLang = objDALSetting.GetAllCountryCodeDetails();

                return lstLang;
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }
        /// <summary>
        /// Validate and add country code to the mobile number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public string ValidateAndAddCountryCode(string mobileNumber, string countryCode)
        {
            string extractedMob = string.Empty;
            int allowdMobileLength = 0;
            try
            {
                using (DataTable dt = new DAL.DAL_Settings().GetCountryTelephoneCodes(countryCode))
                {
                    if (dt.Rows.Count > 0)
                    {
                        string countryTelCode = Convert.ToString(dt.Rows[0]["TelephoneCode"]);
                        allowdMobileLength = Convert.ToInt32(dt.Rows[0]["MobileNumberLength"]);
                        if (mobileNumber.Length >= allowdMobileLength)
                        {
                            mobileNumber = mobileNumber.Substring(mobileNumber.Length - allowdMobileLength, allowdMobileLength);
                            if (Utility.HasSpecialCharacters(mobileNumber))
                            {
                                extractedMob = string.Empty;
                            }
                            else if (!Utility.IsNumberonly(mobileNumber))
                            {
                                extractedMob = string.Empty;
                            }
                            else
                            {
                                extractedMob = countryTelCode + mobileNumber;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(new CustomException(null, ex.Message.ToString(), ex.StackTrace.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return extractedMob;
        }

    }
}
