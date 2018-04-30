using GroupProjectRASQL.Framework;
using GroupProjectRASQL.Operations;
using GroupProjectRASQL.Parser;
using GroupProjectRASQL.Translator;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GroupProjectRASQL.Heuristics;
using GroupProjectRASQL.Schema;
using System.Linq;

namespace GroupProjectRASQL.ViewModel
{
    public class ApplicationViewModel : Reactive
    {
        public string CurrentView { get; set; } = "input";

        public IList<Relation> Relations { get; set; } = new List<Relation> {
            new Relation("animals", new List<Field>() {
                new Field("name", new List<String>{ "cat", "dog", "cow", "sheep", "pig" }),
                new Field("age", new List<String>{ "1", "2", "4", "2", "4" }),
                new Field("colour", new List<String>{ "black", "black", "brown", "white", "pink" }),
                new Field("weight", new List<String>{ "100", "200", "350", "1000", "10" }),
            }),
            new Relation("test", new List<Field>() {
                new Field("a", new List<String>{ "a" }),
                new Field("b", new List<String>{ "b" }),
                new Field("c", new List<String>{ "c" }),
                new Field("d", new List<String>{ "d" }),
                new Field("e", new List<String>{ "e" }),
                new Field("f", new List<String>{ "f" }),
                new Field("g", new List<String>{ "g" }),
                new Field("h", new List<String>{ "h" }),
            })
        };

        Parser.Parser sqlParser = new Parser.Parser("sql");
        Parser.Parser raParser = new Parser.Parser("ra");

        public Heuristic0 Heuristic0 { get; private set; }
        public Heuristic1 Heuristic1 { get; private set; }
        public Heuristic2 Heuristic2 { get; private set; }
        public Heuristic3 Heuristic3 { get; private set; }
        public Heuristic4 Heuristic4 { get; private set; }
        public Heuristic5 Heuristic5 { get; private set; }

        public string SQL { get; private set; }
        public string RA { get; private set; }

        public bool Input_Valid_SQL { get; private set; } = true;
        public bool Input_Valid_RA { get; private set; } = true;
        public string Input_Type { get; set; } = "sql";
        public string Input_SQL { get; private set; }
        public string Input_RA { get; private set; }

        public string Output { get; private set; } = "";
        public string Error { get; private set; }


        public ISimpleCommand<String> ParseSQL { get; private set; }
        public ISimpleCommand<String> ParseRA { get; private set; }
        public ISimpleCommand Step { get; private set; }
        public ISimpleCommand Complete { get; private set; }
        public ISimpleCommand Reset { get; private set; }

        public TreeNode<Operation> ops;

        public ApplicationViewModel()
        {
            ParseSQL = new RelaySimpleCommand<String>(sql =>
            {
                try
                {
                    Input_Valid_SQL = true;
                    if (sql == null) { Input_Valid_SQL = false; return; }

                    List<State>[] stateSets = sqlParser.Parse(sql);
                    
                    if (!sqlParser.IsValid(stateSets)) { Input_Valid_SQL = false; return; }

                    
                    stateSets = sqlParser.FilterAndReverse(stateSets);
                    TreeNode<String> tree = sqlParser.parse_tree(sql, stateSets);

                    this.SQL = sql;
                    this.Input_RA = SqlToRa.TranslateQuery(tree);
                    this.Input_Type = "ra";
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Error = "<div class='alert alert-danger'>" + e.ToString().Replace(Environment.NewLine, "<br/>") + "</div>";
                    return;
                }
            });

            ParseRA = new RelaySimpleCommand<String>(ra =>
            {
                try
                {
                    Input_Valid_RA = true;
                    if (ra == null) { Input_Valid_RA = false; return; }

                    List<State>[] stateSets = raParser.Parse(ra);

                    if (!raParser.IsValid(stateSets)) { Input_Valid_RA = false; return; }

                    stateSets = raParser.FilterAndReverse(stateSets);
                    TreeNode<String> tree = raParser.parse_tree(ra, stateSets);

                    Squish(tree);

                    this.ops = RAToOps.Translate(tree, Relations.ToDictionary(relation => relation.name));
                    ops = new TreeNode<Operation>(new Query()) { ops };

                    Heuristic0 = new Heuristic0(ops);
                    Heuristic1 = new Heuristic1(ops);
                    Heuristic2 = new Heuristic2(ops);
                    Heuristic3 = new Heuristic3(ops);
                    Heuristic4 = new Heuristic4(ops);
                    Heuristic5 = new Heuristic5(ops);

                    Heuristic0.Complete();

                    this.Output = "<div class='card'><div class='card-body'>";
                    this.Output += ops.TreeToDebugString();
                    this.Output += "</div></div>";
                    this.CurrentView = "output";
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Error = "<div class='alert alert-danger'>" + e.ToString().Replace(Environment.NewLine, "<br/>") + "</div>";
                    return;
                }
            });

            Step = new RelaySimpleCommand(() => 
            {
                if (!Heuristic1.IsComplete())
                {
                    Heuristic1.Step();
                    Output += "<div class='card'><div class='card-header'>Heuristic 1</div><div class='card-body'>";
                }
                else if (!Heuristic2.IsComplete())
                {
                    Heuristic2.Step();
                    Output += "<div class='card'><div class='card-header'>Heuristic 2</div><div class='card-body'>";
                }
                else if (!Heuristic3.IsComplete())
                {
                    Heuristic3.Step();
                    Output += "<div class='card'><div class='card-header'>Heuristic 3</div><div class='card-body'>";
                }
                else if (!Heuristic4.IsComplete())
                {
                    Heuristic4.Step();
                    Output += "<div class='card'><div class='card-header'>Heuristic 4</div><div class='card-body'>";
                }
                else if (!Heuristic5.IsComplete())
                {
                    Heuristic5.Step();
                    Output += "<div class='card'><div class='card-header'>Heuristic 5</div><div class='card-body'>";
                }
                else return;

                Output += ops.TreeToDebugString();
                Output += "</div></div>";
            });

            Complete = new RelaySimpleCommand(() =>
            {
                if (!Heuristic1.IsComplete())
                {
                    Heuristic1.Complete();
                    Output += "<div class='card'><div class='card-header'>Heuristic 1</div><div class='card-body'>";
                }
                else if (!Heuristic2.IsComplete())
                {
                    Heuristic2.Complete();
                    Output += "<div class='card'><div class='card-header'>Heuristic 2</div><div class='card-body'>";
                }
                else if (!Heuristic3.IsComplete())
                {
                    Heuristic3.Complete();
                    Output += "<div class='card'><div class='card-header'>Heuristic 3</div><div class='card-body'>";
                }
                else if (!Heuristic4.IsComplete())
                {
                    Heuristic4.Complete();
                    Output += "<div class='card'><div class='card-header'>Heuristic 4</div><div class='card-body'>";
                }
                else if (!Heuristic5.IsComplete())
                {
                    Heuristic5.Complete();
                    Output += "<div class='card'><div class='card-header'>Heuristic 5</div><div class='card-body'>";
                }
                else return;

                Output += ops.TreeToDebugString();
                Output += "</div></div>";
            });
        }

        bool Squish(TreeNode<String> root)
        {
            if (root.Data.Equals("[string]") || root.Data.Equals("[field]"))
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
