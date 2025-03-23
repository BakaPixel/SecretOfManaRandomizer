using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System.Collections.Generic;
using System.Linq;
using SoMRandomizer.native;
using static SoMRandomizer.processing.common.SomVanillaValues;
namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// A collection of open world prize locations, that can be filtered by selected options.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldLocations
    {
        public const string DEPENDENCY_GIRL_SPELLS = "girlCaster";
        public const string DEPENDENCY_SPRITE_SPELLS = "spriteCaster";
        public const string DEPENDENCY_ELINEE_ENTRY = "elinee"; // whip and cutting weapon or axe
        public const string DEPENDENCY_CUTTING_WEAPON = "cuttingWeapon";
        public const string DEPENDENCY_MATANGO_ENTRY = "matango";

        public static List<PrizeLocation> getForSelectedOptions(RandoSettings settings, RandoContext context)
        {
            bool restrictiveLogic = settings.get(OpenWorldSettings.PROPERTYNAME_LOGIC_MODE) == "restrictive";
            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            bool fastManaFort = context.workingData.getBool(OpenWorldGoalProcessor.MANA_FORT_ACCESSIBLE_INDICATOR);
            bool anySpellTriggers = context.workingData.getBool(OpenWorldClassSelection.ANY_MAGIC_EXISTS);
            bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
            bool anyCharsAdded = !context.workingData.getBool(OpenWorldCharacterSelection.START_SOLO);
            Dictionary<int, byte> crystalOrbColorMap = ElementSwaps.getCrystalOrbElementMap(context);

            List<string> grandPalaceBossDependencies = new List<string>();
            List<string> manafortDependencies = new List<string>();

            string _earthPalaceElement = (!anySpellTriggers) ? "no" : SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_EARTHPALACE], false);
            string _firePalaceElement1 = (!anySpellTriggers) ? "no" : SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE1], false);
            string _firePalaceElement2 = (!anySpellTriggers) ? "no" : SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE2], false);
            string _firePalaceElement3 = (!anySpellTriggers) ? "no" : SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE3], false);
            string _lunaPalaceElement = (!anySpellTriggers) ? "no" : SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_LUNAPALACE], false);
            string _matangoCaveElement = (!anySpellTriggers) ? "no" : SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_MATANGO], false);
            string _upperLandElement = "no";
            if (flammieDrumInLogic)
            {
                _upperLandElement = (!anySpellTriggers) ? "no" : SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_UPPERLAND], false);
            }

            populateDependencies(settings, context, grandPalaceBossDependencies, manafortDependencies);

            List<PrizeLocation> allLocations = new List<PrizeLocation>();

            // all event numbers here (second param to PrizeLocation) should correspond to ones created in PrizeEvents, containing the OPENWORLD_EVENT_INJECTION_PATTERN,
            // where the event data for the randomized prize will be injected.
            allLocations.Add(new PrizeLocation("mech rider 3 (new item)", 0x4a4, 0, new string[] { }, new string[] { "in a forgotten land", "in a hard to reach spot", "in a late-game area" }, grandPalaceBossDependencies.ToArray(), 0.1));
            if (goal == OpenWorldGoalProcessor.GOAL_MANABEAST)
            {
                if (fastManaFort)
                {
                    allLocations.Add(new PrizeLocation("buffy (new item)", 0x422, 0, new string[] { }, new string[] { "in the Mana Fortress", "in a late-game area" }, new string[] { "whip", DEPENDENCY_CUTTING_WEAPON }, 0.2));
                    allLocations.Add(new PrizeLocation("dread slime (new item)", 0x425, 0, new string[] { }, new string[] { "in the Mana Fortress", "in a late-game area" }, new string[] { "whip", DEPENDENCY_CUTTING_WEAPON }, 0.2));
                }
                else
                {
                    // vanilla long mode
                    allLocations.Add(new PrizeLocation("buffy (new item)", 0x422, 0, new string[] { }, new string[] { "in the Mana Fortress", "in a late-game area" }, manafortDependencies.ToArray(), 0.1));
                    allLocations.Add(new PrizeLocation("dread slime (new item)", 0x425, 0, new string[] { }, new string[] { "in the Mana Fortress", "in a late-game area" }, manafortDependencies.ToArray(), 0.1));
                }
            }

            List<string> gnomeHints = new string[] { "somewhere underground", "at a Mana seed pedestal", "where a Mana seed should be", "in an early-game area" }.ToList();
            if (anySpellTriggers)
            {
                string ep = _earthPalaceElement == "undine" ? "an " + _earthPalaceElement : "a " + _earthPalaceElement;
                gnomeHints.Add("behind " + ep + " orb");
            }

            if (restrictiveLogic)
            {
                allLocations.Add(new PrizeLocation("gnome item 1 (spells)", 0x22e, 0, new string[] { }, gnomeHints.ToArray(), new string[] { _earthPalaceElement + " spells", "whip", "earth seed" }, 0.5));
                allLocations.Add(new PrizeLocation("gnome item 2 (seed)", 0x22e, 1, new string[] { }, gnomeHints.ToArray(), new string[] { _earthPalaceElement + " spells", "whip", "earth seed" }, 0.5));
            }
            else
            {
                allLocations.Add(new PrizeLocation("gnome item 1 (spells)", 0x22e, 0, new string[] { }, gnomeHints.ToArray(), new string[] { _earthPalaceElement + " spells", "whip" }, 0.5));
                allLocations.Add(new PrizeLocation("gnome item 2 (seed)", 0x22e, 1, new string[] { }, gnomeHints.ToArray(), new string[] { _earthPalaceElement + " spells", "whip" }, 0.5));
            }

            List<string> lunaHints = new List<string> { "in a strange place", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area" };
            if (anySpellTriggers)
            {
                string lp = _lunaPalaceElement == "undine" ? "an " + _lunaPalaceElement : "a " + _lunaPalaceElement;
                lunaHints.Add("behind " + lp + " orb");
            }

            List<string> fp3Hints = new List<string> { "in a warm place", "in a hard to reach spot", "at a Mana seed pedestal", "where a Mana seed should be", "in a mid-game area" };
            if (anySpellTriggers)
            {
                string fp1 = _firePalaceElement1 == "undine" ? "an " + _firePalaceElement1 : "a " + _firePalaceElement1;
                string fp2 = _firePalaceElement2 == "undine" ? "an " + _firePalaceElement2 : "a " + _firePalaceElement2;
                string fp3 = _firePalaceElement3 == "undine" ? "an " + _firePalaceElement3 : "a " + _firePalaceElement3;
                fp3Hints.Add("behind " + fp1 + " orb, " + fp2 + " orb, and " + fp3 + " orb");
            }

            if (restrictiveLogic)
            {
                allLocations.Add(new PrizeLocation("fire seed", 0x589, 0, new string[] { }, fp3Hints.ToArray(), new string[] { "whip", _firePalaceElement1 + " spells", _firePalaceElement2 + " spells", _firePalaceElement3 + " spells", "fire seed" }, 0.2));
                allLocations.Add(new PrizeLocation("luna item 1 (spells)", 0x584, 0, new string[] { }, lunaHints.ToArray(), new string[] { _lunaPalaceElement + " spells", "moon seed" }, 0.5));
                allLocations.Add(new PrizeLocation("luna item 2 (seed)", 0x584, 1, new string[] { }, lunaHints.ToArray(), new string[] { _lunaPalaceElement + " spells", "moon seed" }, 0.5));
            }
            else
            {
                allLocations.Add(new PrizeLocation("fire seed", 0x589, 0, new string[] { }, fp3Hints.ToArray(), new string[] { "whip", _firePalaceElement1 + " spells", _firePalaceElement2 + " spells", _firePalaceElement3 + " spells" }, 0.2));
                allLocations.Add(new PrizeLocation("luna item 1 (spells)", 0x584, 0, new string[] { }, lunaHints.ToArray(), new string[] { _lunaPalaceElement + " spells" }, 0.5));
                allLocations.Add(new PrizeLocation("luna item 2 (seed)", 0x584, 1, new string[] { }, lunaHints.ToArray(), new string[] { _lunaPalaceElement + " spells" }, 0.5));
            }

            allLocations.Add(new PrizeLocation("kakkara (moogle belt)", 0x2b2, 0, new string[] { }, new string[] { "in a warm place", "in a town", "at an oasis", "in a mid-game area" }, new string[] { "sea hare tail" }, 0.5));

            List<string> luminaRequirements = new List<string> { "gold tower key" };
            if (restrictiveLogic)
            {
                luminaRequirements.Add("light seed");
            }
            allLocations.Add(new PrizeLocation("lumina spells", 0x587, 0, new string[] { }, new string[] { "in a tower", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area", "on an island" }, luminaRequirements.ToArray(), 0.5));
            allLocations.Add(new PrizeLocation("lumina seed", 0x587, 1, new string[] { }, new string[] { "in a tower", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area", "on an island" }, luminaRequirements.ToArray(), 0.5));

            List<string> fp1Hints = new List<string> { "in a warm place", "in a chest", "in a mid-game area" };
            List<string> fp2Hints = new List<string> { "in a warm place", "in a chest", "in a mid-game area" };
            if (anySpellTriggers)
            {
                string fp1 = _firePalaceElement1 == "undine" ? "an " + _firePalaceElement1 : "a " + _firePalaceElement1;
                string fp2 = _firePalaceElement2 == "undine" ? "an " + _firePalaceElement2 : "a " + _firePalaceElement2;
                fp1Hints.Add("behind " + fp1 + " orb");
                fp2Hints.Add("behind " + fp1 + " orb and " + fp2 + " orb");
            }

            allLocations.Add(new PrizeLocation("chest next to whip chest", 0x688, 0, new string[] { }, new string[] { "at the witch's castle", "in a chest", "in an early-game area", "in a castle" }, new string[] { DEPENDENCY_ELINEE_ENTRY }, 0.6));
            // orb chests
            allLocations.Add(new PrizeLocation("shade palace glove orb chest", MAPNUM_SHADEPALACE_INTERIOR_E, 2, 0x672, 0, new string[] { }, new string[] { "in the mountains", "in a chest", "in a late-game area" }, new string[] { "axe" }, 0.7));
            allLocations.Add(new PrizeLocation("sunken continent sword orb chest", MAPNUM_GRANDPALACE_INTERIOR_D, 12, 0x675, 0, new string[] { }, new string[] { "in a forgotten land", "in a chest", "in a late-game area" }, new string[] { "whip", }, 0.3));
            allLocations.Add(new PrizeLocation("lumina tower axe orb chest", MAPNUM_LUMINATOWER_2F, 0, 0x677, 0, new string[] { }, new string[] { "in a tower", "in a chest", "in a late-game area", "on an island" }, new string[] { "gold tower key" }, 0.5));
            allLocations.Add(new PrizeLocation("fire palace axe orb chest", MAPNUM_FIREPALACE_F, 0, 0x678, 0, new string[] { }, fp2Hints.ToArray(), new string[] { _firePalaceElement1 + " spells", _firePalaceElement2 + " spells", }, 0.3));
            allLocations.Add(new PrizeLocation("lumina tower spear orb chest", MAPNUM_LUMINATOWER_1F, 0, 0x67b, 0, new string[] { }, new string[] { "in a tower", "in a chest", "in a late-game area", "on an island" }, new string[] { "gold tower key" }, 0.5));
            allLocations.Add(new PrizeLocation("sunken continent boomerang orb chest", MAPNUM_UNDERSEA_AXE_ROOM_A, 1, 0x691, 0, new string[] { }, new string[] { "in a forgotten land", "in a chest", "in a late-game area", "under the sea" }, new string[] { "axe" }, 0.6));
            allLocations.Add(new PrizeLocation("fire palace chest 1", MAPNUM_FIREPALACE_C, 0, 0x68a, 0, new string[] { }, fp1Hints.ToArray(), new string[] { _firePalaceElement1 + " spells", }, 0.4)); // past first orb
            allLocations.Add(new PrizeLocation("fire palace chest 2", MAPNUM_FIREPALACE_G, 1, 0x68b, 0, new string[] { }, fp2Hints.ToArray(), new string[] { _firePalaceElement1 + " spells", _firePalaceElement2 + " spells", }, 0.3)); // past first two orbs

            allLocations.Add(new PrizeLocation("santa (new item)", 0x66d, 0, new string[] { }, new string[] { "in a cold place", "under the Christmas tree", "in a mid-game area", "in a castle" }, new string[] { "whip" }, 0.5));

            if (restrictiveLogic)
            {
                allLocations.Add(new PrizeLocation("shade spells", 0x586, 0, new string[] { }, new string[] { "in the mountains", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area" }, new string[] { "axe", "whip", "dark seed" }, 0.4));
                allLocations.Add(new PrizeLocation("shade seed", 0x586, 1, new string[] { }, new string[] { "in the mountains", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area" }, new string[] { "axe", "whip", "dark seed" }, 0.4));
            }
            else
            {
                allLocations.Add(new PrizeLocation("shade spells", 0x586, 0, new string[] { }, new string[] { "in the mountains", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area" }, new string[] { "axe", "whip" }, 0.4));
                allLocations.Add(new PrizeLocation("shade seed", 0x586, 1, new string[] { }, new string[] { "in the mountains", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area" }, new string[] { "axe", "whip" }, 0.4));
            }
            allLocations.Add(new PrizeLocation("thunder gigas (new item)", 0x5f5, 0, new string[] { }, new string[] { "in a volcano", "beyond the Pure Lands bushes", "in a late-game area" }, new string[] { DEPENDENCY_CUTTING_WEAPON }, 0.4));
            allLocations.Add(new PrizeLocation("red dragon (new item)", 0x5f3, 0, new string[] { }, new string[] { "in a volcano", "beyond the Pure Lands bushes", "in a late-game area" }, new string[] { DEPENDENCY_CUTTING_WEAPON }, 0.4));
            allLocations.Add(new PrizeLocation("blue dragon (new item)", 0x5f7, 0, new string[] { }, new string[] { "in a volcano", "beyond the Pure Lands bushes", "in a late-game area" }, new string[] { DEPENDENCY_CUTTING_WEAPON }, 0.4));
            if (goal == OpenWorldGoalProcessor.GOAL_MANABEAST)
            {
                allLocations.Add(new PrizeLocation("mana tree (new item)", 0x5f8, 0, new string[] { }, new string[] { "in a volcano", "at the Mana Tree", "in a late-game area", "in a hard to reach spot", "beyond the Pure Lands bushes" }, new string[] { DEPENDENCY_CUTTING_WEAPON }, 0.3));
            }

            List<string> mfHints = new string[] { "in the Upper Land", "in a cave", "in a mid-game area" }.ToList();
            if (anySpellTriggers)
            {
                string mf = _matangoCaveElement == "undine" ? "an " + _matangoCaveElement : "a " + _matangoCaveElement;
                mfHints.Add("behind " + mf + " orb");
            }

            if (flammieDrumInLogic)
            {
                // need the axe to walk through the cave
                allLocations.Add(new PrizeLocation("matango flammie", 0x4e4, 0, new string[] { }, mfHints.ToArray(), new string[] { _matangoCaveElement + " spells", "whip", "axe" }, 0.4));
            }
            else
            {
                allLocations.Add(new PrizeLocation("matango flammie", 0x4e4, 0, new string[] { }, mfHints.ToArray(), new string[] { _matangoCaveElement + " spells", "whip" }, 0.4));
            }
            allLocations.Add(new PrizeLocation("jehk", 0x4e5, 0, new string[] { }, new string[] { "in the mountains", "in a cave", "in a late-game area" }, new string[] { "axe", "whip" }, 0.3));
            allLocations.Add(new PrizeLocation("hydra (new item)", 0x59b, 0, new string[] { }, new string[] { "in a forgotten land", "under the sea", "in a late-game area" }, new string[] { "axe" }, 0.5));
            allLocations.Add(new PrizeLocation("kettlekin (new item)", 0x4ca, 0, new string[] { }, new string[] { "in a forgotten land", "under the sea", "in a late-game area", "in a hard to reach spot" }, new string[] { "axe" }, 0.4));
            allLocations.Add(new PrizeLocation("shade palace chest", MAPNUM_SHADEPALACE_INTERIOR_B, 0, 0x68e, 0, new string[] { }, new string[] { "in the mountains", "in a chest", "in a late-game area" }, new string[] { "axe" }, 0.5));

            if (restrictiveLogic)
            {
                allLocations.Add(new PrizeLocation("luka item 1 (spear)", 0x132, 0, new string[] { }, new string[] { "in the Potos area", "at a Mana seed pedestal", "where a Mana seed should be", "in an early-game area" }, new string[] { "water seed" }, 0.8));
                allLocations.Add(new PrizeLocation("luka item 2 (undine seed)", 0x132, 1, new string[] { }, new string[] { "in the Potos area", "at a Mana seed pedestal", "where a Mana seed should be", "in an early-game area" }, new string[] { "water seed" }, 0.8));
                allLocations.Add(new PrizeLocation("sylphid item 1 (spells)", 0x4e2, 0, new string[] { }, new string[] { "in the Upper Land", "at a Mana seed pedestal", "where a Mana seed should be", "in a mid-game area" }, new string[] { "wind seed" }, 0.8));
                allLocations.Add(new PrizeLocation("sylphid item 2 (seed)", 0x4e2, 1, new string[] { }, new string[] { "in the Upper Land", "at a Mana seed pedestal", "where a Mana seed should be", "in a mid-game area" }, new string[] { "wind seed" }, 0.8));
            }
            else
            {
                allLocations.Add(new PrizeLocation("luka item 1 (spear)", 0x132, 0, new string[] { }, new string[] { "in the Potos area", "at a Mana seed pedestal", "where a Mana seed should be", "in an early-game area" }, new string[] { }, 0.8));
                allLocations.Add(new PrizeLocation("luka item 2 (undine seed)", 0x132, 1, new string[] { }, new string[] { "in the Potos area", "at a Mana seed pedestal", "where a Mana seed should be", "in an early-game area" }, new string[] { }, 0.8));
                allLocations.Add(new PrizeLocation("sylphid item 1 (spells)", 0x4e2, 0, new string[] { }, new string[] { "in the Upper Land", "at a Mana seed pedestal", "where a Mana seed should be", "in a mid-game area" }, new string[] { }, 0.8));
                allLocations.Add(new PrizeLocation("sylphid item 2 (seed)", 0x4e2, 1, new string[] { }, new string[] { "in the Upper Land", "at a Mana seed pedestal", "where a Mana seed should be", "in a mid-game area" }, new string[] { }, 0.8));
            }

            // other chests
            allLocations.Add(new PrizeLocation("whip chest", 0x689, 0, new string[] { }, new string[] { "at the witch's castle", "in a chest", "in an early-game area", "in a castle" }, new string[] { DEPENDENCY_ELINEE_ENTRY }, 0.6));
            allLocations.Add(new PrizeLocation("moogle village glove orb chest", MAPNUM_UPPERLAND_MOOGLE_VILLAGE, 7, 0x670, 0, new string[] { }, new string[] { "in the Upper Land", "in a chest", "in a town", "in a mid-game area", "at Moogle Village" }, new string[] { }, 3.0));
            allLocations.Add(new PrizeLocation("ice castle glove orb chest", MAPNUM_ICECASTLE_INTERIOR_E, 0, 0x671, 0, new string[] { }, new string[] { "in a cold place", "in a chest", "in a mid-game area", "in a castle" }, new string[] { }, 1.0));
            allLocations.Add(new PrizeLocation("pandora sword orb chest", MAPNUM_PANDORA_TREASURE_ROOM, 0, 0x673, 0, new string[] { }, new string[] { "in the Pandora area", "in a chest", "in a town", "in a treasure room", "in an early-game area", "in a castle" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("northtown ruins sword orb chest", MAPNUM_NTR_INTERIOR_D, 24, 0x674, 0, new string[] { }, new string[] { "in the Empire", "in a chest", "in a mid-game area", "in Northtown", "at Northtown Ruins" }, new string[] { }, 1.5));
            allLocations.Add(new PrizeLocation("moogle village axe orb chest", MAPNUM_UPPERLAND_MOOGLE_VILLAGE, 8, 0x676, 0, new string[] { }, new string[] { "in the Upper Land", "in a chest", "in a town", "in a mid-game area", "at Moogle Village" }, new string[] { }, 3.0));
            allLocations.Add(new PrizeLocation("northtown castle axe orb chest", MAPNUM_NTC_INTERIOR_F, 0, 0x679, 0, new string[] { }, new string[] { "in the Empire", "in a chest", "in a mid-game area", "in Northtown", "in a castle", "at Northtown Castle" }, new string[] { }, 1.0));
            allLocations.Add(new PrizeLocation("northtown ruins spear orb chest", MAPNUM_NTR_ENTRANCE, 8, 0x67a, 0, new string[] { }, new string[] { "in the Empire", "in a chest", "in a mid-game area", "in Northtown", "at Northtown Ruins" }, new string[] { }, 1.5));
            allLocations.Add(new PrizeLocation("pandora spear orb chest", MAPNUM_PANDORA_TREASURE_ROOM, 1, 0x67c, 0, new string[] { }, new string[] { "in the Pandora area", "in a chest", "in a town", "in a treasure room", "in an early-game area", "in a castle" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("santa spear orb chest", MAPNUM_SANTAHOUSE_INTERIOR, 1, 0x67d, 0, new string[] { }, new string[] { "in a cold place", "in a chest", "in a mid-game area", "in a house" }, new string[] { }, 1.5));
            allLocations.Add(new PrizeLocation("kilroy whip orb chest", MAPNUM_KILROYSHIP_LOWER, 0, 0x68c, 0, new string[] { }, new string[] { "somewhere underground", "in a chest", "in a hole", "in an early-game area" }, new string[] { }, 3.0));
            allLocations.Add(new PrizeLocation("northtown castle whip orb chest", MAPNUM_NTC_INTERIOR_F, 1, 0x68f, 0, new string[] { }, new string[] { "in the Empire", "in a chest", "in a mid-game area", "in Northtown", "in a castle", "at Northtown Castle" }, new string[] { }, 1.0));
            allLocations.Add(new PrizeLocation("northtown ruins bow orb chest", MAPNUM_NTR_ENTRANCE, 7, 0x690, 0, new string[] { }, new string[] { "in the Empire", "in a chest", "in a mid-game area", "in Northtown", "at Northtown Ruins" }, new string[] { }, 1.0));
            allLocations.Add(new PrizeLocation("potos chest", MAPNUM_POTOS_INTERIOR_B, 6, 0x680, 0, new string[] { }, new string[] { "in the Potos area", "in a chest", "in a town", "in an early-game area", "in a house" }, new string[] { }, 3.0));
            allLocations.Add(new PrizeLocation("pandora chest 1", MAPNUM_PANDORA_TREASURE_ROOM, 2, 0x683, 0, new string[] { }, new string[] { "in the Pandora area", "in a chest", "in a town", "in a treasure room", "in an early-game area", "in a castle" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("pandora chest 2", MAPNUM_PANDORA_TREASURE_ROOM, 3, 0x684, 0, new string[] { }, new string[] { "in the Pandora area", "in a chest", "in a town", "in a treasure room", "in an early-game area", "in a castle" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("pandora chest 3", MAPNUM_PANDORA_TREASURE_ROOM, 4, 0x685, 0, new string[] { }, new string[] { "in the Pandora area", "in a chest", "in a town", "in a treasure room", "in an early-game area", "in a castle" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("pandora chest 4", MAPNUM_PANDORA_TREASURE_ROOM, 5, 0x686, 0, new string[] { }, new string[] { "in the Pandora area", "in a chest", "in a town", "in a treasure room", "in an early-game area", "in a castle" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("magic rope chest", MAPNUM_MAGICROPE, 0, 0x687, 0, new string[] { }, new string[] { "somewhere underground", "in a chest", "in a cave", "in an early-game area" }, new string[] { }, 1.5));
            allLocations.Add(new PrizeLocation("northtown castle chest", MAPNUM_NTC_INTERIOR_A, 2, 0x68d, 0, new string[] { }, new string[] { "in the Empire", "in a chest", "in a mid-game area", "in Northtown", "in a castle", "at Northtown Castle" }, new string[] { }, 0.8));
            if (flammieDrumInLogic)
            {
                // axe to walk through the cave
                allLocations.Add(new PrizeLocation("matango inn javelin orb chest", MAPNUM_MATANGO_INTERIOR_CHEST, 0, 0x692, 0, new string[] { }, new string[] { "in the Upper Land", "in a chest", "in a town", "in a mid-game area" }, new string[] { DEPENDENCY_MATANGO_ENTRY }, 2.0));
            }
            else
            {
                allLocations.Add(new PrizeLocation("matango inn javelin orb chest", MAPNUM_MATANGO_INTERIOR_CHEST, 0, 0x692, 0, new string[] { }, new string[] { "in the Upper Land", "in a chest", "in a town", "in a mid-game area" }, new string[] { }, 2.0));
            }
            allLocations.Add(new PrizeLocation("watts (axe)", 0x1b0, 0, new string[] { }, new string[] { "somewhere underground", "in a town", "in a cave", "in an early-game area" }, new string[] { }, 3.0));
            allLocations.Add(new PrizeLocation("undine item 1 (spells)", 0x581, 0, new string[] { }, new string[] { "in the Potos area", "behind a waterfall", "in a cave", "in an early-game area" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("undine item 2 (javelin)", 0x581, 1, new string[] { }, new string[] { "in the Potos area", "behind a waterfall", "in a cave", "in an early-game area" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("salamando", 0x38c, 0, new string[] { }, new string[] { "in a cold place", "in a town", "in a furnace", "in a mid-game area" }, new string[] { }, 0.8));
            if (restrictiveLogic)
            {
                allLocations.Add(new PrizeLocation("dryad spells", 0x4b6, 0, new string[] { }, new string[] { "in a forgotten land", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area" }, new string[] { "dryad seed" }, 0.8));
                allLocations.Add(new PrizeLocation("dryad seed", 0x4b6, 1, new string[] { }, new string[] { "in a forgotten land", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area" }, new string[] { "dryad seed" }, 0.8));
            }
            else
            {
                allLocations.Add(new PrizeLocation("dryad spells", 0x4b6, 0, new string[] { }, new string[] { "in a forgotten land", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area" }, new string[] { }, 0.8));
                allLocations.Add(new PrizeLocation("dryad seed", 0x4b6, 1, new string[] { }, new string[] { "in a forgotten land", "at a Mana seed pedestal", "where a Mana seed should be", "in a late-game area" }, new string[] { }, 0.8));
            }
            allLocations.Add(new PrizeLocation("mara (tower key)", 0x399, 0, new string[] { }, new string[] { "in the Empire", "in a town", "in a mid-game area", "in a house" }, new string[] { }, 3.0));
            allLocations.Add(new PrizeLocation("turtle island (sea hare tail)", 0x2f9, 0, new string[] { }, new string[] { "on an island", "in a town", "in a late-game area" }, new string[] { }, 3.0));
            allLocations.Add(new PrizeLocation("dwarf elder (midge mallet)", 0x366, 0, new string[] { }, new string[] { "somewhere underground", "in a town", "in a cave", "in an early-game area" }, new string[] { }, 3.0));
            allLocations.Add(new PrizeLocation("tropicallo (sprite)", 0x1b9, 0, new string[] { }, new string[] { "somewhere underground", "in a town", "in a cave", "in an early-game area" }, new string[] { }, 1.0));
            allLocations.Add(new PrizeLocation("tropicallo (bow)", 0x1b9, 1, new string[] { }, new string[] { "somewhere underground", "in a town", "in a cave", "in an early-game area" }, new string[] { }, 1.0));
            allLocations.Add(new PrizeLocation("girl", 0x188, 0, new string[] { }, new string[] { "in the Pandora area", "in a town", "in an early-game area", "in a castle" }, new string[] { }, 2.5));
            allLocations.Add(new PrizeLocation("starter weapon (main)", 0x103, 0, new string[] { }, new string[] { }, 1.0));
            if (anyCharsAdded)
            {
                allLocations.Add(new PrizeLocation("starter weapon (alt)", 0x103, 1, new string[] { }, new string[] { }, 1.0));
            }
            allLocations.Add(new PrizeLocation("lighthouse (new item)", 0xaa, 0, new string[] { }, new string[] { "on an island", "in a late-game area" }, new string[] { }, 3.0));
            allLocations.Add(new PrizeLocation("kilroy (new item)", 0x36a, 0, new string[] { }, new string[] { "somewhere underground", "in a hole", "in an early-game area" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("mantis ant (new item)", 0x112, 0, new string[] { }, new string[] { "in the Potos area", "in a town", "in a hole", "in an early-game area" }, new string[] { }, 0.8));
            // don't need cutting weapon to get here (boss 3)
            allLocations.Add(new PrizeLocation("axe beak (new item)", 0x5f1, 0, new string[] { }, new string[] { "in a volcano", "before the Pure Lands bushes", "in a cave", "in a late-game area" }, new string[] { }, 0.8));
            // don't need cutting weapon to get here (boss 2)
            allLocations.Add(new PrizeLocation("snow dragon (new item)", 0x5ef, 0, new string[] { }, new string[] { "in a volcano", "before the Pure Lands bushes", "in a late-game area" }, new string[] { }, 0.8));
            // don't need cutting weapon to get here (boss 1)
            allLocations.Add(new PrizeLocation("dragon worm (new item)", 0x5ed, 0, new string[] { }, new string[] { "in a volcano", "before the Pure Lands bushes", "in a late-game area" }, new string[] { }, 0.8)); 
            allLocations.Add(new PrizeLocation("doom wall", 0x553, 0, new string[] { }, new string[] { "in the Empire", "in a mid-game area", "in Northtown", "at Northtown Ruins" }, new string[] { }, 0.7));
            allLocations.Add(new PrizeLocation("vampire", 0x559, 0, new string[] { }, new string[] { "in the Empire", "in a mid-game area", "in Northtown", "at Northtown Ruins" }, new string[] { }, 0.6));
            allLocations.Add(new PrizeLocation("mech rider 2", 0x52f, 0, new string[] { }, new string[] { "in the Empire", "in a mid-game area", "in Northtown", "in a castle", "at Northtown Castle" }, new string[] { }, 0.6));
            allLocations.Add(new PrizeLocation("watermelon (new item)", 0x4b4, 0, new string[] { }, new string[] { "in a forgotten land", "in a late-game area" }, new string[] { }, 0.7));
            allLocations.Add(new PrizeLocation("hexas (new item)", 0x4a2, 0, new string[] { }, new string[] { "in a forgotten land", "in a late-game area" }, new string[] { }, 0.7));
            allLocations.Add(new PrizeLocation("wall face (new item)", 0x16e, 0, new string[] { }, new string[] { "in the Pandora area", "in an early-game area" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("metal mantis (new item)", 0x527, 0, new string[] { }, new string[] { "in the Empire", "in a mid-game area",  "in Northtown", "in a castle", "at Northtown Castle" }, new string[] { }, 0.8));
            allLocations.Add(new PrizeLocation("jema at tasnica", 0x2d1, 0, new string[] { }, new string[] { "at Tasnica", "in a town", "in a late-game area", "in a castle" }, new string[] { }, 1.0));
            allLocations.Add(new PrizeLocation("triple tonpole", 0x66a, 0, new string[] { }, new string[] { "in a cold place", "in a mid-game area", "in a castle", "in a hole" }, new string[] { }, 0.8));

            if (flammieDrumInLogic)
            {
                // we can repurpose 0x532 here which is the truffle NTC roof dialogue
                allLocations.Add(new PrizeLocation("sword pedestal", 0x532, 0, new string[] { }, new string[] { "in an early-game area", "in the rabite forest", "in the Potos area" }, new string[] { DEPENDENCY_CUTTING_WEAPON }, 0.8));
            }
            return allLocations;
        }

        public static void populateDependencies(RandoSettings settings, RandoContext context, List<string> grandPalaceBossDependencies, List<string> manafortDependencies)
        {
            // based on which characters, spells, and spell orbs are included, determine dependencies needed to complete the grand palace
            bool randomizeGrandPalace = settings.getBool(OpenWorldSettings.PROPERTYNAME_RANDOMIZE_GRANDPALACE_ELEMENTS);
            bool girlSpellsExist = context.workingData.getBool(OpenWorldClassSelection.GIRL_MAGIC_EXISTS);
            bool spriteSpellsExist = context.workingData.getBool(OpenWorldClassSelection.SPRITE_MAGIC_EXISTS);
            grandPalaceBossDependencies.Add("whip");
            manafortDependencies.Add(DEPENDENCY_CUTTING_WEAPON);
            manafortDependencies.Add("whip");
            if (randomizeGrandPalace)
            {
                // map 420-426 for grand palace orbs
                // note that this already accounts for plando, as processed earlier by ElementSwaps
                Dictionary<int, byte> crystalOrbColorMap = ElementSwaps.getCrystalOrbElementMap(context);
                Dictionary<byte, string> elementNames = new Dictionary<byte, string>();
                elementNames[0x81] = "gnome spells";
                elementNames[0x82] = "undine spells";
                elementNames[0x83] = "salamando spells";
                elementNames[0x84] = "lumina spells";
                elementNames[0x85] = "sylphid spells";
                elementNames[0x86] = "shade spells";
                elementNames[0x87] = "luna spells";
                elementNames[0x88] = "dryad spells";
                elementNames[0xFF] = "no";
                bool anyValid = false;
                if (girlSpellsExist || spriteSpellsExist)
                {
                    foreach(int mapNum in crystalOrbColorMap.Keys)
                    {
                        // grand palace orb maps
                        if (mapNum >= ElementSwaps.ORBMAP_GRANDPALACE_FIRST && mapNum <= ElementSwaps.ORBMAP_GRANDPALACE_LAST)
                        {
                            byte eleValue = crystalOrbColorMap[mapNum];
                            string item = elementNames[eleValue];
                            if (item != "no")
                            {
                                if (!grandPalaceBossDependencies.Contains(item))
                                {
                                    grandPalaceBossDependencies.Add(item);
                                    manafortDependencies.Add(item);
                                }
                                anyValid = true;
                            }
                        }
                    }
                }
                if (anyValid)
                {
                    if (girlSpellsExist && spriteSpellsExist)
                    {
                        grandPalaceBossDependencies.Add(DEPENDENCY_GIRL_SPELLS);
                        grandPalaceBossDependencies.Add(DEPENDENCY_SPRITE_SPELLS);
                        manafortDependencies.Add(DEPENDENCY_GIRL_SPELLS);
                        manafortDependencies.Add(DEPENDENCY_SPRITE_SPELLS);
                    }
                    else if (girlSpellsExist)
                    {
                        grandPalaceBossDependencies.Add(DEPENDENCY_GIRL_SPELLS);
                        manafortDependencies.Add(DEPENDENCY_GIRL_SPELLS);
                    }
                    else if (spriteSpellsExist)
                    {
                        grandPalaceBossDependencies.Add(DEPENDENCY_SPRITE_SPELLS);
                        manafortDependencies.Add(DEPENDENCY_SPRITE_SPELLS);
                    }
                }
            }
            else
            {
                if (girlSpellsExist && spriteSpellsExist)
                {
                    grandPalaceBossDependencies.AddRange(new string[] { "gnome spells", "undine spells", "salamando spells", "sylphid spells", "lumina spells", "shade spells", "luna spells", });
                    grandPalaceBossDependencies.Add("girlCaster");
                    grandPalaceBossDependencies.Add("spriteCaster");
                    manafortDependencies.AddRange(new string[] { "gnome spells", "undine spells", "salamando spells", "sylphid spells", "lumina spells", "shade spells", "luna spells", });
                    manafortDependencies.Add("girlCaster");
                    manafortDependencies.Add("spriteCaster");
                }
                else if (girlSpellsExist)
                {
                    grandPalaceBossDependencies.AddRange(new string[] { "salamando spells", "sylphid spells", "lumina spells", });
                    grandPalaceBossDependencies.Add("girlCaster");
                    manafortDependencies.AddRange(new string[] { "salamando spells", "sylphid spells", "lumina spells", });
                    manafortDependencies.Add("girlCaster");
                }
                else if (spriteSpellsExist)
                {
                    grandPalaceBossDependencies.AddRange(new string[] { "gnome spells", "undine spells", "salamando spells", "sylphid spells", "shade spells", "luna spells", });
                    grandPalaceBossDependencies.Add("spriteCaster");
                    manafortDependencies.AddRange(new string[] { "gnome spells", "undine spells", "salamando spells", "sylphid spells", "shade spells", "luna spells", });
                    manafortDependencies.Add("spriteCaster");
                }
            }

            Logging.log("Grand palace dependencies: " + DataUtil.ListToString(grandPalaceBossDependencies), "debug");
        }

        public static List<Location> getAllLocations()
        {
			var list = new List<Location>
			{
				new Location
				{ 
					Name = "Early Game",
					Children = new List<Location>
					{
						new Location
						{
							Name = "Potos",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Sword Pedestal",
									Type = (int)LocationType.CHEST,
									Requires = { new ProgressionLogic { Progression= "canCutStuff" } },
								},
								new Location
								{
									Name = "Potos Chest",
									Type = (int)LocationType.CHEST,
								},
								new Location
								{
									Name = "Mantis Ant",
									Type = (int)LocationType.BOSS,
								},
							}
						},
						new Location
						{
							Name = "Pandora",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Pandora Castle Girl",
									Type = (int)LocationType.CHECK,
								},
								new Location
								{
									Name = "Wall Face",
									Type = (int)LocationType.BOSS,
								},
								new Location
								{
									Name = "Pandora Castle Chest - 1",
									Type = (int)LocationType.CHEST,
								},
								new Location
								{
									Name = "Pandora Castle Chest - 2",
									Type = (int)LocationType.CHEST,
								},
								new Location
								{
									Name = "Pandora Castle Chest - 3",
									Type = (int)LocationType.CHEST,
								},
								new Location
								{
									Name = "Pandora Castle Chest - 4",
									Type = (int)LocationType.CHEST,
								},
								new Location
								{
									Name = "Pandora Castle Chest - 5",
									Type = (int)LocationType.CHEST,
								},
								new Location
								{
									Name = "Pandora Castle Chest - 6",
									Type = (int)LocationType.CHEST,
								},
							}
						},
						new Location
						{
							Name = "Water Palace",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Water Palace Mana Seed",
									Type = (int)LocationType.CHEST,
									Requires = { new ProgressionLogic { Progression= "hasWaterPalaceAccess" } },
								},
								new Location
								{
									Name = "Jabberwocky",
									Type = (int)LocationType.BOSS,
									Requires = { new ProgressionLogic { Progression= "hasWaterPalaceAccess" } },
								},
								new Location
								{
									Name = "Biting Lizard",
									Type = (int)LocationType.BOSS,
								},
								new Location
								{
									Name = "Pole Dart",
									Type = (int)LocationType.CHEST,
								},
								new Location
								{
									Name = "Undine Spells",
									Type = (int)LocationType.CHECK,
								},
							}
						},
						new Location
						{
							Name = "Gaia's Navel",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Gaia's Navel Cave Chest",
									Type = (int)LocationType.CHEST,
								},
								new Location
								{
									Name = "Watts",
									Type = (int)LocationType.CHECK,
								},
								new Location
								{
									Name = "Dwarf Elder",
									Type = (int)LocationType.CHECK,
								},
								new Location
								{
									Name = "Tropicallo",
									Type = (int)LocationType.BOSS,
								},
								new Location
								{
									Name = "Character Sprite",
									Type = (int)LocationType.CHECK,
								},
								new Location
								{
									Name = "Underground Ship Chest",
									Type = (int)LocationType.CHEST,
								},
								new Location
								{
									Name = "Kilroy",
									Type = (int)LocationType.BOSS,
								},
								new Location
								{
									Name = "Earth Temple Orb",
									Type = (int)LocationType.CHECK,
									Requires = { new ProgressionLogic { Progression= "canDestroyETOrb" } },
									Provides = { new ProgressionLogic { Progression= "et_orb" } },
								},
								new Location
								{
									Name = "Fire Gigas",
									Type = (int)LocationType.BOSS,
									Requires = {
										new ProgressionLogic { Progression= "hasWhip" },
										new ProgressionLogic { Progression= "canDestroyETOrb" },
										new ProgressionLogic { Progression= "hasEarthPalaceAccess" },
									},
								},
								new Location
								{
									Name = "Earth Palace Mana Seed",
									Type = (int)LocationType.CHECK,
									Requires = {
										new ProgressionLogic { Progression= "hasWhip" },
										new ProgressionLogic { Progression= "canDestroyETOrb" },
										new ProgressionLogic { Progression= "hasEarthPalaceAccess" },
									},
								},
							}
						},
						new Location
						{
							Name = "Elinee's Castle",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Spikey Tiger",
									Type = (int)LocationType.BOSS,
									Requires = {
										new ProgressionLogic { Progression= "hasAxe" },
										new ProgressionLogic { Progression= "hasWhip", LogicGroup = 1 },
										new ProgressionLogic { Progression= "canCutStuff", LogicGroup = 1 },
									},
								},
								new Location
								{
									Name = "Elinee's Castle Reward",
									Type = (int)LocationType.CHEST,
									Requires = {
										new ProgressionLogic { Progression= "hasAxe" },
										new ProgressionLogic { Progression= "hasWhip", LogicGroup = 1 },
										new ProgressionLogic { Progression= "canCutStuff", LogicGroup = 1 },
									},
								},
							}
						},
						new Location
						{
							Name = "Upper Land",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Moogle Village",
									Children = new List<Location>
									{
										new Location
										{
											Name = "Moogle Village Chest - 1",
											Type = (int)LocationType.CHEST
										},
										new Location
										{
											Name = "Moogle Village Chest - 2",
											Type = (int)LocationType.CHEST
										},
									}
								},
								new Location
								{
									Name = "Sprite Village",
									Children = new List<Location>
									{
										new Location
										{
											Name = "Spring Beak",
											Type = (int)LocationType.BOSS,
											Requires = { new ProgressionLogic { Progression= "hasWindPalaceAccess" } },
										},
										new Location
										{
											Name = "Wind Palace Reward",
											Type = (int)LocationType.CHEST,
											Requires = { new ProgressionLogic { Progression= "hasWindPalaceAccess" } },
										},
									}
								},
								new Location
								{
									Name = "Upper Land Forest",
									Children = new List<Location>
									{
										new Location
										{
											Name = "Upper Land Forest Orb",
											Type = (int)LocationType.CHECK,
											Requires = { new ProgressionLogic { Progression = "canDestroyUFOrb" } },
											Provides = { new ProgressionLogic { Progression = "uf_orb" } }
										},
									}
								},
								new Location
								{
									Name = "Matango",
									Requires =
									{
										new ProgressionLogic { Progression = "hasDrum", },
										new ProgressionLogic { Progression = "canDestroyUFOrb", LogicGroup = 1 },
										new ProgressionLogic { Progression = "hasAxe", LogicGroup = 1 }
									},
									Children = new List<Location>
									{
										new Location
										{
											Name = "Matango Palace Chest",
											Type = (int) LocationType.CHEST
										},
										new Location
										{
											Name = "Matango Cave Orb",
											Type = (int) LocationType.CHECK,
											Requires =
											{
												new ProgressionLogic { Progression = "hasWhip" },
												new ProgressionLogic { Progression = "canDestroyMCOrb" },
											},
											Provides = { new ProgressionLogic { Progression = "mc_orb" } }
										},
										new Location
										{
											Name = "Great Viper",
											Type = (int) LocationType.BOSS,
											Requires =
											{
												new ProgressionLogic { Progression = "hasWhip" },
												new ProgressionLogic { Progression = "canDestroyMCOrb" },
											},
										},
									}
								}
							}
						}
					},
				},
				new Location
				{
					Name = "Mid Game",
					Requires =
					{
						new ProgressionLogic { Progression = "hasDrum" },
						new ProgressionLogic { Progression = "canDestroyUFOrb" },
					},
					Children = new List<Location>
					{
						new Location
						{
							Name = "Kakkara Desert",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Kakkara NPC",
									Type = (int)LocationType.CHECK,
									Requires = { new ProgressionLogic { Progression = "hasTail" } }
								},
								new Location
								{
									Name = "Fire Palace",
									Children = new List<Location>
									{
										new Location
										{
											Name = "Fire Palace Orb 1",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "fp_orb1" } },
											Requires = { new ProgressionLogic { Progression = "canDestroyFPOrb1" } },
										},
										new Location
										{
											Name = "Fire Palace Orb 2",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "fp_orb2" } },
											Requires =
											{
												new ProgressionLogic { Progression = "canDestroyFPOrb1" },
												new ProgressionLogic { Progression = "canDestroyFPOrb2" },
											},
										},
										new Location
										{
											Name = "Fire Palace Orb 3",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "fp_orb3" } },
											Requires =
											{
												new ProgressionLogic { Progression = "canDestroyFPOrb1" },
												new ProgressionLogic { Progression = "canDestroyFPOrb2" },
												new ProgressionLogic { Progression = "canDestroyFPOrb3" },
												new ProgressionLogic { Progression = "hasWhip" },
											},
										},
										new Location
										{
											Name = "Fire Palace Near Entrance Chest",
											Type = (int)LocationType.CHEST,
											Requires = { new ProgressionLogic { Progression = "canDestroyFPOrb1" } },
										},
										new Location
										{
											Name = "Fire Palace Near Orb 2 Chest",
											Type = (int)LocationType.CHEST,
											Requires =
											{
												new ProgressionLogic { Progression = "canDestroyFPOrb1" },
												new ProgressionLogic { Progression = "canDestroyFPOrb2" },
											},
										},
										new Location
										{
											Name = "Fire Palace After Little Maze Chest",
											Type = (int)LocationType.CHEST,
											Requires =
											{
												new ProgressionLogic { Progression = "canDestroyFPOrb1" },
												new ProgressionLogic { Progression = "canDestroyFPOrb2" },
											},
										},
										new Location
										{
											Name = "Minotaur",
											Type = (int)LocationType.BOSS,
											Requires =
											{
												new ProgressionLogic { Progression = "canDestroyFPOrb1" },
												new ProgressionLogic { Progression = "canDestroyFPOrb2" },
												new ProgressionLogic { Progression = "canDestroyFPOrb3" },
												new ProgressionLogic { Progression = "hasFirePalaceAccess" },
												new ProgressionLogic { Progression = "hasWhip" },
											},
										},
									}
								},
								new Location
								{
									Name = "Moon Palace",
									Requires = { new ProgressionLogic { Progression = "hasDrum" } },
									Children = new List<Location>
									{
										new Location
										{
											Name = "Moon Palace Orb",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "mp_orb" } },
											Requires = { new ProgressionLogic { Progression = "canDestroyMPOrb" } }
										},
										new Location
										{
											Name = "Moon Palace Seed", // Is this only one thing??? Poptracker says item_count 2??
											Type = (int)LocationType.CHECK,
											Requires =
											{
												new ProgressionLogic { Progression = "canDestroyMPOrb" },
												new ProgressionLogic { Progression = "hasMoonPalaceAccess" },
											}
										},
									}
								},
							}
						},
						new Location
						{
							Name = "Vandole Empire",
							Children = new List<Location>
							{
								new Location
								{
									Name = "South Town Mara",
									Type = (int)LocationType.CHECK
								},
								new Location
								{
									Name = "North Town",
									Children = new List<Location>
									{
										new Location
										{
											Name = "North Town Castle",
											Children = new List<Location>
											{
												new Location
												{
													Name = "Castle Double Chest - 1",
													Type = (int)LocationType.CHEST,
												},
												new Location
												{
													Name = "Castle Double Chest - 2",
													Type = (int)LocationType.CHEST,
												},
												new Location
												{
													Name = "Metal Mantis",
													Type = (int)LocationType.BOSS,
												},
												new Location
												{
													Name = "Castle Chest",
													Type = (int)LocationType.CHEST,
												},
												new Location
												{
													Name = "Mech Rider II",
													Type = (int)LocationType.BOSS,
												},
											}
										},
										new Location
										{
											Name = "North Town Ruins",
											Children = new List<Location>
											{
												new Location
												{
													Name = "North Town Ruins Left Chest",
													Type = (int)LocationType.CHEST
												},
												new Location
												{
													Name = "North Town Ruins Right Chest",
													Type = (int)LocationType.CHEST
												},
												new Location
												{
													Name = "North Town Ruins Inner Chest",
													Type = (int)LocationType.CHEST
												},
												new Location
												{
													Name = "Doom's Wall",
													Type = (int)LocationType.BOSS
												},
												new Location
												{
													Name = "Vampire",
													Type = (int)LocationType.BOSS
												},
											}
										}
									}
								},
								new Location
								{
									Name = "Gold City",
									Requires = { new ProgressionLogic { Progression = "hasDrum" } },
									Children = new List<Location>
									{
										new Location
										{
											Name = "Gold Tower",
											Requires = { new ProgressionLogic { Progression = "hasKey" } },
											Children = new List<Location>
											{
												new Location
												{
													Name = "Gold Tower Chest 1",
													Type = (int)LocationType.CHEST
												},
												new Location
												{
													Name = "Gold Tower Chest 2",
													Type = (int)LocationType.CHEST
												},
												new Location
												{
													Name = "Gold Tower Reward",
													Type = (int)LocationType.CHECK,
													Requires = { new ProgressionLogic { Progression = "hasLightPalaceAccess" } },
												},
												new Location
												{
													Name = "Gorgon Bull",
													Type = (int)LocationType.BOSS,
													Requires = { new ProgressionLogic { Progression = "hasLightPalaceAccess" } }
												},
											}
										}
									}
								}
							}
						},
						new Location
						{
							Name = "Ice Country",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Santas House Chest",
									Type = (int)LocationType.CHEST
								},
								new Location
								{
									Name = "Salamando",
									Type = (int)LocationType.BOSS
								},
								new Location
								{
									Name = "Ice Castle Chest",
									Type = (int)LocationType.CHEST
								},
								new Location
								{
									Name = "Biting Lizard (x3)",
									Type = (int)LocationType.BOSS
								},
								new Location
								{
									Name = "Frost Gigas",
									Type = (int)LocationType.BOSS
								},
							}
						}
					}
				},
				new Location
				{
					Name = "Late Game",
					Requires = { new ProgressionLogic { Progression = "hasDrum" } },
					Children = new List<Location>
					{
						new Location
						{
							Name = "Lighthouse NPC",
							Type = (int)LocationType.CHECK
						},
						new Location
						{
							Name = "Tasnica Miniboss",
							Type = (int)LocationType.BOSS
						},
						new Location
						{
							Name = "Turtle Island NPC",
							Type = (int)LocationType.CHECK
						},
						new Location
						{
							Name = "Lofty Mountains",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Palace of Darkness",
									Requires = { new ProgressionLogic { Progression = "hasAxe" } },
									Children = new List<Location>
									{
										new Location
										{
											Name = "Palace of Darkness Walkway Chest",
											Type = (int)LocationType.CHEST
										},
										new Location
										{
											Name = "Palace of Darkness Walkway Chest",
											Type = (int)LocationType.CHEST
										},
										new Location
										{
											Name = "Palace of Darkness Reward",
											Type = (int)LocationType.CHECK,
											Requires =
											{
												new ProgressionLogic { Progression = "hasWhip" },
												new ProgressionLogic { Progression = "hasDarkPalaceAccess" },
											},
										},
										new Location
										{
											Name = "Lime Slime",
											Type = (int)LocationType.BOSS,
											Requires =
											{
												new ProgressionLogic { Progression = "hasWhip" },
												new ProgressionLogic { Progression = "hasDarkPalaceAccess" },
											},
										},
									}
								},
								new Location
								{
									Name = "Doppelgangers",
									Type = (int)LocationType.BOSS,
									Requires =
									{
										new ProgressionLogic { Progression = "hasAxe" },
										new ProgressionLogic { Progression = "hasWhip" },
									},
								},
							}
						},
						new Location
						{
							Name = "Lost Continent",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Tree Palace",
									Children = new List<Location>
									{
										new Location
										{
											Name = "Tree Palace Reward",
											Type = (int)LocationType.CHECK,
											Requires = { new ProgressionLogic { Progression = "hasTreePalaceAccess" } },
										},
										new Location
										{
											Name = "Snap Dragon",
											Type = (int)LocationType.BOSS,
											Requires = { new ProgressionLogic { Progression = "hasTreePalaceAccess" } },
										},
										new Location
										{
											Name = "Hexas",
											Type = (int)LocationType.BOSS,
										},
										new Location
										{
											Name = "Aegagropilon",
											Type = (int)LocationType.BOSS,
										},
										new Location
										{
											Name = "Grand Palace Orb 1",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "gp_orb1" } },
											Requires = { new ProgressionLogic { Progression = "canDestroyGPOrb1" } },
										},
										new Location
										{
											Name = "Grand Palace Orb 2",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "gp_orb2" } },
											Requires = { new ProgressionLogic { Progression = "canDestroyGPOrb2" } },
										},
										new Location
										{
											Name = "Grand Palace Orb 3",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "gp_orb3" } },
											Requires =
											{
												new ProgressionLogic { Progression = "hasWhip" },
												new ProgressionLogic { Progression = "canDestroyGPOrb1" },
												new ProgressionLogic { Progression = "canDestroyGPOrb3" },
											},
										},
										new Location
										{
											Name = "Grand Palace Orb 4",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "gp_orb4" } },
											Requires =
											{
												new ProgressionLogic { Progression = "hasWhip" },
												new ProgressionLogic { Progression = "canDestroyGPOrb2" },
												new ProgressionLogic { Progression = "canDestroyGPOrb4" },
											},
										},
										new Location
										{
											Name = "Grand Palace Orb 5",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "gp_orb5" } },
											Requires =
											{
												new ProgressionLogic { Progression = "hasWhip" },
												new ProgressionLogic { Progression = "canDestroyGPOrb1" },
												new ProgressionLogic { Progression = "canDestroyGPOrb3" },
												new ProgressionLogic { Progression = "canDestroyGPOrb5" },
											},
										},
										new Location
										{
											Name = "Grand Palace Orb 6",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "gp_orb6" } },
											Requires =
											{
												new ProgressionLogic { Progression = "hasWhip" },
												new ProgressionLogic { Progression = "canDestroyGPOrb2" },
												new ProgressionLogic { Progression = "canDestroyGPOrb4" },
												new ProgressionLogic { Progression = "canDestroyGPOrb6" },
											},
										},
										new Location
										{
											Name = "Grand Palace Orb 7",
											Type = (int)LocationType.CHECK,
											Provides = { new ProgressionLogic { Progression = "gp_orb7" } },
											Requires =
											{
												new ProgressionLogic { Progression = "hasWhip" },
												new ProgressionLogic { Progression = "canDestroyGPOrb1" },
												new ProgressionLogic { Progression = "canDestroyGPOrb2" },
												new ProgressionLogic { Progression = "canDestroyGPOrb3" },
												new ProgressionLogic { Progression = "canDestroyGPOrb4" },
												new ProgressionLogic { Progression = "canDestroyGPOrb5" },
												new ProgressionLogic { Progression = "canDestroyGPOrb6" },
												new ProgressionLogic { Progression = "canDestroyGPOrb7" },
											},
										},
										new Location
										{
											Name = "Grand Palace Chest",
											Type = (int)LocationType.CHECK,
											Requires = { new ProgressionLogic { Progression = "hasWhip" } },
										},
										new Location
										{
											Name = "Mech Rider III",
											Type = (int)LocationType.BOSS,
											Requires = { new ProgressionLogic { Progression = "grandPalaceBoss" } },
										},
									}
								},
								new Location
								{
									Name = "Undersea Palace",
									Requires = { new ProgressionLogic { Progression = "hasAxe" } },
									Children = new List<Location>
									{
										new Location
										{
											Name = "Undersea Palace Chest",
											Type = (int)LocationType.CHEST,
										},
										new Location
										{
											Name = "Hydra",
											Type = (int)LocationType.BOSS,
										},
										new Location
										{
											Name = "Kettle Kin",
											Type = (int)LocationType.BOSS,
										},
									}
								},
							}
						},
						new Location
						{
							Name = "Pure Land",
							Children = new List<Location>
							{
								new Location
								{
									Name = "Dragon Worm",
									Type = (int)LocationType.BOSS,
								},
								new Location
								{
									Name = "Snow Dragon",
									Type = (int)LocationType.BOSS,
								},
								new Location
								{
									Name = "Axe Beak",
									Type = (int)LocationType.BOSS,
								},
								new Location
								{
									Name = "Red Dragon",
									Type = (int)LocationType.BOSS,
									Requires = { new ProgressionLogic { Progression = "canCutStuff" } },
								},
								new Location
								{
									Name = "Thunder Gigas",
									Type = (int)LocationType.BOSS,
									Requires = { new ProgressionLogic { Progression = "canCutStuff" } },
								},
								new Location
								{
									Name = "Blue Dragon",
									Type = (int)LocationType.BOSS,
									Requires = { new ProgressionLogic { Progression = "canCutStuff" } },
								},
								new Location
								{
									Name = "Pure Land Mana Tree",
									Type = (int)LocationType.CHECK,
									Requires =
									{
										new ProgressionLogic { Progression = "isVanillaShort" },
										new ProgressionLogic { Progression = "canCutStuff" },
										new ProgressionLogic { Progression = "isVanillaLong", LogicGroup = 1 },
										new ProgressionLogic { Progression = "canCutStuff", LogicGroup = 1 },
									},
								},
							}
						},
						new Location
						{
							Name = "Mana Fortress",
							Requires = { new ProgressionLogic { Progression = "hasManaFortressAccess" } },
							Children = new List<Location>
							{
								new Location
								{
									Name = "Buffy",
									Type = (int)LocationType.BOSS
								},
								new Location
								{
									Name = "Dread Slime",
									Type = (int)LocationType.BOSS
								},
							}
						}
					}
				}
			};
            return list;
        }
    }
}
