import ctypes
from dataclasses import dataclass
import json
from typing import Any

from pydantic import BaseModel, model_validator


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
class tdll_Item():
    prizeName: str
    prizeType: str
    data: bytes
    hint: str
    eventFlag: int
    prizeValue: float

    def __init__(self, input: object):
        from .from_dll_models import PrizeItem
        if isinstance(input, PrizeItem):
            temp: PrizeItem = input
            self.prizeName = temp.prizeName
            self.prizeType = temp.prizeType
            self.data = temp.eventData
            self.hint = temp.hintName
            self. eventFlag = temp.gotItemEventFlag
            self.prizeValue = temp.value

    def to_dict(self):
        return {
            "prizeName": self.prizeName,
            "prizeType": self.prizeType,
            "data": self.data.decode(),
            "hint": self.hint,
            "eventFlag": self.eventFlag,
            "prizeValue": self.prizeValue
        }


@dataclass
class tdll_Location():
    name: str
    map: int
    obj: int
    evNum: int
    evReplaceIndex: int
    typeOptions: list[str]
    hints: list[str]
    lockedBy: list[str]
    locationReachability: float

    def __init__(self, input: object):
        from .from_dll_models import PrizeLocation
        if isinstance(input, PrizeLocation):
            temp: PrizeLocation = input
            self.name = temp.locationName
            self.map = temp.mapNum
            self.obj = temp.objNum
            self.evNum = temp.eventNum
            self.evReplaceIndex = temp.eventReplacementIndex
            self.typeOptions = temp.prizeTypeOptions
            self.hints = temp.locationHints
            self.lockedBy = temp.lockedByPrizes
            self.locationReachability = temp.reachability

    def to_dict(self):
        return self.__dict__

@dataclass
class tdll_GenerateConfig(BaseStruct):
    _fields_ = [
        ("sourcePath", ctypes.c_char_p),
        ("destPath", ctypes.c_char_p),
        ("seed", ctypes.c_char_p),
        ("entries", ctypes.c_char_p),
    ]
    sourcePath: str
    destPath: str
    seed: str | int
    entries: str | dict[str, Any]

    def __setattr__(self, name, value):
        if name == "entries" and isinstance(value, dict):
            value = json.dumps(value)  # Convert dict to JSON string
        elif name == "seed" and isinstance(value, int):
            value = f"{value}"
        super().__setattr__(name, value)

@dataclass
class tdll_GenerateAP(BaseStruct):
    _fields_ = [
        ("SoMRHubIndex", ctypes.c_int),
        ("PrizeLocations", ctypes.c_char_p),
        ("PrizeItems", ctypes.c_char_p),
    ]

    SoMRHubIndex: int
    PrizeLocations: str | list[tdll_Location]
    PrizeItems: str | list[tdll_Item]

    def __setattr__(self, name, value):
        if name == "PrizeLocations" and isinstance(value, list):
            temp: list[tdll_Location] = value
            value = json.dumps([loc.to_dict() for loc in temp])
        elif name == "PrizeItems" and isinstance(value, list):
            temp: list[tdll_Item] = value
            value = json.dumps([item.to_dict() for item in temp])
        super().__setattr__(name, value)