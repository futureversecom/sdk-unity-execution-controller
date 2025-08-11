// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Futureverse.UBF.ExecutionController.Runtime.Settings;
using Futureverse.UBF.Runtime;
using Futureverse.UBF.Runtime.Resources;
using Newtonsoft.Json;
using UnityEngine;

namespace Futureverse.UBF.UBFExecutionController.Runtime
{
	[JsonObject]
	public class AssetProfileData
	{
		[JsonProperty(PropertyName = "render-instance")]
		public string RenderBlueprintId;
		[JsonProperty(PropertyName = "render-catalog")]
		public string RenderCatalogUri;
		[JsonProperty(PropertyName = "parsing-instance")]
		public string ParsingBlueprintId;
		[JsonProperty(PropertyName = "parsing-catalog")]
		public string ParsingCatalogUri;
		[JsonProperty(PropertyName = "standard-version")]
		public string StandardVersion;
	}

	[JsonObject]
	public class AssetProfileJson
	{
		[JsonProperty(PropertyName = "profile-version")]
		public string ProfileVersion;
		[JsonProperty(PropertyName = "ubf-variants")]
		public Dictionary<string, Dictionary<string, AssetProfileData>> Variants;
	}
	
	public class AssetProfile
	{
		public string RenderBlueprintResourceId { get; private set; }
		public string ParsingBlueprintResourceId { get; private set; }
		public Catalog RenderCatalog { get; private set; }
		public Catalog ParsingCatalog { get; private set; }

		public static IEnumerator FetchByUri(	
			string uri,
			Action<AssetProfile> onComplete,
			string[] variantsOverride = null)
		{
			var resourceHandler = new JsonResourceLoader<AssetProfileJson>(uri);

			AssetProfileJson profile = null;
			yield return resourceHandler.Get(data => profile = data);
			if (profile == null)
			{
				Debug.LogError($"No asset profile found at {uri}");
				onComplete?.Invoke(null);
				yield break;
			}

			var profileData = GetProfileData(profile, variantsOverride);
			if (profileData == null)
			{
				Debug.LogError($"No asset profile with supported variant and version found at {uri}.");
				yield break;
			}

			yield return FromProfileData(profileData, onComplete);
		}
		
		private static AssetProfileData GetProfileData(AssetProfileJson profile, string[] variantsOverride = null)
		{
			var supportedVariants = variantsOverride ??
				ExecutionControllerSettings.GetOrCreateSettings().SupportedVariants;

			var variant = supportedVariants
				.Select(v => profile.Variants.GetValueOrDefault(v))
				.FirstOrDefault(dict => dict != null);

			if (variant == null)
			{
				return null;
			}

			Version highestVersion = null;
			AssetProfileData highestProfile = null;

			foreach (var kvp in variant)
			{
				if (Version.TryParse(kvp.Key, out var collectionVersion) &&
					Version.TryParse(kvp.Value.StandardVersion, out var standardVersion) &&
					standardVersion.IsSupported())
				{
					if (highestVersion == null || collectionVersion.CompareTo(highestVersion) > 0)
					{
						highestVersion = collectionVersion;
						highestProfile = kvp.Value;
					}
				}
			}

			return highestProfile;
		}

		public static IEnumerator FromProfileData(AssetProfileData profileData, Action<AssetProfile> onComplete)
		{
			var profile = new AssetProfile
			{
				RenderBlueprintResourceId = profileData.RenderBlueprintId,
				ParsingBlueprintResourceId = profileData.ParsingBlueprintId,
			};

			if (!string.IsNullOrEmpty(profileData.RenderCatalogUri))
			{
				var renderCatalogLoader = new CatalogResourceLoader(profileData.RenderCatalogUri);
				yield return renderCatalogLoader.Get(data => profile.RenderCatalog = data);
			}

			if (!string.IsNullOrEmpty(profileData.ParsingCatalogUri))
			{
				var parsingCatalogLoader = new CatalogResourceLoader(profileData.ParsingCatalogUri);
				yield return parsingCatalogLoader.Get(data => profile.ParsingCatalog = data);
			}

			onComplete?.Invoke(profile);
		}
	}
}