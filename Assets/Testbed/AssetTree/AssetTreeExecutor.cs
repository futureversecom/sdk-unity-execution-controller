// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections;
using Futureverse.UBF.UBFExecutionController.Runtime;
using UnityEngine;

namespace Testbed.AssetTree
{
#if UNITY_EDITOR
	using UnityEditor;
	
	[CustomEditor(typeof(AssetTreeExecutor))]

	public class AssetExecutorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(18);
			if (GUILayout.Button("Run"))
			{
				var executor = (AssetTreeExecutor)target;
				executor.Run();
			}

			if (GUILayout.Button("Delete"))
			{
				var executor = (AssetTreeExecutor)target;
				executor.Delete();
			}
		}
	}
#endif
	
	public class AssetTreeExecutor : MonoBehaviour
	{
		[SerializeField] private ExecutionController _executionController;
		[SerializeField] private AssetTreeInventoryItem _rootAsset;
		
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

			yield return _rootAsset.Prepare();
			yield return _executionController.RenderItem(_rootAsset);
		}
	}
}