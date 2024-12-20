// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.PushMessage
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

#nullable disable
namespace InTouchLibrary
{
  public class PushMessage
  {
    public async Task processPushMessage(string message)
    {
      try
      {
        JsonConvert.DeserializeObject(message);
        PushMessageInTouch pushMessageInTouch = JsonConvert.DeserializeObject<PushMessageInTouch>(message);
        string messageText = pushMessageInTouch.message_text;
        if (!string.IsNullOrEmpty(messageText))
        {
          XmlDocument templateContent = ToastNotificationManager.GetTemplateContent((ToastTemplateType) 4);
          ((IReadOnlyList<IXmlNode>) templateContent.GetElementsByTagName("text"))[0].AppendChild((IXmlNode) templateContent.CreateTextNode(messageText));
          if (!pushMessageInTouch.notify)
          {
            IXmlNode ixmlNode = templateContent.SelectSingleNode("/toast");
            XmlElement element = templateContent.CreateElement("audio");
            element.SetAttribute("silent", "true");
            ixmlNode.AppendChild((IXmlNode) element);
          }
          ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(templateContent));
        }
        switch (pushMessageInTouch.cmd)
        {
          case "update":
            try
            {
              if (Sync.isSyncRunning)
                break;
              LocalSettings.localSettings.syncSessionOwner = SyncStateOwner.PushMessage.ToString();
              bool shallDownload = true;
              await Sync.sync.startSync(shallDownload);
              break;
            }
            catch (Exception ex)
            {
              LogFile.Log("Error in update push message. " + ex.Message, EventType.Error);
              if (LocalSettings.localSettings.syncSessionState == 7)
                break;
              LocalSettings.localSettings.syncSessionState = -1;
              break;
            }
          case "upload_client_state":
            try
            {
              ServerConnectionManager SCMObj = new ServerConnectionManager();
              await SCMObj.uploadClientState(await LocalSettings.localSettings.getToken());
              break;
            }
            catch (Exception ex)
            {
              LogFile.Log("Error in upload_client_state push message. " + ex.Message, EventType.Error);
              break;
            }
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in processing push message. " + ex.Message, EventType.Error);
        Sync.setImageStatusBlock(string.Empty);
        Sync.setSyncStatusBlock("Unable to sync");
      }
    }
  }
}
