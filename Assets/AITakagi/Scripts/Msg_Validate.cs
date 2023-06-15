using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Msg_Validate : MonoBehaviour
{
    //VITS语音
    [SerializeField] private VITS_Speech m_VITS_Player;
    //音乐播放器
    [SerializeField] private MusicPlayer m_MusicPlayer;
    //音乐播放器
    [SerializeField] private ChatScript m_ChatScript;
    //返回的信息
    [SerializeField] private Text m_TextBack;
    //服务
    [SerializeField] private Service m_Service;

    public string inputMsgs;
    public string outputMsgs;
    /// <summary>
    ///正在输出文字信息音频
    /// </summary>
    public bool IsPlayingMsg = false;
    /// <summary>
    ///正在逐字输出聊天框文字信息
    /// </summary>
    public bool IsOutputTextMsg = false;
    /// <summary>
    ///回复信息音频正在合成
    /// </summary>
    public bool IsOutputWAVEncoding = false;
    /// <summary>
    ///正在播放音乐
    /// </summary>
    public bool IsPlayingMusic = false;
    /*    /// <summary>
        ///正在播放指定音乐
        /// </summary>
        public bool IsSelectPlaying = false;*/
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// ChatGPT网络问题回复信息设置
    /// return 错误信息
    /// </summary>
    public string checkChatGPTError(long responseCode)
    {
        string lang = m_VITS_Player.langString;
        string _callback = "";
        if (responseCode == 401)
        {
            Debug.Log(responseCode);
            if (lang.Equals("中文"))
            {
                _callback = "你的Api可能有问题呢" + "    返回代码:" + responseCode;
            }
            else if (lang.Equals("日语"))
            {
                _callback = "あなたのAPIが問題がありそうな感じだよね" + "    戻すコード:" + responseCode;
            }
        }
        else if (responseCode == 400)
        {
            Debug.Log(responseCode);
            if (lang.Equals("中文"))
            {
                _callback = "你问的太多了，我头有点晕，请按右上角的清理图标清空会话再试试" + "    返回代码:" + responseCode;
            }
            else if (lang.Equals("日语"))
            {
                _callback = "質問が多すぎて、ちょっと頭が痛い。右上のクリーンアイコンを押して会话をクリアしていい？" + "    戻すコード:" + responseCode;
            }
        }
        else if (responseCode == 404)
        {
            Debug.Log(responseCode);
            if (lang.Equals("中文"))
            {
                _callback = "你的网络可能没法访问ChatGPT呢" + "    返回代码:" + responseCode;
            }
            else if (lang.Equals("日语"))
            {
                _callback = "あなたのネット環境がチャットGPTに接続できなかった" + "    戻すコード:" + responseCode;
            }

        }
        else if (responseCode == 500)
        {
            Debug.Log(responseCode);
            if (lang.Equals("中文"))
            {
                _callback = "ChatGPT那个笨蛋好像自己出问题了，我没法连接它。请等一会在找我吧" + "    返回代码:" + responseCode;
            }
            else if (lang.Equals("日语"))
            {
                _callback = "あのチャットGPTのバカめが問題が起こった！ごめんなさい、しばらくお待ちください" + "    戻すコード:" + responseCode;
            }
        }
        else if (responseCode == 403)
        {
            Debug.Log(responseCode);
            if (lang.Equals("中文"))
            {
                _callback = "我的访问被ChatGPT拒绝了" + "    返回代码:" + responseCode;
            }
            else if (lang.Equals("日语"))
            {
                _callback = "チャットGPTが私のアクセスを拒否しました。" + "    戻すコード:" + responseCode;
            }
        }
        else
        {
            Debug.Log(responseCode);
            if (lang.Equals("中文"))
            {
                _callback = "我遇到了我也解释不清楚的问题，请过段时间再来找我吧" + "    返回代码:" + responseCode;
            }
            else if (lang.Equals("日语"))
            {
                _callback = "この問題は私にもはじめて見てから、解決できないので、しばらくしてからまた来てね" + "    戻すコード:" + responseCode;
            }

        }
        return _callback;
    }

    /// <summary>
    /// 是否触发点歌模式检查
    /// return 回复信息
    /// </summary>
    public string musicSelectionModeCheck(string msg)
    {
        string lang = m_VITS_Player.langString;
        Debug.Log(msg);

        string returnMsg = "";
        if (msg.IndexOf("#进入点歌模式") != -1
            || msg.IndexOf("＃点歌モード") != -1
            || msg.IndexOf("#進入点歌") != -1
            || msg.IndexOf("＃進入点歌") != -1
            || msg.IndexOf("#歌モードに入る") != -1
            || msg.IndexOf("歌うことできませ") != -1
            || msg.IndexOf("歌うことができませ") != -1
            || msg.IndexOf("歌うことはできませ") != -1
            || msg.IndexOf("歌う能力はありませ") != -1
            || msg.IndexOf("歌う能力がありませ") != -1
            || msg.IndexOf("私は歌手ではありませ") != -1
            || msg.IndexOf("私は歌手じゃな") != -1
            || msg.IndexOf("歌う機能は持っていない") != -1
            || msg.IndexOf("歌う機能はありませ") != -1)
        {
            List<Tuple<string, string>> musicName = m_MusicPlayer.getRandomMusicName();
            //去除文件名开头的XX.
            string a = musicName[0].Item1.Substring(0, 3);
            string musicNameJP = musicName[0].Item1.Replace(a, "");
            string musicNameCN = musicName[0].Item2.Replace(a, "");
            m_Service.UnloadAsset(m_MusicPlayer.audioClip);
            m_MusicPlayer.audioClip = Resources.Load<AudioClip>("Music/" + musicName[0].Item1);
            if (lang.Equals("中文"))
            {
                returnMsg = "好的，那我为你唱一首《" + musicNameCN + "》吧";
            }
            else if (lang.Equals("日语"))
            {
                returnMsg = "じゃ、「" + musicNameJP + "」を歌いましょう";
            }
            IsOutputTextMsg = true;
            IsPlayingMusic = true;
            return returnMsg;
        }
        else
        {
            return msg;
        }
    }

    public void InputMsgValidate(string msg)
    {
        string lang = m_VITS_Player.langString;
        if (msg.IndexOf("#切换背景") != -1 || msg.IndexOf("#背景替え") != -1 || msg.IndexOf("＃背景替え") != -1)
        {
            try
            {
                float f = float.Parse(msg.Replace(msg.Substring(0, 5), ""));
                m_ChatScript.setBackGround(f);
                msg = "";
            }
            catch
            {
                msg = "";
            }
        }
        if (msg.IndexOf("#切换立绘") != -1 || msg.IndexOf("#立ち絵替") != -1 || msg.IndexOf("＃立ち絵替") != -1)
        {
            try
            {
                float f = float.Parse(msg.Replace(msg.Substring(0, 5), ""));
                m_ChatScript.setTakagiIllustration(f);
                msg = "";
            }
            catch
            {
                msg = "";
            }
        }
        if (msg.IndexOf("#播放音乐") != -1 || msg.IndexOf("#音楽再生") != -1 || msg.IndexOf("＃音楽再生") != -1)
        {
            try
            {
                m_ChatScript.m_ChatHistory.Add(msg);
                int i = int.Parse(msg.Replace(msg.Substring(0, 5), ""));
                List<Tuple<string, string>> musicName = m_MusicPlayer.getMusicName(i);
                //去除文件名开头的XX.
                string a = musicName[0].Item1.Substring(0, 3);
                string musicNameJP = musicName[0].Item1.Replace(a, "");
                string musicNameCN = musicName[0].Item2.Replace(a, "");
                m_Service.UnloadAsset(m_MusicPlayer.audioClip);
                m_MusicPlayer.audioClip = Resources.Load<AudioClip>("Music/" + musicName[0].Item1);
                if (lang.Equals("中文"))
                {
                    m_ChatScript.CallBack("好的，那我为你唱一首《" + musicNameCN + "》吧");
                    Debug.Log(musicNameCN);
                }
                else if (lang.Equals("日语"))
                {
                    m_ChatScript.CallBack("じゃ、「" + musicNameJP + "」を歌いましょう");
                    Debug.Log(musicNameJP);
                }
                IsOutputTextMsg = true;
                IsPlayingMusic = true;
                msg = "";
            }
            catch
            {
                msg = "";
            }
        }
        if (msg.IndexOf("#帮助") != -1 || msg.IndexOf("#h") != -1 || msg.IndexOf("＃h") != -1)
        {
            try
            {
                m_ChatScript.m_ChatHistory.Add(msg);
                if (lang.Equals("中文"))
                {
                    m_ChatScript.CallBack("输入：“#切换背景0.XX”以用于切换背景图片\n输入：“#切换立绘0.XX”以用于切换背景图片\n输入：“#播放音乐XX”以用于播放指定音乐");
                }
                else if (lang.Equals("日语"))
                {
                    m_ChatScript.CallBack("背景画像の切り替えは「#背景替え0.XX」。\nキャラクター立ち絵の切り替えは「#立ち絵替0.XX」。\n 指定され音楽の再生は「#音楽再生XX」。");
                }
                msg = "";
            }
            catch
            {
                msg = "";
            }
        }
        inputMsgs = msg;
    }

    //输入：string
    //要匹配的源数组string[]
    //string[] can = { "你能", "为我" };
    //string[] canCmd = FindSimilarStrings(msg, can);
    //查找相似的字符串
    public static string[] FindSimilarStrings(string input, string[] strings)
    {
        int threshold = 2; // 定义相似性的阈值，即Levenshtein距离的最大容忍次数

        // 存储相似字符串的列表
        List<string> similarStrings = new List<string>();

        foreach (string str in strings)
        {
            int distance = ComputeLevenshteinDistance(input, str);
            if (distance <= threshold)
            {
                similarStrings.Add(str);
            }
        }

        return similarStrings.ToArray();
    }

    public static int ComputeLevenshteinDistance(string str1, string str2)
    {
        int[,] dp = new int[str1.Length + 1, str2.Length + 1];

        for (int i = 0; i <= str1.Length; i++)
        {
            dp[i, 0] = i;
        }

        for (int j = 0; j <= str2.Length; j++)
        {
            dp[0, j] = j;
        }

        for (int i = 1; i <= str1.Length; i++)
        {
            for (int j = 1; j <= str2.Length; j++)
            {
                int cost = (str1[i - 1] == str2[j - 1]) ? 0 : 1;
                dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
            }
        }

        return dp[str1.Length, str2.Length];
    }
    //获取处理后的输入信息
    public string getInputMsgs()
    {
        return inputMsgs;
    }
    //获取处理后的输出信息
    public string getOututMsgs()
    {
        return outputMsgs;
    }
}
