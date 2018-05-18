using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target :  Singleton<Target> {
    private Slider m_Silder;
    [SerializeField] private Slider CDSlider;
    [SerializeField] private Slider SpecialCDSlider;

    protected override void Awake()
    {
        base.Awake();
        m_Silder = GetComponent<Slider>();
        m_Silder.value = 0;
        CDSlider.value = 0;
        SpecialCDSlider.value = 0;
    }

    /// <summary>
    /// 更新普通攻击CD条
    /// </summary>
    /// <param name="MaxCD">最大CD时间</param>
    /// <param name="CurrentCD">当前CD时间</param>
    public void ChangeCDSlider(float MaxCD, float CurrentCD)
    {
        CDSlider.maxValue = MaxCD;
        CDSlider.value = (MaxCD - CurrentCD);
    }
    /// <summary>
    /// 普通攻击CD条 设为0
    /// </summary>
    public void InitZeroCDSlider()
    {
        CDSlider.value = 0;
    }

    /// <summary>
    /// 更新特殊攻击CD条
    /// </summary>
    /// <param name="MaxCD">最大CD时间</param>
    /// <param name="CurrentCD">当前CD时间</param>
    public void ChangeSpecialCDSlider(float MaxCD, float CurrentCD)
    {
        SpecialCDSlider.maxValue = MaxCD;
        SpecialCDSlider.value = MaxCD - CurrentCD;
    }
    /// <summary>
    /// 普通攻击CD条 设为0
    /// </summary>
    public void InitZeroSpecialCDSlider()
    {
        SpecialCDSlider.value = 0;
    }

    /// <summary>
    /// 更新蓄能值
    /// </summary>
    /// <param name="MaxNums">最大蓄能值</param>
    /// <param name="CurrentNums">当前蓄能值</param>
    public void ChangeTargetSlider(float MaxNums,float CurrentNums)
    {
        m_Silder.maxValue = MaxNums;
        m_Silder.value = CurrentNums;
    }

    /// <summary>
    /// 初始化蓄能条【蓄能条慢慢减少】
    /// </summary>
    public void Init()
    {
        float tempRatio = m_Silder.value / m_Silder.maxValue * 1.0f;
        m_Silder.maxValue = 1;
        m_Silder.value = tempRatio;
        StartCoroutine(ValueToZero());
    }
    IEnumerator ValueToZero()
    {
        while (m_Silder.value > 0)
        {
            m_Silder.value -= Time.deltaTime * 2.0f;
            yield return null;
        }
    }
}
