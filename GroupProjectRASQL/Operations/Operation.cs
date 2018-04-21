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
        public TreeNode<String> parameter = new TreeNode<string>("");

        public override string ToString()
        {
            return "[" + this.GetType().Name + "]{" + parameter.TreeToString() + "}";
        }
    }
}
