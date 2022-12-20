using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace com.doosan.fms.mdc.MapData.Yaml
{
    public enum FmsYaml_Prop_Index
    {
        Build = 0,
        Level = 1,
        End,
    }

    public enum FmsYaml_Levels_Depth
    {
        L1 = 0
    }

    public class FmsYamlPropBuild
    {
        private string _name;

        public string Name { get => _name; set => _name = value; }
    }
    public class FmsYamlPropLevel
    {
        public const string LANES = "lanes";
        public const string VERTICES = "vertices";

        private List<FmsYamlLevelLane> _lanes;
        private List<FmsYamlLevelVertex> _vertices;

        public FmsYamlPropLevel()
        {
            _lanes = new List<FmsYamlLevelLane>();
            _vertices = new List<FmsYamlLevelVertex>();
        }

        public List<FmsYamlLevelLane> Lanes { get => _lanes; set => _lanes = value; }
        public List<FmsYamlLevelVertex> Vertices { get => _vertices; set => _vertices = value; }
    }

    public class FmsYamlLevelVertex
    {
        private decimal _poseX;
        private decimal _poseY;
        private string _name;
        private int _zone;

        public FmsYamlLevelVertex()
        {
        }

        public decimal PoseX { get => _poseX; set => _poseX = value; }
        public decimal PoseY { get => _poseY; set => _poseY = value; }
        public string Name { get => _name; set => _name = value; }
        public int Zone { get => _zone; set => _zone = value; }
    }

    public enum FmsYamlLevelVertex_Prop_Index
    {
        PoseX = 0,
        PoseY = 1,
        NameZone = 2,
    }
    public enum FmsYamlLevelVertex_Prop_NameZone_Index
    {
        Name = 0,
        Zone = 1,
    }


    public class FmsYamlLevelLane
    {
        private int _startVertexNo;
        private int _endVertexNo;

        public FmsYamlLevelLane()
        {
        }

        public int StartVertexNo { get => _startVertexNo; set => _startVertexNo = value; }
        public int EndVertexNo { get => _endVertexNo; set => _endVertexNo = value; }
    }

    public enum FmsYamlLevelLane_Prop_Index
    {
        StartVertexNo = 0,
        EndVertexNo = 1,
    }



    public class FmsYaml
    {
        private bool _init = false;
        private Stream _yamlStream = null;
        private Dictionary<object, object> _yamlObject = null;

        private FmsYamlPropBuild _propBuild;
        private FmsYamlPropLevel _propLevel;

        public FmsYamlPropBuild PropBuild { get => _propBuild; }
        public FmsYamlPropLevel PropLevel { get => _propLevel; }

        public FmsYaml(Stream yamlStream)
        {
            _yamlStream = yamlStream;
        }
        public bool InitializeYaml()
        {
            try
            {
                var sr = new StreamReader(_yamlStream);
                var dsr = new Deserializer();
                var yamlObject = dsr.Deserialize(sr);
                _yamlObject = yamlObject as Dictionary<object, object>;
                _init = true;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void SetBuildProp(FmsYamlPropBuild prop)
        {
            _propBuild = prop;
        }
        private void SetLevelProp(FmsYamlPropLevel prop)
        {
            _propLevel = prop;
        }

        public bool SetYaml()
        {
            if (_init == false) return false;
            if (_yamlObject.Count != (int)FmsYaml_Prop_Index.End) return false;
            var yamlList = _yamlObject.Values.ToList();

            FmsYamlPropBuild propBuild = new FmsYamlPropBuild();
            propBuild.Name = yamlList[(int)FmsYaml_Prop_Index.Build].ToString();
            SetBuildProp(propBuild);

            FmsYamlPropLevel propLevel = new FmsYamlPropLevel();
            var levels = (yamlList[(int)FmsYaml_Prop_Index.Level] as Dictionary<object, object>).ToList();
            var l1 = (levels[(int)FmsYaml_Levels_Depth.L1].Value as Dictionary<object, object>).ToList();
            foreach (var item in l1)
            {
                if (item.Key.ToString() == FmsYamlPropLevel.LANES)
                {
                    var lanes = item.Value as List<object>;
                    foreach (var lane in lanes)
                    {
                        var addLane = new FmsYamlLevelLane();
                        var laneAttr = lane as List<object>;
                        addLane.StartVertexNo = Convert.ToInt32(laneAttr[(int)FmsYamlLevelLane_Prop_Index.StartVertexNo]);
                        addLane.EndVertexNo = Convert.ToInt32(laneAttr[(int)FmsYamlLevelLane_Prop_Index.EndVertexNo]);
                        propLevel.Lanes.Add(addLane);
                    }
                }
                else if (item.Key.ToString() == FmsYamlPropLevel.VERTICES)
                {
                    var vertices = item.Value as List<object>;
                    foreach (var vertex in vertices)
                    {
                        var addVertex = new FmsYamlLevelVertex();
                        var vertexAttr = vertex as List<object>;
                        addVertex.PoseX = Convert.ToDecimal(vertexAttr[(int)FmsYamlLevelVertex_Prop_Index.PoseX]);
                        addVertex.PoseY = Convert.ToDecimal(vertexAttr[(int)FmsYamlLevelVertex_Prop_Index.PoseY]);

                        var nameZone = vertexAttr[(int)FmsYamlLevelVertex_Prop_Index.NameZone] as Dictionary<object, object>;
                        var nameZones = nameZone.Values.ToList();
                        addVertex.Name = nameZones[(int)FmsYamlLevelVertex_Prop_NameZone_Index.Name].ToString();
                        addVertex.Zone = Convert.ToInt32(nameZones[(int)FmsYamlLevelVertex_Prop_NameZone_Index.Zone]);
                        propLevel.Vertices.Add(addVertex);
                    }
                }
                else
                {
                    Console.WriteLine("프로그래밍 되지 않은 levels object code");
                }
            }
            SetLevelProp(propLevel);

            return true;
        }


    }
}
