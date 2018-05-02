using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using GroupProjectRASQL.Operations;
using GroupProjectRASQL.Parser;
using Condition = GroupProjectRASQL.Parser.TreeNode<System.String>;
using Node = GroupProjectRASQL.Parser.TreeNode<GroupProjectRASQL.Operations.Operation>;

namespace GroupProjectRASQL.Heuristics
{
    public class Heuristic3 : Heuristic
    {
        private IEnumerable<Node> SelectionList;
        private IEnumerable<Node> RelationList;
        private List<KeyValuePair<Node, float>> RelationDictList;
        private Dictionary<Node,float> RelationDict = new Dictionary<Node, float>();
        private bool isRun;


        public Heuristic3(Node root) : base(root)
        {
            this.name = "Restriction Heuristic";
            this.description = "Heuristic three is the most complex heuristic. It deals with reordering the leaf nodes of the relational algebra tree such that the most restrictive selections (i.e. the ones that return the smallest data set) are executed first. This only occurs if it is possible without causing the creation of a cross join (cartesian product).";
            isRun = false; // There are alot of one time things in this heuristic so there is a flag to prevent unnessaccery execution of code
        }

        public override bool Run(Node operation)
        {
            
            if (!isRun) // if this is the first time
            {
                isRun = true; // Set the has been run before tag to true
                SelectionList = root.Where(node => // collect selections into one list
                {
                    if (node.Data is Selection)//if selection
                    {
                        return true;// add to list
                    }

                    return false;// else don't

                });
                RelationList = root.Where(node =>//collect relations
                {
                    if (node.Data is Relation)//if relation
                    {
                        return true;//add
                    }

                    return false;//don't

                });

                foreach ( Node re in RelationList) // convert the relation list into a dictionary with the worse case scenario
                {
                    RelationDict.Add(re, 1); // worst case ( everything is unique ) is a selectivity ratio of 1
                }

                foreach (Node Selection in SelectionList) // foreach selection
                {
                    foreach( String field in Selection.Data.getFieldNames()) // foreach field of each selection
                    {
                        string[] fieldsplit = field.Split('.'); // split them - getting the relation names

                        Node fieldSplitNode = operation.Where(node => // Find the relation pointers based on there names.
                        {
                            if (node.Data is Relation) return ((Relation)node.Data).name == fieldsplit[0];
                            if (node.Data is RenameRelation) return ((RenameRelation)node.Data).getNewName() == fieldsplit[0];
                            return false;
                        }).FirstOrDefault();

                        RelationDict[fieldSplitNode] *= selectivityEstimate(fieldSplitNode, fieldsplit[1]);// Find the total selectivity, by timesing 
                        
                    }
                }
                

                RelationDictList = RelationDict.ToList(); // Convert the relation dictionary to a list of KVPs so it can be sorted

                RelationDictList.Sort( // sort this list on the value - smallest first.
                    delegate (KeyValuePair<Node, float> pair1,
                    KeyValuePair<Node, float> pair2)
                    {
                        return pair1.Value.CompareTo(pair2.Value);
                    }
                );
                IList<KeyValuePair<Node, float>> optimalpermu;
                foreach (var permu in Permutate(RelationDictList, RelationDictList.Count))// move through possible permutations of the leaf nodes in optimality order.
                {

                    if (!resultsInCrossJoin(permu))// if it doesn't cause a crossjoin
                    {
                        optimalpermu = permu;// then this is the optimal leaf order
                                             /*
                                              * Unfortunatly due to time constraints the manipulation of the graph to cause this could not be completed.
                                              * This would handle moving the selects and joins to impletement this order
                                              * Instead this optimal permutation will just be output to console.
                                              */

                        Console.WriteLine("\n The Optimal permutation of leaf nodes is : ");

                        foreach(KeyValuePair< Node, float >op in optimalpermu)
                        {
                            Console.WriteLine(op);
                        }
                        Console.WriteLine();
                        break;
                    }

                }

            }

            return false;
        }

        public float selectivityEstimate(Node relation, String field) // Selectivity estimate
        {
            Relation relationData = (Relation)relation.Data;
            return (float)relationData.GetField(field).getDistinctCount() / (float)relationData.GetField(field).getCount(); // Take the number of unique fields and divide it by the total fields to get a estimate ration.
        }

        public bool resultsInCrossJoin(IList<KeyValuePair<Node, float>> permu)// will it result in a cross join
        {
            bool possibleFlag; // flag
            for (int i = 0; i<permu.Count()-1;i++) // for every key value pair in the permutation passed to this function
            {
                possibleFlag = false;
                foreach (Node currentSelect in SelectionList) // for each select
                {
                    List<String> currentSelectList = new List<String>();
                    foreach(String field in currentSelect.Data.getFieldNames()) // get the tables it acts on
                    {
                        currentSelectList.Add(field.Split('.')[0]);
                    }

                    Relation permui= (Relation)permu[i].Key.Data;// Cast the permutations
                    Relation permupitwo = (Relation)permu[i+1].Key.Data;



                    if (currentSelectList.Contains(permui.name))
                    {
                        if (currentSelectList.Contains(permupitwo.name))
                        {
                            possibleFlag = true; // if the select acts on both the tables in some way- switch the flag to true- as this means a cross join doesn't need to be created to use it
                        }
                    }
                }
                if (!possibleFlag) { return true; }// if its not possible, return that it results in a corssjoin
            }

            return false;// else return it doesn't cause one.
        }


        public void RotateRight<T>(IList<T> sequence, int count)// Permutation
        {
            T tmp = sequence[count - 1];
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

        public IEnumerable<IList<KeyValuePair<Node, float>>> Permutate(IList<KeyValuePair<Node, float>> sequence, int count) // Permutaton recursive function
        {

            if (count == 1) yield return sequence;//base case
            else
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (var perm in Permutate(sequence, count - 1))
                    {                    
                        yield return perm;
                    }
                    RotateRight(sequence, count);
                }
            }
        }

        









    }
}
