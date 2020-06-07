using Assets.Scripts.Editor.ArtifactPuller.AzureDevopsAPIResponses.FeedData;
using Assets.Scripts.Editor.ArtifactPuller.AzureDevopsAPIResponses.FeedsData;
using Assets.Scripts.Editor.ArtifactPuller.AzureDevopsAPIResponses.PackagesData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.ArtifactPuller
{
    public class AzureDevopsClient : IDisposable
    {
        private HttpClient _client;
        private IJsonSerializer _serializer;
        public AzureDevopsClient(string personalAccessToken, IJsonSerializer serializer)
        {
            _serializer = serializer;

            ConfigureHTTPClient(personalAccessToken);
        }
        #region Configuration
        private void ConfigureHTTPClient(string personalAccessToken)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetBase64PAT(personalAccessToken));
        }
        private string GetBase64PAT(string pat)
        {
            return Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", pat)));
        }
        #endregion
        public async Task<FeedsData> GetFeeds(string organization)
        {
            string requestURI =
                $"https://feeds.dev.azure.com/{organization}/_apis/packaging/feeds?api-version=5.0-preview.1";
            using (HttpResponseMessage response = await _client.GetAsync(requestURI))
            {
                string result = await response.Content.ReadAsStringAsync();
                return _serializer.Deserialize<FeedsData>(result);
            }
        }
        public async Task<FeedData> GetFeedData(string organization, string url)
        {
            using (HttpResponseMessage response = await _client.GetAsync(url))
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                FeedData data = _serializer.Deserialize<FeedData>(responseBody);
                return data;
            }
        }
        public async Task<PackagesData> GetPackages(string packagesUri)
        {
            using (HttpResponseMessage response = await _client.GetAsync(packagesUri))
            {
                string result = await response.Content.ReadAsStringAsync();
                return _serializer.Deserialize<PackagesData>(result);
            }
        }
        public async Task<PackagesData> GetPackage(string packagesUri)
        {
            using (HttpResponseMessage response = await _client.GetAsync(packagesUri))
            {
                string result = await response.Content.ReadAsStringAsync();
                return _serializer.Deserialize<PackagesData>(result);
            }
        }
        public async Task<byte[]> DownloadPackage(string organization, string feedName, string packageName)
        {
            var feeds = await this.GetFeeds(organization);
            var relatedFeed = feeds.value.First(val => val.fullyQualifiedName == feedName);
            string projectId = feeds.value[0].project.id;
            string uri = relatedFeed.url;
            var feedData = await this.GetFeedData(organization, uri);
            string packageUri = feedData._links.packages.href;
            var packages = await this.GetPackages(packageUri);
            var relatedPackage = packages.value.First(val => val.name == packageName);
            return await this.DownloadPackageByteArray(organization, projectId, relatedFeed.id,
                packageName, relatedPackage.versions[0].version);
        }
        private async Task<byte[]> DownloadPackageByteArray(string Organization, string ProjectID, string FeedID,
            string PackageName, string PackageVersion)
        {
            string uri = $"https://pkgs.dev.azure.com/{Organization}/{ProjectID}/" +
                         $"_apis/packaging/feeds/{FeedID}/nuget/packages/{PackageName}/versions/{PackageVersion}/content?api-version=5.1-preview.1";
            using (HttpResponseMessage response = await _client.GetAsync(uri))
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
