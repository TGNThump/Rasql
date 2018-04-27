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

        public override IEnumerable<String> getFieldNames()
        {
            return new List<String>() { oldName };
        }

        public override void setFieldName(string oldName, string newName)
        {
            this.oldName = newName;
        }

        public String getNewName()
        {
            return newName;
        }

        public String getOldName()
        {
            return oldName;
        }

        public override string ToString()
        {
            return "[" + this.GetType().Name + "]{new: " + newName + ", old: " + oldName + "}";
        }
    }
}
