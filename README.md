# Futureverse Execution Controller

## Installation

Go to the Unity Package Manager window, and select `Add package from git URL...` and enter this link https://github.com/futureversecom/sdk-unity-asset-register.git?path=Assets/Plugins/AssetRegister (append `#vX.X.X` to specify a version). Alternatively, you can get a .unitypackage from the Releases page.

### Dependencies
You will need four additional Futureverse Unity SDKs to use the Execution Controller:
* [Asset Register](https://github.com/futureversecom/sdk-unity-asset-register)
* [Futurepass Auth](https://github.com/futureversecom/sdk-unity-futurepass/)
* [Sylo](https://github.com/futureversecom/sdk-unity-sylo)
* [UBF Interpreter](https://github.com/futureversecom/sdk-unity-ubf)

Consult these repositories' ReadMes for installation instructions.

## Overview

The UBF Execution Controller acts as an orchestration layer on top of the [UBF Interpreter](https://github.com/futureversecom/sdk-unity-ubf). It is responsible for setting up the execution context that the Interpreter requires to function effectively. While the Interpreter is responsible for translating and running the Blueprint logic itself, the Execution Controller is the component that determines what Blueprint to run, with what data, in what order, and under what context.

The Execution Controllers bridges the gap between high-level domain concepts and low-level blueprint execution. They abstract the complexity of managing inputs, resources, dependencies, and orchestration, leaving the interpreter to focus on what it does best: executing Blueprints.
​
## Core Responsibilities

Execution Controllers can serve a wide variety of scenarios, and their responsibilities depend on the particular use case. At a fundamental level, all controllers are responsible for ensuring that everything is properly prepared before blueprint execution begins. Think of the controller as the system that packages and resolves all dependencies, ensuring the interpreter has a complete and coherent context to operate within.

This Unity Execution Controller is designed specifically with Futureverse domain workflows in mind, including the handling of NFT metadata, complex asset trees (via the Asset Registry), and asset profiles. Some of these responsibilities include, but are not limited to:

* Resolving Asset Profiles: Retrieve, deserialize, and interpret asset profiles in order to resolve the correct version and variation of a given asset, including the parsing/render blueprints and artifact catalogs that accompany it.
* Managing Asset Trees: Understand and resolve tree-structured relationships from Asset Registry, where one asset may have other assets equipped onto it.
* Metadata Parsing & Blueprint Instantiation: Execute parsing blueprints to interpret raw metadata and feed the output into rendering blueprints as inputs. This chaining allows dynamic transformation of inputs prior to rendering.
* Artifact Provision: Provide the ability to download external artifacts through HTTPS or Decentralized Identifiers (DIDs), enabling compatibility with systems like Futureverse’s Sylos.
* Execution Ordering: Coordinate and sequence multiple blueprint executions across various entities to achieve a desired final result (e.g., rendering a full NFT asset with all attachments).

## Use Case: NFT Rendering

As mentioned above, this Execution Controller is designed primarily for rendering NFTs from metadata and leveraging other Futureverse technology. Here is how a typical flow works:

* Receive Asset Tree from Experience:: The asset tree—representing the full hierarchy of a target NFT and any attached or equipped assets—is passed into the execution controller by the surrounding application or experience.
* Resolve Asset Profiles: For each asset in the tree, retrieve and load the corresponding asset profile to then evaluate the appropriate version, variant, blueprints, and catalogs.
* Download Blueprints and Catalogs: Download and cache all relevant blueprints and artifact catalogs needed to perform the rendering operation.
* Parse Metadata: Where applicable, take raw metadata or traits associated with the asset and pass it into the parsing blueprint.
* Feed Parsed Inputs into Render Blueprint: Collect the outputs from the parsing blueprint and feed them into the inputs of the rendering blueprint.
* Execute Render Blueprint: Initiate execution of the top-level asset’s render blueprint, now fully configured with its required inputs.
* Provide Artifacts at Runtime: During execution, fulfill any artifact requests from the blueprint(s) by retrieving them from cache or downloading them on demand.

## Using the Execution Controller

To use the Execution Controller in your Unity project, you will need to add the `ExecutionController` component to a GameObject in your scene. This has two serialized fields, which require the `MonoClient` component from the Asset Register SDK and the `UBFRuntimeController` component from the UBF Interpreter SDK to be in your scene as well. 

<img width="612" height="628" alt="image" src="https://github.com/user-attachments/assets/c8dc002d-464f-489a-84c6-fa35f2d4d16a" />

With a reference to the `ExecutionController` component in C#, you can yield on the `RenderItem(IInventoryItem item)` method. This requires an `IInventoryItem` parameter. The default implementation of this interface is the `AssetRegisterInventoryItem`, which uses the Asset Register SDK to fetch all the data for an Asset. You can create a `AssetRegisterInventoryItem` via its static `FromData` or `FromAsset` methods. For a complete example, refer to the Asset Register sample that comes with the SDK. This sample shows how to:

* Query the Asset Register to get all assets in a given wallet
* Populate UI with the asset data from Asset Register
* Create `AssetRegisterInventoryItem` using the data from Asset Register
* How to call `RenderItem` using the `ExecutionController` component

Or you can see the script used here: https://github.com/futureversecom/sdk-unity-execution-controller/blob/main/Assets/Plugins/ExecutionController/Samples/AssetRegister/Scripts/AssetRegisterSample.cs
