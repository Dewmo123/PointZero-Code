using System;

namespace Server.Utiles
{
    public class CountTime
    {
        private int _endTime;
        private float _currentTime;
        private int _timeInterval;
        private Action<double> _elapsed;
        private Action _callback;
        private float _nextElapsedTime;
        public bool IsRunning { get; private set; }
        public CountTime(Action<double> elapsed, Action callback, int endTime, int interval)
        {
            _callback = callback;
            _elapsed = elapsed;
            _timeInterval = interval;
            _endTime = endTime;
            IsRunning = false;
        }
        public CountTime(Action<double> elapsed, Action callback, int interval)
        {
            _callback = callback;
            _elapsed = elapsed;
            _timeInterval = interval;
            IsRunning = false;
        }
        public void SetEndTime(int endTime)
        {
            _endTime = endTime;
        }
        public void StartCount()
        {
            _currentTime = 0;
            _nextElapsedTime = 0;
            IsRunning = true;
        }
        public void UpdateDeltaTime()
        {
            if (!IsRunning)
                return;

            _currentTime += Time.deltaTime;
            float intervalSeconds = _timeInterval <= 0 ? 0 : _timeInterval / 1000f;
            if (intervalSeconds <= 0)
            {
                HandleTimerElapsed();
                return;
            }

            while (IsRunning && _currentTime >= _nextElapsedTime)
            {
                HandleTimerElapsed();
                _nextElapsedTime += intervalSeconds;
            }
        }
        private void HandleTimerElapsed()
        {
            _elapsed?.Invoke(Math.Min(_currentTime, _endTime));
            if (_currentTime >= _endTime)
            {
                _currentTime = 0;
                _nextElapsedTime = 0;
                IsRunning = false;
                _callback?.Invoke();
            }
        }
        public void Abort(bool invokeCallback = true)
        {
            Console.WriteLine("Count Dispose");
            IsRunning = false;
            _currentTime = 0;
            _nextElapsedTime = 0;
            if (invokeCallback)
                _callback?.Invoke();
        }
    }
}
