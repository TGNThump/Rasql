using GroupProjectRASQL.Framework;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;
using System.Diagnostics;

namespace GroupProjectRASQL.ViewModel
{
    public class HelloViewModel : Reactive
    {
        public string message { get; set; } = "Test123";
        public int test { get; set; } = 3;

        public ISimpleCommand TestCommand { get; private set; }

        public HelloViewModel()
        {
            TestCommand = new RelaySimpleCommand(delegate()
            {
                test++;
                Debug.Print("command:" + test);
            });
        }
    }
}
