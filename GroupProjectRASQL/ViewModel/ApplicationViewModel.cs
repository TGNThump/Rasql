﻿using GroupProjectRASQL.Framework;
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
        public string input { get; set; }

        public string output { get; private set; } = "";
        //public string output { get { return output; } private set { Set(ref output, value); }}

        public ISimpleCommand<String> Parse { get; private set; }

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


        public ApplicationViewModel()
        {
            Parser.Parser sqlParser = new Parser.Parser("sql");
            Parser.Parser raParser = new Parser.Parser("ra");

            Parse = new RelaySimpleCommand<String>(delegate (String type)
            {
                try
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
                        output += @"<div class='card'><div class='card-header'>SQL: " + sql + "</div>";

                        stateSets = sqlParser.Parse(sql);

                        valid = sqlParser.IsValid(stateSets);
                        output += @"<div class='card-body'>Valid: " + valid + "</div></div>";
                        if (!valid) return;

                        stateSets = sqlParser.FilterAndReverse(stateSets);
                        tree = sqlParser.parse_tree(sql, stateSets);
                        //DON'T SQUISH TREE BEFORE TRANSLATION. TRANSLATION ASSUMES TREE CORRESPONDS TO THE RA GRAMMAR. Squish(tree);
                        //output += "<div class='card'><div class='card-body'>";
                        //output += tree.TreeToDebugString();
                        //output += "</div></div>";
                        ra = SqlToRa.TranslateQuery(tree);
                    }

                    output += @"<div class='card'><div class='card-header'>RA: " + ra + "</div>";
                    stateSets = raParser.Parse(ra);

                    valid = raParser.IsValid(stateSets);
                    output += @"<div class='card-body'>Valid: " + valid + "</div></div>";
                    if (!valid) return;

                    stateSets = raParser.FilterAndReverse(stateSets);
                    tree = raParser.parse_tree(ra, stateSets);

                    Squish(tree);

                    /*
                    output += "<div class='card'><div class='card-body'>";
                    output += tree.TreeToDebugString();
                    output += "</div></div>";
                    */

                    TreeNode<Operation> ops = RAToOps.Translate(tree, Relations.ToDictionary(relation => relation.name));

                    output += "<div class='card'><div class='card-body'>";
                    output += ops.TreeToDebugString();
                    output += "</div></div>";

                    Heuristics.Heuristics.Heuristic1(ops);

                    output += "<div class='card'><div class='card-header'>Heuristic 1</div><div class='card-body'>";
                    output += ops.TreeToDebugString();
                    output += "</div></div>";

                    Heuristics.Heuristics.Heuristic2(ops);

                    output += "<div class='card'><div class='card-header'>Heuristic 2</div><div class='card-body'>";
                    output += ops.TreeToDebugString();
                    output += "</div></div>";

                    Heuristics.Heuristics.Heuristic3(ops);

                    output += "<div class='card'><div class='card-header'>Heuristic 3</div><div class='card-body'>";
                    output += ops.TreeToDebugString();
                    output += "</div></div>";

                    Heuristics.Heuristics.Heuristic4(ops);

                    output += "<div class='card'><div class='card-header'>Heuristic 4</div><div class='card-body'>";
                    output += ops.TreeToDebugString();
                    output += "</div></div>";

                    Heuristics.Heuristics.Heuristic5(ops);

                    return;
                } catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    output = "<div class='alert alert-danger'>" + e.ToString().Replace(Environment.NewLine, "<br/>") + "</div>";
                }
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
