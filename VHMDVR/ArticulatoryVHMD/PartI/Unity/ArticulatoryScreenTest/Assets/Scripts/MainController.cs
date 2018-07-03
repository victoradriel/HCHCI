using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

public class MainController : MonoBehaviour {
    public static MainController control;
    public float xstrt = 0f;
    public float xend = 0f;
    public long rtbtDouble = 0;
    public long rtbtDur = 0;
    public bool sentSmthng = false;
    public bool isRunning = false;
    public TcpListener tcpListener;
    private Thread tcpServerRunThread;
    public HandleConnections handleConnections = new HandleConnections();
    public volatile List<Client> clients = new List<Client>();
    public volatile bool acceptingConnections = true;
    bool abort = false;

    TcpClient clientDevice;
    Client client;
    NetworkStream stream;

    void Awake() {
        if (control == null) {
            // Dont Destroy This
            DontDestroyOnLoad(gameObject);
            control = this;

            // TCP Listener
            tcpListener = new TcpListener(IPAddress.Any, 8002);
            tcpListener.Start();
            isRunning = true;
            tcpServerRunThread = new Thread(new ThreadStart(TcpServerRun));
            tcpServerRunThread.Start();
        } else if (control != this) { Destroy(gameObject); }
    }

    public void TcpServerRun() {
        while (isRunning) {            
			try{
				clientDevice = tcpListener.AcceptTcpClient();

                string ipport = clientDevice.Client.RemoteEndPoint.ToString();  // get the ip and port of the new client ip:port
				string[] ip = ipport.Split(':');                                // separate ip and port. The ip is in ip[0] position.
				int clientId = handleConnections.getId(ip[0]);                  // get an id for client. If client has already connected with the same ip, the id will be the same as last connection.
                client = new Client(clientId);
                abort = false;

                if (acceptingConnections){
                    clients.Add(client);
                    
                    new Thread(new ThreadStart(() => DeviceListener())).Start();
                }
			}
			catch(Exception ex){ print("Error tcpListener: " + ex.Message);	}
		}
	}

	void DeviceListener (){
		stream = clientDevice.GetStream();        

		while (clientDevice.Connected && isRunning && !abort) {
			int pos = 0;
			byte[] bytes = new byte[24];
			while (pos != bytes.Length && !abort) {
				int l = stream.Read (bytes, pos, bytes.Length - pos);
				if (l == 0 || !clientDevice.Connected || !isRunning) abort = true;
				pos += l;
			}

            if (!abort) {
                xstrt = System.BitConverter.ToSingle(bytes, 0);
                xend = System.BitConverter.ToSingle(bytes, 4);
                rtbtDouble = System.BitConverter.ToInt64(bytes, 8);
                rtbtDur = System.BitConverter.ToInt64(bytes, 16);
                sentSmthng = true;
            }            
		}

        StopDeviceListener();
	}

    void StopDeviceListener( ) {
        if (stream != null) stream.Close ();
		if (clientDevice != null) clientDevice.Close ();
		if (client != null) client.connected = false;
    }

	public void OnApplicationQuit() {
        if (!abort) StopDeviceListener();

        isRunning = false;
		tcpServerRunThread.Abort ();  // Shutdown server
		tcpListener.Stop();
	}		
}
