using Assets.Scripts.Editor.ArtifactPuller.AzureDevopsAPIResponses;
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
namespace Assets.Scripts.Editor.ArtifactPuller
{
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
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        client.DefaultRequestHeaders.Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(
                                System.Text.ASCIIEncoding.ASCII.GetBytes(
                                    string.Format("{0}:{1}", "", Config.PersonalAccessToken))));

                        var feeds = await GetFeeds(client, Config.Organization);
                        var relatedFeed = feeds.value.First(val => val.fullyQualifiedName == Config.FeedName);

                        var packages = await GetPackages(client, Config.Organization, relatedFeed.id);
                        var relatedPackage = packages.value.First(val => val.name == Config.PackageName);

                        var byteArray = await DownloadPackage(client, Config.Organization, relatedFeed.id,
                            Config.PackageName, relatedPackage.versions[0].version);


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
                        string[] xmlResults = Directory.GetFiles(temporaryDecompressionPath, Config.PackageName + ".xml",
                            SearchOption.AllDirectories);

                        string dllOutputPath = Path.Combine(TargetDLLsPath, Path.GetFileName(dllResults[0]));
                        string xmlOutputPath = Path.Combine(TargetDLLsPath, Path.GetFileName(xmlResults[0]));
                        Debug.LogFormat("dll:{0} || xml:{1}", dllOutputPath, xmlOutputPath);
                        File.Copy(dllResults[0], dllOutputPath, true);
                        File.Copy(xmlResults[0], xmlOutputPath, true);

                        File.Delete(zipFilePath);
                        Directory.Delete(temporaryDecompressionPath, true);
                        Debug.LogFormat("Successfully Extracted Packages at {0}", dllOutputPath);
                    }
                    catch (Exception exp)
                    {
                        Debug.LogErrorFormat("Error while downloading. Exp: {0}", exp.Message);
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


        public async Task<AzureDevOps_FeedData> GetFeeds(HttpClient client, string organization)
        {
            string requestURI =
                $"https://feeds.dev.azure.com/{organization}/_apis/packaging/feeds?api-version=5.0-preview.1";
            Debug.Log(requestURI);
            using (HttpResponseMessage response = await client.GetAsync(requestURI))
            {
                response.EnsureSuccessStatusCode();
                string result = await response.Content.ReadAsStringAsync();

                return _serializer.Deserialize<AzureDevOps_FeedData>(result);
            }
        }

        public async Task<AzureDevOps_PackageData> GetPackages(HttpClient client, string organization, string feedId)
        {
            using (HttpResponseMessage response = await client.GetAsync(
                $"https://feeds.dev.azure.com/{organization}/_apis/packaging/Feeds/{feedId}/packages?api-version=5.0-preview.1")
            )
            {
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                AzureDevOps_PackageData data = _serializer.Deserialize<AzureDevOps_PackageData>(responseBody);
                return data;
            }
        }

        public static async Task<byte[]> DownloadPackage(HttpClient client, string Organization, string FeedID,
            string PackageName, string PackageVersion)
        {
            string uri = $"https://pkgs.dev.azure.com/{Organization}/" +
                         //$"https://pkgs.dev.azure.com/{Organization}/{ProjectID}/" +
                         $"_apis/packaging/feeds/{FeedID}/nuget/packages/{PackageName}/versions/{PackageVersion}/content?api-version=5.1-preview.1";
            using (HttpResponseMessage response = client.GetAsync(uri).Result)
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
    }
}