using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropdownOptions : MonoBehaviour
{
    public TMP_Dropdown _objects;
    public Sprite[] _flags;

    void Start()
    {
        _objects.ClearOptions();

        List<TMP_Dropdown.OptionData> flagList = new List<TMP_Dropdown.OptionData>();

        foreach (var flag in _flags)
        {
            string flagName = flag.name;
            var select = new TMP_Dropdown.OptionData(flagName, flag);
            flagList.Add(select);
        }

        _objects.AddOptions(flagList);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
