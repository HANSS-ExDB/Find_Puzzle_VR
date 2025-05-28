using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class CountdownXR : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timerText;  // 타이머 텍스트
    public GameObject xrRigRoot;

    private ContinuousMoveProviderBase moveProvider;
    private ContinuousTurnProviderBase turnProvider;

    private float timer = 0f;
    private bool isTimerRunning = false;

    void Start()
    {
        moveProvider = xrRigRoot.GetComponentInChildren<ContinuousMoveProviderBase>();
        turnProvider = xrRigRoot.GetComponentInChildren<ContinuousTurnProviderBase>();

        if (moveProvider != null) moveProvider.enabled = false;
        if (turnProvider != null) turnProvider.enabled = false;

        timerText.gameObject.SetActive(false);  // 초기에는 숨기기
        timerText.text = "00:00";  // 초기화
        StartCoroutine(CountdownRoutine());
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    IEnumerator CountdownRoutine()
    {
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        if (moveProvider != null) moveProvider.enabled = true;
        if (turnProvider != null) turnProvider.enabled = true;

        timerText.gameObject.SetActive(true);  // 카운트다운 끝나면 타이머 보이게!

        isTimerRunning = true;

        Debug.Log("XR 조작 가능 상태! 타이머 시작!");
    }
}
