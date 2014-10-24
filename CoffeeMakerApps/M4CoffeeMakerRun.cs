using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoffeeMakerApps.Api;
using CoffeeMakerApps.M4CoffeeMaker;

namespace CoffeeMakerApps
{
    class M4CoffeeMakerRun
    {
        static void Main(string[] args) {
            CoffeeMakerApi api = new M4CoffeeMakerApi();
            M4UserInterface ui = new M4UserInterface(api);
            M4HotWaterSource hws = new M4HotWaterSource(api);
            M4ContainmentVessel cv = new M4ContainmentVessel(api);
            ui.Init(hws,cv);
            hws.Init(ui,cv);
            cv.Init(ui,hws);
            bool isGo=true;
          
            while (isGo) {
                ui.Poll();
                hws.Poll();
                cv.Poll();
            }
            Console.ReadKey();
        }

    }
}
