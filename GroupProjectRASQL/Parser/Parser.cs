using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truncon.Collections;

namespace GroupProjectRASQL.Parser
{
    //A class definining an parse that works by the Rarley algorithm
    class Parser
    {
        //A dictionary to contain the grammar
        OrderedDictionary<String, List<String[]>> grammar = new OrderedDictionary<string, List<string[]>>();

        //Define some default grammar rules so they don't have to be specified in the grammar files
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

        //Create a parser for a given grammar
        public Parser(String type = "sql")
        {
            //Get the path to the grammar file, then get contents of file
            String path = @"..\..\..\bnf\" + type + ".lua";
            String[] lines = System.IO.File.ReadAllLines(path);
            
            //For each line in the file, add the rules to the grammar
            for(int i=0; i<lines.Length; i++)
            {
                String line = lines[i];

                //Skip line if empty
                if (line.Equals("")) continue;

                //Skip commented lines
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
                        //inStringSingleQuote = !inStringSingleQuote;
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

                addgrammarRule(split[0], parts.ToArray());
            }
            
            //Add default grammar rules to grammar
            foreach(KeyValuePair<String, List<String[]>> element in defaults)
            {
                foreach(String[] expression in element.Value)
                {
                    addgrammarRule(element.Key, expression);
                }
            }
        }

        private String preparePart(String part)
        {
            if (part.StartsWith("\"") && part.EndsWith("\"")) part = part.Substring(1, part.Length - 2);
            if (part.StartsWith("\'") && part.EndsWith("\'")) part = part.Substring(1, part.Length - 2);

            return part.Replace("\\\"", "\"").Replace("\\\'", "\'").Replace("\\\\", "\\");
        }

        //Add a rule to the grammar
        private void addgrammarRule(String nonterminal, String[] expression)
        {
            //If grammar does not already have a key for the nonterminal, add one
            if (!grammar.ContainsKey(nonterminal)) grammar.Add(nonterminal, new List<string[]>());

            //Add rule for the non terminal
            grammar[nonterminal].Add(expression);
        }

        //Returns a 2d array of states correspoding to full and partial parses of the input string 
        public List<State>[] Parse(String input)
        {

            //Create array of state lists
            List<State>[] stateSets = new List<State>[input.Length + 1];
            for (int i = 0; i <= input.Length; i++)
            {
                stateSets[i] = new List<State>();
            }

            //Add states corresponding to the starting rules to the stateSets
            String start = grammar.Keys.First();
            foreach (String[] rule in grammar[start])
            {
                stateSets[0].Add(new State(start, rule, 0, 0));
            }

            //Iterate through the statesets, adding more states when necessary
            for (int i=0; i< stateSets.Length; i++) {
                int k = 0;
                while (k < stateSets[i].Count)
                {
                    State state = stateSets[i][k];
                    k++;

                    //If the current state represents a full parse, complete the rule that generated it
                    if (state.isFinished())
                    {
                        stateSets[i].AddRange(Complete(state, stateSets));
                    }
                    //If a is the next symbol in the input stream, for every state in S(k) of the form (X → α • a β, j), add (X → α a • β, j) to S(k+1).
                    else if (!grammar.Keys.Contains(state.nextSymbol()))
                    {
                        Scan(state, i, input, stateSets);
                    }
                    //For every state in S(k) of the form(X → α • Y β, j), add(Y → • γ, k) to S(k) for every production in the grammar with Y on the left - hand side(Y → γ).
                    else
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
            if (!grammar.TryGetValue(state.nextSymbol(), out rules)) return items;
            foreach (String[] rule in rules)
            {
                items.Add(new State(state.nextSymbol(), rule, 0, i));
            }
            return items;
        }

        //Returns true if there is a valid parse tree in a given stateset
        public Boolean IsValid(List<State>[] stateSet)
        {
            foreach (State state in stateSet[stateSet.Length - 1])
            {
                if (state.getNonterminal() != grammar.Keys.First()) continue; 
                if (state.isFinished() && state.getOrigin() == 0)
                {
                    return true;
                }
            }

            return false;
        }

        //Reorders and removes unnecessary states from a state set to make it easier to construct a tree from
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

        private List<State> getSubEdges(String input, List<State>[] stateSets, State state)
        {
            String[] symbols = state.getExpression();
            int bottom = symbols.Length;
            int destination = state.getDestination();

            // Node => int origin / index of node in stateSets
            // Edge => State
            return DFS.DepthFirstSearch<int, State>(
                //a function that given a depth and a node, will return a list of edges.
                //Func < int, Node, List < Edge >> edges
                (depth, origin) => {
                    if (depth >= symbols.Length)
                    {
                        return new List<State>();
                    }
                    String part = symbols[depth];
                    if (!grammar.ContainsKey(part))
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

        //Returns a valid parse tree from the state sets
        public TreeNode<String> parse_tree(String input, List<State>[] stateSets)
        {
            int finish = stateSets.Length - 1;
            String name = grammar.Keys.First();

            State startState = stateSets[0].Find((state) => { return state.getDestination() == finish && state.getNonterminal().Equals(name); });

            return rec_parse_Tree(input, stateSets, 0, startState);

        }

        //Recursive function to simplify code for parse_tree. Returns the sub tree connected to the given edge
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
