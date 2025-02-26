using System;
using System.Linq;
using System.Runtime.InteropServices;
using SoMRandomizer.native;
using SoMRandomizer.processing.openworld.randomization;

namespace SoMRandomizer.api;

public static class NativeAPI
{
    public static int allocationCount;
    
    [UnmanagedCallersOnly(EntryPoint = "get_items")]
    public static unsafe int GetItems(NativeItem** items)
    {
        var itemsArray = OpenWorldPrizes.getAllItems().Select(NativeHelpers.ItemToNativeItem).ToArray();
        *items = NativeHelpers.ArrayToHGlobal(itemsArray, Marshal.SizeOf<NativeItem>());
        return itemsArray.Length;
    } 
    
    [UnmanagedCallersOnly(EntryPoint = "free_items")]
    public static unsafe void FreeItems(NativeItem** array_ptr, int length)
    {
        NativeHelpers.FreeArray(array_ptr, length, NativeHelpers.FreeNativeItem);
    }

    [UnmanagedCallersOnly(EntryPoint = "get_locations")]
    public static unsafe int get_locations(NativeLocation** locations)
    {
        var locationArray = OpenWorldLocations.getAllLocations().Select(NativeHelpers.LocationToNativeLocation).ToArray();
        *locations = NativeHelpers.ArrayToHGlobal(locationArray, Marshal.SizeOf<NativeLocation>());
        return locationArray.Length;
    }
    
    [UnmanagedCallersOnly(EntryPoint = "free_locations")]
    public static unsafe void FreeLocations(NativeLocation** array_ptr, int length)
    {
        NativeHelpers.FreeArray(array_ptr, length, NativeHelpers.FreeNativeLocation);
    }

    [UnmanagedCallersOnly(EntryPoint = "get_settings")]
    public static int get_settings()
    {
        return 3;
    }

    [UnmanagedCallersOnly(EntryPoint = "generate_rom")]
    public static int generate_rom()
    {
        return 4;
    }
}