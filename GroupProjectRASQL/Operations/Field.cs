using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupProjectRASQL.Schema
{
    public class Field
    {
        public String name { get; set; }
        public IList<String> values { get; set; }

        public Field(String name, IList<String> values)
        {
            this.name = name;
            this.values = values;
        }

        public int getCount()
        {
            return values.Count;
        }

        public int getDistinctCount()
        {
            return values.Distinct().Count();
        }
    }
}