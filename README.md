# RussJudge.WPFControlsAndStylizer

This project is for primarily offering a ResourceDictionary for common stylizing of various WPF controls.  It also offers controls that are
not available through standard WPF functionality, such as an activity indicator.

This initial publish does not offer much, but more will be added as needed by the developer.

For an example of how to use this, see the example project at https://github.com/russjudge/RussJudge.WPFControlsAndStylizer



## Styles
### Colors
These colors can be overridden in Resources to change colors of choice.

| Color | Description |
| - | - |
| AttentionColor | Used on mouse-over on Window title bar on DialogWindowStyle and StandardWindowStyle.
| PrimaryColor | Used for the primary color style of all stylized controls.
| PrimaryBorderColor | Used for the primary border color style of all stylized controls.
| PrimaryTextColor | The primary foreground color.
| PrimaryInverseTextColor | The inverse primary color of the foreground.
| HighlightColor | Used as the center highlight color of the background of stylized controls.
| BorderHightlightColor | Used as the center highlight color of the background of the border of stylized controls.
| SecondaryColor | Used for the secondary color style of all stylized controls.
| SecondaryBorderColor | Used for the secondary color style for borders of all stylized controls.
| PrimaryControlColor | Primary background color of stylized controls.
| HighlightControlColor | Center highlight background color of stylized controls.
| SecondaryControlColor | Secondary background color of stylized controls.

### Brushes
These brushes can be overridden in Resources to change colors of choice.

| Brush | Description |
| - | - |
| PrimaryBorderBrush | Brush defined to PrimaryBorderColor.  Used as default Color2 in the ActivityIndicator.
| SecondaryBorderBrush | Brush defined to SecondaryBorderColor.  Used as default Color1 in the ActivityIndicator.
| PrimaryTextBrush | Brush defined to PrimaryTextColor, used on the titlebar of the stylized windows.
| PrimaryInverseTextBrush | Brush defined to PrimaryInverseTextColor, used on the titlebar of the stylized windows.
| ControlBackgroundStyle | Brush used for the background of controls (specifically ImportantButtonStyle).
| BackgroundStyle | Background style brush.
| BorderStyle |  Border Style Brush
| AttentionBrush | Brush using AttentionColor.
| ForegroundStyle | Foreground brush using PrimaryTextColor.

### Templates

#### DialogWindowStyle
Stylizes a window to behave like a dialog.  If you use the attached property Attached.TitleContent, the object defined in Attached.TitleContent will
be presented in the Title bar.

#### StandardWindowStyle
Stylizes a window.  If you use the attached property Attached.TitleContent, the object defined in Attached.TitleContent will
be presented in the Title bar.

#### ImportantButtonStyle
Stylizes a button to include animation on MouseOver.

#### TitleBarButtonStyle
Stylizes a button that is used in the TitleContent attached property.

## Controls

### ActivityIndicator
This control is merely a spinner to indicate activity.  By default, it is hidden, and the Visibility is bound to its property
"IsActive", which in turn is bound to the static property "IsActive" on RussJudge.WPFControlsAndStylizer.GlobalControl.Current.
Set the Visibility to make it visible, or set "IsActive" on the ActivityIndicator to make it visible,
or set RussJudge.WPFControlsAndStylizer.GlobalControl.Current.IsActive = true;

Using RussJudge.WPFControlsAndStylizer.GlobalControl.Current.IsActive allows for all ActivityIndicator controls on all open windows to
appear and be controlled from a single location.

There are three color properties that can be stylized:

| Property | Description |
| - | - |
| EllipseBackground | The background circle | Default is the style key "White".
| Color1 | The primary color | Default is the style key "PrimaryColor".
| Color2 | The secondary color | Default is the style key "SecondaryColor".

By default, the ActivityIndicator is a circle.  However, by setting the Height and Width differently, an elllipse is formed that will create a differently
behaving activity indicator.


## Attached Properties

### IsStylized
Set to true only if using the DialogWindowStyle or StandardWindowStyle.  Setting this property causes the System Commands (Maximize, Minimize, Restore, Close)
to be properly bound to the Window Titlebar Buttons.  These buttons won't function without setting this attached property.  Setting this is NOT required if
TitleContent is set.

### TitleContent
Content that will display on the Window Titlebar left of the Title on a Window using the DialogWindowStyle or StandardWindowStyle.

### Placeholder
Setting this attached property on a TextBox will display the Placeholder over the empty area of the TextBox. 
This Placeholder disappears when focus is on the TextBox, or if the TextBox.Text has a value.



