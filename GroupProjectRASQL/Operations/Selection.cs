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

        public IEnumerable<String> getFields()
        {
            return Conditions.GetFields(condition);
        }

        public Selection setCondition(TreeNode<String> condition)
        {
            this.condition = condition;
            return this;
        }

        public override string ToString()
        {
             return "[" + this.GetType().Name + "]{<br />" + condition.TreeToDebugString() + "}";
        }
    }
}
