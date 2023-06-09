using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using static VITS_Speech;
using System;

public class VITS_Speech : MonoBehaviour
{
    /// <summary>
    /// VITS��api��ַ
    /// </summary>
    [SerializeField] private string m_VitsApiPath;
    /// <summary>
    /// ����
    /// </summary>
    public SendDataSetting m_Setting;
    /// <summary>
    /// ��Ƶ���
    /// </summary>
    public AudioSource m_AudioSource;
    //��Ϣ��֤
    [SerializeField] private Msg_Validate m_Msg_Validate;

    //_chatScript.langString
    public string langString = "";
    public string SetChangeLan(string lang)
    {

        //Debug.Log(langString);
        return this.langString = lang;
    }
    public string getLan()
    {
        //Debug.Log(langString);
        return langString;
    }


    public void Speek(string _msg)
    {
        try
        {
            List<object> dataList = new List<object>();
            dataList.Add(_msg);
            dataList.Add(langString);
            //dataList.Add(m_Setting.lan.ToString());
            dataList.Add(m_Setting.speeker.ToString());
            dataList.Add(m_Setting.noise_scale);
            dataList.Add(m_Setting.noise_scale_w);
            dataList.Add(m_Setting.length_scale);

            Dictionary<string, List<object>> dataDict = new Dictionary<string, List<object>>();
            dataDict.Add("data", dataList);

            string jsonData = JsonConvert.SerializeObject(dataDict);

            StartCoroutine(GetSpeech(jsonData));
            //���ûظ���Ϣ��Ƶ���ںϳ�
            m_Msg_Validate.IsOutputWAVEncoding = true;
            Debug.Log(langString);

        }
        catch (Exception)
        {
            Debug.Log(langString + "m_ChatScript.langString is Null");
        }
        
    }

    /// <summary>
    /// VITS�ϳɣ������Ƶ�ļ�
    /// </summary>
    /// <param name="_sendData"></param>
    /// <returns></returns>
    public IEnumerator GetSpeech(string _sendData)
    {
        using (UnityWebRequest request = new UnityWebRequest(m_VitsApiPath, "POST"))
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_sendData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                string _msg = request.downloadHandler.text;
                JObject jsonData = JObject.Parse(_msg);
                Debug.Log(_msg);
                //��ȡ����Ƶ�ĵ�ַ
                string name = (string)jsonData["data"][1]["name"];
                StartCoroutine(LoadAudioClip(name));

            }
            else
            {
                //���ûظ���Ϣ��Ƶ���ںϳɣ��ϳ�ʧ��ʱ��
                m_AudioSource.Stop();
                m_Msg_Validate.IsOutputWAVEncoding = false;
                Debug.Log("�����ϳ�ʧ��");
            }
        }
    }
    /// <summary>
    /// ��ȡ��Ƶ������
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
   private IEnumerator LoadAudioClip(string filePath)
    {
        string url = "file://" + filePath; // ���URL
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV); // ��������
        yield return www.SendWebRequest(); // �ȴ�������Ӧ

        if (www.result == UnityWebRequest.Result.Success)
        {
            // ��ȡ��Ƶ�ļ�
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            m_AudioSource.clip = clip;
            m_AudioSource.Play(); // ������Ƶ�ļ�
            //���ûظ���Ϣ��Ƶ���ںϳɣ����ڲ��źϳ���Ƶ��
            m_Msg_Validate.IsOutputWAVEncoding = false;
        }
        else
        {
            Debug.LogError("Load audio clip failed. " + www.error);
        }
    }

    #region ���ݶ���
    /// <summary>
    /// �ϳ�����
    /// </summary>
    [System.Serializable]public class SendDataSetting
    {
        /// <summary>
        /// ����
        /// </summary>
        [Header("����")]public Lan lan= Lan.����;
        /// <summary>
        /// ����
        /// </summary>
        [Header("����")] public string speeker= "����";
        /// <summary>
        /// ���Ƹ���仯�̶�
        /// </summary>
        [Header("���Ƹ���仯�̶�")] public float noise_scale;
        /// <summary>
        /// �������ط�������
        /// </summary>
        [Header("�������ط�������")] public float noise_scale_w;
        /// <summary>
        /// ������������
        /// </summary>
        [Header("������������")]public float length_scale;
    }

    public enum Lan
    {
        ����,
        ����
    }


    #endregion


}