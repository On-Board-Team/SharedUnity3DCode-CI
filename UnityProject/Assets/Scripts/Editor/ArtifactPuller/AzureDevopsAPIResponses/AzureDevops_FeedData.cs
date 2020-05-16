using System.Collections.Generic;

namespace Assets.Scripts.Editor.ArtifactPuller.AzureDevopsAPIResponses
{
    public class AzureDevOps_FeedData
    {
        public int count { get; set; }
        public List<FeedData> value { get; set; }


        public class Links
        {
            public Self self { get; set; }
            public Packages packages { get; set; }
            public Permissions permissions { get; set; }
        }

        public class FeedData
        {
            public object description { get; set; }
            public string url { get; set; }
            public Links _links { get; set; }
            public bool hideDeletedPackageVersions { get; set; }
            public string defaultViewId { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public bool upstreamEnabled { get; set; }
            public object viewId { get; set; }
            public object viewName { get; set; }
            public string fullyQualifiedName { get; set; }
            public string fullyQualifiedId { get; set; }
            public List<object> upstreamSources { get; set; }
            public string capabilities { get; set; }
        }


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
    }
}