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
        Dictionary<String, List<String[]>> grammer = new Dictionary<string, List<string[]>>()
        {
            { "[Sum]", new List<string[]>(){
                new string[]{ "[Sum]", "+", "[Product]" },
                new string[]{ "[Sum]", "-", "[Product]" },
                new string[]{ "[Product]" }
            }},
            { "[Product]", new List<string[]>(){
                new string[]{ "[Product]", "*", "[Factor]" },
                new string[]{"[Product]", "/", "[Factor]" },
                new string[]{"[Factor]" }
            }},
            { "[Factor]", new List<String[]>(){
                new string[]{ "(", "[Sum]", ")" },
                new string[]{"[Number]" }
            }},
            { "[Number]", new List<String[]>(){
               new string[]{"[0-9]", "[Number]"},
               new string[]{"[0-9]"}
            }},
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
        };

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
                foreach(State state in stateSets[destination])
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

        // Node -> List<State> / StateSet
        // Edge -> State

        /*private List<State> DepthFirstSearch(
            Func<int,List<State>, List<State>> edges,
            Func<int, State, List<State>> child,
            Func<int, List<State>, bool> pred,
            List<State> root,
            int depth = 0,
            List<List<State>> explored = null,
            List<KeyValuePair<int, State>> path_so_far = null)
        {
            if (explored == null) explored = new List<List<State>>();
            if (path_so_far == null) path_so_far = new List<KeyValuePair<int, State>>();

            explored.Add(root);

            //If goal, return empty list.
            if (pred.Invoke(depth, root)) {

            }


        }*/

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
    }
}
