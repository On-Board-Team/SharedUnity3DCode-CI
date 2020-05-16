using System.Collections.Generic;

namespace Assets.Scripts.Editor.ArtifactPuller.AzureDevopsAPIResponses.FeedsData
{
    public class Self
    {
        public string href { get; set; }
    }

    public class Packages
    {
        public string href { get; set; }
    }

    public class Permissions
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public Packages packages { get; set; }
        public Permissions permissions { get; set; }
    }

    public class UpstreamSource
    {
        public string id { get; set; }
        public string name { get; set; }
        public string protocol { get; set; }
        public string location { get; set; }
        public string displayLocation { get; set; }
        public string upstreamSourceType { get; set; }
        public string status { get; set; }
    }

    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string visibility { get; set; }
    }

    public class FeedsValue
    {
        public string description { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
        public bool hideDeletedPackageVersions { get; set; }
        public string defaultViewId { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public object viewId { get; set; }
        public object viewName { get; set; }
        public string fullyQualifiedName { get; set; }
        public string fullyQualifiedId { get; set; }
        public IList<UpstreamSource> upstreamSources { get; set; }
        public string capabilities { get; set; }
        public Project project { get; set; }
        public bool? upstreamEnabled { get; set; }
    }

    public class FeedsData
    {
        public int count { get; set; }
        public IList<FeedsValue> value { get; set; }
    }


}