using System;
using CollectionManager.Enums;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;

namespace OsuMemoryEventSource.Extensions
{
    public class ScPlayerScore
    {
        private readonly PlayerScore _playerScore;
        public string Username => _playerScore.Username;
        public int ModsEnum => _playerScore.Mods.Value;
        public int Mode => _playerScore.Mode;
        public ushort MaxCombo => _playerScore.MaxCombo;
        public virtual int Score => _playerScore.Score;
        public ushort Hit100 => _playerScore.Hit100;
        public ushort Hit300 => _playerScore.Hit300;
        public ushort Hit50 => _playerScore.Hit50;
        public ushort HitGeki => _playerScore.HitGeki;
        public ushort HitKatu => _playerScore.HitKatu;
        public ushort HitMiss => _playerScore.HitMiss;
        public DateTime Date => _playerScore.Date;
        public int? UserId => _playerScore.UserId;

        public double Accuracy { get; set; }
        public string Mods { get; set; }
        public OsuGrade Grade { get; set; }

        public ScPlayerScore(PlayerScore playerScore)
        {
            _playerScore = playerScore;
        }
    }
}