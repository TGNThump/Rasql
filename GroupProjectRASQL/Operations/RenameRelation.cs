using GroupProjectRASQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Operations
{
    class RenameRelation : Operation
    {
        public RenameRelation(TreeNode<string> parameter) : base(parameter) { }
    }
}
