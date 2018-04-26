using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour {
    private Slider m_Silder;
    void Awake()
    {
        m_Silder = GetComponent<Slider>();
        print(m_Silder);
        m_Silder.value = 0;
    }
    public void ChangeTargetSlider(float MaxNums,float CurrentNums)
    {
        m_Silder.maxValue = MaxNums;
        m_Silder.value = CurrentNums;
    }
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
