# joystick-feedback

This tool plays a user configurable sound, when entering and exiting the deadzone of multiple joystick's X,Y,Z axis.

This is for use with my VKB kosmosima joystick with omni-throttle adapter on a gunfighter base, that has the y-axis spring removed.
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
  "PID": "0127",
  "VID": "231D",

  "Y": {
    "Deadband": 1500,
    "InDeadzoneSoundFile": "63528__florian-reinke__button-off.wav",
    "OutDeadzoneSoundFile": "beep-3.wav"
  } 

}
```

You can also, optionally, add the same data for X and Z axes.

This is a useful tool, to find out the joystick VID, PID (this tool is not limited to VKB joysticks):

https://vkbcontrollers.com/wp-content/uploads/2019/02/VKB_JoyTester.zip

![vkb_joytester](https://i.imgur.com/GqW3RHk.pnghttps://i.imgur.com/GqW3RHk.png)

The deadzone lies between 32767 - Deadband and 32767 + Deadband

The sounds can be changed or disabled by editing the 'InDeadzoneSoundFile' and 'OutDeadzoneSoundFile' fields

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


