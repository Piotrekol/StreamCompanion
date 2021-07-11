using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CollectionManager.Enums;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using StreamCompanion.Common.Helpers;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces.Services;
using Mods = CollectionManager.DataTypes.Mods;

namespace OsuMemoryEventSource.Extensions
{
    public static class PlayerScoreExtensions
    {
        private static Dictionary<PlayerScore, ScPlayerScore> _playerScores = new Dictionary<PlayerScore, ScPlayerScore>(64);
        public static ScPlayerScore Convert(this PlayerScore playerScore, IModParser modParser)
        {
            if (!_playerScores.TryGetValue(playerScore, out var scPlayerScore))
                _playerScores[playerScore] = scPlayerScore = new ScPlayerScore(playerScore);

            scPlayerScore.Accuracy = OsuScore.CalculateAccuracy((PlayMode)scPlayerScore.Mode, scPlayerScore.Hit50, scPlayerScore.Hit100, scPlayerScore.Hit300, scPlayerScore.HitMiss, scPlayerScore.HitGeki, scPlayerScore.HitKatu) * 100;
            scPlayerScore.Grade = OsuScore.CalculateGrade((PlayMode)scPlayerScore.Mode, (Mods)scPlayerScore.Mods, scPlayerScore.Accuracy, scPlayerScore.Hit50, scPlayerScore.Hit100, scPlayerScore.Hit300, scPlayerScore.HitMiss);
            scPlayerScore.ModsStr = modParser.GetModsFromEnum(scPlayerScore.Mods, true);

            return scPlayerScore;
        }

        public static IEnumerable<ScPlayerScore> Convert(this List<PlayerScore> playerScores, IModParser modParser) => playerScores.Select(ps => ps.Convert(modParser));
    }
}