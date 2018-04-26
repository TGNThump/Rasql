using GroupProjectRASQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Node = GroupProjectRASQL.Parser.TreeNode<System.String>;

namespace GroupProjectRASQL.Operations
{
    class Selection : Operation
    {
        private TreeNode<String> condition;

        public Selection(TreeNode<string> parameter) : base(parameter) {
            if (parameter == null) return;
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

        public Selection setCondition(TreeNode<String> condition)
        {
            this.condition = condition;
            return this;
        }

        public override string ToString()
        {
             return "[" + this.GetType().Name + "]{<br />" + condition.TreeToDebugString() + "}";
        }

        public void conjunctiveNormalForm() {

            condition =moveOrs(moveNots(condition));

        }

        public static Node moveNots(Node node) {

            if (node.Data == "[not]")
            {



                switch (node.Child(0).Data)
                {

                    case "[not]":

                        node = moveNots(node.Child(0).Child(0));
                        break;

                    case "[and]":

                        node = new Node("[or]")
                        {
                            new Node("[not]"){ moveNots(node.Child(0).Child(0))},
                            new Node("[not]"){ moveNots(node.Child(0).Child(1))}

                        };
                        break;

                    case "[or]":
                        node = new Node("[and]")
                        {
                            new Node("[not]"){ moveNots(node.Child(0).Child(0))},
                            new Node("[not]"){ moveNots(node.Child(0).Child(1))}

                        };
                        break;

                }

            }
            else if (node.Data == "[and]" && node.Data == "[or]"){

                node = new Node(node.Data) { moveNots(node.Child(0)), moveNots(node.Child(1)) };

            }
         

            return node;

        }

        public static Node moveOrs(Node node) {

            if (node.Data == "[or]")
            {


                if (node.Child(0).Data == "[and]")
                {
                    node = new Node("[and]") {

                        new Node("[or]"){node.Child(1), node.Child(0).Child(0)},
                        new Node("[or]"){node.Child(1), node.Child(0).Child(1)}

                    };

                }
                else if (node.Child(1).Data == "[and]")
                {

                    node = new Node("[and]") {

                        new Node("[or]"){node.Child(1), node.Child(0).Child(0)},
                        new Node("[or]"){node.Child(1), node.Child(0).Child(1)}

                    };

                }



            }
            else if (node.Data == "[and]")
            {

                node = new Node(node.Data) { moveOrs(node.Child(0)), moveOrs(node.Child(1)) };

            }
            else if (node.Data == "[not]") {

                node = new Node(node.Data) { moveOrs(node.Child(0))};

            }



            return node;

        }

    }
}
