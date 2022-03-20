# joystick-feedback

Play a user configurable sound when entering and exiting the deadzone of a joystick's y-axis

![full setup](https://i.imgur.com/Mw0T5n6.jpg)

![joystick](https://i.imgur.com/DiYBuDx.jpg)

The (virtual) joystick is configured via joysticksettings.config

```
<?xml version="1.0" encoding="utf-8" ?>
<joystickSettings>
  <add key="PID" value="0126" />
  <add key="VID" value="231D" />
  <add key="Deadband" value="1500" />
</joystickSettings>
```

This is a useful tool, to find out the joystick VID, PID (this tool is not limited to VKB joysticks):

https://vkbcontrollers.com/wp-content/uploads/2019/02/VKB_JoyTester.zip

![vkb_joytester](https://i.imgur.com/GqW3RHk.pnghttps://i.imgur.com/GqW3RHk.png)

The deadband lies between 32767 - Deadband and 327657 + Deadband

This sound can be changed or disabled by editing the 'InYDeadzoneSound' and 'OutYDeadzoneSound' key in in appsettings.config

some example sounds can be found in the Sounds directory.
