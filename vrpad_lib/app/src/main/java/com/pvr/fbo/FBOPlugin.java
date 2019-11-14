package com.pvr.fbo;

import android.graphics.SurfaceTexture;
import android.util.Log;
import android.view.Surface;

public class FBOPlugin implements SurfaceTexture.OnFrameAvailableListener {

    private SurfaceTexture mSurfaceTexture;
    private Surface mSurface;
    private FBOTexture mFBOTexture;
    private int mOESTextureId;
    private int mUnityTextureId;
    private boolean mIsUpdateFrame;

    public void start(int unityTextureId, int width, int height){
        mUnityTextureId = unityTextureId;
        mOESTextureId = FBOUtils.createOESTextureID();

        mSurfaceTexture = new SurfaceTexture(mOESTextureId);
        mSurface = new Surface(mSurfaceTexture);
        mSurfaceTexture.setDefaultBufferSize(width, height);
        mSurfaceTexture.setOnFrameAvailableListener(this);

        mFBOTexture = new FBOTexture(mOESTextureId);
        mFBOTexture.surfaceChanged(width, height, mUnityTextureId);
    }

    @Override
    public void onFrameAvailable(SurfaceTexture surfaceTexture) {
        mIsUpdateFrame = true;
    }

    public void updateTexture() {
        mIsUpdateFrame = false;
        mSurfaceTexture.updateTexImage();
        mFBOTexture.draw();
    }

    public boolean isUpdateFrame(){
        return mIsUpdateFrame;
    }

    public Surface getSurface(){
        return mSurface;
    }

    public void release(){
        if(mSurface != null){
            mSurface.release();
            mSurface = null;
        }
        if(mSurfaceTexture != null){
            mSurfaceTexture.release();
            mSurfaceTexture = null;
        }
    }

}
