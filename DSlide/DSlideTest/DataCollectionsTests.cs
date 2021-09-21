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
            var notificationTracker = new ChangeNotificationReceiver();

            SimpleCollectionTest collectionTester = dataManager.CreateInstance<SimpleCollectionTest>();
            collectionTester.PropertyChanged += notificationTracker.NotificatonHandler;


            dataManager.EnterEditMode();
            collectionTester.Things = new DataSlideCollection<string>();
            dataManager.ExitEditMode();

            dataManager.EnterEditMode();
            collectionTester.Things.Add("Rock");
            collectionTester.Things.Add("Paper");
            collectionTester.Things.Add("Scissors");
            collectionTester.Things.Add("Screen");
            collectionTester.Things.Add("Hogfather");
            dataManager.ExitEditMode();
            Assert.IsTrue(collectionTester.SumLengths == 32);

            dataManager.EnterEditMode();
            collectionTester.Things.Add("Book");
            dataManager.ExitEditMode();
            Assert.IsTrue(collectionTester.SumLengths == 36);

            dataManager.EnterEditMode();
            collectionTester.Things.Add("Movie");
            dataManager.ExitEditMode();
            Assert.IsTrue(collectionTester.SumLengths == 41);

            dataManager.EnterEditMode();
            collectionTester.Things.Remove("Screen");
            dataManager.ExitEditMode();
            Assert.IsTrue(collectionTester.SumLengths == 35);

            dataManager.EnterEditMode();
            collectionTester.Filter = "e";
            collectionTester.UseFilter = true;
            dataManager.ExitEditMode();

            Assert.IsTrue(collectionTester.FilteredCollection == collectionTester.PivotedCollection);
            Assert.IsTrue(collectionTester.FilteredCollection.Count == 3);

            dataManager.EnterEditMode();
            collectionTester.Filter = "S";
            collectionTester.UseFilter = false;
            dataManager.ExitEditMode();

            Assert.IsTrue(collectionTester.Things == collectionTester.PivotedCollection);
            Assert.IsTrue(collectionTester.FilteredCollection.Count == 1);
        }
    }
}
