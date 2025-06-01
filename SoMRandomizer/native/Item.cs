using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoMRandomizer.processing.openworld.randomization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace SoMRandomizer.native
{
	public struct GenerateAP
	{
		public int SoMRHubIndex { get; set; }
		public string PrizeLocations { get; set; }
		public string PrizeItems { get; set; }
		public string sourcePath { get; set; }
		public string destPath { get; set; }

		public List<PrizeItem> GetPrizeItems()
		{
			var list_jo = (JsonConvert.DeserializeObject(PrizeItems) as JArray).ToList();

			var output = new List<PrizeItem>();

			foreach (var item in list_jo)
			{
				var to_add = new PrizeItem
				(
					(string)item["prizeName"],
					(string)item["prizeType"],
					item["data"].Select(i => (byte)i).ToArray(),
					(string)item["hint"],
					(byte)item["eventFlag"],
					(double)item["prizeValue"]
				);
				output.Add(to_add);
			}
			return output;
		}

		public List<PrizeLocation> GetPrizeLocations()
		{
			var list_jo = (JsonConvert.DeserializeObject(PrizeLocations) as JArray).ToList();

			var output = new List<PrizeLocation>();

			foreach (var item in list_jo)
			{
				var to_add = new PrizeLocation
				(
					(string)item["name"],
					(int)item["map"],
					(int)item["obj"],
					(int)item["evNum"],
					(int)item["evReplaceIndex"],
					(item["typeOptions"] as JArray).Select(i => i.ToString()).ToArray(),
					(item["hints"] as JArray).Select(i => i.ToString()).ToArray(),
					(item["lockedBy"] as JArray).Select(i => i.ToString()).ToArray(),
					(double)item["locationReachability"]
				);
				output.Add(to_add);
			}
			return output;
		}
	}

	public struct GenerateConfig
	{
		public string seed { get; set; }
		public string entries { get; set; }

		public Dictionary<string, string> getEntries()
		{
			return JsonConvert.DeserializeObject<Dictionary<string, string>>(entries);
		}
	}
	public interface ISerializableObject
	{
		Dictionary<string, object> toDict();
	}

	public class ProgressionLogic
	{
		public int Amount = 1;
		public string Progression;
		public int LogicGroup = 0;

		public static explicit operator Dictionary<string, object>(ProgressionLogic progressionLogic)
		{
			var output = new Dictionary<string, object>
			{
				{ "progression", progressionLogic.Progression },
				{ "amount", progressionLogic.Amount },
				{ "logic_group", progressionLogic.LogicGroup },
			};
			return output;
		}
	}

	public enum ItemType
	{
		TRAP = -1,
		WEAPON = 0,
		SEED,
		SPELL,
		KEY_ITEM,
		CHARACTER,
		FILLER,
		ORB,
	}

	public class Item : ISerializableObject
	{
        public string name;
        public string internal_name = null;
        public long id;
		public int type = -1;
		public int _count = 1;

		public Dictionary<string, object> toDict()
		{
			var output = new Dictionary<string, object>
			{
				{ "name", name },
				{ "internal_name", internal_name },
				{ "id", id },
				{ "type", type },
			};
			return output;
		}
	}

	public enum LocationType
	{
		NAN = -1,
		CHEST = 0,
		BOSS,
		CHECK,
	}
	public class Location : ISerializableObject
	{
		public string name;
		public string internal_name;
		public long id;
		public int type = -1;

		public Dictionary<string, object> toDict()
		{
			var output = new Dictionary<string, object>
			{
				{ "name", name },
				{ "internal_name", internal_name },
				{ "id", id },
				{ "type", type },
			};
			return output;
		}
	}
}