using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderImage : MonoBehaviour
{
    public TCP tcp;
    public Text text;
    public RawImage my_img;
    
    void Start()
    {
        my_img.texture = new Texture2D(480, 640, TextureFormat.RGB24, false);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        text.text = tcp.recv_keypoint_str;
        
        var texture = my_img.texture as Texture2D;

        texture.LoadImage(tcp.recv_img);
        texture.Apply();
        */
        Debug.Log("123");
    }
}
