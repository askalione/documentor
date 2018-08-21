using Documentor.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Extensions
{
    public static class TempDataExtensions
    {
        private static readonly string _notificationsTempDataKey = "AspNetCore.Notifications";

        public static List<Notification> GetNotifications(this ITempDataDictionary tempData)
        {
            return Get<List<Notification>>(tempData, _notificationsTempDataKey) ?? new List<Notification>();
        }

        public static void SetNotifications(this ITempDataDictionary tempData, List<Notification> notifications)
        {
            tempData.Remove(_notificationsTempDataKey);
            Put(tempData, _notificationsTempDataKey, notifications ?? new List<Notification>());
        }

        private static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        private static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}
