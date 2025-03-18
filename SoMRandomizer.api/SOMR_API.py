import ctypes
import json
import sys
from pathlib import Path
from typing import Any

# Define the Item structure based on the C# struct
class TestStruct(ctypes.Structure):
    _fields_ = [
        ("name", ctypes.c_char_p),
        ("internal_name", ctypes.c_char_p),
        ("id", ctypes.c_int),
    ]
    name: str
    internal_name: str
    id: int

    def __init__(self, name: str, internal_name: str, id: int):
        self.name = name.encode("utf-8") if name else b""
        self.internal_name = internal_name.encode("utf-8") if internal_name else b""
        self.id = id


class SOMR_API:
    def __init__(self, dll_path: str):
        """Load the DLL and initialize function bindings."""

        is_windows = sys.platform.startswith("win")
        if is_windows:
            from ctypes import WinDLL as DLL

            ending = ".dll"
        else:
            from ctypes import CDLL as DLL

            ending = ".so"
        script_dir = Path(__file__).parent
        self.dll = DLL(rf"{script_dir}\{dll_path}{ending}")

        # Needed for auto managing memory later in C#
        self.csharp_ptrs: list[int] = []

        # Needed for auto managing memory in Python
        self.py_ptrs = []

        # Define function signatures

        self._func_declare(
            name="get_items",
            restype=ctypes.c_void_p,
        )

        self._func_declare(
            name="get_locations",
            restype=ctypes.c_void_p,
        )

        self._func_declare(
            name="get_settings",
            restype=ctypes.c_int,
        )

        self._func_declare(
            name="generate_rom",
            argtypes=[ctypes.c_void_p],
            restype=ctypes.c_int,
        )

        self._func_declare(
            name="somr_receive_item",
            argtypes=[ctypes.POINTER(TestStruct)],
        )

        self._func_declare(
            name="get_setting_locations",
            argtypes=[ctypes.c_void_p],
            restype=ctypes.c_void_p,
        )

        self._func_declare(
            name="get_setting_items",
            argtypes=[ctypes.c_void_p],
            restype=ctypes.c_void_p,
        )

        self._func_declare(
            name="free_ptr_memory",
            argtypes=[ctypes.c_void_p],
            restype=ctypes.c_int,
        )

    def __del__(self):
        # Garbage Collection Helper to free memory from DLL Automatically as Object is cleaned up.
        clear_count = 0
        ptr_count = len(self.csharp_ptrs)
        if len(self.csharp_ptrs) > 0:
            for ptr in self.csharp_ptrs:
                if ptr:
                    clear_count += self._free_memory(ptr)
            if ptr_count == clear_count:
                print("Clear Mem Done")
            else:
                raise MemoryError(f"{ptr_count} pointers found, only cleared {clear_count}")

    def _func_declare(
        self, name: str, argtypes: list[ctypes.POINTER] | None = None, restype: ctypes._SimpleCData | None = None
    ) -> None:
        # Get the reference to the DLL function
        dll_ref = f"dll_{name}"

        # Assuming that the DLL has already been loaded and assigned to `self.dll` in the class.
        func = getattr(self.dll, name)

        # Set the argument and return types if they are provided
        if argtypes:
            func.argtypes = argtypes
        if restype:
            func.restype = restype

        # Dynamically add this function to the class
        setattr(self, dll_ref, func)

    def _get_entrypoints(self):
        return [name for name in self.dll.__dict__ if not name.startswith("_")]

    def _free_memory(self, ptr: ctypes.pointer) -> int:
        return self.dll_free_ptr_memory(ptr)

    def _get_data_from_ptr(self, char_ptr: ctypes.c_void_p) -> dict[str, Any]:
        self.csharp_ptrs.append(char_ptr)
        temp_str = ctypes.cast(char_ptr, ctypes.c_char_p).value.decode()
        out_dict = json.loads(temp_str)
        return out_dict
    
    def _str_to_ptr(self, input:str) -> int:
        c_string = ctypes.cast(ctypes.create_string_buffer(input.encode()), ctypes.c_void_p)
        self.py_ptrs.append(c_string)  # Prevent garbage collection
        return c_string.value
        

    def get_setting_locations(self, config: dict[str, Any]) -> dict[str, Any]:
        to_string = json.dumps(config)
        ptr = self._str_to_ptr(to_string)
        data_ptr = self.dll_get_setting_locations(ptr)
        return self._get_data_from_ptr(data_ptr)

    def get_setting_items(self, config: dict[str, Any]) -> dict[str, Any]:
        to_string = json.dumps(config)
        ptr = self._str_to_ptr(to_string)
        data_ptr = self.dll_get_setting_items(ptr)
        return self._get_data_from_ptr(data_ptr)

    def generate_rom(self, config: dict[str, Any]) -> int:
        to_string = json.dumps(config)
        ptr = self._str_to_ptr(to_string)
        return self.dll_generate_rom(ptr)

    def get_items(self) -> dict[str, Any]:
        data_ptr = self.dll_get_items()
        return self._get_data_from_ptr(data_ptr)

    def get_locations(self) -> dict[str, Any]:
        data_ptr = self.dll_get_locations()
        return self._get_data_from_ptr(data_ptr)

    # def get_settings(self) -> list[Setting]:
    #     return self.dll_get_settings()

    def somr_receive_item(self, input_obj: TestStruct) -> None:
        self.dll_somr_receive_item(ctypes.pointer(input_obj))

    # print("generate_rom:", generate_rom())
