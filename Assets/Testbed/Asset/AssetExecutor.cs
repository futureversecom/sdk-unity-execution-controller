// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using AssetRegister.Runtime.Clients;
using AssetRegister.Runtime.Schema.Objects;
using Futureverse.UBF.Runtime.Execution;
using Futureverse.UBF.UBFExecutionController.Runtime;
using UnityEngine;

namespace Testbed.Simple
{
#if UNITY_EDITOR
	using UnityEditor;
	
	[CustomEditor(typeof(AssetExecutor))]

	public class AssetExecutorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(18);
			if (GUILayout.Button("Run"))
			{
				var executor = (AssetExecutor)target;
				executor.Run();
			}

			if (GUILayout.Button("Delete"))
			{
				var executor = (AssetExecutor)target;
				executor.Delete();
			}
		}
	}
#endif

	public class AssetExecutor : MonoBehaviour
	{
		[SerializeField] private MonoClient _client;
		[SerializeField] private string _chainId;
		[SerializeField] private string _chainName;
		[SerializeField] private string _collectionId;
		[SerializeField] private string _tokenId;

		public void Run()
		{
			StartCoroutine(RunRoutine());
		}

		public void Delete()
		{
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}

			Resources.UnloadUnusedAssets();
		}

		private IEnumerator RunRoutine()
		{
			Delete();

			var asset = new Asset()
			{
				Id = _chainId,
				CollectionId = _collectionId,
				TokenId = _tokenId,
			};
			
			IInventoryItem item = null;
			yield return AssetRegisterInventoryItem.FromAsset(_client, asset, i => item = i);
			
			ArtifactProvider.Instance.RegisterCatalog(item.AssetProfile.RenderCatalog);

			var blueprintDefinition = new BlueprintInstanceData(item.AssetProfile.RenderBlueprintResourceId);
			var blueprints = new List<IBlueprintInstanceData>
			{
				blueprintDefinition,
			};

			var executionData = new ExecutionData(
				transform,
				null,
				blueprints
			);
			yield return UBFExecutor.ExecuteRoutine(executionData, blueprintDefinition.InstanceId);
		}
	}
}