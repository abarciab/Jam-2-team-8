using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class accusationLineRenderer : Graphic
{
    public Vector3 startPoint;
    public Vector3 endPoint;

    public Vector3 offsetStart;
    public Vector3 offsetEnd;

    public float offset;

    public GameObject startPin;
    public GameObject endPin;

    private void Update()
    {
        if (startPoint != startPin.transform.position || endPoint != endPin.transform.position) {
            startPoint = startPin.transform.localPosition;
            endPoint = endPin.transform.localPosition;
            SetVerticesDirty();
        }
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        

        vh.Clear();

        //float width = rectTransform.rect.width;
        //float height = rectTransform.rect.height;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        /*vertex.position = startPoint;
        vh.AddVert(vertex);

        vertex.position = startPoint + (Vector3) offset;
        vh.AddVert(vertex);

        vertex.position = endPoint + (Vector3) offset;
        vh.AddVert(vertex);

        vertex.position = endPoint;
        vh.AddVert(vertex);*/

        vertex.position = new Vector3(startPoint.x, startPoint.y);
        vh.AddVert(vertex);

        vertex.position = new Vector3(startPoint.x, startPoint.y - offset);
        vh.AddVert(vertex);

        vertex.position = new Vector3(endPoint.x, endPoint.y);
        vh.AddVert(vertex);

        vertex.position = new Vector3(endPoint.x, endPoint.y + offset);
        vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);

        //vh.FillMesh(m);
    }
}
