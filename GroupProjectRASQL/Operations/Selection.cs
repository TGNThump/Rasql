using GroupProjectRASQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Operations
{
    class Selection : Operation
    {
        private TreeNode<String> condition;

        public static Selection fromParameters(TreeNode<String> parameters)
        {
            return new Selection(Conditions.Translate(parameters));
        }

        public Selection(TreeNode<String> condition)
        {
            this.condition = condition;
        }

        public TreeNode<String> getCondition()
        {
            return condition;
        }

        public override IEnumerable<String> getFieldNames()
        {
            return Conditions.GetFields(condition);
        }

        public override void setFieldName(string oldName, string newName)
        {
            this.condition = Conditions.SetField(condition, oldName, newName);
        }

        public Selection setCondition(TreeNode<String> condition)
        {
            this.condition = condition;
            return this;
        }

        public override string ToString()
        {
             return ToString(0);
        }

        public string ToString(int depth = 0)
        {
            String ret = "[" + this.GetType().Name + "](<br />" + condition.TreeToDebugString(new List<String>() { "[literal]" }, depth+1);
            for (int i = 0; i < depth; i++) ret += "&nbsp;&nbsp;&nbsp;&nbsp;";
            ret += ")";
            return ret;
        }
    }
}
