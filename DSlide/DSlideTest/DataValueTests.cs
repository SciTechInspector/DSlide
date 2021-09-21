using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DSlide;
using System.ComponentModel;
using System;
using System.Linq;

namespace DSlideTest
{
    [TestClass]
    public class DataValuesTests
    {

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
        public void PivotPropertyTests()
        {
            var dataManager = DataManager.Current;
            var notificationTracker = new ChangeNotificationReceiver();

            PivotDataTest obj = dataManager.CreateInstance<PivotDataTest>();
            // obj.PropertyChanged += notificationTracker.NotificatonHandler;

            dataManager.EnterEditMode();
            obj.Data1 = "FirstData";
            obj.Data2 = "SecondData";
            obj.PivotTo2 = false;
            obj.PivotToDeep = false;
            dataManager.ExitEditMode();

            Assert.IsTrue(obj.Data1 == "FirstData");
            Assert.IsTrue(obj.Data2 == "SecondData");
            Assert.IsTrue(obj.PivotTo2 == false);
            Assert.IsTrue(obj.PivotToDeep == false);
            Assert.IsTrue(obj.SimplePivot == "FirstData");
            Assert.IsTrue(obj.TrickyPivot == "FirstDatanow");
            Assert.IsTrue(obj.ComputeDeep5 == "FirstData1s2s3s4s5s");

            dataManager.EnterEditMode();
            obj.PivotTo2 = true;
            obj.PivotToDeep = true;
            dataManager.ExitEditMode();

            Assert.IsTrue(obj.SimplePivot == "SecondData");
            Assert.IsTrue(obj.TrickyPivot == "FirstData1d2d3d4d5d");
        }

        private void getMajorPivots(DataManager dataManager, PivotDataTest obj, 
                    out ReactTimeInstance1 majorPivotTT, 
                    out ReactTimeInstance1 majorPivotFT,
                    out ReactTimeInstance2 majorPivotTF,
                    out ReactTimeInstance2 majorPivotFF)
        {
            dataManager.EnterEditMode();
            obj.PivotToDeep = true;
            obj.PivotTo2 = true;
            dataManager.ExitEditMode();
            majorPivotTT = obj.MajorPivot as ReactTimeInstance1;

            dataManager.EnterEditMode();
            obj.PivotToDeep = false;
            obj.PivotTo2 = true;
            dataManager.ExitEditMode();
            majorPivotFT = obj.MajorPivot as ReactTimeInstance1;

            dataManager.EnterEditMode();
            obj.PivotToDeep = true;
            obj.PivotTo2 = false;
            dataManager.ExitEditMode();
            majorPivotTF = obj.MajorPivot as ReactTimeInstance2;

            dataManager.EnterEditMode();
            obj.PivotToDeep = false;
            obj.PivotTo2 = false;
            dataManager.ExitEditMode();
            majorPivotFF = obj.MajorPivot as ReactTimeInstance2;
        }

