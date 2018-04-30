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
        protected bool isComplete { get { return isStarted && remainingNodes.Count == 0; } }

        protected Queue<Node> remainingNodes = new Queue<Node>();

        public Heuristic(Node root)
        {
            this.root = root;
        }

        public void Init()
        {
            isStarted = true;
            remainingNodes = new Queue<Node>();
            for (Node node = root.Child(); node != null && node != root; node = node.getNextNode())
            {
                remainingNodes.Enqueue(node);
            }
        }

        public void Step()
        {
            if (!isStarted) Init();
            if (isComplete) return;

            Node next = remainingNodes.Dequeue();
            if (next.IsRoot && next.IsLeaf) Step();
            bool stop = Run(next);
            if (!stop && !isComplete) Step();
        }

        public void Complete()
        {
            while (!isComplete) Step();
        }

        public void Reset()
        {
            isStarted = false;
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
