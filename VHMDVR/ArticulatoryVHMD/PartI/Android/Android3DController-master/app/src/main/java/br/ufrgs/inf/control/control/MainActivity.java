package br.ufrgs.inf.control.control;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.opengl.Matrix;
import android.os.Handler;
import android.os.Looper;
import android.preference.PreferenceManager;
import android.support.v4.app.DialogFragment;
import android.support.v4.view.MotionEventCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.ScaleGestureDetector;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageButton;
import android.widget.PopupMenu;
import android.widget.RelativeLayout;
import android.widget.TextView;

import java.nio.ByteBuffer;
import java.util.Random;

public class MainActivity extends AppCompatActivity implements PopupMenu.OnMenuItemClickListener {
    public float touchX = 0.0f;
    public boolean isTouching = false;
    public static SharedPreferences config;
    public TCPClient tcp = new TCPClient();

    public void setConnected(boolean f){
        if (!f) {
            findViewById(R.id.loadingPanel).setAlpha(1.0f);
        } else {
            findViewById(R.id.loadingPanel).setAlpha(0.0f);
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        getWindow().requestFeature(Window.FEATURE_ACTION_BAR);
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        config = PreferenceManager.getDefaultSharedPreferences(this);
        if(config.getString("ip", null) == null) config.edit().putString("ip","143.54.13.230").commit();
        if(config.getInt("port", 0) == 0) config.edit().putInt("port", 8002).commit();

        tcp.activity = this;
        tcp.start();


        final ImageButton menuButton = (ImageButton) findViewById(R.id.menu_button);
        menuButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                PopupMenu popupMenu = new PopupMenu(MainActivity.this, view);
                popupMenu.setOnMenuItemClickListener(MainActivity.this);
                popupMenu.inflate(R.menu.popup_menu);
                popupMenu.show();
            }
        });
    }

    public boolean onMenuItemClick(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.item_server:
                ConfigDialog dialog = new ConfigDialog();
                dialog.show(getSupportFragmentManager(), "config");
                return true;
        }
        return false;
    }

    public static class ConfigDialog extends DialogFragment {
        @Override
        public Dialog onCreateDialog(Bundle savedInstanceState) {

            AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
            LayoutInflater inflater = getActivity().getLayoutInflater();
            final View layout = inflater.inflate(R.layout.dialog_config, null);

            ((TextView) layout.findViewById(R.id.ip)).setText(config.getString("ip",""));
            ((TextView) layout.findViewById(R.id.port)).setText(""+config.getInt("port",0));

            builder.setView(layout)
                    .setPositiveButton("Save", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int id) {
                            String ip =((TextView) layout.findViewById(R.id.ip)).getText().toString();
                            int port = Integer.parseInt(((TextView) layout.findViewById(R.id.port)).getText().toString());

                            config.edit().putString("ip", ip).putInt("port", port).commit();

                        }
                    })
                    .setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
                        public void onClick(DialogInterface dialog, int id) {
                            ConfigDialog.this.getDialog().cancel();
                        }
                    });
            return builder.create();
        }
    }

    @Override
    public boolean onTouchEvent(MotionEvent event) {
        /*DisplayMetrics metrics = new DisplayMetrics();
        getWindowManager().getDefaultDisplay().getMetrics(metrics);
        float thistouchX = event.getX() - (metrics.widthPixels/2);*/
        float thistouchX = event.getX();
        if (event.getAction() == MotionEvent.ACTION_DOWN) {
            touchX = thistouchX;
            isTouching = true;
        }
        return false;
    }

    int asd = 10;
    int qwe = 0;
    public  byte[] dataToSend(){
        qwe = 0;

        int bits = Float.floatToIntBits(touchX);
        byte[] packet = new byte[4];
        packet[0] = (byte)(bits & 0xff);
        packet[1] = (byte)((bits >> 8) & 0xff);
        packet[2] = (byte)((bits >> 16) & 0xff);
        packet[3] = (byte)((bits >> 24) & 0xff);

        return packet;
    }

    @Override
    protected void onPause() {
        super.onPause();
        //mSensorManager.unregisterListener(this);
        //tcp.stopClient();
    }

    @Override
    protected void onStop() {
        super.onStop();
        //finish();
        //tcp.stopClient();
    }

    @Override
    protected void onResume() { super.onResume(); }

    @Override
    protected void onStart() {
        super.onStart();
    }

}
