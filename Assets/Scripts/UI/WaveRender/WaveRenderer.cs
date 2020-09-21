using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class WaveRenderer : Graphic
{
    public AudioClip clip;
    public int samplesFrom, samplesTo;
    
    public List<Multiline> multilines = new List<Multiline>();

    readonly List<UIVertex> _vertexBuffer = new List<UIVertex>();
    readonly List<int> _indexBuffer = new List<int>();
    public float width, height;
    [Range(100, 6000)]public int resolution = 1000;
    [Range(0.01f, 4f)]public float thickness = 1f;

    void DrawWave(int lines = 1000)
    {
        var rect = rectTransform.rect;
        width = rect.width;
        height = rect.height;
        multilines.Clear();

        var dataLength = samplesTo - samplesFrom;
        var data = new float[dataLength];
        clip.GetData(data, samplesFrom);
        var positions = new Vector2[(lines - 1) * 2];
        var positionsInd = 0;
        for (var i = 0; i < lines - 1; i++)
        {
            float min = 1f, max = -1f;
            for (var j = i * dataLength / lines; j < (i + 1) * dataLength / lines; j++)
            {
                min = Mathf.Min(min, data[j]);
                max = Mathf.Max(max, data[j]);
            }
            if (max == -1f) continue;

            positions[positionsInd++] = new Vector2(width / 2 + min * width / 2, height - height / lines * i);
            positions[positionsInd++] = new Vector2(width / 2 + max * width / 2, height - height / lines * i);
        }

        multilines.Add(new Multiline(color, thickness, positions));
    }
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        DrawWave(resolution);
        vh.Clear();

        _vertexBuffer.Clear();
        _indexBuffer.Clear();

        foreach (var multiline in multilines)
        {
            DrawMultiLine(_vertexBuffer, _indexBuffer, multiline);
        }

        vh.AddUIVertexStream(_vertexBuffer, _indexBuffer);
    }
    
    static void DrawMultiLine(List<UIVertex> vertices, List<int> indices, Multiline line)
    {
        var verts = new UIVertex[line.Positions.Length * 2];
        var inds = new int[line.Positions.Length * 6];

        var startIndex = vertices.Count;

        for (int i = 0; i < line.Positions.Length; i++)
        {
            verts[i * 2].position = new Vector2(line.Positions[i].x, line.Positions[i].y - line.Thickness / 2);
            verts[i * 2].color = line.Color;

            verts[i * 2 + 1].position = new Vector2(line.Positions[i].x, line.Positions[i].y + line.Thickness / 2);
            verts[i * 2 + 1].color = line.Color;
        }

        for (var i = 0; i < line.Positions.Length - 1; i++)
        {
            inds[i * 6 + 0] = startIndex + i * 2;
            inds[i * 6 + 1] = startIndex + i * 2 + 1;
            inds[i * 6 + 2] = startIndex + i * 2 + 3;
            inds[i * 6 + 3] = startIndex + i * 2;
            inds[i * 6 + 4] = startIndex + i * 2 + 3;
            inds[i * 6 + 5] = startIndex + i * 2 + 2;
        }

        vertices.AddRange(verts);
        indices.AddRange(inds);
    }
}