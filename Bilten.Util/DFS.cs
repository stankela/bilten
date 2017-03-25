using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bilten.Util
{
    public enum NodeColor
    { 
        White, Gray, Black
    }

    public class Node
    {
        public Node(string name)
        {
            Name = name;
            Color = NodeColor.White;
            Parent = null;
            DiscoveryTime = -1;
        }

        public string Name
        {
            get;
            set;
        }

        public NodeColor Color
        {
            get;
            set;
        }

        public Node Parent
        {
            get;
            set;
        }

        public int DiscoveryTime
        {
            get;
            set;
        }

        public int FinishTime
        {
            get;
            set;
        }

        public ISet<Node> AdjNodes = new HashSet<Node>();
    }

    public class Edge
    { 
    
    }

    public class DFS
    {
        private List<Node> Nodes = new List<Node>();
        private int time;

        private Node getNode(string name)
        {
            Node result = null;
            foreach (Node n in Nodes)
            {
                if (n.Name.Equals(name))
                    return n;
            }
            return result;
        }

        private void createGraph(List<string> nodes, List<string[]> edges)
        {
            Nodes.Clear();

            foreach (string s in nodes)
            {
                Nodes.Add(new Node(s));
            }
            foreach (string[] e in edges)
            {
                Node src = getNode(e[0]);
                Node dst = getNode(e[1]);
                if (src == null || dst == null)
                    throw new Exception();

                src.AdjNodes.Add(dst);
            }
        }

        public void createGraphFromExportSqlCeStript(string fileName)
        {
            List<string> nodes = new List<string>();
            List<string[]> edges = new List<string[]>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("CREATE TABLE "))
                    {
                        int index1 = line.IndexOf('[');
                        int index2 = line.IndexOf(']');
                        string node = line.Substring(index1 + 1, index2 - index1 - 1).Trim();
                        nodes.Add(node);
                    }
                    
                    if (line.Contains(" ADD CONSTRAINT ") && line.Contains(" FOREIGN KEY "))
                    {
                        int index1 = line.IndexOf('[');
                        int index2 = line.IndexOf(']');
                        string firstNode = line.Substring(index1 + 1, index2 - index1 - 1).Trim();

                        line = line.Remove(0, line.IndexOf(" REFERENCES "));
                        index1 = line.IndexOf('[');
                        index2 = line.IndexOf(']');
                        string secondNode = line.Substring(index1 + 1, index2 - index1 - 1).Trim();

                        edges.Add(new string[] { firstNode, secondNode });
                    }
                }
            }

            createGraph(nodes, edges);
        }

        public void doDFS()
        {
            foreach (Node n in Nodes)
            {
                n.Color = NodeColor.White;
                n.Parent = null;
            }
            time = 0;
            foreach (Node n in Nodes)
            {
                if (n.Color == NodeColor.White)
                    dfsVisit(n);
            }
        }

        private void dfsVisit(Node u)
        {
            ++time;
            u.DiscoveryTime = time;
            u.Color = NodeColor.Gray;
            foreach (Node v in u.AdjNodes)
            {
                if (v.Color == NodeColor.White)
                {
                    v.Parent = u;
                    dfsVisit(v);
                }
                else if (v.Color == NodeColor.Gray)
                { 
                    // back edge
                    continue;
                }
                else if (v.Color == NodeColor.Black)
                { 
                    // forward or cross edge
                    continue;
                }
            }
            u.Color = NodeColor.Black;
            ++time;
            u.FinishTime = time;
        }
    }
}
