using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Parser
{
    //A state represents a full or partial parse of a substring of the string given to the parser
    class State : IEquatable<State>
    {

        //The non terminal of a rule you are trying to find a valid parse for
        private String nonterminal;

        //The rest of the rule you are trying to find a valid parse for
        private String[] expression;

        //How many expressions we have found a valid parse for so far
        private int dot;

        //The starting position of the substring we're parsing
        private int origin;

        //The ending position of the substring we're parsing. Not initialised until the state is finished
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

        //Returns whetehr this state represents a terminal symbol
        public bool isTerminal()
        {
            return nonterminal == null;
        }


        //returns true if this state is equal to oher
        public bool Equals(State other)
        {
            return null != other && nonterminal == other.nonterminal && expression == other.expression && dot == other.dot && origin == other.origin && destination == other.destination;
        }

        //Return true if we have found valid parse for every expression
        public bool isFinished()
        {
            return this.dot >= this.expression.Length;
        }

        //Get next symbol after dot
        public String nextSymbol()
        {
            if (isFinished()) return null;
            return this.expression[this.dot];
        }

        //Return a new state that has the dot one place to the right
        public State nextState()
        {
            return new State(nonterminal, expression, dot + 1, origin);
        }

        //Get the state as a string. For debigging purposes
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
