using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupProjectRASQL.Parser;

namespace GroupProjectRASQL.Operations
{
    class Cartesian : Join
    {
        public Cartesian() : base(new TreeNode<String>("")){}

        public override string ToString(int depth = 0)
        {
            return "[" + this.GetType().Name + "]";
        }

        public override string ToJSON()
        {
            return "{'type':'" + "X" + "', 'properties': ''}";
        }
    }
    class Union : Operation
    {
        public override string ToJSON() {
            return "{'type':'" + "∪" + "', 'properties': ''}";
        }
    }
    class Intersect : Operation
    {
        public override string ToJSON() {
            return "{'type':'" + "∩" + "', 'properties': ''}";
        }
    }
    class Difference : Operation
    {
        public override string ToJSON() {
            return "{'type':'" + "-" + "', 'properties': ''}";
        }
    }
}
