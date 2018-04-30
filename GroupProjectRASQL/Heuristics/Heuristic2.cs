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
        public Heuristic2(Node root) : base(root){}

        public override bool Run(Node operation)
        {
            if (!(operation.Data is Selection)) { return false; }  // if the current node is a selection 
            
            Node newChild = null;
            Selection selection = (Selection)operation.Data; // cast it to selection

            IEnumerable<String> relationNames = selection.getFieldNames().Select(name => name.Split('.')[0]).Distinct();
            if (relationNames.Count() < 1) { return false; }
            if (relationNames.Count() == 1)
            {
                Console.WriteLine("Relations = 1");

                newChild = operation.Where(node =>
                {
                    if (node.Data is Relation) return ((Relation)node.Data).name == relationNames.Single();
                    if (node.Data is RenameRelation) return ((RenameRelation)node.Data).getNewName() == relationNames.Single();
                    return false;
                }).SingleOrDefault();
            }



            if (relationNames.Count() > 1)
            {
                Console.WriteLine("Relations > 1");

                newChild = operation.Where(Node =>
                {
                    if (Node.Data is Join)
                    {
                        Join join = (Join)Node.Data; // cast it to a join
                        IEnumerable<String> joins = join.getFieldNames();
                        bool isCorrectFlag = true;
                        foreach (String table in joins)
                        {
                            Console.WriteLine(table);
                            if (!relationNames.Contains(table))
                            {
                                isCorrectFlag = false;
                            }
                        }
                        return isCorrectFlag;

                    }

                    return false;
                }).SingleOrDefault();

            }

            for (Node current = newChild; !current.Equals(operation); current = current.Parent)
            {
                if (current.Data is RenameAttribute) { return false; }
            }
            


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
