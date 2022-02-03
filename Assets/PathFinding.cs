using System.Collections;
using System.Collections.Generic;
// using UnityEngine;

public interface IGraph<IndexT>
{
    float MoveCost(IndexT src, IndexT dst);
    // Vector3Int: (x, y, 0)
    // src and dst are expected to be neighbor.

    float EstimateCost(IndexT src, IndexT dst);
    // z of src and dst are expected to be 0
    
    IEnumerable<IndexT> Neighbors(IndexT pos);
}

public class PathFinding<IndexT>
{
    IGraph<IndexT> graph;

    public PathFinding(IGraph<IndexT> graph)
    {
        this.graph = graph;
    }

    List<IndexT> ReconstructPath(Dictionary<IndexT, IndexT> cameFrom, IndexT current)
    {
        var total_path = new List<IndexT>{current};
        while(cameFrom.ContainsKey(current)){
            current = cameFrom[current];
            total_path.Add(current);
        }
        total_path.Reverse();
        return total_path;
    }

    float TryGet(Dictionary<IndexT, float> dict, IndexT key)
    {
        if (dict.ContainsKey(key))
        {
            return dict[key];
        }
        return float.PositiveInfinity;
    }
    

    public List<IndexT> PathFindingAStar(IndexT src, IndexT dst)
    {
        var openSet = new HashSet<IndexT>{src};
        var cameFrom = new Dictionary<IndexT, IndexT>();

        var gScore = new Dictionary<IndexT, float>{{src, 0}}; // default Mathf.Infinity

        // Debug.Log($"graph:{graph}");
        var fScore = new Dictionary<IndexT, float>{{src, graph.EstimateCost(src, dst)}}; // default Mathf.Infiniy

        while(openSet.Count > 0)
        {
            IEnumerator<IndexT> openSetEnumerator = openSet.GetEnumerator();

            openSetEnumerator.MoveNext(); // assert?
            IndexT current = openSetEnumerator.Current;
            float lowest_f_score = fScore[current];

            while(openSetEnumerator.MoveNext())
            {
                IndexT pos = openSetEnumerator.Current;
                if(fScore[pos] < lowest_f_score)
                {
                    lowest_f_score = TryGet(fScore, pos);
                    current = pos;
                }
            }

            if(current.Equals(dst))
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            foreach(IndexT neighbor in graph.Neighbors(current))
            {
                float tentative_gScore = TryGet(gScore, current) + graph.MoveCost(current, neighbor);
                if(tentative_gScore < TryGet(gScore, neighbor))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    fScore[neighbor] = TryGet(gScore, neighbor) + graph.EstimateCost(neighbor, dst);

                    openSet.Add(neighbor);
                }
            }
        }
        return new List<IndexT>(); // failure
    }

    public string PathToString(List<IndexT> path)
    {
        string s = "";
        foreach(IndexT p in path){
            s += p.ToString() + ","; // TODO: Use `Join` or `StringBuilder`
        }
        return $"Path({path.Count}):{s}";
    }
}
