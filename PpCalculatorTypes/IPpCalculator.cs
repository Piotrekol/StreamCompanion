using System.Collections.Generic;
using System.Threading;

namespace PpCalculatorTypes
{
    public interface IPpCalculator
    {
        double Accuracy { get; set; }
        int? Combo { get; set; }
        double PercentCombo { get; set; }
        int Score { get; set; }
        double ScoreMultiplier { get; }
        string[] Mods { get; set; }
        int Misses { get; set; }
        int? Mehs { get; set; }
        int? Goods { get; set; }
        int? Katus { get; set; }
        int? Hit300 { get; set; }
        int RulesetId { get; }
        double BeatmapLength { get; }
        void PreProcess(string file);
        DifficultyAttributes DifficultyAttributesAt(double time);
        PerformanceAttributes Calculate(CancellationToken cancellationToken, double? startTime = null, double? endTime = null);
        int GetMaxCombo(int? fromTime = null);
        bool IsBreakTime(double time);
        double FirstHitObjectTime();
        public IEnumerable<BreakPeriod> Breaks();
        public IEnumerable<TimingPoint> TimingPoints();

        object Clone();
    }
}