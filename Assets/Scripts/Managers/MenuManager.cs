using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{//Based on Youtube tutorials from @Tarodev

    /// <summary>
    /// This Singleton class manages the popup displays in the gamescreen.
    /// </summary>
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedPlayerObject, _tileObject, _tileUnitObject, _infoPopup;
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Displays info text in the info Popup. This is used for feedback to the player.
    /// </summary>
    /// <param name="info">The info to display in the popup.</param>
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

    /// <summary>
    /// Displays info about a given tile in the tile info popup. <br />
    /// Also displays info (name) about the unit that is currently occupying the given tile.
    /// </summary>
    /// <param name="tile">The tile to pull the info from.</param>
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

    /// <summary>
    /// Displays info (name) about the robot that is currently selected into the selected player popup.
    /// </summary>
    /// <param name="robot">The robot that is selected to pull info from</param>
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
