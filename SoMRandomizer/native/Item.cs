using System.Collections.Generic;

namespace SoMRandomizer.native
{
	public interface ISerializableObject
	{
		Dictionary<string, object> toDict();
	}

    public struct Item : ISerializableObject
	{
        public string name;
        public string internal_name;
        public long id;

		public Dictionary<string, object> toDict()
		{
			var output = new Dictionary<string, object> { };
			output.Add("name", name);
			output.Add("internal_name", internal_name);
			output.Add("id", id);
			return output;
		}

    }
    
    public struct Location : ISerializableObject
	{
        public string name;
        public string internal_name;
        public long id;

		public Dictionary<string, object> toDict()
		{
			var output = new Dictionary<string, object> { };
			output.Add("name", name);
			output.Add("internal_name", internal_name);
			output.Add("id", id);
			return output;
		}
	}
}