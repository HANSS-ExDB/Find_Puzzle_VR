using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InventoryEntry
{
    public string Type;       // ex: "L_shaped_LeftTop"
    public Material Mat;      // 인벤토리에 저장할 머티리얼 참조

    public InventoryEntry(string type, Material mat)
    {
        Type = type;
        Mat = mat;
    }
}

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int MAX_ITEM = 8;

    public static InventoryManager Instance { get; private set; }

    // 타입(Type)과 머티리얼(Mat)을 함께 저장
    public List<InventoryEntry> items = new List<InventoryEntry>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// 타입과 머티리얼을 받아 인벤토리에 추가합니다.
    /// </summary>
    public bool AddItem(string type, Material mat)
    {
        if (items.Count >= MAX_ITEM)
        {
            Debug.LogWarning("[인벤토리] 가득 찼습니다!");
            return false;
        }

        items.Add(new InventoryEntry(type, mat));
        Debug.Log($"[인벤토리] 추가됨: {type}");
        return true;
    }

    /// <summary>
    /// 인벤토리에서 해당 인덱스의 항목을 제거합니다.
    /// </summary>
    public void RemoveAt(int index)
    {
        if (index >= 0 && index < items.Count)
            items.RemoveAt(index);
    }

    /// <summary>
    /// 현재 인벤토리에 담긴 개수입니다.
    /// </summary>
    public int Count => items.Count;
}
