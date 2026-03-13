using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [Header("스킬 UI")] 
    // 쿨타임 표시할 회색 배경
    [SerializeField] private Image skillIconOverlay;
    [SerializeField] private TMP_Text skillText;

    private bool isCoolDown = false;

    public void UpdateSkillSlot(float currentCool, float maxCool)
    {
        // 0 이하로 내려가지 않게 
        float remainTime = Mathf.Max(0, maxCool - currentCool);
        
        // 회색 배경
        skillIconOverlay.fillAmount = remainTime / maxCool;

        // 쿨이 있다는 것 
        if (remainTime > 0)
        {
            isCoolDown = true;
            
            // 1초 이상 정수 1초 미민 소수점 한자리까지
            if (remainTime >= 1f)
            {   
                skillText.text = Mathf.CeilToInt(remainTime).ToString();   
            }
            else
            {
                skillText.text = remainTime.ToString("F1");
            }
        }
        else
        {
            if (isCoolDown)
            {
                OnCooldownEnd();
                isCoolDown = false;
            }
        }
    }

    private void OnCooldownEnd()
    {
        skillText.text = "";

        StartCoroutine(CoolDownDone(0.3f));
    }

    IEnumerator CoolDownDone(float time)
    {
        // cs가 원본 이미지에 붙어있음
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = transform.localScale * 1.2f;
        
        // 잠시 크기 키우기
        transform.localScale = targetScale;
        yield return new WaitForSeconds(time);
        transform.localScale = originalScale;
    }
}