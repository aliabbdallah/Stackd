using UnityEngine;
using UnityEngine.UI;
using System;

public class MainMenuView : MonoBehaviour
{
    public Button summerButton;
    public Button winterButton;
    public Button autumnButton;
    public Button mainButton;
    public Button loginButton;

    public event Action OnSummerClicked;
    public event Action OnWinterClicked;
    public event Action OnAutumnClicked;
    public event Action OnMainClicked;
    public event Action OnLoginClicked;

    void Start()
    {
        if (summerButton != null) summerButton.onClick.AddListener(() => OnSummerClicked?.Invoke());
        if (winterButton != null) winterButton.onClick.AddListener(() => OnWinterClicked?.Invoke());
        if (autumnButton != null) autumnButton.onClick.AddListener(() => OnAutumnClicked?.Invoke());
        if (mainButton != null) mainButton.onClick.AddListener(() => OnMainClicked?.Invoke());
        if (loginButton != null) loginButton.onClick.AddListener(() => OnLoginClicked?.Invoke());
    }
} 