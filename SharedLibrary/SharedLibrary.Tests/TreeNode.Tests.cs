using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace SharedLibrary.UnitTests
{
    [TestClass]
    public class TreeNodeTests
    {
        [TestMethod]
        public void CountTest()
        {
            TreeNode<int> root = GetSample1();
            Assert.AreEqual(7, root.Count);

            root = GetSample2();
            Assert.AreEqual(10, root.Count);
        }
        [TestMethod]
        public void Check_Children()
        {
            TreeNode<int> root = GetSample1();


            Assert.AreEqual(4, root.Children.Count);
            foreach (var child in root.Children)
            {
                Assert.AreEqual(root, child.Parent);
            }
        }
        [TestMethod]
        public void Check_LeafsOnlyList()
        {
            TreeNode<int> root = GetSample1();

            List<TreeNode<int>> leafsOnly = root.LeafsOnlyList();
            Assert.AreEqual(5, leafsOnly.Count);

            int count = 0;
            foreach (var child in root)
            {
                count++;
            }

            Assert.AreEqual(7, count);
        }
        [TestMethod]
        public void GetNext_Test1()
        {
            TreeNode<int> root = GetSample1();

            List<TreeNode<int>> leafs = root.LeafsOnlyList();
            int count = -1;
            TreeNode<int> current = root;
            for (int j = 0; j < leafs.Count; j++)
            {
                count++;
                current = current.GetNextLeaf();

                Assert.AreEqual(leafs[j], current);
            }

            Assert.IsNull(current.GetNextLeaf());
        }

        [TestMethod]
        public void GetNext_Test2()
        {
            TreeNode<int> root = GetSample2();

            List<TreeNode<int>> leafs = root.LeafsOnlyList();
            int count = -1;
            TreeNode<int> current = root;
            for (int j = 0; j < leafs.Count; j++)
            {
                count++;
                current = current.GetNextLeaf();

                Assert.AreEqual(leafs[j], current);
            }

            Assert.IsNull(current.GetNextLeaf());
        }

        [TestMethod]
        public void GetPrevious_Test1()
        {
            TreeNode<int> root = GetSample1();

            List<TreeNode<int>> leafs = root.LeafsOnlyList();

            TreeNode<int> current = leafs.Last();
            for (int i = leafs.Count - 2; i >= 0; i--)
            {
                current = current.GetPreviousLeaf();

                Assert.AreEqual(leafs[i], current);
            }
            Assert.IsNull(current.GetPreviousLeaf());
        }


        [TestMethod]
        public void GetPrevious_Test2()
        {
            TreeNode<int> root = GetSample2();

            List<TreeNode<int>> leafs = root.LeafsOnlyList();

            TreeNode<int> current = leafs.Last();
            for (int i = leafs.Count - 2; i >= 0; i--)
            {
                current = current.GetPreviousLeaf();

                Assert.AreEqual(leafs[i], current);
            }
            Assert.IsNull(current.GetPreviousLeaf());
        }
        [TestMethod]
        public void Equality_Check()
        {
            TreeNode<int> root = GetSample1();
            TreeNode<int> root2 = GetSample1();

            Assert.AreEqual(root, root2);

            root[0].Value++;
            Assert.AreNotEqual(root, root2);

            root[0].Value--;
            Assert.AreEqual(root, root2);

            new TreeNode<int>(37).LinkToParent(root2);
            Assert.AreNotEqual(root, root2);
        }
        [TestMethod]
        public void DuplicationCheck()
        {
            TreeNode<int> root = new TreeNode<int>(0);
            new TreeNode<int>(1).LinkToParent(root);
            new TreeNode<int>(2).LinkToParent(root);


            TreeNode<int> root2 = new TreeNode<int>(0);
            new TreeNode<int>(1).LinkToParent(root2);
            new TreeNode<int>(2).LinkToParent(root2);
            /*
             root=>1, root>=2 | root2=>1, root2=>2
             */
            Assert.AreEqual(root, root2);


            new TreeNode<int>(3).LinkToParent(root);
            new TreeNode<int>(3).LinkToParent(root2[1]);
            Assert.AreNotEqual(root, root2);

            new TreeNode<int>(3).LinkToParent(root[1]);
            new TreeNode<int>(3).LinkToParent(root2);
            Assert.AreEqual(root, root2);
        }
        [TestMethod]
        public void FirstOrDefault_IntTest()
        {
            var root = new TreeNode<int>(0);
            for (int i = 1; i < 10; i++)
            {
                new TreeNode<int>(i).LinkToParent(root);
            }

            int elementTest = 7;
            Assert.AreEqual<int>(elementTest, root.IndexOf(elementTest));
        }
        [TestMethod]
        public void Where_Check()
        {
            var root = new TreeNode<int>(0);
            for (int i = 1; i < 10; i++)
            {
                new TreeNode<int>(i).LinkToParent(root);
            }

            int elementTest = 7;
            Assert.AreEqual<int>(7, root.FirstOrDefault(val => val.Value == elementTest).Value);
        }
        private TreeNode<int> GetSample1()
        {
            TreeNode<int> root = new TreeNode<int>(0);
            /*
            root
           / | | \
          1  2 3  4
         / \
        5   6
             */
            new TreeNode<int>(1).LinkToParent(root);
            new TreeNode<int>(2).LinkToParent(root);
            new TreeNode<int>(3).LinkToParent(root);
            new TreeNode<int>(4).LinkToParent(root);

            new TreeNode<int>(5).LinkToParent(root.Children[0]);
            new TreeNode<int>(6).LinkToParent(root.Children[0]);

            return root;
        }
        private TreeNode<int> GetSample2()
        {
            TreeNode<int> root = new TreeNode<int>(0);

            /*
           root
             |
             a
            / \   \    \
           b   d   5    6
          /   / \
         c   e   f
                /
               g
             */
            TreeNode<int> a = new TreeNode<int>(1).LinkToParent(root);
            var b = new TreeNode<int>(2).LinkToParent(a);
            var c = new TreeNode<int>(3).LinkToParent(b);

            var d = new TreeNode<int>(4).LinkToParent(a);
            var e = new TreeNode<int>(5).LinkToParent(d);
            var f = new TreeNode<int>(6).LinkToParent(d);
            var g = new TreeNode<int>(7).LinkToParent(f);

            new TreeNode<int>(5).LinkToParent(root.Children[0]);
            new TreeNode<int>(6).LinkToParent(root.Children[0]);

            return root;
        }
    }
}
