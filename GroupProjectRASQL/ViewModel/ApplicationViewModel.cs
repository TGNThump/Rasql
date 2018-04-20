using GroupProjectRASQL.Framework;
using GroupProjectRASQL.Parser;
using GroupProjectRASQL.Translator;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GroupProjectRASQL.ViewModel
{
    public class ApplicationViewModel : Reactive
    {
        public string input_sql { get; set; }

        public string output { get; private set; } = "";
        //public string output { get { return output; } private set { Set(ref output, value); }}

        public ISimpleCommand<String> Parse { get; private set; }

        public ApplicationViewModel()
        {
            Parser.Parser sqlParser = new Parser.Parser("sql");
            Parser.Parser raParser = new Parser.Parser("ra");

            Parse = new RelaySimpleCommand<String>(delegate(String type)
            {
                Parser.Parser parser;
                switch (type)
                {
                    case "sql":
                        parser = sqlParser;
                        break;
                    case "ra":
                        parser = raParser;
                        break;
                    default: return;
                }

                if (input_sql == null) return;
                output = "Parse: " + input_sql + "<br />";
                List<State>[] stateSets = parser.Parse(input_sql);
                bool valid = parser.IsValid(stateSets);
                output += "Valid: " + valid + "<br />";

                if (!valid) return;
                stateSets = parser.FilterAndReverse(stateSets);

                TreeNode<String> tree = parser.parse_tree(input_sql, stateSets);

                /*
                for (int i = 0; i < stateSets.Length; i++)
                {
                    output += "=== " + i + " ===" + "<br />";
                    foreach (State state in stateSets[i])
                    {
                        output += state.ToString() + "<br />";
                    }
                }*/

                if (type == "sql")
                {
                    output += tree.TreeToDebugString();
                    output += SqlToRa.TranslateQuery(tree);
                } else if (type == "ra")
                {
                    output += RAToOps.Translate(tree).TreeToDebugString();
                }
                });
        }
    }
}
