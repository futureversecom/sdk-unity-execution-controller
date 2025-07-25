
using System;
using System.Collections;
using System.Collections.Generic;
using Futureverse.UBF.UBFExecutionController.Runtime;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class AssetTreeInventoryItem : IInventoryItem
{
    [SerializeField] private string _id;
    [SerializeField] private string _renderBlueprintId;
    [SerializeField] private string _renderCatalogUri;
    [SerializeField] private string _parsingBlueprintId;
    [SerializeField] private string _parsingCatalogUri;
    [SerializeField] private string _metadata;
    [SerializeField] private List<AssetTreeInventoryItem> _children;
    
    public string Id => _id;
    public string Name => _id;
    public AssetProfile AssetProfile { get; private set; }
    public JObject Metadata { get; private set; }
    public Dictionary<string, IInventoryItem> Children { get; private set; }

    public IEnumerator Prepare()
    {
        Metadata = string.IsNullOrEmpty(_metadata) ? new JObject() : JObject.Parse(_metadata);
        
        Children = new Dictionary<string, IInventoryItem>();
        foreach (var child in _children)
        {
            yield return child.Prepare();
            Children.Add(child.Id, child);
        }

        yield return AssetProfile.FromProfileData(
            new AssetProfileData()
            {
                RenderBlueprintId = _renderBlueprintId,
                ParsingBlueprintId = _parsingBlueprintId,
                RenderCatalogUri = _renderCatalogUri,
                ParsingCatalogUri = _parsingCatalogUri,
            },
            p => AssetProfile = p
        );
    } 
}
