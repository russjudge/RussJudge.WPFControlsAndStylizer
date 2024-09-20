# RussJudge.WPFControlsAndStylizer

This project is for primarily offering a ResourceDictionary for common stylizing of various WPF controls.  It also offers controls that are
not available through standard WPF functionality, such as an activity indicator.

This initial publish does not offer much, but more will be added as needed by the developer.

For an example of how to use this, see the example project at https://github.com/russjudge/RussJudge.WPFControlsAndStylizer



## Styles
| PrimaryColor | Used for the primary color style of all stylized controls.  Default is Maroon.
| SecondaryColor | Used for the secondary color style of all stylized controls.  Default is Red.
| White | The stylized color to identify as "White".


## Controls

### ActivityIndicatory
This control is merely a spinner to indicate activity.  By default, it is hidden, and the Visibility is bound to its property
"IsActive", which in turn is bound to the static property "IsActive" on RussJudge.WPFControlsAndStylizer.GlobalControl.Current.
Set the Visibility to make it visible, or set "IsActive" on the ActivityIndicator to make it visible,
or set RussJudge.WPFControlsAndStylizer.GlobalControl.Current.IsActive = true;

Using RussJudge.WPFControlsAndStylizer.GlobalControl.Current.IsActive allows for all ActivityIndicator controls on all open windows to
appear and be controlled from a single location.

There are three color properties that can be stylized:

| EllipseBackground | The background circle | Default is the style key "White".
| Color1 | The primary color | Default is the style key "PrimaryColor".
| Color2 | The secondary color | Default is the style key "SecondaryColor".

