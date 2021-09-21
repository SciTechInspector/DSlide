using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DSlide;
using System.ComponentModel;
using System;
using System.Linq;
using System.Collections;

namespace DSlideTest
{

    [TestClass]
    public class DataCollectionsTests
    {
        [TestMethod]
        public void SimpleCollectionTests()
        {
            var dataManager = DataManager.Current;
            var propertyNotificationTracker = new PropertyChangeNotificationReceiver();
            var collectionNotificationTracker = new CollectionChangeNotificationReceiver();

            SimpleCollectionTest collectionTester = dataManager.CreateInstance<SimpleCollectionTest>();
            collectionTester.PropertyChanged += propertyNotificationTracker.NotificatonHandler;


            dataManager.EnterEditMode();
            collectionTester.Things = new DataSlideCollection<string>();
            dataManager.ExitEditMode();
            dataManager.SendChangeNotifications();

            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(collectionTester, nameof(collectionTester.Things)));
            Assert.IsTrue(collectionTester.Things.Count == 0);
            Assert.IsTrue(collectionTester.FilteredCollection.Count == 0);

            var originalThings = collectionTester.Things;
            var originalFiltered = collectionTester.FilteredCollection;

            originalThings.PropertyChanged += propertyNotificationTracker.NotificatonHandler;
            originalFiltered.PropertyChanged += propertyNotificationTracker.NotificatonHandler;

            originalThings.CollectionChanged += collectionNotificationTracker.NotificatonHandler;
            originalFiltered.CollectionChanged += collectionNotificationTracker.NotificatonHandler;


            dataManager.EnterEditMode();
            collectionTester.Things.Add("Rock");
            collectionTester.Things.Add("Paper");
            collectionTester.Things.Add("Scissors");
            collectionTester.Things.Add("Screen");
            collectionTester.Things.Add("Hogfather");
            dataManager.ExitEditMode();
            dataManager.SendChangeNotifications();

