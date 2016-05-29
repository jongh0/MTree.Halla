using Microsoft.Azure.NotificationHubs;
using MTree.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class PushUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void NotifyMessage(string message, string tag = null)
        {
            try
            {
                if (Config.General.OfflineMode == true) return;
                if (Config.Push.UsePushNotification == false) return;

                var connectionString = Config.Push.ConnectionString;
                var hubPath = Config.Push.NotificationHubPath;

                if (string.IsNullOrEmpty(connectionString) == true ||
                    string.IsNullOrEmpty(hubPath) == true)
                    return;

                var HubClient = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubPath);

                string json = "{\"data\":{\"message\":\"" + $"[{Environment.MachineName}] {message}" + "\"}}";
                string jsonPayload = JObject.Parse(json).ToString();

                Task task = null;

                if (string.IsNullOrEmpty(tag) == true)
                {
                    task = HubClient.SendGcmNativeNotificationAsync(jsonPayload);
                    logger.Info($"Push message sent, {message}");
                }
                else
                {
                    task = HubClient.SendGcmNativeNotificationAsync(jsonPayload, tag);
                    logger.Info($"Push message sent, {message}/{tag}");
                }

                task?.Wait();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
