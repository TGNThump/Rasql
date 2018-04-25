using GroupProjectRASQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Operations
{
    class RenameAttribute : Operation
    {
        private String oldName;
        private String newName;

        public RenameAttribute(TreeNode<string> parameter) : base(parameter) {
            newName = parameter.Child(0).TreeToString();
            oldName = parameter.Child(1).TreeToString();
        }

        public String getNewName()
        {
            return newName;
        }

        public String getOldName()
        {
            return oldName;
        }
    }
}
