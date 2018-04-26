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
        public static TreeNode<Operation> Translate(TreeNode<string> root, Dictionary<String, Relation> relations)
        {
            if (root.Data.Equals("[string]") && root.Parent.Data.Equals("[query]"))
            {
                Relation relation;
                if (!relations.TryGetValue(root.TreeToString(), out relation)){
                    throw new Exception("Using relations not defined in schema.");
                }
                return new TreeNode<Operation>(relation);
            }

            switch (root.Data)
            {
                case "[union]":
                    TreeNode<Operation> union = new TreeNode<Operation>(new Union());
                    union.AddChild(Translate(root.Child(2), relations));
                    union.AddChild(Translate(root.Child(4), relations));
                    return union;

                case "[intersection]":
                    TreeNode<Operation> intersection = new TreeNode<Operation>(new Intersect());
                    intersection.AddChild(Translate(root.Child(2), relations));
                    intersection.AddChild(Translate(root.Child(4), relations));
                    return intersection;

                case "[difference]":
                    TreeNode<Operation> difference = new TreeNode<Operation>(new Difference());
                    difference.AddChild(Translate(root.Child(1), relations));
                    difference.AddChild(Translate(root.Child(3), relations));
                    return difference;

                case "[cartesian]":
                    TreeNode<Operation> cartesian = new TreeNode<Operation>(new Cartesian());
                    cartesian.AddChild(Translate(root.Child(1), relations));
                    cartesian.AddChild(Translate(root.Child(3), relations));
                    return cartesian;

                case "[projection]":
                    TreeNode<Operation> projection = new TreeNode<Operation>(new Projection(root.Child(1)));
                    projection.AddChild(Translate(root.Child(3), relations));
                    return projection;

                case "[selection]":
                    TreeNode<Operation> selection = new TreeNode<Operation>(Selection.fromParameters(root.Child(1)));
                    selection.AddChild(Translate(root.Child(3), relations));
                    return selection;

                case "[attRename]":
                    TreeNode<String> paramaters = new TreeNode<String>("");
                    paramaters.AddChild(root.Child(1));
                    paramaters.AddChild(root.Child(3));

                    TreeNode<Operation> attRename = new TreeNode<Operation>(new RenameAttribute(paramaters));
                    attRename.AddChild(Translate(root.Child(5), relations));
                    return attRename;
                case "[relRename]":
                    TreeNode<Operation> relRename = new TreeNode<Operation>(new RenameRelation(root.Child(1)));
                    relRename.AddChild(Translate(root.Child(3), relations));
                    return relRename;

                case "[join]":
                    bool hasCondition = root.Children.Count > 6;

                    TreeNode<Operation> join = new TreeNode<Operation>(Join.fromParameters(hasCondition ? root.Child(2) : null));
                    join.AddChild(Translate(root.Child((hasCondition ? 1 : 0) + 2), relations));
                    join.AddChild(Translate(root.Child((hasCondition ? 1 : 0) + 4), relations));
                    return join;
            }

            if (root.Children.Count == 1) return Translate(root.Child(0), relations);
            throw new Exception("Can't parse " + root.TreeToString());
        }
    }
}