            Assert.IsTrue(collectionTester.SumLengths == 32);
            Assert.IsTrue(collectionTester.FilteredCollection == originalFiltered);
            Assert.IsTrue(collectionTester.Things == originalThings);

            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(originalThings, "Enumerator"));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(originalFiltered, "InternalList"));
            Assert.IsTrue(collectionNotificationTracker.ConfirmAndClearFirstNotificationWithItems(
                            originalThings,
                            new List<object> { "Rock", "Paper", "Scissors", "Screen", "Hogfather" }));

            Assert.IsTrue(collectionNotificationTracker.ConfirmAndClearFirstNotificationWithItems(
                            originalFiltered,
                            new List<object> { "Rock", "Paper", "Scissors", "Screen", "Hogfather" }));

            Assert.IsTrue(propertyNotificationTracker.TotalCount() == 0);
            Assert.IsTrue(collectionNotificationTracker.ConfirmNone());

            dataManager.EnterEditMode();
            collectionTester.Things.Add("Book");
            dataManager.ExitEditMode();
            dataManager.SendChangeNotifications();

            Assert.IsTrue(collectionTester.SumLengths == 36);
            Assert.IsTrue(collectionTester.FilteredCollection == originalFiltered);
            Assert.IsTrue(collectionTester.Things == originalThings);

            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(collectionTester, nameof(collectionTester.SumLengths)));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(originalThings, "Enumerator"));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(originalFiltered, "InternalList"));
            Assert.IsTrue(collectionNotificationTracker.ConfirmAndClearFirstNotificationWithItems(
                            originalThings,
                            new List<object> { "Rock", "Paper", "Scissors", "Screen", "Hogfather", "Book" }));

            Assert.IsTrue(collectionNotificationTracker.ConfirmAndClearFirstNotificationWithItems(
                            originalFiltered,
                            new List<object> { "Rock", "Paper", "Scissors", "Screen", "Hogfather", "Book" }));

            Assert.IsTrue(propertyNotificationTracker.TotalCount() == 0);
            Assert.IsTrue(collectionNotificationTracker.ConfirmNone());

            dataManager.EnterEditMode();
            collectionTester.Things.Add("Movie");
            dataManager.ExitEditMode();
            dataManager.SendChangeNotifications();

            Assert.IsTrue(collectionTester.SumLengths == 41);
            Assert.IsTrue(collectionTester.FilteredCollection == originalFiltered);
            Assert.IsTrue(collectionTester.Things == originalThings);

            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(collectionTester, nameof(collectionTester.SumLengths)));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(originalThings, "Enumerator"));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(originalFiltered, "InternalList"));
            Assert.IsTrue(collectionNotificationTracker.ConfirmAndClearFirstNotificationWithItems(
                            originalThings,
                            new List<object> { "Rock", "Paper", "Scissors", "Screen", "Hogfather", "Book", "Movie" }));

            Assert.IsTrue(collectionNotificationTracker.ConfirmAndClearFirstNotificationWithItems(
                            originalFiltered,
                            new List<object> { "Rock", "Paper", "Scissors", "Screen", "Hogfather", "Book", "Movie" }));

            Assert.IsTrue(propertyNotificationTracker.TotalCount() == 0);
            Assert.IsTrue(collectionNotificationTracker.ConfirmNone());


            dataManager.EnterEditMode();
            collectionTester.Things.Remove("Screen");
            dataManager.ExitEditMode();
            dataManager.SendChangeNotifications();

            Assert.IsTrue(collectionTester.SumLengths == 35);
            Assert.IsTrue(collectionTester.FilteredCollection == originalFiltered);
            Assert.IsTrue(collectionTester.Things == originalThings);

            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(collectionTester, nameof(collectionTester.SumLengths)));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(originalThings, "Enumerator"));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(originalFiltered, "InternalList"));
            Assert.IsTrue(collectionNotificationTracker.ConfirmAndClearFirstNotificationWithItems(
                            originalThings,
                            new List<object> { "Rock", "Paper", "Scissors", "Hogfather", "Book", "Movie" }));

            Assert.IsTrue(collectionNotificationTracker.ConfirmAndClearFirstNotificationWithItems(
                            originalFiltered,
                            new List<object> { "Rock", "Paper", "Scissors", "Hogfather", "Book", "Movie" }));

            Assert.IsTrue(propertyNotificationTracker.TotalCount() == 0);
            Assert.IsTrue(collectionNotificationTracker.ConfirmNone());

            dataManager.EnterEditMode();
            collectionTester.Filter = "e";
            collectionTester.UseFilter = true;
            dataManager.ExitEditMode();

            dataManager.SendChangeNotifications();

            Assert.IsTrue(collectionTester.FilteredCollection == originalFiltered);
            Assert.IsTrue(collectionTester.Things == originalThings);
            Assert.IsTrue(collectionTester.PivotedCollection == originalFiltered);

            Assert.IsTrue(collectionTester.FilteredCollection == collectionTester.PivotedCollection);
            Assert.IsTrue(collectionTester.FilteredCollection.Count == 3);

            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(originalFiltered, "InternalList"));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(collectionTester, nameof(collectionTester.Filter)));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(collectionTester, nameof(collectionTester.UseFilter)));

            Assert.IsTrue(collectionNotificationTracker.ConfirmAndClearFirstNotificationWithItems(
                            originalFiltered,
                            new List<object> { "Paper", "Hogfather", "Movie" }));

            Assert.IsTrue(propertyNotificationTracker.TotalCount() == 0);
            Assert.IsTrue(collectionNotificationTracker.ConfirmNone());

            dataManager.EnterEditMode();
            collectionTester.Filter = "S";
            collectionTester.UseFilter = true;
            dataManager.ExitEditMode();
            dataManager.SendChangeNotifications();

            Assert.IsTrue(collectionTester.FilteredCollection == originalFiltered);
            Assert.IsTrue(collectionTester.Things == originalThings);

            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(collectionTester, nameof(collectionTester.Filter)));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(originalFiltered, "InternalList"));
            Assert.IsTrue(collectionNotificationTracker.ConfirmAndClearFirstNotificationWithItems(
                            originalFiltered,
                            new List<object> { "Scissors" }));

            Assert.IsTrue(propertyNotificationTracker.TotalCount() == 0);
            Assert.IsTrue(collectionNotificationTracker.ConfirmNone());
            
            dataManager.EnterEditMode();
            collectionTester.Filter = "S";
            collectionTester.UseFilter = false;
            dataManager.ExitEditMode();
            dataManager.SendChangeNotifications();

            Assert.IsTrue(collectionTester.FilteredCollection == originalFiltered);
            Assert.IsTrue(collectionTester.Things == originalThings);
            Assert.IsTrue(collectionTester.Things == collectionTester.PivotedCollection);
            Assert.IsTrue(collectionTester.FilteredCollection.Count == 1);

            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(collectionTester, nameof(collectionTester.UseFilter)));
            Assert.IsTrue(1 == propertyNotificationTracker.CountAndClear(collectionTester, nameof(collectionTester.PivotedCollection)));

            Assert.IsTrue(propertyNotificationTracker.TotalCount() == 0);
            Assert.IsTrue(collectionNotificationTracker.ConfirmNone());

        }
    }
}
