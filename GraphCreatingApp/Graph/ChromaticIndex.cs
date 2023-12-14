using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCreatingApp.Graph
{
    class ChromaticIndex
    {
        static object locker = new object();
        static List<Edge> result = new List<Edge>();



        public int FindChromaticIndex(List<Edge> edges, int e)
        {

            Parallel.For(1, FindMaxDegree(edges, e) + 2, () => new List<Edge>(), (i, state, newGraph) =>
            {
                newGraph = new List<Edge>(edges);

                Coloring(newGraph, 0, e, i);

                return newGraph;

            },
            (x) =>
            {
                lock (locker)
                {
                    result = x;
                }
            }
            );

            int max = 0;

            Parallel.For(0, e, x => FindMaxColor(result, x, ref max));

            return max;
        }

        private void Coloring(List<Edge> edges, int i, int m, int num_color)
        {
            if (i == m) return;
            for (int c = 1; c <= num_color; c++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (j == i)
                        continue;
                    if ((edges[j].FirstVertex == edges[i].FirstVertex
                    || edges[j].FirstVertex == edges[i].SecondVertex
                    || edges[j].SecondVertex == edges[i].FirstVertex
                    || edges[j].SecondVertex == edges[i].SecondVertex)
                    && edges[j].Color == edges[i].Color)
                    {
                        edges[i].Color = c;
                        Coloring(edges, i + 1, m, num_color);
                    }
                }
            }
            Coloring(edges, i + 1, m, num_color);
        }

        public int FindMaxDegree(List<Edge> edges, int e)
        {
            int maxDegree = int.MinValue;
            for (int i = 0; i < e; i++)
            {
                int degree = 0;
                for (int j = 0; j < e; j++)
                {
                    if (edges[i].FirstVertex == edges[j].FirstVertex
                    || edges[i].FirstVertex == edges[j].SecondVertex)
                    {
                        degree++;
                    }
                }
                if (degree > maxDegree)
                {
                    maxDegree = degree;
                }
            }
            for (int i = 0; i < e; i++)
            {
                int degree = 0;
                for (int j = 0; j < e; j++)
                {
                    if (edges[i].SecondVertex == edges[j].FirstVertex
                    || edges[i].SecondVertex == edges[j].SecondVertex)
                    {
                        degree++;
                    }
                }
                if (degree > maxDegree)
                {
                    maxDegree = degree;
                }
            }
            return maxDegree;
        }

        private void FindMaxColor(List<Edge> edges, int i, ref int max)
        {
            lock (locker)
            {
                if (max < edges[i].Color)
                {
                    max = edges[i].Color;
                }
            }

        }

    }
}
