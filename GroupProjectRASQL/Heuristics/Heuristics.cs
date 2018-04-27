using GroupProjectRASQL.Operations;
using GroupProjectRASQL.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Condition = GroupProjectRASQL.Parser.TreeNode<System.String>;
using Node = GroupProjectRASQL.Parser.TreeNode<GroupProjectRASQL.Operations.Operation>;

namespace GroupProjectRASQL.Heuristics
{
    static class Heuristics
    {
        private static int isRunning = 0;
        private static TreeNode<Operation> currentNode;

        public static void reset()
        {
            isRunning = 0;
            currentNode = null;
        }
        public static int Heuristic1(Node root,int typeOfStep=1)
        {
            /*
             * Heuristic One deals with the splitting of any selection 
             * - σp(r) - statement that has more than one condition,
             * for example  σa=b and b=c  ,into several smaller selections.
            */
            
            switch (isRunning)
            {
                case 0:
                    currentNode = root.ForEach((operation) =>
                    {
                        if (operation.Data is Selection)
                        {
                            Selection selection = (Selection)operation.Data;
                            Condition condition = selection.getCondition();

                            condition = Conditions.ToCNF(condition);

                            if (condition.Data == "[and]")
                            {
                                Node[] children = new Node[operation.Children.Count];
                                operation.Children.CopyTo(children, 0);
                                operation.RemoveChildren();

                                Node newChild = new Node(new Selection(condition.Child(1)));
                                selection.setCondition(condition.Child(0));

                                newChild.AddChildren(children);
                                operation.AddChild(newChild);
                            }
                        }
                    },(typeOfStep==1));
                    if(typeOfStep == 2)
                    {
                        return 2;
                    }
                    
                    isRunning = 1;
                    return 1;
                case 1:
                    if (currentNode != null)
                    {
                        Console.WriteLine(currentNode.ToString());
                        isRunning = 1;
                        if (typeOfStep == 1)
                        {
                            currentNode.step();
                        }
                        else
                        {
                            currentNode.stepToEnd();
                            return 2;
                        }
                        return 1;
                        
                    }
                    else
                    {
                        isRunning = 0;
                        return 2;
                    }
                default:
                    return 1;

            }
            
            
            
        }

        public static int Heuristic2(TreeNode<Operation> rootTree, int typeOfStep = 1)
        {
            rootTree.ForEach((element) =>
            {
                
            });
            return 3;

        }

        public static int Heuristic3(TreeNode<Operation> rootTree, int typeOfStep = 1)
        {
            return 4;
        }

        public static int Heuristic4(TreeNode<Operation> rootTree, int typeOfStep = 1)
        {
            rootTree.ForEach((element) => 
            {
                if (element.Data is Cartesian) 
                {
                    if (element.Parent.Data is Selection)
                    {
                        Selection selection = (Selection)element.Parent.Data;
                        element.Parent.Data = new Join(selection.getCondition());
                        element.Parent.RemoveChild(element);
                        element.Parent.AddChild(element.Child(0));
                        element.Parent.AddChild(element.Child(1));


                    }
                }
            });
            return 5;

        }

        public static int Heuristic5(TreeNode<Operation> rootTree, int typeOfStep = 1)
        {
            return 1;

        }

    }
    
    
}
