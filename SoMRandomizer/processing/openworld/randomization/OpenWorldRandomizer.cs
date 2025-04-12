using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.openworld.PlandoProperties;
using static SoMRandomizer.processing.openworld.randomization.OpenWorldSimulator;

namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// Main randomization for open world.  Pairs locations to prizes with the given settings, and injects the prizes into events.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldRandomizer : RandoProcessor
    {
        protected override string getName()
        {
            return "Open world randomizations";
        }

		protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
		{
			List<PrizeLocation> allLocations;
			List<PrizeItem> allPrizes;

			Dictionary<PrizeLocation, PrizeItem> finalRandomization = null;
			SimResult finalResult = null;
			if (context.genStart == "AP")
			{
				Console.WriteLine("Assuming Archipelago knows what's best and setting the PrizeLocation / PrizeItem pairs based on the data it sent in.");
				allLocations = context.owAllLocations;
				allPrizes = context.owAllPrizes;

				settings.set(OpenWorldSettings.PROPERTYNAME_COMPLEXITY, "dontcare");
				context.plandoSettings.Add(KEY_BYPASS_VALIDATION, new List<string> { "yes" });
			}
			else
			{
				allLocations = OpenWorldLocations.getForSelectedOptions(settings, context);
				allPrizes = OpenWorldPrizes.getForSelectedOptions(settings, context, allLocations);
			}
			string complexity = settings.get(OpenWorldSettings.PROPERTYNAME_COMPLEXITY); // easy, dontcare, hard
																							// number of seeds to make for easy/hard, and take the best/worst one
			int maxIters = 1000;
			SimResult lastSimulatedResult = null;
			Dictionary<PrizeLocation, PrizeItem> lastSimulatedPrizes = null;
			int iter = 0;
			// try to randomize.
			switch (complexity)
			{
				case "easy":
					{
						SimResult bestResult = null;
						Dictionary<PrizeLocation, PrizeItem> bestPrizes = null;
						while (iter < maxIters)
						{
							Dictionary<PrizeLocation, PrizeItem> placedPrizes = new Dictionary<PrizeLocation, PrizeItem>();
							SimResult result = attemptRando(allLocations, allPrizes, settings, context, placedPrizes, false);
							if (result != null)
							{
								lastSimulatedResult = result;
								lastSimulatedPrizes = placedPrizes;
								if (result.score > 0 && (bestResult == null || result.score > bestResult.score))
								{
									bestPrizes = placedPrizes;
									bestResult = result;
									// best one yet; keep trying
									Logging.log("Current open world solution: iteration " + iter + "; score = " + result.score, "debug");
								}
							}
							iter++;
						}
						finalRandomization = bestPrizes;
						finalResult = bestResult;
					}
					break;
				case "hard":
					{
						SimResult bestResult = null;
						Dictionary<PrizeLocation, PrizeItem> bestPrizes = null;
						while (iter < maxIters)
						{
							Dictionary<PrizeLocation, PrizeItem> placedPrizes = new Dictionary<PrizeLocation, PrizeItem>();
							SimResult result = attemptRando(allLocations, allPrizes, settings, context, placedPrizes, false);
							if (result != null)
							{
								lastSimulatedResult = result;
								lastSimulatedPrizes = placedPrizes;
								if (result.score > 0 && (bestResult == null || result.score < bestResult.score))
								{
									bestPrizes = placedPrizes;
									bestResult = result;
									// best one yet; keep trying
									Logging.log("Current open world solution: iteration " + iter + "; score = " + result.score, "debug");
								}
							}
							iter++;
						}
						finalRandomization = bestPrizes;
						finalResult = bestResult;
					}
					break;
				default:
					{
						while (iter < maxIters)
						{
							Dictionary<PrizeLocation, PrizeItem> placedPrizes = new Dictionary<PrizeLocation, PrizeItem>();
							SimResult result = attemptRando(allLocations, allPrizes, settings, context, placedPrizes, true);
							if (result != null)
							{
								lastSimulatedResult = result;
								lastSimulatedPrizes = placedPrizes;
								if (result.score > 0)
								{
									// got any working one; quit now
									finalRandomization = placedPrizes;
									finalResult = result;
									break;
								}
							}
							iter++;
						}
						if (finalResult != null)
						{
							Logging.log("Found an open world solution on iteration " + iter, "debug");
						}
					}
					break;
			}
			if (finalRandomization == null)
			{
				// log lastSimulatedResult if non-null
				if (lastSimulatedResult != null)
				{
					OpenWorldSpoilers.logFailedSimulation(lastSimulatedPrizes, lastSimulatedResult);
				}
				// no attempt to randomize worked. fail out
				throw new Exception("Open world generation failed!");
			}
			// injection into events
			OpenWorldResultInjector.injectRandomization(outRom, finalRandomization, settings, context);
			// hints
			OpenWorldHints.addHints(settings, context, finalRandomization, finalResult);
			// spoiler log
			OpenWorldSpoilers.logOpenWorldSpoilers(finalRandomization, finalResult, context);
            Logging.log("Open world generation finished!");
            return true;
        }

        private SimResult attemptRando(List<PrizeLocation> allLocations, List<PrizeItem> allPrizes, RandoSettings settings, RandoContext context, Dictionary<PrizeLocation, PrizeItem> placedPrizes, bool allowBypass)
		{
			Dictionary<string, List<string>> plandoSettings = context.plandoSettings;
			bool placedItems = false;
			if (context.genStart == "AP")
			{
				for(var i = 0; i  < allPrizes.Count; i++)
				{
					placedPrizes[allLocations[i]] = allPrizes[i];
				}
				placedItems = true;
			}
			else
			{
				placedItems = OpenWorldLocationPrizeMatcher.placeItems(allLocations, allPrizes, settings, context, placedPrizes);
			}
			if (allowBypass && plandoSettings.ContainsKey(KEY_BYPASS_VALIDATION) && plandoSettings[KEY_BYPASS_VALIDATION].Contains("yes"))
            {
                // skip sim if bypass validation was enabled
                SimResult bypass = new SimResult();
                bypass.collectionCycles = new List<List<string>>();
                // just assume everything can be gotten on cycle 0
				bypass.collectionCycles.Add(allLocations.Select(x => x.locationName).ToList());
				bypass.score = 100;
                return bypass;
			}
			if (placedItems)
            {
                return OpenWorldSimulator.runSimulation(placedPrizes, settings, context);
            }
            return null;
        }
    }
}
