// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.CommonCode
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

#nullable disable
namespace InTouchLibrary
{
  public class CommonCode
  {
    private static volatile CommonCode _commonCode;
    public static Random random = new Random();
    private List<string> bugList = new List<string>();

    public static CommonCode commonCode
    {
      get
      {
        try
        {
          if (CommonCode._commonCode == null)
            CommonCode._commonCode = new CommonCode();
          return CommonCode._commonCode;
        }
        catch
        {
          throw;
        }
      }
    }

    public string createRandomStringID()
    {
      try
      {
        string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] chArray = new char[15];
        for (int index = 0; index < chArray.Length; ++index)
          chArray[index] = str[CommonCode.random.Next(str.Length)];
        return "c" + new string(chArray);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in creating contact_id_1. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public string createRandomStringSession()
    {
      try
      {
        string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] chArray = new char[16];
        for (int index = 0; index < chArray.Length; ++index)
          chArray[index] = str[CommonCode.random.Next(str.Length)];
        return new string(chArray);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in creating sessionID. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void reportBug(string id, string message)
    {
      try
      {
        this.bugList.Add(message);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in maintaining bug list. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task bugReporter()
    {
      try
      {
        bool isSuccess = false;
        StringBuilder Bug_message = new StringBuilder();
        for (int index = 0; index < this.bugList.Count; ++index)
          Bug_message.AppendLine(this.bugList[index]);
        if (string.IsNullOrEmpty(Bug_message.ToString()))
          return;
        ServerConnectionManager connectionManager1 = new ServerConnectionManager();
        string message = Bug_message.ToString();
        ServerConnectionManager connectionManager2 = connectionManager1;
        string token = await LocalSettings.localSettings.getToken();
        isSuccess = await connectionManager2.bugReporting("Sync", message, token);
        if (!isSuccess)
          return;
        this.bugList.Clear();
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in bug reporter. " + ex.Message, EventType.Error);
      }
    }

    public string getDisplayName(List<Avatar> avatar, Name contactName)
    {
      try
      {
        string displayName = string.Empty;
        bool flag = false;
        if (avatar != null)
        {
          foreach (Avatar avatar1 in avatar)
          {
            if (avatar1.name != null)
            {
              displayName = avatar1.name.prefix;
              if (!string.IsNullOrEmpty(avatar1.name.given))
                displayName = string.IsNullOrEmpty(displayName) ? avatar1.name.given : displayName + " " + avatar1.name.given;
              if (!string.IsNullOrEmpty(avatar1.name.middle))
                displayName = string.IsNullOrEmpty(displayName) ? avatar1.name.middle : displayName + " " + avatar1.name.middle;
              if (!string.IsNullOrEmpty(avatar1.name.family))
                displayName = string.IsNullOrEmpty(displayName) ? avatar1.name.family : displayName + " " + avatar1.name.family;
              if (!string.IsNullOrEmpty(avatar1.name.suffix))
                displayName = string.IsNullOrEmpty(displayName) ? avatar1.name.suffix : displayName + " " + avatar1.name.suffix;
              flag = true;
              break;
            }
          }
        }
        if (!flag && contactName != null)
        {
          displayName = contactName.prefix;
          if (!string.IsNullOrEmpty(contactName.given))
            displayName = string.IsNullOrEmpty(displayName) ? contactName.given : displayName + " " + contactName.given;
          if (!string.IsNullOrEmpty(contactName.middle))
            displayName = string.IsNullOrEmpty(displayName) ? contactName.middle : displayName + " " + contactName.middle;
          if (!string.IsNullOrEmpty(contactName.family))
            displayName = string.IsNullOrEmpty(displayName) ? contactName.family : displayName + " " + contactName.family;
          if (!string.IsNullOrEmpty(contactName.suffix))
            displayName = string.IsNullOrEmpty(displayName) ? contactName.suffix : displayName + " " + contactName.suffix;
        }
        if (string.IsNullOrEmpty(displayName))
          displayName = string.Empty;
        return displayName;
      }
      catch (Exception ex)
      {
        LogFile.Log("Problem in getting display name." + ex.Message, EventType.Warning);
        return string.Empty;
      }
    }

    public string getHash(string originalData)
    {
      try
      {
        string hash = string.Empty;
        if (!string.IsNullOrEmpty(originalData))
        {
          HashAlgorithmProvider algorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
          IBuffer binary = CryptographicBuffer.ConvertStringToBinary(originalData, (BinaryStringEncoding) 0);
          IBuffer ibuffer = algorithmProvider.HashData(binary);
          if ((int) ibuffer.Length != (int) algorithmProvider.HashLength)
            throw new Exception("There was an error creating the hash");
          hash = CryptographicBuffer.EncodeToBase64String(ibuffer);
        }
        return hash;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting hash. " + ex.Message, EventType.Error);
        throw;
      }
    }
  }
}
