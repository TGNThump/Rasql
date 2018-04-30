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
    public class Heuristic1 : Heuristic
    {
        public Heuristic1(Node root) : base(root){}

        public override bool Run(Node operation)
        {
            if (!(operation.Data is Selection)) return false; 

            Selection selection = (Selection)operation.Data; // cast it to selection
            Condition condition = selection.getCondition(); // get the selections conditions

            condition = Conditions.ToCNF(condition); // In order to split the selection into multiple selections its condition must be in conjunctive normal form, this function handles that.

            if (condition.Data != "[and]") return false;
            // If it can be split

            Node[] children = new Node[operation.Children.Count]; // Create a new node
            operation.Children.CopyTo(children, 0); // Move this nodes child pointers to the new node
            operation.RemoveChildren(); // Remove the original nodes child pointers

            Node newChild = new Node(new Selection(condition.Child(1))); // Make a new selection
            selection.setCondition(condition.Child(0)); // set  its condition to the first branch of the original condition

            newChild.AddChildren(children); // give the new selection its position in the tree
            operation.AddChild(newChild);
            remainingNodes.Enqueue(newChild);

            return true;
        }
    }
}
