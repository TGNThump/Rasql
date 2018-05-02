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

        public List<Heuristic> Heurisitcs { get; private set; } = new List<Heuristic>();

        public Heuristic CurrentHeuristic
        {
            get
            {
                foreach (Heuristic heuristic in Heurisitcs)
                {
                    ; if (heuristic.isEnabled && !heuristic.isComplete) return heuristic;
                }
                return null;
            }
        }

        private string Parsed_SQL = "";
        private string Parsed_RA_From_SQL = "";

        public string SQL { get; private set; } = "";
        public string RA { get; private set; } = "";

        public bool Input_Valid_SQL { get; private set; } = true;
        public bool Input_Valid_RA { get; private set; } = true;

        public string Input_Type { get; set; } = "sql";

        private string input_sql;
        public string Input_SQL
        {
            get
            {
                return input_sql;
            }
            set
            {
                this.Input_Valid_SQL = true;
                this.input_sql = value;
            }
        }

        private string input_ra;
        public string Input_RA
        {
            get
            {
                return input_ra;
            }
            set
            {
                this.Input_Valid_RA = true;
                this.input_ra = value;
            }
        }

        public string Output { get; private set; } = "";
        public string Error { get; set; } = "";


        public ISimpleCommand ParseSQL { get; private set; }
        public ISimpleCommand ParseRA { get; private set; }
        public ISimpleCommand Step { get; private set; }
        public ISimpleCommand Complete { get; private set; }
        public ISimpleCommand Auto { get; private set; }
        public ISimpleCommand Reset { get; private set; }

        private TreeNode<Operation> ops;
        public String OpsJSON { get; private set; }

        public ApplicationViewModel()
        {
            ParseSQL = new RelaySimpleCommand(() =>
            {
                try
                {
                    if (Input_SQL == null)
                    {
                        Input_Valid_SQL = false; return;
                    }
                    String sql = Input_SQL.Replace("\n", " ");
                    while (sql.Last() == ' ') sql = sql.Substring(0, sql.Length - 1);
                    Input_Valid_SQL = true;
                    if (sql == "") { Input_Valid_SQL = false; return; }

                    List<State>[] stateSets = sqlParser.Parse(sql);

                    if (!sqlParser.IsValid(stateSets)) { Input_Valid_SQL = false; return; }

                    stateSets = sqlParser.FilterAndReverse(stateSets);
                    TreeNode<String> tree = sqlParser.parse_tree(sql, stateSets);

                    this.Input_RA = SqlToRa.TranslateQuery(tree);
                    this.Parsed_SQL = sql;
                    this.Parsed_RA_From_SQL = this.Input_RA;
                    this.Input_Type = "ra";
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Error = e.Message.Replace(Environment.NewLine, "<br/>");
                    return;
                }
            });

            ParseRA = new RelaySimpleCommand(() =>
            {
                try
                {
                    if (Input_RA == null)
                    {
                        Input_Valid_RA = false; return;
                    }
                    String ra = Input_RA.Replace("\n", " ");
                    while (ra.Last() == ' ') ra = ra.Substring(0, ra.Length - 1);
                    Input_Valid_RA = true;
                    if (Input_RA == "") { Input_Valid_RA = false; return; }

                    List<State>[] stateSets = raParser.Parse(Input_RA);

                    if (!raParser.IsValid(stateSets)) { Input_Valid_RA = false; return; }

                    if (this.Input_RA == this.Parsed_RA_From_SQL) this.SQL = this.Parsed_SQL;
                    else this.SQL = "";

                    this.RA = Input_RA;

                    stateSets = raParser.FilterAndReverse(stateSets);
                    TreeNode<String> tree = raParser.parse_tree(Input_RA, stateSets);

                    Squish(tree);

                    this.ops = RAToOps.Translate(tree, Relations.ToDictionary(relation => relation.name));
                    ops = new TreeNode<Operation>(new Query()) { ops };

                    new Heuristic0(ops).Complete();

                    Heurisitcs.Add(new Heuristic1(ops));
                    Heurisitcs.Add(new Heuristic2(ops));
                    Heurisitcs.Add(new Heuristic3(ops));
                    Heurisitcs.Add(new Heuristic4(ops));
                    Heurisitcs.Add(new Heuristic5(ops));

                    this.OpsJSON = ops.Child().ToJSON().Replace("\"", "&quot;").Replace("'", "\"");
                    this.CurrentView = "output";
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Error = e.Message.Replace(Environment.NewLine, "<br/>");
                    return;
                }
            });

            Step = new RelaySimpleCommand(() =>
            {
                try
                {
                    if (CurrentHeuristic != null) CurrentHeuristic.Step();
                    this.OpsJSON = ops.Child().ToJSON().Replace("\"", "&quot;").Replace("'", "\"");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Error = e.Message.Replace(Environment.NewLine, "<br/>");
                    return;
                }
            });

            Complete = new RelaySimpleCommand(() =>
            {
                try
                {
                    if (CurrentHeuristic != null) CurrentHeuristic.Complete();
                    this.OpsJSON = ops.Child().ToJSON().Replace("\"", "&quot;").Replace("'", "\"");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Error = e.Message.Replace(Environment.NewLine, "<br/>");
                    return;
                }
            });

            Reset = new RelaySimpleCommand(() =>
            {
                try
                {
                    List<State>[] stateSets = raParser.Parse(RA);
                    stateSets = raParser.FilterAndReverse(stateSets);
                    TreeNode<String> tree = raParser.parse_tree(RA, stateSets);

                    Squish(tree);

                    this.ops = RAToOps.Translate(tree, Relations.ToDictionary(relation => relation.name));
                    ops = new TreeNode<Operation>(new Query()) { ops };

                    new Heuristic0(ops).Complete();

                    Heurisitcs.Add(new Heuristic1(ops));
                    Heurisitcs.Add(new Heuristic2(ops));
                    Heurisitcs.Add(new Heuristic3(ops));
                    Heurisitcs.Add(new Heuristic4(ops));
                    Heurisitcs.Add(new Heuristic5(ops));

                    this.OpsJSON = ops.Child().ToJSON().Replace("\"", "&quot;").Replace("'", "\"");
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Error = e.Message.Replace(Environment.NewLine, "<br/>");
                    return;
                }
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
