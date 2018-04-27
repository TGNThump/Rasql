using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Parser
{
    class DFS
    {

        public static List<Edge> DepthFirstSearch<Node, Edge>(
             //a function that given a depth and a node, will return a list of edges.
             Func<int, Node, List<Edge>> edges,
             //a function that given a depth and an edge, will return a node.
             Func<int, Edge, Node> child,
             //a function that given a depth and a node, will tell you if the node is a leaf.
             Func<int, Node, bool> pred,
             //a node. This will be the starting point of the search.
             Node root
         )
        {
            //Debug.WriteLine("DFS(" + root + "): ");
            // Stack of depths and Nodes.
            Stack<KeyValuePair<int, Node>> stack = new Stack<KeyValuePair<int, Node>>();
            Dictionary<Node, KeyValuePair<Node, Edge>> parent = new Dictionary<Node, KeyValuePair<Node, Edge>>();
            //HashSet<Node> seen = new HashSet<Node>();

            stack.Push(new KeyValuePair<int, Node>(0, root));
            //seen.Add(root);

            while (stack.Count > 0)
            {
                KeyValuePair<int, Node> v = stack.Pop();
                //Debug.WriteLine("  " + v.Value + ", (" + v.Key + ") {");
                List<Edge> us = edges(v.Key, v.Value);
                foreach (Edge u in us)
                {
                    //Debug.WriteLine("    " + u);
                    Node w = child(v.Key, u);
                    //if (!seen.Contains(w))
                    //{
                    parent[w] = new KeyValuePair<Node, Edge>(v.Value, u);

                    if (pred(v.Key + 1, w))
                    {
                        List<Edge> path = new List<Edge>();

                        KeyValuePair<Node, Edge> p = parent[w];
                        do
                        {
                            path.Add(p.Value);
                        } while (parent.TryGetValue(p.Key, out p));

                        path.Reverse();
                        //Debug.WriteLine("    SUCCESS");
                        //Debug.WriteLine("  }");
                        //foreach (Edge edge in path) Debug.WriteLine("  " + edge);
                        return path;
                    }

                    //seen.Add(w);
                    stack.Push(new KeyValuePair<int, Node>(v.Key + 1, w));

                    //}
                }

                //Debug.WriteLine("  }");
            }

            //Debug.WriteLine("  FAIL");
            // Something went very wrong.
            return new List<Edge>();
        }
    }
}
