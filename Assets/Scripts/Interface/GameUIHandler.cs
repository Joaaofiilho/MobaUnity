using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GameUIHandler : MonoBehaviour
{
    private Champion _playerChampion;

    [SerializeField] private TextMeshProUGUI goldText;

    private void Start()
    {
        _playerChampion = FindObjectOfType<Player>().champion;
        _playerChampion.OnGoldChangedCallback += OnGoldChangedCallback;
    }

    private void OnGoldChangedCallback(float gold)
    {
        goldText.text = gold.ToString(CultureInfo.CurrentCulture);
    }
}