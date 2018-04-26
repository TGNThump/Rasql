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
<<<<<<< HEAD
            this.condition = condition;
=======

            switch (root.Data)
            {
                case "[literal]": return new TreeNode<string>(root.TreeToString());
                case "[andCondition]":
                    TreeNode<String> and = new TreeNode<String>("[and]");
                    and.AddChild(Translate(root.Child(0)));
                    and.AddChild(Translate(root.Child(2)));
                    return and;
                case "[orCondition]":
                    TreeNode<String> or = new TreeNode<String>("[or]");
                    or.AddChild(Translate(root.Child(0)));
                    or.AddChild(Translate(root.Child(2)));
                    return or;
                case "[not]":
                    TreeNode<String> not = new TreeNode<String>("[not]");
                    not.AddChild(Translate(root.Child(1)));
                    return not;
            }

            if (root.Children.Count == 1) return Translate(root.Child(0));
            if (root.Children.Count == 3) return Translate(root.Child(1));
            throw new Exception("Can't parse " + root.TreeToString());
>>>>>>> 25ae7a970347480657608758f95db20e3bab4de4
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
<<<<<<< HEAD
=======

        public void conjunctiveNormalForm() {

            if (condition.Data == "[not]") { }


        }
>>>>>>> 25ae7a970347480657608758f95db20e3bab4de4
    }
}
