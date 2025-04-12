from models.to_dll_models import tdll_Item, tdll_Location
from models.from_dll_models import PrizeItem, PrizeLocation


def convert_pipl(items: list[PrizeItem], locations: list[PrizeLocation]) -> tuple[list[tdll_Item], list[tdll_Location]]:
    out_items = [tdll_Item(x) for x in items]
    out_locations = [tdll_Location(x) for x in locations]
    return out_items, out_locations
