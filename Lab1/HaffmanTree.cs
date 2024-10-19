using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    internal class HaffmanTree
    {
        private List<StartNode> _startNodes = new List<StartNode>();
        public Dictionary<char, List<bool>> Codes { get; private set; } = new Dictionary<char, List<bool>>();

        private class TreeNode
        {
            public ulong Amount { get; set; }
            public bool? Bit { get; set; } = null;
            public TreeNode? NextNode { get; set; } = null;
            public List<char> symbols { get; set; } = new List<char>();
        }

        private class StartNode : TreeNode
        {
            public char Symbol { get; set; }
        }

        public HaffmanTree(Dictionary<char, ulong> symbolDictionary)
        {
            BuildTree(symbolDictionary);
            CountCodes();
        }

        private void BuildTree(Dictionary<char, ulong> symbolDictionary)
        {
            ArrayList startNodesArray = new ArrayList(symbolDictionary.Select(x => (x.Key, x.Value)).ToArray());

            foreach ((char, ulong) node in startNodesArray)
            {
                _startNodes.Add(new StartNode { Symbol = node.Item1, Amount = node.Item2 });
            }

            var processingNodes = new List<TreeNode>(_startNodes);

            foreach(var node in processingNodes)
            {
                node.symbols.Add(((StartNode)node).Symbol);
            }


            while (processingNodes.Count > 1)
            {
                processingNodes = processingNodes.OrderByDescending(x => x.Amount).ToList();
                var lastNode1 = processingNodes[processingNodes.Count- 2];
                var lastNode2 = processingNodes[processingNodes.Count - 1];
                var newNode = new TreeNode { Amount = lastNode1.Amount + lastNode2.Amount };
                lastNode1.NextNode = newNode;
                lastNode1.Bit = false;
                lastNode2.NextNode = newNode;
                lastNode2.Bit = true;
                processingNodes.Remove(lastNode1);
                processingNodes.Remove(lastNode2);
                processingNodes.Add(newNode);
            }
        }

        private void CountCodes()
        {
            foreach (StartNode startNode in _startNodes)
            {
                TreeNode currentNode = startNode;
                var currentCode = new List<bool>();
                while (currentNode.Bit != null)
                {
                    currentCode.Add((bool)currentNode.Bit);
                    currentNode = currentNode.NextNode;
                }
                currentCode.Reverse();
                Codes[startNode.Symbol] = currentCode;
            }
        }
    }
}
