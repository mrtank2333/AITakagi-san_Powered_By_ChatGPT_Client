using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour
{
    //音乐播放器
    [SerializeField] private MusicPlayer m_MusicPlayer;
    //脚本
    [SerializeField] private ChatScript m_ChatScript;
    //VITS语音
    [SerializeField] private VITS_Speech m_VITS_Player;
    //信息验证
    [SerializeField] private Msg_Validate m_Msg_Validate;
    //渐变
    [SerializeField] private Raw_Image m_Raw_Image;
    //聊天UI层
    [SerializeField] private GameObject m_PostGUI;
    //聊天发送层
    [SerializeField] private GameObject m_SendText;
    public int 播放次数 = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (m_VITS_Player.getLan().Equals(""))
        {
            Debug.Log("m_VITS_Player.getLan() Is NullorBlank");
            m_VITS_Player.SetChangeLan("中文");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 随机播放模式
    /// </summary>
    public void StartRandomPlay()
    {
        List<Tuple<string, string>> musicName = m_MusicPlayer.getRandomMusicName();
        m_ChatScript.m_ChatHistory.Add("播放随机音乐");
       /* UnloadAsset(m_MusicPlayer.audioClip); 
        m_MusicPlayer.audioClip = Resources.Load<AudioClip>("Music/" + musicName[0].Item1);*/
        m_MusicPlayer.Sts = "0";
        m_SendText.SetActive(false);
        m_PostGUI.SetActive(true);

        //去除文件名开头的XX.
        string a = musicName[0].Item1.Substring(0, 3);
        string musicNameJP = musicName[0].Item1.Replace(a, "");
        string musicNameCN = musicName[0].Item2.Replace(a, "");
        if (播放次数 == 0)
        {

            
            if (m_VITS_Player.langString.Equals("中文"))
            {
                m_ChatScript.CallBack("好的，那我为你唱一首《" + musicNameCN + "》吧");
                Debug.Log(musicNameCN);
                播放次数 += 1;
            }
            else if (m_VITS_Player.langString.Equals("日语"))
            {
                m_ChatScript.CallBack("じゃ、「" + musicNameJP + "」を歌いましょう");
                Debug.Log(musicNameJP);
                播放次数 += 1;
            }
        }
        else
        {
            if (m_VITS_Player.langString.Equals("中文"))
            {
                m_ChatScript.CallBack("接下来我再为你唱一首《" + musicNameCN + "》吧");
                Debug.Log(musicNameCN);
            }
            else if (m_VITS_Player.langString.Equals("日语"))
            {
                m_ChatScript.CallBack("じゃ、続けて「" + musicNameJP + "」を歌いましょう");
                Debug.Log(musicNameJP);
            }
        }
        m_Msg_Validate.IsOutputTextMsg = true;
        m_Msg_Validate.IsPlayingMusic = true;
    }

    //卸载指定资源
    public void UnloadAsset(UnityEngine.Object obj)
    {
        Resources.UnloadAsset(obj);
    }
}
