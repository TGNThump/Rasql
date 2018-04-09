using GroupProjectRASQL.Framework;
using GroupProjectRASQL.Parser;
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

        private string output = "";
        public string Output { get { return output; } private set { Set(ref output, value); }}

        public ISimpleCommand Parse { get; private set; }

        public ApplicationViewModel()
        {
            Parse = new RelaySimpleCommand(delegate()
            {
                Output = "Parse: " + input_sql + "<br />";
                Parser.Parser parser = new Parser.Parser();
                List<State>[] stateSets = parser.Parse(input_sql);
                bool valid = parser.IsValid(stateSets);
                Output += "Valid: " + valid + "<br />";

                if (!valid) return;
                stateSets = parser.FilterAndReverse(stateSets);

                TreeNode<String> tree = parser.parse_tree(input_sql, stateSets);
                outputTree(tree);

                for (int i = 0; i < stateSets.Length; i++)
                {
                    Output += "=== " + i + " ===" + "<br />";
                    foreach (State state in stateSets[i])
                    {
                        Output += state.ToString() + "<br />";
                    }
                }
            });
        }

        public void outputTree(TreeNode<String> tree, int depth = 0)
        {
            for (int i = 0; i < depth; i++) Output += "&nbsp;&nbsp;&nbsp;&nbsp;";
            Output += tree.Data += "<br />";
            foreach (TreeNode<String> child in tree.Children) outputTree(child, depth + 1);
        }
    }
}
