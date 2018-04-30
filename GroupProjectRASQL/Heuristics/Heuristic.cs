using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Condition = GroupProjectRASQL.Parser.TreeNode<System.String>;
using Node = GroupProjectRASQL.Parser.TreeNode<GroupProjectRASQL.Operations.Operation>;

namespace GroupProjectRASQL.Heuristics
{
    abstract class Heuristic
    {
        protected Node root;
        protected Node last;
        protected bool isStarted = false;
        protected bool isComplete = false;

        public Heuristic(Node root)
        {
            this.root = root;
        }

        public void Step()
        {
            if (isComplete) return;

            Node next;

            if (!isStarted)
            {
                isStarted = true;
                next = root.Child();
            }
            else next = last.getNextNode();

            if (next == null || next == root)
            {
                isComplete = true;
                return;
            }

            last = next;
            if (!Run(next)) Step();
        }

        public void Complete()
        {
            while (!isComplete) Step();
        }

        public void Reset()
        {
            isStarted = false;
            isComplete = false;
            last = null;
        }

        // Abstract method run for each Node in the tree.
        // returns true if it did anything.
        abstract public bool Run(Node node);

        public bool IsComplete()
        {
            return isComplete;
        }
    }
}
