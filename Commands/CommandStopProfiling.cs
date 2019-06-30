#region Copyright
/*
 *  Unturned Profiler - A plugin for profiling Unturned servers and analyzing lag causes
 *  Copyright (C) 2017-2019 Enes Sadık Özbek <esozbek.me>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as
 *  published by the Free Software Foundation, either version 3 of the
 *  License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Linq;
using System.Text;
using ImperialPlugins.UnturnedProfiler.Extensions;
using ImperialPlugins.UnturnedProfiler.Patches;
using Newtonsoft.Json.Linq;

namespace ImperialPlugins.UnturnedProfiler.Commands
{
	public class CommandStopProfiling: DSN.Host.RCommand.RCommand
	{
		public override string Execute(string args)
		{
			var pluginInstance = ProfilerPlugin.Instance;
			var registrations = HarmonyProfiling.GetAllRegistrations();

			if(!pluginInstance.IsProfiling)
			{
				return(Error("Profiling is not running"));
			}

			pluginInstance.IsProfiling = false;
			var measureTypes = registrations.Values.Where(d => d.Measurements.Count > 0).Select(c => c.MeasureType).Distinct();

			JObject data = new JObject();

			foreach(var measureType in measureTypes)
			{
				StringBuilder sb = new StringBuilder();
				JArray measureList = new JArray();

				bool anyCallsMeasured = false;
				foreach(var measurableMethod in registrations.Values.Where(d => d.MeasureType.Equals(measureType, StringComparison.OrdinalIgnoreCase)))
				{
					var assemblyName = measurableMethod.Method.DeclaringType?.Assembly?.GetName()?.Name?.StripUtf8() ?? "<unknown>";
					var measurements = measurableMethod.Measurements;
					if(!measurements.Any())
					{
						continue;
					}

					string methodName = measurableMethod.Method.GetFullName().StripUtf8();

					//calculate & log averages
					JObject obj = new JObject
					{
						{"assemblyName", assemblyName},
						{"methodName", methodName},
						{"avg", measurements.Average()},
						{"min", measurements.Min()},
						{"max", measurements.Max()},
						{"calls", measurements.Count}
					};
					measureList.Add(obj);

					anyCallsMeasured = true;
				}

				if(anyCallsMeasured)
				{
					data.Add(measureType, measureList);
				}
			}

			HarmonyProfiling.ClearRegistrations();
			return(Success(data));
		}
	}
}
