using GroupProjectRASQL.Operations;
using GroupProjectRASQL.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
namespace GroupProjectRASQL.Heuristics
{
    static class Heuristics
    {
        //public static void Heuristics()
        //{
        //    GroupProjectRASQL.ViewModel.ops;




        //}

        public static void Heuristic1(TreeNode<Operation> operation)
        {
            /*
             * Heuristic One deals with the splitting of any selection 
             * - σp(r) - statement that has more than one condition,
             * for example  σa=b and b=c  ,into several smaller selections.
            */

            if (!(operation.Data is Selection)) return;

            TreeNode<String> condition = ((Selection)operation.Data).getCondition();
            if (condition.Data != "[and]") return;

            ICollection<TreeNode<Operation>> children = operation.Children;
            operation.RemoveChildren();
            TreeNode<Operation> newChild = new TreeNode<Operation>(new Selection(null).setCondition(condition.Child(1)));
            newChild.AddChildren(children);
            operation.AddChild(newChild);
            ((Selection)operation.Data).setCondition(condition.Child(0));

            for (int i = 0; i < operation.Children.Count; i++)
            {
                Heuristic1(operation.Child(i));
            }
        }

        public static void Heuristic2(TreeNode<Operation> rootTree)
        {

        }
        public static void Heuristic3(TreeNode<Operation> rootTree)
        {

        }
        public static void Heuristic4(TreeNode<Operation> rootTree)
        {

        }
        public static void Heuristic5(TreeNode<Operation> rootTree)
        {

        }



    }
    
    
}
