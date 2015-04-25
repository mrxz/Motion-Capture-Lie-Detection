# Instruction manual


## Getting started with a session

We offer two ways to analyze data. You can either stream realtime data from
MVNX Studio or use a prerecorded MVNX or Binary Mocap file.

To connect to a live recording session make sure MVNX Studio is running and
configured.  Then You can start the session by going to `File` and then click
`Start listening`, or simply press `Ctrl+Shift+O`.

To open a prerecorded file go to `File` and select `Open recording file` or 
press `Ctrl+O`.


## Visualizer
The data should be playing now. And you'll see a 3d character drawn in the
middle of the screen. You can move the character around by holding the right
mouse button and rotate the character by holding the left mouse button. You 
can zoom in and out using the scrollwheel.

On the left of the screen you see the traffic light. The traffic light will
light green when someone is telling the truth, red when someone is lying and
orange if the algorithm isn't sure.

On the left you'll also see a graph that will plot the movement of each recorded
limb.



## Timeline
At the bottom of the screen we see the timeline. The red line is the cursor.
It tells you where in the recording you currently are.  You can drag the
cursor around too to go to certain parts in the recording.

If you click and hold the right mouse button in the timeline you can select
an area.  This area will then turn dark blue.

Now if you hit the `loop` checkbox the red recording will loop over the selected
area.

Furthermore, the lie detection algorithm will only process the data selected
this way.  (If nothing is selected, the whole timeline is used).  This is useful
to select certain answers of questions to check if a certain answer was a lie.

## Markpoints

On the right of the screen we have the Markpoints control. Markpoints are
bookmarks in the timeline that allow for easy navigation.

By clicking `Add` you can add new markpoints in the timeline. They will
show up as orange circles in the timeline.

By either clicking on a markpoint circle or by clicking on the markpoint in
the markpoint list you navigate to that markpoint.

By rightclicking on a markpoint circle and dragging the mouse to another
markpoint circle you can select the exact timeline area between those markpoints.



## Controls
Below the 3d visualization you'll see standard playback controls as seen on a
DVD player. `|<<` and `>>|` will go to the beginning and the end of the timeline.
`|| / >` will play/pause.   `<<` goes nagivates to the previous markpoint.
`>>` navigates to the next markpoint.


## Saving
You can save your session using `Ctrl+S` or by navigating the menu `File->Save recording`.
In the future opening a saved session will restore your previous markpoints.

## Closing

You close a session (either a stream or a recording) by going to `File->Close recording` or by pressing `Ctrl+W`.

Starting a new recording or session automatically closes the previous one.




