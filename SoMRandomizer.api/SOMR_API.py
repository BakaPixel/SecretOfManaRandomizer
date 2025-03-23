import ctypes
import json
import sys
from pathlib import Path
from typing import Any

from pydantic import BaseModel
from models.from_dll_models import PrizeItem, PrizeLocation, fdll_Item, fdll_Location
from models.to_dll_models import tdll_Item, tdll_Location


class SOMR_API:
    def __init__(self):
        """Load the DLL and initialize function bindings."""

        dll_path = r"bin\Debug\net9.0\win-x64\publish\SoMRandomizer.api"
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
            return_type=fdll_Item,
        )

        self._func_declare(
            name="get_locations",
            restype=ctypes.c_void_p,
            return_type=fdll_Location,
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
            argtypes=[ctypes.POINTER(tdll_Item)],
        )

        self._func_declare(
            name="somr_receive_location",
            argtypes=[ctypes.POINTER(tdll_Location)],
        )

        self._func_declare(
            name="get_setting_locations",
            argtypes=[ctypes.c_void_p],
            restype=ctypes.c_void_p,
            return_type=PrizeLocation,
        )

        self._func_declare(
            name="get_setting_items",
            argtypes=[ctypes.c_void_p],
            restype=ctypes.c_void_p,
            return_type=PrizeItem,
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
        self,
        name: str,
        argtypes: list[ctypes.POINTER] | None = None,
        restype: ctypes._SimpleCData | None = None,
        return_type: BaseModel | None = None,
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
        func.return_type = return_type

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

    def _str_to_ptr(self, input: str) -> int:
        c_string = ctypes.cast(ctypes.create_string_buffer(input.encode()), ctypes.c_void_p)
        self.py_ptrs.append(c_string)  # Prevent garbage collection
        return c_string.value

    def _convert_list_to_type(self, func: Any, list_of_data: list[Any]) -> list[Any]:
        return [func.return_type.model_validate(item) for item in list_of_data] if func.return_type else list_of_data

    def _get_hash(self, hash_algorithm: str = "md5") -> str:
        import hashlib

        rom_name = "Secret of Mana (USA)"
        file_path = f"SoMRandomizer.api\\{rom_name}.sfc"
        hash_func = hashlib.new(hash_algorithm)  # You can use 'sha256', 'sha1', etc.

        with open(file_path, "rb") as f:
            while chunk := f.read(8192):  # Read the file in chunks (8KB at a time)
                hash_func.update(chunk)

        return hash_func.hexdigest()

    def get_setting_locations(self, config: dict[str, Any]) -> list[PrizeLocation]:
        to_string = json.dumps(config)
        ptr = self._str_to_ptr(to_string)
        data_ptr = self.dll_get_setting_locations(ptr)
        list_of_data = self._get_data_from_ptr(data_ptr)["list"]
        output = self._convert_list_to_type(self.dll_get_setting_locations, list_of_data)
        return output

    def get_setting_items(self, config: dict[str, Any]) -> list[PrizeItem]:
        to_string = json.dumps(config)
        ptr = self._str_to_ptr(to_string)
        data_ptr = self.dll_get_setting_items(ptr)
        list_of_data = self._get_data_from_ptr(data_ptr)["list"]
        output = self._convert_list_to_type(self.dll_get_setting_items, list_of_data)
        return output

    def generate_rom(self, config: dict[str, Any]) -> int:
        to_string = json.dumps(config)
        ptr = self._str_to_ptr(to_string)
        return self.dll_generate_rom(ptr)

    def get_items(self) -> list[fdll_Item]:
        data_ptr = self.dll_get_items()
        list_of_data = self._get_data_from_ptr(data_ptr)["list"]
        output = self._convert_list_to_type(self.dll_get_items, list_of_data)
        return output

    def get_locations(self) -> list[fdll_Location]:
        data_ptr = self.dll_get_locations()
        list_of_data = self._get_data_from_ptr(data_ptr)["list"]
        output = self._convert_list_to_type(self.dll_get_locations, list_of_data)
        return output

    # def get_settings(self) -> list[Setting]:
    #     return self.dll_get_settings()

    def somr_receive_item(self, input_obj: tdll_Item) -> None:
        self.dll_somr_receive_item(ctypes.pointer(input_obj))

    def somr_receive_location(self, input_obj: tdll_Location) -> None:
        self.dll_somr_receive_location(ctypes.pointer(input_obj))
