using CoffeeMakerApps.Api;

namespace CoffeeMakerApps.M4CoffeeMaker {
    public class M4UserInterface: UserInterface, Pollable {
        private CoffeeMakerApi api;
        public M4UserInterface(CoffeeMakerApi api) {
            this.api = api;
        }

        public void Poll() {
            BrewButtonStatus buttonStatus = api.GetBrewButtonStatus();
            if(buttonStatus==BrewButtonStatus.PUSHED) StartBrewing();
        }
        public override void Done() {
            api.SetIndicatorState(IndicatorState.ON);
        }

        public override void ComleteCycle() {
            api.SetIndicatorState(IndicatorState.OFF);
        }



    }
}