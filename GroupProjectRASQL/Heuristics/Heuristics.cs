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

        public static TreeNode<Operation> Heuristic1(TreeNode<Operation> rootTree, int stepType = 0)
        {
            /*
             * Heuristic One deals with the splitting of any selection 
             * - σp(r) - statement that has more than one condition,
             * for example  σa=b and b=c  ,into several smaller selections.
            */
            Console.WriteLine("Heuristics Start");
            // int stepType. Denotes the Current Step setting. 0 = to end, 1 = wait for button press.
            TreeNode<Operation> currentChild = rootTree;
           
            var visitedNodes = new HashSet<TreeNode<Operation>>();
            var dfsStack = new Stack<TreeNode<Operation>>();
            var tempStack = new Stack<TreeNode<Operation>>();

            dfsStack.Push(currentChild);
            while(dfsStack.Count > 0)
            {
                TreeNode<Operation> node = dfsStack.Pop();
                if (visitedNodes.Contains(node))
                {
                    continue;
                }
                visitedNodes.Add(node);
                ////
                if (node.Data.GetType().Name == "Selection")
                {
                    String currentParameter = node.Data.parameter.TreeToString();
                    if (currentParameter.Contains("AND"))
                    {
                        //split goes here
                    };
                }
                ////
                foreach (TreeNode<Operation> child in node.Children)
                {
                    if (!visitedNodes.Contains(child))
                    {
                        tempStack.Push(child);
                    }
                }
                while (tempStack.Count != 0)// A second stack is used
                {
                    dfsStack.Push(tempStack.Pop());
                }

                Console.WriteLine(node);


            }



            return rootTree;
        }

        public static void Heuristic2()
        {

        }
        public static void Heuristic3()
        {

        }
        public static void Heuristic4()
        {

        }
        public static void Heuristic5()
        {

        }




    }
    
    
}
