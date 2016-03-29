using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman_v2
{
    public class FreqTable
    {
        public Dictionary<char, int> Freqs { get; set; } = new Dictionary<char, int>();

        public void Build(string line)
        {
            foreach(char c in line)
            {
                if(!Freqs.ContainsKey(c))
                {
                    Freqs.Add(c, 1);
                }
                else
                {
                    Freqs[c] += 1;
                }
            }
        }
    }

    public class Node
    {
        public Node Left { get; set; } = null;
        public Node Right { get; set; } = null;
        public char? Char { get; set; } = null;
        public int Freq { get; set; } = default(int); //ale fajne znalazłem <3

        public List<bool> Traverse(char? ch, List<bool> data)
        {
            if(Left == null && Right == null)
            {
                return (bool)ch?.Equals(Char) ? data : null;
            }
            else
            {
                List<bool> left = null;
                List<bool> right = null;

                if(Left != null)
                {
                    List<bool> leftPath = new List<bool>(data);
                    leftPath.Add(false);
                    left = Left.Traverse(ch, leftPath);
                }

                if (null != Right)
                {
                    List<bool> rightPath = new List<bool>(data);
                    rightPath.Add(true); //Add a '1'
                    right = Right.Traverse(ch, rightPath);
                }

                return (null != left) ? left : right;
            }
        }

        public bool IsLeaf()
        {
            return (null == this.Left && null == this.Right);
        }
    }

    class Tree
    {
        private List<Node> nodes = new List<Node>();
        public Node Root { get; set; } = null;
        public FreqTable _freqs { get; private set; } = new FreqTable();
        public int BitCountForTree { get; private set; } = default(int);

        public void BuildTree(string source)
        {
            nodes.Clear(); //As we build a new tree, first make sure it's clean :)

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");
            else
            {
                _freqs.Build(source);

                foreach (KeyValuePair<char, int> symbol in _freqs.Freqs)
                {
                    nodes.Add(new Node() { Char = symbol.Key, Freq = symbol.Value });
                }

                while (nodes.Count > 1)
                {
                    List<Node> orderedNodes = nodes.OrderBy(node => node.Freq).ToList();

                    if (orderedNodes.Count >= 2)
                    {
                        List<Node> takenNodes = orderedNodes.Take(2).ToList();

                        Node parent = new Node()
                        {
                            Char = null,
                            Freq = takenNodes[0].Freq + takenNodes[1].Freq,
                            Left = takenNodes[0],
                            Right = takenNodes[1]
                        };
                        nodes.Remove(takenNodes[0]);
                        nodes.Remove(takenNodes[1]);
                        nodes.Add(parent);
                    }
                }

                Root = nodes.FirstOrDefault();
            }
        }

        public BitArray Encode(string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                List<bool> encodedSource = new List<bool>();
                encodedSource.AddRange(source.SelectMany(character =>
                                            Root.Traverse(character, new List<bool>())
                                        ).ToList()
                                      );
                BitCountForTree = encodedSource.Count;
                return new BitArray(encodedSource.ToArray());
            }
            else return null;
        }

        public string Decode(BitArray bits)
        {
            Node current = Root;
            string decodedString = string.Empty;

            foreach (bool bit in bits)
            {
                current = (bit ? current.Right ?? current : current.Left ?? current);

                if (current.IsLeaf())
                {
                    decodedString += current.Char;
                    current = Root;
                }
            }

            return decodedString;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            string src;
            using (StreamReader fs = new StreamReader(@"F:\Projects\Huffman\Huffman\source.txt"))
            {
                src = fs.ReadToEnd();
            }
            Tree HTree = new Tree();
            HTree.BuildTree(src);
            BitArray encoded = HTree.Encode(src);
            string zero = string.Join(string.Empty, encoded.Cast<bool>().Select(bit => bit ? "1" : "0"));
            Console.Write(zero);
            Console.WriteLine();
            byte[] bytes = new byte[(encoded.Length / 8) + 1];
            encoded.CopyTo(bytes, 0);
            string output = Encoding.Default.GetString(bytes);
            Console.WriteLine(output);

            bool[] boolAr = new BitArray(Encoding.Default.GetBytes(output)).Cast<bool>().Take(HTree.BitCountForTree).ToArray();
            BitArray encoded2 = new BitArray(boolAr);

            string decoded = HTree.Decode(encoded2);
            Console.WriteLine(decoded);

            Console.ReadKey();
        }
    }
}
