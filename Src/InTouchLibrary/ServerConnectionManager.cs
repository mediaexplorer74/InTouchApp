// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.ServerConnectionManager
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking.PushNotifications;
using Windows.Phone.PersonalInformation;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;
using Windows.System.Profile;

#nullable disable
namespace InTouchLibrary
{
  public class ServerConnectionManager
  {
    public static bool mIsDeveloper;

    public bool mIsConnectedToNetwork
    {
      get
      {
        try
        {
          ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();

          return connectionProfile != null 
            && connectionProfile.GetNetworkConnectivityLevel() 
                   == NetworkConnectivityLevel.InternetAccess;
        }
        catch (Exception ex)
        {
          LogFile.Log("Problem in finding internet connectivity. " + ex.Message, EventType.Warning);
          return false;
        }
      }
    }

    public bool mIsWiFiAvailable
    {
      get
      {
        try
        {
          ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();

          return connectionProfile != null 
            && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess 
            && connectionProfile.NetworkAdapter.IanaInterfaceType == 71U;
        }
        catch (Exception ex)
        {
          LogFile.Log("Problem in finding wifi connectivity. " + ex.Message, EventType.Warning);
          return false;
        }
      }
    }

    public static bool mIsDebug => !string.IsNullOrEmpty(LocalSettings.localSettings.serverName);

