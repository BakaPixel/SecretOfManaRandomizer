from collections import defaultdict
from typing import Any
from models.from_dll_models import fdll_Location, ProgressionLogic
from itertools import product


def obj_obj_list_concat(obj_a: list[Any] | Any, obj_b: list[Any] | Any) -> list[Any]:
    """
    Returns a list of items concatenated together

    Args:
        obj_a (Any): First object
        obj_b (Any): Second object

    Returns:
        list[Any]: The concatenated list of items
    """
    if not isinstance(obj_a, list):
        obj_a = [obj_a]
    if not isinstance(obj_b, list):
        obj_b = [obj_b]
    output: list[Any] = obj_a + obj_b
    return output

def group_requirements(requirements: list[ProgressionLogic]) -> list[list[ProgressionLogic]]:
    # Group by logic_group to separate AND/OR conditions
    grouped: dict[int, list[ProgressionLogic]] = defaultdict(list)
    for req in requirements:
        grouped[req.logic_group].append(req)
    return list(grouped.values())

def combine_logic_groups(prev_groups: list[list[ProgressionLogic]], new_groups: list[list[ProgressionLogic]]) -> list[ProgressionLogic]:
    if len(prev_groups) == 0 and len(new_groups) > 0:
        return new_groups[0]
    if len(new_groups) == 0 and len(prev_groups) > 0:
        return prev_groups
    if len(new_groups) + len(prev_groups) == 0:
        return []
    
    to_re_group = [obj_obj_list_concat(prev, new) for prev, new in product(prev_groups, new_groups)]

    # Create new instances with updated logic_group indices
    new_to_re_group = []
    for i, l_group in enumerate(to_re_group):
        new_group = [ProgressionLogic(progression=prog.progression, amount=prog.amount, logic_group=i) for prog in l_group]
        new_to_re_group.append(new_group)

    output = []
    for l_list in new_to_re_group:
        output += l_list

    return output

def process_locations(locations: list[fdll_Location]) -> list[fdll_Location]:
    output = []
    
    def recurse(location: fdll_Location, accumulated_reqs: list[list[ProgressionLogic]]):
        current_reqs = group_requirements(location.requires)
        # Combine the current layer's logic groups with accumulated previous ones
        combined_reqs = combine_logic_groups(accumulated_reqs, current_reqs)

        if len(location.children) == 0:
            # If no children, add this location to the output with combined requirements
            output.append(fdll_Location(
                name=location.name,
                internal_name=location.internal_name,
                type=location.type,
                difficulty=location.difficulty,
                requires=combined_reqs,
                provides=location.provides,
                children=[]
            ))
        else:
            # Recurse for each child, passing down the combined requirements
            for child in location.children:
                recurse(child, combined_reqs)

    # Start recursion for each location in the list
    for loc in locations:
        recurse(loc, [])

    return output
