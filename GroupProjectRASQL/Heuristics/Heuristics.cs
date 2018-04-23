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

            dfsStack.Push(currentChild); // Push start of tree
            while(dfsStack.Count > 0) // Depth first traverse the tree
            {
                /*while ( stepType != 0) // This is where the Step type check happens,
                {
                        if button press > break 
                        if >> stepType = 0
                }
                */
                TreeNode<Operation> node = dfsStack.Pop();
                if (visitedNodes.Contains(node))
                {
                    continue;
                }
                visitedNodes.Add(node);
                ////
                if (node.Data.GetType().Name == "Selection") // if selection
                {
                    String currentParameter = node.Data.parameter.TreeToString();
                    if (currentParameter.Contains("AND")) // if more than one parameter
                    {
                        String[] splitParameter = currentParameter.Split(new string[] { "AND" }, StringSplitOptions.None); // Split String on any ANDs
                        TreeNode<Operation> currentNode = node; // create a current node variable for the swap
                        TreeNode<Operation> finalNode = node.Child(0); // save the child of the node to be split - to allow for the final child/ parent to be set
                        node.Data.parameter = new TreeNode<String>(splitParameter[0]);

                        for (int x=1; x< splitParameter.Length; x++ )
                        {
                            TreeNode<Operation> selection = new TreeNode<Operation>(new Selection(new TreeNode<String>(splitParameter[x]))); // New selection
                            currentNode.RemoveChildren(); // Remove old children from current node
                            currentNode.AddChild(selection); // Add the new node as a Child to current node
                            currentNode = selection; // set the current node to the new node incase there are more than 2 selections
                        }
                        currentNode.AddChild(finalNode); // set the child of the last node that has been split

                        

                    };
                }
                ////
                foreach (TreeNode<Operation> child in node.Children)
                {
                    if (!visitedNodes.Contains(child)) // If unvisited 
                    {
                        tempStack.Push(child); // Pushes unvisted children to a stack
                    }
                }
                while (tempStack.Count != 0)// A second stack is used to reverse the input ( left to right traversal rather than right to left )
                {
                    dfsStack.Push(tempStack.Pop());
                }

                Console.WriteLine(node); // Temp Output - to be replaced with a call to the neutronium outputter - possibly


            }
            return rootTree; // Return pointer to the tree that has been edited.
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
        public static void NeutroniumOutputter()
        {

        }



    }
    
    
}
