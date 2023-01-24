# Monobanda-BallVR-source

## Debug options

![Screenshot (69)](https://user-images.githubusercontent.com/57196554/214278961-f18b5a12-539b-4dc3-8e26-7ebf7649144d.png)

![Screenshot (70)](https://user-images.githubusercontent.com/57196554/214278911-20397524-d29b-4293-bd1b-a76f9b8fe3ef.png)

- 1 When turned on the player will recieve keyboard input instead of audio input.
- 2 makes the player invulnerable.
- 3 turns the game in android mode and makes it ready to be build for android.

A dropdown menu with debug stats can be opened ingame by pressing Q.

## Android studio

![Screenshot (76)](https://user-images.githubusercontent.com/57196554/214278814-0b7633eb-31f2-4d68-aa1f-4c07b946a5d0.png)

- 1	SDK Manager
- 2	API Level

When opening Android studio under the SDK Manager you will see 3 options: SDK Platforms, SDK Tool and SDK Updates Site.
Under SDK Platforms you should check the box with API level 30.

![Screenshot (78)](https://user-images.githubusercontent.com/57196554/214278774-f342e184-a683-46e4-b10b-eb1c4795fa9e.png)

Under SDK Tools you should check the box with name/version 23.0.1

![Screenshot (80)](https://user-images.githubusercontent.com/57196554/214278675-a0f8dd4f-9192-492e-8a23-78a8e0f50805.png)

If for any reason you want to change the versions of those above, you should edit the 'mainTemplate.cradle' and 'launcherTemplate.cradle' like the image above to match the downloaded versions.
