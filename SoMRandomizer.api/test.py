import base64
import json
from typing import Callable
import uuid

from SOMR_API import SOMR_API
from models.to_dll_models import tdll_GenerateConfig, tdll_Item, tdll_Location, tdll_GenerateAP
import somr_utils
import copy
from itertools import product

if __name__ == "__main__":
    # "bin\Debug\net9.0\win-x64\publish\SoMRandomizer.api"

    # In the future this should probably be moved to a base folder and assumed to be there without the string being passed through
    # Leaving this here for sake of testing
    somr_api = SOMR_API()

    print(f"DLL Entry Points: {somr_api._get_entrypoints()}")

    rom_name = "Secret of Mana (USA)"
    seed_value = uuid.uuid4()

    config = tdll_GenerateConfig(
        sourcePath= f"SoMRandomizer.api\\{rom_name}.sfc",
        destPath= f"SoMRandomizer.api\\{rom_name}_out.sfc",
        seed= 77788821357963,
        entries= {
            "opLogic": "restricitve",
        },
    )
    
    # somr_api.generate_rom(config=config)
    # hub_index = somr_api.start_context(config=config)
    for item in somr_api.get_items():
        print(item)
    # somr_api.generate_rom(gen_ap)

    # for loc in output:
    #     print(loc)
    # output = somr_api.get_setting_locations(hub_index)[:3]
    # for loc in output:
    #     print(loc)
    # output = somr_api.get_setting_locations(hub_index)[:3]
    # for loc in output:
    #     print(loc)

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
    # If there are no more references to the object, `cleanup` should run

    # flattened_locations = somr_utils.process_locations()
    # for flat in flattened_locations:
    #     if flat.name == "Pandora Castle Chest - 5":
    #         output = somr_utils.create_logic_tuples(flat)
    #         print(flat)
    #         print(output)

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
