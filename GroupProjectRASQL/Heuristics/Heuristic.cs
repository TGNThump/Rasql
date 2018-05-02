using GroupProjectRASQL.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Condition = GroupProjectRASQL.Parser.TreeNode<System.String>;
using Node = GroupProjectRASQL.Parser.TreeNode<GroupProjectRASQL.Operations.Operation>;

namespace GroupProjectRASQL.Heuristics
{
    public abstract class Heuristic : Reactive
    {
        protected Node root;
        protected Node next;
        public bool isStarted { get; private set; } = false;
        public bool isComplete { get; private set; } = false;
        public bool isEnabled { get; set; } = true;
        public String name { get; protected set; }
        public String description { get; protected set; }

        protected Queue<Node> remainingNodes = new Queue<Node>();

        public Heuristic(Node root) // create the heuristics- saving the root of the tree
        {
            this.root = root;
        }

        public void Init() // init the heuristic
        {
            isStarted = true; // istarted flag
            remainingNodes = new Queue<Node>(); // queue for the nodes
            for (Node node = root.Child(); node != null && node != root; node = node.getNextNode()) // for every non-null non-root node
            {
                remainingNodes.Enqueue(node); // add it to the queue
            }
        }

        public void Step() // step through the currently active heuristic - called by ui button
        {
            if (!isEnabled) return;
            if (!isStarted) Init();
            if (IsComplete()) return;

            Node next = remainingNodes.Dequeue();
            if (next.IsRoot && next.IsLeaf) Step();
            bool stop = Run(next);
            if (!stop && !IsComplete()) Step();
        }

        private bool IsComplete()
        {
            return isComplete = (isStarted && remainingNodes.Count == 0);
        }

        public void Complete() // complete the currently active heurisitc - called by ui button
        {
            if (!isEnabled) return;
            while (!IsComplete()) Step(); // while not done - step without interuptions
        }

        // Abstract method run for each Node in the tree.
        // returns true if it did anything.
        abstract public bool Run(Node node);
    }
}
