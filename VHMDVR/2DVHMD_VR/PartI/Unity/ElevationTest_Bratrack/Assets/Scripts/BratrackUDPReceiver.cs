using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;
using System.Timers;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;


public class BratrackUDPReceiver : MonoBehaviour {

    //UDP variables
    public int UDPport = 3000;
    private UdpClient client;
    private IPEndPoint RemoteIpEndPoint;
    private Regex regexParse;
    private Thread t_udp;

    public static double R1X, R1Y, R1Z, R2X, R2Y, R2Z, R3X, R3Y, R3Z, TX, TY, TZ;

    void Start() {
        //UDP code
        client = new UdpClient(UDPport);
        RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        regexParse = new Regex(@"\d*$");
        t_udp = new Thread(new ThreadStart(UDPRead));
        t_udp.Name = "Mindtuner UDP thread";
        t_udp.Start();
    }



    void OnApplicationQuit() {
        //UDP thread closed
        if (t_udp != null) t_udp.Abort();
        //		print ("UDP thread closed");
    }


    public void UDPRead() {
        while (true) {
            try {
                //print ("listening UDP port " + UDPport);
                byte[] receiveBytes = client.Receive(ref RemoteIpEndPoint);
                //string returnData = Encoding.ASCII.GetString(receiveBytes);

                int byteLeap = 8;
                int byteOffset = 1;
                int incrementLeap = 0;
                byte[] subarray = new byte[8];


                Array.Copy(receiveBytes, 0, subarray, 0, byteLeap);
                long timestamp = BitConverter.ToInt64(subarray, 0);
                //print (timestamp);

                Array.Copy(receiveBytes, byteLeap, subarray, 0, byteLeap);
                string returnData = Encoding.ASCII.GetString(subarray);
                //print (returnData);

                incrementLeap = byteLeap * 2 + byteOffset;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                R1X = BitConverter.ToDouble(subarray, 0);

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                R1Y = BitConverter.ToDouble(subarray, 0);

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                R1Z = BitConverter.ToDouble(subarray, 0);

                //print ("R1X:" + R1X + " R1Y:" + R1Y + " R1Z:" + R1Z); 

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                R2X = BitConverter.ToDouble(subarray, 0);

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                R2Y = BitConverter.ToDouble(subarray, 0);

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                R2Z = BitConverter.ToDouble(subarray, 0);

                //print ("R2X:" + R2X + " R2Y:" + R2Y + " R2Z:" + R2Z); 

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                R3X = BitConverter.ToDouble(subarray, 0);

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                R3Y = BitConverter.ToDouble(subarray, 0);

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                R3Z = BitConverter.ToDouble(subarray, 0);

                //print ("R3X:" + R3X + " R3Y:" + R3Y + " R3Z:" + R3Z);

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                TX = BitConverter.ToDouble(subarray, 0);

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                TY = BitConverter.ToDouble(subarray, 0);

                incrementLeap = incrementLeap + byteLeap;
                Array.Copy(receiveBytes, incrementLeap, subarray, 0, byteLeap);
                TZ = BitConverter.ToDouble(subarray, 0);


                //print ("TX:" + TX + " TY:" + TY + " TZ:" + TZ );


            }
            catch (Exception e) {
                //				Debug.Log("Not so good " + e.ToString());
            }
            //Thread.Sleep(20);
        }
    }


}







/*
public class Mindtuner : MonoBehaviour{

	void Start(){
		client = new UdpClient(port);
		RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
		regexParse = new Regex(@"\d*$");
		t_udp = new Thread (new ThreadStart (UDPRead));
		t_udp.Name = "Mindtuner UDP thread";
		t_udp.Start();
	}
	
	public void UDPRead(){
		while (true){
			try{
				Debug.Log("listening UDP port " + port);
				byte[] receiveBytes = client.Receive(ref RemoteIpEndPoint);
				string returnData = Encoding.ASCII.GetString(receiveBytes);
				// parsing
				Debug.Log(returnData);
				Debug.Log(regexParse.Match(returnData).ToString());
				UDPValue = Int32.Parse(regexParse.Match(returnData).ToString());
			}
			catch (Exception e){
				Debug.Log("Not so good " + e.ToString());
			}
			Thread.Sleep(20);
		}
	}
	
	void Update(){
		if (t_udp != null) Debug.Log(t_udp.IsAlive);
		Vector3 scale = transform.localScale;
		scale.x = (float)UDPValue;
	}
}
*/



