using System;
using System.Runtime.InteropServices;
using System.Text;
using SoMRandomizer.native;

namespace SoMRandomizer.api
{
    public static class NativeHelpers
    {
        public static unsafe T* ArrayToHGlobal<T>(T[] array, int structSize) where T : unmanaged
        {
            var ptr = (T*)NativeMemory.Alloc((nuint)array.Length, (nuint)structSize);
            //Console.WriteLine($"allocated memory: 0x{(IntPtr)ptr:x} (Allocations: {++NativeAPI.allocationCount})");
            for (var i = 0; i < array.Length; i++)
            {
                Marshal.StructureToPtr(array[i], (IntPtr)ptr + structSize * i, false);
            }

            return ptr;
        }

        public static unsafe void FreeArray<T>(T** array_ptr, int length, Action<T> freeItemFunc) where T : unmanaged
        {
            if (array_ptr == null) return;
            var array = *array_ptr;
            if(array == null) return;
        
            for (var i = 0; i < length; i++)
            {
                var nativeItem = array[i];
                freeItemFunc(nativeItem);
            }
            NativeMemory.Free(array);
            //Console.WriteLine($"Freed memory: 0x{(IntPtr)items:x} (Allocations: {--allocationCount})");
        }
        
        public static unsafe NativeItem ItemToNativeItem(Item item)
        {
            return new NativeItem
            {
                name = StringToHGlobalUTF8(item.name),
                internal_name = StringToHGlobalUTF8(item.internal_name),
                id = item.id,
            };
        }

        public static unsafe void FreeNativeItem(NativeItem nativeItem)
        {
            if (nativeItem.name != null)
            {
                NativeMemory.Free(nativeItem.name);
                //Console.WriteLine($"Freed memory: 0x{(IntPtr)nativeItem.name:x} (Allocations: {--allocationCount})");
            }
            if (nativeItem.internal_name != null)
            {
                NativeMemory.Free(nativeItem.internal_name);
                //Console.WriteLine($"Freed memory: 0x{(IntPtr)nativeItem.internal_name:x} (Allocations: {--allocationCount})");
            }
        }
        
        public static unsafe void FreeNativeLocation(NativeLocation nativeItem)
        {
            if (nativeItem.name != null)
            {
                NativeMemory.Free(nativeItem.name);
                //Console.WriteLine($"Freed memory: 0x{(IntPtr)nativeItem.name:x} (Allocations: {--allocationCount})");
            }
            if (nativeItem.internal_name != null)
            {
                NativeMemory.Free(nativeItem.internal_name);
                //Console.WriteLine($"Freed memory: 0x{(IntPtr)nativeItem.internal_name:x} (Allocations: {--allocationCount})");
            }
        }
            
        public static unsafe NativeLocation LocationToNativeLocation(Location item)
        {
            return new NativeLocation
            {
                name = StringToHGlobalUTF8(item.name),
                internal_name = StringToHGlobalUTF8(item.internal_name),
                id = item.id,
            };
        }
        
        public static unsafe byte* StringToHGlobalUTF8(string s) => StringToHGlobalUTF8(s, out _);
        
        public static unsafe byte* StringToHGlobalUTF8(string s, out int length)
        {
            if (s == null)
            {
                length = 0;
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(s);
            var ptr = (byte*)NativeMemory.Alloc((nuint)bytes.Length + 1);
            //Console.WriteLine($"allocated memory: 0x{(IntPtr)ptr:x} (Allocations: {++NativeAPI.allocationCount})");
            fixed (byte* bytesPtr = bytes)
            {
                NativeMemory.Copy(bytesPtr, ptr, (nuint)bytes.Length);
            }
            NativeMemory.Fill((byte*)ptr + bytes.Length, 1, 0);
            length = bytes.Length;
            return ptr;
        }
    }
}