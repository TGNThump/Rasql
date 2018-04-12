using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GroupProjectRASQL.Parser
{
    public class TreeNode<T> : IEnumerable<TreeNode<T>>
    {
        public T Data { get; set; }
        public TreeNode<T> Parent { get; set; }
        public ICollection<TreeNode<T>> Children { get; set; }

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


        public TreeNode(T data)
        {
            this.Data = data;
            this.Children = new LinkedList<TreeNode<T>>();

            this.ElementsIndex = new LinkedList<TreeNode<T>>();
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

            Children.Add(node);

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

        public TreeNode<T> Child(int i) {

            if (i >= Children.Count || i < 0) return null;
            else return Children.ElementAt(i);

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
