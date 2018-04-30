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
    public class Heuristic4 : Heuristic
    {
        public Heuristic4(Node root) : base(root){}

        public override bool Run(Node operation)
        {
             // The following block is the only part that meaningfully changes from Heuristic 1 - see comments there
            
            if (operation.Data is Cartesian) // if this node is a cartisean product
            {
                if (operation.Parent.Data is Selection) // and the node above it is a selection
                {
                    Selection selection = (Selection)operation.Parent.Data;  // cast the selection
                    operation.Parent.Data = new Join(selection.getCondition()); // create a new join using the cast selections condition

                    operation.Parent.RemoveChild(operation);  // Give this join its position in the list
                    operation.Parent.AddChild(operation.Child(0));
                    operation.Parent.AddChild(operation.Child(1));
                    return true;
                }
            }
            return false;
        }
    }
}
