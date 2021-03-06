using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class WaveRenderer : Graphic
{
    public int samplesFrom, samplesTo;
    public int selectSamplesFrom, selectSamplesTo;
    public int resolution = 1000;

    readonly List<Multiline> _multilines = new List<Multiline>();
    readonly List<Line> _lines = new List<Line>();

    readonly List<UIVertex> _vertexBuffer = new List<UIVertex>();
    readonly List<int> _indexBuffer = new List<int>();

    SlicedAudioClip _slicedClip;

    public SlicedAudioClip SlicedClip
    {
        get
        {
            if (_slicedClip == null && clipAsset != null) 
                _slicedClip = SlicedAudioClip.CreateFromAsset(clipAsset);
            return _slicedClip;
        }
        set => _slicedClip = value;
    }

    public SlicedAudioClipAsset clipAsset;
    public float thickness;

    void DrawWave()
    {
        if (SlicedClip == null) return;
        var rect = rectTransform.rect;
        var width = rect.width;
        var height = rect.height;
        _lines.Clear();

        if (samplesTo == 0 && samplesFrom == 0)
            samplesTo = _slicedClip.slices.Last();
        var dataLength = samplesTo - samplesFrom;
        var data = SlicedClip.data;

        const float addWidth = 0.03f;
        for (var i = 0; i < resolution - 1; i++)
        {
            float min = 1f, max = -1f;
            var batchSampleStart = i * dataLength / resolution + samplesFrom;
            for (var j = i * dataLength / resolution; j < (i + 1) * dataLength / resolution; j++)
            {
                min = Mathf.Min(min, data[j + samplesFrom]);
                max = Mathf.Max(max, data[j + samplesFrom]);
            }
            if (max == -1f) continue;
            var inSelection = batchSampleStart > selectSamplesFrom && batchSampleStart < selectSamplesTo;
            var t = inSelection
                ? thickness * 2
                : thickness;
            min -= addWidth / 2;
            max += addWidth / 2;

            var line = new Line(color, t, new Vector2(width / 2 + min * width / 2, height - height / resolution * i), 
                new Vector2(width / 2 + max * width / 2, height - height / resolution * i));
            _lines.Add(line);
        }
    }
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        DrawWave();
        vh.Clear();

        _vertexBuffer.Clear();
        _indexBuffer.Clear();

        foreach (var multiline in _multilines)
        {
            DrawMultiLine(_vertexBuffer, _indexBuffer, multiline);
        }

        foreach (var line in _lines)
        {
            DrawLine(_vertexBuffer, _indexBuffer, line);
        }

        vh.AddUIVertexStream(_vertexBuffer, _indexBuffer);
    }
    
    static void DrawMultiLine(List<UIVertex> vertices, List<int> indices, Multiline line)
    {
        var verts = new UIVertex[line.Positions.Length * 2];
        var inds = new int[line.Positions.Length * 6];

        var startIndex = vertices.Count;

        for (var i = 0; i < line.Positions.Length; i++)
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
    
    static void DrawLine(List<UIVertex> vertices, List<int> indices, Line line)
    {
        var verts = new UIVertex[4];
        var inds = new int[6];

        var startIndex = vertices.Count;
        verts[0].position = new Vector3(line.From.x, line.From.y - line.Thickness / 2);
        verts[0].uv0 = new Vector2(0f, 0f);
        verts[0].color = line.Color;
        verts[1].position = new Vector3(line.From.x, line.From.y + line.Thickness / 2);
        verts[1].uv0 = new Vector2(0f, 1f);
        verts[1].color = line.Color;
        verts[2].position = new Vector3(line.To.x, line.To.y - line.Thickness / 2);
        verts[2].uv0 = new Vector2(1f, 0f);
        verts[2].color = line.Color;
        verts[3].position = new Vector3(line.To.x, line.To.y + line.Thickness / 2);
        verts[3].uv0 = new Vector2(1f, 1f);
        verts[3].color = line.Color;

        inds[0] = startIndex;
        inds[1] = startIndex + 1;
        inds[2] = startIndex + 3;
        inds[3] = startIndex;
        inds[4] = startIndex + 3;
        inds[5] = startIndex + 2;

        vertices.AddRange(verts);
        indices.AddRange(inds);
    }
    
    
}