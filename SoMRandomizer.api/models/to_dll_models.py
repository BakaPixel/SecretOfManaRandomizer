import ctypes
from dataclasses import dataclass


class BaseStruct(ctypes.Structure):
    """
    Custom base class for ctypes.Structure with automatic type conversion.
    Converts Python str to bytes when using c_char_p.
    """

    def __setattr__(self, name, value):
        # Look up field type in _fields_
        for field_name, field_type in self._fields_:
            if field_name == name:
                # Nest another if block so we can add more type conversion if needed
                if field_type is ctypes.c_char_p:
                    # Convert string to c_char_p if it's a string field
                    value = value.encode()
        super().__setattr__(name, value)


# Define the Item structure based on the C# struct
@dataclass
class tdll_Item(BaseStruct):
    _fields_ = [
        ("name", ctypes.c_char_p),
        ("internal_name", ctypes.c_char_p),
        ("id", ctypes.c_int),
    ]

    name: str
    internal_name: str
    id: int


@dataclass
class tdll_Location(BaseStruct):
    _fields_ = [
        ("name", ctypes.c_char_p),
        ("internal_name", ctypes.c_char_p),
        ("id", ctypes.c_int),
    ]

    name: str
    internal_name: str
    id: int
