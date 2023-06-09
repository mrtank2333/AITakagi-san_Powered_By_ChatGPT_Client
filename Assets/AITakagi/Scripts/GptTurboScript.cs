
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GptTurboScript : MonoBehaviour
{
    /// <summary>
    /// api仇峽�┨抃衆罍�
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
    /// 産贋斤三
    /// </summary>
    [SerializeField] public List<SendData> m_DataList = new List<SendData>();
    /// <summary>
    /// AI繁譜嶄猟1
    /// </summary>
    public string PromptCN;
    /// <summary>
    /// AI繁譜嶄猟2
    /// </summary>
    public string PromptCN1;
    /// <summary>
    /// AI繁譜晩猟1
    /// </summary>
    public string PromptJP;
    //譜崔
    [SerializeField] private Setting m_Setting;

    //VITS囂咄
    [SerializeField] private VITS_Speech m_VITS_Player;
    //佚連刮屬
    [SerializeField] private Msg_Validate m_Msg_Validate;
    //麼重云
    [SerializeField] private ChatScript m_ChatScript;
    //症議��連
    private string oldpostWord = "";

    private void Start()
    {
        //塰佩扮��耶紗繁譜
        //m_DataList.Add(new SendData("system", Prompt));
    }
    /// <summary>
    /// 距喘俊笥
    /// </summary>
    /// <param name="_postWord"></param>
    /// <param name="_openAI_Key"></param>
    /// <param name="_callback"></param>
    /// <returns></returns>
    public IEnumerator GetPostData(string _postWord, string _openAI_Key, System.Action<string> _callback)
    {

        //牢辻庁塀;
        if (m_ChatScript.IsBrainwashing)
        {
            //賠腎産贋斤三
            m_DataList.Clear();
            //牢辻頼撹��購液牢辻彜蓑
            m_ChatScript.IsBrainwashing = false;
            Debug.Log("牢辻頼撹");
        }
        //産贋窟僕議佚連双燕
        m_DataList.Add(new SendData("user", _postWord));
        //俳算郊圭嚥及眉圭url
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
                    //殊臥頁倦乾窟泣梧庁塀
                    _backMsg = m_Msg_Validate.musicSelectionModeCheck(_backMsg);
                    //耶紗芝村
                    m_DataList.Add(new SendData("assistant", _backMsg));
                    _callback(_backMsg);
                }
                Debug.Log(_postWord);

            }
            else
            {
                //易耄庁塀詳蒸恵諒扮晦編算url銭俊
                if (request.responseCode == 403 && m_Setting.linkMode.Equals("1"))
                {
                    //易耄庁塀嶷窟肝方 += 1;
                    m_ChatScript.IsBrainwashing = true;
                    m_ChatScript.setSendNum(0);
                    _callback(m_Msg_Validate.checkChatGPTError(request.responseCode));
                    //ReGetPostData(_callback);
                }
                else
                {
                    _callback(m_Msg_Validate.checkChatGPTError(request.responseCode));
                }

                /*//易耄庁塀掲詳蒸恵諒賜岷銭庁塀侭嗤議危列嶷窟
                if (m_Setting.linkMode.Equals("0") && 岷銭庁塀嶷窟肝方 <= 3)
                {
                    m_ChatScript.IsBrainwashing = true;
                    岷銭庁塀嶷窟肝方 += 1;
                    ReGetPostData(_callback);
                }
                else if(m_Setting.linkMode.Equals("0") && 岷銭庁塀嶷窟肝方 >= 3)
                {
                    _callback(m_Msg_Validate.checkChatGPTError(request.responseCode));
                    岷銭庁塀嶷窟肝方 = 0;
                }*/

                Debug.Log("ChatGPT危列" + request.responseCode);
            }

        }


    }

    //囑欺危列扮昨晦編壅窟僕佚連
    private void ReGetPostData(System.Action<string> _callback)
    {
        //牢辻庁塀;
        //賠腎産贋斤三
        m_DataList.Clear();
        //牢辻頼撹��購液牢辻彜蓑
        m_ChatScript.IsBrainwashing = false;
        m_ChatScript.setSendNum(1);
        Debug.Log("牢辻頼撹,蝕兵嶷仟窟僕");

        if (!m_ChatScript.initializing_Hypnosis_StatementsIsJPN)
        {
            //OpenAI岷銭庁塀
            if (m_Setting.linkMode.Equals("0"))
            {
                oldpostWord = PromptCN + string.Format("。萩畠猟聞喘{0}指基��", m_VITS_Player.getLan()) + m_ChatScript.old_msg;
            }
            //慌�軈�俊庁塀
            else if (m_Setting.linkMode.Equals("1"))
            {
                oldpostWord = PromptCN1 + string.Format("。出低喘嶄猟祥喘嶄猟��出低喘晩囂祥喘晩囂(恷互号協)\n萩喘{0}指基��", m_VITS_Player.getLan()) + m_ChatScript.old_msg;
            }
        }
        else if (m_ChatScript.initializing_Hypnosis_StatementsIsJPN)
        {
            oldpostWord = PromptJP + string.Format("。{0}で卦並してください��", m_ChatScript.getChineseToJapaneseLan(m_VITS_Player.getLan())) + m_ChatScript.old_msg;
        }

        StartCoroutine(GetPostData(oldpostWord, m_Setting.getApikey(), _callback));
    }

    #region 方象淫

    [Serializable]
    public class PostData
    {
        //潮範頁��text-davinci-003
        public string model;
        public List<SendData> messages;
    }

    [Serializable]
    public class SendData
    {
        public string role;
        public string content;
        public SendData() { }
        public SendData(string _role, string _content)
        {
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

