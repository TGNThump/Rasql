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
        public string output { get; private set; } = "";
        public ISimpleCommand Parse { get; private set; }

        public ApplicationViewModel()
        {
            Parse = new RelaySimpleCommand(delegate()
            {
                output = "Parse: " + input_sql + "<br />";
                Parser.Parser parser = new Parser.Parser();
                List<State>[] stateSets = parser.FilterAndReverse(parser.Parse(input_sql));

                for (int i=0; i<stateSets.Length; i++)
                {
                    output += "=== " + i + " ===" + "<br />";
                    foreach (State state in stateSets[i])
                    {
                        if (!state.isFinished()) continue;
                        output += state.ToString() + "<br />";
                    }
                }

                output += "Valid: " + parser.IsValid(stateSets) + "<br />";
            });
        }
    }
}
