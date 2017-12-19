using DotBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBook.Processing
{
    public static class INodeExtensions
    {
        public static IEnumerable<INode<T>> Descendants<T>(this INode<T> root)
        {
            var nodes = new Stack<INode<T>>(new[] { root });
            while (nodes.Any())
            {
                var node = nodes.Pop();
                yield return node;
                foreach (var n in node.ChildrenNodes) nodes.Push(n);
            }
        }

        public static IEnumerable<INode<T>> Descendants<T>(this INode<T> root,
            Func<INode<T>, bool> predicate)
        {
            var nodes = new Stack<INode<T>>(new[] { root });
            while (nodes.Any())
            {
                var node = nodes.Pop();
                yield return node;
                foreach (var n in node.ChildrenNodes)
                    if (predicate(n)) nodes.Push(n);
            }
        }

        public static bool IsRoot<T>(this INode<T> node) =>
            node.ParentNode == null;

        public static bool IsLeaf<T>(this INode<T> node) =>
            node.ChildrenNodes == null || node.ChildrenNodes.Count() == 0;

        public static INode<T> GetRoot<T>(this INode<T> node)
        {
            var result = node;
            while (!result.IsRoot() || result == null)
                result = result.ParentNode;
            return result;
        }

        public static INode<T> LocateClosestRelative<T>(this INode<T> child,
            Func<INode<T>, bool> predicate, out INode<T> commonAncestor)
        {
            if (child.IsRoot())
            {
                commonAncestor = null;
                return null;
            }

            Stack<INode<T>> ancestors = new Stack<INode<T>>(new[] { child });
            var node = child;
            while (!node.IsRoot())
            {
                node = node.ParentNode;
                ancestors.Push(node);
            }

            INode<T> lastVisited = child;
            while (ancestors.Any())
            {
                node = ancestors.Pop();
                var result = node.Descendants(n =>
                    !n.Equals(lastVisited) && predicate(n))
                    .FirstOrDefault();
                if (result != null)
                {
                    commonAncestor = lastVisited;
                    return result;
                }
                lastVisited = node;
            }

            commonAncestor = null;
            return null;
        }

        public static int DistanceToAncestor<T>(this INode<T> child, INode<T> ancestor)
        {
            if (child.Equals(ancestor)) return 0;
            int distance = 1;

            var node = child;
            while (!node.IsRoot())
            {
                node = node.ParentNode;
                distance++;
                if (ancestor.Equals(node)) return distance;
            }

            return -1;
        }
    }
}
