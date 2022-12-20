using com.doosan.fms.mdc.MdcEngine.DataStruct;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.ComponentBase;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.PrefabComponentEnum;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json.Serialization;
using static com.doosan.fms.commonLib.Extension.VariableExtension;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.Components
{
    public class PrefabComponentTransform : PrefabComponentBase
    {
        private Vector3 _position;
        private Vector3 _rotation;

        [JsonInclude]
        public Vector3 Position { get => _position; }
        [JsonInclude]
        public Vector3 Rotation { get => _rotation; }

        public override JObject GetJson()
        {
            JObject result = new JObject();

            JObject objPosition = new JObject();
            JProperty posX = new JProperty(nameof(_position.X), _position.X);
            JProperty posY = new JProperty(nameof(_position.Y), _position.Y);
            JProperty posZ = new JProperty(nameof(_position.Z), _position.Z);
            objPosition.Add(posX);
            objPosition.Add(posY);
            objPosition.Add(posZ);

            JObject objRotation = new JObject();
            JProperty rotX = new JProperty(nameof(_rotation.X), _rotation.X);
            JProperty rotY = new JProperty(nameof(_rotation.Y), _rotation.Y);
            JProperty rotZ = new JProperty(nameof(_rotation.Z), _rotation.Z);
            objRotation.Add(rotX);
            objRotation.Add(rotY);
            objRotation.Add(rotZ);

            JProperty position = new JProperty(POSITION, objPosition);
            JProperty rotation = new JProperty(ROTATION, objRotation);
            JProperty prefabComponentType = new JProperty(nameof(PrefabComponentType), PrefabComponentType.Transform);

            result.Add(position);
            result.Add(rotation);
            result.Add(prefabComponentType);

            return result;
        }

        public PrefabComponentTransform() : base(PrefabComponentType.Transform)
        {
            _position = new Vector3();
            _rotation = new Vector3();
        }
        public void UpdateTransformPosition(Vector3 position, Vector3 rotation)
        {
            _position.Set(position);
            _rotation.Set(rotation);
        }

        public override void Update(FmsModel item, PrefabType prefabType, double deltaTime, double mps, double resolution)
        {
            if (prefabType == PrefabType.Robot)
            {
                //임시
                double speed = 1d;

                //#region pose
                var prevPosition = new Vector3(_position);
                var currPosition = new Vector3(item.GetTransformPositionX(), item.GetTransformPositionY(), item.GetTransformPositionZ());

                var directionX = (prevPosition.X > currPosition.X) ? -1 : 1;
                var directionY = (prevPosition.Y > currPosition.Y) ? -1 : 1;
                var directionZ = (prevPosition.Z > currPosition.Z) ? -1 : 1;

                var minimizeDistance = 10;
                var distanceX = Math.Abs(prevPosition.X - currPosition.X);
                if (distanceX < minimizeDistance) distanceX = minimizeDistance;
                var distanceY = Math.Abs(prevPosition.Y - currPosition.Y);
                if (distanceY < minimizeDistance) distanceY = minimizeDistance;
                var distanceZ = Math.Abs(prevPosition.Z - currPosition.Z);
                if (distanceZ < minimizeDistance) distanceZ = minimizeDistance;

                double fpsDriveSpeed = ((speed / 60d / 360d) * deltaTime);
                //double fpsDriveSpeed = 1;

                var updateX = prevPosition.X + ((distanceX * fpsDriveSpeed) * directionX);
                if (directionX > 0)
                {
                    if (updateX > currPosition.X)
                    {
                        updateX = currPosition.X;
                    }
                }
                else
                {
                    if (updateX < currPosition.X)
                    {
                        updateX = currPosition.X;
                    }
                }
                var updateY = prevPosition.Y + ((distanceY * fpsDriveSpeed) * directionY);
                if (directionY > 0)
                {
                    if (updateY > currPosition.Y)
                    {
                        updateY = currPosition.Y;
                    }
                }
                else
                {
                    if (updateY < currPosition.Y)
                    {
                        updateY = currPosition.Y;
                    }
                }
                var updateZ = prevPosition.Z + ((distanceZ * fpsDriveSpeed) * directionZ);
                if (directionZ > 0)
                {
                    if (updateZ > currPosition.Z)
                    {
                        updateZ = currPosition.Z;
                    }
                }
                else
                {
                    if (updateZ < currPosition.Z)
                    {
                        updateZ = currPosition.Z;
                    }
                }

                _position.SetXYZ(updateX, updateY, updateZ); //position setting

                #region rot
                //euler 반시계 방향

                var prevRotation = new Vector3(_rotation);
                var currRotation = new Vector3(item.GetTransformRotationX(), item.GetTransformRotationY(), item.GetTransformRotationZ());


                deltaTime = 43.56982421875;

                var prevRotationValue = prevRotation.Z * (180 / Math.PI);  //rad to deg
                var dataRotationValue = currRotation.Z; //pose_theta;
                var distanceTemp = 360 - (prevRotationValue - dataRotationValue);
                var distanceClockWise = (distanceTemp == 0) ? 0 : (distanceTemp % 360);
                var distanceCounterClockWise = (distanceTemp == 0) ? 0 : (distanceTemp % 360);
                if (distanceClockWise > 0 || distanceCounterClockWise > 0)
                {
                    var rotationDirection = (distanceCounterClockWise > distanceClockWise) ? 1 : -1;
                    var selectRotationValue = (rotationDirection < 0) ? distanceCounterClockWise : distanceClockWise;

                    var fpsAngleSpeed = ((speed / 60 / 360) * deltaTime) * mps * resolution;
                    var addRotationValue = (selectRotationValue * fpsAngleSpeed) * rotationDirection;
                    var updateRotationValue = (prevRotationValue + addRotationValue) % 360;
                    //각도 목표치 근사 시 제자리값 변환할것
                    prevRotation.Z = Math.PI * updateRotationValue / 180; //deg to rad
                }
                #endregion

                _rotation.SetXYZ(_rotation.X, _rotation.Y, _rotation.Z); //rotation setting
            }
            else if (prefabType == PrefabType.Vertex)
            {
                _position.SetXYZ(item.GetTransformPositionX(), item.GetTransformPositionY(), item.GetTransformPositionZ());
            }
            else if (prefabType == PrefabType.Lane)
            {
                _position.SetXYZ(item.GetTransformPositionX(), item.GetTransformPositionY(), item.GetTransformPositionZ());
            }
            else
            {

            }
        }
    }
}
