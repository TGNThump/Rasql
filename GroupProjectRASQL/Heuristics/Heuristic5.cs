using System;
using System.Collections.Generic;
using System.Linq;
using GroupProjectRASQL.Operations;
using System.Text;
using System.Threading.Tasks;
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

            //If the operation is not projection, this heuristic does not apply
            if (!(operation.Data is Projection)) return false;

            bool retVal = false;

            //If the projection has a valid number of children
            if(operation.Children.Count() == 1) {

                Node child = operation.Child(0);

               ////////////////// See what the child of this node is and act accordingly ///////////////

                //If child is projection, remove it from tree
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

                
                //If child is a join or cartesian:
                else if (child.Data is Join || child.Data is Cartesian)
                {

                    

                    HashSet<String> projectFields = new HashSet<String>(operation.Data.getFieldNames());
                    HashSet<String> conditionFields = child.Data is Join? new HashSet <String>(((Join)child.Data).getFieldNames()): null;
                    HashSet<String> lSubtreeFields = GetFields(child.Child(0));
                    HashSet<String> rSubtreeFields = GetFields(child.Child(1));

                    //If there is no condition, the projection can be moved completely
                    if (child.Data is Cartesian || conditionFields.IsSubsetOf(projectFields)) {

                        //Remove operation from tree and connect parent and child
                        operation.Parent.RemoveChildren();
                        operation.Parent.AddChildren(operation.Children);
                        child.Parent = operation.Parent;


                        retVal = true;
                    }

                    //See what fields the sub projections need
                    lSubtreeFields.IntersectWith(conditionFields.Union(projectFields));
                    rSubtreeFields.IntersectWith(conditionFields.Union(projectFields));

                    //Create new subtrees
                    Node rightSubTree = new Node(new Projection(rSubtreeFields));
                    Node leftSubTree = new Node(new Projection(lSubtreeFields));

                    if (lSubtreeFields.Count() > 0)
                    {
                       
                        leftSubTree.Parent = child;
                        leftSubTree.AddChild(child.Child(0));
                        child.Child(0).Parent = leftSubTree;
                        
                        remainingNodes.Enqueue(leftSubTree);

                        

                        retVal = true;
                    }

                    if (rSubtreeFields.Count() > 0)
                    {
                        
                        rightSubTree.Parent = child;
                        rightSubTree.AddChild(child.Child(1));
                        child.Child(1).Parent = rightSubTree;
                        
                        remainingNodes.Enqueue(rightSubTree);

                       

                        retVal = true;
                    }
                    if(lSubtreeFields.Count() > 0|| rSubtreeFields.Count() > 0)child.RemoveChildren();
                    if (lSubtreeFields.Count() > 0) child.AddChild(leftSubTree);
                    if (rSubtreeFields.Count() > 0) child.AddChild(rightSubTree);


                }
                else if (child.Data is Union)
                {
                    //Remove operation from tree and connect parent and child
                    operation.Parent.RemoveChildren();
                    operation.Parent.AddChildren(operation.Children);
                    child.Parent = operation.Parent;



                    //Create new subtrees
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

                    if (projectFields.SetEquals(conditionFields)) {

                        //Remove operation from tree and connect parent and child
                        operation.Parent.RemoveChildren();
                        operation.Parent.AddChildren(operation.Children);
                        child.Parent = operation.Parent;


                        
                        

                        retVal = true;
                    }

                    Node newNode = new Node(new Projection(conditionFields.Union(((Projection)operation.Data).getFieldNames())));

                    //Insert operation beneath select
                    newNode.Parent = child;
                    newNode.RemoveChildren();
                    newNode.AddChildren(child.Children);
                    child.RemoveChildren();
                    child.AddChild(newNode);

                    remainingNodes.Enqueue(newNode);


                    retVal = true;
                }

            }

            return retVal;
        }

        //Function to get fields of subtree
        public static HashSet<String> GetFields(Node node) {

            if (!(node.Data is Operation)) return null;

            HashSet<String> returnSet = new HashSet<string>();

            if (node.Data is Relation)
            {

                returnSet.UnionWith(((Relation)node.Data).getFullFieldNames());


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
