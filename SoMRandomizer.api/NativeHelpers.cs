using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using SoMRandomizer.native;
using SoMRandomizer.processing.openworld.randomization;

namespace SoMRandomizer.api
{
    public static class NativeHelpers
    {
		public static unsafe Dictionary<string, object> dataToDict<T>(List<T> inputData) where T : ISerializableObject
		{
			Dictionary<string, object> dictOut = new Dictionary<string, object> { };
			dictOut.Add("list", new List<Dictionary<string, object>>());
			foreach (ISerializableObject item in inputData)
			{
				(dictOut["list"] as List<Dictionary<string, object>>).Add(item.toDict());
			}

			return dictOut;
		}

		public static unsafe IntPtr ObjectToIntPtr(object input)
		{
			return Marshal.StringToHGlobalAnsi(JsonConvert.SerializeObject(input));
		}
    }
}