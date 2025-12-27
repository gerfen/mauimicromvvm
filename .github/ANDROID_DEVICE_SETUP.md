# Android Device Setup for MauiMicroSample

## Environment Variables Configured

The following environment variables have been set to enable Android device detection:

- **ANDROID_HOME**: `C:\Program Files (x86)\Android\android-sdk`
- **PATH**: Added `C:\Program Files (x86)\Android\android-sdk\platform-tools`

## Next Steps

### 1. **Restart Visual Studio**
   Close and reopen Visual Studio for the environment variable changes to take effect.

### 2. **Verify Device Connection**
   After restarting Visual Studio, you should see your Android device in the device dropdown.
   
   Your connected device:
   - Device ID: `47031FDAS0042Q`
   - Status: ? Connected and authorized

### 3. **Enable USB Debugging on Your Android Device**
   If you haven't already:
   1. Go to **Settings** ? **About Phone**
   2. Tap **Build Number** 7 times to enable Developer Mode
   3. Go back to **Settings** ? **Developer Options**
   4. Enable **USB Debugging**
   5. When you plug in the USB cable, accept the **"Allow USB debugging?"** prompt

### 4. **Select Deployment Target in Visual Studio**
   1. In Visual Studio, set **MauiMicroSample** as the startup project
   2. In the toolbar, you should now see your Android device in the deployment target dropdown
   3. Select your device (it will show as a physical device, not an emulator)
   4. Click **Run** (F5) or **Start Without Debugging** (Ctrl+F5)

## Troubleshooting

### Device Not Showing in Visual Studio?

**Option 1: Verify ADB Connection**
```powershell
adb devices
```
You should see:
```
List of devices attached
47031FDAS0042Q	device
```

**Option 2: Restart ADB Server**
```powershell
adb kill-server
adb start-server
adb devices
```

**Option 3: Check USB Cable**
- Use a **data cable**, not just a charging cable
- Try a different USB port
- Try a different cable

**Option 4: Re-authorize Device**
```powershell
adb kill-server
adb start-server
```
Then unplug and replug your device, and accept the USB debugging prompt again.

**Option 5: Check Device Driver (Windows)**
1. Open **Device Manager** (Win+X ? Device Manager)
2. Look for your device under **Portable Devices** or **Other Devices**
3. If it shows a yellow warning, update the driver:
   - Right-click ? **Update Driver**
   - Choose **Browse my computer for drivers**
   - Point to: `C:\Program Files (x86)\Android\android-sdk\extras\google\usb_driver`

### Still Not Working?

**Check ANDROID_HOME in current session:**
```powershell
$env:ANDROID_HOME
```
Should return: `C:\Program Files (x86)\Android\android-sdk`

**If empty, manually set for current PowerShell session:**
```powershell
$env:ANDROID_HOME = "C:\Program Files (x86)\Android\android-sdk"
$env:Path += ";C:\Program Files (x86)\Android\android-sdk\platform-tools"
```

## Alternative: Use Android Emulator

If you prefer to use an emulator instead:

1. Open **Visual Studio** ? **Tools** ? **Android** ? **Android Device Manager**
2. Click **New Device**
3. Choose a device profile (e.g., Pixel 7)
4. Select a system image (e.g., API 34)
5. Click **Create**
6. Start the emulator from the Device Manager
7. Select the emulator as your deployment target

## Build Configuration Notes

The `MauiMicroSample` project is configured to build for:
- **net10.0-android** (Android apps on .NET 10)
- **net10.0-ios** (iOS apps on .NET 10)
- **net10.0-maccatalyst** (macOS apps on .NET 10)
- **net10.0-windows** (Windows apps on .NET 10)

When you select an Android device, Visual Studio will build the `net10.0-android` target automatically.

---

**Created**: During .NET 9 & 10 multi-target upgrade
**Device Confirmed**: Physical Android device `47031FDAS0042Q` successfully detected via ADB
