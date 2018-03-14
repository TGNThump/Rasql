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

        List<State>[] StateSets;

        public List<State>[] Parse(String input)
        {
            StateSets = new List<State>[input.Length + 1];
            for (int i = 0; i <= input.Length; i++)
            {
                StateSets[i] = new List<State>();
            }

            String start = grammer.Keys.First();
            foreach (String[] rule in grammer[start])
            {
                StateSets[0].Add(new State(start, rule, 0, 0));
            }

            for (int i=0; i< StateSets.Length; i++) {
                int k = 0;
                while (k < StateSets[i].Count)
                {
                    State state = StateSets[i][k];
                    k++;

                    if (state.isFinished())
                    {
                        StateSets[i].AddRange(Complete(state));
                    } else if (!grammer.Keys.Contains(state.nextSymbol()))
                    {
                        Scan(state, i, input);
                    } else
                    {
                        List<State> newStates = Predict(state, i);
                        foreach (State newState in newStates)
                        {
                            if (!StateSets[i].Contains(newState))
                            {
                                StateSets[i].Add(newState);
                            }
                        }
                        
                    }
                }
            }
            return StateSets;
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

        private List<State> Complete(State state)
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

        private void Scan(State state, int i, String input)
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
