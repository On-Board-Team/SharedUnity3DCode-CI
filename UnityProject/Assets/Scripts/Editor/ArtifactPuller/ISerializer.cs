using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.ArtifactPuller
{
    public interface IJsonSerializer
    {
        string Serialize(object target);
        T Deserialize<T>(string serializedData);
    }
}
