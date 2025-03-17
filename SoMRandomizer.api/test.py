import uuid
from SOMR_API import SOMR_API, TestStruct

if __name__ == "__main__":
    # "bin\Debug\net9.0\win-x64\publish\SoMRandomizer.api"

    # In the future this should probably be moved to a base folder and assumed to be there without the string being passed through
    # Leaving this here for sake of testing
    somr_api = SOMR_API(r"bin\Debug\net9.0\win-x64\publish\SoMRandomizer.api")

    print(f"DLL Entry Points: {somr_api._get_entrypoints()}")

    rom_name = "Secret of Mana (USA)"
    seed_value = uuid.uuid4()

    config = {
        "sourcePath":f"SoMRandomizer.api\\{rom_name}.sfc",
        "destPath":f"SoMRandomizer.api\\{rom_name}_out.sfc",
        "seed":f"{seed_value}",
        "entries":{
            "opStartChar": "random",
            "characterColors": "none",
        },
    }

    # somr_api.generate_rom(config=config)
    # output = somr_api.get_setting_locations(config=config)["list"]
    output = somr_api.get_setting_items(config=config)["list"]
    print(output)
    # output = somr_api.get_items()
    # print(output)
    # output = somr_api.get_locations()
    # print(output)

    # del somr_api

    print("Test Done")

    # items = somr_api.get_items()
    # for item in items:
    #     # item.id
    #     # item.internal_name
    #     # item.name
    #     print(item)

    # locations = somr_api.get_locations()
    # for location in locations:
    #     print(location)

    # print("get_settings:")
    # settings = somr_api.get_settings()
    # print(settings)
    # # for setting in settings:
    # #     print(setting)

    # print("Creating an 'item' to send to dll to have it print out.")
    # test = TestStruct("TEST", "INTERNAL", 1001)
    # somr_api.somr_receive_item(test)

    # for item in items:
    #     temp_struct = TestStruct(name=item.name, internal_name=item.internal_name, id=item.id)
    #     somr_api.somr_receive_item(temp_struct)
