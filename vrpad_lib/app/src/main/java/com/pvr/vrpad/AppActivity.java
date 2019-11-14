package com.pvr.vrpad;

import androidx.appcompat.app.AppCompatActivity;

import android.app.Activity;
import android.content.ComponentName;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.GridView;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.List;

public class AppActivity extends Activity {

    private GridView mGridView;
    private PackageManager mPackageManager;
    private List<ResolveInfo> mResolveInfoList;
    private MyAdapter mAdapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_app);
        mGridView = findViewById(R.id.gv);
        mPackageManager = getPackageManager();

        Intent intent = new Intent(Intent.ACTION_MAIN);
        intent.addCategory(Intent.CATEGORY_LAUNCHER);
        mResolveInfoList = mPackageManager.queryIntentActivities(intent, 0);

        mAdapter = new MyAdapter();
        mGridView.setAdapter(mAdapter);
        mAdapter.notifyDataSetChanged();

        mGridView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> adapterView, View view, int i, long l) {
                final ResolveInfo resolveInfo = mResolveInfoList.get(i);
                final String pkg = resolveInfo.activityInfo.packageName;
                final String cls = resolveInfo.activityInfo.name;
                Intent intent = new Intent();
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK
                        | Intent.FLAG_ACTIVITY_RESET_TASK_IF_NEEDED);
                intent.setClassName(pkg, cls);
                AppActivity.this.startActivity(intent);
            }
        });
    }

    class MyAdapter extends BaseAdapter {

        @Override
        public int getCount() {
            return mResolveInfoList.size();
        }

        @Override
        public Object getItem(int i) {
            return mResolveInfoList.get(i);
        }

        @Override
        public long getItemId(int i) {
            return i;
        }

        @Override
        public View getView(int i, View view, ViewGroup viewGroup) {
            ViewHolder holder;
            if(view == null){
                view = View.inflate(AppActivity.this, R.layout.grid_item, null);
                holder = new ViewHolder();
                holder.mImageView = view.findViewById(R.id.iv);
                holder.mTextView = view.findViewById(R.id.tv);
                view.setTag(holder);
            }else{
                holder = (ViewHolder) view.getTag();
            }
            final ResolveInfo resolveInfo = mResolveInfoList.get(i);
            holder.mTextView.setText(resolveInfo.loadLabel(mPackageManager).toString());
            try {
                ComponentName componentName = new ComponentName(
                        resolveInfo.activityInfo.packageName, resolveInfo.activityInfo.name);
                holder.mImageView.setImageDrawable(mPackageManager.getActivityIcon(componentName));
            } catch (PackageManager.NameNotFoundException e) {
                e.printStackTrace();
            }
            return view;
        }

        class ViewHolder{
            ImageView mImageView;
            TextView mTextView;
        }
    }

}
