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

        public string output { get; private set; } = "";
        //public string output { get { return output; } private set { Set(ref output, value); }}

        public ISimpleCommand Parse { get; private set; }

        public ApplicationViewModel()
        {
            Parser.Parser parser = new Parser.Parser();

            Parse = new RelaySimpleCommand(delegate()
            {
                output = "Parse: " + input_sql + "<br />";
                List<State>[] stateSets = parser.Parse(input_sql);
                bool valid = parser.IsValid(stateSets);
                output += "Valid: " + valid + "<br />";

                if (!valid) return;
                stateSets = parser.FilterAndReverse(stateSets);

                TreeNode<String> tree = parser.parse_tree(input_sql, stateSets);
                outputTree(tree);

                for (int i = 0; i < stateSets.Length; i++)
                {
                    output += "=== " + i + " ===" + "<br />";
                    foreach (State state in stateSets[i])
                    {
                        output += state.ToString() + "<br />";
                    }
                }
            });
        }

        public void outputTree(TreeNode<String> tree, int depth = 0)
        {
            for (int i = 0; i < depth; i++) output += "&nbsp;&nbsp;&nbsp;&nbsp;";
            output += tree.Data += "<br />";
            foreach (TreeNode<String> child in tree.Children) outputTree(child, depth + 1);
        }
    }
}
