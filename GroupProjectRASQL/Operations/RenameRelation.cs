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
        private String newName;

        public RenameRelation(TreeNode<string> parameter) : base(parameter) {
            this.newName = parameter.TreeToString();
        }

        public String getNewName()
        {
            return newName;
        }

        public override string ToString()
        {
            return "[" + this.GetType().Name + "]{new: " + newName + "}";
        }
    }
}
