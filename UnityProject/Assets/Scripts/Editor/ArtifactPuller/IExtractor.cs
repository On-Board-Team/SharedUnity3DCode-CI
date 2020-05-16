using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.ArtifactPuller
{
    public interface IExtractor
    {
        void ExtractIntoFolder(string zipFilePath, string outputFolder);
    }
}
