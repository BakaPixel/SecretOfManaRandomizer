import ctypes
import gc
import tracemalloc
import sys


# Define the Item structure based on the C# struct
class Item(ctypes.Structure):
    _fields_ = [
        ("name", ctypes.c_char_p),
        ("internal_name", ctypes.c_char_p),
        ("id", ctypes.c_long)
    ]
    def __str__(self):
        name = self.name.decode() if self.name else ""
        internal_name = self.internal_name.decode() if self.internal_name else ""
        id = self.id
        return f"{type(self).__name__}|name:{name}|internal_name:{internal_name}|id:{id}"

class Location(ctypes.Structure):
    _fields_ = [
        ("name", ctypes.c_char_p),
        ("internal_name", ctypes.c_char_p),
        ("id", ctypes.c_long)
    ]
    def __str__(self):
        name = self.name.decode() if self.name else ""
        internal_name = self.internal_name.decode() if self.internal_name else ""
        id = self.id
        return f"{type(self).__name__}|name:{name}|internal_name:{internal_name}|id:{id}"

# class NativeArray(ctypes.Structure):
#     _fields_ = [
#         ("size", ctypes.c_int32),
#         ("type_size", ctypes.c_int32),
#         ("items", ctypes.c_void_p)
#     ]


def get_safe_array(ptr, type, length):
    if length < 1:
        return []
    return ctypes.cast(ptr, ctypes.POINTER(type * length)).contents

class NativeArray:
    def get_safe_array(self):
        if self.size < 1:
            return []
        return ctypes.cast(self.items, ctypes.POINTER(self.arr_type * self.size)).contents
    @classmethod
    def get(cls, arr_type: type):
        return type(f"NativeArray_{arr_type.__name__}", (ctypes.Structure, ), {
            "_fields_": [
                ("size", ctypes.c_int32),
                ("type_size", ctypes.c_int32),
                ("items", ctypes.POINTER(arr_type))
            ],
            "arr_type": arr_type,
            "get_safe_array": get_safe_array
        })

def test_items():
    for i in range(1000000):
        array_ptr = ctypes.POINTER(Item)()
        array_ptr_ptr = ctypes.pointer(array_ptr)
        array_len = get_items(array_ptr_ptr)
        free_items(array_ptr_ptr, array_len)

def test_locations():
    for i in range(1000000):
        array_ptr = ctypes.POINTER(Location)()
        array_ptr_ptr = ctypes.pointer(array_ptr)
        array_len = get_locations(array_ptr_ptr)
        free_locations(array_ptr_ptr, array_len)

if __name__ == "__main__":
    is_windows = sys.platform.startswith('win')
    if is_windows:
        from ctypes import WinDLL as DLL, c_void_p

        ending = '.dll'
    else:
        from ctypes import CDLL as DLL
    
        ending = '.so'
    
    dll = DLL(rf"bin\Debug\net9.0\win-x64\publish\SoMRandomizer.api{ending}")
    
    print(dll)
    
    get_items = dll.get_items   
    get_items.argtypes = [ctypes.POINTER(ctypes.POINTER(Item))]
    get_items.restype = ctypes.c_int
    
    get_locations = dll.get_locations
    get_locations.argtypes = [ctypes.POINTER(ctypes.POINTER(Location))]
    get_locations.restype = ctypes.c_int
    
    get_settings = dll.get_settings
    get_settings.restype = ctypes.c_int
    
    generate_rom = dll.generate_rom
    generate_rom.restype = ctypes.c_int

    free_items = dll.free_items
    free_items.argtypes = [ctypes.POINTER(ctypes.POINTER(Item)), ctypes.c_int]
    free_items.restype = None
    
    free_locations = dll.free_locations
    free_locations.argtypes = [ctypes.POINTER(ctypes.POINTER(Location)), ctypes.c_int]
    free_locations.restype = None

    #test_items()
    #test_locations()

    # for i in range(1000000):
    #     print(i)

    items_ptr = ctypes.POINTER(Item)()
    item_cnt = get_items(ctypes.pointer(items_ptr))
    print("get_items:", item_cnt, items_ptr)
    items = get_safe_array(items_ptr, Item, item_cnt)
    for item in items:
        print(item)
    free_items(ctypes.pointer(items_ptr), item_cnt)
    
    locations_ptr = ctypes.POINTER(Location)()
    location_cnt = get_locations(ctypes.pointer(locations_ptr))
    print("get_locations:", location_cnt, locations_ptr)
    locations = get_safe_array(locations_ptr, Location, location_cnt)
    for location in locations:
        print(location)
    free_locations(ctypes.pointer(locations_ptr), location_cnt)
    
    print("get_settings:", get_settings())
    print("generate_rom:", generate_rom())