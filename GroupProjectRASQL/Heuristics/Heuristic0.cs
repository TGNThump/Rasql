using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupProjectRASQL.Operations;
using GroupProjectRASQL.Parser;
using Condition = GroupProjectRASQL.Parser.TreeNode<System.String>;
using Node = GroupProjectRASQL.Parser.TreeNode<GroupProjectRASQL.Operations.Operation>;

namespace GroupProjectRASQL.Heuristics
{
    public static class Extensions{
        public static void AddSingle<K, V>(this IDictionary<K, IList<V>> dictionary, K key, V value)
        {
            if (!dictionary.ContainsKey(key)) dictionary.Add(key, new List<V>());
            dictionary[key].Add(value);
        }
    }

    public class Heuristic0 : Heuristic
    {
        public Heuristic0(Node root) : base(root){}

        public override bool Run(Node operation)
        {
            if (operation.Data is Relation) return false;
            if (operation.Data.getFieldNames().Count() == 0) return false;

            IEnumerable<Node> relations = operation.Where(node => node.Data is Relation);
            IDictionary<String, IList<String>> fields = new Dictionary<string, IList<string>>();

            foreach (Node relation in relations)
            {
                IDictionary<String, IList<String>> fieldslocal = new Dictionary<string, IList<string>>();
                String relationName = ((Relation)relation.Data).name;
                List<Node> path = new List<Node>();

                for (Node current = relation; !current.Equals(operation); current = current.Parent)
                {
                    path.Add(current);
                }

                //path.Reverse();

                foreach (Node node in path)
                {
                    if (node.Data is Relation)
                    {
                        foreach (String field in ((Relation)node.Data).getFieldNames())
                        {
                            fieldslocal.AddSingle(((Relation)node.Data).name, field);
                        }
                    }
                    else if (node.Data is RenameAttribute)
                    {
                        String oldName = ((RenameAttribute)node.Data).getOldName();
                        String newName = ((RenameAttribute)node.Data).getNewName();

                        if (oldName.Contains('.'))
                        {
                            String[] old = oldName.Split('.');
                            if (fieldslocal.ContainsKey(old[0]))
                            {
                                if (!fieldslocal[old[0]].Contains(old[1])) throw new HeuristicException("Field '" + old[1] + "' not found in Relation " + old[0]);
                                fieldslocal[old[0]].Remove(old[1]);
                                fieldslocal.AddSingle(old[0], newName);
                            }
                        }
                        else
                        {
                            // Convert Dictionary<relationName, List<fieldName>> to List<KeyValuePair<relationName,fieldName>>
                            IEnumerable<KeyValuePair<String, String>> fieldPairs = fieldslocal.ToList().SelectMany(key => key.Value, (key, value) => new KeyValuePair<String, String>(key.Key, value));
                            // Filter to when fieldName = oldName
                            IEnumerable<KeyValuePair<String, String>> filtered = fieldPairs.Where(p => p.Value.Equals(oldName));

                            if (filtered.Count() > 1) throw new HeuristicException("Field '" + oldName + "' found in multiple relations, please remove ambiguity.");
                            if (filtered.Count() == 0) continue;

                            KeyValuePair<String, String> pair = filtered.Single();

                            fieldslocal[pair.Key].Remove(pair.Value);
                            fieldslocal.AddSingle(pair.Key, newName);
                        }
                    }
                    else if (node.Data is RenameRelation)
                    {
                        String newName = ((RenameRelation)node.Data).getNewName();
                        if (fieldslocal.ContainsKey(newName)) throw new HeuristicException("Cannot rename to a relation name that already exists.");
                        if (fieldslocal.Values.Distinct().Count() != fieldslocal.Values.Count()) throw new HeuristicException("Cannot rename relation '" + newName + "' as a result of ambigious field names");
                        IList<String> values = fieldslocal.Values.Aggregate((a, b) => {
                            foreach (String value in b)
                            {
                                a.Add(value);
                            }
                            return a;
                        });
                        fieldslocal.Clear();
                        foreach (String value in values) fieldslocal.AddSingle(newName, value);
                    }
                }
            foreach (KeyValuePair<String, IList<String>> kvp in fieldslocal)
            {
                    if (!fields.ContainsKey(kvp.Key)) { fields.Add(kvp.Key, kvp.Value); }
            }

            }

            IEnumerable<String> fieldNames = operation.Data.getFieldNames().ToArray();
            foreach (String oldName in fieldNames)
            {

                if (oldName.Contains('.'))
                {
                    String[] split = oldName.Split('.');
                    if (!fields.ContainsKey(split[0])) throw new HeuristicException("Relation '" + split[0] + "' not found.");
                    if (!fields[split[0]].Contains(split[1])) throw new HeuristicException("Field '" + split[1] + "' not found in Relation " + split[0]);
                }
                else
                {
                    // Convert Dictionary<relationName, List<fieldName>> to List<KeyValuePair<relationName,fieldName>>
                    IEnumerable<KeyValuePair<String, String>> fieldPairs = fields.ToList().SelectMany(key => key.Value, (key, value) => new KeyValuePair<String, String>(key.Key, value));
                    // Filter to fieldnames
                    IEnumerable<KeyValuePair<String, String>> filtered = fieldPairs.Where(p => p.Value.Equals(oldName));

                    if (filtered.Count() > 1) throw new HeuristicException("Field '" + oldName + "' found in multiple relations, please remove ambiguity.");
                    if (filtered.Count() == 0) throw new HeuristicException("Field '" + oldName + "' not found in any relations.");

                    KeyValuePair<String, String> pair = filtered.Single();
                    String newName = pair.Key + "." + pair.Value;
                    operation.Data.setFieldName(oldName, newName);
                    Console.WriteLine("Replaced '" + oldName + "' with '" + newName + "'");
                }
            }

            return true;
        }
    }
}
