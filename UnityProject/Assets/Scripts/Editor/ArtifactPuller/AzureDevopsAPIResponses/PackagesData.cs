using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.ArtifactPuller.AzureDevopsAPIResponses.PackagesData
{
    public class View
    {
        public string id { get; set; }
        public string name { get; set; }
        public object url { get; set; }
        public string type { get; set; }
    }

    public class Version
    {
        public string id { get; set; }
        public string normalizedVersion { get; set; }
        public string version { get; set; }
        public bool isLatest { get; set; }
        public bool isListed { get; set; }
        public string storageId { get; set; }
        public IList<View> views { get; set; }
        public DateTime publishDate { get; set; }
    }

    public class PackageSelf
    {
        public string href { get; set; }
    }

    public class Feed
    {
        public string href { get; set; }
    }

    public class Versions
    {
        public string href { get; set; }
    }

    public class PackageLinks
    {
        public PackageSelf self { get; set; }
        public Feed feed { get; set; }
        public Versions versions { get; set; }
    }

    public class Value
    {
        public string id { get; set; }
        public string normalizedName { get; set; }
        public string name { get; set; }
        public string protocolType { get; set; }
        public string url { get; set; }
        public IList<Version> versions { get; set; }
        public PackageLinks _links { get; set; }
    }

    public class PackagesData
    {
        public int count { get; set; }
        public IList<Value> value { get; set; }
    }
}
