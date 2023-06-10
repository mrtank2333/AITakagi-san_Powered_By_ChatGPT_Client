using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using File = System.IO.File;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using UnityEngine.UIElements;
using System;

public class ChatScript : MonoBehaviour
{
    //API key
    //[SerializeField] private string m_OpenAI_Key = "填写你的Key";
    // 定义Chat API的URL
    ///private string m_ApiUrl = "https://api.openai.com/v1/completions";
    //配置参数
    [SerializeField] private GetOpenAI.PostData m_PostDataSetting;
    //聊天UI层
    [SerializeField] private GameObject m_ChatPanel;
    //输入的信息
    [SerializeField] private InputField m_InputWord;
    //设置的Api
    [SerializeField] private InputField m_InputAPI;
    //返回的信息
    [SerializeField] private Text m_TextBack;
    //VITS语音
    [SerializeField] private VITS_Speech m_VITS_Player;
    //gpt-3.5-turbo
    [SerializeField] public GptTurboScript m_GptTurboScript;
    //设置
    [SerializeField] private Setting m_Setting;
    //信息验证
    [SerializeField] private Msg_Validate m_InputMsgValidate;
    //高木立绘动画控制器
    [SerializeField] private Animator m_TakagiIllustration;
    //背景板动画控制器
    [SerializeField] private Animator m_BackGround;
    //信息验证
    [SerializeField] private Msg_Validate m_Msg_Validate;
    //服务
    [SerializeField] private Service m_Service;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    void OnGUI()
    {
        if (Input.anyKeyDown)
        {
            Event e = Event.current;
            //按下回车发送信息
            if (e.keyCode == KeyCode.Return)
            {
                SendData();
                Debug.Log("按下的键值：" + e.keyCode.ToString());
            }
        }
    }



    //回复次数计数器
    private int sendNum = 0;
 
    //触发洗脑
    public bool IsBrainwashing = false;
    //洗脑前最后的消息
    public string LastSendMsg = "";
    /// <summary>
    /// <br>催眠语句选用的语言，默认false（中文）</br>
    /// <br>false是中文，true是日语</br>
    /// </summary>
    public bool initializing_Hypnosis_StatementsIsJPN;
    //前回催眠语句语言
    private bool oldInitializing_Hypnosis_Statements;

    //旧连接方式
    private string oldlinkMode = "";

    //触发随机播放
    public bool IsRandomPlaying = false;

    //前回发的消息
    public string old_msg = "";


