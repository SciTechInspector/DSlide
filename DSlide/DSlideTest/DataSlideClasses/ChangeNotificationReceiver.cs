using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSlideTest
{
    public class ChangeNotificationReceiver
    {
        public Dictionary<object, List<string>> receivedNotifications = new Dictionary<object, List<string>>();

        public void NotificatonHandler(object sender, PropertyChangedEventArgs args)
        {
            if (sender == null)
                sender = DBNull.Value;

            List<string> notifications;
            if (!receivedNotifications.TryGetValue(sender, out notifications))
            {
                notifications = new List<string>();
                receivedNotifications[sender] = notifications;
            }

            notifications.Add(args.PropertyName);
        }

        public int Count(object sender, string property)
        {
            List<string> notifications;
            if (!receivedNotifications.TryGetValue(sender, out notifications))
                return 0;

            return notifications.Count(x => x == property);
        }

        public int CountAndClear(object sender, string property)
        {
            List<string> notifications;
            if (!receivedNotifications.TryGetValue(sender, out notifications))
                return 0;

            var count = notifications.Count(x => x == property);

            while (notifications.Contains(property))
                notifications.Remove(property);

            if (notifications.Count == 0)
                receivedNotifications.Remove(sender);

            return count;
        }

        public int TotalCount()
        {
            return receivedNotifications.Values.Sum(x => x.Count);
        }

        public void Clear()
        {
            this.receivedNotifications.Clear();
        }
    }

}
