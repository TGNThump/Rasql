using GroupProjectRASQL.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GroupProjectRASQL.Parser
{
    public class TreeNode<T> : IEnumerable<TreeNode<T>>
    {
        public T Data { get; set; }
        public TreeNode<T> Parent { get; set; }
        public IList<TreeNode<T>> Children { get; set; }

        public Boolean IsRoot
        {
            get { return Parent == null; }
        }

        public Boolean IsLeaf
        {
            get { return Children.Count == 0; }
        }

        public int Level
        {
            get
            {
                if (this.IsRoot)
                    return 0;
                return Parent.Level + 1;
            }
        }

        internal void RemoveChild(TreeNode<T> child)
        {
            this.Children.Remove(child);
            this.ElementsIndex.Remove(child);
        }

        public TreeNode(T data)
        {
            this.Data = data;
            this.Children = new List<TreeNode<T>>();

            this.ElementsIndex = new List<TreeNode<T>>();
            this.ElementsIndex.Add(this);
        }

        public TreeNode<T> AddChildren(IEnumerable<TreeNode<T>> children)
        {
            foreach (TreeNode<T> child in children) AddChild(child);
            return this;
        }

        public TreeNode<T> AddChild(T child)
        {
            TreeNode<T> childNode = new TreeNode<T>(child);
            return AddChild(childNode);
        }

        public TreeNode<T> AddChild(TreeNode<T> childNode)
        {
            childNode.Parent = this;

            this.Children.Add(childNode);

            this.RegisterChildForSearch(childNode);

            return childNode;
        }

        public void RemoveChildren()
        {
            this.Children.Clear();
            this.ElementsIndex.Clear();
            this.ElementsIndex.Add(this);
        }

        public void ForEach(Func<TreeNode<T>, TreeNode<T>> action)
        {
            TreeNode<T> temp = action(this);
            this.Data = temp.Data;
            this.Parent = temp.Parent;
            this.Children = temp.Children;
            for (int i = 0; i < this.Children.Count; i++)
                Children.ElementAt(i).ForEach(action);
        }

        public TreeNode<T> getNextNode()
        {
            TreeNode<T> last = this;
            if (last.Children.Count() > 0) return last.Child();
            while (!last.IsRoot)
            {
                int indexInParent = last.Parent.Children.IndexOf(last);
                if (indexInParent+1 < last.Parent.Children.Count)
                {
                    return last.Parent.Children.ElementAt(indexInParent + 1);
                } else
                {
                    last = last.Parent;
                }
            }
            return null;
        }

        public TreeNode<T> ForEach(Action<TreeNode<T>> action, bool stepping = true )
        {
            action(this);
            if (!stepping)
            {
                for (int i=0; i < this.Children.Count; i++)
                {
                    Children.ElementAt(i).ForEach(action,false);
                }
                return null;

            }
            else
            {
                forEachStore(action);
                return this;
                
                
            }
        }

        private Action<TreeNode<T>> currentAction;
        
        public void forEachStore(Action<TreeNode<T>> action)
        {
            this.currentAction = action;
            foreach ( TreeNode<T> child in this.Children )
            {
                child.currentAction = action;
            }
        }


        public bool step()
        {
            Console.WriteLine("Step");
            Console.WriteLine(this);

            for (int i = 0; i < this.Children.Count; i++)
            {
                Children.ElementAt(i).ForEach(this.currentAction, true);
            }
            return true;
        }
        public bool stepToEnd()
        {
            Console.WriteLine("toEnd");

            ForEach(this.currentAction, false);
            return true;



        }

        public string ToJSON()
        {
            MethodInfo toJSON = Data.GetType().GetMethod("ToJSON");
            String json = "{'data': " + ((string) toJSON.Invoke(Data, new object[0])) + ", 'children': [";
            foreach(TreeNode<T> child in Children)
            {
                json += child.ToJSON() + ",";
            }
            if (Children.Count > 0) json = json.Substring(0, json.Length - 1);

            json += "]}";
            return json;
        }

        public override string ToString()
        {
            return Data != null ? Data.ToString() : "[data null]";
        }


        #region searching

        private ICollection<TreeNode<T>> ElementsIndex { get; set; }

        private void RegisterChildForSearch(TreeNode<T> node)
        {
            ElementsIndex.Add(node);
            if (Parent != null)
                Parent.RegisterChildForSearch(node);
        }

        public TreeNode<T> FindTreeNode(Func<TreeNode<T>, bool> predicate)
        {
            return this.ElementsIndex.FirstOrDefault(predicate);
        }


        //Allows for custom collection initialiser
        public void Add(TreeNode<T> node) {
            if (node == null) return;
            AddChild(node);
        }

        //Outputs string subtree represents
        public String TreeToString() {

            if (Children.Count() == 0) return ToString();

            String childrenString = "";

            foreach (TreeNode<T> node in Children) {

                childrenString += node.TreeToString();

            }

            return childrenString;

        }

        public String TreeToDebugString(IList<T> squish = null, int depth = 0)
        {
            squish = squish ?? new List<T>();
            String ret = "";
            for (int i = 0; i < depth; i++) ret += "&nbsp;&nbsp;&nbsp;&nbsp;";
            if (squish.Contains(Data)) return ret + TreeToString() + "<br />";

            MethodInfo toString = Data.GetType().GetMethod("ToString", new Type[] { typeof(int) });

            ret +=  (toString != null ? toString.Invoke(Data, new object[]{ depth }) : Data.ToString()) + (Children.Count == 0 ? "<br/>" : "{<br />");
            if (Children.Count == 0) return ret; // TEMPish
            foreach (TreeNode<T> child in Children) ret += child.TreeToDebugString(squish, depth + 1);
            for (int i = 0; i < depth; i++) ret += "&nbsp;&nbsp;&nbsp;&nbsp;";
            ret += "}<br />";
            return ret;
        }

        public TreeNode<T> Child(int i) {

            if (i >= Children.Count || i < 0) return null;
            else return Children.ElementAt(i);

        }

        public TreeNode<T> Child()
        {
            return Child(0);
        }

        #endregion


        #region iterating

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            yield return this;
            foreach (var directChild in this.Children)
            {
                foreach (var anyChild in directChild)
                    yield return anyChild;
            }
        }

        #endregion
    }
}
