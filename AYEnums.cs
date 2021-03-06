﻿/**
 * Enums.cs
 * (C) Copyright 2015, Jamie Leighton
 * AmpYear power management.
 * The original code and concept of AmpYear rights go to SodiumEyes on the Kerbal Space Program Forums, which was covered by GNU License GPL (no version stated).
 * As such this code continues to be covered by GNU GPL license.
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 *
 *  This file is part of AmpYear.
 *
 *  AmpYear is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  AmpYear is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with AmpYear.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.ComponentModel;

namespace AY
{
    public enum Subsystem
    {
        [Description("#autoLOC_AmpYear_1000159")] SAS,
        [Description("#autoLOC_AmpYear_1000160")] RCS,
        [Description("#autoLOC_AmpYear_1000161")] CLIMATE,
        [Description("#autoLOC_AmpYear_1000162")] MUSIC,
        [Description("#autoLOC_AmpYear_1000163")] MASSAGE
    }

    public enum GUISection
    {
        [Description("#autoLOC_AmpYear_1000164")] SUBSYSTEM,
        [Description("#autoLOC_AmpYear_1000165")] RESERVE,
        [Description("#autoLOC_AmpYear_1000166")] LUXURY
    }

    public enum GameState
    {
        FLIGHT = 0,
        EDITOR = 1,
        EVA = 2
    }

    public enum IconAlertState
    {
        GREEN = 0,
        YELLOW = 1,
        RED = 2,
        GRAY = 3
    }

    public enum ESPPriority
    {
        HIGH = 1,
        MEDIUM = 2,
        LOW = 3
    }

    public enum PowerState
    {
        Increasing = 1,
        Decreasing = 2
    }
}