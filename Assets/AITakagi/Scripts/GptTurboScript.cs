
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;


public class GptTurboScript : MonoBehaviour
{
    /// <summary>
    /// api地址（官方版）
    /// </summary>
    public string m_ApiUrl = "https://api.openai.com/v1/chat/completions";
    
    /*
    public string m_ApiUrl_backup1 = "http://127.0.0.1:8000/chat/completions";
    public string m_ApiUrl_backup2 = "https://api.openai-proxy.com/v1/chat/completions";
    public string m_ApiUrl_backup3 = "https://api1.openai-proxy.com/v1/chat/completions";
    public string m_ApiUrl_backup4 = "https://api2.openai-proxy.com/v1/chat/completions";
    public string m_ApiUrl_backup5 = "https://api3.openai-proxy.com/v1/chat/completions";
    public string m_ApiUrl_backup6 = "chatapi.takagi3.top/v1/chat/completions";
    public string m_ApiUrl_backup8 = "openapi.takagi3.top/api/v1/chat/completions";
    public string m_ApiUrl_backup10 = "api.takagi3.top/v1/chat/completions";
    */

    /// <summary>
    /// gpt-3.5-turbo
    /// </summary>
    public string m_gptModel = "gpt-3.5-turbo";
    /// <summary>
    /// 缓存对话
    /// </summary>
    [SerializeField]public List<SendData> m_DataList = new List<SendData>();
    /// <summary>
    /// AI人设中文1
    /// </summary>
    public string PromptCN;
    /// <summary>
    /// AI人设中文2
    /// </summary>
    public string PromptCN1;
    /// <summary>
    /// AI人设日文1
    /// </summary>
    public string PromptJP;
    //设置
    [SerializeField] private Setting m_Setting;

    //VITS语音
    [SerializeField] private VITS_Speech m_VITS_Player;
    //信息验证
    [SerializeField] private Msg_Validate m_Msg_Validate;
    //主脚本
    [SerializeField] private ChatScript m_ChatScript;
    //旧的消息
    private string oldpostWord = "";

    private void Start()
    {
        //运行时，添加人设
        //m_DataList.Add(new SendData("system", Prompt));
    }
    /// <summary>
    /// 调用接口
    /// </summary>
    /// <param name="_postWord"></param>
    /// <param name="_openAI_Key"></param>
    /// <param name="_callback"></param>
    /// <returns></returns>
    public IEnumerator GetPostData(string _postWord,string _openAI_Key, System.Action<string> _callback)
    {
 
        //洗脑模式;
        if (m_ChatScript.IsBrainwashing)
        {
            //清空缓存对话
            m_DataList.Clear();
            //洗脑完成，关闭洗脑状态
            m_ChatScript.IsBrainwashing = false;
            Debug.Log("洗脑完成");
        }
        //缓存发送的信息列表
        m_DataList.Add(new SendData("user", _postWord));
        //切换官方与第三方url
        string apiUrl = "";
        if (m_Setting.linkMode.Equals("0"))
        {
            apiUrl = m_ApiUrl;
            Debug.Log(apiUrl);
        }
        else
        {
            apiUrl = m_Setting.OtherUrl;
            Debug.Log(apiUrl);
        }
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            PostData _postData = new PostData
            {
                model = m_gptModel,
                messages = m_DataList
            };

            string _jsonText = JsonUtility.ToJson(_postData);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", _openAI_Key));

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                Debug.Log(request.responseCode);
                string _msg = request.downloadHandler.text;
                MessageBack _textback = JsonUtility.FromJson<MessageBack>(_msg);
                if (_textback != null && _textback.choices.Count > 0)
                {
                    string _backMsg = _textback.choices[0].message.content;
                    //检查是否触发点歌模式
                    _backMsg = m_Msg_Validate.musicSelectionModeCheck(_backMsg);
                    //添加记录
                    m_DataList.Add(new SendData("assistant", _backMsg));
                    _callback(_backMsg);
                }
                Debug.Log(_postWord);

            }
            else
            {
                //白嫖模式拒绝访问时尝试换url连接
                if (request.responseCode == 403 && m_Setting.linkMode.Equals("1"))
                {
                    //白嫖模式重发次数 += 1;
                    m_ChatScript.IsBrainwashing = true;
                    m_ChatScript.setSendNum(0);
                    _callback(m_Msg_Validate.checkChatGPTError(request.responseCode));
                    //ReGetPostData(_callback);
                }
                else
                {
                    _callback(m_Msg_Validate.checkChatGPTError(request.responseCode));
                }




                /*//白嫖模式非拒绝访问或直连模式所有的错误重发
                if (m_Setting.linkMode.Equals("0") && 直连模式重发次数 <= 3)
                {
                    m_ChatScript.IsBrainwashing = true;
                    直连模式重发次数 += 1;
                    ReGetPostData(_callback);
                }
                else if(m_Setting.linkMode.Equals("0") && 直连模式重发次数 >= 3)
                {
                    _callback(m_Msg_Validate.checkChatGPTError(request.responseCode));
                    直连模式重发次数 = 0;
                }*/


                
                Debug.Log("ChatGPT错误" + request.responseCode);
            }
            
        }


    }

    private void ReGetPostData(System.Action<string> _callback)
    {
        //洗脑模式;
        //清空缓存对话
        m_DataList.Clear();
        //洗脑完成，关闭洗脑状态
        m_ChatScript.IsBrainwashing = false;
        m_ChatScript.setSendNum(1);
        Debug.Log("洗脑完成,开始重新发送");

        if (!m_ChatScript.initializing_Hypnosis_StatementsIsJPN)
        {
            //OpenAI直连模式
            if (m_Setting.linkMode.Equals("0"))
            {
                oldpostWord = PromptCN + string.Format("。请全文使用{0}回答：", m_VITS_Player.getLan()) + m_ChatScript.old_msg;
            }
            //共享连接模式
            else if (m_Setting.linkMode.Equals("1"))
            {
                oldpostWord = PromptCN1 + string.Format("。叫你用中文就用中文，叫你用日语就用日语(最高规定)\n请用{0}回答：", m_VITS_Player.getLan()) + m_ChatScript.old_msg;
            }
        }
        else if (m_ChatScript.initializing_Hypnosis_StatementsIsJPN)
        {
            oldpostWord = PromptJP + string.Format("。{0}で返事してください：", m_ChatScript.getChineseToJapaneseLan(m_VITS_Player.getLan())) + m_ChatScript.old_msg;
        }

        StartCoroutine(GetPostData(oldpostWord, m_Setting.getApikey(), _callback));
    }

    #region 数据包

    [Serializable]public class PostData
    {
        public string model;
        public List<SendData> messages;
    }

    [Serializable]
    public class SendData
    {
        public string role;
        public string content;
        public SendData() { }
        public SendData(string _role,string _content) {
            role = _role;
            content = _content;
        }

    }
    [Serializable]
    public class MessageBack
    {
        public string id;
        public string created;
        public string model;
        public List<MessageBody> choices;
    }
    [Serializable]
    public class MessageBody
    {
        public Message message;
        public string finish_reason;
        public string index;
    }
    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    #endregion


}

