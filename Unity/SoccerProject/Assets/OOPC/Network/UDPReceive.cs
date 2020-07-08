using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

#if UNITY_PS4 && !UNITY_EDITOR
using UnityEngine.PS4;
#endif

public class UDPReceive : MonoBehaviour
{
    [Tooltip("In case of PC is connected to multiple networks")]
    public string m_localIP;
    public int port;
    public bool HUDenabled;
    public bool leftHUD;
    public GameObject team;

    [HideInInspector]
    public string udpString;
    [HideInInspector]
    public bool isNewMsgToDisplay = false;
    [HideInInspector]
    public bool isNewMsgToProcess = false;

    private IPAddress IP_sender;
    private IPEndPoint remoteEndPoint;
    private UdpClient receivingUdpClient;
    private string udpStringAsync;    
    private bool UDPreceivingStarted = false;
    private string localIP;
    private string teamName;
    private List<string> listMsgs;
    private int xScreenPos = 10;
    private int boxWidht = 300;
    private int boxHeight = 200;


    void Awake()
    {
        if (string.IsNullOrEmpty(m_localIP))
            localIP = GetLocalIP();        
        else
            localIP = m_localIP;

        receivingUdpClient = new UdpClient(port);                   
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);        
        receivingUdpClient.BeginReceive(new AsyncCallback(ReceiveUdpData), null);
    }


    private void Start()
    {
        teamName = team.name;

        listMsgs = new List<string>();
    }



    private void Update()
    {
        /*
#if !UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
#else
        if (Input.GetKeyDown(KeyCode.A))        
#endif
        {
            HUDenabled = !HUDenabled;            
        }
        */
        if (string.IsNullOrEmpty(udpString))
        {
            isNewMsgToDisplay = true;
            udpString = "Local IP:" + localIP + "     Waiting on port: " + port;
        }
    }



    void OnGUI()
    {        
        if (HUDenabled == true)
        {
            displayHUD();
        }
    }


    void ReceiveUdpData(IAsyncResult res)
    {
        // End a pending asynchronous receive
        Byte[] receiveBytes = receivingUdpClient.EndReceive(res, ref remoteEndPoint);
        // get udp message
        string message = Encoding.ASCII.GetString(receiveBytes);        
        udpStringAsync = message;        
        ProcessMsg(udpStringAsync);
        isNewMsgToDisplay = true;
        isNewMsgToProcess = true;
        // get next packet 
        receivingUdpClient.BeginReceive(ReceiveUdpData, null);
    }


    private void ProcessMsg(string amsg)
    {
        if (listMsgs.Count >= 10)
        {
            listMsgs.RemoveAt(0);
        }

        listMsgs.Add(amsg);

        string temp = "";
        foreach (string str in listMsgs)
        {
            temp = temp + str + '\n';
        }
        udpString = temp;
    }

    
    void displayHUD()
    {
        if (!leftHUD)
        {
            if (Screen.width > 800)
                xScreenPos = Screen.width - (boxWidht + 10);
            else
                xScreenPos = 500;
        }

        GUI.BeginGroup(new Rect(xScreenPos, 10, boxWidht, boxHeight));
            GUI.Box(new Rect(0, 0, boxWidht, boxHeight), "Spectators comments for: " + teamName);
            if (UDPreceivingStarted)
            {
                //GUI.Label(new Rect(20, 25, 400, 25), "UDP string:  " + udpString);
                GUI.Label(new Rect(20, 25, boxWidht, boxHeight), "UDP string:  " + udpString);
            }
            else
            {
                GUI.Label(new Rect(20, 25, boxWidht, boxHeight), udpString);
            }
        GUI.EndGroup();
    }    


    public void closeUDP()
    {
        try
        {
            receivingUdpClient.Close();
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
}
