using System;
using System.Runtime.InteropServices;

namespace SoMRandomizer.api;

[StructLayout(LayoutKind.Sequential)]
public struct NativeArray
{
    public static NativeArray Empty => new()
    {
        size = 0,
        items = null
    };
        
    public int size;
    public int type_size;
    public unsafe byte* items;
}

[StructLayout(LayoutKind.Sequential)]
public struct NativeItem
{
    public unsafe byte* name;
    public unsafe byte* internal_name;
    public long id;
}

[StructLayout(LayoutKind.Sequential)]
public struct NativeLocation
{
    public unsafe byte* name;
    public unsafe byte* internal_name;
    public long id;
}