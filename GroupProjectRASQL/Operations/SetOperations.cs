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
    }
    class Union : Operation { }
    class Intersect : Operation { }
    class Difference : Operation { }
}
