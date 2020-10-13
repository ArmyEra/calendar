using TreeModule;

namespace Audio.Extensions
{
    public static class TreeNodeExtensions
    {
        /// <summary>
        /// Возвращает TRUE, если узел является дочерним узлом в N-м поколении 
        /// </summary>
        public static bool IsChildNodeOf<T>(this TreeNode<T> treeNode, TreeRootNode<T> parentNode)
        {
            if (!parentNode.Container.Equals(treeNode.Container))
                return false;
            
            if (parentNode.Level >= treeNode.Level)
                return false;

            if (parentNode.IsRoot)
                return true;
                
            var currentNode = treeNode;
            while (currentNode.Level > parentNode.Level)
                currentNode = (TreeNode<T>)currentNode.Parent;

            return currentNode.Equals(parentNode);
        }
    }
}