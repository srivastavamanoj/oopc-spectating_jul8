using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
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
            udpString = "Local IP:" + localIP + "     Waiting UDP msgs on port: " + port;
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
        udpString = udpStringAsync;
        isNewMsgToDisplay = true;
        isNewMsgToProcess = true;
        // get next packet 
        receivingUdpClient.BeginReceive(ReceiveUdpData, null);
    }    


    void displayHUD()
    {
        int c = 10;
        if (!leftHUD)
        {
            if (Screen.width > 800)
                c = Screen.width - 410;
            else
                c = 500;
        }

        GUI.BeginGroup(new Rect(c, 10, 400, 300));
            GUI.Box(new Rect(0, 0, 400, 300), "Spectators comments for: " + teamName);
            if (UDPreceivingStarted)
            {
                GUI.Label(new Rect(20, 25, 400, 25), "UDP string:  " + udpString);            
            }
            else
            {
                GUI.Label(new Rect(20, 25, 400, 25), udpString);
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
