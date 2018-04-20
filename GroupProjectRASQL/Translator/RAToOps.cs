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
                    TreeNode<Operation> projection = new TreeNode<Operation>(new Projection(root.Child(1)));
                    projection.AddChild(Translate(root.Child(3)));
                    return projection;

                case "[selection]":
                    TreeNode<Operation> selection = new TreeNode<Operation>(new Selection(root.Child(1)));
                    selection.AddChild(Translate(root.Child(3)));
                    return selection;

                case "[attRename]":
                    TreeNode<String> paramaters = new TreeNode<String>("");
                    paramaters.AddChild(root.Child(1));
                    paramaters.AddChild(root.Child(3));

                    TreeNode<Operation> attRename = new TreeNode<Operation>(new RenameAttribute(paramaters));
                    attRename.AddChild(Translate(root.Child(5)));
                    return attRename;
                case "[relRename]":
                    TreeNode<Operation> relRename = new TreeNode<Operation>(new RenameRelation(root.Child(1)));
                    relRename.AddChild(Translate(root.Child(3)));
                    return relRename;

                case "[join]":
                    bool hasCondition = root.Children.Count > 6;

                    TreeNode<Operation> join = new TreeNode<Operation>(new Join(hasCondition ? root.Child(2) : null));
                    join.AddChild(Translate(root.Child((hasCondition ? 1 : 0) + 2)));
                    join.AddChild(Translate(root.Child((hasCondition ? 1 : 0) + 4)));
                    return join;
            }

            if (root.Children.Count == 1) return Translate(root.Child(0));
            throw new Exception("Can't parse " + root.TreeToString());
        }
    }
}
