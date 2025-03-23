using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace SoMRandomizer.native
{
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
		WEAPON = 0,
		SEED,
		SPELL,
		PROGRESSION,
		CHARACTER,
		FILLER,
		ORB,
	}

	public class Item : ISerializableObject
	{
        public string name;
        public string internal_name = null;
        public long id;
		public bool progression = false;
		public bool useful = false;
		public int type = -1;
		public List<ProgressionLogic> provides = new List<ProgressionLogic> { };

		public Dictionary<string, object> toDict()
		{
			var output = new Dictionary<string, object>
			{
				{ "name", name },
				{ "internal_name", internal_name },
				{ "id", id },
				{ "progression", progression },
				{ "useful", useful },
				{ "type", type },
				{ "provides", provides.ConvertAll(p => (Dictionary<string, object>)p) },
			};
			return output;
		}

	}
	public enum LocationType
	{
		BOSS = 0,
		CHEST,
		CHECK,
	}

	public class Location : ISerializableObject
	{
        public string Name;
		public int Difficulty = 0;
		public int Type = -1;
		public int Id = -1;
		public List<ProgressionLogic> Requires = new List<ProgressionLogic> { };
		public List<ProgressionLogic> Provides = new List<ProgressionLogic> { };
		public List<Location> Children = new List<Location> { };

		public Dictionary<string, object> toDict()
		{
			var output = new Dictionary<string, object>
			{
				{ "name", Name },
				{ "difficulty", Difficulty },
				{ "type", Type },
				{ "id", Id },
				{ "requires", Requires.ConvertAll(p => (Dictionary<string, object>)p) },
				{ "provides", Provides.ConvertAll(p => (Dictionary<string, object>)p) },
				{ "children", Children.ConvertAll(p => (Dictionary<string, object>)p) },
			};
			return output;
		}

		public static explicit operator Dictionary<string, object>(Location location)
		{
			return location.toDict();
		}
	}
}