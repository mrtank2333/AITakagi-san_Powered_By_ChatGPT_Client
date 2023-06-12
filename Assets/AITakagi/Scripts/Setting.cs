using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    //设置的Api
    [SerializeField] private InputField m_InputAPI;
    //设置自定义URL
    [SerializeField] private InputField m_InputOtherURL;
    //xx轮后自动洗脑
    [SerializeField] private InputField m_InputNumberOfConversations;
    //洗脑模式开关
    [SerializeField] private GameObject m_BrainwashingButtonONIsChecked;
    [SerializeField] private GameObject m_BrainwashingButtonONIsNoChecked;
    [SerializeField] private GameObject m_BrainwashingButtonOFFIsChecked;
    [SerializeField] private GameObject m_BrainwashingButtonOFFIsNoChecked;
    //保存文件按钮
    [SerializeField] private GameObject m_SaveButton;
    private string apiKey;
    private string apiKey1 = "填写白嫖模式的key";

    /// <summary>
    /// api地址（共享版）
    /// </summary>
    public string OtherUrl = "";

    //当xx轮对话时候自动洗脑
    public int AutoBrainwashingNum = 3;
    //自动洗脑模式是否开启
    public int AutoBrainwashing = 1;

    private string configFile = "";

    public string linkMode = "0";


    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            configFile = Application.persistentDataPath + "/Config.txt";
            if (!File.Exists(configFile))
            {
                WWW loadWWW = new WWW(configFile);
                while (!loadWWW.isDone)
                {

                }
                File.WriteAllBytes(configFile, loadWWW.bytes);
                File.WriteAllText(configFile,
                    "APIKey=" + "" +
                    "\nAutoBrainwashing=" + "1" +
                    "\nAutoBrainwashingNum=" + "3" +
                    "\nOtherAPIUrl=" + "https://api.openai-proxy.com/v1/chat/completions");
            }
            GetConfig();
        }
        else
        {
            configFile = Application.dataPath + "\\config.txt";
#if !UNITY_EDITOR
        configFile = System.Environment.CurrentDirectory + "/config.txt";
#endif

            GetConfig();
        }

        //string m = MachineCode.GetMachineCodeString();
        //Debug.Log(m);
        
    }

    void Update()
    {

    }
    private void GetConfig()
    {

        if (File.Exists(configFile))
        {
            string[] strs = File.ReadAllLines(configFile);
            if (strs.Length < 1)
                return;
            for (int i = 0; i < strs.Length; i++)
            {
                strs[i] = strs[i].Replace(" ", "");
            }
            try
            {
                apiKey = strs[0].Replace("APIKey=", "");
                if (apiKey.Length == 51)
                {
                    m_InputAPI.text = "已输入APIKey，点击此框可更换新的APIKey";
                }
                else
                {
                    m_InputAPI.text = "你的APIKey可能不正确，请检查你的APIKey或尝试使用白嫖模式";
                }
                
                AutoBrainwashing = int.Parse(strs[1].Replace("AutoBrainwashing=", ""));
                //设置开关催眠的按钮状态
                setAutoBrainwashingSW();
                AutoBrainwashingNum = getStringToInt(strs[2].Replace("AutoBrainwashingNum=", ""));
                m_InputNumberOfConversations.text = getIntToString(AutoBrainwashingNum);
                OtherUrl = strs[3].Replace("OtherAPIUrl=", "");
                m_InputOtherURL.text = OtherUrl;
                //Debug.Log(apiKey);
            }
            catch (System.Exception)
            {
                Debug.Log("读取配置文件出错");
                SaveConfigFile();
            }

        }
    }
    //设置直连模式的APIKey
    public void setApikey()
    {
        if (!m_InputAPI.text.Equals("已输入APIKey，点击此框可更换新的APIKey"))
        {
            if (m_InputAPI.text.Length == 51)
            {
                apiKey = m_InputAPI.text;
                //SaveConfigFile();
                m_SaveButton.SetActive(true);
                Debug.Log("Apikey设置成功！！！");
            }
            else
            {
                m_InputAPI.text = "你的APIKey可能不正确，请检查你的APIKey或尝试使用白嫖模式";
                Debug.Log("Apikey不能为空！！！");
            }
        }
    }
    //设置连接模式为直连
    public void setLinkModeIsOpenAI()
    {
        linkMode = "0";
    }
    //设置连接模式为白嫖
    public void setLinkModeIsPublic()
    {
        linkMode = "1";
    }

    //设置开启自动催眠
    public void setAutoBrainwashingIsOn()
    {
        AutoBrainwashing = 1;
        m_SaveButton.SetActive(true);
        //SaveConfigFile();
    }
    //设置关闭自动催眠
    public void setAutoBrainwashingIsOff()
    {
        AutoBrainwashing = 0;
        m_SaveButton.SetActive(true);
        //SaveConfigFile();
    }
    //设置开关催眠的按钮状态
    private void setAutoBrainwashingSW()
    {
        if (AutoBrainwashing == 1)
        {
            m_BrainwashingButtonONIsChecked.SetActive(true);
            m_BrainwashingButtonONIsNoChecked.SetActive(false);
            m_BrainwashingButtonOFFIsChecked.SetActive(false);
            m_BrainwashingButtonOFFIsNoChecked.SetActive(true);
        }
        else
        {
            m_BrainwashingButtonONIsChecked.SetActive(false);
            m_BrainwashingButtonONIsNoChecked.SetActive(true);
            m_BrainwashingButtonOFFIsChecked.SetActive(true);
            m_BrainwashingButtonOFFIsNoChecked.SetActive(false);
        }
    }
    //设置xx轮后自动催眠
    public void setAutoBrainwashingNum()
    {
        try
        {
            AutoBrainwashingNum = int.Parse(m_InputNumberOfConversations.text);
            //SaveConfigFile();
            m_SaveButton.SetActive(true);
            Debug.Log("设置轮数成功！当前设置的轮数为："+ AutoBrainwashingNum);
        }
        catch
        {
            AutoBrainwashingNum = 3;
            m_InputNumberOfConversations.text = "3";
            //SaveConfigFile();
            m_SaveButton.SetActive(true);
            Debug.Log("输入轮数无法转换成整数，自动设置为3轮");
        }
        
    }

    //设置自定义链接
    public void setOhterUrl()
    {
        if (!m_InputOtherURL.text.Equals(""))
        {
            OtherUrl = m_InputOtherURL.text;
            m_SaveButton.SetActive(true);
            //SaveConfigFile();
            Debug.Log("Api链接设置成功！！！");
        }
        else
        {
            Debug.Log("Api链接不能为空！！！");
        }


    }
    
    public string getApikey()
    {
        if (linkMode.Equals("0"))
        {
            Debug.Log("key是直连模式API");
            return apiKey;
        }
        else
        {

            Debug.Log("key是白嫖模式API");
            return apiKey1;
        }
    }
    private int getStringToInt(string num)
    {
        try
        {
            int a = int.Parse(num);
            return a;
        }
        catch
        {
            return 0;
        }
    }

    private string getIntToString(int num)
    {
        try
        {
            string a = num.ToString();
            return a;
        }
        catch
        {
            return "";
        }
    }
    //保存config文件
    public void SaveConfigFile()
    {
        File.WriteAllText(configFile, 
            "APIKey=" + apiKey + 
            "\nAutoBrainwashing=" + AutoBrainwashing + 
            "\nAutoBrainwashingNum=" + m_InputNumberOfConversations.text + 
            "\nOtherAPIUrl=" + m_InputOtherURL.text);
        GetConfig();
    }

}