    //发送信息
    public void SendData()
    {
        if (m_VITS_Player.getLan().Equals(""))
        {
            Debug.Log("m_VITS_Player.getLan() Is NullorBlank");
            m_VITS_Player.SetChangeLan("中文");
        }
        //输入信息验证
        m_InputMsgValidate.InputMsgValidate(m_InputWord.text);
        m_InputWord.text = m_InputMsgValidate.getInputMsgs();
        //输入信息为空时返回
        if (m_InputWord.text.Equals(""))
            return;


        old_msg = m_InputWord.text;
        //记录聊天
        m_ChatHistory.Add(m_InputWord.text);
        //发送的文字列
        string _msg = "";
        //前回连接模式
        if (oldInitializing_Hypnosis_Statements != initializing_Hypnosis_StatementsIsJPN || !oldlinkMode.Equals(m_Setting.linkMode))
        {
            setSendNum(0);
            IsBrainwashing = true;
            Debug.Log("当中途切换连接模式或跟换催眠语言的时候自动归零");
        }

        if (!initializing_Hypnosis_StatementsIsJPN)
        {
            oldInitializing_Hypnosis_Statements = false;
            //OpenAI直连模式
            if (m_Setting.linkMode.Equals("0"))
            {
                oldlinkMode = "0";
                if (getSendNum() == 0)
                {
                    _msg = m_GptTurboScript.PromptCN + string.Format("。请全文使用{0}回答：", m_VITS_Player.getLan()) + m_InputWord.text;
                    setSendNum(getSendNum() + 1);
                }
                else if (getSendNum() >= m_Setting.AutoBrainwashingNum && m_Setting.AutoBrainwashing == 1)
                {
                    _msg = m_GptTurboScript.PromptCN + string.Format("。请全文使用{0}回答：", m_VITS_Player.getLan()) + m_InputWord.text;
                    IsBrainwashing = true;
                    setSendNum(1);
                }
                else
                {
                    _msg = string.Format("。请全文使用{0}回答：", m_VITS_Player.getLan()) + m_InputWord.text;
                    setSendNum(getSendNum() + 1);
                }
            }
            //共享连接模式
            else if (m_Setting.linkMode.Equals("1"))
            {
                oldlinkMode = "1";
                if (getSendNum() == 0)
                {
                    _msg = m_GptTurboScript.PromptCN1 + string.Format("。叫你用中文就用中文，叫你用日语就用日语(最高规定)\n请用{0}回答：", m_VITS_Player.getLan()) + m_InputWord.text;
                    setSendNum(getSendNum() + 1);
                }
                else if (getSendNum() >= m_Setting.AutoBrainwashingNum && m_Setting.AutoBrainwashing == 1)
                {
                    _msg = m_GptTurboScript.PromptCN1 + string.Format("。叫你用中文就用中文，叫你用日语就用日语(最高规定)\n请用{0}回答：", m_VITS_Player.getLan()) + m_InputWord.text;
                    IsBrainwashing = true;
                    setSendNum(1);
                }
                else
                {
                    _msg = string.Format("。叫你用中文就用中文，叫你用日语就用日语(最高规定)\n请用{0}回答：", m_VITS_Player.getLan()) + m_InputWord.text;
                    setSendNum(getSendNum() + 1);
                }
            }
        }
        else if (initializing_Hypnosis_StatementsIsJPN)
        {
            oldInitializing_Hypnosis_Statements = true;
            //OpenAI直连模式
            if (m_Setting.linkMode.Equals("0"))
            {
                oldlinkMode = "0";
                if (getSendNum() == 0)
                {
                    _msg = m_GptTurboScript.PromptJP + string.Format("。{0}で返事してください：", getChineseToJapaneseLan(m_VITS_Player.getLan())) + m_InputWord.text;
                    setSendNum(getSendNum() + 1);
                }
                else if (getSendNum() >= m_Setting.AutoBrainwashingNum && m_Setting.AutoBrainwashing == 1)
                {
                    _msg = m_GptTurboScript.PromptJP + string.Format("。{0}で返事してください：", getChineseToJapaneseLan(m_VITS_Player.getLan())) + m_InputWord.text;
                    IsBrainwashing = true;
                    setSendNum(1);
                }
                else
                {
                    _msg = string.Format("。{0}で返事してください：", getChineseToJapaneseLan(m_VITS_Player.getLan())) + m_InputWord.text;
                    setSendNum(getSendNum() + 1);
                }
            }
            //共享连接模式
            else if (m_Setting.linkMode.Equals("1"))
            {
                oldlinkMode = "1";
                if (getSendNum() == 0)
                {
                    _msg = m_GptTurboScript.PromptJP + string.Format("。{0}で返事してください：", getChineseToJapaneseLan(m_VITS_Player.getLan())) + m_InputWord.text;
                    setSendNum(getSendNum() + 1);
                }
                else if (getSendNum() >= m_Setting.AutoBrainwashingNum && m_Setting.AutoBrainwashing == 1)
                {
                    _msg = m_GptTurboScript.PromptJP + string.Format("。{0}で返事してください：", getChineseToJapaneseLan(m_VITS_Player.getLan())) + m_InputWord.text;
                    IsBrainwashing = true;
                    setSendNum(1);
                }
                else
                {
                    _msg = string.Format("。{0}で返事してください：", getChineseToJapaneseLan(m_VITS_Player.getLan())) + m_InputWord.text;
                    setSendNum(getSendNum() + 1);
                }
            }
        }
        Debug.Log("发送次数："+getSendNum());
        StartCoroutine(m_GptTurboScript.GetPostData(_msg, m_Setting.getApikey(), CallBack));
        m_InputWord.text = "";
        m_TextBack.text = "...";


    }


    //AI回复的信息
    public void CallBack(string _callback)
    {
        _callback = _callback.Trim();
        m_TextBack.text = "";
        //开始逐个显示返回的文本
        m_WriteState = true;
        StartCoroutine(SetTextPerWord(_callback));

        //记录聊天
        m_ChatHistory.Add(_callback);

        /*if (m_PlayToggle.isOn)
        {
            StartCoroutine(Speek(_callback));
        }*/
        StartCoroutine(Speek(_callback));

    }


    private IEnumerator Speek(string _msg)
    {
        yield return new WaitForEndOfFrame();
        //播放合成并播放音频
        m_VITS_Player.Speek(_msg);
    }

    #region 文字逐个显示
    //逐字显示的时间间隔
    [SerializeField] private float m_WordWaitTime = 0.2f;
    //是否显示完成
    [SerializeField] private bool m_WriteState = false;
    private IEnumerator SetTextPerWord(string _msg)
    {
        int currentPos = 0;
        while (m_WriteState)
        {
            yield return new WaitForSeconds(m_WordWaitTime);
            currentPos++;
            //更新显示的内容
            m_TextBack.text = _msg.Substring(0, currentPos);

            m_WriteState = currentPos < _msg.Length;

        }
        //逐字显示状态结束
        m_InputMsgValidate.IsOutputTextMsg = false;
    }

    #endregion


    #region 聊天记录
    //保存聊天记录
    [SerializeField] public List<string> m_ChatHistory;
    //缓存已创建的聊天气泡
    [SerializeField] private List<GameObject> m_TempChatBox;
    //聊天记录显示层
    [SerializeField] private GameObject m_HistoryPanel;
    //聊天文本放置的层
    [SerializeField] private RectTransform m_rootTrans;
    //发送聊天气泡
    [SerializeField] private ChatPrefab m_PostChatPrefab;
    //回复的聊天气泡
    [SerializeField] private ChatPrefab m_RobotChatPrefab;
    //滚动条
    [SerializeField] private ScrollRect m_ScroTectObject;
    //获取聊天记录
    public void OpenAndGetHistory()
    {
        m_ChatPanel.SetActive(false);
        m_HistoryPanel.SetActive(true);

        ClearChatBox();
        StartCoroutine(GetHistoryChatInfo());
    }
    
