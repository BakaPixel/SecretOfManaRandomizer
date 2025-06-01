using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SoMRandomizer.native;
using SoMRandomizer.processing.openworld.randomization;
using SoMRandomizer.processing.openworld;
using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.IO;
using System.Text;

namespace SoMRandomizer.api;

public static class NativeAPI
{
	private static ArrayList arrSoMRHubs = new ArrayList();

	[UnmanagedCallersOnly(EntryPoint = "start_context")]
	public static unsafe int start_context(IntPtr input)
	{
		var genConfig = Marshal.PtrToStructure<GenerateConfig>(input);
		// create default settings and apply our overrides
		CommonSettings commonSettings = new CommonSettings();
		OpenWorldSettings openWorldSettings = new OpenWorldSettings(commonSettings);

		// set a few common options for the log that the UI normally sets
		commonSettings.set(CommonSettings.PROPERTYNAME_MODE, OpenWorldSettings.MODE_KEY);
		commonSettings.set(CommonSettings.PROPERTYNAME_ALL_ENTERED_OPTIONS, genConfig.entries);
		commonSettings.set(CommonSettings.PROPERTYNAME_VERSION, RomGenerator.VERSION_NUMBER);

		openWorldSettings.processNewSettings(genConfig.getEntries());
		RandoContext context = new RandoContext();

		var gen = new OpenWorldGenerator();

		gen.owFirstHacks();
		RomGenerator.preGenerate(genConfig.seed, openWorldSettings, context);
		gen.owPreApplyHacks(genConfig.seed, openWorldSettings, context);
		StartingWeaponRandomizer.setStartingWeapons(openWorldSettings, context);
		List<PrizeLocation> lpl = OpenWorldLocations.getForSelectedOptions(openWorldSettings, context);
		List<PrizeItem> lpi = OpenWorldPrizes.getForSelectedOptions(openWorldSettings, context, lpl);

		context.owAllLocations = lpl;
		context.owAllPrizes = lpi;
		context.genStart = "AP";
		context.owGenerator = gen;

		arrSoMRHubs.Add(new SoMRHub(genConfig.seed, openWorldSettings ,context, genConfig));

		return arrSoMRHubs.Count - 1;
	}

	[UnmanagedCallersOnly(EntryPoint = "get_items")]
	public static unsafe IntPtr get_items()
	{
		var itemsArray = OpenWorldPrizes.getAllItems();
		Dictionary<string, object> dictOut = new Dictionary<string, object> { };
		dictOut.Add("list", new List<Dictionary<string, object>>());
		foreach (ISerializableObject value in itemsArray)
		{
			(dictOut["list"] as List<Dictionary<string, object>>).Add(value.toDict());
		}
		return NativeHelpers.ObjectToIntPtr(dictOut);
	}

	[UnmanagedCallersOnly(EntryPoint = "get_locations")]
	public static unsafe IntPtr get_locations()
	{
		var itemsArray = OpenWorldLocations.getAllLocations();
		Dictionary<string, object> dictOut = new Dictionary<string, object> { };
		dictOut.Add("list", new List<Dictionary<string, object>>());
		foreach (ISerializableObject value in itemsArray)
		{
			(dictOut["list"] as List<Dictionary<string, object>>).Add(value.toDict());
		}
		return NativeHelpers.ObjectToIntPtr(dictOut);
	}

	[UnmanagedCallersOnly(EntryPoint = "somr_receive_item")]
	public static unsafe void somr_receive_item(IntPtr input)
	{
		// Print the data to verify
		Item testInput = Marshal.PtrToStructure<Item>(input);
		Console.WriteLine("Item Received");
		Console.WriteLine($"Received name: {testInput.name}");
		Console.WriteLine($"Received internal_name: {testInput.internal_name}");
		Console.WriteLine($"Received id: {testInput.id}");
	}

	[UnmanagedCallersOnly(EntryPoint = "get_setting_locations")]
	public static unsafe IntPtr get_setting_locations(int input)
	{
		if (input >= 0 && input < arrSoMRHubs.Count)
		{
			SoMRHub hub = (SoMRHub)arrSoMRHubs[input];

			var dictOut = NativeHelpers.dataToDict(hub.context.owAllLocations);

			return NativeHelpers.ObjectToIntPtr(dictOut);
		}
		throw new Exception($"Invalid Index: {input}");
	}

	[UnmanagedCallersOnly(EntryPoint = "get_setting_items")]
	public static unsafe IntPtr get_setting_items(int input)
	{
		if (input >= 0 && input < arrSoMRHubs.Count)
		{
			SoMRHub hub = (SoMRHub)arrSoMRHubs[input];

			var dictOut = NativeHelpers.dataToDict(hub.context.owAllPrizes);

			return NativeHelpers.ObjectToIntPtr(dictOut);
		}
		throw new Exception($"Invalid Index: {input}");
	}

	[UnmanagedCallersOnly(EntryPoint = "free_ptr_memory")]
	public static unsafe int free_ptr_memory(IntPtr ptr)
	{
		try
		{
			Marshal.FreeHGlobal(ptr);
			return 1;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error freeing memory: {ex.Message}");
			return 0;
		}
	}

	[UnmanagedCallersOnly(EntryPoint = "generate_rom")]
	public static unsafe int generate_rom(IntPtr input)
	{
		var gen = Marshal.PtrToStructure<GenerateAP>(input);

		if (gen.SoMRHubIndex >= 0 && gen.SoMRHubIndex < arrSoMRHubs.Count)
		{
			SoMRHub hub = (SoMRHub)arrSoMRHubs[gen.SoMRHubIndex];

			hub.context.owAllPrizes = gen.GetPrizeItems();
			hub.context.owAllLocations = gen.GetPrizeLocations();

			// run rom generation
			// note there are no checks here for whether the dstRom exists - it will overwrite
			try
			{
				var (origRom, outRom) = RomGenerator.initRomFiles(gen.sourcePath, gen.destPath);
				hub.context.owGenerator.owSecondHacks();
				hub.context.owGenerator.owGenerate(origRom, outRom, hub.seed, hub.settings, hub.context);
				File.WriteAllBytes(gen.destPath, outRom);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: " + e.Message);
			}
			arrSoMRHubs[gen.SoMRHubIndex] = null;
			return 1;
		}
		throw new Exception($"Invalid Config: {input}");
	}
}