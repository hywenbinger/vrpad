package com.pvr.vrpad;

import android.app.Activity;
import android.app.ActivityOptions;
import android.app.Instrumentation;
import android.content.Context;
import android.content.Intent;
import android.graphics.SurfaceTexture;
import android.hardware.display.DisplayManager;
import android.hardware.display.VirtualDisplay;
import android.media.projection.MediaProjection;
import android.opengl.GLES11Ext;
import android.opengl.GLES30;
import android.os.Handler;
import android.text.TextUtils;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Display;
import android.view.KeyEvent;
import android.view.Surface;

import com.pvr.fbo.FBOPlugin;
import com.pvr.input.JniInput;
import com.unity3d.player.UnityPlayer;
import java.lang.reflect.Method;

public class PvrPadManager {

    private Activity mActivity;
    private DisplayManager mDisplayManager;
    private VirtualDisplay mVirtualDisplay;
    private int mVirtualDisplayId = -1;
    private int mRotation = -1;
    private FBOPlugin mFBOPlugin;
    private JniInput mJniInput;
    private Instrumentation mInstrumentation;

    public PvrPadManager(Activity activity) {
        mActivity = activity;
        mJniInput = new JniInput();
        mDisplayManager = (DisplayManager) mActivity.getSystemService(Context.DISPLAY_SERVICE);
        mDisplayManager.registerDisplayListener(mDisplayListener, null);
    }

    public int initSurface(int unityTextureId, int width, int height, int densityDpi){
        Utils.print("initSurface---"+unityTextureId+", "+width+", "+height+", "+densityDpi);
        mFBOPlugin = new FBOPlugin();
        mFBOPlugin.start(unityTextureId, width, height);
        mVirtualDisplay = Utils.createVirtualDisplay(mDisplayManager, width, height, densityDpi);
        mVirtualDisplay.setSurface(mFBOPlugin.getSurface());
        mVirtualDisplayId = mVirtualDisplay.getDisplay().getDisplayId();
        Utils.print(mVirtualDisplay.getDisplay().toString());
        return mVirtualDisplayId;
    }

    public boolean isUpdateFrame(){
        if(mFBOPlugin == null){
            return false;
        }
        return mFBOPlugin.isUpdateFrame();
    }

    public void updateTexture() {
        if(mFBOPlugin == null){
            return;
        }
        mFBOPlugin.updateTexture();
    }

    public void startApp(String packageName, String className){
        Utils.print("startActivity---"+packageName+", "+className);
        String testPkg = Utils.getSystemProperties("pvr.vrpad.pkg", "");
        String testCls = Utils.getSystemProperties("pvr.vrpad.cls", "");
        if(!TextUtils.isEmpty(testPkg) && !TextUtils.isEmpty(testCls)){
            packageName = testPkg;
            className = testCls;
        }
        ActivityOptions options = ActivityOptions.makeBasic();
        options.setLaunchDisplayId(mVirtualDisplayId);
        Intent secondIntent = new Intent();
        secondIntent.setClassName(packageName,className);
        secondIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK
                | Intent.FLAG_ACTIVITY_RESET_TASK_IF_NEEDED);
        mActivity.startActivity(secondIntent, options.toBundle());
    }

    public void down(float x, float y){
        mJniInput.native_input_virtual_display(x, y, 1, 1);
    }

    public void up(float x, float y){
        mJniInput.native_input_virtual_display(x, y, 0, 1);
    }

    public void back(){
        if(mInstrumentation == null){
            mInstrumentation = new Instrumentation();
        }
        mInstrumentation.sendKeyDownUpSync(KeyEvent.KEYCODE_BACK);
    }

    public void release() {
        Utils.print("release");
        mDisplayManager.unregisterDisplayListener(mDisplayListener);
        if(mVirtualDisplay != null){
            mVirtualDisplay.release();
            mVirtualDisplay = null;
        }
        if(mFBOPlugin != null){
            mFBOPlugin.release();
            mFBOPlugin = null;
        }
    }

    private DisplayManager.DisplayListener mDisplayListener = new DisplayManager.DisplayListener() {
        @Override
        public void onDisplayAdded(int displayId) {
            Utils.print("onDisplayAdded--->"+displayId);
            if(mVirtualDisplay != null && mVirtualDisplayId == displayId){
                UnityPlayer.UnitySendMessage(Utils.UNITY_OBJ,"OnDisplayListener", String.valueOf(1));
            }
        }

        @Override
        public void onDisplayRemoved(int displayId) {
            Utils.print("onDisplayRemoved--->"+displayId);
            if(mVirtualDisplay != null && mVirtualDisplayId == displayId){
                UnityPlayer.UnitySendMessage(Utils.UNITY_OBJ,"OnDisplayListener", String.valueOf(2));
            }
        }

        @Override
        public void onDisplayChanged(int displayId) {
            Utils.print("onDisplayChanged--->"+displayId);
            if(mVirtualDisplay != null && mVirtualDisplayId == displayId){
                UnityPlayer.UnitySendMessage(Utils.UNITY_OBJ,"OnDisplayListener", String.valueOf(3));
                final Display display = mVirtualDisplay.getDisplay();
                final int rotation = display.getRotation();
                if(mRotation != rotation){
                    Utils.print(display.toString());
                    UnityPlayer.UnitySendMessage(Utils.UNITY_OBJ,"SetDisplayRotation", String.valueOf(rotation));
                    mRotation = rotation;
                }
            }
        }
    };

}
