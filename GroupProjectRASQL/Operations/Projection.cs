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

        public Projection(IEnumerable<String> parameter) {
            this.fields = new List<String>(parameter);
        }

        public Projection(TreeNode<string> parameter) : base(parameter) {
            this.fields = new List<String>(parameter.TreeToString().Split(','));
        }

        

        public override IEnumerable<String> getFieldNames()
        {
            return fields;
        }

        public override void setFieldName(string oldName, string newName)
        {
            for(int i = 0; i < fields.Count; i++)
            {
                String field = fields[i];
                if (field.Equals(oldName))
                {
                    fields.RemoveAt(i);
                    fields.Add(newName);
                    return;
                }
            }
        }

        public override string ToString()
        {
           return "[" + this.GetType().Name + "](" + fields.Aggregate((a,b) => a + ", " + b) + ")";
        }

        public override string ToJSON()
        {
            return "{'type':'" + this.GetType().Name + "', 'properties': '" + fields.Aggregate((a, b) => a + ", " + b) + "'}";
        }
    }
}
