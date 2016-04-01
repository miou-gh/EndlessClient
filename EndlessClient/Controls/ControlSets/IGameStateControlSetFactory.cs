﻿// Original Work Copyright (c) Ethan Moffat 2014-2016
// This file is subject to the GPL v2 License
// For additional details, see the LICENSE file

using EndlessClient.Game;

namespace EndlessClient.Controls.ControlSets
{
	public interface IGameStateControlSetFactory
	{
		IControlSet CreateForState(GameStates newState, IControlSet currentControlSet);
	}
}
