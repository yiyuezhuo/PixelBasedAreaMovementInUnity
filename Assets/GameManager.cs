using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform srcAnchor;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Mapshower mapshower;

    PathFinding<Area> pathfinding;
    // Start is called before the first frame update
    void Start(){
        pathfinding = new PathFinding<Area>(mapshower);
    }
    public void OnAreaSelected(Area dst){
        Debug.Log($"OnAreaSelected dst={dst}");

        var src = mapshower.Pos2Area((Vector2)srcAnchor.transform.position);
        //Debug.Log($"src.center={src.center} dst.center={dst.center}");
        var path = pathfinding.PathFindingAStar(src, dst);

        Debug.Log(pathfinding.PathToString(path));

        var positions = new Vector3[path.Count];
        for(int i=0; i<positions.Length; i++){
            var c = path[i].center;
            positions[i] = mapshower.CenterToWorld(c);
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
