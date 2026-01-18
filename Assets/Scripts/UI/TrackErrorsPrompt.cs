using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrackErrorsPrompt : MonoBehaviour
{
    [SerializeField] InitializationManager Manager;
    [SerializeField] AudioSource ButtonSFX;

    public float OnScreenPosY;
    public float OffScreenPosY;
    public Vector3 OffscreenScale;
    public Vector3 OnscreenScale;
    public float AnimDuration;
    public RectTransform Window;

    // Start is called before the first frame update
    void Start()
    {
        SetOffscreenState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetOnscreenState()
    {
        Window.localScale = OnscreenScale;
        Window.localPosition = new Vector3(Window.localPosition.x, OnScreenPosY);
    }

    private void SetOffscreenState()
    {
        Window.localScale = OffscreenScale;
        Window.localPosition = new Vector3(Window.localPosition.x, OffScreenPosY);
    }

    public void AnimateOnscreen()
    {
        Window.DOKill();
        SetOffscreenState();
        
        Sequence seq = DOTween.Sequence();
        seq.Append(Window.DOScale(OnscreenScale, AnimDuration).SetEase(Ease.OutBack));
        seq.Join(Window.DOLocalMoveY(OnScreenPosY, AnimDuration).SetEase(Ease.OutQuart));
        seq.Play();
    }

    public void AnimateOffscreen()
    {
        Window.DOKill();
        SetOnscreenState();

        Sequence seq = DOTween.Sequence();
        seq.Append(Window.DOScale(OnscreenScale, AnimDuration).SetEase(Ease.InBack));
        seq.Join(Window.DOLocalMoveY(OnScreenPosY, AnimDuration).SetEase(Ease.InQuart));
        seq.Play();
    }

    public void OnYesButtonClick()
    {
        if (ButtonSFX != null) { ButtonSFX.Play(); }
        SentryUtils.EnableSentry();
        PlayerPrefs.SetInt(Constants.ErrorTrackingConsent, 1);
        AnimateOffscreen();
        Manager.ContinueWithInitialization();
    }

    public void OnNoButtonClick()
    {
        if (ButtonSFX != null) { ButtonSFX.Play(); }
        SentryUtils.DisableSentry();
        PlayerPrefs.SetInt(Constants.ErrorTrackingConsent, 0);
        AnimateOffscreen();
        Manager.ContinueWithInitialization();
    }
}
