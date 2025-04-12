from enum import Enum, auto
from pydantic import BaseModel, field_validator


class PrizeItem(BaseModel):
    prizeName: str
    prizeType: str
    eventData: bytes
    hintName: str
    gotItemEventFlag: int
    value: float
    uid: int


class PrizeLocation(BaseModel):
    locationName: str
    mapNum: int
    objNum: int
    eventNum: int
    eventReplacementIndex: int
    prizeTypeOptions: list[str]
    locationHints: list[str]
    lockedByPrizes: list[str]
    reachability: float = 0


class ItemType(Enum):
    TRAP = -1
    WEAPON = 0
    SEED = auto()
    SPELL = auto()
    KEY_ITEM = auto()
    CHARACTER = auto()
    FILLER = auto()
    ORB = auto()


class fdll_Item(BaseModel):
    name: str
    internal_name: str | None = ""
    id: int
    type: ItemType | None = -1

    @field_validator("internal_name", mode="before")
    def convert_none_to_empty_string(cls, v):
        if v is None:
            return ""
        return v

class LocationType(Enum):
    NAN = -1
    CHEST = 0
    BOSS = auto()
    CHECK = auto()

class fdll_Location(BaseModel):
    name: str
    internal_name: str | None = ""
    id: int
    type: LocationType | None = -1

    @field_validator("internal_name", mode="before")
    def convert_none_to_empty_string(cls, v):
        if v is None:
            return ""
        return v
