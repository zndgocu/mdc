namespace com.doosan.fms.mdc.MdcEngine.DataStruct
{
    public class Vector3
    {
        public Vector3()
        {
        }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }


        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public void SetXYZ(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public void Set(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }
    }
}
