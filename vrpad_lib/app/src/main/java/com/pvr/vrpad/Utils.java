package com.pvr.vrpad;

import android.hardware.display.DisplayManager;
import android.hardware.display.VirtualDisplay;
import android.media.projection.MediaProjection;
import android.os.Handler;
import android.text.TextUtils;
import android.util.Log;
import android.view.Display;
import android.view.Surface;

import java.lang.reflect.Method;

public class Utils {

    public static String getSystemProperties(String key, String defaultValue) {
        try {
            final Class<?> systemProperties = Class.forName("android.os.SystemProperties");
            final Method get = systemProperties.getMethod("get", String.class, String.class);
            String result = (String) get.invoke(null, key, defaultValue);
            return TextUtils.isEmpty(result) ? defaultValue : result;
        } catch (Exception e) {
            return defaultValue;
        }
    }

    private static final int DISPLAY_TYPE_UNKNOWN = 0;
    private static final int DISPLAY_TYPE_BUILT_IN = 1;
    private static final int DISPLAY_TYPE_HDMI = 2;
    private static final int DISPLAY_TYPE_WIFI = 3;
    private static final int DISPLAY_TYPE_OVERLAY = 4;
    private static final int DISPLAY_TYPE_VIRTUAL = 5;
    private static final String DISPLAY_NAME = "PvrPadDisplay";
    public static final String UNITY_OBJ = "UI Root";
    private static final String DISPLAY_UNIQUE_ID = "277f1a09-b88d-4d1e-8716-796f114d09cd";

    public static VirtualDisplay createVirtualDisplay(DisplayManager displayManager, int width, int height, int dpi) {
        try {
            Class<?> sysClass = Class.forName("android.hardware.display.DisplayManager");
            Method method = sysClass.getMethod("createVirtualDisplay", MediaProjection.class, String.class, int.class, int.class,
                    int.class, Surface.class, int.class, VirtualDisplay.Callback.class, Handler.class, String.class);
            int flags = DisplayManager.VIRTUAL_DISPLAY_FLAG_PUBLIC;
            flags |= 1 << 6;//DisplayManager.VIRTUAL_DISPLAY_FLAG_SUPPORTS_TOUCH
            flags |= 1 << 7;//DisplayManager.VIRTUAL_DISPLAY_FLAG_ROTATES_WITH_CONTENT
            flags |= 1 << 8;//DisplayManager.VIRTUAL_DISPLAY_FLAG_DESTROY_CONTENT_ON_REMOVAL
            flags |= DisplayManager.VIRTUAL_DISPLAY_FLAG_OWN_CONTENT_ONLY;
            Object obj = method.invoke(displayManager, null,  DISPLAY_NAME, width,
                    height, dpi, null, flags, null, null, DISPLAY_UNIQUE_ID);
            return (VirtualDisplay) obj;
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }

    public static int getDisplayType(Display display) {
        try {
            Class<?> sysClass = Class.forName("android.view.Display");
            Method method = sysClass.getMethod("getType");
            Object obj = method.invoke(display);
            int displayType = Integer.valueOf(String.valueOf(obj)).intValue();
            return displayType;
        } catch (Exception e) {
            e.printStackTrace();
        }
        return 0;
    }

    public static void print(String msg){
        Log.i("PvrPad", msg);
    }

}
