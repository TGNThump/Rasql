using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Parser
{
    class Parser
    {
        Dictionary<String, List<String[]>> grammer = new Dictionary<string, List<string[]>>();
        Dictionary<String, List<String[]>> defaults = new Dictionary<string, List<string[]>>()
        {
            { "[0-9]", new List<String[]>(){
                new string[]{"0"},
                new string[]{"1"},
                new string[]{"2"},
                new string[]{"3"},
                new string[]{"4"},
                new string[]{"5"},
                new string[]{"6"},
                new string[]{"7"},
                new string[]{"8"},
                new string[]{"9"}
            }},
            { "[a-z]", new List<String[]>(){
                new string[]{"a"}, new string[]{"b"}, new string[]{"c"}, new string[]{"d"}, new string[]{"e"}, new string[]{"f"},
                new string[]{"g"}, new string[]{"h"}, new string[]{"i"}, new string[]{"j"}, new string[]{"k"}, new string[]{"l"},
                new string[]{"m"}, new string[]{"n"}, new string[]{"o"}, new string[]{"p"}, new string[]{"q"}, new string[]{"r"},
                new string[]{"s"}, new string[]{"t"}, new string[]{"u"}, new string[]{"v"}, new string[]{"w"}, new string[]{"x"},
                new string[]{"y"}, new string[]{"z"},
            }},
            { "[A-Z]", new List<String[]>(){
                new string[]{"A"}, new string[]{"B"}, new string[]{"C"}, new string[]{"D"}, new string[]{"E"}, new string[]{"F"},
                new string[]{"G"}, new string[]{"H"}, new string[]{"I"}, new string[]{"J"}, new string[]{"K"}, new string[]{"L"},
                new string[]{"M"}, new string[]{"N"}, new string[]{"O"}, new string[]{"P"}, new string[]{"Q"}, new string[]{"R"},
                new string[]{"S"}, new string[]{"T"}, new string[]{"U"}, new string[]{"V"}, new string[]{"W"}, new string[]{"X"},
                new string[]{"Y"}, new string[]{"Z"},
            }},
            { "[a-zA-Z]", new List<string[]>() {
                new string[]{"[a-z]"},
                new string[]{"[A-Z]"},
            }},
            { "[A-Za-z]", new List<string[]>() {
                new string[]{"[a-z]"},
                new string[]{"[A-Z]"},
            }},
        };

        public Parser(String path = @"C:\Users\ben\Projects\COMP208G17\bnf\bnfregexp-sql.lua")
        {
            String[] lines = System.IO.File.ReadAllLines(path);
            
            for(int i=0; i<lines.Length; i++)
            {
                String line = lines[i];
                if (line.Equals("")) continue;
                if (line.StartsWith("-- ")) continue;
                String formatted = "";

                bool inStringSingleQuote = false;
                bool inStringDoubleQuote = false;

                for (int c=0; c<line.Length; c++)
                {
                    if (line[c].Equals('\'') && !(line[c - 1].Equals('\\') || inStringDoubleQuote))
                    {
                        inStringSingleQuote = !inStringSingleQuote;
                    } else if (line[c].Equals('"') && !(line[c - 1].Equals('\\') || inStringSingleQuote))
                    {
                        inStringDoubleQuote = !inStringDoubleQuote;
                    }

                    if (inStringSingleQuote || inStringDoubleQuote || !line[c].Equals(' '))
                    {
                        formatted += line[c];
                    }
                }

                List<String> parts = new List<string>();

                inStringSingleQuote = false;
                inStringDoubleQuote = false;

                String[] split = formatted.Split(new String[] { "=>" }, StringSplitOptions.None);
                String currentPart = "";
                for (int c = 0; c < split[1].Length; c++)
                {
                    char current = split[1][c];
                    char? last = c > 1 ? split[1][c - 1] : (char?) null;

                    if (current.Equals('\'') && !(last.Equals('\\') || inStringDoubleQuote)){
                        inStringSingleQuote = !inStringSingleQuote;
                    } else if (current.Equals('"') && !(last.Equals('\\') || inStringSingleQuote))
                    {
                        inStringDoubleQuote = !inStringDoubleQuote;
                    }

                    if (!inStringSingleQuote && !inStringDoubleQuote && split[1][c].Equals('+'))
                    {
                        parts.Add(preparePart(currentPart));
                        currentPart = "";
                    } else
                    {
                        currentPart += split[1][c];
                    }

                }

                parts.Add(preparePart(currentPart));

                addGrammerRule(split[0], parts.ToArray());
            }
            
            foreach(KeyValuePair<String, List<String[]>> element in defaults)
            {
                foreach(String[] expression in element.Value)
                {
                    addGrammerRule(element.Key, expression);
                }
            }
        }

        private String preparePart(String part)
        {
            if (part.StartsWith("\"") && part.EndsWith("\"")) part = part.Substring(1, part.Length - 2);
            if (part.StartsWith("\'") && part.EndsWith("\'")) part = part.Substring(1, part.Length - 2);

            return part.Replace("\\\"", "\"").Replace("\\\'", "\'").Replace("\\\\", "\\");
        }

        private void addGrammerRule(String nonterminal, String[] expression)
        {
            if (!grammer.ContainsKey(nonterminal)) grammer.Add(nonterminal, new List<string[]>());
            grammer[nonterminal].Add(expression);
        }

        public List<State>[] Parse(String input)
        {
            List<State>[] stateSets = new List<State>[input.Length + 1];
            for (int i = 0; i <= input.Length; i++)
            {
                stateSets[i] = new List<State>();
            }

            String start = grammer.Keys.First();
            foreach (String[] rule in grammer[start])
            {
                stateSets[0].Add(new State(start, rule, 0, 0));
            }

            for (int i=0; i< stateSets.Length; i++) {
                int k = 0;
                while (k < stateSets[i].Count)
                {
                    State state = stateSets[i][k];
                    k++;

                    if (state.isFinished())
                    {
                        stateSets[i].AddRange(Complete(state, stateSets));
                    } else if (!grammer.Keys.Contains(state.nextSymbol()))
                    {
                        Scan(state, i, input, stateSets);
                    } else
                    {
                        List<State> newStates = Predict(state, i);
                        foreach (State newState in newStates)
                        {
                            if (!stateSets[i].Contains(newState))
                            {
                                stateSets[i].Add(newState);
                            }
                        }
                        
                    }
                }
            }
            return stateSets;
        }

        private List<State> Complete(State state, List<State>[] StateSets)
        {
            List<State> stateSet = StateSets[state.getOrigin()];
            List<State> parents = new List<State>();

            foreach (State potentialParent in stateSet)
            {
                if (potentialParent.nextSymbol() != state.getNonterminal()) continue;
                parents.Add(potentialParent.nextState());
            }
            return parents;
        }

        private void Scan(State state, int i, String input, List<State>[] StateSets)
        {
            int length = state.nextSymbol().Length;
            if (i + length - 1 >= input.Length) return;
            if (!input.Substring(i, length).Equals(state.nextSymbol())) return;
            StateSets[i + length].Add(state.nextState());
        }

        private List<State> Predict(State state, int i)
        {
            List<State> items = new List<State>();
            List<String[]> rules = new List<string[]>();
            if (!grammer.TryGetValue(state.nextSymbol(), out rules)) return items;
            foreach (String[] rule in rules)
            {
                items.Add(new State(state.nextSymbol(), rule, 0, i));
            }
            return items;
        }

        public Boolean IsValid(List<State>[] stateSet)
        {
            foreach (State state in stateSet[stateSet.Length - 1])
            {
                if (state.isFinished() && state.getOrigin() == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public List<State>[] FilterAndReverse(List<State>[] stateSets)
        {
            List<State>[] ret = new List<State>[stateSets.Length];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = new List<State>();
            }

            for (int destination = 0; destination < stateSets.Length; destination++)
            {
                foreach (State state in stateSets[destination])
                {
                    if (!state.isFinished()) continue;
                    int origin = state.getOrigin();
                    ret[origin].Add(new State(state.getNonterminal(), state.getExpression(), state.getDot(), origin, destination));
                }
            }

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i].Reverse();
            }
            return ret;
        }

        private List<Edge> DepthFirstSearch<Node, Edge>(
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
            Stack<Node> stack = new Stack<Node>();
            Dictionary<Node, KeyValuePair<Node, Edge>> parent = new Dictionary<Node, KeyValuePair<Node, Edge>>();
            //HashSet<Node> seen = new HashSet<Node>();

            stack.Push(root);
            //seen.Add(root);

            for (int depth = 0; stack.Count > 0; depth++)
            {
                Node v = stack.Pop();
                List<Edge> us = edges(depth, v);
                if (us.Count == 0) depth--;
                foreach (Edge u in us)
                {
                    Node w = child(depth, u);
                    //if (!seen.Contains(w))
                    //{
                        parent[w] = new KeyValuePair<Node, Edge>(v, u);

                        if (pred(depth + 1, w))
                        {
                            List<Edge> path = new List<Edge>();

                            KeyValuePair<Node, Edge> p = parent[w];
                            do
                            {
                                path.Add(p.Value);
                            } while (parent.TryGetValue(p.Key, out p));

                            path.Reverse();
                            return path;
                        }

                        //seen.Add(w);
                        stack.Push(w);

                    //}
                }
            }

            // Something went very wrong.
            return new List<Edge>();
        }

        private List<State> getSubEdges(String input, List<State>[] stateSets, State state)
        {
            String[] symbols = state.getExpression();
            int bottom = symbols.Length;
            int destination = state.getDestination();

            // Node => int origin / index of node in stateSets
            // Edge => State
            return DepthFirstSearch<int, State>(
                //a function that given a depth and a node, will return a list of edges.
                //Func < int, Node, List < Edge >> edges
                (depth, origin) => {
                    if (depth >= symbols.Length)
                    {
                        return new List<State>();
                    }
                    String part = symbols[depth];
                    if (!grammer.ContainsKey(part))
                    {
                        // terminal
                        if (!input.Substring(origin).StartsWith(part)) return new List<State>();
                        return new List<State>() { new State(null, new string[1] { part }, 1, origin, origin + part.Length) };
                    }
                    else
                    {
                        // nonterminal
                        List<State> states = stateSets[origin].FindAll((s) => { return s.getNonterminal().Equals(part); });
                        return states;
                    }
                },

                //a function that given a depth and an edge, will return a node.
                //Func < int, Edge, Node > child
                (depth, edge) => {
                    return edge.getDestination();
                },

                //a function that given a depth and a node, will tell you if the node is a leaf.
                //Func < int, Node, bool > pred
                (depth, origin) => {
                    return depth == bottom && origin == destination;
                },

                //a node. This will be the starting point of the search.
                //Node root
                state.getOrigin()
            );
        }


        public TreeNode<String> parse_tree(String input, List<State>[] stateSets)
        {
            int finish = stateSets.Length - 1;
            String name = grammer.Keys.First();

            State startState = stateSets[0].Find((state) => { return state.getDestination() == finish && state.getNonterminal().Equals(name); });

            return rec_parse_Tree(input, stateSets, 0, startState);

        }

        public TreeNode<String> rec_parse_Tree(String input, List<State>[] stateSets, int origin, State edge)
        {
            if (edge.isTerminal())
            {
                return new TreeNode<String>(edge.getExpression()[0]);
            }
            else
            {
                TreeNode<String> parent = new TreeNode<String>(edge.getNonterminal());
                List<State> subEdges = getSubEdges(input, stateSets, edge);
                if (subEdges == null) return parent;

                List<TreeNode<String>> children = subEdges.ConvertAll((state) =>
                {
                    return rec_parse_Tree(input, stateSets, origin, state);
                });
                parent.AddChildren(children);
                return parent;

            }
        }
    }
}
