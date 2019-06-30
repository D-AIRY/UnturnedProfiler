﻿#region Copyright
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

using Harmony;
using ImperialPlugins.UnturnedProfiler.Configuration;
using Rocket.Core.Plugins;

namespace ImperialPlugins.UnturnedProfiler
{
	public class ProfilerPlugin: RocketPlugin<ProfilerConfig>
	{
		public const string HarmonyInstanceId = "host.dsn.com.imperialplugins.unturnedprofiler";

		public static ProfilerPlugin Instance
		{
			get; private set;
		}
		public bool IsProfiling
		{
			get; internal set;
		}
		public HarmonyInstance Harmony { get; } = HarmonyInstance.Create(HarmonyInstanceId);

		protected override void Load()
		{
			base.Load();
			Instance = this;

			DSN.Host.RCommand.RCommand.Register("prof_begin", new Commands.CommandStartProfiling());
			DSN.Host.RCommand.RCommand.Register("prof_end", new Commands.CommandStopProfiling());
		}

		protected override void Unload()
		{
			base.Unload();
			Instance = null;
		}
	}
}
