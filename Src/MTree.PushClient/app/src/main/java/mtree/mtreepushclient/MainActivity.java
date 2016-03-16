package mtree.mtreepushclient;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.AsyncTask;
import com.google.android.gms.gcm.*;
import com.microsoft.windowsazure.messaging.*;
import com.microsoft.windowsazure.notifications.NotificationsManager;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity {

    private String SENDER_ID = "6957127032";
    private GoogleCloudMessaging gcm;
    private NotificationHub hub;
    private String HubName = "MTreeNotificationHub";
    private String HubListenConnectionString = "Endpoint=sb://mtreenotificationspace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=RQDGxAhXvvS/Mgg5AYwx5FfecJifbIEadaWu/44LzOo=";
    private static Boolean isVisible = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        MyHandler.mainActivity = this;
        NotificationsManager.handleNotifications(this, SENDER_ID, MyHandler.class);
        gcm = GoogleCloudMessaging.getInstance(this);
        hub = new NotificationHub(HubName, HubListenConnectionString, this);
        registerWithNotificationHubs();
    }

    @Override
    protected void onStart() {
        super.onStart();
        isVisible = true;
    }

    @Override
    protected void onPause() {
        super.onPause();
        isVisible = false;
    }

    @Override
    protected void onResume() {
        super.onResume();
        isVisible = true;
    }

    @Override
    protected void onStop() {
        super.onStop();
        isVisible = false;
    }

    public void ToastNotify(final String notificationMessage)
    {
        if (isVisible == true)
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    Toast.makeText(MainActivity.this, notificationMessage, Toast.LENGTH_LONG).show();
                }
            });
    }

    @SuppressWarnings("unchecked")
    private void registerWithNotificationHubs() {
        new AsyncTask() {
            @Override
            protected Object doInBackground(Object... params) {
                try {
                    String regid = gcm.register(SENDER_ID);
                    ToastNotify("Registered Successfully - RegId : " + hub.register(regid).getRegistrationId());
                } catch (Exception e) {
                    ToastNotify("Registration Exception Message - " + e.getMessage());
                    return e;
                }
                return null;
            }
        }.execute(null, null, null);
    }
}
