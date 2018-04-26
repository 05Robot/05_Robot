using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    private Slider HPSlider;
    private Slider MPSlider;
	// Use this for initialization
	void Start () {
	    if (HPSlider==null)
	    {
	        HPSlider = transform.Find("Slider_HP").GetComponent<Slider>();
	    }
        if (MPSlider == null)
        {
            MPSlider = transform.Find("Slider_MP").GetComponent<Slider>();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 同步ui条，需要事先计算相关的百分比在传值
    /// </summary>
    /// <param name="hp_value"></param>
    /// <param name="mp_value"></param>
    public void SyncHPMp(float hp_value,float mp_value)
    {
        HPSlider.value = hp_value;
        MPSlider.value = mp_value;
    }

    public void TestDamage(int mpdamage)
    {
        GameManager.Instance.PRC.GetDamage(mpdamage,mpdamage-5);
    }
}
