using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{

    //主角
    private PlayerRobotContral playerRobotContral;
    //血条
    private float m_CurrentHP, m_CurrentMP, m_CurrentMAXHP, m_CurrentMAXMP;
    private float MAXHPMPPercent;
    //核心
    private BaseCore.CoreElement m_CurrentCoreElement;
    //武器
    private GunType m_CurrentLeftGunType, m_CurrentRightGunType;
    [Header("7把武器图片")]
    public Sprite[] WeaponGuns;
    [Header("主副武器位置")]
    public Image LeftGunPos, RightGunPos;
    [Header("核心图片")]
    public Sprite[] Cores;
    [Header("核心位置")]
    public Image Corepos;
    [Header("血条")]
    public Slider[] MainSlider;
    public Slider HPSlider, MPSlider;
    [Header("零件数量")]
    public Text PartNums;


    void Start ()
    {
        StartCoroutine(GetInfo());
    }

    IEnumerator GetInfo()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            yield return null;
            playerRobotContral = GameManager.Instance.PRC;

            m_CurrentHP = playerRobotContral._mPlayerRobot.CurrentHp;
            m_CurrentMP = playerRobotContral._mPlayerRobot.CurrentMp;
            m_CurrentMAXHP = playerRobotContral._mPlayerRobot.MaxHp;
            m_CurrentMAXMP = playerRobotContral._mPlayerRobot.MaxMp;
            MAXHPMPPercent = m_CurrentMAXHP / (m_CurrentMAXHP + m_CurrentMAXMP) * 1.0f;
            m_CurrentCoreElement = playerRobotContral._mPlayerRobot.Core.Element;
            m_CurrentLeftGunType = WeaponManager.Instance.CurrentLeftGunType;
            m_CurrentRightGunType = WeaponManager.Instance.RightGunType;



            //赋值
            LeftGunPos.sprite = WeaponGuns[(int)m_CurrentLeftGunType];
            if (m_CurrentRightGunType != GunType.Null)
            { RightGunPos.sprite = WeaponGuns[(int)m_CurrentRightGunType]; }
            else
            {
                RightGunPos.sprite = WeaponGuns[7];
            }


            CoreAttribute currentCoreAttribute = CoreAttribute.Initial;
            switch (m_CurrentCoreElement)
            {
                case BaseCore.CoreElement.Primary:
                    currentCoreAttribute = CoreAttribute.Initial;
                    break;
                case BaseCore.CoreElement.Fire:
                    currentCoreAttribute = CoreAttribute.Fire;
                    break;
                case BaseCore.CoreElement.Amethyst:
                    currentCoreAttribute = CoreAttribute.Amethyst;
                    break;
                case BaseCore.CoreElement.Ice:
                    currentCoreAttribute = CoreAttribute.Frozen;
                    break;
            }
            Corepos.sprite = Cores[(int)currentCoreAttribute];

            MainSlider[0].maxValue = MainSlider[1].maxValue = m_CurrentMAXMP + m_CurrentMAXHP;
            MainSlider[0].value = m_CurrentMAXHP;
            MainSlider[1].value = m_CurrentMAXMP;

            HPSlider.maxValue = m_CurrentMAXHP;
            MPSlider.maxValue = m_CurrentMAXMP;
            HPSlider.value = m_CurrentHP;
            MPSlider.value = m_CurrentMP;

            PartNums.text = WeaponManager.Instance.PartNums.ToString();
        }
    }

}
