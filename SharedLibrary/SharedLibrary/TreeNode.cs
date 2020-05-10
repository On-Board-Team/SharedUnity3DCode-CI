using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class TreeNode<TElement> : IEnumerable<TreeNode<TElement>>
    {
        public ObservableCollection<TreeNode<TElement>> Children { get; set; }
        public TElement Value { get; set; }

        public TreeNode<TElement> Parent { get; set; }
        bool HasChildren { get { return Children.Count > 0; } }
        bool IsMostLeftChild { get { return Parent != null && Parent.Children.IndexOf(this) == 0; } }
        bool IsMostRightChild { get { return Parent != null && Parent.Children.IndexOf(this) == Parent.Children.Count - 1; } }

        private TreeNode() { Children = new ObservableCollection<TreeNode<TElement>>(); }
        public TreeNode(TElement content)
        {
            Value = content;
            Children = new ObservableCollection<TreeNode<TElement>>();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is TreeNode<TElement>))
                return false;

            TreeNode<TElement> other = obj as TreeNode<TElement>;
            if (!Value.Equals(other.Value))
                return false;

            if (Count != other.Count)
                return false;

            for (int i = 1; i < this.Count; i++)
            {
                if (!(this[i].Equals(other[i])))
                    return false;
            }

            return true;
        }
        public TreeNode<TElement> EmptyCopy()
        {
            var node = new TreeNode<TElement>();
            foreach (var child in Children)
            {
                var duplicatedChildNode = child.EmptyCopy().LinkToParent(node);
            }

            return node;
        }
        public int IndexOf(TElement tElement)
        {
            int index = -1;
            foreach (var item in this)
            {
                index++;
                if (item == null && tElement == null)
                    return index;
                else if (item != null)
                    if (item.Value.Equals(tElement))
                        return index;
            }
            return -1;
        }
        public TreeNode<TElement> LinkToParent(TreeNode<TElement> parentNode)
        {
            DetachFromParent();
            Parent = parentNode;
            Parent.Children.Add(this);

            return this;
        }
        public List<TreeNode<TElement>> LeafsOnlyList()
        {
            List<TreeNode<TElement>> leafs = new List<TreeNode<TElement>>();

            foreach (var child in this)
            {
                if (child.Children.Count == 0)
                    leafs.Add(child);
            }

            return leafs;
        }

        public IEnumerator<TreeNode<TElement>> GetEnumerator()
        {
            yield return this;
            foreach (var child in Children)
            {
                foreach (var nestedChild in child)
                {
                    yield return nestedChild;
                }
            }
        }
        public int Count
        {
            get
            {
                int count = 0;
                foreach (var item in this)
                {
                    count++;
                }
                return count;
            }
        }
        public TreeNode<TElement> this[int index]
        {
            get
            {
                if (index < 0)
                    throw new IndexOutOfRangeException();
                int current = -1;
                foreach (var item in this)
                {
                    current++;
                    if (current == index)
                        return item;
                }

                //Index is bigger than Count
                throw new IndexOutOfRangeException();
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void DetachFromParent()
        {
            if (Parent == null)
                return;

            if (Parent.Children.Contains(this))
                Parent.Children.Remove(this);

            Parent = null;
        }

        public TreeNode<TElement> GetNextLeaf()
        {
            return GetNextLeaf(false);
        }
        public TreeNode<TElement> GetPreviousLeaf()
        {
            return GetPreviousLeaf(false);
        }
        private TreeNode<TElement> DeepestLeftChild()
        {
            if (!HasChildren)
                return null;

            TreeNode<TElement> current = this;
            while (current.HasChildren)
                current = current.Children[0];

            return current;
        }
        private TreeNode<TElement> DeepestRightChild()
        {
            if (!HasChildren)
                return null;

            TreeNode<TElement> current = this;
            while (current.HasChildren)
                current = current.Children.Last();

            return current;
        }
        private TreeNode<TElement> GetNextLeaf(bool hasCheckedChildren)
        {
            if (!hasCheckedChildren && HasChildren)
            {
                return DeepestLeftChild();
            }

            if (Parent == null)
                return null;

            if (!IsMostRightChild)
            {
                int index = Parent.Children.IndexOf(this);
                TreeNode<TElement> rightSameLevel = Parent.Children[index + 1];
                TreeNode<TElement> child = rightSameLevel.DeepestLeftChild();

                return child == null ? rightSameLevel : child;
            }

            return Parent.GetNextLeaf(true);
        }
        private TreeNode<TElement> GetPreviousLeaf(bool hasCheckedChildren)
        {
            if (!hasCheckedChildren && HasChildren)
            {
                return DeepestRightChild();
            }

            if (Parent == null)
                return null;

            if (!IsMostLeftChild)
            {
                int index = Parent.Children.IndexOf(this);
                TreeNode<TElement> leftSameLevel = Parent.Children[index - 1];
                TreeNode<TElement> child = leftSameLevel.DeepestRightChild();

                return child == null ? leftSameLevel : child;
            }

            return Parent.GetPreviousLeaf(true);
        }

        public override int GetHashCode()
        {
            var hashCode = -571848642;
            hashCode = hashCode * -1521134295 + EqualityComparer<ObservableCollection<TreeNode<TElement>>>.Default.GetHashCode(Children);
            hashCode = hashCode * -1521134295 + EqualityComparer<TElement>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + EqualityComparer<TreeNode<TElement>>.Default.GetHashCode(Parent);
            hashCode = hashCode * -1521134295 + HasChildren.GetHashCode();
            hashCode = hashCode * -1521134295 + IsMostLeftChild.GetHashCode();
            hashCode = hashCode * -1521134295 + IsMostRightChild.GetHashCode();
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            return hashCode;
        }
    }
}
