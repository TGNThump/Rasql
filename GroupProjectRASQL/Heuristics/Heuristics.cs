﻿using GroupProjectRASQL.Operations;
using GroupProjectRASQL.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Condition = GroupProjectRASQL.Parser.TreeNode<System.String>;
using Node = GroupProjectRASQL.Parser.TreeNode<GroupProjectRASQL.Operations.Operation>;

namespace GroupProjectRASQL.Heuristics
{
    static class Heuristics
    {
        public static void AddSingle<K,V>(this IDictionary<K,IList<V>> dictionary, K key, V value)
        {
            if (!dictionary.ContainsKey(key)) dictionary.Add(key, new List<V>());
            dictionary[key].Add(value);
        }

        public static Node Heuristic0(Node root)
        {
            root.ForEach((operation) =>
            {
                if (operation.Data is Relation) return operation;
                if (operation.Data.getFieldNames().Count() == 0) return operation;

                IEnumerable<Node> relations = operation.Where(node => node.Data is Relation);
                IDictionary<String, IList<String>> fields = new Dictionary<string, IList<string>>();

                foreach (Node relation in relations)
                {
                    String relationName = ((Relation)relation.Data).name;
                    List<Node> path = new List<Node>();

                    for(Node current = relation; !current.Equals(operation); current = current.Parent)
                    {
                        path.Add(current);
                    }

                    path.Reverse();

                    foreach (Node node in path)
                    {
                        if (node.Data is Relation)
                        {
                            foreach(String field in ((Relation)node.Data).getFieldNames())
                            {
                                fields.AddSingle(((Relation)node.Data).name, field);
                            }
                        } else if (node.Data is RenameAttribute)
                        {
                            String oldName = ((RenameAttribute)node.Data).getOldName();
                            String newName = ((RenameAttribute)node.Data).getNewName();
                            
                            if (oldName.Contains('.'))
                            {
                                String[] old = oldName.Split('.');
                                if (fields.ContainsKey(old[0]))
                                {
                                    if (!fields[old[0]].Contains(old[1])) throw new Exception("Field '" + old[1] + "' not found in Relation " + old[0]);
                                    fields[old[0]].Remove(old[1]);
                                    fields.AddSingle(old[0], newName);
                                }
                            } else {
                                // Convert Dictionary<relationName, List<fieldName>> to List<KeyValuePair<relationName,fieldName>>
                                new KeyValuePair<String, String>("a", "b");
                                IEnumerable<KeyValuePair<String,String>> fieldPairs = fields.ToList().SelectMany(key => key.Value, (key, value) => new KeyValuePair<String,String>(key.Key, value));
                                // Filter to when fieldName = oldName
                                IEnumerable<KeyValuePair<String, String>> filtered = fieldPairs.Where(pair => pair.Value.Equals(oldName));

                                if (filtered.Count() > 1) throw new Exception("Field '" + oldName + "' found in multiple relations, please remove ambiguity.");

                                fields[filtered.Single().Key].Remove(filtered.Single().Value);
                                fields.AddSingle(filtered.Single().Key, newName);
                            }
                        } else if (node.Data is RenameRelation)
                        {
                            String newName = ((RenameRelation)node.Data).getNewName();
                            if (fields.Values.Distinct().Count() != fields.Values.Count()) throw new Exception("Cannot rename relation '" + newName + "' as a result of ambigious field names");
                            IList<String> values = fields.Values.Aggregate((a,b) => {
                                foreach (String value in b)
                                {
                                    a.Add(value);
                                }
                                return a;
                            });
                            fields.Clear();
                            foreach(String value in values) fields.AddSingle(newName, value);
                        }
                    }
                }
                
                IEnumerable<String> fieldNames = operation.Data.getFieldNames().ToArray();
                foreach(String oldName in fieldNames)
                {

                    if (oldName.Contains('.'))
                    {
                        String[] split = oldName.Split('.');
                        if (!fields.ContainsKey(split[0])) throw new Exception("Relation '" + split[0] + "' not found.");
                        if (!fields.ContainsKey(split[1])) throw new Exception("Field '" + split[1] + "' not found in Relation " + split[0]);
                    }
                    else
                    {
                        // Convert Dictionary<relationName, List<fieldName>> to List<KeyValuePair<relationName,fieldName>>
                        IEnumerable<KeyValuePair<String, String>> fieldPairs = fields.ToList().SelectMany(key => key.Value, (key, value) => new KeyValuePair<String, String>(key.Key, value));
                        // Filter to fieldnames
                        IEnumerable<KeyValuePair<String, String>> filtered = fieldPairs.Where(p => p.Value.Equals(oldName));

                        if (filtered.Count() > 1) throw new Exception("Field '" + oldName + "' found in multiple relations, please remove ambiguity.");
                        if (filtered.Count() == 0) throw new Exception("Field '" + oldName + "' not found in any relations.");

                        KeyValuePair<String, String> pair = filtered.Single();
                        String newName = pair.Key + "." + pair.Value;
                        operation.Data.setFieldName(oldName, newName);
                        Console.WriteLine("Replaced '" + oldName + "' with '" + newName + "'");
                    }
                }

                return operation;
            });

            return root;
        }

        public static void Heuristic1(Node root)
        {
            /*
             * Heuristic One deals with the splitting of any selection 
             * - σp(r) - statement that has more than one condition,
             * for example  σa=b and b=c  ,into several smaller selections.
            */

            root.ForEach((operation) =>
            {
                if (operation.Data is Selection)
                {
                    Selection selection = (Selection)operation.Data;
                    Condition condition = selection.getCondition();

                    condition = Conditions.ToCNF(condition);

                    if (condition.Data == "[and]")
                    {
                        Node[] children = new Node[operation.Children.Count];
                        operation.Children.CopyTo(children, 0);
                        operation.RemoveChildren();

                        Node newChild = new Node(new Selection(condition.Child(1)));
                        selection.setCondition(condition.Child(0));

                        newChild.AddChildren(children);
                        operation.AddChild(newChild);
                    }
                }
                return operation;
            });
        }

        public static void Heuristic2(TreeNode<Operation> rootTree)
        {
            rootTree.ForEach((element) =>
            {
                if (element.Data.GetType().Name == "Selection")
                {

                }

                return element;
            });
        }

        public static void Heuristic3(TreeNode<Operation> rootTree)
        {

        }

        public static void Heuristic4(TreeNode<Operation> rootTree)
        {
            rootTree.ForEach((element) => 
            {
                if (element.Data is Cartesian) 
                {
                    if (element.Parent.Data is Selection)
                    {
                        Selection selection = (Selection)element.Parent.Data;
                        element.Parent.Data = new Join(selection.getCondition());
                        element.Parent.RemoveChild(element);
                        element.Parent.AddChild(element.Child(0));
                        element.Parent.AddChild(element.Child(1));


                    }
                }
                return element;
            });
        }
        
        public static void Heuristic5(TreeNode<Operation> rootTree)
        {

        }



    }
    
    
}
