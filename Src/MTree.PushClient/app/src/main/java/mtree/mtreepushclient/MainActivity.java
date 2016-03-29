package mtree.mtreepushclient;

import android.content.ContentValues;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.AsyncTask;
import com.google.android.gms.gcm.*;
import com.microsoft.windowsazure.messaging.*;
import com.microsoft.windowsazure.notifications.NotificationsManager;

import android.util.Log;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.SimpleCursorAdapter;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends AppCompatActivity {

    private String SENDER_ID = "6957127032";
    private GoogleCloudMessaging gcm;
    private NotificationHub hub;
    private String HubName = "MTreeNotificationHub";
    private String HubListenConnectionString = "Endpoint=sb://mtreenotificationspace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=RQDGxAhXvvS/Mgg5AYwx5FfecJifbIEadaWu/44LzOo=";
    private static Boolean isVisible = false;
    public static List<String> tempList = new ArrayList<String>();
    private ArrayAdapter<String> adapter;

    SQLiteDatabase db;
    MySQLiteOpenHelper helper;
    private int simple_list_item_1;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        MyHandler.mainActivity = this;
        NotificationsManager.handleNotifications(this, SENDER_ID, MyHandler.class);
        gcm = GoogleCloudMessaging.getInstance(this);
        hub = new NotificationHub(HubName, HubListenConnectionString, this);
        registerWithNotificationHubs();


        helper = new MySQLiteOpenHelper(MainActivity.this, "MTree.db", null, 1);
        db = helper.getWritableDatabase();

        for (String s : tempList) {
            insetDb(s);
        }
        tempList.clear();

        Cursor cursor = db.rawQuery("SELECT * FROM PushMessage order by _id desc", null);
        cursor.moveToFirst();
        Log.i("MTree", ">>>>>>>>>>>> db count" + cursor.getCount());

        String[] from = new String[] {"content"};
        int[] to = new int[] {android.R.id.text1};

        ListView listView = (ListView)findViewById(R.id.listView);
        SimpleCursorAdapter adapter = new SimpleCursorAdapter(listView.getContext(), android.R.layout.simple_list_item_1, cursor, from, to, 0x01);
        listView.setAdapter(adapter);
    }

    public boolean insetDb(String content) {
        if (helper == null) return false;
        if (db == null) return false;

        ContentValues values = new ContentValues();
        values.put("content", content);
        if (db.insert("PushMessage", null, values) != -1) {
            Log.i("MTree", ">>>>>>>>>>>>>>>>> db inserted");
            return true;
        }

        return false;
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

    public void AddToListView(final String message)
    {
        adapter.insert(message, 0);
    }

    @SuppressWarnings("unchecked")
    private void registerWithNotificationHubs() {
        new AsyncTask() {
            @Override
            protected Object doInBackground(Object... params) {
                try {
                    String regid = gcm.register(SENDER_ID);
                    //ToastNotify("Registered Successfully - RegId : " + hub.register(regid).getRegistrationId());
                } catch (Exception e) {
                    ToastNotify("Registration Exception Message - " + e.getMessage());
                    return e;
                }
                return null;
            }
        }.execute(null, null, null);
    }
}
