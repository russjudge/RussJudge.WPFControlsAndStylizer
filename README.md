# RussJudge.WPFControlsAndStylizer

This project is for primarily offering a ResourceDictionary for common stylizing of various WPF controls.  It also offers controls that are
not available through standard WPF functionality, such as an activity indicator.

This initial publish does not offer much, but more will be added as needed by the developer.

For an example of how to use this, see the example project at https://github.com/russjudge/RussJudge.WPFControlsAndStylizer

## Supported frameworks

| Framework | Versions |
| - | - |
| .NET | 5, 6, 7, 8, 9, 10, 11 (Windows) |
| .NET Framework | 4.5, 4.5.1, 4.5.2, 4.6, 4.6.1, 4.6.2, 4.7, 4.7.1, 4.7.2, 4.8, 4.8.1 |

The package has no dependencies on other packages.

## Getting started

Merge the resource dictionary into your application's resources (App.xaml):

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/RussJudge.WPFControlsAndStylizer;component/StylizerDictionary.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

Then reference the styles by key, for example `Style="{StaticResource StandardWindowStyle}"` on a Window.

## What's new in 1.2.0

- Now builds for every framework listed above (previously .NET 8 and 9 only).
- Maximizing a window that uses StandardWindowStyle no longer covers the Taskbar or pushes the edges of the window off
  screen, whichever way the window is maximized and on whichever monitor.  The style is no longer removed and re-applied
  to achieve this, so the content of the window is no longer unloaded and reloaded, and a window can no longer end up
  with the wrong style (or none) after being maximized.
- DialogWindowStyle and StandardWindowStyle now work with `SizeToContent`.
- The title bar buttons work without setting `Attached.IsStylized`; the styles take care of it.
- The system menu opens in the right place when the window is maximized or on another monitor.
- A TextBox with a Placeholder returns to its normal background after being disabled or read-only, and no longer
  throws when it is disabled while its Background is not a SolidColorBrush.
- `About.DisplayAbout` now shows the window when an assembly is passed in (it was ignored before).
- `About` and `InputPromptDialog` work even if the application has not merged StylizerDictionary.xaml.
- `InputPromptDialog` sizes itself to its content, and opens with the focus in the text box and the default text selected.
- `TaskMonitor.Current` and `GlobalControl.Current` can be used first from any thread, `GlobalControl.Current.IsActive`
  can be set from any thread, and `TaskMonitor.Cancel` no longer throws for a task that has already finished.
- A wide ActivityIndicator now draws its arcs on the ring.  An ActivityIndicator only animates while it is visible.
- Faster: the Placeholder template is loaded once instead of once per TextBox, and the ActivityIndicator updates its
  shapes instead of rebuilding them on every size or brush change.
- The package no longer copies rjicon.ico, rjicon2.png and window.png into projects that reference it.


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
| BorderHighlightColor | Used as the center highlight color of the background of the border of stylized controls.
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

When maximized, the window fills the work area of its monitor exactly: the Taskbar stays visible and nothing is cut off.

#### ImportantButton
Stylizes a button to include animation on MouseOver.  The resource key is `ImportantButton`.

#### TitleBarButtonStyle
Stylizes a button that is used in the TitleContent attached property.

## Controls

### ActivityIndicator
This control is merely a spinner to indicate activity.  By default, it is hidden, and the Visibility is bound to its property
"IsActive", which in turn is bound to the static property "IsActive" on RussJudge.WPFControlsAndStylizer.GlobalControl.Current.
Set the Visibility to make it visible, or set "IsActive" on the ActivityIndicator to make it visible,
or set RussJudge.WPFControlsAndStylizer.GlobalControl.Current.IsActive = true;

Using RussJudge.WPFControlsAndStylizer.GlobalControl.Current.IsActive allows for all ActivityIndicator controls on all open windows to
appear and be controlled from a single location.  It can be set from any thread, so a background task can switch it on and off itself.

There are three color properties that can be stylized:

| Property | Description | Default |
| - | - | - |
| EllipseBackground | The background circle | The style key "ControlBackgroundStyle".
| Color1 | The primary color | The style key "SecondaryBorderBrush".
| Color2 | The secondary color | The style key "PrimaryBorderBrush".

By default, the ActivityIndicator is a circle.  However, by setting the Height and Width differently, an elllipse is formed that will create a differently
behaving activity indicator.

### About
This is a standard "About" window, stylized with the "DialogWindowStyle" that displays properties from any given Assembly.  The default
Assembly is the Entry Assembly.  
Open the About window with:

```
var optionalAssembly = System.Reflection.Assembly.GetEntryAssembly();

About.DisplayAbout("optional informational string", optionalAssembly);
```

### InputPromptDialog
This is a simplified dialog window for providing a prompt to accept a string from the user.
```
string? result = InputPromptDialog.ShowDialog("Enter some text:", "Window title", "optional default value");
if (result == null)
{
	//User selected "Cancel"
}
else
{
	//User entered a string for processing.
	string value = result;
}
```

### TaskMonitor
This Singleton provides a mechanism for starting and monitoring background tasks, with the capability to cancel.

Tasks can be added by calling the AddTask method.  The first parameter is a delegate that must accept a CancellationToken as its
only parameter.  This token can be ignored within the delegate, but can be used to allow controlled cancellation of the Task.
The second parameter of AddTask is a simple description. 

TaskMonitor.Current.BackgroundProcesses is a bindable collection of running tasks.

Cancel a single task with TaskMonitor.Cancel(item), or all of them with TaskMonitor.CancelAllTasks().  Cancelling a task that has
already finished does nothing.

## Attached Properties

### IsStylized
Causes the System Commands (Maximize, Minimize, Restore, Close) to be properly bound to the Window Titlebar Buttons, and keeps the
window clear of the Taskbar when it is maximized.

DialogWindowStyle and StandardWindowStyle set this property themselves, so it no longer needs to be set on each window.  Setting it
explicitly, as earlier versions required, still works.

### TitleContent
Content that will display on the Window Titlebar left of the Title on a Window using the DialogWindowStyle or StandardWindowStyle.

### Placeholder
Setting this attached property on a TextBox will display the Placeholder over the empty area of the TextBox. 
This Placeholder disappears when focus is on the TextBox, or if the TextBox.Text has a value.



