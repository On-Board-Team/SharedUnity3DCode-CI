# SharedUnity3DCode-CI
This repository is used as a source code for a Continuous Integration pipeline using [Azure DevOps](https://azure.microsoft.com/en-us/services/devops/)<br/>
Here's a [link](https://dev.azure.com/SharedUnity3DCode/SharedUnity3DCode-CI/_build) to the Azure DevOps project.

# Purpose
Visual Studio projects that are created within a Unity3D Project do not have that "Manage NuGet packages" button.
For that, this Unity3D Project contains a NuGet-like package manager that pulls artifacts that are built through an Azure DevOps pipeline

# Hierarchy
The [/SharedLibrary](/SharedLibrary) contains a .NET Standard library and a UnitTests project against it. A NuGet package is generated and pushed into this [ArtifactsFeed](https://dev.azure.com/SharedUnity3DCode/SharedUnity3DCode-CI/_packaging?_a=feed&feed=ArtifactsFeed%40Local)
That library is being built and the tests are executed in this Azure DevOps [pipeline](https://dev.azure.com/SharedUnity3DCode/SharedUnity3DCode-CI/_build)

The [/UnityProjects](/UnityProject) holds the Unity3D project that has the editor extension that pulls the artifacts given a specific configuration that you can create through the extension too. 


