using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TCP : MonoBehaviour
{
    public static TCP instance;

    public RawImage my_img;
    public GameObject R_shoe;
    public GameObject L_shoe;
    public GameObject R_shoe_1;
    public GameObject L_shoe_1;
    public GameObject R_shoe_2;
    public GameObject L_shoe_2;
    public GameObject R_shoe_3;
    public GameObject L_shoe_3;
    public GameObject R_shoe_4;
    public GameObject L_shoe_4;
    public List<GameObject> R_shoes = new List<GameObject>();
    public List<GameObject> L_shoes = new List<GameObject>();


    public float shoe_scale = 1.0f;
    int R_deg = 0;
    int L_deg = 0;
    int R_leg_len = 0;
    int L_leg_len = 0;

    Socket serverSocket; //服務器端socket
    IPAddress ip; //主機ip
    IPEndPoint ipEnd;
    string recvStr; //接收的字符串
    public string recv_keypoint_str;
    string sendStr; //發送的字符串
    byte[] recvData = new byte[1024]; //接收的數據，必須為字節
    byte[] sendData = new byte[1024]; //發送的數據，必須為字節
    byte[] recv_keypoint = new byte[1024];
    public byte[] recv_img;
    int recvLen; //接收的數據長度
    int flag = 0;

    float R_yRotation = 5.0f;
    float R_center_x = 0f;
    float R_center_y = 0f;

    float L_yRotation = 5.0f;
    float L_center_x = 0f;
    float L_center_y = 0f;

    int R_ankel_x;
    int R_ankel_y;
    int R_Toe_x;
    int R_Toe_y;

    public GameObject R_center;
    public GameObject L_center;

    Thread connectThread; //連接線程

    private void Awake()
    {
        if (instance == null)
            instance = this;

        R_shoes.Add(R_shoe);
        R_shoes.Add(R_shoe_1);
        R_shoes.Add(R_shoe_2);
        R_shoes.Add(R_shoe_3);
        R_shoes.Add(R_shoe_4);

        L_shoes.Add(L_shoe);
        L_shoes.Add(L_shoe_1);
        L_shoes.Add(L_shoe_2);
        L_shoes.Add(L_shoe_3);
        L_shoes.Add(L_shoe_4);
    }

    //初始化
    void InitSocket()
    {
        //定義服務器的IP和端口，端口與服務器對應
        ip = IPAddress.Parse("127.0.0.1"); //可以是局域網或互聯網ip，此處是本機
        ipEnd = new IPEndPoint(ip, 9000);


        //開啟一個線程連接，必須的，否則主線程卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketConnet()
    {
        if (serverSocket != null)
            serverSocket.Close();
        //定義套接字類型,必須在子線程中定義
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        print("ready to connect");
        //連接
        serverSocket.Connect(ipEnd);

        //輸出初次連接收到的字符串
        //recvLen = serverSocket.Receive(recvData);
        //recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
        //print(recvStr);
    }

    void SocketSend(string sendStr)
    {
        //清空發送緩存
        sendData = new byte[1024];
        //數據類型轉換
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //發送
        serverSocket.Send(sendData, sendData.Length, SocketFlags.None);
    }

    void SocketReceive()
    {
        SocketConnet();
        //不斷接收服務器發來的數據
        while (true)
        {
            SocketSend("OK_1");

            recvData = new byte[1024];
            recvLen = serverSocket.Receive(recvData);

            if (recvLen == 0)
            {
                SocketConnet();
                continue;
            }

            SocketSend("OK_2");


            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);

            int img_size = Convert.ToInt32(recvStr);
            //int cou = 0;
            flag = 0;
            recv_img = new byte[img_size];
            int recv_img_len = serverSocket.Receive(recv_img);

            if (img_size != recv_img_len)
            {
                Debug.Log("error");
                print("error");
            }
            else
            {
                flag = 1;

                SocketSend("OK_3");

                recv_keypoint = new byte[1024];
                int recv_keypoint_len = serverSocket.Receive(recv_keypoint);
                recv_keypoint_str = Encoding.ASCII.GetString(recv_keypoint, 0, recv_keypoint_len);

                string[] arr = recv_keypoint_str.Split(',');
                R_deg = int.Parse(arr[50]);
                L_deg = int.Parse(arr[51]);
                R_leg_len = int.Parse(arr[52]);
                L_leg_len = int.Parse(arr[53]);

                if (int.Parse(arr[54]) != -1)
                {
                    if (change_shoe.index != int.Parse(arr[54]))
                    {
                        change_shoe.animation_flag = true;
                        //print("Change");
                    }

                    //change_shoe.index = int.Parse(arr[54]);
                }



                if (R_deg != 999)
                {
                    R_yRotation = R_deg;


                    R_ankel_x = int.Parse(arr[22]);
                    R_ankel_y = int.Parse(arr[23]);
                    R_Toe_x = int.Parse(arr[44]);
                    R_Toe_y = int.Parse(arr[45]);
                    R_center_x = (R_ankel_x + R_Toe_x) / 2f;
                    R_center_y = (R_ankel_y + R_Toe_y) / 2f;
                }
                if (L_deg != 999)
                {
                    L_yRotation = L_deg;

                    int L_ankel_x = int.Parse(arr[28]);
                    int L_ankel_y = int.Parse(arr[29]);
                    int L_Toe_x = int.Parse(arr[38]);
                    int L_Toe_y = int.Parse(arr[39]);
                    L_center_x = (L_ankel_x + L_Toe_x) / 2f;
                    L_center_y = (L_ankel_y + L_Toe_y) / 2f;
                }
            }
        }
    }

    void SocketQuit()
    {
        //關閉線程
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最後關閉服務器
        if (serverSocket != null)
            serverSocket.Close();
        print("diconnect");
    }

    // Use this for initialization
    void Start()
    {
        my_img.texture = new Texture2D(640, 480, TextureFormat.RGB24, false);

        InitSocket();

        R_yRotation = 0f;

        L_yRotation = 0f;

    }


    // Update is called once per frame
    void Update()
    {

        if (flag == 1)
        {
            var texture = my_img.texture as Texture2D;
            texture.LoadImage(recv_img);
            texture.Apply();

            if (change_shoe.index == 0)
            {
                Debug.Log("Shoe1");

                R_center.GetComponent<RectTransform>().localPosition = new Vector3((float)(-320f + R_center_x), (float)(240f - R_center_y));
                R_shoe.transform.position = R_center.transform.position;
                R_shoe.transform.localEulerAngles = new Vector3(-90f, 270f, R_yRotation);
            }
            else if (change_shoe.index == 1)
            {
                Debug.Log("Shoe2");
                R_center.GetComponent<RectTransform>().localPosition = new Vector3((float)(-320f + R_center_x), (float)(240f - R_center_y));
                R_shoe_1.transform.position = R_center.transform.position - new Vector3(0f, 5f, 0f);
                R_shoe_1.transform.localEulerAngles = new Vector3(-90f, -90f, R_yRotation);
            }
            else if (change_shoe.index == 2)
            {
                Debug.Log("Shoe3");
                R_center.GetComponent<RectTransform>().localPosition = new Vector3((float)(-320f + R_center_x), (float)(240f - R_center_y));
                R_shoe_2.transform.position = R_center.transform.position - new Vector3(0f, 5f, 0f);
                R_shoe_2.transform.localEulerAngles = new Vector3(-90f, -90f, R_yRotation);
            }
            else if (change_shoe.index == 3)
            {
                Debug.Log("Shoe4");
                R_center.GetComponent<RectTransform>().localPosition = new Vector3((float)(-320f + R_center_x), (float)(240f - R_center_y));
                R_shoe_3.transform.position = R_center.transform.position - new Vector3(0f, 5f, 0f);
                R_shoe_3.transform.localEulerAngles = new Vector3(-90f, -90f, R_yRotation);
            }
            else if (change_shoe.index == 4)
            {
                Debug.Log("Shoe5");
                R_center.GetComponent<RectTransform>().localPosition = new Vector3((float)(-320f + R_center_x), (float)(240f - R_center_y));
                R_shoe_4.transform.position = R_center.transform.position - new Vector3(0f, 5f, 0f);
                R_shoe_4.transform.localEulerAngles = new Vector3(-90f, -90f, R_yRotation);
            }
        }
        if (L_deg != 999)
        {
            //print(change_shoe.index);
            if (change_shoe.index == 0)
            {
                L_center.GetComponent<RectTransform>().localPosition = new Vector3((float)(-320f + L_center_x), (float)(240f - L_center_y));
                L_shoe.transform.position = L_center.transform.position;
                L_shoe.transform.localEulerAngles = new Vector3(-90f, 270f, L_yRotation);
            }
            else if (change_shoe.index == 1)
            {
                L_center.GetComponent<RectTransform>().localPosition = new Vector3((float)(-320f + L_center_x), (float)(240f - L_center_y));
                L_shoe_1.transform.position = L_center.transform.position - new Vector3(0f, 5f, 0f);
                L_shoe_1.transform.localEulerAngles = new Vector3(-90f, -90f, L_yRotation);
            }
            else if (change_shoe.index == 2)
            {
                L_center.GetComponent<RectTransform>().localPosition = new Vector3((float)(-320f + L_center_x), (float)(240f - L_center_y));
                L_shoe_2.transform.position = L_center.transform.position - new Vector3(0f, 5f, 0f);
                L_shoe_2.transform.localEulerAngles = new Vector3(-90f, -90f, L_yRotation);
            }
            else if (change_shoe.index == 3)
            {
                L_center.GetComponent<RectTransform>().localPosition = new Vector3((float)(-320f + L_center_x), (float)(240f - L_center_y));
                L_shoe_3.transform.position = L_center.transform.position - new Vector3(0f, 5f, 0f);
                L_shoe_3.transform.localEulerAngles = new Vector3(-90f, -90f, L_yRotation);
            }
            else if (change_shoe.index == 4)
            {
                L_center.GetComponent<RectTransform>().localPosition = new Vector3((float)(-320f + L_center_x), (float)(240f - L_center_y));
                L_shoe_4.transform.position = L_center.transform.position - new Vector3(0f, 5f, 0f);
                L_shoe_4.transform.localEulerAngles = new Vector3(-90f, -90f, L_yRotation);
            }
        }
    }


    //程序退出則關閉連接
    void OnApplicationQuit()
    {
        SocketQuit();
    }
}
