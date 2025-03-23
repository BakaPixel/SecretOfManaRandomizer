from enum import Enum, auto
from typing import Any, Self
from pydantic import BaseModel, field_validator, model_validator


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
    WEAPON = 0
    SEED = auto()
    SPELL = auto()
    PROGRESSION = auto()
    CHARACTER = auto()
    FILLER = auto()
    ORB = auto()


class LocationType(Enum):
    NAN = -1
    BOSS = auto()
    CHEST = auto()
    CHECK = auto()


class ProgressionLogic(BaseModel):
    progression: str
    amount: int = 1
    logic_group: int = 0


class fdll_Item(BaseModel):
    name: str
    internal_name: str | None = ""
    id: int
    progression: bool | None = False
    useful: bool | None = False
    type: ItemType | None = -1
    provides: list[ProgressionLogic] | None = []

    @field_validator("internal_name", mode="before")
    def convert_none_to_empty_string(cls, v):
        if v is None:
            return ""
        return v


class fdll_Location(BaseModel):
    name: str
    internal_name: str | None = ""
    type: LocationType | None = LocationType.NAN
    difficulty: int = -1
    id: int = -1
    requires: list[ProgressionLogic] | None = []
    provides: list[ProgressionLogic] | None = []
    children: list[Self] | None = []

    @model_validator(mode="before")
    def before_validator(cls, values: dict[str, Any]):
        id = values.get("id", -1)
        # TODO: Add id's??? and remove the False to enable this safety check or remove this whole thing
        if id < 0 and len(values.get("children", [])) == 0 and False:
            raise ValueError(f"Id: {id} isn't valid for {values} - Update with real id.")
        return values

    @field_validator("internal_name", mode="before")
    def convert_none_to_empty_string(cls, v):
        if v is None:
            return ""
        return v
