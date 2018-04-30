using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Condition = GroupProjectRASQL.Parser.TreeNode<System.String>;
using Node = GroupProjectRASQL.Parser.TreeNode<GroupProjectRASQL.Operations.Operation>;

namespace GroupProjectRASQL.Heuristics
{
    public abstract class Heuristic
    {
        protected Node root;
        protected Node next;
        protected bool isStarted = false;
        protected bool isComplete = false;

        public Heuristic(Node root)
        {
            this.root = root;
        }

        public void Step()
        {
            if (isComplete) return;

            if (!isStarted)
            {
                isStarted = true;
                next = root.Child();
            }

            bool stop = Run(next);
            Node last = next;

            next = last.getNextNode();

            if (next == null || next == root)
            {
                isComplete = true;
                return;
            }
            if (!stop) Step();
        }

        public void Complete()
        {
            while (!isComplete) Step();
        }

        public void Reset()
        {
            isStarted = false;
            isComplete = false;
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
