using Assets.Scripts.Editor.ArtifactPuller.AzureDevopsAPIResponses.FeedData;
using Assets.Scripts.Editor.ArtifactPuller.AzureDevopsAPIResponses.FeedsData;
using Assets.Scripts.Editor.ArtifactPuller.AzureDevopsAPIResponses.PackagesData;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Editor.ArtifactPuller
{
    public class Extractor : IExtractor
    {
        public void ExtractIntoFolder(string zipFilePath, string outputFolder)
        {
            new FastZip().ExtractZip(zipFilePath, outputFolder, null);
        }
    }
    public class Serializer : IJsonSerializer
    {
        public T Deserialize<T>(string serializedData)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(serializedData);
        }

        public string Serialize(object target)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(target);
        }
    }

    public class PullerConfig
    {
        //Required
        public string PersonalAccessToken = "<pat from AzureDevops>";
        public string Organization = "<Azure Devops organization>";
        public string FeedName = "<Name of the ArtifactFeeds>";
        public string PackageName = "<Package Name>";
    }
    public class ArtifactPuller : EditorWindow
    {
        private static PullerConfig _internalConfig;
        //You need to supply the class with the correct instances of IExtractor and IJsonSerializer.
        private static IExtractor _extractor;
        private static IJsonSerializer _serializer;

        private static PullerConfig Config
        {
            get
            {
                if (_internalConfig == null)
                    LoadConfigOrCreate();
                return _internalConfig;
            }
        }

        [MenuItem("My Extensions/Configuration Puller")]
        public static void ShowWindow()
        {
            LoadConfigOrCreate();
            EditorWindow.GetWindow(typeof(ArtifactPuller));
        }
        public string TargetDLLsPath { get { return Path.Combine(Application.dataPath, "Plugins/"); } }
        async void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            Config.PersonalAccessToken =
                EditorGUILayout.TextField("Personal Access Token", Config.PersonalAccessToken);
            Config.Organization = EditorGUILayout.TextField("Organization", Config.Organization);
            Config.FeedName = EditorGUILayout.TextField("FeedName", Config.FeedName);
            Config.PackageName = EditorGUILayout.TextField("PackageName", Config.PackageName);
            if (GUILayout.Button("Save Config"))
            {
                string configJsonPath =
                    Path.Combine("Assets", "Scripts", "Editor", "ArtifactPuller", "puller_config.json");
                File.WriteAllText(configJsonPath, _serializer.Serialize(Config));
            }

            if (GUILayout.Button("Download"))
            {
                using (AzureDevopsClient client = new AzureDevopsClient(Config.PersonalAccessToken, _serializer))
                {
                    try
                    {
                        var byteArray = await client.DownloadPackage(Config.Organization, Config.FeedName, Config.PackageName);
                        if (!Directory.Exists(TargetDLLsPath))
                            Directory.CreateDirectory(TargetDLLsPath);

                        string temporaryDecompressionPath = Path.Combine(TargetDLLsPath, "tmp_zipFolder");
                        if (!Directory.Exists(temporaryDecompressionPath))
                            Directory.CreateDirectory(temporaryDecompressionPath);

                        string zipFilePath = Path.Combine(TargetDLLsPath, "packageZip.rar");
                        File.WriteAllBytes(zipFilePath, byteArray);

                        _extractor.ExtractIntoFolder(zipFilePath, temporaryDecompressionPath);

                        string[] dllResults = Directory.GetFiles(temporaryDecompressionPath, Config.PackageName + ".dll",
                            SearchOption.AllDirectories);

                        string dllOutputPath = Path.Combine(TargetDLLsPath, Path.GetFileName(dllResults[0]));
                        File.Copy(dllResults[0], dllOutputPath, true);

                        File.Delete(zipFilePath);
                        Directory.Delete(temporaryDecompressionPath, true);
                        Debug.LogFormat("Successfully Extracted Packages at {0}", dllOutputPath);
                    }
                    catch (Exception exp)
                    {
                        Debug.LogErrorFormat("Error while downloading. Exp: {0}. St:{1}", exp.Message, exp.StackTrace);
                    }
                }
            }
        }
        private static void LoadConfigOrCreate()
        {
            string configJsonPath = Path.Combine("Assets", "Scripts", "Editor", "ArtifactPuller", "puller_config.json");
            if (!File.Exists(configJsonPath))
            {
                _internalConfig = new PullerConfig();
                File.WriteAllText(configJsonPath, _serializer.Serialize(Config));
            }
            else
            {
                _internalConfig = _serializer.Deserialize<PullerConfig>(File.ReadAllText(configJsonPath));
            }
        }


    }
}