﻿using System;
using System.Linq;
using EliteJournalReader;
using EliteJournalReader.Events;


namespace joystick_feedback
{
    public class EliteData
    {
        public class Status
        {
            public bool Docked { get; set; }
            public bool Landed { get; set; }
            public bool LandingGearDown { get; set; }
            public bool ShieldsUp { get; set; }
            public bool Supercruise { get; set; }
            public bool FlightAssistOff { get; set; }
            public bool HardpointsDeployed { get; set; }
            public bool InWing { get; set; }
            public bool LightsOn { get; set; }
            public bool CargoScoopDeployed { get; set; }
            public bool SilentRunning { get; set; }
            public bool ScoopingFuel { get; set; }
            public bool SrvHandbrake { get; set; }
            public bool SrvTurret { get; set; }
            public bool SrvUnderShip { get; set; }
            public bool SrvDriveAssist { get; set; }
            public bool FsdMassLocked { get; set; }
            public bool FsdCharging { get; set; }
            public bool FsdCooldown { get; set; }
            public bool LowFuel { get; set; }
            public bool Overheating { get; set; }
            public bool HasLatLong { get; set; }
            public bool IsInDanger { get; set; }
            public bool BeingInterdicted { get; set; }
            public bool InMainShip { get; set; }
            public bool InFighter { get; set; }
            public bool InSRV { get; set; }
            public bool HudInAnalysisMode { get; set; }
            public bool NightVision { get; set; }
            public bool AltitudeFromAverageRadius { get; set; }
            public bool FsdJump { get; set; }
            public bool SrvHighBeam { get; set; }

            public StatusFuel Fuel { get; set; } = new StatusFuel();

            public double FuelCapacity { get; set; }

            public double Cargo { get; set; }
            public string LegalState { get; set; }
            public double JumpRange { get; set; }

            public int Firegroup { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Altitude { get; set; }
            public double Heading { get; set; }
            public string BodyName { get; set; }
            public double PlanetRadius { get; set; }
            public StatusGuiFocus GuiFocus { get; set; }

            public int[] Pips { get; set; } = new int[3];

            public bool OnFoot { get; set; }
            public bool InTaxi { get; set; }
            public bool InMulticrew { get; set; }
            public bool OnFootInStation { get; set; }
            public bool OnFootOnPlanet { get; set; }
            public bool AimDownSight { get; set; }
            public bool LowOxygen { get; set; }
            public bool LowHealth { get; set; }
            public bool Cold { get; set; }
            public bool Hot { get; set; }
            public bool VeryCold { get; set; }
            public bool VeryHot { get; set; }
            public bool GlideMode { get; set; }
            public bool OnFootInHangar { get; set; }
            public bool OnFootSocialSpace { get; set; }
            public bool OnFootExterior { get; set; }
            public bool BreathableAtmosphere { get; set; }
            public bool TelepresenceMulticrew { get; set; }
            public bool PhysicalMulticrew { get; set; }
        }

        public static Status StatusData = new Status();

