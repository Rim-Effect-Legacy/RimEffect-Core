﻿namespace RimEffect
{
    using System;
    using System.Collections.Generic;
    using RimWorld;
    using Verse;
    using Verse.Sound;

    public class CompAbilities : VFEMech.ShieldMechBubble
    {

        private new Pawn Pawn => (Pawn) this.parent;

        private List<Ability> learnedAbilities = new List<Ability>();

        public           Ability currentlyCasting;

        private float energyMax;
        protected override float EnergyMax => this.energyMax;

        protected override float EnergyGainPerTick => 0f;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if(this.learnedAbilities == null)
                this.learnedAbilities = new List<Ability>();

            this.ticksToReset = Int32.MaxValue;
        }

        public void GiveAbility(AbilityDef abilityDef)
        {
            Ability ability = (Ability) Activator.CreateInstance(abilityDef.abilityClass);
            ability.def    = abilityDef;
            ability.pawn   = this.Pawn;
            ability.holder = this.Pawn;

            this.learnedAbilities.Add(ability);
        }
        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra()) 
                yield return gizmo;

            foreach (Ability ability in this.learnedAbilities) 
                yield return ability.GetGizmo();

            foreach (Hediff_Abilities hediff in this.Pawn.health.hediffSet.GetHediffs<Hediff_Abilities>())
            {
                foreach (Gizmo gizmo in hediff.DrawGizmos()) 
                    yield return gizmo;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref this.learnedAbilities, nameof(this.learnedAbilities), LookMode.Deep);
            Scribe_References.Look(ref this.currentlyCasting, nameof(this.currentlyCasting));

            if (this.learnedAbilities == null)
                this.learnedAbilities = new List<Ability>();
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                foreach (Ability ability in this.learnedAbilities)
                {
                    ability.holder = this.parent;
                }
            }
        }

        protected override void Break()
        {
            base.Break();
            this.energyMax = 0f;
        }

        protected override void Reset() => 
            this.ticksToReset = Int32.MaxValue;

        public bool ReinitShield(float newEnergy)
        {
            if (newEnergy < this.energy)
                return false;

            if (this.Pawn.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(this.Pawn.Position, this.Pawn.Map));
                MoteMaker.ThrowLightningGlow(this.Pawn.TrueCenter(), this.Pawn.Map, 3f);
            }

            this.ticksToReset = -1;
            this.energyMax    = newEnergy;
            this.energy       = newEnergy;
            return true;
        }
    }
}