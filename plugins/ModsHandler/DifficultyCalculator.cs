using System;
using System.Collections.Generic;
using CollectionManager.DataTypes;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace ModsHandler
{
    public class DifficultyCalculator
    {
        readonly float ar0_ms = 1800f,
           ar5_ms = 1200f,
           ar10_ms = 450f;

        readonly float ar_ms_step1 = 120, // ar0-5 
            ar_ms_step2 = 150; // ar5-10 

        public Dictionary<string, float> ApplyMods(Beatmap map, Mods mods)
        {
            float od = map.OverallDifficulty;
            float ar = map.ApproachRate;
            float cs = map.CircleSize;
            float hp = map.HpDrainRate;
            double minBpm = map.MinBpm;
            double maxBpm = map.MaxBpm;
            var retValue = new Dictionary<string, float>();

            if ((mods & Mods.MapChanging) == 0)
            {
                retValue.Add("AR", ar);
                retValue.Add("CS", cs);
                retValue.Add("OD", od);
                retValue.Add("HP", hp);
                retValue.Add("MinBpm", (float)minBpm);
                retValue.Add("MaxBpm", (float)maxBpm);
                return retValue;
            }

            float speed = 1;
            if ((mods & Mods.Dt) != 0 || (mods & Mods.Nc) != 0)
                speed *= 1.5f;
            if ((mods & Mods.Ht) != 0)
                speed *= 0.75f;

            //hp
            if ((mods & Mods.Ez) != 0)
                hp *= 0.5f;
            else if ((mods & Mods.Hr) != 0)
                hp *= 1.4f;

            //bpm
            double modifier = 1;
            if ((mods & Mods.Dt) != 0)
            {
                modifier *= 1.5;
            }
            else if ((mods & Mods.Ht) != 0)
            {
                modifier *= 0.75;
            }

            minBpm *= modifier;
            maxBpm *= modifier;

            //ar 
            float ar_multiplier = 1;

            if ((mods & Mods.Hr) != 0)
                ar_multiplier *= 1.4f;
            if ((mods & Mods.Ez) != 0)
                ar_multiplier *= 0.5f;

            ar *= ar_multiplier;
            float arms = ar <= 5
            ? (ar0_ms - ar_ms_step1 * ar)
            : (ar5_ms - ar_ms_step2 * (ar - 5));

            //cs 
            float cs_multiplier = 1;
            if ((mods & Mods.Hr) != 0)
                cs_multiplier *= 1.3f;
            if ((mods & Mods.Ez) != 0)
                cs_multiplier *= 0.5f;

            // stats must be capped to 0-10 before HT/DT which bring them to a range 
            // of -5 to 11 for AR 
            arms = Math.Min(ar0_ms, Math.Max(ar10_ms, arms));

            // apply speed-changing mods 
            arms /= speed;

            // convert AR back into their stat form 
            ar = ar <= 5.0f
                ? ((ar0_ms - arms) / ar_ms_step1)
                : (5.0f + (ar5_ms - arms) / ar_ms_step2);

            cs *= cs_multiplier;
            cs = Math.Max(0.0f, Math.Min(10.0f, cs));

            if ((mods & Mods.Ez) != 0)
                od /= Math.Max(0, od / 2);
            else
                od = Math.Min(10, od * 1.4f);


            retValue.Add("AR", ar);
            retValue.Add("CS", cs);
            retValue.Add("OD", od);
            retValue.Add("HP", hp);
            retValue.Add("MinBpm", (float)minBpm);
            retValue.Add("MaxBpm", (float)maxBpm);
            if ((mods & Mods.SpeedChanging) == 0)
                return retValue;

            return retValue;
        }

    }
}