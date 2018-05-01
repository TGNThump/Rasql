using System;
using System.Collections.Generic;
using System.Linq;
using GroupProjectRASQL.Operations;
using System.Text;
using System.Threading.Tasks;
using GroupProjectRASQL.Operations;
using GroupProjectRASQL.Parser;
using Condition = GroupProjectRASQL.Parser.TreeNode<System.String>;
using Node = GroupProjectRASQL.Parser.TreeNode<GroupProjectRASQL.Operations.Operation>;

namespace GroupProjectRASQL.Heuristics
{
    public class Heuristic5 : Heuristic
    {
        public Heuristic5(Node root) : base(root){}

        public override bool Run(Node operation)
        {

            if (!(operation.Data is Projection)) return false;

            bool retVal = false;

            if(operation.Children.Count() == 1) {

                Node child = operation.Child(0);

                if (child.Data is Projection)
                {
                    operation.RemoveChildren();
                    operation.AddChildren(child.Children);
                    operation.Child(0).Parent = operation;
                    child.RemoveChildren();
                    child.Parent = null;

                    remainingNodes.Enqueue(operation);

                    retVal = true;

                }

                

                else if (child.Data is Join || child.Data is Cartesian)
                {

                    

                    HashSet<String> projectFields = new HashSet<String>(operation.Data.getFieldNames());
                    HashSet<String> conditionFields = child.Data is Join? new HashSet <String>(Conditions.GetFields(((Join)child.Data).getCondition())): null;
                    HashSet<String> lSubtreeFields = GetFields(child.Child(0));
                    HashSet<String> rSubtreeFields = GetFields(child.Child(1));

                    if (child.Data is Cartesian || conditionFields.IsSubsetOf(projectFields)) {

                        //Remove operation from tree and connect parent and child
                        operation.Parent.RemoveChildren();
                        operation.Parent.AddChildren(operation.Children);
                        child.Parent = operation.Parent;


                        retVal = true;
                    }

                    lSubtreeFields.IntersectWith(projectFields);
                    rSubtreeFields.IntersectWith(projectFields);

                    if (lSubtreeFields.Count() > 0)
                    {
                        Node leftSubTree = new Node(new Projection(lSubtreeFields));
                        leftSubTree.AddChild(child.Child(0));
                        child.Child(0).Parent = leftSubTree;
                        leftSubTree.Parent = child;
                        remainingNodes.Enqueue(leftSubTree);

                        retVal = true;
                    }

                    if (rSubtreeFields.Count() > 0)
                    {
                        Node rightSubTree = new Node(new Projection(rSubtreeFields));
                        rightSubTree.AddChild(child.Child(1));
                        child.Child(1).Parent = rightSubTree;
                        rightSubTree.Parent = child;
                        remainingNodes.Enqueue(rightSubTree);

                        retVal = true;
                    }


                }
                else if (child.Data is Union)
                {
                    //Remove operation from tree and connect parent and child
                    operation.Parent.RemoveChildren();
                    operation.Parent.AddChildren(operation.Children);
                    child.Parent = operation.Parent;




                    Node leftSubTree = new Node(new Projection(operation.Data.parameter));
                    Node rightSubTree = new Node(new Projection(operation.Data.parameter));

                    leftSubTree.AddChildren(child.Child(0));
                    rightSubTree.AddChildren(child.Child(1));
                    child.Child(0).Parent = leftSubTree;
                    child.Child(1).Parent = rightSubTree;

                    child.RemoveChildren();
                    child.AddChild(leftSubTree);
                    child.AddChild(rightSubTree);
                    leftSubTree.Parent = child;
                    rightSubTree.Parent = child;

                    remainingNodes.Enqueue(leftSubTree);
                    remainingNodes.Enqueue(rightSubTree);




                    retVal = true;


                }
                else if (child.Data is Selection)
                {
                    HashSet<String> conditionFields = new HashSet<String>(Conditions.GetFields(((Selection)child.Data).getCondition()));
                    HashSet<String> projectFields = new HashSet<String>(operation.Data.getFieldNames());

                    if (projectFields.IsSubsetOf(conditionFields)) {

                        //Remove operation from tree and connect parent and child
                        operation.Parent.RemoveChildren();
                        operation.Parent.AddChildren(operation.Children);
                        child.Parent = operation.Parent;


                        //Insert operation beneath select
                        operation.Parent = child;
                        operation.RemoveChildren();
                        operation.AddChildren(child.Children);
                        child.RemoveChildren();
                        child.AddChild(operation);
                        

                        retVal = true;
                    }


                }

            }

            return retVal;
        }

        public static HashSet<String> GetFields(Node node) {

            if (!(node.Data is Operation)) return null;

            HashSet<String> returnSet = new HashSet<string>();

            if (node.Data is Relation)
            {

                returnSet.UnionWith(node.Data.getFieldNames());


            }
            else {
                foreach (Node child in node.Children) {

                    returnSet.UnionWith(GetFields(child));


                }



            }

            return returnSet;
        }

        
    }
}
