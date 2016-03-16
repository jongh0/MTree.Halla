using Microsoft.Azure.NotificationHubs;
using MTree.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.PushService
{
    public class NotificationHub
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static object lockObject = new object();

        private static NotificationHub _instance;
        public static NotificationHub Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                            _instance = new NotificationHub();
                    }
                }

                return _instance;
            }
        }

        NotificationHubClient HubClient { get; set; }

        public NotificationHub()
        {
            HubClient = NotificationHubClient.CreateClientFromConnectionString(Config.Push.ConnectionString, Config.Push.NotificationHubPath);
            
        }

        public void Send(string message, string tag = null)
        {
            try
            {
                string json = "{\"data\":{\"message\":\"" + $"[{Environment.MachineName}] {message}" + "\"}}";
                string jsonPayload = JObject.Parse(json).ToString();

                Task task = null;

                if (string.IsNullOrEmpty(tag) == true)
                {
                    task = HubClient.SendGcmNativeNotificationAsync(jsonPayload);
                    logger.Info($"[{GetType().Name}] Push message sent, {message}");
                }
                else
                {
                    task = HubClient.SendGcmNativeNotificationAsync(jsonPayload, tag);
                    logger.Info($"[{GetType().Name}] Push message sent, {message}/{tag}");
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
