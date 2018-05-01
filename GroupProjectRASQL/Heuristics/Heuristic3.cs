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
            isRun = false;
        }

        public override bool Run(Node operation)
        {
            
            if (!isRun)
            {
                isRun = true;
                SelectionList = root.Where(node =>
                {
                    if (node.Data is Selection)
                    {
                        return true;
                    }

                    return false;

                });
                RelationList = root.Where(node =>
                {
                    if (node.Data is Relation)
                    {
                        return true;
                    }

                    return false;

                });

                foreach ( Node re in RelationList)
                {
                    RelationDict.Add(re, 1);
                }

                foreach (Node Selection in SelectionList)
                {
                    foreach( String field in Selection.Data.getFieldNames())
                    {
                        string[] fieldsplit = field.Split('.');

                        Node fieldSplitNode = operation.Where(node =>
                        {
                            if (node.Data is Relation) return ((Relation)node.Data).name == fieldsplit[0];
                            if (node.Data is RenameRelation) return ((RenameRelation)node.Data).getNewName() == fieldsplit[0];
                            return false;
                        }).SingleOrDefault();

                        RelationDict[fieldSplitNode] *= selectivityEstimate(fieldSplitNode, fieldsplit[1]);
                        
                    }
                }
                

                RelationDictList = RelationDict.ToList();

                RelationDictList.Sort(
                    delegate (KeyValuePair<Node, float> pair1,
                    KeyValuePair<Node, float> pair2)
                    {
                        return pair1.Value.CompareTo(pair2.Value);
                    }
                );
                IList<KeyValuePair<Node, float>> optimalpermu;
                foreach (var permu in Permutate(RelationDictList, RelationDictList.Count))
                {

                    if (!resultsInCrossJoin(permu))
                    {
                        optimalpermu = permu;
                        break;
                    }
                    
                    foreach ( var kvp in permu)
                    {
                        Console.Write(kvp.Key);
                        Console.Write(" : ");
                        Console.Write(kvp.Value);
                        Console.Write("\n");

                    }
                }









            }















            return false;
        }

        public float selectivityEstimate(Node relation, String field)
        {
            Relation relationData = (Relation)relation.Data;
            return (float)relationData.GetField(field).getDistinctCount() / (float)relationData.GetField(field).getCount();
        }

        public bool resultsInCrossJoin(IList<KeyValuePair<Node, float>> permu)
        {
            bool possibleFlag;
            for (int i = 0; i<permu.Count()-1;i++)
            {
                possibleFlag = false;
                foreach(Node currentSelect in SelectionList)
                {
                    if (currentSelect.Contains(permu[i].Key))
                    {
                        if (currentSelect.Contains(permu[i+1].Key))
                        {
                            possibleFlag = true;
                        }
                    }
                }
            }
            Console.WriteLine("Asking for crossjoins");
            Console.WriteLine(permu);
        

            return true;
        }


        public void RotateRight<T>(IList<T> sequence, int count)
        {
            T tmp = sequence[count - 1];
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

        public IEnumerable<IList<KeyValuePair<Node, float>>> Permutate(IList<KeyValuePair<Node, float>> sequence, int count)
        {
            Console.WriteLine("Asking for permutations");
            Console.WriteLine(count);

            if (count == 1) yield return sequence;
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
