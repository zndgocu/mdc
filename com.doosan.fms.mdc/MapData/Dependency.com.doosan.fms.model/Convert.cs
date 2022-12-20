using com.doosan.fms.mdc.MapData.Yaml;
using com.doosan.fms.model.Runtime;
using System.Collections.Generic;

namespace com.doosan.fms.mdc.MapData.Dependency.com.doosan.fms.model
{
    public static class Convert
    {
        public static List<VertexInfo> GetFmsModelVertices(FmsYaml fmsYaml)
        {
            if (fmsYaml.PropLevel == null) return null;
            if (fmsYaml.PropLevel.Vertices == null) return null;
            List<VertexInfo> vertices = new List<VertexInfo>();
            foreach (var vertex in fmsYaml.PropLevel.Vertices)
            {
                vertices.Add(new VertexInfo(fmsYaml.PropBuild.Name
                                        , vertex.PoseX
                                        , vertex.PoseY
                                        , vertex.Name
                                        , vertex.Zone
                            )
                         );
            }
            return vertices;
        }


        public static List<LaneInfo> GetFmsModelLanes(FmsYaml fmsYaml)
        {
            if (fmsYaml.PropLevel == null) return null;
            if (fmsYaml.PropLevel.Lanes == null) return null;
            List<LaneInfo> lanes = new List<LaneInfo>();
            foreach (var lane in fmsYaml.PropLevel.Lanes)
            {
                lanes.Add(new LaneInfo(fmsYaml.PropBuild.Name
                                , lane.StartVertexNo
                                , lane.EndVertexNo
                            )
                         );
            }

            return lanes;
        }
    }
}
