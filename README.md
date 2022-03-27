# joystick-feedback

This tool plays a user configurable sound, when entering and exiting the deadzone of multiple joystick's X,Y,Z axis.
You can also add sounds to joystick buttons.

Also, for Elite Dangerous only, you can execute macro keys after pressing a joystick button. 
This will only work if that joystick button is not already used in the game. 
When there is only a binding to a joystick / controller / mouse for a particular macro function, then you need to add a keyboard binding.
All the configured key bindings from the game are automatically used and also game state feedback is used. 

(This kind of macro functionality is also possible using a streamdeck or voice attack.)

The audible cue is for use with my VKB kosmosima joystick with omni-throttle adapter on a gunfighter base, that has the y-axis spring removed.
This joystick is now used as a throttle with 3 degrees of freedom.

I changed the cams to 'space soft center' and changed to #10 springs.

The red led turns on when the y-axis is in the center (deadzone).

I increased the y-axis deadzone to 5%.

I removed the y-axis spring and tightened the dry clutch.
You will need to tighten the clutch a bit, because otherwise the rubber dust cover will pull back the stick when you let it go. 

By removing the spring, you won't feel the center anymore. 
There is no mechanical center detent.
That's what the LED and increased deadzone is for.

In addition, this tool adds an audible cue, that you have entered or exited the y-axis deadzone.

You don't have to remove the y-axis spring, if you, for example, turn flight assist mode off in Elite Dangerous or use cruise control in star citizen.
But, I like to have more of a conventional throttle feel..