        [TestMethod]
        public void TestPersistedReactiveObjects()
        {
            var dataManager = DataManager.Current;
            var notificationTracker = new ChangeNotificationReceiver();

            PivotDataTest obj = dataManager.CreateInstance<PivotDataTest>();
            DiamondTest diamondA = dataManager.CreateInstance<DiamondTest>();
            DiamondTest diamondB = dataManager.CreateInstance<DiamondTest>();
            DiamondTest diamondC = dataManager.CreateInstance<DiamondTest>();
            DiamondTest diamondD = dataManager.CreateInstance<DiamondTest>();

            dataManager.EnterEditMode();

            diamondA.FirstName = "Emerald";
            diamondA.LastName = "Green";

            diamondB.FirstName = "Onyx";
            diamondB.LastName = "Black";

            diamondC.FirstName = "Ruby";
            diamondC.LastName = "Red";

            diamondD.FirstName = "Saphire";
            diamondD.LastName = "Blue";

            obj.Data1 = "FirstData";
            obj.Data2 = "SecondData";
            obj.PivotTo2 = false;
            obj.PivotToDeep = false;

            obj.DiamondTest1 = diamondA;
            obj.DiamondTest2 = diamondB;
            dataManager.ExitEditMode();

            ReactTimeInstance1 majorPivotTT1, majorPivotFT1;
            ReactTimeInstance2 majorPivotTF1, majorPivotFF1;
            getMajorPivots(dataManager, obj, out majorPivotTT1, out majorPivotFT1, out majorPivotTF1, out majorPivotFF1);
            Assert.IsTrue(new HashSet<DataSlideBase> { majorPivotTT1, majorPivotTF1, majorPivotFT1, majorPivotFF1 }.Count == 4);
            Assert.IsTrue(majorPivotTT1.ResultingDataSource == "FirstData");
            Assert.IsTrue(majorPivotFT1.ResultingDataSource == "SecondData");
            Assert.IsTrue(majorPivotTF1.LegalDirectReference == "Emerald");
            Assert.IsTrue(majorPivotFF1.LegalDirectReference == "Onyx");

            dataManager.EnterEditMode();
            obj.Data2 = "FirstData";
            obj.Data1 = "SecondData";
            dataManager.ExitEditMode();

            ReactTimeInstance1 majorPivotTT2, majorPivotFT2;
            ReactTimeInstance2 majorPivotTF2, majorPivotFF2;
            getMajorPivots(dataManager, obj, out majorPivotTT2, out majorPivotFT2, out majorPivotTF2, out majorPivotFF2);
            Assert.IsTrue(new HashSet<DataSlideBase> 
                        {
                            majorPivotTT1, majorPivotTF1, majorPivotFT1, majorPivotFF1,
                            majorPivotTT2, majorPivotFT2, majorPivotTF2, majorPivotFF2 
                        }.Count == 4);
            Assert.IsTrue(majorPivotTT1 == majorPivotFT2 && majorPivotFT1 == majorPivotTT2);
            Assert.IsTrue(majorPivotTT1 == majorPivotFT2 && majorPivotFT1 == majorPivotTT2);

            dataManager.EnterEditMode();
            obj.DiamondTest1 = diamondB;
            obj.DiamondTest2 = diamondC;
            dataManager.ExitEditMode();

            ReactTimeInstance1 majorPivotTT3, majorPivotFT3;
            ReactTimeInstance2 majorPivotTF3, majorPivotFF3;
            getMajorPivots(dataManager, obj, out majorPivotTT3, out majorPivotFT3, out majorPivotTF3, out majorPivotFF3);
            Assert.IsTrue(new HashSet<DataSlideBase>
                        {
                            majorPivotTT1, majorPivotTF1, majorPivotFT1, majorPivotFF1,
                            majorPivotTT2, majorPivotFT2, majorPivotTF2, majorPivotFF2,
                            majorPivotTT3, majorPivotFT3, majorPivotTF3, majorPivotFF3
                        }.Count == 5);
            Assert.IsTrue(majorPivotFF2 == majorPivotTF3);
            Assert.IsTrue(majorPivotFF3.LegalDirectReference == "Ruby");


            dataManager.EnterEditMode();
            obj.Data1 = "NewDataOne";
            obj.Data2 = "NewDataTwo";
            dataManager.ExitEditMode();

            ReactTimeInstance1 majorPivotTT4, majorPivotFT4;
            ReactTimeInstance2 majorPivotTF4, majorPivotFF4;
            getMajorPivots(dataManager, obj, out majorPivotTT4, out majorPivotFT4, out majorPivotTF4, out majorPivotFF4);
            Assert.IsTrue(new HashSet<DataSlideBase>
                        {
                            majorPivotTT1, majorPivotTF1, majorPivotFT1, majorPivotFF1,
                            majorPivotTT2, majorPivotFT2, majorPivotTF2, majorPivotFF2,
                            majorPivotTT3, majorPivotFT3, majorPivotTF3, majorPivotFF3,
                            majorPivotTT4, majorPivotFT4, majorPivotTF4, majorPivotFF4
                        }.Count == 7);
            Assert.IsTrue(majorPivotTT4.ResultingDataSource == "NewDataOne");
            Assert.IsTrue(majorPivotFT4.ResultingDataSource == "NewDataTwo");
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
