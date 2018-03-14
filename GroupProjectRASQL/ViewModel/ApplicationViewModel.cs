using GroupProjectRASQL.Framework;
using GroupProjectRASQL.Parser;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;
using System.Collections.Generic;
using System.Diagnostics;

namespace GroupProjectRASQL.ViewModel
{
    public class ApplicationViewModel : Reactive
    {
        public string input_sql { get; set; }
        public ISimpleCommand Parse { get; private set; }

        public ApplicationViewModel()
        {
            Parse = new RelaySimpleCommand(delegate()
            {
                Debug.Print("Parse: " + input_sql);
                Parser.Parser parser = new Parser.Parser();
                List<State>[] stateSets = parser.Parse(input_sql);

                for (int i=0; i<stateSets.Length; i++)
                {
                    Debug.Print("=== " + i + " ===");
                    foreach(State state in stateSets[i])
                    {
                        Debug.Print(state.ToString());
                    }
                }

                Debug.Print("Valid: " + parser.IsValid(stateSets));
            });
        }
    }
}
