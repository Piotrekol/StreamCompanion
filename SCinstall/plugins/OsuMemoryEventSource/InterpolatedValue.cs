using System;

namespace OsuMemoryEventSource
{
    public enum InterpolationType
    {
        Linear, EaseIn, EaseOutQuint
    }

    public class RoundedInterpolatedValue : InterpolatedValue
    {
        public int DecimalPlaces { get; set; } = 3;
        public override double Current => Math.Round(base.Current, DecimalPlaces);

        public RoundedInterpolatedValue(double speed) : base(speed)
        {
        }
    }

    public class InterpolatedValue
    {
        public InterpolationType InterpolationType { get; set; } = InterpolationType.EaseOutQuint;
        public virtual double Current { get; private set; } = 0;
        private double _orginalValue = 0;
        private double _finalValue = 0;
        private double _transitionSpeed;
        private double _currentPosition = 0;

        public InterpolatedValue(double speed)
        {
            ChangeSpeed(speed);
        }

        private static double Lerp(double @from, double @to, double by)
        {
            return @from * (1 - by) + @to * by;
        }

        //https://easings.net/#easeInCirc
        private static double EaseInPosition(double normalizedTime)
        {
            normalizedTime = Math.Max(0.0, Math.Min(1.0, normalizedTime));
            return 1.0 - Math.Sqrt(1.0 - normalizedTime * normalizedTime);
        }

        //https://easings.net/#easeOutQuint
        private static double EaseOutQuint(double value)
        {
            return 1 - Math.Pow(1d - value, 3);
        }

        public void Tick()
        {
            _currentPosition += _transitionSpeed;
            if (_currentPosition > 1)
                _currentPosition = 1;
            switch (InterpolationType)
            {
                case InterpolationType.EaseIn:
                    Current = Lerp(_orginalValue, _finalValue, EaseInPosition(_currentPosition));
                    break;
                case InterpolationType.Linear:
                    Current = Lerp(_orginalValue, _finalValue, _currentPosition);
                    break;
                case InterpolationType.EaseOutQuint:
                    Current = Lerp(_orginalValue, _finalValue, EaseOutQuint(_currentPosition));
                    break;
            }
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

        public void Reset()
        {
            _currentPosition = _orginalValue = Current = _finalValue = 0;
        }

        public void ChangeSpeed(double speed)
        {
            if (speed <= 0) throw new ArgumentException(nameof(speed));

            _transitionSpeed = speed;
        }
    }
}