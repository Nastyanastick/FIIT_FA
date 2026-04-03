using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Implementations.BST;

namespace TreeDataStructures.Implementations.Splay;

public class SplayTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    protected override BstNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    private void Splay(BstNode<TKey, TValue> node)
    {
        while (node != Root)
        {
            BstNode<TKey, TValue> parent = node.Parent;
            BstNode<TKey, TValue> grandparent = parent.Parent;

            if (grandparent == null) // zig
            {
                if (node == parent.Left) RotateRight(parent);
                else RotateLeft(parent);
            }
            else if (parent == grandparent.Left && node == parent.Left) RotateBigRight(grandparent); // zig-zig
            else if (parent == grandparent.Right && node == parent.Right) RotateBigLeft(grandparent);
            else if (parent == grandparent.Left && node == parent.Right) RotateDoubleRight(grandparent); // zig-zag
            else if (parent == grandparent.Right && node == parent.Left) RotateDoubleLeft(grandparent);
        }
    }

    public override bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }
    
    protected override void OnNodeAdded(BstNode<TKey, TValue> newNode)
    {
        Splay(newNode);
    }
    
    protected override void OnNodeRemoved(BstNode<TKey, TValue>? parent, BstNode<TKey, TValue>? child)
    {
        if (parent != null) Splay(parent);
        else if (child != null) Splay(child);
    }
    
    public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        BstNode<TKey, TValue>? current = Root;
        BstNode<TKey, TValue>? parCurrent = null;
        while (current != null)
        {
            parCurrent = current;
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp < 0) current = current.Left;
            else if (cmp > 0) current = current.Right;
            else if (cmp == 0) 
            {
                Splay(current);
                value = current.Value;
                return true;
            }

        }
        if (parCurrent != null) Splay(parCurrent);
        value = default; // значение по умолчанию
        return false;
    }
}
