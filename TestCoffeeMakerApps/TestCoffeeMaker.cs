using System;
using CoffeeMakerApps.Api;
using CoffeeMakerApps.M4CoffeeMaker;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCoffeeMakerApps {
    internal class CoffeeMakerStub : CoffeeMakerApi {
        public bool buttonPressed;
        public bool lightOn;
        public bool boilerOn;
        public bool valveClosed;
        public bool plateOn;
        public bool boilerEmpty;
        public bool potPresent;
        public bool potNotEmpty;

        public CoffeeMakerStub() {
            buttonPressed = false;
            lightOn = false;
            boilerOn = false;
            valveClosed = true;
            plateOn = false;
            boilerEmpty = true;
            potPresent = true;
            potNotEmpty = false;
        }

        public WarmerPlateStatus GetWarmerPlateStatus() {
            if (!potPresent) return WarmerPlateStatus.WARMER_EMPTY;
            else if (potNotEmpty) {
                return WarmerPlateStatus.POT_NOT_EMPTY;
            }
            else {
                return WarmerPlateStatus.POT_EMPTY;
            }
        }

        public BoilerStatus GetBoilerStatus() {
            return boilerEmpty ? BoilerStatus.EMPTY : BoilerStatus.NOT_EMPTY;
        }

        public BrewButtonStatus GetBrewButtonStatus() {
            if (buttonPressed) {
                buttonPressed = false;
                return BrewButtonStatus.PUSHED;
            }
            else {
                return BrewButtonStatus.NOT_PUSHED;
            }
        }

        public void SetBoilerState(BoilerState boilerState) {
            boilerOn = boilerState == BoilerState.ON;
        }

        public void SetWarmerState(WarmerState boilerState) {
            plateOn = boilerState == WarmerState.ON;
        }

        public void SetIndicatorState(IndicatorState indicatorState) {
            lightOn = indicatorState == IndicatorState.ON;
        }

        public void SetReliefValveState(ReliefValveState reliefValveState) {
            valveClosed = reliefValveState == ReliefValveState.CLOSED;
        }
    }

    [TestClass]
    public class TestCoffeeMaker {
        private M4UserInterface ui;
        private M4HotWaterSource hws;
        private M4ContainmentVessel cv;
        private CoffeeMakerStub api;
        [TestInitialize]
        public void SetUp()
        {
            api = new CoffeeMakerStub();
            ui = new M4UserInterface(api);
            hws = new M4HotWaterSource(api);
            cv = new M4ContainmentVessel(api);
            ui.Init(hws, cv);
            hws.Init(ui, cv);
            cv.Init(ui, hws);
        }
        private void Poll()
        {
            ui.Poll();
            hws.Poll();
            cv.Poll();
        }
        [TestMethod]
        public void InitialConditions()
        {
            Poll();
            Assert.IsFalse(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsFalse(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
        [TestMethod]
        public void StartNoPot()
        {
            Poll();
            api.buttonPressed = true;
            api.potPresent = false;
            Poll();
            Assert.IsFalse(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsFalse(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
        [TestMethod]
        public void StartNoWater()
        {
            Poll();
            api.buttonPressed = true;
            api.boilerEmpty = true;
            Poll();
            Assert.IsFalse(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsFalse(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
        [TestMethod]
        public void GoodStart()
        {
            NormalStart();
            Assert.IsTrue(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsFalse(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
        private void NormalStart()
        {
            Poll();
            api.boilerEmpty = false;
            api.buttonPressed = true;
            Poll();
        }
        [TestMethod]
        public void StartedPotNotEmpty()
        {
            NormalStart();
            api.potNotEmpty = true;
            Poll();
            Assert.IsTrue(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsTrue(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
        [TestMethod]
        public void PotRemovedAndReplacedWhileEmpty()
        {
            NormalStart();
            api.potPresent = false;
            Poll();
            Assert.IsFalse(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsFalse(api.plateOn);
            Assert.IsFalse(api.valveClosed);
            api.potPresent = true;
            Poll();
            Assert.IsTrue(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsFalse(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
        [TestMethod]
        public void PotRemovedWhileNotEmptyAndReplacedEmpty()
        {
            NormalFill();
            api.potPresent = false;
            Poll();
            Assert.IsFalse(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsFalse(api.plateOn);
            Assert.IsFalse(api.valveClosed);
            api.potPresent = true;
            api.potNotEmpty = false;
            Poll();
            Assert.IsTrue(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsFalse(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
        private void NormalFill()
        {
            NormalStart();
            api.potNotEmpty = true;
            Poll();
        }
        [TestMethod]
        public void PotRemovedWhileNotEmptyAndReplacedNotEmpty()
        {
            NormalFill();
            api.potPresent = false;
            Poll();
            api.potPresent = true;
            Poll();
            Assert.IsTrue(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsTrue(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
        [TestMethod]
        public void BoilerEmptyPotNotEmpty()
        {
            NormalBrew();
            Assert.IsFalse(api.boilerOn);
            Assert.IsTrue(api.lightOn);
            Assert.IsTrue(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
        private void NormalBrew()
        {
            NormalFill();
            api.boilerEmpty = true;
            Poll();
        }
        [TestMethod]
        public void BoilerEmptiesWhilePotRemoved()
        {
            NormalFill();
            api.potPresent = false;
            Poll();
            api.boilerEmpty = true;
            Poll();
            Assert.IsFalse(api.boilerOn);
            Assert.IsTrue(api.lightOn);
            Assert.IsFalse(api.plateOn);
            Assert.IsTrue(api.valveClosed);
            api.potPresent = true;
            Poll();
            Assert.IsFalse(api.boilerOn);
            Assert.IsTrue(api.lightOn);
            Assert.IsTrue(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
        [TestMethod]
        public void EmptyPotReturnedAfter()
        {
            NormalBrew();
            api.
            potNotEmpty = false;
            Poll();
            Assert.IsFalse(api.boilerOn);
            Assert.IsFalse(api.lightOn);
            Assert.IsFalse(api.plateOn);
            Assert.IsTrue(api.valveClosed);
        }
    }
}