using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.openworld
{
	/// <summary>
	/// Hack to swap elements & orbs for vanilla rando, or randomize spell orbs for open world.
	/// </summary>
	/// 
	/// <remarks>Author: Mostly Moppleton with NovaPixel assist</remarks>
	public class ElementSwapsApply : RandoProcessor
	{
		public const string ORBELEMENT_PREFIX = "orbElement";
		public const string VANILLARANDO_ELEMENTLIST = "vanillaRandoElementList";
		public static List<string> elementNames = new string[] { "Gnome", "Undine", "Salamando", "Sylphid", "Lumina", "Shade", "Luna", "Dryad" }.ToList();

		// keys into the crystal orb colors map - all the vanilla maps that have orbs on them
		public const int ORBMAP_MATANGO = MAPNUM_MATANGOCAVE_GNOME_ORB; // 307
		public const int ORBMAP_EARTHPALACE = MAPNUM_EARTHPALACE_ORB; // 291
		public const int ORBMAP_FIREPALACE3 = MAPNUM_FIREPALACE_UNDINE_ORB; // 240
		public const int ORBMAP_FIREPALACE1 = MAPNUM_FIREPALACE_FIRST_ORB; // 348
		public const int ORBMAP_FIREPALACE2 = MAPNUM_FIREPALACE_G; // 345
		public const int ORBMAP_LUNAPALACE = MAPNUM_LUNAPALACE_SPACE; // 35
		public const int ORBMAP_UPPERLAND = MAPNUM_UPPERLAND_SOUTHEAST; // 41
		public const int ORBMAP_GRANDPALACE1 = MAPNUM_GRANDPALACE_GNOME_ORB; // 420
		public const int ORBMAP_GRANDPALACE2 = MAPNUM_GRANDPALACE_UNDINE_ORB; // 421
		public const int ORBMAP_GRANDPALACE3 = MAPNUM_GRANDPALACE_SYLPHID_ORB; // 422
		public const int ORBMAP_GRANDPALACE4 = MAPNUM_GRANDPALACE_SALAMANDO_ORB; // 423
		public const int ORBMAP_GRANDPALACE5 = MAPNUM_GRANDPALACE_LUMINA_ORB; // 424
		public const int ORBMAP_GRANDPALACE6 = MAPNUM_GRANDPALACE_SHADE_ORB; // 425
		public const int ORBMAP_GRANDPALACE7 = MAPNUM_GRANDPALACE_LUNA_ORB; // 426
		public const int ORBMAP_GRANDPALACE_FIRST = ORBMAP_GRANDPALACE1; // 420
		public const int ORBMAP_GRANDPALACE_LAST = ORBMAP_GRANDPALACE7; // 426
		protected override string getName()
		{
			return "Spell orb element randomizer - Open World apply";
		}
		protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
		{
			string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
			if (randoMode == OpenWorldSettings.MODE_KEY)
			{
				bool girlMagicExists = context.workingData.getBool(OpenWorldClassSelection.GIRL_MAGIC_EXISTS);
				bool spriteMagicExists = context.workingData.getBool(OpenWorldClassSelection.SPRITE_MAGIC_EXISTS);
				bool randomizeGrandPalace = settings.getBool(OpenWorldSettings.PROPERTYNAME_RANDOMIZE_GRANDPALACE_ELEMENTS);
				bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
				processOpenWorld(outRom, context.crystalOrbElementMap, context.replacementEvents, girlMagicExists, spriteMagicExists, randomizeGrandPalace, flammieDrumInLogic);
				foreach (int mapNum in context.crystalOrbElementMap.Keys)
				{
					context.workingData.setInt(ORBELEMENT_PREFIX + mapNum, context.crystalOrbElementMap[mapNum]);
				}
			}
			else
			{
				Logging.log("Unsupported mode for element randomizer applyer");
				return false;
			}
			return true;
		}
		// for open world, set the orb elements to whatever was randomized for them.
		// don't swap spell rewards to match like in vanilla rando; randomized prize locations & logic will determine a new path through them
		public void processOpenWorld(byte[] rom, Dictionary<int, byte> crystalOrbColorMap, Dictionary<int, List<byte>> replacementEvents, bool girlExists, bool spriteExists, bool randomizeGrandPalace, bool flammieDrumInLogic)
		{
			// note that lumina and sylphid seem like they've been swapped in vanilla and no one ever noticed?
			// 358 gnome = 81
			// 359 undine = 82
			// 35a sylphid = 84
			// 35b salamando = 83
			// 35c lumina = 85
			// 35d shade = 86
			// 35e luna = 87
			// 35f dryad = 88

			// x81->x350 = gnome
			// x82->x351 = undine
			// x83->x353 = salamando
			// x84->x354 = lumina
			// x85->x352 = sylphid
			// x86->x355 = shade
			// x87->x356 = luna
			// x88->x357 = dryad

			if (randomizeGrandPalace)
			{
				Dictionary<byte, byte> palSets = new Dictionary<byte, byte>();
				palSets[0x81] = 89;
				palSets[0x82] = 88;
				palSets[0x83] = 91;
				palSets[0x84] = 93;
				palSets[0x85] = 47;
				palSets[0x86] = 92;
				palSets[0x87] = 95;
				palSets[0x88] = 97;
				palSets[0xFF] = 0xFF;
				rom[0x8DD19] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE1]]);
				rom[0x8DD41] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE2]]);
				rom[0x8DD69] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE3]]);
				rom[0x8DD91] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE4]]);
				rom[0x8DDB9] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE5]]);
				rom[0x8DDE1] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE6]]);
				rom[0x8DE09] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE7]]);
			}

			// x81->x350 = gnome
			// x82->x351 = undine
			// x83->x353 = salamando
			// x84->x354 = lumina
			// x85->x352 = sylphid
			// x86->x355 = shade
			// x87->x356 = luna
			// x88->x357 = dryad
			int[] eventConversions = new int[] { 0, 1, 3, 4, 2, 5, 6, 7 };


			// event types 4E and 49
			// they check analyzer in there too; should be able to use that as a template
			int spellId = 0;
			int[] spellIds = new int[] {
				0, 1, 2, 3, 4, 5, // gnome
                6, 7, 8, 9, 10, 11, // undine
                18, 19, 20, 21, 22, 23, // sylphid
                12, 13, 14, 15, 16, 17, // salamando
                39, 40, 41, // lumina
                36, 37, 38, // shade
                24, 25, 26, 27, 28, 29, // luna
                30, 31, 32, 33, 34, 35, // dryad
            };
			for (int i = 0; i < 8; i++)
			{
				EventScript ev = new EventScript();
				replacementEvents[0x350 + i] = ev;
				// orb animation
				ev.Add(EventCommandEnum.CHARACTER_ANIM.Value);
				ev.Add(0x04);
				ev.Add(0x80);
				// wait for animation
				ev.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

				// set flag to 1 initially
				ev.SetFlag((byte)(0x98 + i), 1);
				// break out if acceptable spells
				int max = 6;
				if (i == 4 || i == 5)
				{
					// shade, lumina
					max = 3;
				}

				for (int j = 0; j < max; j++)
				{
					// allow selected (all) spells
					ev.Add(0x4E);
					ev.Add(0x04);
					ev.Add(0x81);
					ev.Add((byte)spellIds[spellId]);
					ev.Add(0x02);
					ev.Add(0x01);
					spellId++;
				}

				// set flag to 0, didn't find correct spell
				ev.SetFlag((byte)(0x98 + i), 0);
				ev.Return();
				ev.End();
			}

			// gnome orb; matango cave
			if (crystalOrbColorMap[ORBMAP_MATANGO] != 0xFF)
			{
				EventScript ev27e = new EventScript();
				replacementEvents[0x27e] = ev27e;
				ev27e.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_MATANGO] - 0x81]); // change to check element we want
				ev27e.Logic(EventFlags.MATANGO_PROGRESS_FLAG, 0x4, 0xF, EventScript.GetJumpCmd(0));
				ev27e.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_MATANGO] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x27c)); // change to any spell acceptable (1-3)
				ev27e.Jsr(0x799); // idk sound maybe
				ev27e.End();
			}

			// undine orb; gaia's navel
			if (crystalOrbColorMap[ORBMAP_EARTHPALACE] != 0xFF)
			{
				EventScript ev239 = new EventScript();
				replacementEvents[0x239] = ev239;
				ev239.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_EARTHPALACE] - 0x81]); // change to check element we want
				ev239.Logic(EventFlags.EARTHPALACE_FLAG, 0x2, 0xF, EventScript.GetJumpCmd(0));
				ev239.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_EARTHPALACE] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x38)); // change to any spell acceptable (1-3)
				ev239.End();
			}

			// undine orb; end of fire palace
			if (crystalOrbColorMap[ORBMAP_FIREPALACE3] != 0xFF)
			{
				EventScript ev6ce = new EventScript();
				replacementEvents[0x6ce] = ev6ce;
				ev6ce.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE3] - 0x81]); // change to check element we want
				ev6ce.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE3] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x6c7)); // change to any spell acceptable (1-3)
				ev6ce.End();
			}

			// fire palace entrance salamando orb
			if (crystalOrbColorMap[ORBMAP_FIREPALACE1] != 0xFF)
			{
				EventScript ev2c8 = new EventScript();
				replacementEvents[0x2c8] = ev2c8;
				ev2c8.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE1] - 0x81]); // change to check element we want
				ev2c8.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE1] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x2c9)); // change to any spell acceptable (1-3)
				ev2c8.End();
			}

			// salamando orb; middle of fire palace
			if (crystalOrbColorMap[ORBMAP_FIREPALACE2] != 0xFF)
			{
				EventScript ev6cb = new EventScript();
				replacementEvents[0x6cb] = ev6cb;
				ev6cb.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE2] - 0x81]); // change to check element we want
				ev6cb.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE2] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x6ca)); // change to any spell acceptable (1-3)
				ev6cb.End();
			}

			// lumina orb; luna palace
			if (crystalOrbColorMap[ORBMAP_LUNAPALACE] != 0xFF)
			{
				EventScript ev2c4 = new EventScript();
				replacementEvents[0x2c4] = ev2c4;
				ev2c4.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_LUNAPALACE] - 0x81]); // change to check element we want
				ev2c4.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_LUNAPALACE] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x2c5)); // change to any spell acceptable (1-3)
				ev2c4.End();
			}

			// 25a - upper land; sylphid in vanilla
			if (flammieDrumInLogic)
			{
				if (crystalOrbColorMap[ORBMAP_UPPERLAND] != 0xFF)
				{
					EventScript ev25a = new EventScript();
					replacementEvents[0x25a] = ev25a;
					ev25a.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_UPPERLAND] - 0x81]); // change to check element we want
					ev25a.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_UPPERLAND] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x25b)); // change to any spell acceptable (1-3)
					ev25a.End();
				}
			}

			// set the lost continent ones
			if (!spriteExists || !girlExists || randomizeGrandPalace)
			{
				for (int mapId = ORBMAP_GRANDPALACE_FIRST; mapId <= ORBMAP_GRANDPALACE_LAST; mapId++) // skip dryad map, which is actually 419
				{
					if (crystalOrbColorMap[mapId] != 0xFF)
					{
						EventScript ev = new EventScript();
						replacementEvents[0x570 + (mapId - ORBMAP_GRANDPALACE_FIRST)] = ev;
						ev.Jsr(0x350 + eventConversions[crystalOrbColorMap[mapId] - 0x81]); // change to check element we want
						ev.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[mapId] - 0x81]), 0x0, 0x0, EventScript.GetJumpCmd(0)); // skip if any spell of right element cast
						ev.IncrFlag((byte)(0xE8 + (mapId - ORBMAP_GRANDPALACE_FIRST))); // change to set element we want
						ev.Jump(0x578);
						ev.End();
					}
				}
			}

			// map header[1] & 7F = palette
			// lumina palette = 93, shade palette = 92
			else if (!spriteExists && girlExists)
			{
				// set all the palettes to lumina
				for (int mapId = ORBMAP_GRANDPALACE_FIRST; mapId <= ORBMAP_GRANDPALACE_LAST; mapId++) // skip dryad map, which is actually 419
				{
					int mapObjOffset = 0x80000 + rom[0x87000 + mapId * 2] + (rom[0x87000 + mapId * 2 + 1] << 8);
					bool msb = (rom[mapObjOffset + 1] & 0x80) > 0;
					rom[mapObjOffset + 1] = 93;
					if (msb)
					{
						rom[mapObjOffset + 1] |= 0x80;
					}
				}
			}

			else if (spriteExists && !girlExists)
			{
				// set the lumina palette to shade
				int mapId = MAPNUM_GRANDPALACE_LUMINA_ORB;
				int mapObjOffset = 0x80000 + rom[0x87000 + mapId * 2] + (rom[0x87000 + mapId * 2 + 1] << 8);
				bool msb = (rom[mapObjOffset + 1] & 0x80) > 0;
				rom[mapObjOffset + 1] = 92;
				if (msb)
				{
					rom[mapObjOffset + 1] |= 0x80;
				}
			}
		}

	}
}
