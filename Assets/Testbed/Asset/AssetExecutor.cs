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
		[SerializeField] private string _collectionId;
		[SerializeField] private string _tokenId;
		[SerializeField] private ExecutionController _executionController;

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
			
			IInventoryItem item = null;
			yield return AssetRegisterInventoryItem.FromData(_client, _collectionId, _tokenId, i => item = i);
			if (item?.AssetProfile == null)
			{
				yield break;
			}

			yield return _executionController.RenderItem(item);
		}
	}
}