using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DSlide;
using System.ComponentModel;
using System;
using System.Linq;

namespace DSlideTest
{
    [TestClass]
    public class UnitTest1
    {
        private class ChangeNotificationReceiver
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

                while(notifications.Contains(property))
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

        [TestMethod]
        public void SimplePropertyTests()
        {
            var dataManager = DataManager.Current;
            var notificationTracker = new ChangeNotificationReceiver();

            DiamondTest obj = dataManager.CreateInstance<DiamondTest>();
            obj.PropertyChanged += notificationTracker.NotificatonHandler;

            Assert.IsTrue(obj.FirstName == null);
            Assert.IsTrue(obj.LastName == null);

            dataManager.EnterEditMode();
            obj.FirstName = "Bob";
            obj.LastName = "Morane";

            Assert.IsTrue(obj.FirstName == null);
            Assert.IsTrue(obj.LastName == null);
            dataManager.ExitEditMode();
            dataManager.SendChangeNotifications();

            Assert.IsTrue(notificationTracker.CountAndClear(obj, "FirstName") == 1);
            Assert.IsTrue(notificationTracker.CountAndClear(obj, "LastName") == 1);
            Assert.IsTrue(notificationTracker.TotalCount() == 0);

            Assert.IsTrue(obj.FirstName == "Bob");
            Assert.IsTrue(obj.LastName == "Morane");

            Assert.IsTrue(obj.FullName == "Bob Morane");

            dataManager.EnterEditMode();
            obj.FirstName = "Bill";
            obj.LastName = "Balantine";

            Assert.IsTrue(obj.FirstName == "Bob");
            Assert.IsTrue(obj.LastName == "Morane");

            Assert.IsTrue(obj.FullName == "Bob Morane");

            dataManager.ExitEditMode();
            dataManager.SendChangeNotifications();

            Assert.IsTrue(notificationTracker.CountAndClear(obj, "FirstName") == 1);
            Assert.IsTrue(notificationTracker.CountAndClear(obj, "LastName") == 1);
            Assert.IsTrue(notificationTracker.CountAndClear(obj, "FullName") == 1);
            Assert.IsTrue(notificationTracker.TotalCount() == 0);


            Assert.IsTrue(obj.FirstName == "Bill");
            Assert.IsTrue(obj.LastName == "Balantine");

            Assert.IsTrue(obj.FullName == "Bill Balantine");

            dataManager.EnterEditMode();
            obj.FirstName = "Bob";

            dataManager.ExitEditMode();
            dataManager.SendChangeNotifications();


            Assert.IsTrue(obj.FirstName == "Bob");
            Assert.IsTrue(obj.LastName == "Balantine");

            Assert.IsTrue(obj.FullName == "Bob Balantine");

            Assert.IsTrue(notificationTracker.CountAndClear(obj, "FirstName") == 1);
            Assert.IsTrue(notificationTracker.CountAndClear(obj, "FullName") == 1);
            Assert.IsTrue(notificationTracker.TotalCount() == 0);
        }

        [TestMethod]
        public void TestFindNearestLessThanInSortedList()
        {
            var sorted = new SortedList<int, int>();
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(1) == -1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(2) == -1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(3) == -1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(5) == -1);

            sorted.Add(2, 2);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(1) == -1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(2) == 0);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(3) == 0);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(5) == 0);

            sorted.Add(5, 5);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(1) == -1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(2) == 0);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(3) == 0);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(5) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(6) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(7) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(8) == 1);

            sorted.Add(8, 8);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(1) == -1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(2) == 0);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(3) == 0);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(5) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(6) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(7) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(8) == 2);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(11) == 2);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(14) == 2);

            sorted.Add(14, 14);
            sorted.Add(82, 82);
            sorted.Add(192, 192);

            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(1) == -1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(2) == 0);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(3) == 0);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(5) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(6) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(7) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(8) == 2);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(11) == 2);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(14) == 3);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(15) == 3);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(82) == 4);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(83) == 4);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(191) == 4);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(192) == 5);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(193) == 5);
        }
    }
}
