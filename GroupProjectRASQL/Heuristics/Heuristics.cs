using GroupProjectRASQL.Operations;
using GroupProjectRASQL.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
namespace GroupProjectRASQL.Heuristics
{
    static class Heuristics
    {

        public static void Heuristic1(TreeNode<Operation> operation)
        {
            /*
             * Heuristic One deals with the splitting of any selection 
             * - σp(r) - statement that has more than one condition,
             * for example  σa=b and b=c  ,into several smaller selections.
            */
            Console.WriteLine("Heuristics Start");


            rootTree.ForEach((operation) =>
            {
                if (!(operation.Data is Selection)) return;
                
                ((Selection)operation.Data).getCondition().ForEach((condition) => {
                    switch (condition.Data){
                        case "[or]":
                        case "[not]":
                            return true;
                        case "[and]":
                            ICollection<TreeNode<Operation>> children = operation.Children;
                            operation.RemoveChildren();
                            TreeNode<Operation> newChild = new TreeNode<Operation>(new Selection(null).setCondition(condition.Child(1)));
                            newChild.AddChildren(children);
                            operation.AddChild(newChild);
                            ((Selection)operation.Data).setCondition(condition.Child(0));
                            return false;
                        default: return true;
                    }

                    
                });

            });

            return rootTree;

        }

        public static void Heuristic2(TreeNode<Operation> rootTree)
        {

        }

        public static void Heuristic3(TreeNode<Operation> rootTree)
        {

        }

        public static void Heuristic4(TreeNode<Operation> rootTree)
        {
            rootTree.ForEach((element) => 
            {
                Console.WriteLine(element.Data.GetType().Name);
                Console.WriteLine(element.Data.parameter);

                if (element.Data.GetType().Name == "Cartesian") 
                {
                    if (element.Parent.Data.GetType().Name == "Selection")
                    {
                        Console.WriteLine("Being Edited");
                        
                        element.Parent.Data = new Join(element.Parent.Data.parameter);
                        element.Parent.RemoveChild(element);
                        element.Parent.AddChild(element.Child(0));
                        element.Parent.AddChild(element.Child(1));


                    }
                }
            });

            Console.WriteLine(rootTree.TreeToDebugString());
        }
        
        public static void Heuristic5(TreeNode<Operation> rootTree)
        {

        }



    }
    
    
}
