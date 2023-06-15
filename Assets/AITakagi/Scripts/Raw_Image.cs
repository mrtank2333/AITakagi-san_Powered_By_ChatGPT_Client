using UnityEngine;
using UnityEngine.UI;


public class Raw_Image : MonoBehaviour
{
    //转场总持续时间
    private float fadeTime = 1f;

    //聊天UI层
    //[SerializeField] private FadeImage m_FadeImage;
    private RawImage m_RawImage;
    //开始倒计时
    public bool timestart = false;
    //结束倒计时
    public bool timeend = false;
    //渐隐正在执行
    public bool FadeIn = false;
    //渐显正在执行
    public bool FadeOut = false;

    public string Sts = "0";

    public float times1 = 0f;

    void Start()
    {
        //获取Rawimage实例
        m_RawImage = GetComponent<RawImage>();
        //将图片大小设置为屏幕大小
        m_RawImage.uvRect = new Rect(0, 0, Screen.width, Screen.height);
        m_RawImage.CrossFadeAlpha(0f, 0f, false);
    }

    void Update()
    {

        //计时器完成倒计时的功能
        if (timestart)
        {

            times1 -= Time.deltaTime;
            if (times1 <= 0f)
            {
                if (Sts == "1")
                //if (timestart && !timeend)
                {
                    //判定倒计时结束时该做什么
                    FadeToBlack(fadeTime);
                    Sts = "2";

                    Debug.Log("timestart OK");
                    Debug.Log(Sts);
                }
            }
        }

    }

    //屏幕渐隐效果方法     
    public void FadeToClear(float fadeSpeeds)
    {
        //rawImage.color = Color.Lerp(rawImage.color, Color.clear, fadeSpeed * Time.deltaTime);
        m_RawImage.CrossFadeAlpha(1f, fadeSpeeds / 2, false);
        fadeTime = fadeSpeeds;
        times1 = (fadeSpeeds / 2) + 0.1f;
        Sts = "1";
        timestart = true;
        Debug.Log("FadeToClear OK");
        Debug.Log(Sts);
    }
    //屏幕渐显效果方法     
    public void FadeToBlack(float fadeSpeeds)
    {


        m_RawImage.CrossFadeAlpha(0f, fadeSpeeds / 2, false);

        //times1 = (fadeSpeeds/2) + 0.2f;
        //Sts = "3";
        Debug.Log("FadeToBlack OK");
        Debug.Log(Sts);

        timestart = false;
    }
    /*
        public IEnumerator ChangeTime()
        {
            while (time > 0)
            {
                yield return new WaitForSeconds(1);// 每次 自减1，等待 1 秒
                time--;
                //GetComponent<Text>().text = "倒计时:" + time;
                Debug.Log("倒计时:" + time);
            }
            FadeToBlack();

        }*/

}