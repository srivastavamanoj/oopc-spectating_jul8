using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
#if UNITY_PS4 && !UNITY_EDITOR
using UnityEngine.PS4;
#endif


public class UDPSend : MonoBehaviour
{    
    public string IPofReceiver;
    public int port;
    public bool readIPfromFile;
    public string textFile;

    private IPEndPoint remoteEndPoint;
    private UdpClient client;
    private int counter;
    private string localIP;



    private void Awake()
    {
        if (readIPfromFile)
        {
            ReadIPfromFile();
        }

        //Debug.Log("UDP Send IP address: " + IPofReceiver);
    }



    void Start()
	{        
        localIP = GetLocalIP();
        try
        {
            // Set the IP endPoint and establish the connection		
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(IPofReceiver), port);
            client = new UdpClient();           
        }
        catch (Exception e)
        {
            Debug.Log("Not valid IP address: " + e.ToString());
        }
	}

        
    public void SendString(string message)
    {
    	try 
        {
            // Encode string message
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Send message
            client.Send(data, data.Length, remoteEndPoint);			          
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }



    void Update()
    {
        // for testing purposes      
#if !UNITY_EDITOR
        TestMessagePS4();
#else
        TestMessage();
#endif
    }


    void TestMessage()
    {
        // If spacebar is pressed send test message
        if (Input.GetKeyDown(KeyCode.Space))
        {
            counter++;
            string tempStr = String.Format("({0}) This is a test message sent from {1} ", counter, localIP);            
            SendString(tempStr);
            Debug.Log("111   test message sent to IP: " + IPofReceiver + "   port: " + port);
        }
    }


    void TestMessagePS4()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button10))        // crosspad up 
        {
            counter++;            
            string tempStr = String.Format("({0}) This is a test message sent from {1} ", counter, localIP);
            SendString(tempStr);
            Debug.Log("test message sent to IP: " + IPofReceiver + "   port: " + port);
        }
    }


    public void closeUDP()
    {
        try
        {
            client.Close();
            //Debug.Log("Connection to "+ IPofReceiver +" : "+ port + " is closed");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }


    void OnApplicationQuit()
    {
        closeUDP();
    }


    private void OnDisable()
    {
        closeUDP();
    }


    private string GetLocalIP()
    {
        IPHostEntry host;
        host = Dns.GetHostEntry(Dns.GetHostName());
        string lIp = "";

        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                lIp = ip.ToString();
                break;
            }
        }

        return lIp;
    }


    private void ReadIPfromFile()
    {
        string txtFilePath;

#if UNITY_EDITOR        
        txtFilePath = "Assets/" + textFile;
#else        
        txtFilePath = textFile;
#endif

        if (File.Exists(txtFilePath))
        {
            string readText = File.ReadAllText(txtFilePath);
            IPofReceiver = readText;
        }
        else
        {
            Debug.Log("File with IP address does not exist... ");
        }
    }
}