![full setup](https://i.imgur.com/Mw0T5n6.jpg)

![joystick](https://i.imgur.com/DiYBuDx.jpg)

Each (virtual) joystick is configured via a separate .json file in the Data\Joysticks directory.

```
{
  "PID": "0126",
  "VID": "231D",

  "Y": {
    "Deadband": 1500,
    "InDeadzoneSoundFile": "63528__florian-reinke__button-off.wav",
    "OutDeadzoneSoundFile": "beep-3.wav"
  },

  "Buttons": {
    "4": {
      "SoundFile": "63528__florian-reinke__button-off.wav",
      // turn lights on then off again
      "EDActions": [
        "ShipSpotLightToggle-ON",
        "2000",
        "ShipSpotLightToggle-OFF"
      ]
    },
    "5": {
      "SoundFile": "63528__florian-reinke__button-off.wav",
      //trigger discovery scanner bound to secondary fire button on fire group D
      "EDActions": [
        "DeployHardpointToggle-ON",
        "PlayerHUDModeToggle-AM",
        "FireGroup-D",
        "SecondaryFire-DOWN",
        "10000",
        "SecondaryFire-UP",
        "FireGroup-A",
        "PlayerHUDModeToggle-CM",
        "3000"
      ]
    }
  }
}
```

You can also, optionally, add the deadband and sound files for X and Z axes and add sounds to joystick buttons.

This is a useful tool, to find out the joystick VID, PID and button numbers (this tool is not limited to VKB joysticks):

https://vkbcontrollers.com/wp-content/uploads/2019/02/VKB_JoyTester.zip

![vkb_joytester](https://i.imgur.com/GqW3RHk.pnghttps://i.imgur.com/GqW3RHk.png)

The deadzone lies between 32767 - Deadband and 32767 + Deadband

The sounds can be changed or disabled by editing the various SoundFile fields

Some example sounds can be found in the Sounds directory.

# Changes to my VKB kosmosima on a gunfighter base

Get VKBDevCfg-C.exe from the vkb site.

https://vkbcontrollers.com/?page_id=4609

There is a manual controller_2_15_En.pdf for the VKBDevCfg-C application.

Edit zconfig.ini and add User=Developer to the [User] section.

Start VKBDevCfg-C and go to Global\External\LEDs and add a new entry for the Axis in center event.

My setup :

![setup1](https://i.imgur.com/K4zZG9U.png)

I increased the y-axis deadband from 0.5% to 5% on the profile\axes\logical axes screen. (You need to uncheck the 2D checkbox)

My setup :

![setup2](https://i.imgur.com/W1ZH353.png)

# Elite Dangerous joystick macro actions

**When using Odyssey, the on-foot binding must be 'custom'. (this will happen automatically once you make at least one on-foot keyboard binding)
If you see a default binding name, then the plugin won't work correctly.**

If nothing happens, when executing macros:

You may see errors like this in the plugin log file :

`file not found C:\Users\xxx\AppData\Local\Frontier Developments\Elite Dangerous\Options\Bindings\Custom.4.0.binds`

In that case, the application has no access to the bindings directory. Start the application as administrator.

All keyboard bindings in the .bindings file are available :

`%LocalAppData%\Frontier Developments\Elite Dangerous\Options\Bindings\`

If the macro function is a number, then there will be a delay (the unit is milliseconds)

The following macro functions use the game state. 
They should not be executed multiple times in quick succession, because it takes some time for the application to receive the game state change.

| Function                | Toggles         |
| ----------------------- | --------------- |
| PlayerHUDModeToggle-AM | Analysis Mode |
| PlayerHUDModeToggle-CM | Combat Mode |
| ToggleCargoScoop-ON | Cargo Scoop On |
| ToggleCargoScoop-OFF | Cargo Scoop Off |
| ToggleFlightAssist-ON | Flight Assist On |
| ToggleFlightAssist-OFF | Flight Assist Off |
| DeployHardpointToggle-ON | Deploy Hardpoints |
| DeployHardpointToggle-OFF | Retract Hardpoints |
| LandingGearToggle-ON | Deploy Landing Gear |
| LandingGearToggle-OFF | Retract Landing Gear |
| ShipSpotLightToggle-ON | Lights On |
| ShipSpotLightToggle-OFF | Lights Off |
| NightVisionToggle-ON | Night Vision On |
| NightVisionToggle-OFF | Night Vision Off |
| ToggleButtonUpInput-ON | Silent Running On |
| ToggleButtonUpInput-OFF | Silent Running Off |

| Function                | Fire Groups     |
| ----------------------- | --------------- |
| FireGroup-A | Fire Group A |
| FireGroup-B | Fire Group B |
| FireGroup-C | Fire Group C |
| FireGroup-D | Fire Group D |
| FireGroup-E | Fire Group E |
| FireGroup-F | Fire Group F |
| FireGroup-G | Fire Group G |
| FireGroup-H | Fire Group H |

These macro function allow the fire buttons to be 'pressed' for a longer time. 

| Function                | Fire            |
| ----------------------- | --------------- |
| PrimaryFire-DOWN | Primary Fire Press |
| PrimaryFire-UP | Primary Fire Release |
| SecondaryFire-DOWN | Secondary Fire Press |
| SecondaryFire-UP | Seconday Fire Release |

These are regular key bindings. 
You can find more in the .binding file, that are available to be used as macro functions.
Only a few have been tested, some may not work. 
Especially the ones that require a key to be pressed for a longer time.

| Function                | Combat     |
| ----------------------- | --------------- |
| ChargeECM | ECM |
| CycleFireGroupNext | Next Fire Group |
| CycleFireGroupPrevious | Prev Fire Group |
| CycleNextHostileTarget | Next Hostile |
| CyclePreviousHostileTarget | Prev Hostile |
| CycleNextSubsystem | Next Subsystem |
| CyclePreviousSubsystem | Prev Subsystem |
| CycleNextTarget | Next Contact |
| CyclePreviousTarget | Prev Contact |
| SelectTarget | Target Ahead |

| Function                | Fighter     |
| ----------------------- | --------------- |
| OpenOrders | Crew Orders |
| OrderAggressiveBehaviour | Be Aggressive |
| OrderDefensiveBehaviour | Be Defensive |
| OrderFocusTarget | Attack My Target |
| OrderFollow | Follow |
| OrderHoldFire | Hold Fire |
| OrderHoldPosition | Hold Position |
| OrderRequestDock | Dock SLF |

| Function                | Misc     |
| ----------------------- | --------------- |
| FriendsMenu | Friends |
| GalnetAudio_ClearQueue | Clear Audio Queue |
| GalnetAudio_Play_Pause | Play/Pause Audio |
| GalnetAudio_SkipBackward | Prev Audio Track |
| GalnetAudio_SkipForward | Next Audio Track |
| MicrophoneMute | Microphone |
| OpenCodexGoToDiscovery | Codex |
| Pause | Main Menu |
| HMDReset | Reset HMD |
| OculusReset | Reset Oculus |
| RadarDecreaseRange | Dec Sensor Range |
| RadarIncreaseRange | Inc Sensor Range |

| Function                | Navigation     |
| ----------------------- | --------------- |
| SetSpeed100 | 100% Throttle |
| SetSpeed75 | 75% Throttle |
| SetSpeed50 | 50% Throttle |
| SetSpeed25 | 25% Throttle |
| SetSpeedMinus100 | 100% Reverse |
| SetSpeedMinus75 | 75% Reverse |
| SetSpeedMinus50 | 50% Reverse |
| SetSpeedMinus25 | 25% Reverse |
| SetSpeedZero | All Stop |
| TargetNextRouteSystem | Next Jump Dest |
| ToggleReverseThrottleInput | Reverse |
| UseAlternateFlightValuesToggle | Alternate Controls |
| UseBoostJuice | Boost |
| DisableRotationCorrectToggle | Rotational Correction |

| Function                | Ship     |
| ----------------------- | --------------- |
| QuickCommsPanel | Quick Comms |
| EjectAllCargo | Eject All Cargo |
| HeadLookToggle | Toggle Headlook |
| MouseReset | Reset Mouse |
| OrbitLinesToggle | Orbit Lines |
| SelectTargetsTarget | Wingman's target |
| WingNavLock | Wingman Navlock |
| TargetWingman0 | Wingman 1 |
| TargetWingman1 | Wingman 2 |
| TargetWingman2 | Wingman 3 |
| EngineColourToggle | Engine Colour |
| WeaponColourToggle | Weapon Colour |

| Function                | SRV     |
| ----------------------- | --------------- |
| RecallDismissShip | Recall/Dismiss Ship |
| BuggyToggleReverseThrottleInput | Reverse |
| DecreaseSpeedButtonMax | Zero Speed |
| IncreaseSpeedButtonMax | Maximum Speed |
| SelectTarget_Buggy | Target Ahead |

| Function                | On Foot     |
| ----------------------- | --------------- |
| HumanoidPrimaryInteractButton | Primary Interact |
| HumanoidSecondaryInteractButton | Secondary Interact |
| HumanoidSelectPrimaryWeaponButton | Select Primary Weapon |
| HumanoidSelectSecondaryWeaponButton | Select Secondary Weapon |
| HumanoidHideWeaponButton | Holster Weapon |
| HumanoidSwitchWeapon | Switch Weapon |
| HumanoidSelectUtilityWeaponButton | Select Tool |
| HumanoidSelectNextWeaponButton | Next Weapon |
| HumanoidSelectPreviousWeaponButton | Previous Weapon |
| HumanoidReloadButton | Reload |
| HumanoidSelectNextGrenadeTypeButton | Next Grenade Type |
| HumanoidSelectPreviousGrenadeTypeButton | Previous Grenade Type |
| HumanoidThrowGrenadeButton | Throw Grenade |
| HumanoidMeleeButton | Melee |
| HumanoidSwitchToRechargeTool | Energy Link |
| HumanoidSwitchToCompAnalyser | Profile Analyser |
| HumanoidToggleToolModeButton | Tool Mode |
| HumanoidSwitchToSuitTool | Suit Specific Tool |
| QuickCommsPanel_Humanoid | Quick Comms |
| HumanoidConflictContextualUIButton | Conflict Zone Battle Stats |
| HumanoidToggleShieldsButton | Shields |
| HumanoidToggleMissionHelpPanelButton | Help |
| HumanoidClearAuthorityLevel | Clear Stolen Profile |
| HumanoidHealthPack | Use Medkit |
| HumanoidBattery | Use Energy Cells |
| HumanoidSelectFragGrenade | Select Frag Grenade |
| HumanoidSelectEMPGrenade | Select EMP Grenade |
| HumanoidSelectShieldGrenade | Select Shield Grenade |

| Function                | UI     |
| ----------------------- | --------------- |
| CycleNextPage | Next Page |
| CycleNextPanel | Next Panel |
| CyclePreviousPage | Prev Page |
| CyclePreviousPanel | Prev Panel |
| UI_Back | UI Back |
| UI_Down | UI Down |
| UI_Left | UI Left |
| UI_Right | UI Right |
| UI_Select | UI Select |
| UI_Toggle | UI Toggle |
| UI_Up | UI Up |
| PrimaryFire | Primary Fire |
| SecondaryFire | Secondary Fire |

| Function                | Ship     |
| ----------------------- | --------------- |
| ShowPGScoreSummaryInput | CQC Score |
| UIFocus | UI Focus |

| Function                | On Foot     |
| ----------------------- | --------------- |
| HumanoidForwardButton | Forward |
| HumanoidBackwardButton | Backward |
| HumanoidStrafeLeftButton | Strafe Left |
| HumanoidStrafeRightButton | Strafe Righ |
| HumanoidRotateLeftButton | Rotate Left |
| HumanoidRotateRightButton | Rotate Right |
| HumanoidPitchUpButton | Pitch Up |
| HumanoidPitchDownButton | Pitch Down |
| HumanoidSprintButton | Sprint |
| HumanoidCrouchButton | Crouch |
| HumanoidJumpButton | Jump |
| HumanoidItemWheelButton | Item Wheel |
| HumanoidEmoteWheelButton | Emote Wheel |
| HumanoidUtilityWheelCycleMode | Cycle Wheel Mode |
| HumanoidPrimaryFireButton | Primary Fire |

| Function                | Camera     |
| ----------------------- | --------------- |
| FixCameraRelativeToggle | Lock to Vehicle |
| FixCameraWorldToggle | Lock to World |
| FocusDistanceDec | Focus Nearer |
| FocusDistanceInc | Focus Further |
| FreeCamSpeedDec | Dec Cam Speed |
| FreeCamSpeedInc | Inc Cam Speed |
| FreeCamToggleHUD | Toggle HUD |
| FreeCamZoomIn | Zoom In |
| FreeCamZoomOut | Zoom Out |
| FStopDec | Dec Blur |
| FStopInc | Inc Blur |
| MoveFreeCamBackwards | Cam Backwards |
| MoveFreeCamDown | Cam Down |
| MoveFreeCamForward | Cam Forwards |
| MoveFreeCamLeft | Cam Left |
| MoveFreeCamRight | Cam Right |
| MoveFreeCamUp | Cam Up |
| PhotoCameraToggle | External Cam |
| PhotoCameraToggle_Buggy | External Cam Buggy |
| PhotoCameraToggle_Humanoid | External Cam Cmdr |
| PitchCameraDown | Cam Pitch Down |
| PitchCameraUp | Cam Pitch Up |
| QuitCamera | Exit free Cam |
| RollCameraLeft | Cam Roll Left |
| RollCameraRight | Cam Roll Right |
| StoreCamZoomIn | Store Cam Zoom In |
| StoreCamZoomOut | Store Cam Zoom Out |
| StoreEnableRotation | Store Cam Rotation |
| StorePitchCameraDown | Store Cam Pitch Down |
| StorePitchCameraUp | Store Cam Pitch Up |
| StoreToggle | Store Toggle Preview |
| StoreYawCameraLeft | Store Cam Yaw Left |
| StoreYawCameraRight | Store Cam Yaw Right |
| ToggleAdvanceMode | Advanced Cam |
| ToggleFreeCam | Free Cam |
| ToggleReverseThrottleInputFreeCam | Cam Reverse |
| ToggleRotationLock | Cam Rotation Lock |
| VanityCameraEight | Cam - Back |
| VanityCameraFive | Cam - Co-Pilot 1 |
| VanityCameraFour | Cam - Commander 2 |
| VanityCameraNine | Cam - Back |
| VanityCameraTen | Cam - Back Low |
| VanityCameraOne | Cam - Cockpit Front |
| VanityCameraScrollLeft | Prev Cam |
| VanityCameraScrollRight | Next Cam |
| VanityCameraSeven | Cam - Front |
| VanityCameraSix | Cam - Co-Pilot 2 |
| VanityCameraThree | Cam - Commander 1 |
| VanityCameraTwo | Cam - Cockpit Back |
| YawCameraLeft | Cam Yaw Left |
| YawCameraRight | Cam Yaw Right |

| Function                | Galaxy map     |
| ----------------------- | --------------- |
| CamPitchDown | GalMap Pitch Down |
| CamPitchUp | GalMap Pitch Up |
| CamTranslateDown | GalMap Down |
| CamTranslateUp | GalMap Up |
| CamTranslateZHold | GalMap Z Hold |
| CamYawLeft | GalMap Yaw Left |
| CamYawRight | GalMap Yaw Right |
| CamZoomIn | GalMap Zoom In |
| CamZoomOut | GalMap Zoom Out |

| Function                | Head look     |
| ----------------------- | --------------- | 
| HeadLookPitchDown | Look Down |
| HeadLookYawLeft | Look Left |
| HeadLookYawRight | Look Right |
| HeadLookPitchUp | Look Up |
| HeadLookReset | Reset Headlook |

| Function                | Holo-Me     |
| ----------------------- | --------------- |
| CommanderCreator_Redo | Redo Holo-Me |
| CommanderCreator_Rotation | Rotate Holo-Me |
| CommanderCreator_Rotation_MouseToggle | Toggle Holo-Me Rotation |
| CommanderCreator_Undo | Undo Holo-Me |

| Function                | Multicrew     |
| ----------------------- | --------------- |
| MultiCrewCockpitUICycleBackward | UI Backward |
| MultiCrewCockpitUICycleForward | UI Forward |
| MultiCrewPrimaryFire | Fire 1 |
| MultiCrewPrimaryUtilityFire | Primary Utility |
| MultiCrewSecondaryFire | Fire 2 |
| MultiCrewSecondaryUtilityFire | Secondary Utility |
| MultiCrewThirdPersonFovInButton | Field of View In |
| MultiCrewThirdPersonFovOutButton | Field of View Out |
| MultiCrewThirdPersonPitchDownButton | Pitch Down |
| MultiCrewThirdPersonPitchUpButton | Pitch Up |
| MultiCrewThirdPersonYawLeftButton | Yaw Left |
| MultiCrewThirdPersonYawRightButton | Yaw Right |
| MultiCrewToggleMode | Multicrew Mode |

| Function                | Scanners     |
| ----------------------- | --------------- |
| ExplorationFSSCameraPitchDecreaseButton | FSS Pitch Down |
| ExplorationFSSCameraPitchIncreaseButton | FSS Pitch Up |
| ExplorationFSSCameraYawDecreaseButton | FSS Yaw Left |
| ExplorationFSSCameraYawIncreaseButton | FSS Yaw Right |
| ExplorationFSSDiscoveryScan | FSS Honk |
| ExplorationFSSMiniZoomIn | Step Zoom FSS In |
| ExplorationFSSMiniZoomOut | Step Zoom FSS Out |
| ExplorationFSSRadioTuningX_Decrease | FSS Tune Left |
| ExplorationFSSRadioTuningX_Increase | FSS Tune Right |
| ExplorationFSSShowHelp | FSS Help |
| ExplorationFSSTarget | Target FSS |
| ExplorationFSSZoomIn | Zoom FSS In |
| ExplorationFSSZoomOut | Zoom FSS Out |
| ExplorationSAAChangeScannedAreaViewToggle | Toggle Planet Front/Back |
| ExplorationSAAExitThirdPerson | Exit DSS |
| ExplorationSAANextGenus | DSS Next Genus |
| ExplorationSAAPreviousGenus | DSS Previous Genus |
| SAAThirdPersonFovInButton | DSS Field of View In |
| SAAThirdPersonFovOutButton | DSS Field of View Out |
| SAAThirdPersonPitchDownButton | Pitch Down (DSS) |
| SAAThirdPersonPitchUpButton | Pitch Up (DSS) |
| SAAThirdPersonYawLeftButton | Yaw Left (DSS) |
| SAAThirdPersonYawRightButton | Yaw Right (DSS) |

| Function                | SRV     |
| ----------------------- | --------------- |
| BuggyPitchDownButton | Pitch Down |
| BuggyPitchUpButton | Pitch Up |
| BuggyPrimaryFireButton | Primary Weapons |
| BuggySecondaryFireButton | Secondary Weapons |
| BuggyRollLeft | Roll Left |
| BuggyRollLeftButton | Roll Left |
| BuggyRollRight | Roll Right |
| BuggyRollRightButton | Roll Right |
| BuggyTurretPitchDownButton | Turret Down |
| BuggyTurretPitchUpButton | Turret Up |
| BuggyTurretYawLeftButton | Turret Left |
| BuggyTurretYawRightButton | Turret Right |
| EjectAllCargo_Buggy | Eject All Cargo |
| FocusCommsPanel_Buggy | Comms Panel |
| FocusLeftPanel_Buggy | Nav Panel |
| FocusRadarPanel_Buggy | Role Panel |
| FocusRightPanel_Buggy | Systems Panel |
| HeadLookToggle_Buggy | Toggle Headlook |
| IncreaseSpeedButtonPartial | Inc Speed |
| DecreaseSpeedButtonPartial | Dec Speed |
| QuickCommsPanel_Buggy | Quick Comms |
| SteerLeftButton | Steer Left |
| SteerRightButton | Steer Right |
| UIFocus_Buggy | UI Focus |
| VerticalThrustersButton | Vertical Thrusters |



