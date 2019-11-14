package com.pvr.vrpad;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.KeyEvent;

import com.pvr.PvrCallback;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerNativeActivityPico;

import java.lang.reflect.Method;

public class MainActivity extends UnityPlayerNativeActivityPico {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        registerPvrManagerCallback();
        Utils.print("onCreate");
    }

    @Override
    protected void onStart() {
        super.onStart();
        Utils.print("onStart");
    }

    @Override
    protected void onResume() {
        super.onResume();
        Utils.print("onResume");
        UnityPlayer.UnitySendMessage(Utils.UNITY_OBJ,"ActivityOnResume", "");
    }

    @Override
    protected void onPause() {
        super.onPause();
        Utils.print("onPause");
    }

    @Override
    protected void onStop() {
        super.onStop();
        Utils.print("onStop");
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        Utils.print("onDestroy");
        unregisterPvrManagerCallback();
    }

    private PvrCallback mPvrCallback = new PvrCallback() {
        @Override
        public void onEventChanged(Bundle extras) {
            String data = extras.getString("vr2d_key_event", "");
//            LogUtils.i("PvrCallback.onEventChanged--->"+data);
            if(TextUtils.isEmpty(data)){
                return;
            }
            int action = Integer.valueOf(data.split(":")[0]).intValue();
            int keyCode = Integer.valueOf(data.split(":")[1]).intValue();
            UnityPlayer.UnitySendMessage(Utils.UNITY_OBJ,"OnEventChanged", action+"|"+keyCode);
            KeyEvent event = new KeyEvent(action, keyCode);
            if(keyCode == 1001) {
                event = new KeyEvent(action, 96);
            }
            mUnityPlayer.injectEvent(event);
        }
    };

    private void registerPvrManagerCallback() {
        try {
            Class<?> pvr = Class.forName("com.pvr.PvrManager");
            Method getInstance = pvr.getDeclaredMethod("getInstance",
                    Context.class);
            Object pvrManager = getInstance.invoke(pvr, this);
            Method addPvrCallback = pvrManager.getClass().getDeclaredMethod(
                    "addPvrCallback", PvrCallback.class);
            addPvrCallback.invoke(pvrManager,mPvrCallback);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void unregisterPvrManagerCallback() {
        try {
            Class<?> pvr = Class.forName("com.pvr.PvrManager");
            Method getInstance = pvr.getDeclaredMethod("getInstance",
                    Context.class);
            Object pvrManager = getInstance.invoke(pvr, this);
            Method removePvrCallback = pvrManager.getClass().getDeclaredMethod(
                    "removePvrCallback", int.class);
            removePvrCallback.invoke(pvrManager, android.os.Process.myPid());
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

}
