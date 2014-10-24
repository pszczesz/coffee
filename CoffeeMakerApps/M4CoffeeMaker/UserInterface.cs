namespace CoffeeMakerApps.M4CoffeeMaker {
    public abstract class UserInterface {
        private HotWaterSource hws;
        private ContainmentVessel cv;
        protected bool isComplete;
        protected UserInterface( bool isComplete) {
            this.isComplete = isComplete;
        }

        public UserInterface() {
            isComplete = true;
        }
        public void Init(HotWaterSource hws, ContainmentVessel cv)
        {
            this.hws = hws;
            this.cv = cv;
        }
        public abstract void Done();

        public abstract void ComleteCycle();

        public void Complete() {
            isComplete = true;
            ComleteCycle();
        }


        protected void StartBrewing() {
            if (hws.IsReady() && cv.IsReady()) {
                isComplete = false;
                hws.Start();
                cv.Start();
            }
        }
    }
}