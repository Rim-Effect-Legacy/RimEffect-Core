﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimEffect
{
    [DefOf]
    public static class RE_DefOf
    {
        public static ThingDef RE_AmmoCryoBelt;
        public static ThingDef RE_AmmoDisruptorBelt;
        public static ThingDef RE_AmmoExplosiveBelt;
        public static ThingDef RE_AmmoIncendiaryBelt;
        public static ThingDef RE_AmmoPiercingBelt;
        public static ThingDef RE_AmmoToxicBelt;

        public static HediffDef Hypothermia;
        public static DamageDef RE_BombNoShake;
    }
}