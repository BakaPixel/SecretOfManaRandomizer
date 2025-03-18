using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SoMRandomizer.native;
using SoMRandomizer.processing.openworld.randomization;
using SoMRandomizer.processing.openworld;
using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using Newtonsoft.Json.Linq;

namespace SoMRandomizer.api;

public static class NativeAPI
{
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
		var locationArray = OpenWorldLocations.getAllLocations();
		Dictionary<string, object> dictOut = new Dictionary<string, object> { };
		dictOut.Add("list", new List<Dictionary<string, object>>());
		foreach (ISerializableObject value in locationArray)
		{
			(dictOut["list"] as List<Dictionary<string, object>>).Add(value.toDict());
		}

		return NativeHelpers.ObjectToIntPtr(dictOut);
	}
    

	[UnmanagedCallersOnly(EntryPoint = "get_settings")]
	public static int get_settings()
	{
		Console.WriteLine("TEST - blank");
		return 3;
	}

	[UnmanagedCallersOnly(EntryPoint = "somr_receive_item")]
	public static unsafe void RecItem(IntPtr input)
	{
		// Print the data to verify
		Item testInput = Marshal.PtrToStructure<Item>(input);
		Console.WriteLine($"Received name: {testInput.name}");
		Console.WriteLine($"Received internal_name: {testInput.internal_name}");
		Console.WriteLine($"Received id: {testInput.id}");
	}

	[UnmanagedCallersOnly(EntryPoint = "get_setting_locations")]
	public static unsafe IntPtr get_setting_locations(IntPtr input)
	{
		var marshaled = Marshal.PtrToStringAnsi(input);

		Dictionary<string, object> dict = JObject.Parse(marshaled).ToObject<Dictionary<string, object>>();
		Dictionary<string, string> entries = (dict["entries"] as JObject).ToObject<Dictionary<string, string>>();

		// create default settings and apply our overrides
		CommonSettings commonSettings = new CommonSettings();
		OpenWorldSettings openWorldSettings = new OpenWorldSettings(commonSettings);
		// set a few common options for the log that the UI normally sets
		commonSettings.set(CommonSettings.PROPERTYNAME_MODE, OpenWorldSettings.MODE_KEY);
		commonSettings.set(CommonSettings.PROPERTYNAME_ALL_ENTERED_OPTIONS, marshaled);
		commonSettings.set(CommonSettings.PROPERTYNAME_VERSION, RomGenerator.VERSION_NUMBER);

		openWorldSettings.processNewSettings(entries);
		RandoContext rc = new RandoContext();
		List<PrizeLocation> lpl = OpenWorldLocations.getForSelectedOptions(openWorldSettings, rc);
		var dictOut = NativeHelpers.dataToDict(lpl);

		return NativeHelpers.ObjectToIntPtr(dictOut);
	}

	[UnmanagedCallersOnly(EntryPoint = "get_setting_items")]
	public static unsafe IntPtr get_setting_items(IntPtr input)
	{
		var marshaled = Marshal.PtrToStringAnsi(input);

		Dictionary<string, object> dict = JObject.Parse(marshaled).ToObject<Dictionary<string, object>>();
		Dictionary<string, string> entries = (dict["entries"] as JObject).ToObject<Dictionary<string, string>>();
		string seed = dict["seed"] as string;

		// create default settings and apply our overrides
		CommonSettings commonSettings = new CommonSettings();
		OpenWorldSettings openWorldSettings = new OpenWorldSettings(commonSettings);

		// set a few common options for the log that the UI normally sets
		commonSettings.set(CommonSettings.PROPERTYNAME_MODE, OpenWorldSettings.MODE_KEY);
		commonSettings.set(CommonSettings.PROPERTYNAME_ALL_ENTERED_OPTIONS, marshaled);
		commonSettings.set(CommonSettings.PROPERTYNAME_VERSION, RomGenerator.VERSION_NUMBER);

		openWorldSettings.processNewSettings(entries);
		RandoContext context = new RandoContext();
		RomGenerator.initGenerate(seed, openWorldSettings, context);
		StartingWeaponRandomizer.setStartingWeapons(openWorldSettings, context);
		OpenWorldCharacterSelection.setStartingCharacter(seed, openWorldSettings, context);
		List<PrizeLocation> lpl = OpenWorldLocations.getForSelectedOptions(openWorldSettings, context);
		List<PrizeItem> lpi = OpenWorldPrizes.getForSelectedOptions(openWorldSettings, context, lpl);

		var dictOut = NativeHelpers.dataToDict(lpi);

		return NativeHelpers.ObjectToIntPtr(dictOut);
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
		// Dereference the pointer to the GenerateConfig struct
		var marshaled = Marshal.PtrToStringAnsi(input);

		Dictionary<string, object> dict = JObject.Parse(marshaled).ToObject<Dictionary<string, object>>();
		Dictionary<string, string> entries = (dict["entries"] as JObject).ToObject<Dictionary<string, string>>();

		// Print the dictionary contents
		foreach (var kvp in dict)
		{
			Console.WriteLine($"{kvp.Key}: {kvp.Value}");
		}

		// create default settings and apply our overrides
		CommonSettings commonSettings = new CommonSettings();
		OpenWorldSettings openWorldSettings = new OpenWorldSettings(commonSettings);
		// set a few common options for the log that the UI normally sets
		commonSettings.set(CommonSettings.PROPERTYNAME_MODE, OpenWorldSettings.MODE_KEY);
		commonSettings.set(CommonSettings.PROPERTYNAME_ALL_ENTERED_OPTIONS, marshaled);
		commonSettings.set(CommonSettings.PROPERTYNAME_VERSION, RomGenerator.VERSION_NUMBER);

		openWorldSettings.processNewSettings(entries);
		OpenWorldGenerator openWorldGenerator = new OpenWorldGenerator();
		Dictionary<string, RomGenerator> generatorsByRomType = new Dictionary<string, RomGenerator> { { OpenWorldSettings.MODE_KEY, openWorldGenerator } };
		Dictionary<string, RandoSettings> settingsByRomType = new Dictionary<string, RandoSettings> { { OpenWorldSettings.MODE_KEY, openWorldSettings } };
		// run rom generation
		// note there are no checks here for whether the dstRom exists - it will overwrite
		try
		{
			RomGenerator.initGeneration(dict["sourcePath"] as string, dict["destPath"] as string, dict["seed"] as string, generatorsByRomType, commonSettings, settingsByRomType);
			Console.WriteLine("done!");
		}
		catch (Exception e)
		{
			Console.WriteLine("Error: " + e.Message);
		}
		return 1;
	}
}