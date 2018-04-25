using GroupProjectRASQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Operations
{
    class Join : Operation
    {
        private TreeNode<String> condition;

        public Join(TreeNode<string> parameter) : base(parameter) {
            this.condition = Translate(parameter);
        }

        public TreeNode<String> Translate(TreeNode<string> root)
        {

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
        }

        public TreeNode<String> getCondition()
        {
            return condition;
        }

        public void setCondition(TreeNode<String> condition)
        {
            this.condition = condition;
        }

        public override string ToString()
        {
            return "[" + this.GetType().Name + "]{<br />" + condition.TreeToDebugString() + "}";
        }
    }
}
}
