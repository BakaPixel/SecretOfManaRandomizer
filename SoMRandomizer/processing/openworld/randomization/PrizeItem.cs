
using SoMRandomizer.native;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// An open world "check" prize.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class PrizeItem : ISerializableObject
    {
        // this is so i can stick them in dictionaries and not rely on them all to have unique names, because they do not
        private static int PRIZE_UID = 0;
        // name of the prize
        public string prizeName;
        // i think this is actually not used currently and can probably be removed
        public string prizeType;
        // event data to inject (including dialogue) to give prize
        public byte[] eventData;
        // anything else needed here?
        public string hintName;
        // event flag to flip to 1 when we got the prize
        public byte gotItemEventFlag;
        public double value; // higher = more important
        private int uid;

		public static void initUID()
		{
			// Used mainly by Python API to make sure the Random Generation returns expected results with multiple calls.
			PRIZE_UID = 0;
		}

        public PrizeItem(string name, string type, byte[] data, string hint, byte eventFlag, double prizeValue)
        {
            prizeName = name;
            prizeType = type;
            eventData = data;
            hintName = hint;
            gotItemEventFlag = eventFlag;
            value = prizeValue;
            uid = PRIZE_UID++;
        }


		public Dictionary<string, object> toDict()
		{
			Dictionary<string, object> outDict = new Dictionary<string, object> { };
			//outDict.Add("PRIZE_UID", PRIZE_UID); // Not needed since everything has the same PRIZE_UID?
			outDict.Add("prizeName", prizeName);
			outDict.Add("prizeType", prizeType);
			outDict.Add("eventData", eventData.Select(b => (int)b).ToList());
			outDict.Add("hintName", hintName);
			outDict.Add("gotItemEventFlag", gotItemEventFlag);
			outDict.Add("value", value);
			outDict.Add("uid", uid);

			return outDict;
		}

        public override bool Equals(object obj)
        {
            return obj is PrizeItem && uid == ((PrizeItem)obj).uid;
        }

        public override int GetHashCode()
        {
            return uid;
        }

    }
}
