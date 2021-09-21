using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSlideTest
{
    public class CollectionChangeNotificationReceiver
    {
        public Dictionary<object, List<NotifyCollectionChangedEventArgs>> receivedNotifications = new Dictionary<object, List<NotifyCollectionChangedEventArgs>>();

        public void NotificatonHandler(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (sender == null)
                sender = DBNull.Value;

            List<NotifyCollectionChangedEventArgs> notifications;
            if (!receivedNotifications.TryGetValue(sender, out notifications))
            {
                notifications = new List<NotifyCollectionChangedEventArgs>();
                receivedNotifications[sender] = notifications;
            }

            notifications.Add(args);
        }

        public bool ConfirmAndClearFirstNotificationWithItems(object sender, List<object> items)
        {
            if (!this.receivedNotifications.ContainsKey(sender))
                return false;

            if (receivedNotifications[sender].Count < 2)
                return false;

            var firstInList = receivedNotifications[sender][0];
            var secondInList = receivedNotifications[sender][1];

            if (firstInList.Action != NotifyCollectionChangedAction.Reset)
                return false;

            if (!Enumerable.SequenceEqual(secondInList.NewItems.Cast<object>(), items))
                return false;

            receivedNotifications[sender].RemoveAt(0);
            receivedNotifications[sender].RemoveAt(0);

            if (receivedNotifications[sender].Count == 0)
                receivedNotifications.Remove(sender);

            return true;
        }

        public bool ConfirmNone(object sender)
        {
            return !this.receivedNotifications.ContainsKey(sender);
        }

        public bool ConfirmNone()
        {
            return this.receivedNotifications.Count == 0;
        }

        public void Clear()
        {
            this.receivedNotifications.Clear();
        }
    }

}
