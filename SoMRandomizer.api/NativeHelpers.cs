using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoMRandomizer.config.settings;
using SoMRandomizer.native;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.openworld.randomization;

namespace SoMRandomizer.api
{
	public class SoMRHub(string seed, OpenWorldSettings settings, RandoContext context, GenerateConfig config)
	{
		public string seed = seed;
		public OpenWorldSettings settings = settings;
		public RandoContext context = context;
		public GenerateConfig config = config;
	}

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