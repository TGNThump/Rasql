using GroupProjectRASQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Operations
{
    public abstract class Operation
    {

        public Operation(TreeNode<String> parameter = null)
        {
            this.parameter = parameter;
        }

        public TreeNode<String> parameter;

        public override string ToString()
        {
            if (parameter == null)
            {
                return "[" + this.GetType().Name + "]";
            } else
            {
                return "[" + this.GetType().Name + "](" + parameter.TreeToString() + ")";
            }
        }

        public virtual string ToJSON()
        {
            return "{'type':'" + this.GetType().Name + "', 'properties': ''}";
        }

        public virtual IEnumerable<String> getFieldNames()
        {
            return new List<String>();
        }

        public virtual void setFieldName(String oldName, String newName){
            throw new NotImplementedException(GetType().Name);
        }
    }
}
