using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;

    [ReadOnly] public bool isOpen = false;

    [Header("References")]
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject rootButtons;

    [Space]
    [SerializeField] private Button leftButton;
    [SerializeField] private TextMeshProUGUI leftBtnText;

    [Space]
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI rightBtnText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
    }

    private void Awake()
    {
        Instance = this;

        DisableRoot();
    }

    /// <summary>
    /// Opens the PopUp only with text, needs to be closed manally!
    /// </summary>
    /// <param name="_text"></param>
    public void OpenPopUp(string _text)
    {
        OpenPopUp(_text, false, "", null, false, "", null);
    }

    /// <summary>
    /// Opens the PopUp only with text, uses duration to close
    /// </summary>
    /// <param name="_text">Title</param>
    /// <param name="duration">In seconds</param>
    public void OpenPopUp(string _text, float duration)
    {
        OpenPopUp(_text, false, "", null, false, "", null);
        CloseAfterTime(duration).Forget();
    }

    /// <summary>
    /// Opens the PopUp with text and 1 button
    /// </summary>
    /// <param name="_text">Title</param>
    /// <param name="_leftText">Button Text</param>
    /// <param name="_leftAction">Button Action</param>
    public void OpenPopUp(string _text, string _leftText, Action _leftAction = null)
    {
        OpenPopUp(_text, true, _leftText, _leftAction, false, "", null);
    }

    /// <summary>
    /// Opens the PopUp with text and 2 buttons, the left button is default, the right button is the passed action
    /// </summary>
    /// <param name="_text">Title</param>
    /// <param name="_leftText">Left Button Text ("" means Cancel)</param>
    /// <param name="_rightText">Right Button Text</param>
    /// <param name="_rightAction">Right Button Action</param>
    public void OpenPopUp(string _text, string _leftText, string _rightText, Action _rightAction = null)
    {
        OpenPopUp(_text, true, _leftText, null, true, _rightText, _rightAction);
    }

    /// <summary>
    /// Opens the PopUp with text and 2 buttons, and 2 actions
    /// </summary>
    /// <param name="_text">Title</param>
    /// <param name="_leftText">Left Button Text ("" means Cancel)</param>
    /// <param name="_leftAction">Left Button Action</param>
    /// <param name="_rightText">Right Button Tex ("" means Accept)t</param>
    /// <param name="_rightAction">Right Button Action</param>
    public void OpenPopUp(string _text, string _leftText, Action _leftAction, string _rightText, Action _rightAction)
    {
        OpenPopUp(_text, true, _leftText, _leftAction, true, _rightText, _rightAction);
    }

    private void OpenPopUp(string _text, bool _leftButton, string _leftText, Action _leftAction, bool _rightButton, string _rightText, Action _rightAction)
    {
        isOpen = true;

        text.text = _text;

        if (_leftButton)
        {
            leftButton.gameObject.SetActive(true);

            leftBtnText.text = _leftText == "" ? "Cancel" : _leftText;

            leftButton.onClick.AddListener(() =>
            {
                _leftAction?.Invoke();
                ClosePopUp();
            });
        }

        if (_rightButton)
        {
            rightButton.gameObject.SetActive(true);

            rightBtnText.text = _rightText == "" ? "Accept" : _rightText;

            rightButton.onClick.AddListener(() =>
            {
                _rightAction?.Invoke();
                ClosePopUp();
            });
        }

        if (!_leftButton && !_rightButton)
        {
            rootButtons.SetActive(false);
        }
        else
        {
            rootButtons.SetActive(true);
        }

        root.transform.localScale = Vector3.zero;
        LeanTween.scale(root, Vector3.one, 0.3f)
            .setEaseOutBack();

        root.SetActive(true);
    }

    private async UniTaskVoid CloseAfterTime(float _duration)
    {
        await UniTask.WaitForSeconds(_duration);
        ClosePopUp();
    }

    public void ClosePopUp()
    {
        isOpen = false;

        LeanTween.scale(root, Vector3.zero, 0.2f)
            .setEaseInBack()
            .setOnComplete(DisableRoot);
    }

    private void DisableRoot()
    {
        root.SetActive(false);
        root.transform.localScale = Vector3.zero;

        leftButton.onClick.RemoveAllListeners();
        leftButton.gameObject.SetActive(false);

        rightButton.onClick.RemoveAllListeners();
        rightButton.gameObject.SetActive(false);
    }
}