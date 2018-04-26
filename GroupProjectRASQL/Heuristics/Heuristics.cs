using GroupProjectRASQL.Operations;
using GroupProjectRASQL.Parser;
using System;
using System.Collections;
using System.Collections.Generic;

using Condition = GroupProjectRASQL.Parser.TreeNode<System.String>;
using Node = GroupProjectRASQL.Parser.TreeNode<GroupProjectRASQL.Operations.Operation>;

namespace GroupProjectRASQL.Heuristics
{
    static class Heuristics
    {

        public static void Heuristic1(Node root)
        {
            /*
             * Heuristic One deals with the splitting of any selection 
             * - σp(r) - statement that has more than one condition,
             * for example  σa=b and b=c  ,into several smaller selections.
            */

            root.ForEach((operation) =>
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
            });
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
