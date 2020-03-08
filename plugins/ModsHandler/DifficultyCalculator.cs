﻿using System;
using System.Collections.Generic;
using CollectionManager.DataTypes;
using StreamCompanionTypes.DataTypes;

namespace ModsHandler
{
    public class DifficultyCalculator
    {
        readonly float od0_ms = 79.5f,
           od10_ms = 19.5f,
           ar0_ms = 1800f,
           ar5_ms = 1200f,
           ar10_ms = 450f;

        readonly float od_ms_step = 6,
            ar_ms_step1 = 120, // ar0-5 
            ar_ms_step2 = 150; // ar5-10 

        public Dictionary<string, float> ApplyMods(IBeatmap map, Mods mods)
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

            float od_multiplier = 1;
            if ((mods & Mods.Hr) != 0)
                od_multiplier *= 1.4f;
            if ((mods & Mods.Ez) != 0)
                od_multiplier *= 0.5f;

            od *= od_multiplier;
            float odms = (mods & Mods.Hr) != 0 && (mods & Mods.Dt) == 0 && (mods & Mods.Ht) == 0
                ? od0_ms - (od_ms_step * od)
                : od0_ms - (float)Math.Ceiling(od_ms_step * od);

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
            // of -4.42 to 11.08 for OD and -5 to 11 for AR 
            odms = Math.Min(od0_ms, Math.Max(od10_ms, odms));
            arms = Math.Min(ar0_ms, Math.Max(ar10_ms, arms));

            // apply speed-changing mods 
            odms /= speed;
            arms /= speed;

            // convert OD and AR back into their stat form 
            //od = (-(odms - od0_ms)) / od_ms_step; 
            od = (od0_ms - odms) / od_ms_step;
            ar = ar <= 5.0f
                ? ((ar0_ms - arms) / ar_ms_step1)
                : (5.0f + (ar5_ms - arms) / ar_ms_step2);

            cs *= cs_multiplier;
            cs = Math.Max(0.0f, Math.Min(10.0f, cs));

            retValue.Add("AR", ar);
            retValue.Add("CS", cs);
            retValue.Add("OD", od);
            retValue.Add("HP", hp);
            retValue.Add("MinBpm", (float)minBpm);
            retValue.Add("MaxBpm", (float)maxBpm);

            return retValue;
        }

    }
}