using System.ComponentModel;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum
{
    public enum PrefabUpdateType
    {
        All = 0,
        Background = 1,
        Animate = 2,
    }

    public enum PrefabType
    {
        [Description("Vertex")]
        Vertex = 0,
        [Description("Lane")]
        Lane = 1,
        [Description("Robot")]
        Robot = 2,
    }
}
