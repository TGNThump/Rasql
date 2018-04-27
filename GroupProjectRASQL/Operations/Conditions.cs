using GroupProjectRASQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Node = GroupProjectRASQL.Parser.TreeNode<System.String>;


namespace GroupProjectRASQL.Operations
{
    static class Conditions
    {
        public static Node Translate(Node root)
        {

            switch (root.Data)
            {
                case "[literal]": return root;
                case "[andCondition]":
                    return new TreeNode<String>("[and]")
                    {
                        Translate(root.Child(0)),
                        Translate(root.Child(2))
                    };
                case "[orCondition]":
                    return new TreeNode<String>("[or]")
                    {
                        Translate(root.Child(0)),
                        Translate(root.Child(2))
                    };
                case "[notCondition]":
                    return new TreeNode<String>("[not]")
                    {
                        Translate(root.Child(1))
                    };
            }

            if (root.Children.Count == 1) return Translate(root.Child(0));
            if (root.Children.Count == 3) return Translate(root.Child(1));
            throw new Exception("Can't parse " + root.TreeToString());
        }

        public static IEnumerable<String> GetFields(Node condition)
        {
            return condition.Where(node => node.Data == "[field]").Select(node => node.Child().Data);
        }

        public static Node SetField(Node root, String oldName, String newName)
        {
           foreach(Node condition in root)
            {
                if (root.Data.Equals(oldName)) root.Data = newName;
            }
            return root;
        }

        public static Node ToCNF(Node condition)
        {
            // NOTs only directly above literals
            // ORs only below ANDs

            condition = moveNots(condition);
            while (!isCNF(condition))
            {
                condition = moveOrs(condition);
            }

            return condition;
        }

        public static bool isCNF(Node node)
        {
            foreach (Node child in node.Where(child => child.Data == "[or]"))
            {
                if (child.FindTreeNode(grandchild => grandchild.Data == "[and]") != null)
                {
                    return false;
                }
            }
            return true;
        }

        public static Node moveNots(Node node)
        {

            if (node.Data == "[not]")
            {
                switch (node.Child().Data)
                {
                    case "[not]":
                        node = moveNots(node.Child(0).Child(0));
                        break;

                    case "[and]":
                        node = new Node("[or]")
                        {
                            node.Child().Child(0).Data == "[not]" ? moveNots(node.Child().Child(0).Child()) : new Node("[not]"){ moveNots(node.Child().Child(0))},
                            node.Child().Child(1).Data == "[not]" ? moveNots(node.Child().Child(1).Child()) : new Node("[not]"){ moveNots(node.Child().Child(1))},
                        };
                        break;

                    case "[or]":
                        node = new Node("[and]")
                        {
                            node.Child().Child(0).Data == "[not]" ? moveNots(node.Child().Child(0).Child()) : new Node("[not]"){ moveNots(node.Child().Child(0))},
                            node.Child().Child(1).Data == "[not]" ? moveNots(node.Child().Child(1).Child()) : new Node("[not]"){ moveNots(node.Child().Child(1))},
                        };
                        break;
                }
            }
            else if (node.Data == "[and]" && node.Data == "[or]")
            {
                node = new Node(node.Data) { moveNots(node.Child(0)), moveNots(node.Child(1)) };
            }

            return node;
        }

        public static Node moveOrs(Node node)
        {
            if (node.Data == "[or]")
            {
                if (node.Child(0).Data == "[and]")
                {
                    node = new Node("[and]") {
                        new Node("[or]"){node.Child(1), node.Child().Child(0)},
                        new Node("[or]"){node.Child(1), node.Child().Child(1)}
                    };
                }
                else if (node.Child(1).Data == "[and]")
                {
                    node = new Node("[and]") {
                        new Node("[or]"){node.Child(1), node.Child().Child(0)},
                        new Node("[or]"){node.Child(1), node.Child().Child(1)}
                    };
                }
            }
            else if (node.Data == "[and]")
            {
                node = new Node(node.Data) { moveOrs(node.Child(0)), moveOrs(node.Child(1)) };
            }
            else if (node.Data == "[not]")
            {
                node = new Node(node.Data) { moveOrs(node.Child()) };
            }
            return node;
        }

    }
}
