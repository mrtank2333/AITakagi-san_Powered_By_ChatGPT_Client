using System;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour
{
    //咄赤殴慧匂
    [SerializeField] private MusicPlayer m_MusicPlayer;
    //重云
    [SerializeField] private ChatScript m_ChatScript;
    //VITS囂咄
    [SerializeField] private VITS_Speech m_VITS_Player;
    //佚連刮屬
    [SerializeField] private Msg_Validate m_Msg_Validate;
    //愁延
    [SerializeField] private Raw_Image m_Raw_Image;
    //祖爺UI蚊
    [SerializeField] private GameObject m_PostGUI;
    //祖爺窟僕蚊
    [SerializeField] private GameObject m_SendText;
    public int 殴慧肝方 = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (m_VITS_Player.getLan().Equals(""))
        {
            Debug.Log("m_VITS_Player.getLan() Is NullorBlank");
            m_VITS_Player.SetChangeLan("嶄猟");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 昧字殴慧庁塀
    /// </summary>
    public void StartRandomPlay()
    {
        List<Tuple<string, string>> musicName = m_MusicPlayer.getRandomMusicName();
        m_ChatScript.m_ChatHistory.Add("殴慧昧字咄赤");
        /* UnloadAsset(m_MusicPlayer.audioClip); 
         m_MusicPlayer.audioClip = Resources.Load<AudioClip>("Music/" + musicName[0].Item1);*/
        m_MusicPlayer.Sts = "0";
        m_SendText.SetActive(false);
        m_PostGUI.SetActive(true);

        //肇茅猟周兆蝕遊議XX.
        string a = musicName[0].Item1.Substring(0, 3);
        string musicNameJP = musicName[0].Item1.Replace(a, "");
        string musicNameCN = musicName[0].Item2.Replace(a, "");
        if (殴慧肝方 == 0)
        {


            if (m_VITS_Player.langString.Equals("嶄猟"))
            {
                m_ChatScript.CallBack("挫議��椎厘葎低蟹匯遍ゞ" + musicNameCN + "〃杏");
                Debug.Log(musicNameCN);
                殴慧肝方 += 1;
            }
            else if (m_VITS_Player.langString.Equals("晩囂"))
            {
                m_ChatScript.CallBack("じゃ、仝" + musicNameJP + "々を梧いましょう");
                Debug.Log(musicNameJP);
                殴慧肝方 += 1;
            }
        }
        else
        {
            if (m_VITS_Player.langString.Equals("嶄猟"))
            {
                m_ChatScript.CallBack("俊和栖厘壅葎低蟹匯遍ゞ" + musicNameCN + "〃杏");
                Debug.Log(musicNameCN);
            }
            else if (m_VITS_Player.langString.Equals("晩囂"))
            {
                m_ChatScript.CallBack("じゃ、�Aけて仝" + musicNameJP + "々を梧いましょう");
                Debug.Log(musicNameJP);
            }
        }
        m_Msg_Validate.IsOutputTextMsg = true;
        m_Msg_Validate.IsPlayingMusic = true;
    }

    //亢墮峺協彿坿
    public void UnloadAsset(UnityEngine.Object obj)
    {
        Resources.UnloadAsset(obj);
    }
}
