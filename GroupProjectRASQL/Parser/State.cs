using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Parser
{
    class State : IEquatable<State>
    {
        private String nonterminal;
        private String[] expression;
        private int dot;
        private int origin;
        private int? destination;

        public State(String nonterminal, String[] expression, int dot = 0, int origin = 0, int? destination = null)
        {
            this.nonterminal = nonterminal;
            this.expression = expression;
            this.dot = dot;
            this.origin = origin;
            this.destination = destination;
        }

        public int getOrigin()
        {
            return origin;
        }
        
        public int getDestination()
        {
            return (int) destination;
        }

        public String getNonterminal()
        {
            return nonterminal;
        }

        public bool isTerminal()
        {
            return nonterminal == null;
        }

        public bool Equals(State other)
        {
            return null != other && nonterminal == other.nonterminal && expression == other.expression && dot == other.dot && origin == other.origin && destination == other.destination;
        }

        public bool isFinished()
        {
            return this.dot >= this.expression.Length;
        }

        public String nextSymbol()
        {
            if (isFinished()) return null;
            return this.expression[this.dot];
        }

        public State nextState()
        {
            return new State(nonterminal, expression, dot + 1, origin);
        }

        public override string ToString()
        {
            String before = dot > 0 ? expression.SubArray(0, dot).Aggregate((merge, next) => merge += next): "";
            String after = dot <= expression.Length-1 ? expression.SubArray(dot, expression.Length - dot).Aggregate((merge, next) => merge += next) : "";
            return nonterminal + "\t=>\t" + before + "•" + after + "\t(" + (destination == null ? origin : destination) + ")";
        }

        public string[] getExpression()
        {
            return expression;
        }

        public int getDot()
        {
            return dot;
        }
    }

    static class ArrayExtension
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
