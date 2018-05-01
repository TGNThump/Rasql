using GroupProjectRASQL.Parser;
using GroupProjectRASQL.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectRASQL.Operations
{
    public class Relation : Operation
    {
        public String name { get; set; }
        public IList<Field> fields { get; set; }

        public Relation(String name, IList<Field> fields)
        {
            this.name = name;
            this.fields = fields;
        }
        public Field GetField(String name)
        {
            return fields.FirstOrDefault(field => field.name == name);
        }
        public override IEnumerable<String> getFieldNames()
        {
            return fields.Select(field => field.name);
        }

        public IEnumerable<String> getFullFieldNames() {


            return fields.Select(field => name+"."+field.name);

        }

        public override string ToString()
        {
            return "[Relation " + name + "]{" + getFieldNames().Aggregate((all, next) => all + ", " + next) + "}";
        }
    }
}
