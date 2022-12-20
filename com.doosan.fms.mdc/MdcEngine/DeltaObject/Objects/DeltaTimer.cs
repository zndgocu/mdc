using com.doosan.fms.mdc.MdcEngine.DeltaObject.DeltaEnum;
using com.doosan.fms.mdc.MdcEngine.DeltaObject.ObjectBase;
using System.Diagnostics;

namespace com.doosan.fms.mdc.MdcEngine.DeltaObject.Objects
{
    public class DeltaTimer : DeltaObjectBase
    {
        private Stopwatch _stopwatch;

        private long _lastMilliseconds = 0;
        private long _elapsedMilliseconds = 0;
        private double _hitMillseconds = 0;

        private double _fpsInterval = 0;
        private double _useFps = 180;

        public DeltaTimer() : base(DeltaObjectType.DeltaTimer)
        {
            _stopwatch = new Stopwatch();
        }

        public void Start(double fps)
        {
            if (fps < 1) fps = 180;
            CounterInterval(fps);
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public bool IsUpdate(out double deltaTime)
        {
            deltaTime = _elapsedMilliseconds - _lastMilliseconds;
            return (deltaTime > _fpsInterval);
        }

        private void CounterInterval(double fps)
        {
            _useFps = fps;
            _fpsInterval = 1000 / _useFps;
        }

        protected override void UpdateDelta()
        {
            _elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
        }

        public void HitDelta()
        {
            _hitMillseconds = _elapsedMilliseconds - _lastMilliseconds;
            _lastMilliseconds = _elapsedMilliseconds;
        }

        public double GetHitMillsec()
        {
            return _hitMillseconds;
        }

        public double GetCalcFps()
        {
            return 1000 / ((_hitMillseconds == 0) ? 1 : _hitMillseconds);
        }
    }
}
