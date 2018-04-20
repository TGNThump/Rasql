using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupProjectRASQL.Parser;
using GroupProjectRASQL.Operations;

namespace GroupProjectRASQL.Translator
{
    class RAToOps
    {
        public static TreeNode<Operation> Translate(TreeNode<string> root)
        {
            if (root.Data.Equals("[string]") && root.Parent.Data.Equals("[query]"))
            {
                return new TreeNode<Operation>(new Relation(root.TreeToString()));
            }

            switch (root.Data)
            {
                case "[union]":
                    TreeNode<Operation> union = new TreeNode<Operation>(new Union());
                    union.AddChild(Translate(root.Child(2)));
                    union.AddChild(Translate(root.Child(4)));
                    return union;

                case "[intersection]":
                    TreeNode<Operation> intersection = new TreeNode<Operation>(new Intersect());
                    intersection.AddChild(Translate(root.Child(2)));
                    intersection.AddChild(Translate(root.Child(4)));
                    return intersection;

                case "[difference]":
                    TreeNode<Operation> difference = new TreeNode<Operation>(new Difference());
                    difference.AddChild(Translate(root.Child(1)));
                    difference.AddChild(Translate(root.Child(3)));
                    return difference;

                case "[cartesian]":
                    TreeNode<Operation> cartesian = new TreeNode<Operation>(new Cartesion());
                    cartesian.AddChild(Translate(root.Child(1)));
                    cartesian.AddChild(Translate(root.Child(3)));
                    return cartesian;

                case "[projection]":

                case "[selection]":
                case "[rename]":
                case "[join]":
                
                    break;
            }

            if (root.Children.Count == 1) return Translate(root.Child(0));
            throw new Exception("Can't parse " + root.TreeToString());
            return null;
        }
    }
}
