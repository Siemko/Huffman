using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Huffman

{
    class Node : IComparable<Node>
    {
        /// <summary>
        /// Klasa Node reprezentuje pojedyńcz węzeł drzewa, przechowuje trzy wartości:
        /// letter - litera
        /// freq - częstość występowania litery
        /// code - zakodowaną literę
        /// Trzy referencje:
        /// leftNode - węzeł niżej po lewej
        /// rightNode - węzeł niżej po prawej
        /// parentNode - węzeł nadrzędny
        /// Oraz wartość boolowską isLeaf informującą o byciu liście w drzewie
        /// </summary>
        public char letter { get; set; }
        public int freq { get; set; }
        public Node leftNode, rightNode, parentNode;
        public string code;
        public bool isLeaf;
        /// <summary>
        /// Konstruktor parametryczny dla dodaawnia nowego LIŚCIA przy tworzeniu drzewa, freq = 1 bo to pierwsze wystąpienie litery
        /// </summary>
        /// <param name="c"></param>
        public Node(char c)
        {
            isLeaf = true;
            leftNode = rightNode = parentNode = null;
            this.letter = c;
            this.freq = 1;
        }
        /// <summary>
        /// Konstruktor parametryczny dla łączenia dwóch liści w jeden węzeł z sumowanym freq. W zależności od wielkości freq, jeden z liści ustawiany jest po prawej, drugi po lewej, jako dzieci.
        /// </summary>
        /// <param name="n1">pierwszy liść</param>
        /// <param name="n2">drugi liść</param>
        public Node(Node n1, Node n2)
        {
            isLeaf = false;
            this.letter = '*';
            if (n1.freq >= n2.freq)
            {
                rightNode = n1;
                leftNode = n2;
            }
            else
            {
                rightNode = n2;
                leftNode = n1;
            }
            this.freq = n1.freq + n2.freq;
            rightNode.parentNode = leftNode.parentNode = this;
        }
            /// <summary>
            /// Przeciążona metoda interfejsu IComparable, aby móc stosować metodę .Sort() dla listy obiektów klasy Node
            /// </summary>
            /// <param name="other">referencja do porównywanego obiektu klasy Node</param>
            /// <returns></returns>
        public int CompareTo(Node other)
        {
            return this.freq.CompareTo(other.freq);
        }
        /// <summary>
        /// prosta metoda zwiększająca częstość występowania litery w tekście źródłowym
        /// </summary>
        public void IncFreq()
        {
            freq++;
        }
    }
    /// <summary>
    /// Klasa odpowiedzialna za budowanie i wyświetlanie drzewa Huffmana
    /// </summary>
    class HuffmanTree
    {
        /// <summary>
        /// Słownik, w którym kluczem jest litera, a wartością ciąg znaków reprezentujący zakodowaną literę
        /// </summary>
        public static Dictionary<char, string> codes = new Dictionary<char, string>();

        /// <summary>
        /// Tworzenie listy obiektów klasy Node
        /// Dla każdej litery w źródle sprawdzana jest jej obecność w liście, jeżeli litera występuje, to znajduje się jej indeks i inkrementuje wartość częstości występowania
        /// W przeciwnym wypadku dodajemy nowy obiekt klasy Node do listy, przechowujący naszą literę z źródła
        /// </summary>
        /// <param name="source">tekst źródłowy</param>
        /// <returns></returns>
        public static List<Node> buildList(string source)
        {
            List<Node> nodes = new List<Node>();
            foreach (char c in source)
            {
                if (nodes.Exists(x => x.letter == c))
                    nodes[nodes.FindIndex(y => y.letter == c)].IncFreq();
                else
                    nodes.Add(new Node(c));
            }

            nodes.Sort();
            return nodes;
        }
        /// <summary>
        /// Metoda odpowiedzialna za budowę drzewa Huffmana. Liście łączone są w węzły, aż wyczerpie się ich liczba. Po każdym połączeniu lista węzłów jest sortowana
        /// </summary>
        /// <param name="nodes"></param>
        public static void buildTree(List<Node> nodes)
        {
            while (nodes.Count > 1)
            {
                Node n1 = nodes[0];
                nodes.RemoveAt(0);
                Node n2 = nodes[0];
                nodes.RemoveAt(0);
                nodes.Add(new Node(n1, n2));
                nodes.Sort();
            }
        }
        /// <summary>
        /// Ustawianie kodu dla liter rekturencyjne oraz zapisywanie pary litera, kod w słowniku
        /// </summary>
        /// <param name="code">kod, domyślnie pusty string</param>
        /// <param name="Nodes">referencja do obiektu klasy Node</param>
        public static void setCodeToTheTree(string code, Node Nodes)
        {
            if (Nodes == null)
                return;
            if (Nodes.leftNode == null && Nodes.rightNode == null)
            {
                Nodes.code = code;
                if (!codes.ContainsKey(Nodes.letter))
                {
                    codes.Add(Nodes.letter, code);
                }
                return;
            }
            setCodeToTheTree(code + "0", Nodes.leftNode);
            setCodeToTheTree(code + "1", Nodes.rightNode);
        }
        /// <summary>
        /// Rekurencyjne wyrosowanie drzewa na ekranie => metoda znaleziona w internecie, dopasowana do naszej wersji programu
        /// Zródło: github.com
        /// </summary>
        /// <param name="level">poziom wypisywania</param>
        /// <param name="node">obiekt klasy Node</param>
        public static void PrintTree(int level, Node node)
        {
            if (node == null)
                return;
            for (int i = 0; i < level; i++)
            {
                Console.Write("\t");
            }
            Console.Write("[" + node.letter + "]");
            Console.WriteLine("(" + node.code + ")");
            PrintTree(level + 1, node.rightNode);
            PrintTree(level + 1, node.leftNode);
        }

        public static void PrintInformation(List<Node> nodeList)
        {
            foreach (var item in nodeList)
                Console.WriteLine("Litera : {0} - Częstość występowania: {1}", item.letter, item.freq);
        }
    }
    class Program
    {
        public static string output;
        static void Main(string[] args)
        {
            List<Node> nodeList;
            string src;
            
            using (StreamReader fs = new StreamReader(@"F:\Projects\Huffman\Huffman\source.txt"))
            {
                src = fs.ReadToEnd();
            }
            Console.WriteLine("Oryginał: " + src);
            nodeList = HuffmanTree.buildList(src);
            HuffmanTree.PrintInformation(nodeList);
            HuffmanTree.buildTree(nodeList);
            HuffmanTree.setCodeToTheTree("", nodeList[0]);
            Console.WriteLine("Wygląd drzewa:");
            HuffmanTree.PrintTree(0, nodeList[0]);
            Console.WriteLine("Kody liter:");
            /*
            foreach(KeyValuePair<char, string> para in HuffmanTree.codes)
            {
                Console.WriteLine(para.ToString());
            }
            */
            string val;
            foreach (char c in src)
            {
                HuffmanTree.codes.TryGetValue(c, out val);
                output += val;
            }
            Console.Write(output);
            Console.ReadKey();
        }
    }
}
