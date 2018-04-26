using GroupProjectRASQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Operations
{
    abstract class Operation
    {

        public Operation(TreeNode<String> parameter = null)
        {
            this.parameter = parameter;
        }

        public TreeNode<String> parameter;

        public override string ToString()
        {
            if (parameter == null)
            {
                return "[" + this.GetType().Name + "]";
            } else
            {
                return "[" + this.GetType().Name + "](" + parameter.TreeToString() + ")";
            }
           
        }
    }
}
