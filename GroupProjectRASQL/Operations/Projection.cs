using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupProjectRASQL.Parser;

namespace GroupProjectRASQL.Operations
{
    class Projection : Operation
    {
        private List<String> fields;

        public Projection(TreeNode<string> parameter) : base(parameter) {
            this.fields = new List<String>(parameter.TreeToString().Split(','));
        }

        public List<String> getFields()
        {
            return fields;
        }

        public void setFields(List<String> fields)
        {
            this.fields = fields;
        }
    }
}
