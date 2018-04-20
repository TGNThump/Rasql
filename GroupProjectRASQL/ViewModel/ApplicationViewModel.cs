using GroupProjectRASQL.Framework;
using GroupProjectRASQL.Operations;
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
        public string input { get; set; }

        public string output { get; private set; } = "";
        //public string output { get { return output; } private set { Set(ref output, value); }}

        public ISimpleCommand<String> Parse { get; private set; }

        public ApplicationViewModel()
        {
            Parser.Parser sqlParser = new Parser.Parser("sql");
            Parser.Parser raParser = new Parser.Parser("ra");

            Parse = new RelaySimpleCommand<String>(delegate (String type)
            {
                if (input == null) return;
                output = "";

                String sql = input;
                String ra = input;
                List<State>[] stateSets;
                TreeNode<String> tree;
                bool valid;

                if (type.Equals("sql"))
                {
                    output += @"<div class='card'><div class='card-header'>SQL: " + sql + "</div></div>";

                    stateSets = sqlParser.Parse(sql);

                    valid = sqlParser.IsValid(stateSets);
                    output += @"<div class='card'><div class='card-body'>Valid: " + valid + "</div></div>";
                    if (!valid) return;

                    stateSets = sqlParser.FilterAndReverse(stateSets);
                    tree = sqlParser.parse_tree(sql, stateSets);
                    Squish(tree);
                    output += "<div class='card'><div class='card-body'>";
                    output += tree.TreeToDebugString();
                    output += "</div></div>";
                    ra = SqlToRa.TranslateQuery(tree);
                }

                output += @"<div class='card'><div class='card-header'>RA: " + ra + "</div></div>";
                stateSets = raParser.Parse(ra);

                valid = raParser.IsValid(stateSets);
                output += @"<div class='card'><div class='card-body'>Valid: " + valid + "</div></div>";
                if (!valid) return;

                stateSets = raParser.FilterAndReverse(stateSets);
                tree = raParser.parse_tree(ra, stateSets);
                //output += "<div class='card'><div class='card-body'>";
                //output += tree.TreeToDebugString();
                //output += "</div></div>";

                Squish(tree);
                TreeNode<Operation> ops = RAToOps.Translate(tree);

                output += "<div class='card'><div class='card-body'>";
                output += ops.TreeToDebugString();
                output += "</div></div>";

                return;
            });
        }

        bool Squish(TreeNode<String> root)
        {
            if (root.Data.Equals("[string]"))
            {
                String value = root.TreeToString();
                root.RemoveChildren();
                root.AddChild(new TreeNode<String>(value));
            }

            if (root.Data.Equals(" ") && root.Children.Count == 0)
            {
                root.Parent.RemoveChild(root);
                return true;
            }

            for (int i = 0; i < root.Children.Count; i++)
            {
                TreeNode<String> child = root.Child(i);
                if (Squish(child)) i--;
            }
            return false;
        }
    }
}
