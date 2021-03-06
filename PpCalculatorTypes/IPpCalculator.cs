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
        string[] Mods { get; set; }
        int Misses { get; set; }
        int? Mehs { get; set; }
        int? Goods { get; set; }
        int? Katsus { get; set; }
        int RulesetId { get; }
        double BeatmapLength { get; }
        void PreProcess(string file);
        double Calculate(double startTime, double endTime = double.NaN, Dictionary<string, double> categoryAttribs = null);
        DifficultyAttributes AttributesAt(double time);
        double Calculate(double? endTime = null, Dictionary<string, double> categoryAttribs = null);
        double Calculate(CancellationToken cancellationToken, double? endTime = null, Dictionary<string, double> categoryAttribs = null);
        int GetMaxCombo(int? fromTime = null);
        bool IsBreakTime(double time);
        object Clone();
    }
}