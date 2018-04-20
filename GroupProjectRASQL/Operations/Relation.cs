using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Operations
{
    class Relation : Operation
    {
        private String name;

        public Relation(string name) => this.name = name;

        public override string ToString()
        {
            return "[Relation " + name + "]";
        }
    }
}
