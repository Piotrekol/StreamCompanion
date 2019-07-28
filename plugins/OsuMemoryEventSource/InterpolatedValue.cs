using System;

namespace OsuMemoryEventSource
{
    public enum InterpolationType
    {
        Linear, EaseIn
    }
    public class InterpolatedValue
    {

        public InterpolationType InterpolationType { get; set; } = InterpolationType.EaseIn;
        public double Current { get; private set; } = 0;
        private double _orginalValue = 0;
        private double _finalValue = 0;
        private double _transitionSpeed;
        private double _currentPosition = 0;

        public InterpolatedValue(double speed)
        {
            ChangeSpeed(speed);
        }

        private double Lerp(double @from, double @to, double by)
        {
            return @from * (1 - by) + @to * by;
        }

        private double EaseInPosition(double normalizedTime)
        {
            normalizedTime = Math.Max(0.0, Math.Min(1.0, normalizedTime));
            return 1.0 - Math.Sqrt(1.0 - normalizedTime * normalizedTime);
        }

        public void Tick()
        {
            _currentPosition += _transitionSpeed;
            if (_currentPosition > 1)
                _currentPosition = 1;
            if(InterpolationType==InterpolationType.EaseIn)
                Current = Lerp(_orginalValue, _finalValue, EaseInPosition(_currentPosition));
            else
                Current = Lerp(_orginalValue, _finalValue, _currentPosition);

        }

        public void Set(double value)
        {
            if (double.IsNaN(value))
            {
                value = 0;
            }

            if (double.IsNaN(Current))
            {
                Current = 0;
            }

            _currentPosition = 0;
            _orginalValue = Current;
            _finalValue = value;
        }

        public void ChangeSpeed(double speed)
        {
            if (speed <= 0) throw new ArgumentException(nameof(speed));

            _transitionSpeed = speed;
        }
    }
}