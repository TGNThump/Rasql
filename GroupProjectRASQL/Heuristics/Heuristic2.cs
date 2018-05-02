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
    public class Heuristic2 : Heuristic
    {
        public Heuristic2(Node root) : base(root){
            this.name = "Selection Move Heuristic";
            this.description = "Heuristic two moves every selection down the tree as far as possible, it reduce the amount of data that must be acted upon. This means that every select should end up above either the join or the table that it acts upon.";
        }

        public override bool Run(Node operation)
        {
            if (!(operation.Data is Selection)) { return false; }  // if the current node is a selection 

            Node newChild = null;
            Selection selection = (Selection)operation.Data; // cast it to selection

            IEnumerable<String> relationNames = selection.getFieldNames().Select(name => name.Split('.')[0]).Distinct(); // Get the relation. portion of the conditions of a selection and add them to a ienumerable
            if (relationNames.Count() < 1) { return false; } // If it accesses no tables - an error case - return false
            if (relationNames.Count() == 1)// if it accesses one table it will be moved above it
            {
                //Console.WriteLine("Relations = 1"); // 


                newChild = operation.Where(node => // Search the reset of the tree
                {
                    if (node.Data is Relation)
                    { // if its a relation with the first ( and only in this case ) name return it


                        for (Node current = node; !current.Equals(operation); current = current.Parent) // check if there is a rename relation between the search node and the current searches root
                        {
                            if (current.Data is RenameRelation) { return false; }// if their is, the select cannot be moved - so return false.
                        }
                        return ((Relation)node.Data).name == relationNames.Single();
                    }




                    if (node.Data is RenameRelation) return ((RenameRelation)node.Data).getNewName() == relationNames.Single(); // or if its a renamed relation with the same name return it
                    return false;//else don't
                }).SingleOrDefault();//make sure there is only one- then convert the output from a list/Ienumerable to a single Node
            }



            if (relationNames.Count() > 1) // If it accesses multiple tables, it will be moved over the join that accesses all of them.
            {
                //Console.WriteLine("Relations > 1");

                newChild = operation.Where(Node => // find the join the select should go over
                {
                    if (Node.Data is Join) // if join
                    {

                        IEnumerable<Node> joins = Node.Where(NodeTables => // find the tables this join accesses by searching the tree below it for relations
                        {
                            if (NodeTables.Data is Relation)//if relation
                            {
                                return true;//return it
                            }
                            return false;//else don't
                        });

                        List<String> joinnames = new List<String>(); // make a list for the names
                        foreach( Node node in joins) // convert the node positions into name strings for comparison
                        {
                            Relation re = (Relation)node.Data;
                            joinnames.Add(re.name);
                        }// 
                        
                        foreach(String relationName in relationNames) // for each relation in the selected to be moved
                        {
                            if (!(joinnames.Contains(relationName))) { return false; }// if it is not contained in the list of relations the join accesses, this join cannot be the correct position so return false.
                        }
                        return true;// if it is, in all cases, it must be the correct position, so return true.

                    }
                    return false;// if there are no joins - error case - return false
                }).FirstOrDefault();// make sure it only returns a single node

            }

            
            for (Node current = newChild; !current.Equals(operation); current = current.Parent) // check if there is a rename relation between the newchild ( the position to move over ) and the current select
            {
                if (current.Data is RenameAttribute) { return false; }// if their is, the select cannot be moved - so return false.
            }
            

            // Peform the move 
            operation.Parent.RemoveChild(operation);
            operation.Parent.AddChildren(operation.Children);
            operation.RemoveChildren();

            Node newParent = newChild.Parent;
            newChild.Parent.RemoveChild(newChild);
            newParent.AddChild(operation);
            operation.AddChild(newChild);
            
            return true;
        }
    }
}
