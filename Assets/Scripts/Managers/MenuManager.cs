using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedPlayerObject, _tileObject, _tileUnitObject, _infoPopup;
    private void Awake()
    {
        Instance = this;
    }

    public void ShowInfoPopup(string info)
    {
        if (info == null)
        {
            _infoPopup.SetActive(false);
            return;
        }
        _infoPopup.GetComponentInChildren<TMP_Text>().text = info;
        _infoPopup.SetActive(true);
    }

    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }

        _tileObject.GetComponentInChildren<TMP_Text>().text = tile.TileName;
        _tileObject.SetActive(true);

        if (tile.OccupiedUnit)
        {
            _tileUnitObject.GetComponentInChildren<TMP_Text>().text = tile.OccupiedUnit.UnitName;
            _tileUnitObject.SetActive(true);
        }
        else _tileUnitObject.SetActive(false);
    }

    public void ShowSelectedPlayer(BaseRobot robot)
    {
        if (robot == null)
        {
            _selectedPlayerObject.SetActive(false);
            return;
        }
        _selectedPlayerObject.GetComponentInChildren<TMP_Text>().text = robot.UnitName;
        _selectedPlayerObject.SetActive(true);
    }
}
