import json
import uuid

from SOMR_API import SOMR_API
from models.to_dll_models import tdll_Item, tdll_Location
from models.from_dll_models import ProgressionLogic, fdll_Location
import SOMR_Utils
import copy
from itertools import product

if __name__ == "__main__":
    # "bin\Debug\net9.0\win-x64\publish\SoMRandomizer.api"

    # In the future this should probably be moved to a base folder and assumed to be there without the string being passed through
    # Leaving this here for sake of testing
    somr_api = SOMR_API()

    print(somr_api._get_hash().upper())
    print(f"DLL Entry Points: {somr_api._get_entrypoints()}")

    rom_name = "Secret of Mana (USA)"
    seed_value = uuid.uuid4()

    config = {
        "sourcePath": f"SoMRandomizer.api\\{rom_name}.sfc",
        "destPath": f"SoMRandomizer.api\\{rom_name}_out.sfc",
        "seed": f"{seed_value}",
        "entries": {
            "opStartChar": "random",
            "opExp": 1000,
            "opStatGrowth": "bosses",
        },
    }

    # somr_api.generate_rom(config=config)
    # output = somr_api.get_setting_locations(config=config)

    # output = copy.deepcopy(somr_api.get_setting_items(config=config))
    # for value in output:
    #     print(value.prizeName)
    # config["seed"] = "a"
    # output_b = copy.deepcopy(somr_api.get_setting_items(config=config))
    # config["seed"] = f"{seed_value}"
    # output_c = copy.deepcopy(somr_api.get_setting_items(config=config))
    # print(output)
    # output = somr_api.get_items()
    # print(output)
    # output = somr_api.get_locations()
    # print(output)

    # del somr_api

    # items = somr_api.get_items()
    # for item in items:
    #     # item.id
    #     # item.internal_name
    #     # item.name
    #     print(item)

    def print_loc(input: fdll_Location):
        print(f"name: {input.name}, type: {input.type}")
        for child in input.children:
            print_loc(child)

    locations = somr_api.get_locations()
    # for location in locations:
    #     print_loc(location)
    flattened_locations = SOMR_Utils.process_locations(locations)
    for flat in flattened_locations:
        if flat.name == "Pure Land Mana Tree":
            print(flat)

    # print("get_settings:")
    # settings = somr_api.get_settings()
    # print(settings)
    # # for setting in settings:
    # #     print(setting)

    # print("Creating an 'item' to send to dll to have it print out.")
    # test = Item(name="TEST", internal_name="INTERNAL", id=1001)
    # print(test)
    # somr_api.somr_receive_item(test)
    # test = Location("TEST", "INTERNAL", 1001)
    # print(test)
    # somr_api.somr_receive_location(test)

    # for item in items:
    #     temp_struct = tdll_Item(name=item.name, internal_name=item.internal_name, id=item.id)
    #     somr_api.somr_receive_item(temp_struct)

    print("Test Done")
