using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeWallRemoveNode : NodeWallModificationNode
{
    NodeWall linkedWall;
    Vector2Int thisCoordinate;
    Vector2Int otherCoordinate;

    public void Setup(NodeWall linkedWall)
    {
        this.linkedWall = linkedWall;

        linkedSystem = linkedWall.LinkedSystem;
    }

    public override void UpdatePosition()
    {
        Vector3 thisPosition = linkedWall.LinkedSystem.LinkedFloor.GetLocalNodePositionFromNodeIndex(linkedWall.StartPosition);
        Vector3 otherPosition = linkedWall.LinkedSystem.LinkedFloor.GetLocalNodePositionFromNodeIndex(linkedWall.EndPosition);

        transform.localPosition = (thisPosition + otherPosition) * 0.5f;
        transform.localRotation = Quaternion.LookRotation(thisPosition - otherPosition, Vector3.up);

        float width = linkedSystem.WallThickness + widthOvershoot * 2;
        transform.localScale = new Vector3(width, transform.localScale.y, transform.localScale.z);
        //Warning: UpdteNodeSize calls UpdatePosition() -> Do not add here
    }

    /*
    public override void UpdateNodeSize()
    {
        UpdatePosition();
    }
    */

    public void RemoveNodeWall()
    {
        linkedWall.DestroyObject();
        linkedWall.LinkedSystem.HideModificationNodes();
        linkedWall.LinkedSystem.ShowModificationNodes(activateCollider: true);
    }
}