        public static void HandleStatusEvents(object sender, StatusFileEvent evt)
        {
            StatusData.ShieldsUp = (evt.Flags & StatusFlags.ShieldsUp) != 0;
            StatusData.FlightAssistOff = (evt.Flags & StatusFlags.FlightAssistOff) != 0;
            StatusData.InWing = (evt.Flags & StatusFlags.InWing) != 0;
            StatusData.LightsOn = (evt.Flags & StatusFlags.LightsOn) != 0;
            StatusData.NightVision = (evt.Flags & StatusFlags.NightVision) != 0;
            StatusData.AltitudeFromAverageRadius = (evt.Flags & StatusFlags.AltitudeFromAverageRadius) != 0;
            StatusData.LowFuel = (evt.Flags & StatusFlags.LowFuel) != 0;
            StatusData.Overheating = (evt.Flags & StatusFlags.Overheating) != 0;
            StatusData.HasLatLong = (evt.Flags & StatusFlags.HasLatLong) != 0;
            StatusData.InMainShip = (evt.Flags & StatusFlags.InMainShip) != 0;
            StatusData.InFighter = (evt.Flags & StatusFlags.InFighter) != 0;
            StatusData.InSRV = (evt.Flags & StatusFlags.InSRV) != 0;
            StatusData.SrvDriveAssist = (evt.Flags & StatusFlags.SrvDriveAssist) != 0 && StatusData.InSRV;
            StatusData.SrvUnderShip = (evt.Flags & StatusFlags.SrvUnderShip) != 0 && StatusData.InSRV;
            StatusData.SrvTurret = (evt.Flags & StatusFlags.SrvTurret) != 0 && StatusData.InSRV;
            StatusData.SrvHandbrake = (evt.Flags & StatusFlags.SrvHandbrake) != 0 && StatusData.InSRV;
            StatusData.SrvHighBeam = (evt.Flags & StatusFlags.SrvHighBeam) != 0 && StatusData.InSRV;

            StatusData.Docked = (evt.Flags & StatusFlags.Docked) != 0;
            StatusData.Landed = (evt.Flags & StatusFlags.Landed) != 0;
            StatusData.LandingGearDown = (evt.Flags & StatusFlags.LandingGearDown) != 0;
            StatusData.CargoScoopDeployed = (evt.Flags & StatusFlags.CargoScoopDeployed) != 0;
            StatusData.SilentRunning = (evt.Flags & StatusFlags.SilentRunning) != 0;
            StatusData.ScoopingFuel = (evt.Flags & StatusFlags.ScoopingFuel) != 0;
            StatusData.IsInDanger = (evt.Flags & StatusFlags.IsInDanger) != 0;
            StatusData.BeingInterdicted = (evt.Flags & StatusFlags.BeingInterdicted) != 0;
            StatusData.HudInAnalysisMode = (evt.Flags & StatusFlags.HudInAnalysisMode) != 0;

            StatusData.FsdMassLocked = (evt.Flags & StatusFlags.FsdMassLocked) != 0;
            StatusData.FsdCharging = (evt.Flags & StatusFlags.FsdCharging) != 0;
            StatusData.FsdCooldown = (evt.Flags & StatusFlags.FsdCooldown) != 0;

            StatusData.Supercruise = (evt.Flags & StatusFlags.Supercruise) != 0;
            StatusData.FsdJump = (evt.Flags & StatusFlags.FsdJump) != 0;
            StatusData.HardpointsDeployed = (evt.Flags & StatusFlags.HardpointsDeployed) != 0 && !StatusData.Supercruise && !StatusData.FsdJump;

            StatusData.Fuel = evt.Fuel ?? new StatusFuel();

            StatusData.Cargo = evt.Cargo;

            StatusData.LegalState = evt.LegalState;

            StatusData.Firegroup = evt.Firegroup;
            StatusData.GuiFocus = evt.GuiFocus;

            StatusData.Latitude = evt.Latitude;
            StatusData.Longitude = evt.Longitude;
            StatusData.Altitude = evt.Altitude;
            StatusData.Heading = evt.Heading;
            StatusData.BodyName = evt.BodyName;
            StatusData.PlanetRadius = evt.PlanetRadius;

            StatusData.Pips[0] = evt.Pips.System;
            StatusData.Pips[1] = evt.Pips.Engine;
            StatusData.Pips[2] = evt.Pips.Weapons;

            StatusData.OnFoot = (evt.Flags2 & MoreStatusFlags.OnFoot) != 0;
            StatusData.InTaxi = (evt.Flags2 & MoreStatusFlags.InTaxi) != 0;
            StatusData.InMulticrew = (evt.Flags2 & MoreStatusFlags.InMulticrew) != 0;
            StatusData.OnFootInStation = (evt.Flags2 & MoreStatusFlags.OnFootInStation) != 0;
            StatusData.OnFootOnPlanet = (evt.Flags2 & MoreStatusFlags.OnFootOnPlanet) != 0;
            StatusData.AimDownSight = (evt.Flags2 & MoreStatusFlags.AimDownSight) != 0;
            StatusData.LowOxygen = (evt.Flags2 & MoreStatusFlags.LowOxygen) != 0;
            StatusData.LowHealth = (evt.Flags2 & MoreStatusFlags.LowHealth) != 0;
            StatusData.Cold = (evt.Flags2 & MoreStatusFlags.Cold) != 0;
            StatusData.Hot = (evt.Flags2 & MoreStatusFlags.Hot) != 0;
            StatusData.VeryCold = (evt.Flags2 & MoreStatusFlags.VeryCold) != 0;
            StatusData.VeryHot = (evt.Flags2 & MoreStatusFlags.VeryHot) != 0;

            StatusData.GlideMode = (evt.Flags2 & MoreStatusFlags.GlideMode) != 0;
            StatusData.OnFootInHangar = (evt.Flags2 & MoreStatusFlags.OnFootInHangar) != 0;
            StatusData.OnFootSocialSpace = (evt.Flags2 & MoreStatusFlags.OnFootSocialSpace) != 0;
            StatusData.OnFootExterior = (evt.Flags2 & MoreStatusFlags.OnFootExterior) != 0;
            StatusData.BreathableAtmosphere = (evt.Flags2 & MoreStatusFlags.BreathableAtmosphere) != 0;

            StatusData.TelepresenceMulticrew = (evt.Flags2 & MoreStatusFlags.TelepresenceMulticrew) != 0;
            StatusData.PhysicalMulticrew = (evt.Flags2 & MoreStatusFlags.PhysicalMulticrew) != 0;
        }

    }

}