    public string getServerName()
    {
      try
      {
        string empty = string.Empty;
        string serverName;
        if (ServerConnectionManager.mIsDebug)
        {
          serverName = LocalSettings.localSettings.serverName;
          if (serverName == "https://api13.intouchapp.com")
            serverName = "https://new.intouchapp.com";
        }
        else
          serverName = "https://new.intouchapp.com";
        return serverName;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in retrieving server name. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public string getServerNameAPI()
    {
      try
      {
        string empty = string.Empty;
        return !ServerConnectionManager.mIsDebug 
                    ? "https://api13.intouchapp.com" 
                    : LocalSettings.localSettings.serverName;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in retrieving server name for API. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private string getConsumerKey()
    {
      try
      {
        string consumerKey = "3MJbgAuzgHdvnuHFmv";

        if (ServerConnectionManager.mIsDebug
            && string.Compare(LocalSettings.localSettings.serverName, 
            "https://api13.intouchapp.com") != 0)
          consumerKey = "5yApyuqDeJB6YtrkPh";
        return consumerKey;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in retrieving consumer key. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public LoginEssentials getLoginEssentials()
    {
      try
      {
        EasClientDeviceInformation deviceInformation = new EasClientDeviceInformation();
        LoginEssentials loginEssentials = new LoginEssentials();

        loginEssentials.consumer_key = this.getConsumerKey();
        loginEssentials.model = deviceInformation.SystemProductName;

        IBuffer id = HardwareIdentification.GetPackageSpecificToken((IBuffer) null).Id;

        string hexString = CryptographicBuffer.EncodeToHexString(
            HashAlgorithmProvider.OpenAlgorithm("MD5").HashData(id));
        loginEssentials.imei = hexString;
        loginEssentials.os = "winphone";
        loginEssentials.os_ver = "8.1";
        loginEssentials.vendor = deviceInformation.SystemManufacturer;
        LogFile.Log("Retrieved device information.", EventType.Information);
        return loginEssentials;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting login essentials. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task<Tuple<bool, RootObjectToken, string, string, string>> loginToInTouch(
      string InTouchID,
      string Password)
    {
      RootObjectToken rootObjectToken = new RootObjectToken();
      string responseMessage = string.Empty;
      string actionUrl = string.Empty;
      string action = string.Empty;
      try
      {
        LoginEssentials loginEssentials = this.getLoginEssentials();
        string channelUri = string.Empty;
        PushNotificationChannel channel = 
           await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
        if (channel != null)
          channelUri = channel.Uri;
        loginEssentials.push_mesg_key = channelUri;
        try
        {
          string uri = this.getServerNameAPI() + "/api/basic/direct_access_token/";
          HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
          webRequest.Headers["Authorization"] = "Basic " 
                        + Convert.ToBase64String(Encoding.UTF8.GetBytes(InTouchID + ":" + Password));
          webRequest.Method = "POST";
          webRequest.ContentType = "application/json";

          string json = JsonConvert.SerializeObject(
              (object) loginEssentials, new JsonSerializerSettings()
          {
            NullValueHandling = NullValueHandling.Ignore
          });
          using (Stream requestStream = await webRequest.GetRequestStreamAsync())
          {
            using (StreamWriter streamWriter = new StreamWriter(requestStream))
              streamWriter.Write(json);
          }
          HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
          string jsonResponse = string.Empty;
          StreamReader sReader = new StreamReader(webResponse.GetResponseStream());
          jsonResponse = sReader.ReadToEnd();
          rootObjectToken = JsonConvert.DeserializeObject<RootObjectToken>(jsonResponse);
          sReader.Dispose();
          webResponse.Dispose();
          LocalSettings.localSettings.channelUri = channelUri;
          return Tuple.Create<bool, RootObjectToken, string, string, string>(true, rootObjectToken, responseMessage, actionUrl, action);
        }
        catch (WebException ex)
        {
          WebResponse response = ex.Response;
          if (response != null)
          {
            HttpWebResponse httpWebResponse = (HttpWebResponse) response;
            string empty = string.Empty;
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            RootObjectLoginFailed objectLoginFailed = JsonConvert.DeserializeObject<RootObjectLoginFailed>(streamReader.ReadToEnd());
            responseMessage = objectLoginFailed.message;
            action = objectLoginFailed.action;
            actionUrl = objectLoginFailed.action_url;
            streamReader.Dispose();
            httpWebResponse.Dispose();
          }

          if (string.IsNullOrEmpty(responseMessage))
          {
            responseMessage = "Failed to login";
            throw;
          }
          else
          {
            LogFile.Log("Login failed. " + ex.Message, EventType.Warning);

            return Tuple.Create<bool, RootObjectToken, string, string, string>(
                false, rootObjectToken, responseMessage, actionUrl, action);
          }
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Login failed. " + ex.Message, EventType.Error);
        responseMessage = "Failed to login";

        return Tuple.Create<bool, RootObjectToken, string, string, string>(
            false, rootObjectToken, responseMessage, actionUrl, action);
      }
    }

    public async Task logoutFromInTouch(string token)
    {
      try
      {
        string uri = this.getServerNameAPI() + "/api/basic/release_token/";
        CredentialCache wrCache = new CredentialCache();
        wrCache.Add(new Uri(uri), "Basic", new NetworkCredential(this.getConsumerKey(), token));
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
        webRequest.Credentials = (ICredentials) wrCache;
        webRequest.Method = "GET";
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        webResponse.Dispose();
        LogFile.Log("Logout successful.", EventType.Information);
      }
      catch
      {
        LogFile.Log("Logout API failed.", EventType.Warning);
      }
    }

    public async Task<RootObjectContactbookManual> downloadContactbookManual(
      string token,
      int versionManual)
    {
      RootObjectContactbookManual contactsManual = new RootObjectContactbookManual();
      RootObjectContactbookManual contactbookManual;
      try
      {
        bool isSuccess = false;
        string uri = this.getServerNameAPI() + "/api/v1/contactbook_manual/json/";

        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);

        webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(
            Encoding.UTF8.GetBytes(this.getConsumerKey() + ":" + token));
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json";
        string json = JsonConvert.SerializeObject((object) new LoadContactbook()
        {
          version = versionManual
        }, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });

        using (Stream requestStream = await webRequest.GetRequestStreamAsync())
        {
          using (StreamWriter streamWriter = new StreamWriter(requestStream))
            streamWriter.Write(json);
        }
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        isSuccess = webResponse.StatusCode == HttpStatusCode.OK;
        if (isSuccess)
        {
          string empty = string.Empty;
          StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());

          contactsManual = JsonConvert.DeserializeObject<RootObjectContactbookManual>(
              streamReader.ReadToEnd());
          if (string.Equals(contactsManual.status, "error", StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException(contactsManual.message);
          streamReader.Dispose();
        }
        webResponse.Dispose();
        int count = 0;
        // ISSUE: reference to a compiler-generated field
        if (ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1d == null)
        {
          // ISSUE: reference to a compiler-generated field
          ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1d = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (ServerConnectionManager), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target1 = ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1d.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> pSite1d = ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1d;
        // ISSUE: reference to a compiler-generated field
        if (ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1e == null)
        {
          // ISSUE: reference to a compiler-generated field
          ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1e = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (ServerConnectionManager), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj1 = ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1e.Target((CallSite) ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1e, contactsManual.contacts, (object) null);
        if (target1((CallSite) pSite1d, obj1))
        {
          // ISSUE: reference to a compiler-generated field
          if (ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1f == null)
          {
            // ISSUE: reference to a compiler-generated field
            ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1f = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (int), typeof (ServerConnectionManager)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, int> target2 = ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1f.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, int>> pSite1f = ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site1f;
          // ISSUE: reference to a compiler-generated field
          if (ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site20 == null)
          {
            // ISSUE: reference to a compiler-generated field
            ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Count", typeof (ServerConnectionManager), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj2 = ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site20.Target((CallSite) ServerConnectionManager.\u003CdownloadContactbookManual\u003Eo__SiteContainer1c.\u003C\u003Ep__Site20, contactsManual.contacts);
          count = target2((CallSite) pSite1f, obj2);
        }
        LogFile.Log("Downloaded Manual contacts (version = " + (object) versionManual + ", contacts = " + (object) count + ").", EventType.Information);
        contactbookManual = contactsManual;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in downloading manual contactbook. " + ex.Message, EventType.Error);
        contactsManual = (RootObjectContactbookManual) null;
        throw;
      }
      return contactbookManual;
    }

    public async Task<RootObjectContactbookAuto> downloadContactbookAuto(
      string token,
      int versionInTouch)
    {
      RootObjectContactbookAuto contactsAuto = new RootObjectContactbookAuto();
      RootObjectContactbookAuto objectContactbookAuto;
      try
      {
        bool isSuccess = false;
        string uri = this.getServerNameAPI() + "/api/v1/contactbook/json/";
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
        webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.getConsumerKey() + ":" + token));
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json";
        string json = JsonConvert.SerializeObject((object) new LoadContactbook()
        {
          version = versionInTouch
        }, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        using (Stream requestStream = await webRequest.GetRequestStreamAsync())
        {
          using (StreamWriter streamWriter = new StreamWriter(requestStream))
            streamWriter.Write(json);
        }
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        isSuccess = webResponse.StatusCode == HttpStatusCode.OK;
        if (isSuccess)
        {
          string empty = string.Empty;
          StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
          contactsAuto = JsonConvert.DeserializeObject<RootObjectContactbookAuto>(streamReader.ReadToEnd());
          if (string.Equals(contactsAuto.status, "error", StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException(contactsAuto.message);
          streamReader.Dispose();
        }
        webResponse.Dispose();
        int count = 0;
        if (contactsAuto.contacts != null)
          count = contactsAuto.contacts.Count;
        LogFile.Log("Downloaded Auto contacts (version = " + (object) versionInTouch + ", contacts = " + (object) count + ").", EventType.Information);
        objectContactbookAuto = contactsAuto;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in downloading auto contactbook. " + ex.Message, EventType.Error);
        contactsAuto = (RootObjectContactbookAuto) null;
        throw;
      }
      return objectContactbookAuto;
    }

    public async Task<Stream> downloadPhoto(string contactPhotoUrl, string MCI_CID, string token)
    {
      try
      {
        Stream stream = (Stream) null;
        LogFile.Log("Downloading photo of contact " + MCI_CID + ".", EventType.Debug);
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(contactPhotoUrl);
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        stream = webResponse.GetResponseStream();
        webResponse.Dispose();
        return stream;
      }
      catch (Exception ex)
      {
        string message = MCI_CID + ": Error in downloading photo of contact with URL: \"" + contactPhotoUrl + "\"." + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
        return (Stream) null;
      }
    }

    public async Task<string> passwordReset(string email)
    {
      string messageText = string.Empty;
      try
      {
        string uri = this.getServerNameAPI() + "/api/v1/reset_password/";
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json";
        string json = JsonConvert.SerializeObject((object) new ResetEmail()
        {
          consumer_key = this.getConsumerKey(),
          email = email
        }, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        using (Stream requestStream = await webRequest.GetRequestStreamAsync())
        {
          using (StreamWriter streamWriter = new StreamWriter(requestStream))
            streamWriter.Write(json);
        }
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        string jsonResponse = string.Empty;
        StreamReader sReader = new StreamReader(webResponse.GetResponseStream());
        jsonResponse = sReader.ReadToEnd();
        ForgotPassword forgotPassword = new ForgotPassword();
        forgotPassword = JsonConvert.DeserializeObject<ForgotPassword>(jsonResponse);
        messageText = forgotPassword.message;
        sReader.Dispose();
        webResponse.Dispose();
        LogFile.Log("Reset password successful.", EventType.Information);
        return messageText;
      }
      catch (Exception ex)
      {
        LogFile.Log("Problem in resetting password. " + ex.Message, EventType.Warning);
        messageText = "Failed to reset password.";
        return messageText;
      }
    }

    public async Task<Tuple<bool, string, string>> getUserAccountStatus(string token)
    {
      string accountTypeName = string.Empty;
      string accountType = string.Empty;
      try
      {
        string uri = this.getServerNameAPI() + "/api/v1/account_status/";
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
        webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.getConsumerKey() + ":" + token));
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json";
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        string jsonResponse = string.Empty;
        StreamReader sReader = new StreamReader(webResponse.GetResponseStream());
        jsonResponse = sReader.ReadToEnd();
        RootObjectAccountStatus rootObjectAccountStatus = new RootObjectAccountStatus();
        rootObjectAccountStatus = JsonConvert.DeserializeObject<RootObjectAccountStatus>(jsonResponse);
        if (string.Equals(rootObjectAccountStatus.status, "error", StringComparison.OrdinalIgnoreCase))
          throw new NotSupportedException(rootObjectAccountStatus.message);
        accountTypeName = rootObjectAccountStatus.account_status.account_type_name;
        accountType = rootObjectAccountStatus.account_status.account_type;
        sReader.Dispose();
        webResponse.Dispose();
        return Tuple.Create<bool, string, string>(true, accountTypeName, accountType);
      }
      catch (Exception ex)
      {
        LogFile.Log("Unable to get account type of user. " + ex.Message, EventType.Error);
        return !(ex is NotSupportedException) ? Tuple.Create<bool, string, string>(false, accountTypeName, accountType) : Tuple.Create<bool, string, string>(false, ex.Message, accountType);
      }
    }

    public async Task<bool> bugReporting(string id, string message, string token)
    {
      try
      {
        string uri = this.getServerNameAPI() + "/api/v1/bug_report/";
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
        webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.getConsumerKey() + ":" + token));
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json";
        string json = JsonConvert.SerializeObject((object) new BugReporting()
        {
          id = id,
          message = message
        }, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        using (Stream requestStream = await webRequest.GetRequestStreamAsync())
        {
          using (StreamWriter streamWriter = new StreamWriter(requestStream))
            streamWriter.Write(json);
        }
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        string jsonResponse = string.Empty;
        StreamReader sReader = new StreamReader(webResponse.GetResponseStream());
        jsonResponse = sReader.ReadToEnd();
        JsonReply jsonReply = JsonConvert.DeserializeObject<JsonReply>(jsonResponse);
        if (string.Equals(jsonReply.status, "error", StringComparison.OrdinalIgnoreCase))
          throw new NotSupportedException(jsonReply.message);
        LogFile.Log("Reported bug : " + id + ".", EventType.Information);
        sReader.Dispose();
        webResponse.Dispose();
        return true;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in reporting bug " + id + ". " + ex.Message, EventType.Error);
        return false;
      }
    }

    public async Task<bool> uploadContacts(UploadContactbook uploadContacts, string token)
    {
      bool flag;
      try
      {
        bool isSuccess = false;
        string uri = this.getServerNameAPI() + "/api/v1/upload_contacts/json/";
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
        webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.getConsumerKey() + ":" + token));
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json";
        string json = JsonConvert.SerializeObject((object) uploadContacts, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        using (Stream requestStream = await webRequest.GetRequestStreamAsync())
        {
          using (StreamWriter streamWriter = new StreamWriter(requestStream))
            streamWriter.Write(json);
        }
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        isSuccess = webResponse.StatusCode == HttpStatusCode.OK;
        if (isSuccess)
        {
          string empty = string.Empty;
          StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
          JsonReply jsonReply = JsonConvert.DeserializeObject<JsonReply>(streamReader.ReadToEnd());
          if (string.Equals(jsonReply.status, "error", StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException(jsonReply.message);
          streamReader.Dispose();
        }
        webResponse.Dispose();
        LogFile.Log("Uploaded WP contacts (packet = " + (object) uploadContacts.packet_id + ", count = " + (object) uploadContacts.contacts.Count + ").", EventType.Information);
        flag = true;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in uploading packet " + (object) uploadContacts.packet_id + ". " + ex.Message, EventType.Error);
        throw;
      }
      return flag;
    }

    public async Task<Tuple<bool, string>> feedback(string token, string feedbackMessage)
    {
      try
      {
        string message = string.Empty;
        string uri = this.getServerNameAPI() + "/api/v1/user_feedback/";
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
        webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.getConsumerKey() + ":" + token));
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json";
        string json = JsonConvert.SerializeObject((object) new Feedback()
        {
          message = feedbackMessage
        }, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        using (Stream requestStream = await webRequest.GetRequestStreamAsync())
        {
          using (StreamWriter streamWriter = new StreamWriter(requestStream))
            streamWriter.Write(json);
        }
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        string jsonResponse = string.Empty;
        StreamReader sReader = new StreamReader(webResponse.GetResponseStream());
        jsonResponse = sReader.ReadToEnd();
        FeedbackReply feedbackReply = new FeedbackReply();
        feedbackReply = JsonConvert.DeserializeObject<FeedbackReply>(jsonResponse);
        message = feedbackReply.message;
        sReader.Dispose();
        webResponse.Dispose();
        return Tuple.Create<bool, string>(true, message);
      }
      catch (WebException ex)
      {
        string str = string.Empty;
        WebResponse response = ex.Response;
        if (response != null)
        {
          HttpWebResponse httpWebResponse = (HttpWebResponse) response;
          StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
          str = streamReader.ReadToEnd();
          streamReader.Dispose();
          httpWebResponse.Dispose();
        }
        return Tuple.Create<bool, string>(false, str);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in reporting feedback. " + ex.Message, EventType.Error);
        return Tuple.Create<bool, string>(false, "Error in reporting feedback. ");
      }
    }

    public async Task<RegistrationReply> registerUser(RegistrationEssentials registrationEssentials)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      RegistrationReply registrationReply1;
      try
      {
        string uri = this.getServerNameAPI() + "/api/v1/register_user/";
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json";
        registrationEssentials.consumer_key = this.getConsumerKey();
        string json = JsonConvert.SerializeObject((object) registrationEssentials, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        using (Stream requestStream = await webRequest.GetRequestStreamAsync())
        {
          using (StreamWriter streamWriter = new StreamWriter(requestStream))
            streamWriter.Write(json);
        }
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        RegistrationReply registrationReply = new RegistrationReply();
        string jsonResponse = string.Empty;
        StreamReader sReader = new StreamReader(webResponse.GetResponseStream());
        jsonResponse = sReader.ReadToEnd();
        registrationReply = JsonConvert.DeserializeObject<RegistrationReply>(jsonResponse);
        sReader.Dispose();
        webResponse.Dispose();
        registrationReply1 = registrationReply;
      }
      catch (Exception ex)
      {
        LogFile.Log("Unable to register user. " + ex.Message, EventType.Error);
        throw;
      }
      return registrationReply1;
    }

    public async Task pushMessaging(string token, string channelUri)
    {
      try
      {
        string uri = this.getServerNameAPI() + "/api/v1/upload_push_msg_key/";
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
        webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.getConsumerKey() + ":" + token));
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json";
        string json = JsonConvert.SerializeObject((object) new UploadPushMessage()
        {
          key = channelUri,
          device_os = "winphone"
        }, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        using (Stream requestStream = await webRequest.GetRequestStreamAsync())
        {
          using (StreamWriter streamWriter = new StreamWriter(requestStream))
            streamWriter.Write(json);
        }
        HttpWebResponse webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
        LocalSettings.localSettings.channelUri = webResponse.StatusCode != HttpStatusCode.OK ? string.Empty : channelUri;
      }
      catch (Exception ex)
      {
        LocalSettings.localSettings.channelUri = string.Empty;
        LogFile.Log("Unable to send push message key to server. " + ex.Message, EventType.Error);
      }
    }

    public async Task uploadClientState(string token)
    {
      try
      {
        string uri = this.getServerNameAPI() + "/api/v1/upload_client_state/";
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
        webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.getConsumerKey() + ":" + token));
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json";
        UploadClientState uploadClientState = new UploadClientState();
        STATE_DATA_DICT stateDataDict = new STATE_DATA_DICT();
        stateDataDict.SyncSessionID = LocalSettings.localSettings.syncSessionID;
        stateDataDict.SyncSessionOwner = LocalSettings.localSettings.syncSessionOwner;
        SyncSessionState SyncState = (SyncSessionState) LocalSettings.localSettings.syncSessionState;
        stateDataDict.SyncSessionState = SyncState.ToString();
        stateDataDict.SyncSessionLastUpdateTime = LocalSettings.localSettings.syncSessionLastUpdateTime;
        stateDataDict.DBDirtyIntouchContactsCount = InTouchAppDatabase.InTouchAppDB.getDirtyIntouchEntriesCount(LocalSettings.localSettings.MCI).ToString();
        if (ContactstoreInTouch.contactStore == null)
        {
          Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
          if (result.Item2)
            await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
        }
        IReadOnlyList<ContactChangeRecord> result1 = await ContactstoreInTouch.contactStore.GetChangesAsync((ulong) LocalSettings.localSettings.revisionNumber);
        stateDataDict.StoreModifiedContactsCount = result1.Count.ToString();
        stateDataDict.SavedRevisionNumber = LocalSettings.localSettings.revisionNumber.ToString();
        stateDataDict.StoreRevisionNumber = ContactstoreInTouch.contactStore.RevisionNumber.ToString();
        uploadClientState.state = stateDataDict;
        string json = JsonConvert.SerializeObject((object) uploadClientState, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        using (Stream requestStream = await webRequest.GetRequestStreamAsync())
        {
          using (StreamWriter streamWriter = new StreamWriter(requestStream))
            streamWriter.Write(json);
        }
        HttpWebResponse responseAsync = (HttpWebResponse) await webRequest.GetResponseAsync();
      }
      catch (Exception ex)
      {
        LogFile.Log("Unable to upload client state. " + ex.Message, EventType.Error);
      }
    }
  }
}
