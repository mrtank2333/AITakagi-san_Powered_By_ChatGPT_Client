using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    /// <summary>
    /// 音频组件
    /// </summary>
    public AudioSource m_AudioSource;
    //信息验证
    [SerializeField] private Msg_Validate m_Msg_Validate;
    //渐变
    [SerializeField] private Raw_Image m_Raw_Image;

    [SerializeField] private ChatScript m_ChatScript;
    //停止播放按钮
    [SerializeField] private GameObject m_musicStop;
    //背景板
    [SerializeField] private Animator m_Quad;
    //聊天UI层
    [SerializeField] private GameObject m_PostGUI;
    //聊天发送层
    [SerializeField] private GameObject m_SendText;
    //返回的信息
    [SerializeField] private Text m_TextBack;
    //VITS
    [SerializeField] private VITS_Speech m_VITS_Player;
    //服务
    [SerializeField] private Service m_Service;

    /// <summary>
    /// musicName：音乐名称
    /// </summary>
    public string musicName;
    /// <summary>
    /// <br>Sts：播放进程</br>
    /// <br>0：初始化</br>
    /// <br>1：开始播放前的转场</br>
    /// <br>2：转场完成并开始播放</br>
    /// <br>3：播放结束并开始转场回初始场景</br>
    /// </summary>
    public string Sts = "0";

    //
    string lang = "";

    public AudioClip audioClip;
    //当随机播放播放到第一首后面的歌时候的随机
    public List<Tuple<string, string>> musicName1;

    /// <summary>
    /// <br>音乐元组</br>
    /// <br>int, string, string, float, float</br>
    /// <br>曲目番号</br>
    /// <br>日文曲名</br>
    /// <br>中文曲名（信息播报用）</br>
    /// <br>播放时的背景板番号</br>
    /// <br>播放时的人物立绘番号</br>
    /// </summary>
    public List<Tuple<int, string, string, float, float>> MusicAll = new List<Tuple<int, string, string, float, float>>();
    //string path = "";
    // Start is called before the first frame update
    void Start()
    {
        //初始化时候隐藏停止播放按钮
        m_musicStop.SetActive(false);
        m_Quad.SetFloat("BackGround", 0.04f);
        //全部导入
        //第一季ED合集
        MusicAll.Add(new Tuple<int, string, string, float, float>(11, "11.気まぐれロマンティック", "11.心血来潮的浪漫", 0.01f, 0.011f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(12, "12.AM11：00", "12.上午11点", 0.02f, 0.021f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(13, "13.自転車", "13.自行车", 0.03f, 0.31f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(14, "14.風吹けば恋", "14.起风之恋", 0.04f, 0.041f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(15, "15.小さな恋のうた", "15.小小恋歌", 0.05f, 0.081f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(16, "16.愛唄", "16.爱歌", 0.06f, 0.061f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(17, "17.出逢った頃のように", "17.仿若相逢时", 0.07f, 0.071f));

        //第二季ED合集
        MusicAll.Add(new Tuple<int, string, string, float, float>(21, "21.奏(かなで)", "21.奏", 0.08f, 0.081f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(22, "22.粉雪", "22.粉雪", 0.09f, 0.051f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(23, "23.キセキ", "23.奇迹", 0.10f, 0.011f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(24, "24.ありがとう", "24.谢谢", 0.11f, 0.021f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(25, "25.STARS", "25.STARS", 0.01f, 0.031f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(26, "26.あなたに", "26.献给你", 0.02f, 0.041f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(27, "27.言わないけどね。", "27.虽然不会说出口。", 0.03f, 0.051f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(28, "28.やさしい気持ち", "28.温柔的心情", 0.04f, 0.061f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(29, "29.100万回の「I love you」", "29.100万次说“爱你”", 0.05f, 0.071f));

        //第三季ED合集
        MusicAll.Add(new Tuple<int, string, string, float, float>(31, "31.夢で逢えたら", "31.若在梦中相逢", 0.06f, 0.031f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(32, "32.Over Drive", "32.Over Drive", 0.07f, 0.081f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(33, "33.ひまわりの約束", "33.向日葵的约定", 0.08f, 0.091f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(34, "34.学園天国", "34.学园天国", 0.10f, 0.021f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(35, "35.じょいふる", "35.快乐", 0.09f, 0.091f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(36, "36.サンタが町にやってくる", "36.圣诞老人进城来", 0.11f, 0.051f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(37, "37.スノーマジックファンタジー", "37.雪之魔法幻想", 0.01f, 0.041f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(38, "38.花", "38.花", 0.02f, 0.081f));

        //剧场版ED合集
        MusicAll.Add(new Tuple<int, string, string, float, float>(41, "41.明日への扉", "41.通向明天之门", 0.03f, 0.011f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(42, "42.天体観測", "42.天体观测", 0.04f, 0.021f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(43, "43.fragile", "43.脆弱", 0.05f, 0.031f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(44, "44.君に届け", "44.好想告诉你", 0.06f, 0.041f));

        //手游（心动记录）
        MusicAll.Add(new Tuple<int, string, string, float, float>(51, "51.First Love", "51.初恋", 0.07f, 0.051f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(52, "52.LOVEマシーン", "52.恋爱机器", 0.08f, 0.061f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(53, "53.Majiでkoiする5秒前", "53.认真恋爱的5秒前", 0.09f, 0.071f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(54, "54.大阪Lover", "54.大阪情人", 0.10f, 0.081f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(55, "55.君と光", "55.你和光", 0.11f, 0.091f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(56, "56.青いベンチ", "56.蓝色长椅", 0.01f, 0.011f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(57, "57.若者のすべて", "57.青春无悔", 0.02f, 0.021f));
        MusicAll.Add(new Tuple<int, string, string, float, float>(58, "58.クリスマス·イブ", "58.平安夜", 0.11f, 0.031f));

        //AI翻唱


        //Debug.Log(path);

    }

    // Update is called once per frame
    void Update()
    {
        lang = m_VITS_Player.langString;
        //检测是否开始播放随机音乐
        if (!m_ChatScript.IsRandomPlaying)
        {
            if (Sts == "0" && !m_AudioSource.isPlaying && !m_Msg_Validate.IsOutputTextMsg && !m_Msg_Validate.IsOutputWAVEncoding && m_Msg_Validate.IsPlayingMusic)
            {
                Sts = "1";
                m_Raw_Image.FadeToClear(2f);
                Debug.Log("Sts=" + Sts);
            }
            else if (Sts == "1" && m_Raw_Image.Sts.Equals("2"))
            {
                Sts = "2";

                //显示停止播放按钮
                m_musicStop.SetActive(true);
                //根据曲目不同切换背景板
                Tuple<int, string, string, float, float> result = MusicAll.Find(x => x.Item2 == musicName);
                m_Quad.SetFloat("BackGround", result.Item4);
                //根据曲目不同切换立绘
                m_ChatScript.setTakagiIllustration(result.Item5);
                //隐藏聊天窗口
                m_PostGUI.SetActive(false);

                MusicPlay();
                Debug.Log("Sts=" + Sts);
            }

            if (Sts == "2" && !m_AudioSource.isPlaying)
            {
                Sts = "3";
                m_Raw_Image.FadeToClear(2f);
                Debug.Log("Sts=" + Sts);

            }
            else if (Sts == "3" && m_Raw_Image.Sts.Equals("2"))
            {
                Sts = "0";
                //隐藏停止播放按钮
                m_musicStop.SetActive(false);
                //显示聊天窗口
                m_PostGUI.SetActive(true);
                m_SendText.SetActive(true);
                //还原背景板
                m_Quad.SetFloat("BackGround", 0.04f);
                //m_ChatScript.DefaultTakagi();
                //还原立绘
                m_ChatScript.setTakagiIllustration(0f);
                if (lang.Equals("中文"))
                {
                    m_VITS_Player.Speek("播放结束...输入新的信息来继续对话吧");
                    m_TextBack.text = "播放结束...输入新的信息来继续对话吧～";

                }
                else if (lang.Equals("日语"))
                {
                    m_VITS_Player.Speek("再生完了...メッセージボックスに入力して会話を続けよう");
                    m_TextBack.text = "再生完了...メッセージボックスに入力して会話を続けよう～";
                }

                Debug.Log("Sts=" + Sts);
            }
        }

        if (m_ChatScript.IsRandomPlaying)
        {
            //检测是否开始播放随机音乐
            if (Sts == "0" && !m_AudioSource.isPlaying && !m_Msg_Validate.IsOutputTextMsg && !m_Msg_Validate.IsOutputWAVEncoding && m_Msg_Validate.IsPlayingMusic)
            {
                Sts = "1";
                m_Raw_Image.FadeToClear(2f);
                Debug.Log("Sts=" + Sts);
            }
            else if (Sts == "1" && m_Raw_Image.Sts.Equals("2"))
            {
                Sts = "2";

                //m_ChatScript.MusicTakagi();
                //显示停止播放按钮
                m_musicStop.SetActive(true);
                //根据曲目不同切换背景板
                Tuple<int, string, string, float, float> result = MusicAll.Find(x => x.Item2 == musicName);
                m_Quad.SetFloat("BackGround", result.Item4);
                //根据曲目不同切换立绘
                m_ChatScript.setTakagiIllustration(result.Item5);
                //隐藏聊天窗口
                m_PostGUI.SetActive(false);

                MusicPlay();
                Debug.Log("Sts=" + Sts);
            }

            if (Sts == "2" && !m_AudioSource.isPlaying)
            {
                /*//当随机播放播放到第一首后面的歌时候的随机
                musicName1 = getRandomMusicName();*/
                Sts = "3";
                m_Raw_Image.FadeToClear(2f);
                Debug.Log("Sts=" + Sts);

            }
            else if (Sts == "3" && m_Raw_Image.Sts.Equals("2") && m_ChatScript.IsRandomPlaying && m_Service.播放次数 != 1)
            {
                Sts = "2";
                //显示停止播放按钮
                m_musicStop.SetActive(true);
                //根据曲目不同切换背景板
                Tuple<int, string, string, float, float> result = MusicAll.Find(x => x.Item2 == musicName);
                m_Quad.SetFloat("BackGround", result.Item4);
                //根据曲目不同切换立绘
                m_ChatScript.setTakagiIllustration(result.Item5);
                //隐藏聊天窗口
                m_PostGUI.SetActive(false);

                MusicPlay();

                Debug.Log("Sts=" + Sts);
            }
            else if (Sts == "3" && m_Raw_Image.Sts.Equals("2") && m_ChatScript.IsRandomPlaying && m_Service.播放次数 == 1)
            {

                m_Service.StartRandomPlay();
                Sts = "0";

                Debug.Log("Sts=" + Sts);
            }
            else if (Sts == "3" && m_Raw_Image.Sts.Equals("2") && !m_ChatScript.IsRandomPlaying)
            {
                Sts = "0";
                //隐藏停止播放按钮
                m_musicStop.SetActive(false);
                //显示聊天窗口
                m_PostGUI.SetActive(true);
                m_SendText.SetActive(true);
                //还原背景板
                m_Quad.SetFloat("BackGround", 0.04f);
                //m_ChatScript.DefaultTakagi();
                //还原立绘
                m_ChatScript.setTakagiIllustration(0f);
                if (lang.Equals("中文"))
                {
                    m_VITS_Player.Speek("播放结束...输入新的信息来继续对话吧");
                    m_TextBack.text = "播放结束...输入新的信息来继续对话吧～";

                }
                else if (lang.Equals("日语"))
                {
                    m_VITS_Player.Speek("再生完了...メッセージボックスに入力して会話を続けよう");
                    m_TextBack.text = "再生完了...メッセージボックスに入力して会話を続けよう～";
                }
                Debug.Log("Sts=" + Sts);
            }
        }
    }


    /// <summary>
    /// <br>获取指定歌名</br>
    /// <br>int num：序号</br>
    /// <br>return 歌名</br>
    /// </summary>
    public List<Tuple<string, string>> getMusicName(int num)
    {
        List<Tuple<string, string>> MusicName = new List<Tuple<string, string>>();
        try
        {
            Tuple<int, string, string, float, float> result = MusicAll.Find(x => x.Item1 == num);
            //音乐名字
            string musicNameJP = result.Item2;
            string musicNameCN = result.Item3;
            m_Service.UnloadAsset(audioClip);
            audioClip = Resources.Load<AudioClip>("Music/" + musicNameJP);
            MusicName.Add(new Tuple<string, string>(musicNameJP, musicNameCN));
            return MusicName;
        }
        catch
        {
            string musicNameJP = MusicAll[4].Item2;
            string musicNameCN = MusicAll[4].Item2;
            m_Service.UnloadAsset(audioClip);
            audioClip = Resources.Load<AudioClip>("Music/" + musicNameJP);
            MusicName.Add(new Tuple<string, string>(musicNameJP, musicNameCN));
            return MusicName;
        }
    }

    /// <summary>
    /// 获取随机播放的歌曲名
    /// return 日语歌名，中文歌名
    /// </summary>
    public List<Tuple<string, string>> getRandomMusicName()
    {
        List<Tuple<string, string>> MusicName = new List<Tuple<string, string>>();
        try
        {
            System.Random random = new System.Random();
            int rad = MusicAll.Count - 1;
            rad = random.Next(0, rad);
            int rad1 = 0;
            if (rad < 0)
            {
                rad1 = 0;
            }
            else
            {
                rad1 = rad;
            }
            Debug.Log(rad1);
            //音乐名字
            string musicNameJP = MusicAll[rad1].Item2;
            string musicNameCN = MusicAll[rad1].Item3;
            musicName = musicNameJP;
            m_Service.UnloadAsset(audioClip);
            audioClip = Resources.Load<AudioClip>("Music/" + musicNameJP);
            MusicName.Add(new Tuple<string, string>(musicNameJP, musicNameCN));
            return MusicName;
        }
        catch (ArgumentOutOfRangeException)
        {
            string musicNameJP = MusicAll[4].Item2;
            string musicNameCN = MusicAll[4].Item3;
            musicName = musicNameJP;
            m_Service.UnloadAsset(audioClip);
            audioClip = Resources.Load<AudioClip>("Music/" + musicNameJP);
            MusicName.Add(new Tuple<string, string>(musicNameJP, musicNameCN));
            return MusicName;
        }
    }

    /// <summary>
    /// 通过歌曲名指定歌曲播放
    /// parameter：音乐名称
    /// </summary>
    public void MusicPlay()
    {
        //Resources.LoadAssetAtPath();
        //AudioClip audioClip = Resources.Load<AudioClip>("Music/" + musicName);
        m_AudioSource.clip = audioClip;

        m_Msg_Validate.IsPlayingMusic = true;

        m_AudioSource.Play(); // 播放音频文件
        m_Msg_Validate.IsPlayingMusic = false;
        Debug.Log(audioClip);
        string a = "";
        if (m_Msg_Validate.IsOutputTextMsg)
        {
            a += "正在逐字输出聊天框文字信息,";
        }
        else
        {
            a += "不在逐字输出聊天框文字信息,";
        }

        if (m_Msg_Validate.IsOutputWAVEncoding)
        {
            a += "回复信息音频正在合成,";
        }
        else
        {
            a += "回复信息音频合成结束,";
        }
        if (m_Msg_Validate.IsPlayingMusic)
        {
            a += "正在播放音乐,";
        }
        else
        {
            a += "音乐播放已结束,";
        }

        Debug.Log(a);

        //设置随机播放音乐状态结束
        /*
        path += "/Music/" + getMusicName() + ".mp3";
        //使用www类加载播放
        StartCoroutine(LoadMusicClip(path));
        */
    }
    /// <summary>
    /// 停止播放音乐
    /// </summary>
    public void StopMusic()
    {
        m_AudioSource.Stop();
        m_Service.UnloadAsset(audioClip);
        //退出随机播放模式
        m_ChatScript.IsRandomPlaying = false;
        m_Service.播放次数 = 0;
    }


    /*
        /// <summary>
        /// 读取外文件夹音频并播放
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private IEnumerator LoadMusicClip(string filePath)
        {
            string url = "file://" + filePath; // 组成URL
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG); // 发送请求
            yield return www.SendWebRequest(); // 等待请求响应

            if (www.result == UnityWebRequest.Result.Success)
            {
                // 获取音频文件
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                m_AudioSource.clip = clip;
                m_AudioSource.Play(); // 播放音频文件
            }
            else
            {
                Debug.Log(filePath);
                //Debug.LogError("Load audio clip failed. " + www.error);
            }
        }
        */



}
