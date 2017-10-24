using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarframeAlerts
{
    public class Reward
    {
        public static string[] AURAS = new string[] { "Brief Respite", "Corrosive Projection", "Dead Eye", "Emp Aura", "Empowered Blades", "Enemy Radar", "Energy Siphon", "Growing Power", "Infested Impedance", "Loot Detector", "Physique", "Pistol Amp", "Pistol Scavenger", "Rejuvenation", "Rifle Amp", "Rifle Scavenger", "Shield Disruption", "Shotgun Amp", "Shotgun Scavenger", "Sniper Scavenger", "Speed Holster", "Sprint Boost", "Stand United", "Steel Charge", "Toxin Resistance" };
        public static string[] BLUEPRINTS = new string[] { "Ceramic Dagger", "Dark Dagger", "Dark Sword", "Exilus Adapter", "Forma", "Glaive", "Heat Dagger", "Heat Sword", "Jaw Sword", "Orokin Catalyst", "Orokin Reactor", "Pangolin Sword", "Plasma Sword", "Vauban Chassis", "Vauban Neuroptics", "Vauban Systems" };
        public static string[] MODS = new string[] { "Accelerated Blast", "Animal Instinct", "Armored Agility", "Blaze", "Chilling Reload", "Constitution", "Drifting Contact", "Focus Energy", "Fortitude", "Hammer Shot", "Ice Storm", "Lethal Torrent", "Rending Strike", "Seeking Fury", "Shred", "Streamlined Form", "Stunning Speed", "Vigor", "Wildfire" };
        public static string[] RESOURCES = new string[] { "Alloy Plate", "Argon Crystal", "Circuits", "Control Module", "Detonite Injector", "Ferrite", "Fieldron", "Gallium", "Kavat Genetic Code", "Morphics", "Mutagen Mass", "Mutalist Alad V Nav Coordinate", "Neural Sensors", "Nano Spores", "Neurodes", "Nitain Extract", "Orokin Cell", "Oxium", "Polymer Bundle", "Plastids", "Rubedo", "Salvage", "Synthula", "Tellurium", "Void Traces" };
        public static string[] WEAPON_PARTS = new string[] { "Dera Vandal Blueprint", "Dera Vandal Barrel", "Dera Vandal Receiver", "Dera Vandal Stock",
                                                             "Karak Wraith Blueprint", "Karak Wraith Barrel", "Karak Wraith Receiver", "Karak Wraith Stock",
                                                             "Latron Wraith Blueprint", "Latron Wraith Barrel", "Latron Wraith Receiver", "Latron Wraith Stock",
                                                             "Sheev Blueprint", "Sheev Blade", "Sheev Heatsink", "Sheev Hilt",
                                                             "Snipetron Vandal Blueprint", "Snipetron Vandal Barrel", "Snipetron Vandal Receiver", "Snipetron Vandal Stock",
                                                             "Strun Wraith Blueprint", "Strun Wraith Barrel", "Strun Wraith Receiver", "Strun Wraith Stock",
                                                             "Twin Vipers Wraith Blueprint", "Twin Vipers Wraith Barrels", "Twin Vipers Wraith Receivers", "Twin Vipers Wraith Link" };

        public RewardType? type;
        public string name;

        public static Reward FromString(string rewardString)
        {
            Reward reward = new Reward();
            reward.name = rewardString;
            if (AURAS.Contains(rewardString))
                reward.type = RewardType.Aura;
            else if (BLUEPRINTS.Contains(rewardString) || rewardString.EndsWith("Helmet") || rewardString.EndsWith("Skin"))
                reward.type = RewardType.Blueprint;
            else if (MODS.Contains(rewardString))
                reward.type = RewardType.Mod;
            else if (rewardString == "Kubrow Egg")
                reward.type = RewardType.Item;
            else if (RESOURCES.Contains(rewardString))
                reward.type = RewardType.Resource;
            else if (WEAPON_PARTS.Contains(rewardString))
                reward.type = RewardType.WeaponPart;
            else
                throw new Exception("Unrecognized reward.");
            return reward;
        }
    }

    public enum RewardType
    {
        Aura,
        Blueprint,
        Item,
        Mod,
        Resource,
        WeaponPart,
    }
}
