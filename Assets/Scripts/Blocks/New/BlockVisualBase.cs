using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockVisualBase : MonoBehaviour
{
    public Block block;
    public readonly Dictionary<Model, BlockVisualModel> _models = new Dictionary<Model, BlockVisualModel>();
    Model _currentModel;
    public Action<BlockVisualModel> onModelChange;

    void Update()
    {
        transform.position = block.transform.position;
    }

    public void Select(Model model)
    {
        if (model == _currentModel) return;
        if (_models.ContainsKey(_currentModel)) _models[_currentModel].Hide();
        if (!_models.ContainsKey(model)) AddModel(model);
        _models[model].Show();
        _currentModel = model;
        onModelChange?.Invoke(Current);
    }

    public BlockVisualModel Current => _models[_currentModel];
    
    public enum Model
    {
        NodeDeadend, NodePipe, Root
    }

    void AddModel(Model model)
    {
        GameObject prefab;
        switch (model)
        {
            case Model.Root:
                prefab = Prefabs.Instance.rootBlockVisualModel;
                break;
            case Model.NodeDeadend:
                prefab = Prefabs.Instance.nodeDeadendVisualModel;
                break;
            case Model.NodePipe:
                prefab = Prefabs.Instance.nodePipeVisualModel;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(model), model, null);
        }

        var modelObject = Instantiate(prefab, transform).GetComponent<BlockVisualModel>();
        _models.Add(model, modelObject);
    }
    
    public static BlockVisualBase Create(Block block, Model model)
    {
        var b = Instantiate(Prefabs.Instance.blockVisualBase, SharedObjects.Instance.blockVisualCanvases[block.rootNum].transform)
            .GetComponent<BlockVisualBase>();
        b.block = block;
        b.AddModel(model);
        b._currentModel = model;
        b.Current.Show();
        return b;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}