    //返回
    public void BackChatMode()
    {
        m_ChatPanel.SetActive(true);
        m_HistoryPanel.SetActive(false);
    }

    //清空已创建的对话框
    private void ClearChatBox()
    {
        while (m_TempChatBox.Count != 0)
        {
            if (m_TempChatBox[0])
            {
                Destroy(m_TempChatBox[0].gameObject);
                m_TempChatBox.RemoveAt(0);
            }
        }
        m_TempChatBox.Clear();
    }

    //获取聊天记录列表
    private IEnumerator GetHistoryChatInfo()
    {

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < m_ChatHistory.Count; i++)
        {
            if (i % 2 == 0)
            {
                ChatPrefab _sendChat = Instantiate(m_PostChatPrefab, m_rootTrans.transform);
                _sendChat.SetText(m_ChatHistory[i]);
                m_TempChatBox.Add(_sendChat.gameObject);
                continue;
            }

            ChatPrefab _reChat = Instantiate(m_RobotChatPrefab, m_rootTrans.transform);
            _reChat.SetText(m_ChatHistory[i]);
            m_TempChatBox.Add(_reChat.gameObject);
        }

        //重新计算容器尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
    }

    private IEnumerator TurnToLastLine()
    {
        yield return new WaitForEndOfFrame();
        //滚动到最近的消息
        m_ScroTectObject.verticalNormalizedPosition = 0;
    }


    #endregion

    /// <summary>
    /// <br>高木立绘设置</br>
    /// <br>Parameter:float</br>
    /// </summary>
    public void setTakagiIllustration(float numF)
    {
        m_TakagiIllustration.SetFloat("Cha_Illustration_SW", numF);
    }
    /// <summary>
    /// <br>背景板图片设置</br>
    /// <br>Parameter:float</br>
    /// </summary>
    public void setBackGround(float numF)
    {
        m_BackGround.SetFloat("BackGround", numF);
    }


    //获取回复次数
    public int getSendNum()
    {
        return sendNum;
    }
    //设置回复次数
    public void setSendNum(int a)
    {
        this.sendNum = a;
    }

    //回复语言设置为中文
    public void setChangeLanIsZH()
    {
        m_VITS_Player.SetChangeLan("中文");
        Debug.Log(m_VITS_Player.getLan());
    }
    //回复语言设置为日语
    public void setChangeLanIsJA()
    {
        m_VITS_Player.SetChangeLan("日语");
        Debug.Log(m_VITS_Player.getLan());
    }

    //
    /// <summary>
    /// <br>当催眠语句为日语时候，命令ChatGPT回复的语言文字列转换成日本語</br>
    /// <br>Parameter:中文、日语</br>
    /// <br>return:中国語、日本語</br>
    /// </summary>
    public string getChineseToJapaneseLan(string lan)
    {
        if (lan.Equals("中文"))
        {
            return "中国語";
        }
        else if (lan.Equals("日语"))
        {
            return "日本語";
        }
        else
        {
            return "中国語";
        }
    }
    /// <summary>
    /// <br>设置催眠语句选用的语言是日语</br>
    /// </summary>
    public void setInitializing_Hypnosis_StatementsIsJPN() {
        this.initializing_Hypnosis_StatementsIsJPN = true;
    }
    /// <summary>
    /// <br>设置催眠语句选用的语言是中文</br>
    /// </summary>
    public void setInitializing_Hypnosis_StatementsIsCN()
    {
        this.initializing_Hypnosis_StatementsIsJPN = false;
    }

    //清空会话
    public void Clear()
    {
        //清空缓存对话
        m_GptTurboScript.m_DataList.Clear();
        //洗脑完成，关闭洗脑状态
        IsBrainwashing = true;
        setSendNum(0);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //随机不间断播放音乐
    public void RandomPlay()
    {
        IsRandomPlaying = true;
        m_Service.StartRandomPlay();
    }

















    /*#region 切换妹子
    //Lo娘
    [SerializeField] private GameObject m_LoGirl;
    [SerializeField] private GameObject m_Girl;

    //
    public void SetLoGirlShowed(GameObject _settingPanel)
    {
        if (!m_LoGirl.activeSelf)
        {
            m_LoGirl.SetActive(true);
            m_Girl.SetActive(false);
        }
        //m_AzurePlayer.SetSound("zh-CN-XiaoyiNeural");

        _settingPanel.SetActive(false);
    }
    //zh-CN-XiaoxiaoNeural
    public void SetGirlShowed(GameObject _settingPanel)
    {
        if (!m_Girl.activeSelf)
        {
            m_LoGirl.SetActive(false);
            m_Girl.SetActive(true);
        }
        //m_AzurePlayer.SetSound("zh-CN-liaoning-XiaobeiNeural");

        _settingPanel.SetActive(false);
    }

    #endregion*/

}
