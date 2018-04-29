using GroupProjectRASQL.Operations;
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
        private static int isRunning = 0; // Running flag
        private static TreeNode<Operation> currentNode; // Current Root for any paused heuristic

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
                            if (fields.ContainsKey(newName)) throw new Exception("Cannot rename to a relation name that already exists.");
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

        public static void reset()
        {
            isRunning = 0;
            currentNode = null;
        }
        public static int Heuristic1(Node root, int typeOfStep = 1) // Selection Split Heuristic
        {
            /*
             * Heuristic One deals with the splitting of any selection 
             * - σp(r) - statement that has more than one condition,
             * for example  σa=b and b=c  ,into several smaller selections.
             * 
             * 
             * 
             *      currentNode = root.ForEach((operation) => // ForEach is a iteration function for the tree, it returns the node at each step incase it needs to be interupted ( for stepwise explinations )
                    {
                        if (operation.Data is Selection) // if the current node is a selection 
                        {
                            Selection selection = (Selection)operation.Data; // cast it to selection
                            Condition condition = selection.getCondition(); // get the selections conditions

                            condition = Conditions.ToCNF(condition); // In order to split the selection into multiple selections its condition must be in conjunctive normal form, this function handles that.

                            if (condition.Data == "[and]") // If it can be split
                            {
                                Node[] children = new Node[operation.Children.Count]; // Create a new node
                                operation.Children.CopyTo(children, 0); // Move this nodes child pointers to the new node
                                operation.RemoveChildren(); // Remove the original nodes child pointers

                                Node newChild = new Node(new Selection(condition.Child(1))); // Make a new selection
                                selection.setCondition(condition.Child(0)); // set  its condition to the first branch of the original condition

                                newChild.AddChildren(children); // give the new selection its position in the tree
                                operation.AddChild(newChild);
                            }
                        }
                    }, (typeOfStep == 1)); // passes true if it needs to be done stepwise

            */

            bool isDone = false; // Init a temporary boolean to handle whether the tree has been explored
            switch (isRunning) // Switch on isRunning - heuristics class static variable that handles whether a heuristic is currently being run
            {
                case 0: // If not running
                    isRunning = 1;
                    currentNode = root.ForEach((operation) => // ForEach is a iteration function for the tree, it returns the node at each step incase it needs to be interupted ( for stepwise explinations )
                    {
                        if (operation.Data is Selection) // if the current node is a selection 
                        {
                            Selection selection = (Selection)operation.Data; // cast it to selection
                            Condition condition = selection.getCondition(); // get the selections conditions

                            condition = Conditions.ToCNF(condition); // In order to split the selection into multiple selections its condition must be in conjunctive normal form, this function handles that.

                            if (condition.Data == "[and]") // If it can be split
                            {
                                Node[] children = new Node[operation.Children.Count]; // Create a new node
                                operation.Children.CopyTo(children, 0); // Move this nodes child pointers to the new node
                                operation.RemoveChildren(); // Remove the original nodes child pointers

                                Node newChild = new Node(new Selection(condition.Child(1))); // Make a new selection
                                selection.setCondition(condition.Child(0)); // set  its condition to the first branch of the original condition

                                newChild.AddChildren(children); // give the new selection its position in the tree
                                operation.AddChild(newChild);
                            }
                        }
                    },( typeOfStep==1)); // passes true if it needs to be done stepwise
                    break;
                case 1: // if currently running
                    if(currentNode.Data is Query)
                    {
                        isDone = true;
                    }
                    currentNode = currentNode.ForEach((operation) => { operation.step(); }, (typeOfStep == 1));

                    break;
            }
            if (!isDone)
            {
                return 1;
            }
            else
            {
                return 2;
            }



        }

        public static int Heuristic2(Node root, int typeOfStep = 1) // Move Selection Heuristic
        {
            /*
             * Heuristic One deals with the splitting of any selection 
             * - σp(r) - statement that has more than one condition,
             * for example  σa=b and b=c  ,into several smaller selections.
            */
            Console.WriteLine("H2");

            bool temp = false;
            switch (isRunning)
            {
                case 0:
                    Console.WriteLine("H2 START");

                    currentNode = root.ForEach((operation) =>
                    {
                        //// CODE goes here
                        if (operation.Data is Selection) // if the current node is a selection 
                        {
                            Node newChild;
                            Selection selection = (Selection)operation.Data; // cast it to selection

                            IEnumerable<String> relationNames = selection.getFieldNames().Select(name => name.Split('.')[0]).Distinct();

                            if (relationNames.Count() == 1)
                            {
                                newChild = operation.Where(node => 
                                {
                                    if (node.Data is Relation) return ((Relation)node.Data).name == relationNames.Single();
                                    if (node.Data is RenameRelation) return ((RenameRelation)node.Data).getNewName() == relationNames.Single();
                                    return false;
                                }).SingleOrDefault();

                                operation.Parent.RemoveChild(operation);
                                operation.Parent.AddChildren(operation.Children);
                                operation.RemoveChildren();

                                Node newParent = newChild.Parent;
                                newChild.Parent.RemoveChild(newChild);
                                newParent.AddChild(operation);
                                operation.AddChild(newChild);
                            }

                        }
                    }, (typeOfStep == 1));



                    if (typeOfStep == 2)
                    {
                        return 3;// Changed to reflect current heuristic
                    }

                    isRunning = 1;
                    break;
                case 1:
                    Console.WriteLine("H2 STEP");
                    Console.WriteLine(currentNode.ToString());

                    isRunning = 1;
                    if (typeOfStep == 1)
                    {
                        temp = currentNode.step();
                    }
                    else
                    {
                        temp = currentNode.stepToEnd();
                    }
                    break;
                default:
                    break;
            }
            if (temp)
            {
                isRunning = 0; // this is finished
                return 3; }// Changed to reflect current heuristic
            else { return 2; } // Changed to reflect current heuristic

            
        }

        public static int Heuristic3(Node root, int typeOfStep = 1) // Restriction Heuristic
        {
            Console.WriteLine(root.TreeToDebugString());
            bool temp = false;
            switch (isRunning)
            {
                case 0:
                    currentNode = root.ForEach((operation) =>
                    {
                        //// CODE goes here
                    }, (typeOfStep == 1));



                    if (typeOfStep == 2)
                    {
                        return 4;// Changed to reflect current heuristic
                    }

                    isRunning = 1;
                    break;
                case 1:

                    Console.WriteLine(currentNode.ToString());
                    isRunning = 1;
                    if (typeOfStep == 1)
                    {
                        temp = currentNode.step();
                    }
                    else
                    {
                        temp = currentNode.stepToEnd();
                    }
                    break;
                default:
                    break;
            }
            return 4; // TEMPORARY SKIPS THIS
            if (temp)
            {
                isRunning = 0; // this is finished
                return 4; }// Changed to reflect current heuristic
            else { return 3; } // Changed to reflect current heuristic
        }

        public static int Heuristic4(Node root, int typeOfStep = 1) // Cartisean removal Heuristic

        {
            bool temp = false;
            switch (isRunning)
            {
                case 0:
                    currentNode = root.ForEach((element) => // The following block is the only part that meaningfully changes from Heuristic 1 - see comments there
                    {
                        if (element.Data is Cartesian) // if this node is a cartisean product
                        {
                            if (element.Parent.Data is Selection) // and the node above it is a selection
                            {
                                Selection selection = (Selection)element.Parent.Data;  // cast the selection
                                element.Parent.Data = new Join(selection.getCondition()); // create a new join using the cast selections condition

                                element.Parent.RemoveChild(element);  // Give this join its position in the list
                                element.Parent.AddChild(element.Child(0));
                                element.Parent.AddChild(element.Child(1));
                            }
                        }
                    }, (typeOfStep == 1));



                    if (typeOfStep == 2)
                    {
                        return 5; // Changed to reflect current heuristic
                    }

                    isRunning = 1;
                    break;
                case 1:

                   // Console.WriteLine(currentNode.ToString());
                    isRunning = 1;
                    if (typeOfStep == 1)
                    {
                        temp = currentNode.step();
                    }
                    else
                    {
                        temp = currentNode.stepToEnd();
                    }
                    break;
                default:
                    break;
            }
            if (temp)
            {
                isRunning = 0; // this is finished
                return 5; }// Changed to reflect current heuristic
            else { return 4; }// Changed to reflect current heuristic


        }

        public static int Heuristic5(Node root, int typeOfStep = 1) // Move projection heuristic
        {
            bool temp = false;
            switch (isRunning)
            {
                case 0:
                    currentNode = root.ForEach((operation) =>
                    {
                        //// CODE goes here
                    }, (typeOfStep == 1));



                    if (typeOfStep == 2)
                    {
                        return 1;// Changed to reflect current heuristic
                    }

                    isRunning = 1;
                    break;
                case 1:

                   // Console.WriteLine(currentNode.ToString());
                    isRunning = 1;
                    if (typeOfStep == 1)
                    {
                        temp = currentNode.step();
                    }
                    else
                    {
                        temp = currentNode.stepToEnd();
                    }
                    break;
                default:
                    break;
            }
            if (temp)
            {
                isRunning = 0; // this is finished
                return 1; }// Changed to reflect current heuristic
            else { return 5; } // Changed to reflect current heuristic // this heuristic loops back to the start.

        }

    }
    
    
}

